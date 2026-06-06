using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace 游戏服务器.模板类
{
	public class 装备精炼
	{
		public static Dictionary<int, 装备精炼> 数据表;

		public static Dictionary<int, 精炼属性> 属性表;

		public int 装备编号;

		public string 装备名字;

		public int 开槽编号;

		public int[] 开槽数量;

		public int 继承编号;

		public int 继承数量;

		public int 精炼编号;

		public int[] 精炼组号;

		public int 精炼上锁;

		public int 需要金币一;

		public int 需要金币二;

		public int 需要金币三;

		public int 精炼物品数量一;

		public int 精炼物品分组数量一;

		public int 精炼上锁数量一;

		public int 精炼物品数量二;

		public int 精炼物品分组数量二;

		public int 精炼上锁数量二;

		public int 精炼物品数量三;

		public int 精炼物品分组数量三;

		public int 精炼上锁数量三;

		public static void 载入数据()
		{
			装备精炼.数据表 = new Dictionary<int, 装备精炼>();
			string[] array;
			array = File.ReadAllLines(Settings.游戏数据目录 + "\\System\\装备养成\\equip_refine.txt");
			foreach (string obj in array)
			{
				装备精炼 装备精炼2;
				装备精炼2 = new 装备精炼();
				string[] array2;
				array2 = obj.Split("\t");
				装备精炼2.装备编号 = Convert.ToInt32(array2[0]);
				装备精炼2.装备名字 = array2[1];
				装备精炼2.开槽编号 = Convert.ToInt32(array2[2]);
				装备精炼2.开槽数量 = new int[3]
				{
					Convert.ToInt32(array2[3]),
					Convert.ToInt32(array2[4]),
					Convert.ToInt32(array2[5])
				};
				装备精炼2.继承编号 = Convert.ToInt32(array2[6]);
				装备精炼2.继承数量 = Convert.ToInt32(array2[7]);
				装备精炼2.精炼编号 = Convert.ToInt32(array2[8]);
				string[] array3;
				array3 = (from s in array2[9].Split(";")
					where !string.IsNullOrEmpty(s)
					select s).ToArray();
				装备精炼2.精炼组号 = new int[array3.Length];
				for (int j = 0; j < array3.Length; j++)
				{
					装备精炼2.精炼组号[j] = Convert.ToInt32(array3[j]);
				}
				装备精炼2.精炼上锁 = Convert.ToInt32(array2[10]);
				装备精炼2.需要金币一 = Convert.ToInt32(array2[11]);
				装备精炼2.需要金币二 = Convert.ToInt32(array2[12]);
				装备精炼2.需要金币三 = Convert.ToInt32(array2[13]);
				装备精炼2.精炼物品数量一 = Convert.ToInt32(array2[14]);
				装备精炼2.精炼物品分组数量一 = Convert.ToInt32(array2[15]);
				装备精炼2.精炼上锁数量一 = Convert.ToInt32(array2[16]);
				装备精炼2.精炼物品数量二 = Convert.ToInt32(array2[17]);
				装备精炼2.精炼物品分组数量二 = Convert.ToInt32(array2[18]);
				装备精炼2.精炼上锁数量二 = Convert.ToInt32(array2[19]);
				装备精炼2.精炼物品数量三 = Convert.ToInt32(array2[20]);
				装备精炼2.精炼物品分组数量三 = Convert.ToInt32(array2[21]);
				装备精炼2.精炼上锁数量三 = Convert.ToInt32(array2[22]);
				装备精炼.数据表.Add(装备精炼2.装备编号, 装备精炼2);
			}
			装备精炼.属性表 = new Dictionary<int, 精炼属性>();
			array = File.ReadAllLines(Settings.游戏数据目录 + "\\System\\装备养成\\equip_refine_attribute.txt");
			foreach (string obj2 in array)
			{
				精炼属性 精炼属性2;
				精炼属性2 = new 精炼属性();
				string[] array4;
				array4 = obj2.Split("\t");
				精炼属性2.属性编号 = Convert.ToUInt16(array4[0]);
				精炼属性2.属性类型 = 计算类.字符转属性(array4[1]);
				精炼属性2.属性数值 = Convert.ToInt32(array4[2]);
				精炼属性2.战力加成 = Convert.ToInt32(array4[3]);
				精炼属性2.高级属性 = Convert.ToInt32(array4[4]) == 1;
				精炼属性2.开槽几率 = Convert.ToInt32(array4[5]);
				精炼属性2.精炼几率一 = Convert.ToInt32(array4[6]);
				精炼属性2.精炼几率二 = Convert.ToInt32(array4[7]);
				精炼属性2.精炼几率三 = Convert.ToInt32(array4[8]);
				精炼属性2.锁定精炼几率二 = Convert.ToInt32(array4[9]);
				精炼属性2.锁定精炼几率三 = Convert.ToInt32(array4[10]);
				精炼属性2.职业专属 = new bool[6];
				精炼属性2.职业专属[0] = Convert.ToInt32(array4[11]) == 1;
				精炼属性2.职业专属[1] = Convert.ToInt32(array4[12]) == 1;
				精炼属性2.职业专属[2] = Convert.ToInt32(array4[13]) == 1;
				精炼属性2.职业专属[3] = Convert.ToInt32(array4[14]) == 1;
				精炼属性2.职业专属[4] = Convert.ToInt32(array4[15]) == 1;
				精炼属性2.职业专属[5] = Convert.ToInt32(array4[16]) == 1;
				精炼属性2.装备专属 = new bool[16];
				精炼属性2.装备专属[9] = Convert.ToInt32(array4[17]) == 1;
				精炼属性2.装备专属[11] = Convert.ToInt32(array4[18]) == 1;
				精炼属性2.装备专属[8] = Convert.ToInt32(array4[19]) == 1;
				精炼属性2.装备专属[1] = Convert.ToInt32(array4[20]) == 1;
				精炼属性2.装备专属[10] = Convert.ToInt32(array4[21]) == 1;
				精炼属性2.装备专属[3] = Convert.ToInt32(array4[22]) == 1;
				精炼属性2.装备专属[4] = Convert.ToInt32(array4[23]) == 1;
				精炼属性2.装备专属[8] = Convert.ToInt32(array4[24]) == 1;
				精炼属性2.装备专属[5] = Convert.ToInt32(array4[25]) == 1;
				精炼属性2.装备专属[6] = Convert.ToInt32(array4[26]) == 1;
				精炼属性2.装备专属[7] = Convert.ToInt32(array4[27]) == 1;
				精炼属性2.装备专属[12] = Convert.ToInt32(array4[28]) == 1;
				精炼属性2.装备专属[13] = Convert.ToInt32(array4[29]) == 1;
				精炼属性2.属性品质 = Convert.ToInt32(array4[32]);
				装备精炼.属性表.Add(精炼属性2.属性编号, 精炼属性2);
			}
		}
	}
}
