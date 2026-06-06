using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 物品分解
	{
		public static Dictionary<int, 物品分解> 数据表;

		public int 物品模版;

		public int 掉落概率一;

		public int 掉落分组一;

		public int 掉落概率二;

		public int 掉落分组二;

		public int 掉落概率三;

		public int 掉落分组三;

		public int 触发脚本;

		public static void 载入数据()
		{
			物品分解.数据表 = new Dictionary<int, 物品分解>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = 配置读取.打开(Settings.游戏数据目录 + "\\System\\装备养成\\物品分解.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				物品分解 物品分解2;
				物品分解2 = new 物品分解();
				物品分解2.物品模版 = int.Parse(item["ItemTemplateID"].ToString());
				物品分解2.掉落概率一 = int.Parse(item["DropChance1"].ToString());
				物品分解2.掉落分组一 = int.Parse(item["DropGroupID1"].ToString());
				物品分解2.掉落概率二 = int.Parse(item["DropChance2"].ToString());
				物品分解2.掉落分组二 = int.Parse(item["DropGroupID2"].ToString());
				物品分解2.掉落概率三 = int.Parse(item["DropChance3"].ToString());
				物品分解2.掉落分组三 = int.Parse(item["DropGroupID3"].ToString());
				物品分解2.触发脚本 = int.Parse(item["Script"].ToString());
				物品分解.数据表.Add(物品分解2.物品模版, 物品分解2);
			}
		}
	}
}
