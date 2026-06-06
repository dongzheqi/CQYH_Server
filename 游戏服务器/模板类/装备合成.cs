using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 装备合成
	{
		public static Dictionary<int, 装备合成> 数据表;

		public int 模板编号;

		public bool 是否启用;

		public int 特殊标志;

		public int 合成物品;

		public int 成功概率;

		public int 物品类型;

		public int 需要金币;

		public 合成需要物品[] 合成材料;

		public static void 载入数据()
		{
			装备合成.数据表 = new Dictionary<int, 装备合成>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = 配置读取.打开(Settings.游戏数据目录 + "\\System\\装备养成\\装备合成.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				装备合成 装备合成2;
				装备合成2 = new 装备合成();
				装备合成2.模板编号 = int.Parse(item["FormulaID"].ToString());
				装备合成2.是否启用 = int.Parse(item["Enabled"].ToString()) == 1;
				装备合成2.特殊标志 = int.Parse(item["Flag"].ToString());
				装备合成2.合成物品 = int.Parse(item["ResultItemID"].ToString());
				装备合成2.成功概率 = int.Parse(item["SuccRate"].ToString());
				装备合成2.物品类型 = int.Parse(item["ItemType"].ToString());
				装备合成2.合成材料 = new 合成需要物品[8];
				for (int i = 1; i < 9; i++)
				{
					string[] array;
					array = item["AltItemID" + i + "1"].ToString().Split(';');
					int[] array2;
					array2 = new int[array.Length + 1];
					array2[0] = int.Parse(item["ItemID" + i].ToString());
					for (int j = 0; j < array.Length; j++)
					{
						array2[j + 1] = int.Parse(array[j]);
					}
					装备合成2.合成材料[i - 1] = new 合成需要物品
					{
						替换物品一 = array2,
						替换物品二 = int.Parse(item["AltItemID" + i + "2"].ToString()),
						替换物品三 = int.Parse(item["AltItemID" + i + "3"].ToString()),
						替换物品四 = int.Parse(item["AltItemID" + i + "4"].ToString()),
						需要数量 = int.Parse(item["ItemCount" + i].ToString())
					};
				}
				装备合成2.需要金币 = int.Parse(item["Money"].ToString());
				装备合成.数据表.Add(装备合成2.模板编号, 装备合成2);
			}
		}
	}
}
