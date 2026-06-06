using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 装备神佑消耗
	{
		public Dictionary<string, int> 数量配置V2 = new Dictionary<string, int> { ["装备套装_升级次数_幸运等级_孔洞颜色"] = 1 };

		public static Dictionary<string, int> 数据表 = new Dictionary<string, int>();

		public static 装备神佑消耗 配置;

		public static bool 读取中;

		public 装备神佑消耗()
		{
			string path;
			path = Settings.游戏数据目录 + "\\System\\装备养成\\装备神佑消耗.csv";
			装备神佑消耗.数据表.Clear();
			if (File.Exists(path))
			{
				DataTable dataTable;
				dataTable = new DataTable();
				using StreamReader reader = 配置读取.打开(path);
				using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
				{
					using CsvDataReader reader2 = new CsvDataReader(csv);
					dataTable.Load(reader2);
				}
				foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
				{
					int value;
					value = int.Parse(item["Value"].ToString());
					装备神佑消耗.数据表.Add(string.Format("{0}_{1}_{2}_{3}", item["Type"].ToString(), item["Up"].ToString(), item["Luck"].ToString(), item["Hold"].ToString()), value);
				}
				return;
			}
			装备神佑消耗.配置 = this;
			装备神佑消耗.数据表 = 装备神佑消耗.配置.数量配置V2;
			装备神佑消耗.保存数据();
		}

		public static void 载入数据()
		{
			装备神佑消耗.配置 = new 装备神佑消耗();
		}

		public static int 获取数量(string k, int def)
		{
			if (!装备神佑消耗.数据表.TryGetValue(k, out var value))
			{
				装备神佑消耗.数据表.Add(k, def);
				装备神佑消耗.保存数据();
			}
			else
			{
				def = value;
			}
			return def;
		}

		public static void 保存数据()
		{
			using StreamWriter writer = new StreamWriter(Settings.游戏数据目录 + "\\System\\装备养成\\装备神佑消耗.csv");
			using CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
			csvWriter.WriteHeader<神佑CsvColumns>();
			csvWriter.NextRecord();
			foreach (KeyValuePair<string, int> item in 装备神佑消耗.数据表)
			{
				string[] array;
				array = item.Key.Split('_');
				csvWriter.WriteRecord(new 神佑CsvColumns
				{
					Value = item.Value,
					Type = array[0],
					Up = array[1],
					Luck = array[2],
					Hold = array[3]
				});
				csvWriter.NextRecord();
			}
		}
	}
}
