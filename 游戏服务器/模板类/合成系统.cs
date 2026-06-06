using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 合成系统
	{
		public static Dictionary<int, 合成系统> 数据表;

		public int 模板编号;

		public int 获得物品;

		public int 双倍概率;

		public int 花费金币;

		public int 物品编号一;

		public int 物品数量一;

		public int 物品编号二;

		public int 物品数量二;

		public int 物品编号三;

		public int 物品数量三;

		public int 物品编号四;

		public int 物品数量四;

		public int 物品编号五;

		public int 物品数量五;

		public int 物品编号六;

		public int 物品数量六;

		public bool 是否广播;

		public static void 载入数据()
		{
			合成系统.数据表 = new Dictionary<int, 合成系统>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = 配置读取.打开(Settings.游戏数据目录 + "\\System\\装备养成\\合成系统.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				合成系统 合成系统2;
				合成系统2 = new 合成系统();
				合成系统2.模板编号 = int.Parse(item["FormulaID"].ToString());
				合成系统2.获得物品 = int.Parse(item["ResultItem"].ToString());
				合成系统2.双倍概率 = int.Parse(item["DoubleRate"].ToString());
				合成系统2.花费金币 = int.Parse(item["Money"].ToString());
				合成系统2.物品编号一 = int.Parse(item["ItemID1"].ToString());
				合成系统2.物品数量一 = int.Parse(item["ItemCount1"].ToString());
				合成系统2.物品编号二 = int.Parse(item["ItemID2"].ToString());
				合成系统2.物品数量二 = int.Parse(item["ItemCount2"].ToString());
				合成系统2.物品编号三 = int.Parse(item["ItemID3"].ToString());
				合成系统2.物品数量三 = int.Parse(item["ItemCount3"].ToString());
				合成系统2.物品编号四 = int.Parse(item["ItemID4"].ToString());
				合成系统2.物品数量四 = int.Parse(item["ItemCount4"].ToString());
				合成系统2.物品编号五 = int.Parse(item["ItemID5"].ToString());
				合成系统2.物品数量五 = int.Parse(item["ItemCount5"].ToString());
				合成系统2.物品编号六 = int.Parse(item["ItemID6"].ToString());
				合成系统2.物品数量六 = int.Parse(item["ItemCount6"].ToString());
				合成系统2.是否广播 = int.Parse(item["BroadCast"].ToString()) == 1;
				合成系统.数据表.Add(合成系统2.模板编号, 合成系统2);
			}
		}
	}
}
