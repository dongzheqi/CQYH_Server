using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using DevExpress.Data.Extensions;
using Newtonsoft.Json;

namespace 游戏服务器.模板类
{
	public sealed class 铭文技能
	{
		public static Dictionary<ushort, 铭文技能> 数据表;

		private static Dictionary<byte, List<铭文技能>> 概率表;

		private static Dictionary<byte, List<铭文技能>> 十次概率表;

		private static Dictionary<byte, List<铭文技能>> 五十概率表;

		[JsonIgnore]
		private Dictionary<游戏对象属性, int>[] _属性加成;

		[Category("基本属性")]
		public string 技能名字 { get; set; }

		[Category("基本属性")]
		public 游戏对象职业 技能职业 { get; set; }

		[Category("基本属性")]
		public ushort 技能编号 { get; set; }

		[Category("基本属性")]
		public byte 铭文编号 { get; set; }

		[Category("技能计数")]
		public byte 技能计数 { get; set; }

		[Category("技能计数")]
		public ushort 计数周期 { get; set; }

		[Category("基本属性")]
		public bool 被动技能 { get; set; }

		[Category("基本属性")]
		public byte 铭文品质 { get; set; }

		[Category("基本属性")]
		public int 洗练概率 { get; set; }

		[Category("基本属性")]
		public bool 十次节点 { get; set; }

		[Category("基本属性")]
		public bool 五十节点 { get; set; }

		[Category("基本属性")]
		public bool 广播通知 { get; set; }

		[Category("基本属性")]
		public string 铭文描述 { get; set; }

		[Category("条件列表")]
		public byte[] 需要角色等级 { get; set; }

		[Category("条件列表")]
		public int[] 需要技能经验 { get; set; }

		[Category("加成列表")]
		public int[] 技能战力加成 { get; set; }

		[Category("加成列表")]
		public 铭文属性[] 铭文属性加成 { get; set; }

		[Category("加成列表")]
		public List<ushort> 铭文附带Buff { get; set; }

		[Category("技能列表")]
		public List<ushort> 被动技能列表 { get; set; }

		[Category("技能列表")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
		public List<string> 主体技能列表 { get; set; }

		[Category("技能列表")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
		public List<string> 开关技能列表 { get; set; }

		[Browsable(false)]
		[JsonIgnore]
		public ushort 铭文索引 => (ushort)(this.技能编号 * 10 + this.铭文编号);

		[Browsable(false)]
		[JsonIgnore]
		public Dictionary<游戏对象属性, int>[] 属性加成
		{
			get
			{
				if (this._属性加成 != null)
				{
					return this._属性加成;
				}
				this._属性加成 = new Dictionary<游戏对象属性, int>[4]
				{
					new Dictionary<游戏对象属性, int>(),
					new Dictionary<游戏对象属性, int>(),
					new Dictionary<游戏对象属性, int>(),
					new Dictionary<游戏对象属性, int>()
				};
				if (this.铭文属性加成 != null)
				{
					铭文属性[] array;
					array = this.铭文属性加成;
					for (int i = 0; i < array.Length; i++)
					{
						铭文属性 铭文属性2;
						铭文属性2 = array[i];
						this._属性加成[0][铭文属性2.属性] = 铭文属性2.零级;
						this._属性加成[1][铭文属性2.属性] = 铭文属性2.一级;
						this._属性加成[2][铭文属性2.属性] = 铭文属性2.二级;
						this._属性加成[3][铭文属性2.属性] = 铭文属性2.三级;
					}
				}
				return this._属性加成;
			}
		}

		public static 铭文技能 随机洗练(byte 洗练职业, byte 洗练类型 = 0, int 洗练节点 = 0)
		{
			switch (洗练类型)
			{
			case 1:
				if (铭文洗炼技能.高级节点次数.FindIndex((ushort x) => x == 洗练节点) >= 0)
				{
					ushort[] array2;
					array2 = 铭文洗炼技能.高级节点铭文[洗练职业];
					if (铭文技能.数据表.TryGetValue(array2[主程.随机数.Next(array2.Length)], out var value3))
					{
						return value3;
					}
				}
				else if (洗练节点 >= 铭文洗炼技能.高级节点最终次数)
				{
					ushort[] array3;
					array3 = 铭文洗炼技能.高级最终铭文[洗练职业];
					if (铭文技能.数据表.TryGetValue(array3[主程.随机数.Next(array3.Length)], out var value4))
					{
						return value4;
					}
				}
				break;
			case 0:
			{
				铭文技能 value2;
				if (铭文洗炼技能.普通节点次数.FindIndex((ushort x) => x == 洗练节点) >= 0)
				{
					ushort[] array;
					array = 铭文洗炼技能.普通节点铭文[洗练职业];
					if (铭文技能.数据表.TryGetValue(array[主程.随机数.Next(array.Length)], out var value))
					{
						return value;
					}
				}
				else if (洗练节点 >= 铭文洗炼技能.普通节点最终次数 && 铭文技能.数据表.TryGetValue(铭文洗炼技能.普通最终铭文[洗练职业], out value2))
				{
					return value2;
				}
				break;
			}
			}
			if (((洗练节点 > 0 && 洗练节点 % 50 == 0) ? 铭文技能.五十概率表 : ((洗练节点 <= 0 || 洗练节点 % 10 != 0) ? 铭文技能.概率表 : 铭文技能.十次概率表)).TryGetValue(洗练职业, out var value5) && value5.Count > 0)
			{
				return value5[主程.随机数.Next(value5.Count)];
			}
			return null;
		}

		// 先填局部表、最后一起原子换引用(理由同 游戏Buff.载入数据 的注释)。本处共四张互相关联的静态表,
		// 必须全部填好后再一次性换过去, 否则热重载期间会出现「数据表 已新、概率表 还旧」之类的中间态,
		// 洗炼/铭文相关逻辑会读到对不上的组合。
		public static void 载入数据()
		{
			Dictionary<ushort, 铭文技能> 新数据表;
			新数据表 = new Dictionary<ushort, 铭文技能>();
			string text;
			text = Settings.游戏数据目录 + "\\System\\技能数据\\铭文数据\\";
			if (Directory.Exists(text))
			{
				object[] array;
				array = 序列化类.反序列化(text, typeof(铭文技能));
				foreach (object obj in array)
				{
					新数据表.Add(((铭文技能)obj).铭文索引, (铭文技能)obj);
				}
			}
			Dictionary<byte, List<铭文技能>> 新概率表;
			新概率表 = new Dictionary<byte, List<铭文技能>>
			{
				[0] = new List<铭文技能>(),
				[1] = new List<铭文技能>(),
				[2] = new List<铭文技能>(),
				[3] = new List<铭文技能>(),
				[4] = new List<铭文技能>(),
				[5] = new List<铭文技能>()
			};
			Dictionary<byte, List<铭文技能>> 新十次概率表;
			新十次概率表 = new Dictionary<byte, List<铭文技能>>
			{
				[0] = new List<铭文技能>(),
				[1] = new List<铭文技能>(),
				[2] = new List<铭文技能>(),
				[3] = new List<铭文技能>(),
				[4] = new List<铭文技能>(),
				[5] = new List<铭文技能>()
			};
			Dictionary<byte, List<铭文技能>> 新五十概率表;
			新五十概率表 = new Dictionary<byte, List<铭文技能>>
			{
				[0] = new List<铭文技能>(),
				[1] = new List<铭文技能>(),
				[2] = new List<铭文技能>(),
				[3] = new List<铭文技能>(),
				[4] = new List<铭文技能>(),
				[5] = new List<铭文技能>()
			};
			foreach (铭文技能 value2 in 新数据表.Values)
			{
				if (value2.铭文编号 != 0)
				{
					Dictionary<byte, List<铭文技能>> dictionary;
					dictionary = (value2.十次节点 ? 新十次概率表 : ((!value2.五十节点) ? 新概率表 : 新五十概率表));
					for (int j = 0; j < value2.洗练概率; j++)
					{
						dictionary[(byte)value2.技能职业].Add(value2);
					}
				}
			}
			foreach (List<铭文技能> value3 in 新概率表.Values)
			{
				for (int k = 0; k < value3.Count; k++)
				{
					铭文技能 value;
					value = value3[k];
					int index;
					index = 主程.随机数.Next(value3.Count);
					value3[k] = value3[index];
					value3[index] = value;
				}
			}
			铭文技能.数据表 = 新数据表;
			铭文技能.概率表 = 新概率表;
			铭文技能.十次概率表 = 新十次概率表;
			铭文技能.五十概率表 = 新五十概率表;
		}

		public 铭文技能()
		{
			this.技能战力加成 = new int[4];
			this.需要角色等级 = new byte[4];
			this.主体技能列表 = new List<string>();
			this.开关技能列表 = new List<string>();
			this.铭文附带Buff = new List<ushort>();
			this.被动技能列表 = new List<ushort>();
		}

		public static void 保存数据()
		{
			string text;
			text = Settings.游戏数据目录 + "\\System\\技能数据\\铭文数据\\";
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
			foreach (KeyValuePair<ushort, 铭文技能> item in 铭文技能.数据表)
			{
				StreamWriter streamWriter;
				streamWriter = File.CreateText($"{text}{item.Value.技能编号}-{item.Value.铭文编号}-{item.Value.技能名字}-{item.Value.铭文编号}-{item.Value.铭文描述}.txt");
				streamWriter.Write(JsonConvert.SerializeObject(item.Value, Formatting.Indented, 序列化类.全局设置));
				streamWriter.Close();
			}
		}

		public override string ToString()
		{
			return $"{this.技能编号}-{this.技能名字}";
		}
	}
}
