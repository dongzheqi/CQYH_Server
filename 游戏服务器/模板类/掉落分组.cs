using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 掉落分组
	{
		public static Dictionary<int, 掉落分组> 数据表;

		public int 分组编号;

		public bool 是否开启;

		public int 物品编号;

		public int 物品重量;

		public int 最小数量;

		public int 最大数量;

		public int 适用职业;

		public static void 载入数据()
		{
			掉落分组.数据表 = new Dictionary<int, 掉落分组>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = File.OpenText(Settings.游戏数据目录 + "\\System\\世界其他\\掉落分组.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				掉落分组 掉落分组2;
				掉落分组2 = new 掉落分组();
				掉落分组2.分组编号 = int.Parse(item["DropGroupID"].ToString());
				掉落分组2.是否开启 = int.Parse(item["Enabled"].ToString()) == 1;
				掉落分组2.物品编号 = int.Parse(item["Item"].ToString());
				掉落分组2.物品重量 = int.Parse(item["Weight"].ToString());
				掉落分组2.最小数量 = int.Parse(item["CountMin"].ToString());
				掉落分组2.最大数量 = int.Parse(item["CountMax"].ToString());
				掉落分组2.适用职业 = int.Parse(item["FitClass"].ToString());
				掉落分组.数据表.TryAdd(掉落分组2.分组编号, 掉落分组2);
			}
		}
	}
}
