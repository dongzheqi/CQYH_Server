using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 月卡奖励
	{
		public static Dictionary<byte, 月卡奖励> 数据表;

		public byte 模板编号;

		public int 时间类型;

		public int 开卡费用;

		public int 持续时间;

		public int[] 奖励物品;

		public int[] 奖励数量;

		public static void 载入数据()
		{
			月卡奖励.数据表 = new Dictionary<byte, 月卡奖励>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = File.OpenText(Settings.游戏数据目录 + "\\System\\商城福利\\月卡奖励.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				月卡奖励 月卡奖励2;
				月卡奖励2 = new 月卡奖励();
				月卡奖励2.模板编号 = byte.Parse(item["CardType"].ToString());
				月卡奖励2.奖励物品 = new int[28];
				月卡奖励2.奖励数量 = new int[28];
				月卡奖励2.时间类型 = int.Parse(item["CardTimeType"].ToString());
				月卡奖励2.开卡费用 = int.Parse(item["Cost"].ToString());
				月卡奖励2.持续时间 = int.Parse(item["Time"].ToString());
				for (int i = 1; i < 29; i++)
				{
					月卡奖励2.奖励物品[i - 1] = int.Parse(item["RewardItemID" + i].ToString());
					月卡奖励2.奖励数量[i - 1] = int.Parse(item["RewardItemCount" + i].ToString());
				}
				月卡奖励.数据表.Add(月卡奖励2.模板编号, 月卡奖励2);
			}
		}
	}
}
