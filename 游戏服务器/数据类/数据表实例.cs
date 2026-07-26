using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace 游戏服务器.数据类
{
	public sealed class 数据表实例<T> : 数据表基类 where T : 游戏数据, new()
	{
		public 数据表实例()
		{
			base.当前映射 = new 数据映射(base.数据类型 = typeof(T));
			base.数据表 = new Dictionary<int, 游戏数据>();
			base.检索表 = new Dictionary<string, 游戏数据>();
			数据快速检索 customAttribute;
			customAttribute = CustomAttributeExtensions.GetCustomAttribute<数据快速检索>(base.数据类型);
			if (customAttribute != null)
			{
				base.检索字段 = base.数据类型.GetField(customAttribute.检索字段, BindingFlags.Instance | BindingFlags.Public);
			}
			if (base.数据类型 == typeof(行会数据))
			{
				base.当前索引 = 1610612736;
			}
			if (base.数据类型 == typeof(队伍数据))
			{
				base.当前索引 = 1879048192;
			}
		}

		public override void 添加数据(游戏数据 数据, bool 分配索引 = false)
		{
			if (分配索引)
			{
				数据.数据索引.V = ++base.当前索引;
			}
			if (数据.数据索引.V == 0)
			{
				MessageBox.Show("数据表添加数据异常, 索引为零.");
			}
			数据.数据存表 = this;
			base.数据表.Add(数据.数据索引.V, 数据);
			if (base.检索字段 != null)
			{
				base.检索表.Add((base.检索字段.GetValue(数据) as 数据监视器<string>).V, 数据);
			}
			if (!游戏数据网关.已经修改)
			{
				游戏数据网关.已经修改 = true;
			}
		}

		public override void 删除数据(游戏数据 数据)
		{
			base.数据表.Remove(数据.数据索引.V);
			if (base.检索字段 != null)
			{
				base.检索表.Remove((base.检索字段.GetValue(数据) as 数据监视器<string>).V);
			}
			if (!游戏数据网关.已经修改)
			{
				游戏数据网关.已经修改 = true;
			}
		}

		public override void 保存数据()
		{
			foreach (KeyValuePair<int, 游戏数据> item in base.数据表.ToList())
			{
				if (!base.版本一致 || item.Value.已经修改)
				{
					item.Value.保存数据();
				}
			}
			base.版本一致 = true;
		}

		public override void 强制保存()
		{
			foreach (KeyValuePair<int, 游戏数据> item in base.数据表.ToList())
			{
				item.Value.保存数据();
			}
			base.版本一致 = true;
		}

		public override void 加载数据(byte[] 存表数据, 数据映射 历史映射)
		{
			base.版本一致 = 历史映射.检查映射版本(base.当前映射);
			using MemoryStream input = new MemoryStream(存表数据);
			using BinaryReader binaryReader = new BinaryReader(input);
			base.当前索引 = binaryReader.ReadInt32();
			int num;
			num = binaryReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				T val;
				val = new T
				{
					数据存表 = this
				};
				int num2;
				num2 = binaryReader.ReadInt32();
				if (num2 <= 0)
				{
					主程.添加系统日志($"{base.数据类型.Name}.无效,  len: {num2}");
				}
				else
				{
					try
					{
						val.原始数据 = binaryReader.ReadBytes(num2);
					}
					catch (Exception ex)
					{
						val.原始数据 = null;
						主程.添加系统日志($"{base.数据类型.Name}.加载数据 , Len:{num2} 线程ID:{Thread.CurrentThread.ManagedThreadId} e:{ex.Message} {((ex.InnerException != null) ? (".IE:" + ex.InnerException.Message) : "...")}");
					}
				}
				val.加载数据(历史映射);
				if (val.数据索引 != null)
				{
					base.数据表[val.数据索引.V] = val;
					if (base.检索字段 != null && base.检索字段.GetValue(val) is 数据监视器<string> { V: not null } 数据监视器2)
					{
						base.检索表[数据监视器2.V] = val;
					}
				}
			}
			主程.添加系统日志($"{base.数据类型.Name}已经加载,  数量: {num}");
		}

		public override byte[] 存表数据()
		{
			// 本方法跑在后台线程(主程.保存数据库 → Task.Run → 游戏数据网关.导出数据), 而主循环同时会在
			// 添加数据(:45 数据表.Add) / 删除数据(:58 数据表.Remove) 里增删同一张表——捡物/吃药/卖物每秒都在跑。
			// 原先「先写死 数据表.Count 再裸 foreach 数据表」有两条坏路径:
			//   撞 Add    → Dictionary 递增 _version, foreach 抛「集合已修改」, 被 主程.服务.cs 的 catch
			//               吞成一行「客户数据保存异常」, 自动保存长期静默失效, 挂机后丢几小时数据;
			//   撞 Remove → .NET Core 的 Dictionary.Remove 不递增 _version(与 Add 不同), foreach 静默少写
			//               记录, 头部写死的 Count 大于实际条数 → 下次开服 加载数据(:98) 按 Count 循环,
			//               读尽后 :106 的 ReadInt32 撞流尾抛 EndOfStreamException(该行不在 try 内), 存档报废。
			// 改为先取快照, 使「头部记录数」与「实际写出条数」同源, 彻底消除后者(存档损坏)。前者收窄到
			// ToList 自身那一瞬, 故加有限重试, 避免一次撞车就丢掉整轮存盘。
			// 与同文件 保存数据(:71) / 强制保存(:83) 的 .ToList() 快照范式保持一致。
			List<KeyValuePair<int, 游戏数据>> 快照 = null;
			for (int 重试次数 = 0; 重试次数 < 5; 重试次数++)
			{
				try
				{
					快照 = base.数据表.ToList();
					break;
				}
				catch (InvalidOperationException)
				{
					Thread.Sleep(1);
				}
			}
			if (快照 == null)
			{
				// 宁可让本轮存盘整体失败(导出数据 未走到 File.Move, 磁盘上的旧 数据文件 原样保留),
				// 也不写出记录数错位的存档。
				throw new InvalidOperationException(base.数据类型.Name + ".存表数据 连续 5 次取快照均撞上主循环增删, 本轮放弃存盘以免写出错位存档");
			}
			using MemoryStream memoryStream = new MemoryStream();
			using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(base.当前索引);
			binaryWriter.Write(快照.Count);
			foreach (KeyValuePair<int, 游戏数据> item in 快照)
			{
				item.Value.导出数据(binaryWriter);
			}
			// 原有 memoryStream.Seek(4L, SeekOrigin.Begin) 已删除: ToArray() 无视流位置, 该行是死代码,
			// 且偏移 4 正好是 Count 字段, 极易被误读为「回头重写真实条数」而掩盖上面这个缺陷。
			return memoryStream.ToArray();
		}
	}
}
