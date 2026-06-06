using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 装备升级
	{
		public static Dictionary<升级装备, 装备升级> 数据表;

		public int 装备编号;

		public byte 升级等级;

		public 装备升级_物品数量 需要物品一;

		public 装备升级_物品数量 需要物品二;

		public 装备升级_物品数量 需要物品三;

		public 装备升级_物品数量 需要物品四;

		public int 需要金币;

		public ushort 出现概率;

		public 装备升级_升级属性 升级属性一;

		public 装备升级_升级属性 升级属性二;

		public static void 载入数据()
		{
			装备升级.数据表 = new Dictionary<升级装备, 装备升级>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = File.OpenText(Settings.游戏数据目录 + "\\System\\装备养成\\装备升级.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				装备升级 装备升级2;
				装备升级2 = new 装备升级();
				装备升级2.装备编号 = int.Parse(item["EquipID"].ToString());
				装备升级2.升级等级 = byte.Parse(item["LevelupTimeLimit"].ToString());
				装备升级2.需要物品一.编号 = int.Parse(item["ItemCostID"].ToString());
				装备升级2.需要物品一.数量 = int.Parse(item["ItemCostCount"].ToString());
				装备升级2.需要物品二.编号 = int.Parse(item["ItemCostID2"].ToString());
				装备升级2.需要物品二.数量 = int.Parse(item["ItemCostCount2"].ToString());
				装备升级2.需要物品三.编号 = int.Parse(item["ItemCostID3"].ToString());
				装备升级2.需要物品三.数量 = int.Parse(item["ItemCostCount3"].ToString());
				装备升级2.需要物品四.编号 = int.Parse(item["ItemCostID4"].ToString());
				装备升级2.需要物品四.数量 = int.Parse(item["ItemCostCount4"].ToString());
				装备升级2.需要金币 = int.Parse(item["MoneyCostCount"].ToString());
				装备升级2.出现概率 = ushort.Parse(item["Prob"].ToString());
				装备升级2.升级属性一.属性 = 计算类.字符转属性(item["Attr1"].ToString());
				装备升级2.升级属性一.数值 = int.Parse(item["AttrValue1"].ToString());
				装备升级2.升级属性一.战力 = int.Parse(item["AttrPower1"].ToString());
				装备升级2.升级属性二.属性 = 计算类.字符转属性(item["Attr2"].ToString());
				装备升级2.升级属性二.数值 = int.Parse(item["AttrValue2"].ToString());
				装备升级2.升级属性二.战力 = int.Parse(item["AttrPower2"].ToString());
				装备升级.数据表.Add(new 升级装备
				{
					装备编号 = 装备升级2.装备编号,
					升级等级 = 装备升级2.升级等级
				}, 装备升级2);
			}
		}
	}
}
