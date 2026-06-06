using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 战功任务
	{
		public static Dictionary<ushort, 战功任务> 数据表;

		public ushort 模板编号;

		public QuestResetType 任务分类;

		public ushort 最大数值;

		public ushort 奖励点数;

		public static void 载入数据()
		{
			战功任务.数据表 = new Dictionary<ushort, 战功任务>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = File.OpenText(Settings.游戏数据目录 + "\\System\\任务成就\\战功任务.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				战功任务 战功任务2;
				战功任务2 = new 战功任务();
				战功任务2.模板编号 = ushort.Parse(item["ID"].ToString());
				战功任务2.任务分类 = ((item["RequiredValue"].ToString() == "ftcDaily") ? QuestResetType.Daily : QuestResetType.Weekly);
				战功任务2.最大数值 = ushort.Parse(item["RequiredValue"].ToString());
				战功任务2.奖励点数 = ushort.Parse(item["Score"].ToString());
				战功任务.数据表.Add(战功任务2.模板编号, 战功任务2);
			}
		}
	}
}
