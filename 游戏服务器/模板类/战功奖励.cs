using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 战功奖励
	{
		public static Dictionary<byte, 战功奖励> 数据表;

		public byte 模板编号;

		public byte 需要等级;

		public bool 检测战令;

		public ushort 需要点数;

		public int 物品编号;

		public ushort 物品数量;

		public static void 载入数据()
		{
			战功奖励.数据表 = new Dictionary<byte, 战功奖励>();
			DataTable dataTable;
			dataTable = new DataTable();
			using StreamReader reader = File.OpenText(Settings.游戏数据目录 + "\\System\\任务成就\\战功奖励.csv");
			using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				战功奖励 战功奖励2;
				战功奖励2 = new 战功奖励();
				战功奖励2.模板编号 = byte.Parse(item["ID"].ToString());
				战功奖励2.需要等级 = byte.Parse(item["Level"].ToString());
				战功奖励2.检测战令 = byte.Parse(item["CheckToken"].ToString()) == 1;
				战功奖励2.需要点数 = ushort.Parse(item["ScoreRequired"].ToString());
				战功奖励2.物品编号 = int.Parse(item["ItemID"].ToString());
				战功奖励2.物品数量 = ushort.Parse(item["ItemCount"].ToString());
				战功奖励.数据表.Add(战功奖励2.模板编号, 战功奖励2);
			}
		}
	}
}
