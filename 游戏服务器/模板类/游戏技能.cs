using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using 游戏服务器.工具类;
using Newtonsoft.Json;

namespace 游戏服务器.模板类
{
	public sealed class 游戏技能
	{
		public static Dictionary<string, 游戏技能> 数据表;

		[Category("基本属性")]
		public string 技能名字 { get; set; }

		[Category("基本属性")]
		public 游戏对象职业 技能职业 { get; set; }

		[Category("基本属性")]
		public 技能对应类型 技能类型 { get; set; }

		[Category("基本属性")]
		public ushort 自身技能编号 { get; set; }

		[Category("基本属性")]
		public byte 自身铭文编号 { get; set; }

		[Category("基本属性")]
		public byte 技能分组编号 { get; set; }

		[Category("基本属性")]
		public bool 动作打断 { get; set; }

		[Category("高级属性")]
		public ushort 绑定等级编号 { get; set; }

		[Category("检测状态")]
		public bool 需要正向走位 { get; set; }

		[Category("高级属性")]
		public byte 技能最远距离 { get; set; }

		[Category("条件判断")]
		public bool 计算幸运概率 { get; set; }

		[Category("条件判断")]
		public float 计算触发概率 { get; set; }

		[Category("属性提升")]
		public 游戏对象属性 属性提升概率 { get; set; }

		[Category("属性提升")]
		public float 属性提升系数 { get; set; }

		[Category("检测状态")]
		public bool 检查忙绿状态 { get; set; }

		[Category("检测状态")]
		public bool 检查硬直状态 { get; set; }

		[Category("检测状态")]
		public bool 检查职业武器 { get; set; }

		[Category("检测状态")]
		public bool 检查被动标记 { get; set; }

		[Category("检测状态")]
		public bool 检查技能标记 { get; set; }

		[Category("检测状态")]
		public bool 检查技能计数 { get; set; }

		[Category("高级属性")]
		public ushort 技能标记编号 { get; set; }

		[Category("条件判断")]
		public int[] 需要消耗魔法 { get; set; }

		[Category("条件判断")]
		[Editor(typeof(HashSetEditor), typeof(UITypeEditor))]
		public HashSet<int> 需要消耗物品 { get; set; }

		[Category("高级属性")]
		public int 消耗物品数量 { get; set; }

		[Category("高级属性")]
		public int 战具扣除点数 { get; set; }

		[Category("条件判断")]
		public ushort 验证已学技能 { get; set; }

		[Category("条件判断")]
		public byte 验证技能铭文 { get; set; }

		[Category("条件判断")]
		public ushort 验证角色Buff { get; set; }

		[Category("条件判断")]
		public int 角色Buff层数 { get; set; }

		[Category("条件判断")]
		[Editor(typeof(FlagEnumUIEditor), typeof(UITypeEditor))]
		public 指定目标类型 验证目标类型 { get; set; }

		[Category("条件判断")]
		public ushort 验证目标Buff { get; set; }

		[Category("条件判断")]
		public int 目标Buff层数 { get; set; }

		[Category("血量校验-仅怪物有效")]
		public bool 验证自身血量 { get; set; }

		[Category("血量校验-仅怪物有效")]
		public float 自身血量高于 { get; set; }

		[Category("血量校验-仅怪物有效")]
		public float 自身血量低于 { get; set; }

		[Category("血量校验-仅怪物有效")]
		public bool 验证目标血量 { get; set; }

		[Category("血量校验-仅怪物有效")]
		public float 目标血量高于 { get; set; }

		[Category("血量校验-仅怪物有效")]
		public float 目标血量低于 { get; set; }

		[Editor(typeof(NodeEditor), typeof(UITypeEditor))]
		public SortedDictionary<int, 技能任务> 节点列表 { get; set; }

		// 先填局部表、最后一行原子换引用, 消除热重载期间的空表窗口(理由同 游戏Buff.载入数据 的注释)。
		public static void 载入数据()
		{
			Dictionary<string, 游戏技能> dictionary;
			dictionary = new Dictionary<string, 游戏技能>();
			string text;
			text = Settings.游戏数据目录 + "\\System\\技能数据\\技能数据\\";
			if (Directory.Exists(text))
			{
				object[] array;
				array = 序列化类.反序列化(text, typeof(游戏技能));
				foreach (object obj in array)
				{
					dictionary.Add(((游戏技能)obj).技能名字, (游戏技能)obj);
				}
			}
			游戏技能.数据表 = dictionary;
		}

		public static void 保存数据()
		{
			string text;
			text = Settings.游戏数据目录 + "\\System\\技能数据\\技能数据\\";
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
			foreach (KeyValuePair<string, 游戏技能> item in 游戏技能.数据表)
			{
				StreamWriter streamWriter;
				streamWriter = File.CreateText($"{text}{item.Value.自身技能编号}-{item.Value.自身铭文编号}-{item.Value.技能名字}.txt");
				streamWriter.Write(JsonConvert.SerializeObject(item.Value, Formatting.Indented, 序列化类.全局设置).Replace("游戏服务器.模板类.", ""));
				streamWriter.Close();
			}
		}

		public static 游戏技能 GetById(ushort id)
		{
			foreach (KeyValuePair<string, 游戏技能> item in 游戏技能.数据表)
			{
				if (item.Value.自身技能编号 == id)
				{
					return item.Value;
				}
			}
			return null;
		}

		public 游戏技能()
		{
			this.角色Buff层数 = 1;
			this.目标Buff层数 = 1;
			this.节点列表 = new SortedDictionary<int, 技能任务>();
			this.需要消耗物品 = new HashSet<int>();
		}

		public override string ToString()
		{
			return $"{this.自身技能编号}-{this.技能名字}";
		}
	}
}
