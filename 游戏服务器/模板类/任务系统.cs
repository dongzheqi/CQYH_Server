using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 任务系统
	{
		public static Dictionary<int, 任务系统> 数据表;

		public int 模板编号;

		public int 获得物品;

		public static void 载入数据()
		{
			任务系统.数据表 = new Dictionary<int, 任务系统>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = 配置读取.打开(Settings.游戏数据目录 + "\\System\\任务系统.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				任务系统 任务系统2;
				任务系统2 = new 任务系统();
				任务系统2.模板编号 = int.Parse(item["FormulaID"].ToString());
				任务系统2.获得物品 = int.Parse(item["ResultItem"].ToString());
				任务系统.数据表.Add(任务系统2.模板编号, 任务系统2);
			}
		}
	}
}
