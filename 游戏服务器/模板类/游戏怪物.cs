using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace 游戏服务器.模板类
{
	public sealed class 游戏怪物
	{
		public static Dictionary<string, 游戏怪物> 数据表;

		[Browsable(false)]
		[JsonIgnore]
		public Dictionary<游戏物品, long> 掉落统计;

		[Browsable(false)]
		[JsonIgnore]
		private Dictionary<游戏对象属性, int> _基础属性;

		[Browsable(false)]
		[JsonIgnore]
		private Dictionary<游戏对象属性, int>[] _成长属性;

		[Category("基本属性")]
		public string 怪物名字 { get; set; }

		[Category("基本属性")]
		public string 备注信息 { get; set; }

		[Category("基本属性")]
		public ushort 怪物编号 { get; set; }

		[Category("基本属性")]
		public ushort 分组编号 { get; set; }

		[Category("基本属性")]
		public byte 怪物等级 { get; set; }

		[Category("基本属性")]
		public bool 阻塞网格 { get; set; }

		[Category("基本属性")]
		public bool 能被命中 { get; set; }

		[Category("基本属性")]
		public bool 刷新通知 { get; set; }

		[Category("基本属性")]
		public bool 不计存活 { get; set; }

		[Category("怪物类别")]
		public 技能范围类型 怪物体型 { get; set; }

		[Category("怪物类别")]
		public 怪物种族分类 怪物分类 { get; set; }

		[Category("怪物类别")]
		public 怪物级别分类 怪物级别 { get; set; }

		[Category("怪物特性")]
		public bool 怪物禁止移动 { get; set; }

		[Category("怪物特性")]
		public bool 脱战自动石化 { get; set; }

		[Category("怪物特性")]
		public ushort 石化状态编号 { get; set; }

		[Category("怪物特性")]
		public bool 可见隐身目标 { get; set; }

		[Category("怪物特性")]
		public bool 可被技能推动 { get; set; }

		[Category("怪物特性")]
		public bool 可被技能控制 { get; set; }

		[Category("怪物特性")]
		public bool 可被技能诱惑 { get; set; }

		[Category("怪物特性")]
		public float 基础诱惑概率 { get; set; }

		[Category("怪物特性")]
		public ushort 狂暴状态编号 { get; set; }

		[Category("怪物特性")]
		public float 狂暴状态血量 { get; set; }

		[Category("基本属性")]
		public ushort 怪物移动间隔 { get; set; }

		[Category("基本属性")]
		public ushort 怪物漫游间隔 { get; set; }

		[Category("基本属性")]
		public ushort 尸体保留时长 { get; set; }

		[Category("基本属性")]
		public bool 主动攻击目标 { get; set; }

		[Category("基本属性")]
		public byte 怪物仇恨范围 { get; set; }

		[Category("基本属性")]
		public ushort 怪物仇恨时间 { get; set; }

		[Category("怪物攻击")]
		public string 普通攻击技能 { get; set; }

		[Category("怪物攻击")]
		public string[] 概率触发技能 { get; set; }

		[Category("怪物攻击")]
		public string 进入战斗技能 { get; set; }

		[Category("怪物攻击")]
		public string 退出战斗技能 { get; set; }

		[Category("怪物攻击")]
		public string 移动释放技能 { get; set; }

		[Category("怪物攻击")]
		public string 出生释放技能 { get; set; }

		[Category("怪物攻击")]
		public string 死亡释放技能 { get; set; }

		[Category("怪物攻击")]
		public string 狂暴释放技能 { get; set; }

		[Category("怪物属性")]
		public 基础属性[] 怪物基础 { get; set; }

		[Category("宠物属性")]
		public 成长属性[] 怪物成长 { get; set; }

		[Category("宠物属性")]
		public 属性继承[] 继承属性 { get; set; }

		[Category("宠物属性")]
		public ushort 死亡主人BUFF { get; set; }

		[Category("基本属性")]
		public ushort 怪物提供经验 { get; set; }

		[Category("基本属性")]
		public bool 仇恨锁定主人 { get; set; }

		[Browsable(false)]
		[JsonIgnore]
		public List<怪物掉落> 怪物掉落物品 { get; set; }

		[Category("LUA联动")]
		public bool 执行脚本 { get; set; }

		[Category("触发lua")]
		public bool 触发lua { get; set; }

		[Browsable(false)]
		[JsonIgnore]
		public Dictionary<游戏对象属性, int> 基础属性
		{
			get
			{
				if (this._基础属性 != null)
				{
					return this._基础属性;
				}
				this._基础属性 = new Dictionary<游戏对象属性, int>();
				if (this.怪物基础 != null)
				{
					基础属性[] array;
					array = this.怪物基础;
					for (int i = 0; i < array.Length; i++)
					{
						基础属性 基础属性2;
						基础属性2 = array[i];
						this._基础属性[基础属性2.属性] = 基础属性2.数值;
					}
				}
				return this._基础属性;
			}
		}

		[Browsable(false)]
		[JsonIgnore]
		public Dictionary<游戏对象属性, int>[] 成长属性
		{
			get
			{
				if (this._成长属性 != null)
				{
					return this._成长属性;
				}
				this._成长属性 = new Dictionary<游戏对象属性, int>[8]
				{
					new Dictionary<游戏对象属性, int>(),
					new Dictionary<游戏对象属性, int>(),
					new Dictionary<游戏对象属性, int>(),
					new Dictionary<游戏对象属性, int>(),
					new Dictionary<游戏对象属性, int>(),
					new Dictionary<游戏对象属性, int>(),
					new Dictionary<游戏对象属性, int>(),
					new Dictionary<游戏对象属性, int>()
				};
				if (this.怪物成长 != null)
				{
					成长属性[] array;
					array = this.怪物成长;
					for (int i = 0; i < array.Length; i++)
					{
						成长属性 成长属性2;
						成长属性2 = array[i];
						this._成长属性[0][成长属性2.属性] = 成长属性2.零级;
						this._成长属性[1][成长属性2.属性] = 成长属性2.一级;
						this._成长属性[2][成长属性2.属性] = 成长属性2.二级;
						this._成长属性[3][成长属性2.属性] = 成长属性2.三级;
						this._成长属性[4][成长属性2.属性] = 成长属性2.四级;
						this._成长属性[5][成长属性2.属性] = 成长属性2.五级;
						this._成长属性[6][成长属性2.属性] = 成长属性2.六级;
						this._成长属性[7][成长属性2.属性] = 成长属性2.七级;
					}
				}
				return this._成长属性;
			}
		}

		public static void 重载所有怪物爆率()
		{
			foreach (KeyValuePair<string, 游戏怪物> item in 游戏怪物.数据表)
			{
				游戏怪物.加载怪物爆率(item.Value);
			}
		}

		public static void 加载怪物爆率(游戏怪物 游戏怪物2, string 自定义文件 = "")
		{
			string path;
			path = Settings.游戏数据目录 + "\\System\\Npc数据\\怪物爆率\\" + (自定义文件.Equals(string.Empty) ? (游戏怪物2.怪物名字 + ".txt") : 自定义文件);
			if (!File.Exists(path))
			{
				return;
			}
			if (游戏怪物2.怪物掉落物品 == null)
			{
				游戏怪物2.怪物掉落物品 = new List<怪物掉落>();
			}
			if (自定义文件.Equals(string.Empty))
			{
				游戏怪物2.怪物掉落物品.Clear();
			}
			StreamReader streamReader;
			streamReader = File.OpenText(path);
			int num;
			num = 0;
			int num2;
			num2 = 0;
			掉落条件分组 条件分组;
			条件分组 = null;
			while (true)
			{
				string text;
				text = streamReader.ReadLine();
				if (text == null)
				{
					break;
				}
				if (text.StartsWith(";") || text.Equals(string.Empty))
				{
					continue;
				}
				string[] array;
				array = text.Trim().Split(' ', '\t');
				if (text.StartsWith("#CALL"))
				{
					if (array.Length == 2)
					{
						游戏怪物.加载怪物爆率(游戏怪物2, array[1]);
					}
				}
				else if (text.StartsWith("#BEGIN"))
				{
					num2 = ++num;
					num = num2;
					text = text.Remove(0, 6);
					if (text.Length > 5)
					{
						条件分组 = new 掉落条件分组(text);
					}
				}
				else if (text.StartsWith("#CHECK"))
				{
					text = text.Remove(0, 6);
					if (text.Length > 5)
					{
						条件分组 = new 掉落条件分组(text);
					}
				}
				else if (text.StartsWith("#END"))
				{
					num2 = 0;
					条件分组 = null;
				}
				else
				{
					if (array.Length < 2)
					{
						continue;
					}
					string[] array2;
					array2 = array[0].Split('/');
					if (!array[1].StartsWith('$') && !array[1].StartsWith('#'))
					{
						游戏怪物2.怪物掉落物品.Add(new 怪物掉落
						{
							怪物名字 = 游戏怪物2.怪物名字,
							物品名字 = array[1],
							最小数量 = ((array.Length <= 3) ? 1 : Convert.ToInt32(array[3])),
							最大数量 = ((array.Length <= 2) ? 1 : Convert.ToInt32(array[2])),
							公告ID = ((array.Length > 4) ? Convert.ToInt32(array[4]) : 0),
							掉落概率 = Convert.ToInt32(array2[1]),
							暴率分组 = num2,
							条件分组 = 条件分组
						});
						continue;
					}
					StreamReader streamReader2;
					string 爆率名 = array[1].Substring(1);
					foreach (char c in 爆率名)
					{
						if (!(char.IsLetterOrDigit(c) || c == '_' || c == '-'))
						{
							throw new ArgumentException("非法怪物爆率名: " + 爆率名);
						}
					}
					string 爆率根目录 = Path.GetFullPath(Settings.游戏数据目录 + "\\System\\Npc数据\\怪物爆率");
					string 爆率文件 = Path.GetFullPath(Path.Combine(爆率根目录, "@" + 爆率名 + ".txt"));
					if (!爆率文件.StartsWith(爆率根目录 + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
					{
						throw new UnauthorizedAccessException("路径越权: " + 爆率文件);
					}
					streamReader2 = File.OpenText(爆率文件);
					if (array[1].StartsWith('#'))
					{
						num2 = ++num;
						num = num2;
					}
					while (true)
					{
						string text2;
						text2 = streamReader2.ReadLine();
						if (text2 == null)
						{
							break;
						}
						游戏怪物2.怪物掉落物品.Add(new 怪物掉落
						{
							怪物名字 = 游戏怪物2.怪物名字,
							物品名字 = text2,
							最小数量 = ((array.Length <= 3) ? 1 : Convert.ToInt32(array[3])),
							最大数量 = ((array.Length <= 2) ? 1 : Convert.ToInt32(array[2])),
							公告ID = ((array.Length > 4) ? Convert.ToInt32(array[4]) : 0),
							掉落概率 = Convert.ToInt32(array2[1]),
							暴率分组 = num2
						});
					}
					if (array[1].StartsWith('#'))
					{
						num2 = 0;
					}
				}
			}
		}

		public static void 载入数据()
		{
			Dictionary<string, 游戏怪物> dictionary;
			dictionary = new Dictionary<string, 游戏怪物>();
			string text;
			text = Settings.游戏数据目录 + "\\System\\Npc数据\\怪物数据\\";
			if (!Directory.Exists(text))
			{
				return;
			}
			object[] array;
			array = 序列化类.反序列化(text, typeof(游戏怪物));
			// 怪物爆率为每怪物一份独立 .txt(还可经 #CALL/@ 递归读更多文件), 各自只写自己的 怪物掉落物品,
			// 彼此独立 -> 并行加载这波二次 IO(约4215次 File.OpenText). dictionary.Add 留到下面单线程做.
			Parallel.ForEach(array, delegate (object obj)
			{
				if (obj is 游戏怪物 游戏怪物3)
				{
					游戏怪物.加载怪物爆率(游戏怪物3);
				}
			});
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] is 游戏怪物 游戏怪物2)
				{
					dictionary.Add(游戏怪物2.怪物名字, 游戏怪物2);
				}
			}
			游戏怪物.数据表 = dictionary;
		}

		public static 游戏怪物 获取游戏怪物(string 怪物名字)
		{
			游戏怪物.数据表.TryGetValue(怪物名字, out var value);
			return value;
		}

		public 游戏怪物()
		{
			this.概率触发技能 = new string[0];
			this.掉落统计 = new Dictionary<游戏物品, long>();
			this.怪物掉落物品 = new List<怪物掉落>();
		}

		public override string ToString()
		{
			return $"{this.怪物编号}-{this.怪物名字}";
		}

		internal static void 保存数据()
		{
			string text;
			text = Settings.游戏数据目录 + "\\System\\Npc数据\\怪物数据\\";
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
			foreach (KeyValuePair<string, 游戏怪物> item in 游戏怪物.数据表)
			{
				StreamWriter streamWriter;
				streamWriter = File.CreateText($"{text}{item.Value.怪物编号}-{item.Value.怪物名字}.txt");
				streamWriter.Write(JsonConvert.SerializeObject(item.Value, Formatting.Indented, 序列化类.全局设置));
				streamWriter.Close();
			}
		}
	}
}
