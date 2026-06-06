using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public sealed class 主题礼包商品模板
	{
		public static Dictionary<int, int> 数据表;

		public int 物品编号;

		public int 物品数量;

		public static void 载入数据()
		{
			主题礼包商品模板.数据表 = new Dictionary<int, int>();
			string text;
			text = Settings.游戏数据目录 + "\\System\\";
			if (!Directory.Exists(text))
			{
				return;
			}
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			CsvReader csvReader;
			csvReader = new CsvReader(new StreamReader(text + "商城福利\\主题礼包商品.csv", Encoding.GetEncoding("GB18030")), CultureInfo.InvariantCulture);
			csvReader.Read();
			csvReader.ReadHeader();
			try
			{
				while (csvReader.Read())
				{
					主题礼包商品模板 主题礼包商品模板2;
					主题礼包商品模板2 = new 主题礼包商品模板
					{
						物品编号 = csvReader.GetField<int>("物品编号"),
						物品数量 = csvReader.GetField<int>("物品数量")
					};
					主题礼包商品模板.数据表.Add(主题礼包商品模板2.物品编号, 主题礼包商品模板2.物品数量);
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
