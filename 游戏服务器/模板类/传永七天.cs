using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 传永七天
	{
		public static Dictionary<int, int> 奖励表 = new Dictionary<int, int>
		{
			{ 0, 1500521 },
			{ 1, 31000 },
			{ 2, 1500012 },
			{ 3, 1500038 },
			{ 4, 1500233 },
			{ 5, 1501061 },
			{ 6, 140007 },
			{ 7, 0 }
		};

		public static Dictionary<int, int> 积分表 = new Dictionary<int, int>
		{
			{ 0, 40 },
			{ 1, 90 },
			{ 2, 150 },
			{ 3, 210 },
			{ 4, 280 },
			{ 5, 330 },
			{ 6, 400 },
			{ 7, 0 }
		};

		public static Dictionary<int, 传永七天> 数据表;

		public int 编号 { get; set; }

		public string 标题 { get; set; }

		public string 激活状态 { get; set; }

		public int 参数一 { get; set; }

		public int 天数 { get; set; }

		public int 最大值 { get; set; }

		public int 状态类型 { get; set; }

		public int 奖励积分 { get; set; }

		public int 奖励道具 { get; set; }

		public int 奖励数量 { get; set; }

		public static void 载入数据()
		{
			_ = Settings.游戏数据目录 + "\\System\\商城福利\\传永七天.csv";
			传永七天.数据表 = new Dictionary<int, 传永七天>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = File.OpenText(Settings.游戏数据目录 + "\\System\\商城福利\\传永七天.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				传永七天 传永七天2;
				传永七天2 = new 传永七天();
				传永七天2.编号 = int.Parse(item[0].ToString());
				传永七天2.标题 = item[1].ToString();
				传永七天2.激活状态 = item[2].ToString();
				传永七天2.参数一 = int.Parse(item[3].ToString());
				传永七天2.天数 = int.Parse(item[4].ToString());
				传永七天2.最大值 = int.Parse(item[5].ToString());
				传永七天2.状态类型 = int.Parse(item[6].ToString());
				传永七天2.奖励积分 = int.Parse(item[7].ToString());
				传永七天2.奖励道具 = int.Parse(item[8].ToString());
				传永七天2.奖励数量 = int.Parse(item[9].ToString());
				传永七天.数据表.Add(传永七天2.编号, 传永七天2);
			}
		}
	}
}
