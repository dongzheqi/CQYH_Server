using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;

namespace 游戏服务器.模板类
{
	public class 游戏物品
	{
		public static Dictionary<int, 游戏物品> 数据表;

		public static Dictionary<string, 游戏物品> 检索表;

		[Category("基本属性")]
		public string 物品名字 { get; set; }

		[Category("基本属性")]
		public int 物品编号 { get; set; }

		[Category("通用属性")]
		public int 物品持久 { get; set; }

		[Category("通用属性")]
		public int 物品重量 { get; set; }

		[Category("通用属性")]
		public int 物品等级 { get; set; }

		[Category("物品条件")]
		public int 需要等级 { get; set; }

		[Category("物品条件")]
		public int 冷却时间 { get; set; }

		[Category("物品条件")]
		public byte 物品分组 { get; set; }

		[Category("物品条件")]
		public int 分组冷却 { get; set; }

		[Category("通用属性")]
		public int 出售价格 { get; set; }

		[Category("技能联动")]
		public ushort 附加技能 { get; set; }

		[Category("特殊属性")]
		public bool 是否绑定 { get; set; }

		[Category("特殊属性")]
		public bool 能否掉落 { get; set; }

		[Category("特殊属性")]
		public bool 能否出售 { get; set; }

		[Category("通用属性")]
		public bool 贵重物品 { get; set; }

		[Category("通用属性")]
		public bool 爆不通知 { get; set; }

		[Category("特殊属性")]
		public bool 资源物品 { get; set; }

		[Category("特殊属性")]
		public bool 广播通知 { get; set; }

		[Category("特殊属性")]
		public bool 系统通知 { get; set; }

		[Category("基本属性")]
		public 物品使用分类 物品分类 { get; set; }

		[Category("物品条件")]
		public 游戏对象职业 需要职业 { get; set; }

		[Category("物品条件")]
		public 游戏对象性别 需要性别 { get; set; }

		[Category("基本属性")]
		public 物品持久分类 持久类型 { get; set; }

		[Category("基本属性")]
		public 物品出售分类 商店类型 { get; set; }

		[Category("物品分解")]
		public bool 能否分解 { get; set; }

		[Category("物品分解")]
		public int 元宝数量 { get; set; }

		[Category("物品分解")]
		public int 金币数量 { get; set; }

		[Category("物品分解")]
		public int 银币数量 { get; set; }

		[Category("物品分解")]
		public int 经验数量 { get; set; }

		[Category("物品分解")]
		public int 双倍经验 { get; set; }

		[Category("物品分解")]
		public string 物品名一 { get; set; }

		[Category("物品分解")]
		public string 物品名二 { get; set; }

		[Category("物品分解")]
		public string 物品名三 { get; set; }

		[Category("物品分解")]
		public int 物品数一 { get; set; }

		[Category("物品分解")]
		public int 物品数二 { get; set; }

		[Category("物品分解")]
		public int 物品数三 { get; set; }

		[Category("可用物品")]
		public 物品使用类型 物品使用 { get; set; }

		[Category("可用物品")]
		public int? 解包物品编号 { get; set; }

		[Category("可用物品")]
		public string 召唤下属名字 { get; set; }

		[Category("可用物品")]
		public List<游戏宝箱> 宝箱物品 { get; set; } = new List<游戏宝箱>();


		[Category("可用物品")]
		public List<使用属性数值> 属性字典 { get; set; } = new List<使用属性数值>();


		[Category("基本属性")]
		public bool 触发lua { get; set; }

		public static 游戏物品 获取数据(int 索引)
		{
			if (!游戏物品.数据表.TryGetValue(索引, out var value))
			{
				return null;
			}
			return value;
		}

		public static 游戏物品 获取数据(string 名字)
		{
			if (!游戏物品.检索表.TryGetValue(名字, out var value))
			{
				return null;
			}
			return value;
		}

		// 先填局部表、最后原子换引用(理由同 游戏Buff.载入数据 的注释)。本处是双表, 两张表必须一起换,
		// 否则会出现「数据表 已是新的、检索表 还是旧的」的中间态。
		public static void 载入数据()
		{
			Dictionary<int, 游戏物品> dictionary;
			dictionary = new Dictionary<int, 游戏物品>();
			Dictionary<string, 游戏物品> dictionary2;
			dictionary2 = new Dictionary<string, 游戏物品>();
			string text;
			text = Settings.游戏数据目录 + "\\System\\物品数据\\普通物品\\";
			if (Directory.Exists(text))
			{
				object[] array;
				array = 序列化类.反序列化(text, typeof(游戏物品));
				for (int i = 0; i < array.Length; i++)
				{
					游戏物品 游戏物品2;
					游戏物品2 = array[i] as 游戏物品;
					dictionary.Add(游戏物品2.物品编号, 游戏物品2);
					dictionary2.Add(游戏物品2.物品名字, 游戏物品2);
				}
			}
			text = Settings.游戏数据目录 + "\\System\\物品数据\\装备物品\\";
			if (Directory.Exists(text))
			{
				object[] array2;
				array2 = 序列化类.反序列化(text, typeof(游戏装备));
				for (int j = 0; j < array2.Length; j++)
				{
					游戏装备 游戏装备2;
					游戏装备2 = array2[j] as 游戏装备;
					dictionary.Add(游戏装备2.物品编号, 游戏装备2);
					dictionary2.Add(游戏装备2.物品名字, 游戏装备2);
				}
			}
			游戏物品.数据表 = dictionary;
			游戏物品.检索表 = dictionary2;
		}

		public override string ToString()
		{
			return $"{this.物品编号}-{this.物品名字}";
		}

		public static void 保存数据()
		{
			string text;
			text = Settings.游戏数据目录 + "\\System\\物品数据\\普通物品\\";
			string text2;
			text2 = Settings.游戏数据目录 + "\\System\\物品数据\\装备物品\\";
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
			files = new DirectoryInfo(text2).GetFiles();
			for (int i = 0; i < files.Length; i++)
			{
				files[i].Delete();
			}
			foreach (KeyValuePair<int, 游戏物品> item in 游戏物品.数据表)
			{
				StreamWriter streamWriter;
				streamWriter = File.CreateText((!(item.Value is 游戏装备)) ? $"{text}{item.Value.物品编号}-{item.Value.物品名字}.txt" : $"{text2}{item.Value.物品编号}-{item.Value.物品名字}.txt");
				streamWriter.Write(JsonConvert.SerializeObject(item.Value, Formatting.Indented, 序列化类.全局设置));
				streamWriter.Close();
			}
		}
	}
}
