using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace 游戏服务器.模板类
{
	internal sealed class 安全类型绑定器 : ISerializationBinder
	{
		private static readonly HashSet<string> 允许程序集 = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"游戏服务器", "Assembly-CSharp"
		};

		public Type BindToType(string assemblyName, string typeName)
		{
			string asmShort = (assemblyName ?? string.Empty).Split(',')[0].Trim();
			if (!string.IsNullOrEmpty(asmShort) && !允许程序集.Contains(asmShort))
			{
				throw new System.Runtime.Serialization.SerializationException(
					"拒绝反序列化未授权类型: " + typeName + " from " + assemblyName);
			}
			string fullName = string.IsNullOrEmpty(assemblyName) ? typeName : (typeName + ", " + assemblyName);
			Type resolved = Type.GetType(fullName);
			if (resolved == null)
			{
				throw new System.Runtime.Serialization.SerializationException(
					"无法解析反序列化类型: " + fullName);
			}
			return resolved;
		}

		public void BindToName(Type serializedType, out string assemblyName, out string typeName)
		{
			assemblyName = serializedType.Assembly.GetName().Name;
			typeName = serializedType.FullName;
		}
	}

	// 快速 Point 转换器: 直接解析 "x, y", 避开 System.Drawing.PointConverter 的文化相关慢路径.
	// 坐标点集模板(地图区域/怪物刷新, 合计百余 MB、数百万个点)反序列化的大头就在这里.
	internal sealed class 快速坐标转换器 : JsonConverter<Point>
	{
		public override Point ReadJson(JsonReader reader, Type objectType, Point existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return Point.Empty;
			}
			if (reader.TokenType == JsonToken.String)
			{
				string s = (string)reader.Value;
				int comma = s.IndexOf(',');
				if (comma > 0
					&& int.TryParse(s.AsSpan(0, comma).Trim(), out int x)
					&& int.TryParse(s.AsSpan(comma + 1).Trim(), out int y))
				{
					return new Point(x, y);
				}
				return Point.Empty;
			}
			if (reader.TokenType == JsonToken.StartObject)
			{
				JObject o = JObject.Load(reader);
				return new Point((int?)o["X"] ?? 0, (int?)o["Y"] ?? 0);
			}
			return Point.Empty;
		}

		public override void WriteJson(JsonWriter writer, Point value, JsonSerializer serializer)
		{
			writer.WriteValue(value.X + ", " + value.Y);
		}
	}

	public static class 序列化类
	{
		public static readonly JsonSerializerSettings 全局设置;

		private static readonly Dictionary<string, string> 定向字典;

		static 序列化类()
		{
			序列化类.全局设置 = new JsonSerializerSettings
			{
				DefaultValueHandling = DefaultValueHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore,
				TypeNameHandling = TypeNameHandling.Auto,
				SerializationBinder = new 安全类型绑定器(),
				Formatting = Formatting.Indented
			};
			序列化类.全局设置.Converters.Add(new 快速坐标转换器());
			序列化类.定向字典 = new Dictionary<string, string> { ["Assembly-CSharp"] = "游戏服务器" };
			Type[] types;
			types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (Type type in types)
			{
				if (type.IsSubclassOf(typeof(技能任务)))
				{
					序列化类.定向字典[type.Name] = type.FullName;
				}
			}
		}

		public static object[] 反序列化(string 文件夹, Type 类型)
		{
			ConcurrentQueue<object> concurrentQueue;
			concurrentQueue = new ConcurrentQueue<object>();
			if (Directory.Exists(文件夹))
			{
				FileInfo[] files;
				files = new DirectoryInfo(文件夹).GetFiles();
				// 每个文件独立: 读文本 -> 定向替换 -> 反序列化, 并行加载海量小文件以缩短启动时间.
				// 定向字典/全局设置/类型绑定器静态构造后只读, concurrentQueue 线程安全, 故并行无竞态.
				// 注: 结果顺序不再确定, 调用方按对象自身字段建索引, 不依赖顺序.
				// 按体积降序 + 负载均衡分区, 让 3.4MB 巨型文件先被领取, 避免默认分区把大文件压在一个核上拖成长尾.
				Array.Sort(files, (FileInfo a, FileInfo b) => b.Length.CompareTo(a.Length));
				Parallel.ForEach(Partitioner.Create(files, loadBalance: true), delegate (FileInfo file)
				{
					string text;
					text = File.ReadAllText(file.FullName);
					// 定向字典 只为修正 $type 里的程序集名(Assembly-CSharp)与短类型名(技能任务子类),
					// 这些只出现在含 $type 的文件(如技能节点). 不含 $type 的文件(如巨型坐标点集
					// 地图区域/怪物刷新, 单文件可达数 MB)无需替换, 跳过 18 轮整串 Replace 省掉大量分配.
					if (text.IndexOf("$type", StringComparison.Ordinal) >= 0
						|| text.IndexOf("Assembly-CSharp", StringComparison.Ordinal) >= 0)
					{
						foreach (KeyValuePair<string, string> item in 序列化类.定向字典)
						{
							text = text.Replace(item.Key, item.Value);
						}
					}
					try
					{
						object obj;
						obj = JsonConvert.DeserializeObject(text, 类型, 序列化类.全局设置);
						if (obj != null)
						{
							concurrentQueue.Enqueue(obj);
						}
					}
					catch (Exception ex)
					{
						主程.添加系统日志(file.FullName + " " + ex.Message);
					}
				});
			}
			return concurrentQueue.ToArray();
		}

		public static TItem[] 反序列化<TItem>(string folder) where TItem : class, new()
		{
			ConcurrentQueue<TItem> concurrentQueue;
			concurrentQueue = new ConcurrentQueue<TItem>();
			if (Directory.Exists(folder))
			{
				FileInfo[] files;
				files = new DirectoryInfo(folder).GetFiles();
				// 与非泛型重载一致: 并行加载, 用线程安全的 ConcurrentQueue 替代 List(后者并发 Add 会损坏).
				// 同时改为逐文件 try/catch+记日志, 单个损坏文件不再中断整批加载. 结果顺序不再确定.
				// 按体积降序 + 负载均衡分区, 同非泛型重载.
				Array.Sort(files, (FileInfo a, FileInfo b) => b.Length.CompareTo(a.Length));
				Parallel.ForEach(Partitioner.Create(files, loadBalance: true), delegate (FileInfo file)
				{
					string text;
					text = File.ReadAllText(file.FullName);
					// 同非泛型重载: 仅含 $type / Assembly-CSharp 的文件才需替换, 否则跳过 18 轮整串扫描.
					if (text.IndexOf("$type", StringComparison.Ordinal) >= 0
						|| text.IndexOf("Assembly-CSharp", StringComparison.Ordinal) >= 0)
					{
						foreach (KeyValuePair<string, string> item in 序列化类.定向字典)
						{
							text = text.Replace(item.Key, item.Value);
						}
					}
					try
					{
						TItem val;
						val = JsonConvert.DeserializeObject<TItem>(text, 序列化类.全局设置);
						if (val != null)
						{
							concurrentQueue.Enqueue(val);
						}
					}
					catch (Exception ex)
					{
						主程.添加系统日志(file.FullName + " " + ex.Message);
					}
				});
			}
			return concurrentQueue.ToArray();
		}

		public static byte[] 压缩字节(byte[] data)
		{
			MemoryStream memoryStream;
			memoryStream = new MemoryStream();
			DeflaterOutputStream deflaterOutputStream;
			deflaterOutputStream = new DeflaterOutputStream(memoryStream);
			deflaterOutputStream.Write(data, 0, data.Length);
			deflaterOutputStream.Close();
			return memoryStream.ToArray();
		}

		public static byte[] 解压字节(byte[] data)
		{
			MemoryStream baseInputStream;
			baseInputStream = new MemoryStream(data);
			MemoryStream memoryStream;
			memoryStream = new MemoryStream();
			new InflaterInputStream(baseInputStream).CopyTo(memoryStream);
			return memoryStream.ToArray();
		}

		public static void 备份文件夹(string 源目录, string 文件名)
		{
			if (Directory.Exists(源目录))
			{
				new FastZip().CreateZip(文件名, 源目录, recurse: false, "");
			}
		}
	}
}
