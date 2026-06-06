using System;
using System.Collections.Generic;
using System.IO;

namespace 游戏服务器.模板类
{
	public class 高级狩猎
	{
		public static Dictionary<int, 高级狩猎> 数据表;

		public int 狩猎编号;

		public int 最低等级;

		public int 怪物数量;

		public int 需要时间;

		public int 接取物品;

		public int 接取数量;

		public int 每个怪的成本;

		public int 每个怪的经验;

		public int 经验奖励物品;

		public int 经验奖励数量;

		public int 奖励物品;

		public int 奖励数量;

		public static void 载入数据()
		{
			高级狩猎.数据表 = new Dictionary<int, 高级狩猎>();
			string[] array;
			array = File.ReadAllLines(Settings.游戏数据目录 + "\\System\\玩家成长\\advanced_exercise.txt");
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2;
				array2 = array[i].Split("\t");
				高级狩猎 高级狩猎2;
				高级狩猎2 = new 高级狩猎();
				高级狩猎2.狩猎编号 = Convert.ToInt32(array2[1]);
				高级狩猎2.最低等级 = Convert.ToInt32(array2[2]);
				高级狩猎2.怪物数量 = Convert.ToInt32(array2[4]);
				高级狩猎2.需要时间 = Convert.ToInt32(array2[5]);
				高级狩猎2.接取物品 = Convert.ToInt32(array2[6]);
				高级狩猎2.接取数量 = Convert.ToInt32(array2[7]);
				高级狩猎2.每个怪的经验 = Convert.ToInt32(array2[10]);
				高级狩猎2.经验奖励物品 = Convert.ToInt32(array2[11]);
				高级狩猎2.经验奖励数量 = Convert.ToInt32(array2[12]);
				高级狩猎2.奖励物品 = Convert.ToInt32(array2[14]);
				高级狩猎2.奖励数量 = Convert.ToInt32(array2[15]);
				高级狩猎.数据表.Add(高级狩猎2.狩猎编号, 高级狩猎2);
			}
		}
	}
}
