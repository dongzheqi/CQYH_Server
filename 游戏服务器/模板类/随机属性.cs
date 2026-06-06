using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public sealed class 随机属性
	{
		public static Dictionary<int, 随机属性> 数据表;

		public 游戏对象属性 对应属性;

		public int 属性数值;

		public int 属性编号;

		public int 战力加成;

		public string 属性描述;

		public static void 载入数据()
		{
			随机属性.数据表 = new Dictionary<int, 随机属性>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = 配置读取.打开(Settings.游戏数据目录 + "\\System\\物品数据\\随机属性\\attribute.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				随机属性 随机属性2;
				随机属性2 = new 随机属性();
				随机属性2.对应属性 = 计算类.字符转属性(item["Param1"].ToString());
				随机属性2.属性数值 = int.Parse(item["Param2"].ToString());
				随机属性2.属性编号 = int.Parse(item["AttributeID"].ToString());
				随机属性2.战力加成 = int.Parse(item["Pow"].ToString());
				随机属性2.属性描述 = item["Tip"].ToString();
				随机属性.数据表.Add(随机属性2.属性编号, 随机属性2);
			}
		}
	}
}
