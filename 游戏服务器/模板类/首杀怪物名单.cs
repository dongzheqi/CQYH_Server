using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	// 首杀统计「额外纳入」名单(CSV 可配): 名单内的怪物编号即便 怪物级别 没标成 精英干将/头目首领, 也纳入全服首杀统计.
	// 与 怪物实例 的原级别判定(精英干将 / 头目首领 / 刷新通知)取并集——解决「名字是头目/精英、模板级别却被标成普通/巨型」的怪打了不算首杀的问题.
	// 文件: System\世界其他\首杀怪物名单.csv, 表头首列须为「怪物编号」(其余列如备注仅供人读、忽略). 文件缺失或无数据行即空名单, 行为回退到纯级别判定(零影响).
	// 改动后需重启服务器或执行 @重载模板 生效.
	public class 首杀怪物名单
	{
		public static HashSet<int> 数据表 = new HashSet<int>();

		public static void 载入数据()
		{
			HashSet<int> 集合 = new HashSet<int>();
			string 路径 = Settings.游戏数据目录 + "\\System\\世界其他\\首杀怪物名单.csv";
			if (File.Exists(路径))
			{
				DataTable dataTable = new DataTable();
				using (StreamReader reader = 配置读取.打开(路径))
				using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
				using (CsvDataReader reader2 = new CsvDataReader(csv))
				{
					dataTable.Load(reader2);
				}
				foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
				{
					string text = item["怪物编号"].ToString().Trim();
					if (text.Length != 0 && int.TryParse(text, out int 编号))
					{
						集合.Add(编号);
					}
				}
			}
			首杀怪物名单.数据表 = 集合;
		}
	}
}
