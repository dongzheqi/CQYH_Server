using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 锻石炼药
	{
		public static Dictionary<int, 锻石炼药> 数据表;

		public int 模板编号;

		public ushort 需要等级;

		public ushort 模版类型;

		public ushort 限定职业;

		public ushort 检测技能;

		public int 奖励道具;

		public ushort 奖励数量;

		public ushort 额外概率增加;

		public int 基础材料一编号;

		public int 基础材料一概率;

		public int 基础材料二编号;

		public int 基础材料二概率;

		public int 基础材料数量;

		public int 需要金币;

		public int 额外材料一编号;

		public int 额外材料一数量;

		public int 额外材料二编号;

		public int 额外材料二数量;

		public static void 载入数据()
		{
			锻石炼药.数据表 = new Dictionary<int, 锻石炼药>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = 配置读取.打开(Settings.游戏数据目录 + "\\System\\装备养成\\锻石炼药.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				锻石炼药 锻石炼药2;
				锻石炼药2 = new 锻石炼药();
				锻石炼药2.模板编号 = ushort.Parse(item["ID"].ToString());
				锻石炼药2.需要等级 = ushort.Parse(item["Level"].ToString());
				锻石炼药2.模版类型 = ushort.Parse(item["Type"].ToString());
				锻石炼药2.限定职业 = ushort.Parse(item["Occupation"].ToString());
				锻石炼药2.检测技能 = ushort.Parse(item["SpellID"].ToString());
				锻石炼药2.奖励道具 = int.Parse(item["DisplayID"].ToString());
				锻石炼药2.奖励数量 = ushort.Parse(item["ResultCount"].ToString());
				锻石炼药2.额外概率增加 = ushort.Parse(item["ExPropAdd"].ToString());
				锻石炼药2.基础材料一编号 = int.Parse(item["ItemRequired1"].ToString());
				锻石炼药2.基础材料一概率 = int.Parse(item["ItemRequiredProb1"].ToString());
				锻石炼药2.基础材料二编号 = int.Parse(item["ItemRequired2"].ToString());
				锻石炼药2.基础材料二概率 = int.Parse(item["ItemRequiredProb2"].ToString());
				锻石炼药2.基础材料数量 = int.Parse(item["ItemRequiredCount"].ToString());
				锻石炼药2.需要金币 = int.Parse(item["Money"].ToString());
				锻石炼药2.额外材料一编号 = int.Parse(item["Item1"].ToString());
				锻石炼药2.额外材料一数量 = int.Parse(item["Count1"].ToString());
				锻石炼药2.额外材料二编号 = int.Parse(item["Item2"].ToString());
				锻石炼药2.额外材料二数量 = int.Parse(item["Count2"].ToString());
				锻石炼药.数据表.Add(锻石炼药2.模板编号, 锻石炼药2);
			}
		}
	}
}
