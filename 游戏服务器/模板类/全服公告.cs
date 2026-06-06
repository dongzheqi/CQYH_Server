using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using 游戏服务器.网络类;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 全服公告
	{
		private static Dictionary<int, 全服公告> 数据表 = new Dictionary<int, 全服公告>();

		private int 编号;

		public string 内容 { get; set; }

		public static void 保存数据()
		{
			using StreamWriter writer = new StreamWriter(Settings.游戏数据目录 + "\\System\\公告\\全服公告.csv");
			using CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
			csvWriter.WriteHeader<全服公告csvColumns>();
			csvWriter.NextRecord();
			foreach (KeyValuePair<int, 全服公告> item in 全服公告.数据表)
			{
				csvWriter.WriteRecord(new 全服公告csvColumns
				{
					Id = item.Value.编号,
					Key = item.Value.内容
				});
				csvWriter.NextRecord();
			}
		}

		public static void 发送(int A编号, string Item, string Play, string Src)
		{
			if (A编号 >= 1 && 全服公告.数据表.TryGetValue(A编号, out var value))
			{
				网络服务网关.发送公告(value.内容.Replace("%Play%", Play).Replace("%Item%", Item).Replace("%Src%", Src));
			}
		}

		public static void 载入数据()
		{
			string path;
			path = Settings.游戏数据目录 + "\\System\\公告\\全服公告.csv";
			全服公告.数据表.Clear();
			if (File.Exists(path))
			{
				DataTable dataTable;
				dataTable = new DataTable();
				using StreamReader reader = File.OpenText(path);
				using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
				{
					using CsvDataReader reader2 = new CsvDataReader(csv);
					dataTable.Load(reader2);
				}
				foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
				{
					int key;
					key = int.Parse(item["Id"].ToString());
					全服公告.数据表.Add(key, new 全服公告
					{
						编号 = key,
						内容 = item["Key"].ToString()
					});
				}
				return;
			}
			string text;
			text = Settings.游戏数据目录 + "\\System\\全服公告\\";
			if (Directory.Exists(text))
			{
				object[] array;
				array = 序列化类.反序列化(text, typeof(全服公告));
				for (int i = 0; i < array.Length; i++)
				{
					全服公告 全服公告2;
					全服公告2 = (全服公告)array[i];
					全服公告.数据表.Add(全服公告2.编号, 全服公告2);
				}
			}
		}
	}
}
