using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using 游戏服务器.网络类;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 系统公告
	{
		public static List<系统公告> 数据表;

		private int _已发送次数;

		private DateTime _已发送时间;

		public DateTime 开始时间 { get; set; }

		public int 间隔时间_秒 { get; set; }

		public int 滚动播报 { get; set; }

		public int 启用 { get; set; }

		public int 发送次数 { get; set; }

		public string 公告内容 { get; set; }

		public int 已发送次数 => this._已发送次数;

		public DateTime 已发送时间 => this._已发送时间;

		public static void 保存数据()
		{
			using StreamWriter writer = new StreamWriter(Settings.游戏数据目录 + "\\System\\公告\\系统公告.csv");
			using CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
			csvWriter.WriteHeader<系统公告csvColumns>();
			csvWriter.NextRecord();
			foreach (系统公告 item in 系统公告.数据表)
			{
				系统公告csvColumns 系统公告csvColumns2;
				系统公告csvColumns2 = new 系统公告csvColumns();
				系统公告csvColumns2.StartTime = item.开始时间;
				系统公告csvColumns2.TimeSec = item.间隔时间_秒;
				系统公告csvColumns2.Roll = item.滚动播报;
				系统公告csvColumns2.Enable = item.启用;
				系统公告csvColumns2.Text = item.公告内容;
				系统公告csvColumns2.Count = item.发送次数;
				csvWriter.WriteRecord(系统公告csvColumns2);
				csvWriter.NextRecord();
			}
		}

		public static void 载入数据()
		{
			系统公告.数据表 = new List<系统公告>();
			string path;
			path = Settings.游戏数据目录 + "\\System\\公告\\系统公告.csv";
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
					系统公告 系统公告2;
					系统公告2 = new 系统公告();
					系统公告2.开始时间 = DateTime.Parse(item["StartTime"].ToString());
					系统公告2.间隔时间_秒 = int.Parse(item["TimeSec"].ToString());
					系统公告2.滚动播报 = int.Parse(item["Roll"].ToString());
					系统公告2.启用 = int.Parse(item["Enable"].ToString());
					系统公告2.公告内容 = item["Text"].ToString();
					if (dataTable.Columns.Contains("Count"))
					{
						系统公告2.发送次数 = int.Parse(item["Count"].ToString());
					}
					系统公告.数据表.Add(系统公告2);
				}
				return;
			}
			string text;
			text = Settings.游戏数据目录 + "\\System\\系统公告\\";
			if (Directory.Exists(text))
			{
				object[] array;
				array = 序列化类.反序列化(text, typeof(系统公告));
				foreach (object obj in array)
				{
					系统公告.数据表.Add((系统公告)obj);
				}
			}
		}

		public 系统公告()
		{
			this.发送次数 = 1;
		}

		public static void 处理数据()
		{
			foreach (系统公告 item in 系统公告.数据表)
			{
				if (item.启用 == 1 && item.已发送次数 < item.发送次数 && 主程.当前时间 >= item.已发送时间)
				{
					item._已发送时间 = 主程.当前时间.AddSeconds(item.间隔时间_秒);
					item._已发送次数++;
					item.发送();
				}
			}
		}

		public void 发送()
		{
			网络服务网关.发送公告(this.公告内容, this.滚动播报 == 1);
		}
	}
}
