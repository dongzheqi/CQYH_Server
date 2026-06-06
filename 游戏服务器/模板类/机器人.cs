using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using 游戏服务器.地图类;
using 游戏服务器.数据类;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 机器人
	{
		public static List<机器人> 数据表;

		private static Dictionary<DayOfWeek, byte> DayOfWeekNum = new Dictionary<DayOfWeek, byte>
		{
			[DayOfWeek.Sunday] = 7,
			[DayOfWeek.Monday] = 1,
			[DayOfWeek.Tuesday] = 2,
			[DayOfWeek.Wednesday] = 3,
			[DayOfWeek.Thursday] = 4,
			[DayOfWeek.Friday] = 5,
			[DayOfWeek.Saturday] = 6
		};

		public static 玩家实例 假人;

		private int _已执行次数;

		private DateTime _下次执行时间;

		private short[] _间隔值;

		private DateTime _间隔周年月日时分秒;

		public static string 默认账号名 = "JiaRen9592596";

		public DateTime 开始时间 { get; set; }

		public string 间隔值
		{
			get
			{
				return $" {this._间隔周年月日时分秒.Hour},{this._间隔周年月日时分秒.Minute},{this._间隔周年月日时分秒.Second}";
			}
			set
			{
				this._间隔值 = this.解码时间(value);
				this.重置时间();
			}
		}

		public 机器人类型 类型 { get; set; }

		public int 启用 { get; set; }

		public int 执行次数 { get; set; }

		public string 脚本段 { get; set; }

		public int 已执行次数 => this._已执行次数;

		public DateTime 已执行时间 => this._下次执行时间;

		public static void 保存数据()
		{
			using StreamWriter writer = new StreamWriter(Settings.游戏数据目录 + "\\System\\世界其他\\机器人配置.csv");
			using CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
			csvWriter.WriteHeader<机器人csvColumns>();
			csvWriter.NextRecord();
			foreach (机器人 item in 机器人.数据表)
			{
				机器人csvColumns 机器人csvColumns2;
				机器人csvColumns2 = new 机器人csvColumns();
				机器人csvColumns2.StartTime = item.开始时间;
				机器人csvColumns2.Interval = item.间隔值;
				机器人csvColumns2.Type = item.类型;
				机器人csvColumns2.Enable = item.启用;
				机器人csvColumns2.Key = item.脚本段;
				机器人csvColumns2.Count = item.执行次数;
				csvWriter.WriteRecord(机器人csvColumns2);
				csvWriter.NextRecord();
			}
		}

		public static void 载入数据()
		{
			机器人.数据表 = new List<机器人>();
			string path;
			path = Settings.游戏数据目录 + "\\System\\世界其他\\机器人配置.csv";
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
					if (Enum.TryParse<机器人类型>(item["Type"].ToString(), ignoreCase: true, out var result))
					{
						机器人 机器人2;
						机器人2 = new 机器人();
						机器人2.开始时间 = DateTime.Parse(item["StartTime"].ToString());
						机器人2.类型 = result;
						机器人2.启用 = int.Parse(item["Enable"].ToString());
						机器人2.脚本段 = item["Key"].ToString();
						if (dataTable.Columns.Contains("Count"))
						{
							机器人2.执行次数 = int.Parse(item["Count"].ToString());
						}
						机器人2.间隔值 = item["Interval"].ToString();
						机器人.数据表.Add(机器人2);
					}
				}
				return;
			}
			string text;
			text = Settings.游戏数据目录 + "\\System\\机器人\\";
			if (Directory.Exists(text))
			{
				object[] array;
				array = 序列化类.反序列化(text, typeof(机器人));
				foreach (object obj in array)
				{
					机器人.数据表.Add((机器人)obj);
				}
			}
		}

		public 机器人()
		{
			this._下次执行时间 = DateTime.MinValue;
			this.执行次数 = int.MaxValue;
		}

		public static void 初始化()
		{
			if (机器人.假人 != null)
			{
				return;
			}
			账号数据 账号数据;
			if (游戏数据网关.账号数据表.检索表.TryGetValue(机器人.默认账号名, out var value))
			{
				账号数据 = value as 账号数据;
				if (账号数据 != null)
				{
					goto IL_0037;
				}
			}
			账号数据 = new 账号数据(机器人.默认账号名);
			goto IL_0037;
			IL_0037:
			角色数据 角色数据;
			if (游戏数据网关.角色数据表.检索表.TryGetValue("假人", out var value2))
			{
				角色数据 = value2 as 角色数据;
				if (角色数据 != null)
				{
					goto IL_0074;
				}
			}
			角色数据 = new 角色数据(账号数据, "假人", 游戏对象职业.战士, 游戏对象性别.女性, 对象发型分类.战士女00, 对象发色分类.发色00, 对象脸型分类.战士女00);
			goto IL_0074;
			IL_0074:
			机器人.假人 = new 玩家实例(角色数据, null);
			NPCScript nPCScript;
			nPCScript = NPCScript.Get(主程.DefaultNPC.ScriptID);
			机器人.假人.NPCObjectID = 主程.DefaultNPC.LoadedObjectID;
			nPCScript.Call(机器人.假人, 主程.DefaultNPC.LoadedObjectID, "[@_Startup]");
		}

		public static void 处理数据()
		{
			if (机器人.假人 == null)
			{
				return;
			}
			foreach (机器人 item in 机器人.数据表)
			{
				if (item.启用 == 1 && (item._下次执行时间 == DateTime.MinValue || 主程.当前时间 >= item.开始时间) && item.重置时间())
				{
					item._已执行次数++;
					item.执行();
				}
			}
		}

		public void 执行()
		{
			NPCScript nPCScript;
			nPCScript = NPCScript.Get(主程.DefaultNPC.ScriptID);
			机器人.假人.NPCObjectID = 主程.DefaultNPC.LoadedObjectID;
			nPCScript.Call(机器人.假人, 主程.DefaultNPC.LoadedObjectID, "[" + this.脚本段.ToUpper() + "]");
		}

		private short[] 解码时间(string s)
		{
			List<string> list;
			list = s.Split(',', '.', ':', '/', '-', ' ').ToList();
			while (list.Count < 7)
			{
				list.Insert(0, "0");
			}
			short[] array;
			array = new short[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				if (!short.TryParse(list[i], out var result))
				{
					result = 0;
				}
				array[i] = result;
			}
			this._间隔周年月日时分秒 = new DateTime((array[1] < 1) ? 1 : array[1], (array[2] < 1) ? 1 : array[2], (array[3] < 1) ? 1 : array[3], (array[4] >= 0 && array[4] <= 23) ? array[4] : 0, (array[5] >= 0 && array[5] <= 59) ? array[5] : 0, (array[6] >= 0 && array[6] <= 59) ? array[6] : 0);
			return array;
		}

		public bool 重置时间()
		{
			bool flag;
			flag = this.已执行次数 < this.执行次数 && 主程.当前时间 >= this.已执行时间;
			DateTime dateTime;
			dateTime = 主程.当前时间;
			switch (this.类型)
			{
			default:
				dateTime = dateTime.AddSeconds(118.0);
				break;
			case 机器人类型.RODAY:
				if (dateTime.TimeOfDay >= this._间隔周年月日时分秒.TimeOfDay)
				{
					dateTime = dateTime.AddDays(1.0);
				}
				dateTime = dateTime.Date + this._间隔周年月日时分秒.TimeOfDay;
				break;
			case 机器人类型.ROHOUR:
			case 机器人类型.ROMIN:
			case 机器人类型.ROSEC:
				dateTime = dateTime.AddTicks(this._间隔周年月日时分秒.TimeOfDay.Ticks);
				break;
			case 机器人类型.RUNONWEEK:
			{
				short num;
				num = (short)(this._间隔值[3] - 机器人.DayOfWeekNum[dateTime.DayOfWeek]);
				if (num > 0)
				{
					dateTime = dateTime.AddDays(num);
				}
				if (num < 0 || (num == 0 && this._间隔周年月日时分秒.TimeOfDay <= dateTime.TimeOfDay))
				{
					dateTime = dateTime.AddDays(7 - -num);
				}
				dateTime = dateTime.Date + this._间隔周年月日时分秒.TimeOfDay;
				break;
			}
			case 机器人类型.RUNONDAY:
				if (dateTime.TimeOfDay >= this._间隔周年月日时分秒.TimeOfDay)
				{
					dateTime = dateTime.AddDays(1.0);
				}
				dateTime = dateTime.Date + this._间隔周年月日时分秒.TimeOfDay;
				break;
			case 机器人类型.RUNONHOUR:
			{
				DateTime dateTime2;
				dateTime2 = dateTime.Date.AddHours(dateTime.Hour);
				dateTime = ((!(dateTime >= dateTime2 + this._间隔周年月日时分秒.TimeOfDay)) ? dateTime2 : (dateTime2.AddHours(1.0) + this._间隔周年月日时分秒.TimeOfDay));
				break;
			}
			case 机器人类型.RUNONMIN:
			{
				DateTime dateTime2;
				dateTime2 = dateTime.Date.AddHours(dateTime.Hour).AddMinutes(dateTime.Minute) + this._间隔周年月日时分秒.TimeOfDay;
				dateTime = ((!(dateTime >= dateTime2)) ? dateTime2 : dateTime2.AddMinutes(1.0));
				break;
			}
			case 机器人类型.RUNONSEC:
			{
				DateTime dateTime2;
				dateTime2 = dateTime.Date.AddHours(dateTime.Hour).AddMinutes(dateTime.Minute) + this._间隔周年月日时分秒.TimeOfDay;
				dateTime = ((!(dateTime >= dateTime2)) ? dateTime2 : dateTime2.AddSeconds(1.0));
				break;
			}
			case 机器人类型.RUNDATETIME:
				break;
			}
			if (flag)
			{
				this._下次执行时间 = dateTime;
			}
			return flag;
		}
	}
}
