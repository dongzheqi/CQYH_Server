using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 技能进度
	{
		public static Dictionary<byte, 技能进度> 数据表;

		public static void 载入数据()
		{
			技能进度.数据表 = new Dictionary<byte, 技能进度>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = 配置读取.打开(Settings.游戏数据目录 + "\\System\\技能进度.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				ushort num;
				num = ushort.Parse(item["SpellID"].ToString());
				ushort.Parse(item["RuneSlot"].ToString());
				byte b;
				b = byte.Parse(item["Level"].ToString());
				foreach (KeyValuePair<ushort, 铭文技能> item2 in 铭文技能.数据表)
				{
					if (item2.Value.技能编号 == num)
					{
						if (b == 0)
						{
							item2.Value.技能战力加成 = new int[1] { int.Parse(item["Pow"].ToString()) };
							item2.Value.需要技能经验 = new int[1] { int.Parse(item["ConsumePts"].ToString()) };
							item2.Value.需要角色等级 = new byte[1] { byte.Parse(item["RequireLevel"].ToString()) };
						}
						int[] array;
						array = new int[b + 1];
						for (int i = 0; i < item2.Value.技能战力加成.Length; i++)
						{
							array[i] = item2.Value.技能战力加成[i];
						}
						array[b] = int.Parse(item["Pow"].ToString());
						item2.Value.技能战力加成 = array;
						int[] array2;
						array2 = new int[b + 1];
						for (int j = 0; j < item2.Value.需要技能经验.Length; j++)
						{
							array2[j] = item2.Value.需要技能经验[j];
						}
						array2[b] = int.Parse(item["ConsumePts"].ToString());
						item2.Value.需要技能经验 = array2;
						byte[] array3;
						array3 = new byte[b + 1];
						for (int k = 0; k < item2.Value.需要角色等级.Length; k++)
						{
							array3[k] = item2.Value.需要角色等级[k];
						}
						array3[b] = byte.Parse(item["RequireLevel"].ToString());
						item2.Value.需要角色等级 = array3;
					}
				}
			}
		}
	}
}
