using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 每周特惠
	{
		public static List<每周特惠> 数据表;

		public byte 版本节点;

		public int 特惠类型;

		public int 需要元宝;

		public int 附加经验;

		public int 货币类型;

		public int 奖励货币;

		public int[] 奖励物品;

		public int[] 奖励数量;

		public bool[] 是否绑定;

		public static void 载入数据()
		{
			每周特惠.数据表 = new List<每周特惠>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = File.OpenText(Settings.游戏数据目录 + "\\System\\商城福利\\每周特惠.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				每周特惠 每周特惠2;
				每周特惠2 = new 每周特惠();
				每周特惠2.版本节点 = byte.Parse(item["VersionFlag"].ToString());
				每周特惠2.特惠类型 = byte.Parse(item["Type"].ToString());
				每周特惠2.需要元宝 = int.Parse(item["RealMoneyConsume"].ToString());
				每周特惠2.附加经验 = int.Parse(item["ExtraExp"].ToString());
				每周特惠2.货币类型 = int.Parse(item["MoneyType"].ToString());
				每周特惠2.奖励货币 = int.Parse(item["MoneyCnt"].ToString());
				每周特惠2.奖励物品 = new int[10];
				每周特惠2.奖励数量 = new int[10];
				每周特惠2.是否绑定 = new bool[10];
				for (int i = 0; i < 10; i++)
				{
					每周特惠2.奖励物品[i] = int.Parse(item["ItemID" + i].ToString());
					每周特惠2.奖励数量[i] = int.Parse(item["ItemCnt" + i].ToString());
					每周特惠2.是否绑定[i] = int.Parse(item["ForceBinded" + i].ToString()) == 1;
				}
				每周特惠.数据表.Add(每周特惠2);
			}
		}
	}
}
