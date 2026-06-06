using System;
using System.Collections.Generic;
using System.IO;

namespace 游戏服务器.模板类
{
	public class 传奇之力
	{
		public static List<传奇之力> 数据表;

		public static Dictionary<游戏对象职业, Dictionary<int, 传奇之力>> 检索表;

		public int 需要等级;

		public 游戏对象职业 需要职业;

		public Dictionary<游戏对象属性, int> 数据属性;

		public int 需要物品;

		public int 需要数量;

		public int 战斗力值;

		public static void 载入数据()
		{
			传奇之力.数据表 = new List<传奇之力>();
			传奇之力.检索表 = new Dictionary<游戏对象职业, Dictionary<int, 传奇之力>>();
			传奇之力.检索表[游戏对象职业.战士] = new Dictionary<int, 传奇之力>();
			传奇之力.检索表[游戏对象职业.法师] = new Dictionary<int, 传奇之力>();
			传奇之力.检索表[游戏对象职业.刺客] = new Dictionary<int, 传奇之力>();
			传奇之力.检索表[游戏对象职业.弓手] = new Dictionary<int, 传奇之力>();
			传奇之力.检索表[游戏对象职业.道士] = new Dictionary<int, 传奇之力>();
			传奇之力.检索表[游戏对象职业.龙枪] = new Dictionary<int, 传奇之力>();
			string[] array;
			array = File.ReadAllLines(Settings.游戏数据目录 + "\\System\\玩家成长\\legend_power.txt");
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2;
				array2 = array[i].Split("\t");
				传奇之力 传奇之力2;
				传奇之力2 = new 传奇之力();
				传奇之力2.需要等级 = Convert.ToInt32(array2[0]);
				传奇之力2.需要职业 = 计算类.字符转职业(array2[1]);
				游戏对象属性 key;
				key = 计算类.字符转属性(array2[4]);
				int value;
				value = Convert.ToInt32(array2[5]);
				游戏对象属性 key2;
				key2 = 计算类.字符转属性(array2[6]);
				int value2;
				value2 = Convert.ToInt32(array2[7]);
				传奇之力2.需要物品 = Convert.ToInt32(array2[8]);
				传奇之力2.需要数量 = Convert.ToInt32(array2[9]);
				传奇之力2.战斗力值 = Convert.ToInt32(array2[10]);
				传奇之力2.数据属性 = new Dictionary<游戏对象属性, int>
				{
					{ key, value },
					{ key2, value2 }
				};
				传奇之力.数据表.Add(传奇之力2);
				传奇之力.检索表[传奇之力2.需要职业][传奇之力2.需要等级] = 传奇之力2;
			}
		}
	}
}
