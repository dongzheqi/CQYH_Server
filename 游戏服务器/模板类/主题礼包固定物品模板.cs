using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public sealed class 主题礼包固定物品模板
	{
		public static Dictionary<int, Dictionary<int, int>> 数据表;

		public int 星期;

		public int 物品1编号;

		public int 物品1数量;

		public int 物品1绑定;

		public int 物品2编号;

		public int 物品2数量;

		public int 物品2绑定;

		public int 物品3编号;

		public int 物品3数量;

		public int 物品3绑定;

		public int 物品4编号;

		public int 物品4数量;

		public int 物品4绑定;

		public static void 载入数据()
		{
			主题礼包固定物品模板.数据表 = new Dictionary<int, Dictionary<int, int>>();
			string text;
			text = Settings.游戏数据目录 + "\\System\\";
			if (!Directory.Exists(text))
			{
				return;
			}
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
			CsvReader csvReader;
			csvReader = new CsvReader(配置读取.打开(text + "商城福利\\主题礼包固定物品.csv"), CultureInfo.InvariantCulture);
			csvReader.Read();
			csvReader.ReadHeader();
			try
			{
				while (csvReader.Read())
				{
					主题礼包固定物品模板 主题礼包固定物品模板2;
					主题礼包固定物品模板2 = new 主题礼包固定物品模板
					{
						星期 = csvReader.GetField<int>("星期"),
						物品1编号 = csvReader.GetField<int>("物品1编号"),
						物品1数量 = csvReader.GetField<int>("物品1数量"),
						物品1绑定 = csvReader.GetField<int>("物品1绑定"),
						物品2编号 = csvReader.GetField<int>("物品2编号"),
						物品2数量 = csvReader.GetField<int>("物品2数量"),
						物品2绑定 = csvReader.GetField<int>("物品2绑定"),
						物品3编号 = csvReader.GetField<int>("物品3编号"),
						物品3数量 = csvReader.GetField<int>("物品3数量"),
						物品3绑定 = csvReader.GetField<int>("物品3绑定"),
						物品4编号 = csvReader.GetField<int>("物品4编号"),
						物品4数量 = csvReader.GetField<int>("物品4数量"),
						物品4绑定 = csvReader.GetField<int>("物品4绑定")
					};
					Dictionary<int, int> dictionary;
					dictionary = new Dictionary<int, int>();
					if (主题礼包固定物品模板2.物品1编号 != 0)
					{
						dictionary.Add(主题礼包固定物品模板2.物品1编号, 主题礼包固定物品模板2.物品1数量);
					}
					if (主题礼包固定物品模板2.物品2编号 != 0)
					{
						dictionary.Add(主题礼包固定物品模板2.物品2编号, 主题礼包固定物品模板2.物品2数量);
					}
					if (主题礼包固定物品模板2.物品3编号 != 0)
					{
						dictionary.Add(主题礼包固定物品模板2.物品3编号, 主题礼包固定物品模板2.物品3数量);
					}
					if (主题礼包固定物品模板2.物品4编号 != 0)
					{
						dictionary.Add(主题礼包固定物品模板2.物品4编号, 主题礼包固定物品模板2.物品4数量);
					}
					主题礼包固定物品模板.数据表.Add(主题礼包固定物品模板2.星期, dictionary);
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
