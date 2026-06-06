using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.数据类
{
	public class 杀怪成就
	{
		public static Dictionary<ushort, 杀怪成就> 数据表;

		public ushort 成就编号;

		public ushort 怪物编号;

		public ushort 分组编号;

		public int[] 击杀数量;

		public int[] 成就点数;

		public int[] 奖励经验;

		public 杀怪成就()
		{
			this.击杀数量 = new int[8];
			this.成就点数 = new int[8];
			this.奖励经验 = new int[8];
		}

		public static void 载入数据()
		{
			杀怪成就.数据表 = new Dictionary<ushort, 杀怪成就>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = File.OpenText(Settings.游戏数据目录 + "\\System\\任务成就\\杀怪成就.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				杀怪成就 杀怪成就2;
				杀怪成就2 = new 杀怪成就();
				杀怪成就2.成就编号 = ushort.Parse(item["QuestID"].ToString());
				杀怪成就2.怪物编号 = ushort.Parse(item["NpcTemplateID"].ToString());
				杀怪成就2.分组编号 = ushort.Parse(item["NpcGroupID"].ToString());
				for (int i = 0; i < 8; i++)
				{
					杀怪成就2.击杀数量[i] = int.Parse(item[$"KillCount{i + 1}"].ToString());
					杀怪成就2.成就点数[i] = int.Parse(item[$"AchievementPts{i + 1}"].ToString());
					杀怪成就2.奖励经验[i] = int.Parse(item[$"RewardExp{i + 1}"].ToString());
				}
				杀怪成就.数据表.Add(杀怪成就2.成就编号, 杀怪成就2);
			}
		}
	}
}
