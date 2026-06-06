using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using 游戏服务器.工具类;
using Newtonsoft.Json;

namespace 游戏服务器.模板类
{
	public class 龙卫模板
	{
		public static Dictionary<int, 龙卫模板> 数据表;

		public static Dictionary<string, 龙卫模板> 检索表;

		public static List<龙卫设置> 龙卫配置;

		[Category("基本属性")]
		public string 词缀名字 { get; set; }

		[Category("基本属性")]
		public 龙卫词缀类型 词缀类型 { get; set; }

		[Category("基本属性")]
		public int 龙卫编号 { get; set; }

		[Category("基本属性")]
		public int 占位数量 { get; set; }

		[Category("基本属性")]
		public 游戏对象职业 需要职业 { get; set; }

		[Category("基本属性")]
		public float 洗练几率 { get; set; }

		[Category("伤害增减")]
		public ushort[] 增伤地图 { get; set; }

		[Category("伤害增减")]
		public ushort[] 减伤地图 { get; set; }

		[Category("伤害增减")]
		public bool 物理系数减伤 { get; set; }

		[Category("伤害增减")]
		public bool 魔法系数减伤 { get; set; }

		[Category("技能联动")]
		[Editor(typeof(FlagEnumUIEditor), typeof(UITypeEditor))]
		public 指定目标类型 目标类型 { get; set; }

		[Category("技能联动")]
		public bool 检测铭文 { get; set; }

		[Category("技能联动")]
		public byte 绑定铭文 { get; set; }

		[Category("技能联动")]
		public ushort[] 绑定技能 { get; set; }

		[Category("技能联动")]
		public bool 提升治疗总量 { get; set; }

		[Category("增加BUFF")]
		public bool 自身不算几率 { get; set; }

		[Category("增加BUFF")]
		public ushort 自身增加BUFF { get; set; }

		[Category("增加BUFF")]
		public bool 目标不算几率 { get; set; }

		[Category("增加BUFF")]
		public ushort 目标增加BUFF { get; set; }

		[Category("伤害增减")]
		public ushort[] 增伤技能 { get; set; }

		[Category("伤害增减")]
		public ushort[] 减伤技能 { get; set; }

		[Category("技能冷却")]
		public bool 释放减少冷却 { get; set; }

		[Category("技能冷却")]
		public bool 命中概率减少 { get; set; }

		[Category("技能冷却")]
		public int 减少冷却时间 { get; set; }

		[Category("技能冷却")]
		public bool 命中减少冷却 { get; set; }

		[Category("技能冷却")]
		public bool 命中异类放弃 { get; set; }

		[Category("技能冷却")]
		public ushort 减少冷却编号 { get; set; }

		[Category("技能冷却")]
		public byte 减少分组编号 { get; set; }

		[Category("技能冷却")]
		public bool 刷新技能冷却 { get; set; }

		[Category("基本属性")]
		public List<龙卫属性> 龙卫属性 { get; set; }

		[Category("增加属性")]
		public 基础属性[] 增加属性 { get; set; }

		[Category("BUFF联动")]
		public bool 延长BUFF时间 { get; set; }

		[Category("BUFF联动")]
		public bool 增加BUFF护盾 { get; set; }

		[Category("BUFF联动")]
		public bool BUFF伤害基数 { get; set; }

		[Category("BUFF联动")]
		public bool BUFF伤害系数 { get; set; }

		[Category("BUFF联动")]
		public bool 自身伤害基数 { get; set; }

		[Category("BUFF联动")]
		public bool 自身伤害系数 { get; set; }

		[Category("增加BUFF")]
		public bool BUFF属性基数 { get; set; }

		[Category("增加BUFF")]
		public bool BUFF属性系数 { get; set; }

		[Category("BUFF联动")]
		public bool BUFF神圣攻击 { get; set; }

		[Category("BUFF联动")]
		public ushort[] 检测BUFF编号 { get; set; }

		[Category("BUFF联动")]
		public bool 触发BUFF概率 { get; set; }

		[Category("技能联动")]
		public bool 延长诱惑时间 { get; set; }

		[Category("技能联动")]
		public bool 触发技能概率 { get; set; }

		[Category("宠物联动")]
		public bool 延长宠物时间 { get; set; }

		[Category("宠物联动")]
		public ushort 检测已学技能 { get; set; }

		[Category("宠物联动")]
		public ushort 召唤物加BUFF { get; set; }

		public static 龙卫模板 获取数据(int 索引)
		{
			if (!龙卫模板.数据表.TryGetValue(索引, out var value))
			{
				return null;
			}
			return value;
		}

		public static 龙卫模板 获取数据(string 名字)
		{
			if (!龙卫模板.检索表.TryGetValue(名字, out var value))
			{
				return null;
			}
			return value;
		}

		public static TValue RandomValues<TKey, TValue>(Dictionary<TKey, TValue> dict)
		{
			Random random;
			random = new Random();
			new Dictionary<TKey, TValue>();
			int count;
			count = dict.Count;
			return dict[dict.Keys.ToList()[random.Next(count)]];
		}

		public static 龙卫模板 获取龙卫模板(龙卫词缀类型 词缀, 游戏对象职业 职业, bool 不许多格词条, out 龙卫品质 龙卫品质)
		{
			while (true)
			{
				龙卫品质 龙卫品质2;
				龙卫品质2 = (龙卫品质)计算类.范围随机(1, 5);
				List<龙卫模板> list;
				list = 龙卫模板.数据表.Values.Where((龙卫模板 x) => x.词缀类型 == 词缀 && (x.需要职业 == 游戏对象职业.通用 || x.需要职业 == 职业)).ToList();
				龙卫模板 龙卫模板2;
				龙卫模板2 = list[主程.随机数.Next(list.Count)];
				if (龙卫模板2 == null || (不许多格词条 && 龙卫模板2.占位数量 > 1))
				{
					continue;
				}
				foreach (龙卫属性 item in 龙卫模板2.龙卫属性)
				{
					if (item.龙卫品质 == 龙卫品质2 && (item.最大数值 != 0 || item.最小数值 != 0))
					{
						龙卫品质 = item.龙卫品质;
						return 龙卫模板2;
					}
				}
			}
		}

		public static void 载入数据()
		{
			龙卫模板.数据表 = new Dictionary<int, 龙卫模板>();
			龙卫模板.检索表 = new Dictionary<string, 龙卫模板>();
			龙卫模板.龙卫配置 = new List<龙卫设置>();
			string text;
			text = Settings.游戏数据目录 + "\\System\\龙卫数据\\";
			if (Directory.Exists(text))
			{
				object[] array;
				array = 序列化类.反序列化(text, typeof(龙卫模板));
				for (int i = 0; i < array.Length; i++)
				{
					龙卫模板 龙卫模板2;
					龙卫模板2 = array[i] as 龙卫模板;
					龙卫模板.数据表.Add(龙卫模板2.龙卫编号, 龙卫模板2);
					龙卫模板.检索表.Add($"{龙卫模板2.词缀名字}-{龙卫模板2.需要职业}", 龙卫模板2);
				}
			}
			string 配置文件;
			配置文件 = Settings.游戏数据目录 + "\\System\\龙卫数据\\龙卫设置.txt";
			if (!File.Exists(配置文件))
			{
				return;
			}
			string[] array2;
			array2 = Regex.Split(File.ReadAllText(配置文件).Trim('\r', '\n'), "\r?\n", RegexOptions.IgnoreCase);
			for (int j = 0; j < array2.Length; j++)
			{
				if (string.IsNullOrWhiteSpace(array2[j]))
				{
					continue;
				}
				string[] array3;
				array3 = Regex.Split(array2[j], "\t", RegexOptions.IgnoreCase);
				if (array3.Length < 10)
				{
					continue;
				}
				龙卫模板.龙卫配置.Add(new 龙卫设置
				{
					位置 = Convert.ToInt32(array3[0]),
					阶段 = Convert.ToByte(array3[1]),
					金币 = Convert.ToInt32(array3[2]),
					物品编号一 = Convert.ToInt32(array3[4]),
					物品数量一 = Convert.ToUInt16(array3[5]),
					物品编号二 = Convert.ToInt32(array3[6]),
					物品数量二 = Convert.ToUInt16(array3[7]),
					物品编号三 = Convert.ToInt32(array3[8]),
					物品数量三 = Convert.ToUInt16(array3[9])
				});
			}
		}

		public 龙卫模板()
		{
			this.龙卫属性 = new List<龙卫属性>();
		}

		public override string ToString()
		{
			return $"{this.龙卫编号}-{this.词缀名字}";
		}

		public static void 保存数据()
		{
			string text;
			text = Settings.游戏数据目录 + "\\System\\龙卫数据\\";
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
			foreach (KeyValuePair<int, 龙卫模板> item in 龙卫模板.数据表)
			{
				StreamWriter streamWriter;
				streamWriter = File.CreateText($"{text}{item.Value.龙卫编号}-{item.Value.词缀名字}.txt");
				streamWriter.Write(JsonConvert.SerializeObject(item.Value, Formatting.Indented, 序列化类.全局设置));
				streamWriter.Close();
			}
		}

		public static 龙卫设置 获取龙卫设置(int 位置, byte 阶段)
		{
			foreach (龙卫设置 item in 龙卫模板.龙卫配置)
			{
				if (item.位置 == 位置 && item.阶段 == 阶段)
				{
					return item;
				}
			}
			return new 龙卫设置();
		}
	}
}
