using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 合成公式
	{
		public static Dictionary<byte, 合成公式> 数据表;

		public byte 模板编号;

		public int 物品编号一;

		public int 物品数量一;

		public int 物品编号二;

		public int 物品数量二;

		public int 物品编号三;

		public int 物品数量三;

		public int 物品编号四;

		public int 物品数量四;

		public int 获得物品;

		public int 花费金币;

		public static void 载入数据()
		{
			合成公式.数据表 = new Dictionary<byte, 合成公式>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = 配置读取.打开(Settings.游戏数据目录 + "\\System\\装备养成\\合成公式.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				合成公式 合成公式2;
				合成公式2 = new 合成公式();
				合成公式2.模板编号 = byte.Parse(item["FormulaID"].ToString());
				合成公式2.物品编号一 = int.Parse(item["ItemID1"].ToString());
				合成公式2.物品数量一 = int.Parse(item["ItemCount1"].ToString());
				合成公式2.物品编号二 = int.Parse(item["ItemID2"].ToString());
				合成公式2.物品数量二 = int.Parse(item["ItemCount2"].ToString());
				合成公式2.物品编号三 = int.Parse(item["ItemID3"].ToString());
				合成公式2.物品数量三 = int.Parse(item["ItemCount3"].ToString());
				合成公式2.物品编号四 = int.Parse(item["ItemID4"].ToString());
				合成公式2.物品数量四 = int.Parse(item["ItemCount4"].ToString());
				合成公式2.获得物品 = int.Parse(item["ResultItem"].ToString());
				合成公式2.花费金币 = int.Parse(item["Money"].ToString());
				合成公式.数据表.Add(合成公式2.模板编号, 合成公式2);
			}
		}
	}
}
