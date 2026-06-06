using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 传送法阵
	{
		public static List<传送法阵> 数据表;

		public byte 法阵编号;

		public byte 所处地图;

		public byte 跳转地图;

		public string 法阵名字;

		public string 所处地名;

		public string 跳转地名;

		public string 所处别名;

		public string 跳转别名;

		public Point 所处坐标;

		public Point 跳转坐标;

		public static void 载入数据()
		{
			传送法阵.数据表 = new List<传送法阵>();
			string path;
			path = Settings.游戏数据目录 + "\\System\\世界其他\\传送法阵.csv";
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
					if (游戏地图.检索表.TryGetValue(item["FromMap"].ToString(), out var value) && 游戏地图.检索表.TryGetValue(item["ToMap"].ToString(), out var value2))
					{
						传送法阵 传送法阵2;
						传送法阵2 = new 传送法阵();
						传送法阵2.法阵编号 = byte.Parse(item["SrcId"].ToString());
						传送法阵2.所处地图 = value.地图编号;
						传送法阵2.所处别名 = value.地图别名;
						传送法阵2.所处地名 = value.地图名字;
						传送法阵2.所处坐标 = 计算类.游戏坐标转点阵坐标(new PointF(float.Parse(item["FromX"].ToString()), float.Parse(item["FromY"].ToString())));
						传送法阵2.跳转地图 = value2.地图编号;
						传送法阵2.跳转别名 = value2.地图别名;
						传送法阵2.跳转地名 = value2.地图名字;
						传送法阵2.跳转坐标 = 计算类.游戏坐标转点阵坐标(new PointF(float.Parse(item["ToX"].ToString()), float.Parse(item["ToY"].ToString())));
						传送法阵.数据表.Add(传送法阵2);
					}
				}
				return;
			}
			string text;
			text = Settings.游戏数据目录 + "\\System\\游戏地图\\法阵数据\\";
			if (Directory.Exists(text))
			{
				object[] array;
				array = 序列化类.反序列化(text, typeof(传送法阵));
				foreach (object obj in array)
				{
					传送法阵.数据表.Add((传送法阵)obj);
				}
			}
		}
	}
}
