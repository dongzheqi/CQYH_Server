using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using 游戏服务器.数据类;
using 游戏服务器.网络类;
using CsvHelper;

namespace 游戏服务器.模板类
{
	public class 充值奖励
	{
		public static List<充值奖励> 数据表;

		public static List<充值奖励> 累计数据表;

		public static List<充值奖励> 单次数据表;

		private int _已执行次数;

		private DateTime _下次执行时间;

		private short[] _间隔值;

		private DateTime _间隔周年月日时分秒;

		public 充值奖励类型 类型 { get; set; }

		public int 需要 { get; set; }

		public string 标题 { get; set; }

		public string 正文 { get; set; }

		public string 公告 { get; set; }

		public int 物品id { get; set; }

		public byte Id { get; set; }

		public int 物品数量 { get; set; }

		public int 已执行次数 => this._已执行次数;

		public DateTime 已执行时间 => this._下次执行时间;

		public override string ToString()
		{
			return this.需要.ToString();
		}

		public static void 保存数据()
		{
			using StreamWriter writer = new StreamWriter(Settings.游戏数据目录 + "\\System\\商城福利\\充值奖励.csv");
			using CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
			csvWriter.WriteHeader<充值奖励csvColumns>();
			csvWriter.NextRecord();
			foreach (充值奖励 item in 充值奖励.数据表)
			{
				充值奖励csvColumns 充值奖励csvColumns2;
				充值奖励csvColumns2 = new 充值奖励csvColumns();
				充值奖励csvColumns2.Type = item.类型;
				充值奖励csvColumns2.Caption = item.标题;
				充值奖励csvColumns2.Text = item.正文;
				充值奖励csvColumns2.ItemId = item.物品id;
				充值奖励csvColumns2.Id = item.Id;
				充值奖励csvColumns2.ItemCnt = item.物品数量;
				充值奖励csvColumns2.Count = item.需要;
				充值奖励csvColumns2.GG = item.公告;
				csvWriter.WriteRecord(充值奖励csvColumns2);
				csvWriter.NextRecord();
			}
		}

		public static void 载入数据()
		{
			充值奖励.数据表 = new List<充值奖励>();
			充值奖励.累计数据表 = new List<充值奖励>();
			充值奖励.单次数据表 = new List<充值奖励>();
			string path;
			path = Settings.游戏数据目录 + "\\System\\商城福利\\充值奖励.csv";
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
					if (Enum.TryParse<充值奖励类型>(item["Type"].ToString(), ignoreCase: true, out var result))
					{
						充值奖励 充值奖励2;
						充值奖励2 = new 充值奖励
						{
							类型 = result,
							Id = byte.Parse(item["Id"].ToString()),
							标题 = item["Caption"].ToString(),
							正文 = item["Text"].ToString(),
							公告 = item["GG"].ToString(),
							物品id = int.Parse(item["ItemId"].ToString()),
							物品数量 = int.Parse(item["ItemCnt"].ToString()),
							需要 = int.Parse(item["Count"].ToString())
						};
						充值奖励.数据表.Add(充值奖励2);
						switch (充值奖励2.类型)
						{
						case 充值奖励类型.单次:
							充值奖励.单次数据表.Add(充值奖励2);
							break;
						case 充值奖励类型.累计:
							充值奖励.累计数据表.Add(充值奖励2);
							break;
						}
					}
				}
			}
			foreach (充值奖励 item2 in 充值奖励.数据表)
			{
				switch (item2.类型)
				{
				case 充值奖励类型.单次:
					充值奖励.单次数据表.Add(item2);
					break;
				case 充值奖励类型.累计:
					充值奖励.累计数据表.Add(item2);
					break;
				}
			}
			充值奖励.单次数据表.Sort((充值奖励 a, 充值奖励 b) => (a.需要 < b.需要) ? 1 : (-1));
			充值奖励.累计数据表.Sort((充值奖励 a, 充值奖励 b) => (a.需要 < b.需要) ? 1 : (-1));
			充值奖励.单次数据表.Find((充值奖励 x) => 150 >= x.需要);
			充值奖励.累计数据表.Find((充值奖励 x) => 150 >= x.需要);
		}

		public 充值奖励()
		{
			this._下次执行时间 = DateTime.MinValue;
		}

		public static void 来钱了(角色数据 角, uint 充值金额)
		{
			充值奖励.单次数据表.Find((充值奖励 x) => x.需要 <= 充值金额)?.发放(角);
			foreach (充值奖励 item in 充值奖励.累计数据表)
			{
				if (item != null && item.需要 <= 角.累计充值.V && 角.累计充值奖励已领取[item.Id] == 0)
				{
					角.累计充值奖励已领取[item.Id] = 1;
					item.发放(角);
				}
			}
			if (游戏数据网关.账号数据表.检索表.TryGetValue(角.所属账号.V.推广代码.V, out var value) && value is 账号数据 账号数据)
			{
				账号数据.推广点.V += (uint)((double)充值金额 * 0.05);
			}
		}

		private void 发放(角色数据 角)
		{
			角.发送邮件(null, this.标题, this.正文, this.物品id, this.物品数量);
			if (this.公告 != string.Empty)
			{
				网络服务网关.发送公告(this.公告.Replace("%Play%", 角.角色名字.V));
			}
		}
	}
}
