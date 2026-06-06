using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 灵石配置
	{
		public static Dictionary<int, 灵石配置> 数据表;

		public int 模板编号;

		public int 属性编号;

		public int 匹配孔洞;

		public int 灵石等级;

		public int 移除花费;

		public int 升级编号;

		public int 升级数量;

		public int 升级价格;

		public int 升级概率;

		public int 幸运物品;

		public int 添加概率;

		public int 特殊匹配;

		public static void 载入数据()
		{
			灵石配置.数据表 = new Dictionary<int, 灵石配置>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = 配置读取.打开(Settings.游戏数据目录 + "\\System\\装备养成\\灵石配置.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				灵石配置 灵石配置2;
				灵石配置2 = new 灵石配置();
				灵石配置2.模板编号 = int.Parse(item["TemplateID"].ToString());
				灵石配置2.属性编号 = int.Parse(item["AttrID"].ToString());
				灵石配置2.匹配孔洞 = int.Parse(item["MatchSocket"].ToString());
				灵石配置2.灵石等级 = int.Parse(item["SoulLevel"].ToString());
				灵石配置2.移除花费 = int.Parse(item["RemovePrice"].ToString());
				灵石配置2.升级编号 = int.Parse(item["UpgradeSoulID"].ToString());
				灵石配置2.升级数量 = int.Parse(item["UpgradeSoulCount"].ToString());
				灵石配置2.升级价格 = int.Parse(item["UpgradePrice"].ToString());
				灵石配置2.升级概率 = int.Parse(item["UpgradeSuccessRate"].ToString());
				灵石配置2.幸运物品 = int.Parse(item["LuckItemID"].ToString());
				灵石配置2.添加概率 = int.Parse(item["LuckItemSuccessRate"].ToString());
				灵石配置2.特殊匹配 = int.Parse(item["SpecialMatchID"].ToString());
				灵石配置.数据表.Add(灵石配置2.模板编号, 灵石配置2);
			}
		}
	}
}
