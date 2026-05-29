using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using 游戏服务器.数据类;
using Newtonsoft.Json;

namespace 游戏服务器.模板类
{
	public sealed class 游戏商店
	{
		[JsonIgnore]
		public static byte[] 商店文件数据;

		[JsonIgnore]
		public static int 商店文件效验;

		[JsonIgnore]
		public static int 商店物品数量;

		[JsonIgnore]
		public static int 商店回购排序;

		public static Dictionary<int, 游戏商店> 数据表;

		public int 商店编号;

		public string 商店名字;

		public 物品出售分类 回收类型;

		public List<游戏商品> 商品列表;

		[JsonIgnore]
		public SortedSet<物品数据> 回购列表;

		public static void 载入数据()
		{
			Dictionary<int, 游戏商店> dictionary;
			dictionary = new Dictionary<int, 游戏商店>();
			byte[] array;
			array = null;
			int num;
			num = 0;
			int num2;
			num2 = 0;
			string text;
			text = Settings.游戏数据目录 + "\\System\\物品数据\\游戏商店\\";
			if (Directory.Exists(text))
			{
				object[] array2;
				array2 = 序列化类.反序列化(text, typeof(游戏商店));
				foreach (object obj in array2)
				{
					dictionary.Add(((游戏商店)obj).商店编号, (游戏商店)obj);
				}
			}
			using MemoryStream memoryStream = new MemoryStream();
			using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			foreach (游戏商店 item in from X in dictionary.Values.ToList()
				orderby X.商店编号
				select X)
			{
				foreach (游戏商品 item2 in item.商品列表)
				{
					byte[] bytes;
					bytes = Encoding.UTF8.GetBytes(item.商店名字);
					binaryWriter.Write(item.商店编号);
					binaryWriter.Write(bytes);
					binaryWriter.Write(new byte[64 - bytes.Length]);
					binaryWriter.Write(item2.商品编号);
					binaryWriter.Write(item2.单位数量);
					binaryWriter.Write(item2.货币类型);
					binaryWriter.Write(item2.商品价格);
					binaryWriter.Write(item2.备选货币);
					binaryWriter.Write(item2.备选价格);
					binaryWriter.Write(item2.未知1);
					binaryWriter.Write(item2.未知2);
					binaryWriter.Write(item2.未知3);
					binaryWriter.Write(item2.未知4);
					binaryWriter.Write((int)item.回收类型);
					binaryWriter.Write((item2.货币类型 == 0 || item2.绑定物品) ? 1 : 0);
					binaryWriter.Write(item2.活动页面);
					binaryWriter.Write(item2.未知5);
					binaryWriter.Write(item2.未知6);
					binaryWriter.Write(item2.未知7);
					num2++;
				}
			}
			array = 序列化类.压缩字节(memoryStream.ToArray());
			// 原代码对整段固定字节算 CRC 却循环 array.Length 次并累加(O(n²)+溢出), 是本 loader 慢的根因.
			// CRC 对固定数组只需算一次; 商店文件效验 仅作客户端缓存版本令牌(玩家实例.cs:8028 回传比对、不自算),
			// 改单次后客户端部署首次开商店会重新同步一次(无害), 之后稳定.
			num = 计算类.CRC(array);
			游戏商店.商店文件数据 = array;
			游戏商店.商店文件效验 = num;
			游戏商店.商店物品数量 = num2;
			游戏商店.数据表 = dictionary;
		}

		public bool 回购物品(物品数据 物品)
		{
			return this.回购列表.Remove(物品);
		}

		public void 出售物品(物品数据 物品)
		{
			物品.回购编号 = ++游戏商店.商店回购排序;
			if (this.回购列表.Add(物品) && this.回购列表.Count > 50)
			{
				物品数据 物品数据;
				物品数据 = this.回购列表.Last();
				this.回购列表.Remove(物品数据);
				物品数据.删除数据();
			}
		}

		public 游戏商店()
		{
			this.回购列表 = new SortedSet<物品数据>(new 回购排序());
		}

		internal static void 保存数据()
		{
			string text;
			text = Settings.游戏数据目录 + "\\System\\物品数据\\游戏商店\\";
			if (!Directory.Exists(text))
			{
				return;
			}
			FileInfo[] files;
			files = new DirectoryInfo(text).GetFiles();
			for (int i = 0; i < files.Length; i++)
			{
				files[i].Delete();
			}
			foreach (KeyValuePair<int, 游戏商店> item in 游戏商店.数据表)
			{
				StreamWriter streamWriter;
				streamWriter = File.CreateText($"{text}{item.Value.商店编号}-{item.Value.商店名字}.txt");
				streamWriter.Write(JsonConvert.SerializeObject(item.Value, Formatting.Indented, 序列化类.全局设置));
				streamWriter.Close();
			}
		}
	}
}
