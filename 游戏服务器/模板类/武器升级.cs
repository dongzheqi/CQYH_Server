using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 武器升级
	{
		public static List<武器升级> 数据表;

		public int 模板编号;

		public int 升级次数;

		public int 需要物品;

		public int 需要数量;

		public int 需要金币;

		public int 需要物品二;

		public static void 载入数据()
		{
			武器升级.数据表 = new List<武器升级>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = File.OpenText(Settings.游戏数据目录 + "\\System\\装备养成\\武器升级.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				武器升级 武器升级2;
				武器升级2 = new 武器升级();
				武器升级2.模板编号 = int.Parse(item["EquipID"].ToString());
				武器升级2.升级次数 = int.Parse(item["LevelupTimeLimit"].ToString());
				武器升级2.需要物品 = int.Parse(item["ItemCostID"].ToString());
				武器升级2.需要数量 = int.Parse(item["ItemCostCount"].ToString());
				武器升级2.需要金币 = int.Parse(item["MoneyCostCount"].ToString());
				武器升级2.需要物品二 = int.Parse(item["ItemCostID2"].ToString());
				武器升级.数据表.Add(武器升级2);
			}
		}
	}
}
