using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 游戏威望
	{
		public static Dictionary<byte, 游戏威望> 数据表;

		public byte 模板编号;

		public short 初始数值;

		public bool 是否可用;

		public short 最大数值;

		public Dictionary<游戏对象职业, List<奖励物品>> 奖励物品;

		public static void 载入数据()
		{
			游戏威望.数据表 = new Dictionary<byte, 游戏威望>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = File.OpenText(Settings.游戏数据目录 + "\\System\\玩家成长\\游戏威望.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				游戏威望 游戏威望2;
				游戏威望2 = new 游戏威望();
				游戏威望2.模板编号 = byte.Parse(item["ForceID"].ToString());
				游戏威望2.初始数值 = short.Parse(item["InitValue"].ToString());
				游戏威望2.是否可用 = int.Parse(item["Active"].ToString()) == 1;
				游戏威望2.最大数值 = short.Parse(item["MaxValue"].ToString());
				游戏威望2.奖励物品 = new Dictionary<游戏对象职业, List<奖励物品>>();
				List<奖励物品> list;
				list = new List<奖励物品>();
				List<奖励物品> list2;
				list2 = new List<奖励物品>();
				List<奖励物品> list3;
				list3 = new List<奖励物品>();
				List<奖励物品> list4;
				list4 = new List<奖励物品>();
				List<奖励物品> list5;
				list5 = new List<奖励物品>();
				List<奖励物品> list6;
				list6 = new List<奖励物品>();
				for (int i = 0; i < 8; i++)
				{
					if (int.Parse(item[i * 2 + 6].ToString()) > 0 && int.Parse(item[i * 2 + 7].ToString()) > 0)
					{
						list.Add(new 奖励物品
						{
							物品编号 = int.Parse(item[i * 2 + 6].ToString()),
							物品数量 = int.Parse(item[i * 2 + 7].ToString())
						});
					}
					if (int.Parse(item[i * 2 + 22].ToString()) > 0 && int.Parse(item[i * 2 + 23].ToString()) > 0)
					{
						list2.Add(new 奖励物品
						{
							物品编号 = int.Parse(item[i * 2 + 22].ToString()),
							物品数量 = int.Parse(item[i * 2 + 23].ToString())
						});
					}
					if (int.Parse(item[i * 2 + 38].ToString()) > 0 && int.Parse(item[i * 2 + 39].ToString()) > 0)
					{
						list3.Add(new 奖励物品
						{
							物品编号 = int.Parse(item[i * 2 + 38].ToString()),
							物品数量 = int.Parse(item[i * 2 + 39].ToString())
						});
					}
					if (int.Parse(item[i * 2 + 54].ToString()) > 0 && int.Parse(item[i * 2 + 55].ToString()) > 0)
					{
						list4.Add(new 奖励物品
						{
							物品编号 = int.Parse(item[i * 2 + 54].ToString()),
							物品数量 = int.Parse(item[i * 2 + 55].ToString())
						});
					}
					if (int.Parse(item[i * 2 + 70].ToString()) > 0 && int.Parse(item[i * 2 + 71].ToString()) > 0)
					{
						list5.Add(new 奖励物品
						{
							物品编号 = int.Parse(item[i * 2 + 70].ToString()),
							物品数量 = int.Parse(item[i * 2 + 71].ToString())
						});
					}
					if (int.Parse(item[i * 2 + 86].ToString()) > 0 && int.Parse(item[i * 2 + 87].ToString()) > 0)
					{
						list6.Add(new 奖励物品
						{
							物品编号 = int.Parse(item[i * 2 + 86].ToString()),
							物品数量 = int.Parse(item[i * 2 + 87].ToString())
						});
					}
				}
				游戏威望2.奖励物品.Add(游戏对象职业.战士, list);
				游戏威望2.奖励物品.Add(游戏对象职业.法师, list2);
				游戏威望2.奖励物品.Add(游戏对象职业.刺客, list3);
				游戏威望2.奖励物品.Add(游戏对象职业.弓手, list4);
				游戏威望2.奖励物品.Add(游戏对象职业.道士, list5);
				游戏威望2.奖励物品.Add(游戏对象职业.龙枪, list6);
				游戏威望.数据表.Add(游戏威望2.模板编号, 游戏威望2);
			}
		}
	}
}
