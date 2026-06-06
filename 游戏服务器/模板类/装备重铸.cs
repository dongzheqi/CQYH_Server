using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 装备重铸
	{
		public static List<装备重铸> 数据表;

		public static Dictionary<int, float> 武器重铸表;

		public static Dictionary<int, float> 头盔重铸表;

		public static Dictionary<int, float> 衣服重铸表;

		public static Dictionary<int, float> 项链重铸表;

		public static Dictionary<int, float> 戒指重铸表;

		public static Dictionary<int, float> 手镯重铸表;

		public static Dictionary<int, float> 普通技能重铸表;

		public static Dictionary<int, float> 高级技能重铸表;

		public int 装备类型;

		public int 保底节点;

		public int 物品编号;

		public float 重铸概率;

		public static void 载入数据()
		{
			装备重铸.数据表 = new List<装备重铸>();
			装备重铸.武器重铸表 = new Dictionary<int, float>();
			装备重铸.头盔重铸表 = new Dictionary<int, float>();
			装备重铸.衣服重铸表 = new Dictionary<int, float>();
			装备重铸.项链重铸表 = new Dictionary<int, float>();
			装备重铸.戒指重铸表 = new Dictionary<int, float>();
			装备重铸.手镯重铸表 = new Dictionary<int, float>();
			装备重铸.普通技能重铸表 = new Dictionary<int, float>();
			装备重铸.高级技能重铸表 = new Dictionary<int, float>();
			DataTable dataTable;
			dataTable = new DataTable();
			string path;
			path = Settings.游戏数据目录 + "\\System\\装备养成\\装备重铸.csv";
			if (!File.Exists(path))
			{
				return;
			}
			using StreamReader reader = 配置读取.打开(path);
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				装备重铸 装备重铸2;
				装备重铸2 = new 装备重铸();
				装备重铸2.装备类型 = int.Parse(item["EquipType"].ToString());
				装备重铸2.保底节点 = int.Parse(item["Bonus"].ToString());
				装备重铸2.物品编号 = int.Parse(item["ItemID"].ToString());
				装备重铸2.重铸概率 = float.Parse(item["Rate"].ToString());
				装备重铸.数据表.Add(装备重铸2);
				switch (装备重铸2.装备类型)
				{
				case 99:
					装备重铸.高级技能重铸表.Add(装备重铸2.物品编号, 装备重铸2.重铸概率);
					break;
				case 0:
					装备重铸.武器重铸表.Add(装备重铸2.物品编号, 装备重铸2.重铸概率);
					break;
				case 1:
					装备重铸.头盔重铸表.Add(装备重铸2.物品编号, 装备重铸2.重铸概率);
					break;
				case 3:
					装备重铸.衣服重铸表.Add(装备重铸2.物品编号, 装备重铸2.重铸概率);
					break;
				case 8:
					装备重铸.项链重铸表.Add(装备重铸2.物品编号, 装备重铸2.重铸概率);
					break;
				case 9:
					装备重铸.戒指重铸表.Add(装备重铸2.物品编号, 装备重铸2.重铸概率);
					break;
				case 10:
					装备重铸.手镯重铸表.Add(装备重铸2.物品编号, 装备重铸2.重铸概率);
					break;
				case 12:
					装备重铸.普通技能重铸表.Add(装备重铸2.物品编号, 装备重铸2.重铸概率);
					break;
				}
			}
		}
	}
}
