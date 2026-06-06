using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 精炼阶段
	{
		public static Dictionary<阶段判断, 精炼阶段> 数据表;

		public int 阶段;

		public int 最小值;

		public int 最大值;

		public 游戏对象职业 职业;

		public 精炼阶段属性 升级属性一;

		public 精炼阶段属性 升级属性二;

		public Dictionary<游戏对象属性, int> 属性值;

		public static void 载入数据()
		{
			精炼阶段.数据表 = new Dictionary<阶段判断, 精炼阶段>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = File.OpenText(Settings.游戏数据目录 + "\\System\\装备养成\\精炼阶段.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				精炼阶段 精炼阶段2;
				精炼阶段2 = new 精炼阶段();
				精炼阶段2.阶段 = int.Parse(item["Level"].ToString());
				精炼阶段2.最小值 = int.Parse(item["Power"].ToString());
				精炼阶段2.最大值 = int.Parse(item["PowerMax"].ToString());
				精炼阶段2.职业 = 计算类.字符转职业(item["Class"].ToString());
				精炼阶段2.升级属性一.属性 = 计算类.字符转属性(item["AttrID1"].ToString());
				精炼阶段2.升级属性一.数值 = int.Parse(item["Value1"].ToString());
				精炼阶段2.升级属性二.属性 = 计算类.字符转属性(item["AttrID2"].ToString());
				精炼阶段2.升级属性二.数值 = int.Parse(item["Value2"].ToString());
				精炼阶段2.属性值 = new Dictionary<游戏对象属性, int>();
				if (精炼阶段2.升级属性一.数值 > 0)
				{
					精炼阶段2.属性值.Add(精炼阶段2.升级属性一.属性, 精炼阶段2.升级属性一.数值);
				}
				if (精炼阶段2.升级属性二.数值 > 0)
				{
					精炼阶段2.属性值.Add(精炼阶段2.升级属性二.属性, 精炼阶段2.升级属性二.数值);
				}
				精炼阶段.数据表.Add(new 阶段判断
				{
					职业 = 精炼阶段2.职业,
					阶段 = 精炼阶段2.阶段
				}, 精炼阶段2);
			}
		}
	}
}
