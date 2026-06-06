using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 每日签到
	{
		public static List<每日签到> 数据表;

		public byte 签到天数;

		public int 签到类型;

		public int 限制等级;

		public int 奖励物品;

		public int 奖励数量;

		public static void 载入数据()
		{
			每日签到.数据表 = new List<每日签到>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = File.OpenText(Settings.游戏数据目录 + "\\System\\商城福利\\每日签到.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				每日签到 每日签到2;
				每日签到2 = new 每日签到();
				每日签到2.签到天数 = byte.Parse(item["Day"].ToString());
				每日签到2.签到类型 = byte.Parse(item["Type"].ToString());
				每日签到2.限制等级 = byte.Parse(item["LevelLimit"].ToString());
				每日签到2.奖励物品 = int.Parse(item["AwardItemID"].ToString());
				每日签到2.奖励数量 = int.Parse(item["AwardItemCount"].ToString());
				每日签到.数据表.Add(每日签到2);
			}
		}
	}
}
