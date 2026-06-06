using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 物品过滤
	{
		public static Dictionary<ushort, 物品过滤> 数据表;

		public ushort 模板编号;

		public List<int> 物品编号;

		public static void 载入数据()
		{
			物品过滤.数据表 = new Dictionary<ushort, 物品过滤>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = 配置读取.打开(Settings.游戏数据目录 + "\\System\\世界其他\\物品过滤.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				物品过滤 物品过滤2;
				物品过滤2 = new 物品过滤();
				物品过滤2.模板编号 = ushort.Parse(item["ID"].ToString());
				物品过滤2.物品编号 = new List<int>();
				string[] array;
				array = item["Items"].ToString().Split(";");
				foreach (string text in array)
				{
					if (text != string.Empty)
					{
						物品过滤2.物品编号.Add(int.Parse(text));
					}
				}
				物品过滤.数据表.Add(物品过滤2.模板编号, 物品过滤2);
			}
		}
	}
}
