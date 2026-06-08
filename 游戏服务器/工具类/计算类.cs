using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using 游戏服务器.地图类;
using 游戏服务器.模板类;

namespace 游戏服务器.工具类
{
	public static class 计算类
	{
		public static readonly DateTime 系统相对时间;

		private static uint[] crcTable;

		public static string ToHexStrFromByte(byte[] byteDatas)
		{
			StringBuilder stringBuilder;
			stringBuilder = new StringBuilder();
			for (int i = 0; i < byteDatas.Length; i++)
			{
				stringBuilder.Append($"{byteDatas[i]:X2} ");
			}
			return stringBuilder.ToString().Trim();
		}

		public static 游戏对象职业 字符转职业(string 职业)
		{
			return 职业 switch
			{
				"clsSpear" => 游戏对象职业.龙枪, 
				"clsTaoist" => 游戏对象职业.道士, 
				"clsArcher" => 游戏对象职业.弓手, 
				"clsRogue" => 游戏对象职业.刺客, 
				"clsMage" => 游戏对象职业.法师, 
				"clsWarrior" => 游戏对象职业.战士, 
				_ => 游戏对象职业.战士, 
			};
		}

		public static 游戏对象属性 字符转属性(string 属性)
		{
			return 属性 switch
			{
				"catLuck" => 游戏对象属性.幸运等级, 
				"catCrit" => 游戏对象属性.暴击概率, 
				"catMaxMp" => 游戏对象属性.最大魔力, 
				"catMaxHp" => 游戏对象属性.最大体力, 
				"catTaoAtk" => 游戏对象属性.最小道术, 
				"catPhyAtk" => 游戏对象属性.最小攻击, 
				"catPhyDef" => 游戏对象属性.最小防御, 
				"catMagAtk" => 游戏对象属性.最小魔法, 
				"catMagDef" => 游戏对象属性.最小魔防, 
				"catStabAtk" => 游戏对象属性.最小刺术, 
				"catAgility" => 游戏对象属性.物理敏捷, 
				"catBurning" => 游戏对象属性.灼烧强化, 
				"catCritDmg" => 游戏对象属性.暴击伤害, 
				"catAccurate" => 游戏对象属性.物理准确, 
				"catMagDodge" => 游戏对象属性.魔法闪避, 
				"catAtkSpeed" => 游戏对象属性.攻击速度, 
				"catBleeding" => 游戏对象属性.流血强化, 
				"catMaxPhyAtk" => 游戏对象属性.最大攻击, 
				"catMaxPhyDef" => 游戏对象属性.最大防御, 
				"catPhyResist" => 游戏对象属性.物理抗性, 
				"catMaxTaoAtk" => 游戏对象属性.最大道术, 
				"catMaxMagAtk" => 游戏对象属性.最大魔法, 
				"catMaxMagDef" => 游戏对象属性.最大魔防, 
				"catMaxStabAtk" => 游戏对象属性.最大刺术, 
				"catDmgEnhance" => 游戏对象属性.伤害加成, 
				"catSlowResist" => 游戏对象属性.减速抗性, 
				"catCritResist" => 游戏对象属性.减暴击, 
				"catMageResist" => 游戏对象属性.魔法抗性, 
				"catArcheryAtk" => 游戏对象属性.最小弓术, 
				"catAntiMagDef" => 游戏对象属性.破魔防, 
				"catAntiPhyDef" => 游戏对象属性.破物防, 
				"catMagAccurate" => 游戏对象属性.魔法命中, 
				"catFreezeResist" => 游戏对象属性.定身抗性, 
				"catMaxArcheryAtk" => 游戏对象属性.最大弓术, 
				"catCritDmgResist" => 游戏对象属性.减暴伤, 
				"catAntiPhyDefRate" => 游戏对象属性.破物防, 
				"catAntiMagDefRate" => 游戏对象属性.破魔防, 
				"catMinIgnoreDefDmg" => 游戏对象属性.最小圣伤, 
				"catMaxIgnoreDefDmg" => 游戏对象属性.最大圣伤, 
				"catPoisonResist" => 游戏对象属性.中毒躲避,
				_ => 游戏对象属性.未知属性, 
			};
		}

		public static bool 是否为龙卫装备(装备穿戴部位 部位)
		{
			if (部位 != 0 && 部位 != 装备穿戴部位.衣服 && 部位 != 装备穿戴部位.头盔 && 部位 != 装备穿戴部位.项链 && 部位 != 装备穿戴部位.腰带 && 部位 != 装备穿戴部位.鞋子 && 部位 != 装备穿戴部位.左戒指)
			{
				return 部位 == 装备穿戴部位.右戒指;
			}
			return true;
		}

		public static int 扩展背包(int 扩展次数, int 当前消耗 = 0, int 当前位置 = 1, int 累计消耗 = 0)
		{
			if (当前位置 > 扩展次数)
			{
				return 累计消耗;
			}
			if (当前位置 <= 1)
			{
				int num;
				num = 累计消耗;
				当前消耗 = 2000;
				累计消耗 = num + 2000;
			}
			else if (当前位置 <= 16)
			{
				累计消耗 += (当前消耗 += 1000);
			}
			else if (当前位置 == 17)
			{
				int num2;
				num2 = 累计消耗;
				当前消耗 = 20000;
				累计消耗 = num2 + 20000;
			}
			else
			{
				累计消耗 += (当前消耗 += 10000);
			}
			return 计算类.扩展背包(扩展次数, 当前消耗, 当前位置 + 1, 累计消耗);
		}

		public static int 扩展仓库(int 扩展次数, int 当前消耗 = 0, int 当前位置 = 1, int 累计消耗 = 0)
		{
			if (当前位置 > 扩展次数)
			{
				return 累计消耗;
			}
			if (当前位置 > 56 && 当前位置 < 129)
			{
				int num;
				num = 累计消耗;
				当前消耗 = 400000;
				累计消耗 = num + 400000;
			}
			else if (当前位置 > 128 && 当前位置 < 201)
			{
				int num2;
				num2 = 累计消耗;
				当前消耗 = 500000;
				累计消耗 = num2 + 500000;
			}
			else if (当前位置 <= 1)
			{
				int num3;
				num3 = 累计消耗;
				当前消耗 = 2000;
				累计消耗 = num3 + 2000;
			}
			else if (当前位置 <= 24)
			{
				累计消耗 += (当前消耗 += 1000);
			}
			else if (当前位置 == 25)
			{
				int num4;
				num4 = 累计消耗;
				当前消耗 = 30000;
				累计消耗 = num4 + 30000;
			}
			else
			{
				累计消耗 += (当前消耗 += 10000);
			}
			return 计算类.扩展仓库(扩展次数, 当前消耗, 当前位置 + 1, 累计消耗);
		}

		public static int 数值限制(int 下限, int 数值, int 上限)
		{
			if (数值 > 上限)
			{
				return 上限;
			}
			if (数值 < 下限)
			{
				return 下限;
			}
			return 数值;
		}

		public static 游戏方向 TurnAround(游戏方向 当前方向, int RotationVector)
		{
			return 当前方向 + RotationVector % 8 * 1024 + 0;
		}

		public static int 网格距离(Point 原点, Point 终点)
		{
			return Math.Max(Math.Abs(终点.X - 原点.X), Math.Abs(终点.Y - 原点.Y));
		}

		public static int 时间转换(DateTime 时间)
		{
			return (int)(时间 - 计算类.系统相对时间).TotalSeconds;
		}

		public static int 日期转换(DateTime 时间)
		{
			return (int)(时间 - 计算类.系统相对时间).TotalDays;
		}

		public static bool 日期同周(DateTime 日期一, DateTime 日期二)
		{
			DateTime dateTime;
			dateTime = ((日期二 > 日期一) ? 日期二 : 日期一);
			DateTime dateTime2;
			dateTime2 = ((日期二 > 日期一) ? 日期一 : 日期二);
			if ((dateTime - dateTime2).TotalDays > 7.0)
			{
				return false;
			}
			int num;
			num = Convert.ToInt32(dateTime.DayOfWeek);
			if (num == 0)
			{
				num = 7;
			}
			int num2;
			num2 = Convert.ToInt32(dateTime2.DayOfWeek);
			if (num2 == 0)
			{
				num2 = 7;
			}
			if (num2 > num)
			{
				return false;
			}
			return true;
		}

		public static DateTime 转换时间(int 秒数)
		{
			return 计算类.系统相对时间.AddSeconds(秒数);
		}

		public static DateTime 转换日期(int 天数)
		{
			return 计算类.系统相对时间.AddDays(天数);
		}

		public static long GetTimeStamp(DateTime time, bool len13 = false)
		{
			long num;
			num = (time.ToUniversalTime().Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks) / 10000L;
			if (!len13)
			{
				return num / 1000L;
			}
			return num;
		}

		public static float 收益衰减(int 玩家等级, int 怪物等级)
		{
			return (float)Math.Max(0m, (decimal)Math.Max(0, 玩家等级 - 怪物等级 - Settings.减收益等级差) * Settings.收益减少比率);
		}

		public static int 范围随机(int min, int max)
		{
			Random random;
			random = new Random(Guid.NewGuid().GetHashCode());
			if (min > max)
			{
				return random.Next(max, min);
			}
			return random.Next(min, max);
		}

		public static bool 计算概率(float 概率)
		{
			if (概率 >= 1f)
			{
				return true;
			}
			if (概率 <= 0f)
			{
				return false;
			}
			return 概率 * 100000000f > (float)主程.随机数.Next(100000000);
		}

		public static int 概率表取值(Dictionary<int, float> 表)
		{
			int num;
			num = 0;
			int num2;
			while (true)
			{
				num2 = 表.Keys.ElementAt(主程.随机数.Next(表.Count));
				if (!计算类.计算概率(表[num2]))
				{
					if (num == 20)
					{
						break;
					}
					num++;
					continue;
				}
				return num2;
			}
			return num2;
		}

		public static Point 螺旋坐标(Point 原点, int 步数)
		{
			if (--步数 >= 0)
			{
				int num;
				num = (int)Math.Floor(Math.Sqrt((double)步数 + 0.25) - 0.5);
				int num2;
				num2 = num * (num + 1);
				int num3;
				num3 = num2 + num + 1;
				int num4;
				num4 = ((num & 1) << 1) - 1;
				int num5;
				num5 = num4 * (num + 1 >> 1);
				int num6;
				num6 = num5;
				if (步数 < num3)
				{
					num5 -= num4 * (步数 - num2 + 1);
				}
				else
				{
					num5 -= num4 * (num + 1);
					num6 -= num4 * (步数 - num3 + 1);
				}
				return new Point(原点.X + num5, 原点.Y + num6);
			}
			return 原点;
		}

		public static Point 前方坐标(Point 原点, Point 终点, int 步数)
		{
			if (原点 == 终点)
			{
				return 原点;
			}
			float num;
			num = (float)步数 / (float)计算类.网格距离(原点, 终点);
			int num2;
			num2 = (int)Math.Round((float)(终点.X - 原点.X) * num);
			int num3;
			num3 = (int)Math.Round((float)(终点.Y - 原点.Y) * num);
			return new Point(原点.X + num2, 原点.Y + num3);
		}

		public static Point 前方坐标(Point 原点, 游戏方向 方向, int 步数)
		{
			return 方向 switch
			{
				游戏方向.上方 => new Point(原点.X, 原点.Y + 步数), 
				游戏方向.左上 => new Point(原点.X + 步数, 原点.Y + 步数), 
				游戏方向.左方 => new Point(原点.X + 步数, 原点.Y), 
				游戏方向.右方 => new Point(原点.X - 步数, 原点.Y), 
				游戏方向.右上 => new Point(原点.X - 步数, 原点.Y + 步数), 
				游戏方向.下方 => new Point(原点.X, 原点.Y - 步数), 
				游戏方向.右下 => new Point(原点.X - 步数, 原点.Y - 步数), 
				_ => new Point(原点.X + 步数, 原点.Y - 步数), 
			};
		}

		public static 游戏方向 随机方向()
		{
			return (游戏方向)(主程.随机数.Next(8) * 1024);
		}

		public static 游戏方向 计算方向(Point 原点, Point 终点)
		{
			return (游戏方向)((int)(Math.Round((Math.Atan2(x: 终点.X - 原点.X, y: 终点.Y - 原点.Y) * 180.0 / Math.PI + 360.0) % 360.0 / 45.0) * 1024.0) % 8192);
		}

		public static 游戏方向 正向方向(Point 原点, Point 终点)
		{
			if (原点 == 终点)
			{
				return 游戏方向.左方;
			}
			return 计算类.计算方向(终点: 计算类.前方坐标(方向: 计算类.计算方向(终点, 原点), 步数: Math.Max(Math.Abs(终点.X - 原点.X), Math.Abs(终点.Y - 原点.Y)) - 1, 原点: 终点), 原点: 原点);
		}

		public static 游戏方向 旋转方向(游戏方向 当前方向, int 旋转向量)
		{
			return (游戏方向)((int)(当前方向 + 旋转向量 % 8 * 1024 + 8192) % 8192);
		}

		public static Point 点阵坐标转协议坐标(Point 点阵坐标)
		{
			return new Point(点阵坐标.X * 32 - 16, 点阵坐标.Y * 32 - 16);
		}

		public static int 点阵坐标转协议坐标(int 点阵坐标)
		{
			return 点阵坐标 * 32 - 16;
		}

		public static ushort 点阵坐标转协议坐标(ushort 点阵坐标)
		{
			return (ushort)(点阵坐标 * 32 - 16);
		}

		public static Point 协议坐标转点阵坐标(Point 协议坐标)
		{
			return new Point((int)Math.Round(((float)协议坐标.X + 16f) / 32f), (int)Math.Round(((float)协议坐标.Y + 16f) / 32f));
		}

		public static Point 游戏坐标转点阵坐标(PointF 游戏坐标)
		{
			PointF pointF;
			pointF = default(PointF);
			pointF.Y = (游戏坐标.X + 游戏坐标.Y) / 0.707107f / 0.000976562f / 2f / 4096f;
			pointF.X = (游戏坐标.X / 0.707107f / 0.000976562f + 134217730f) / 4096f - pointF.Y;
			return new Point((int)((double)(pointF.X / 32f) + 0.5), (int)((double)(pointF.Y / 32f) + 0.5));
		}

		public static PointF 点阵坐标转游戏坐标(Point 点阵坐标)
		{
			PointF pointF;
			pointF = new PointF(((float)点阵坐标.X - 0.5f) * 32f, ((float)点阵坐标.Y - 0.5f) * 32f);
			PointF result;
			result = default(PointF);
			result.X = ((pointF.Y + pointF.X) * 4096f - 134217730f) * 0.707107f * 0.000976562f;
			result.Y = ((pointF.Y - pointF.X) * 4096f + 134217730f) * 0.707107f * 0.000976562f;
			return result;
		}

		public static int 计算攻速(int 攻速)
		{
			return 计算类.数值限制(-50, 攻速, 50) * 60;
		}

		public static float 计算幸运(int 幸运)
		{
			switch (幸运)
			{
			default:
				if (幸运 >= 9)
				{
					return 1f;
				}
				return 0f;
			case 0:
				return 0.1f;
			case 1:
				return 0.11f;
			case 2:
				return 0.13f;
			case 3:
				return 0.14f;
			case 4:
				return 0.17f;
			case 5:
				return 0.2f;
			case 6:
				return 0.25f;
			case 7:
				return 0.33f;
			case 8:
				return 0.5f;
			}
		}

		public static int 计算攻击(int 下限, int 上限, int 幸运)
		{
			int result;
			result = ((幸运 >= 0) ? 上限 : 下限);
			if (计算类.计算概率(计算类.计算幸运(Math.Abs(幸运))))
			{
				return result;
			}
			return 主程.随机数.Next(Math.Min(下限, 上限), Math.Max(下限, 上限) + 1);
		}

		// 幸运倍攻(从零原创): 按攻击方幸运值查最高满足档位的伤害倍率. 未开启或低于最低档返回 1f(不增伤).
		public static float 幸运倍攻倍率(int 幸运值)
		{
			if (!Settings.开启幸运倍率功能)
			{
				return 1f;
			}
			float 倍率 = 1f;
			for (int i = 0; i < Settings.幸运额外增伤值.Length && i < Settings.幸运增伤倍率值.Length; i++)
			{
				if (幸运值 >= Settings.幸运额外增伤值[i] && Settings.幸运增伤倍率值[i] > 倍率)
				{
					倍率 = Settings.幸运增伤倍率值[i];
				}
			}
			return 倍率;
		}

		public static int 计算防御(int 下限, int 上限)
		{
			if (上限 >= 下限)
			{
				return 主程.随机数.Next(下限, 上限 + 1);
			}
			return 主程.随机数.Next(上限, 下限 + 1);
		}

		public static bool 直线方向(Point 原点, Point 锚点)
		{
			int num;
			num = 原点.X - 锚点.X;
			int num2;
			num2 = 原点.Y - 锚点.Y;
			if (num != 0 && num2 != 0)
			{
				return Math.Abs(num) == Math.Abs(num2);
			}
			return true;
		}

		public static bool 计算命中(float 命中基数, float 闪避基数, float 命中系数, float 闪避系数)
		{
			float 概率;
			概率 = ((闪避基数 == 0f) ? 1f : (命中基数 / 闪避基数));
			float num;
			num = 命中系数 - 闪避系数;
			if (num == 0f)
			{
				return 计算类.计算概率(概率);
			}
			if (num >= 0f)
			{
				if (!计算类.计算概率(概率))
				{
					return 计算类.计算概率(num);
				}
				return true;
			}
			if (计算类.计算概率(概率))
			{
				return !计算类.计算概率(0f - num);
			}
			return false;
		}

		public static bool 计算位移(地图实例 地图, 地图对象 来源, 游戏方向 方向, int 力度, out List<地图对象> 目标)
		{
			目标 = new List<地图对象>();
			return false;
		}

		public static Point[] 技能范围(Point 锚点, List<Point> 范围, bool 计算锚点自身)
		{
			List<Point> list;
			list = new List<Point>();
			if (计算锚点自身)
			{
				list.Add(锚点);
			}
			foreach (Point item in 范围)
			{
				list.Add(new Point(item.X + 锚点.X, item.Y + 锚点.Y));
			}
			return list.ToArray();
		}

		public static Point[] 技能范围(Point 锚点, 游戏方向 方向, 技能范围类型 范围)
		{
			return 范围 switch
			{
				技能范围类型.单体1x1 => new Point[1] { 锚点 }, 
				技能范围类型.半月3x1 => 方向 switch
				{
					游戏方向.上方 => new Point[5]
					{
						锚点,
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y - 1)
					}, 
					游戏方向.左上 => new Point[5]
					{
						锚点,
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 2),
						new Point(锚点.X - 2, 锚点.Y)
					}, 
					游戏方向.左方 => new Point[5]
					{
						锚点,
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y + 1)
					}, 
					游戏方向.右方 => new Point[5]
					{
						锚点,
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y - 1)
					}, 
					游戏方向.右上 => new Point[5]
					{
						锚点,
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X + 2, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 2)
					}, 
					游戏方向.左下 => new Point[5]
					{
						锚点,
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X - 2, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 2)
					}, 
					游戏方向.下方 => new Point[5]
					{
						锚点,
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y + 1)
					}, 
					_ => new Point[5]
					{
						锚点,
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 2),
						new Point(锚点.X + 2, 锚点.Y)
					}, 
				}, 
				技能范围类型.半月3x2 => 方向 switch
				{
					游戏方向.上方 => new Point[8]
					{
						锚点,
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y + 1)
					}, 
					游戏方向.左上 => new Point[8]
					{
						锚点,
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y + 1)
					}, 
					游戏方向.左方 => new Point[8]
					{
						锚点,
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y + 1)
					}, 
					游戏方向.右方 => new Point[8]
					{
						锚点,
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y - 1)
					}, 
					游戏方向.右上 => new Point[8]
					{
						锚点,
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y - 1)
					}, 
					游戏方向.左下 => new Point[8]
					{
						锚点,
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y + 1)
					}, 
					游戏方向.下方 => new Point[8]
					{
						锚点,
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y - 1)
					}, 
					_ => new Point[8]
					{
						锚点,
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y - 1)
					}, 
				}, 
				技能范围类型.半月3x3 => 方向 switch
				{
					游戏方向.上方 => new Point[12]
					{
						锚点,
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X + 2, 锚点.Y),
						new Point(锚点.X - 2, 锚点.Y),
						new Point(锚点.X + 2, 锚点.Y - 1),
						new Point(锚点.X - 2, 锚点.Y - 1)
					}, 
					游戏方向.左上 => new Point[12]
					{
						锚点,
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 2),
						new Point(锚点.X - 2, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y - 2),
						new Point(锚点.X - 2, 锚点.Y + 1)
					}, 
					游戏方向.左方 => new Point[12]
					{
						锚点,
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y - 2),
						new Point(锚点.X, 锚点.Y + 2),
						new Point(锚点.X - 1, 锚点.Y - 2),
						new Point(锚点.X - 1, 锚点.Y + 2)
					}, 
					游戏方向.右方 => new Point[12]
					{
						锚点,
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y + 2),
						new Point(锚点.X, 锚点.Y - 2),
						new Point(锚点.X + 1, 锚点.Y + 2),
						new Point(锚点.X + 1, 锚点.Y - 2)
					}, 
					游戏方向.右上 => new Point[12]
					{
						锚点,
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X + 2, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 2),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X + 2, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y - 2)
					}, 
					游戏方向.左下 => new Point[12]
					{
						锚点,
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X - 2, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 2),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X - 2, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y + 2)
					}, 
					游戏方向.下方 => new Point[12]
					{
						锚点,
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X - 2, 锚点.Y),
						new Point(锚点.X + 2, 锚点.Y),
						new Point(锚点.X - 2, 锚点.Y + 1),
						new Point(锚点.X + 2, 锚点.Y + 1)
					}, 
					_ => new Point[12]
					{
						锚点,
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 2),
						new Point(锚点.X + 2, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y + 2),
						new Point(锚点.X + 2, 锚点.Y - 1)
					}, 
				}, 
				技能范围类型.空心3x3 => new Point[8]
				{
					计算类.前方坐标(锚点, 游戏方向.上方, 1),
					计算类.前方坐标(锚点, 游戏方向.下方, 1),
					计算类.前方坐标(锚点, 游戏方向.左方, 1),
					计算类.前方坐标(锚点, 游戏方向.右方, 1),
					计算类.前方坐标(锚点, 游戏方向.左上, 1),
					计算类.前方坐标(锚点, 游戏方向.左下, 1),
					计算类.前方坐标(锚点, 游戏方向.右上, 1),
					计算类.前方坐标(锚点, 游戏方向.右下, 1)
				}, 
				技能范围类型.实心3x3 => new Point[9]
				{
					锚点,
					计算类.前方坐标(锚点, 游戏方向.上方, 1),
					计算类.前方坐标(锚点, 游戏方向.下方, 1),
					计算类.前方坐标(锚点, 游戏方向.左方, 1),
					计算类.前方坐标(锚点, 游戏方向.右方, 1),
					计算类.前方坐标(锚点, 游戏方向.左上, 1),
					计算类.前方坐标(锚点, 游戏方向.左下, 1),
					计算类.前方坐标(锚点, 游戏方向.右上, 1),
					计算类.前方坐标(锚点, 游戏方向.右下, 1)
				}, 
				技能范围类型.实心5x5 => new Point[25]
				{
					锚点,
					new Point(锚点.X + 1, 锚点.Y + 1),
					new Point(锚点.X, 锚点.Y + 1),
					new Point(锚点.X - 1, 锚点.Y + 1),
					new Point(锚点.X + 1, 锚点.Y),
					new Point(锚点.X - 1, 锚点.Y),
					new Point(锚点.X + 1, 锚点.Y - 1),
					new Point(锚点.X, 锚点.Y - 1),
					new Point(锚点.X - 1, 锚点.Y - 1),
					new Point(锚点.X + 2, 锚点.Y),
					new Point(锚点.X + 2, 锚点.Y + 1),
					new Point(锚点.X + 2, 锚点.Y + 2),
					new Point(锚点.X + 1, 锚点.Y + 2),
					new Point(锚点.X, 锚点.Y + 2),
					new Point(锚点.X - 1, 锚点.Y + 2),
					new Point(锚点.X - 2, 锚点.Y + 2),
					new Point(锚点.X - 2, 锚点.Y + 1),
					new Point(锚点.X - 2, 锚点.Y),
					new Point(锚点.X - 2, 锚点.Y - 1),
					new Point(锚点.X - 2, 锚点.Y - 2),
					new Point(锚点.X - 1, 锚点.Y - 2),
					new Point(锚点.X, 锚点.Y - 2),
					new Point(锚点.X + 1, 锚点.Y - 2),
					new Point(锚点.X + 2, 锚点.Y - 2),
					new Point(锚点.X + 2, 锚点.Y - 1)
				}, 
				技能范围类型.斩月1x3 => new Point[3]
				{
					锚点,
					计算类.前方坐标(锚点, 方向, 1),
					计算类.前方坐标(锚点, 方向, 2)
				}, 
				技能范围类型.斩月3x3 => 方向 switch
				{
					游戏方向.上方 => new Point[9]
					{
						锚点,
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y + 2),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y + 2),
						new Point(锚点.X - 1, 锚点.Y + 2)
					}, 
					游戏方向.左上 => new Point[9]
					{
						锚点,
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X + 2, 锚点.Y + 2),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y + 2),
						new Point(锚点.X + 2, 锚点.Y + 1)
					}, 
					游戏方向.左方 => new Point[9]
					{
						锚点,
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X + 2, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X + 2, 锚点.Y - 1),
						new Point(锚点.X + 2, 锚点.Y + 1)
					}, 
					游戏方向.右方 => new Point[9]
					{
						锚点,
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X - 2, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X - 2, 锚点.Y + 1),
						new Point(锚点.X - 2, 锚点.Y - 1)
					}, 
					游戏方向.右上 => new Point[9]
					{
						锚点,
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X - 2, 锚点.Y + 2),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y + 2),
						new Point(锚点.X - 2, 锚点.Y + 1)
					}, 
					游戏方向.左下 => new Point[9]
					{
						锚点,
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X + 2, 锚点.Y - 2),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y - 2),
						new Point(锚点.X + 2, 锚点.Y - 1)
					}, 
					游戏方向.下方 => new Point[9]
					{
						锚点,
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y - 2),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y - 2),
						new Point(锚点.X + 1, 锚点.Y - 2)
					}, 
					_ => new Point[9]
					{
						锚点,
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X - 2, 锚点.Y - 2),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 2, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y - 2)
					}, 
				}, 
				技能范围类型.线型1x5 => new Point[5]
				{
					锚点,
					计算类.前方坐标(锚点, 方向, 1),
					计算类.前方坐标(锚点, 方向, 2),
					计算类.前方坐标(锚点, 方向, 3),
					计算类.前方坐标(锚点, 方向, 4)
				}, 
				技能范围类型.线型1x8 => new Point[8]
				{
					锚点,
					计算类.前方坐标(锚点, 方向, 1),
					计算类.前方坐标(锚点, 方向, 2),
					计算类.前方坐标(锚点, 方向, 3),
					计算类.前方坐标(锚点, 方向, 4),
					计算类.前方坐标(锚点, 方向, 5),
					计算类.前方坐标(锚点, 方向, 6),
					计算类.前方坐标(锚点, 方向, 7)
				}, 
				技能范围类型.线型3x8 => 方向 switch
				{
					游戏方向.上方 => new Point[24]
					{
						锚点,
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y + 2),
						new Point(锚点.X + 1, 锚点.Y + 2),
						new Point(锚点.X - 1, 锚点.Y + 2),
						new Point(锚点.X, 锚点.Y + 3),
						new Point(锚点.X + 1, 锚点.Y + 3),
						new Point(锚点.X - 1, 锚点.Y + 3),
						new Point(锚点.X, 锚点.Y + 4),
						new Point(锚点.X + 1, 锚点.Y + 4),
						new Point(锚点.X - 1, 锚点.Y + 4),
						new Point(锚点.X, 锚点.Y + 5),
						new Point(锚点.X + 1, 锚点.Y + 5),
						new Point(锚点.X - 1, 锚点.Y + 5),
						new Point(锚点.X, 锚点.Y + 6),
						new Point(锚点.X + 1, 锚点.Y + 6),
						new Point(锚点.X - 1, 锚点.Y + 6),
						new Point(锚点.X, 锚点.Y + 7),
						new Point(锚点.X + 1, 锚点.Y + 7),
						new Point(锚点.X - 1, 锚点.Y + 7)
					}, 
					游戏方向.左上 => new Point[24]
					{
						锚点,
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 2, 锚点.Y + 2),
						new Point(锚点.X + 2, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y + 2),
						new Point(锚点.X + 3, 锚点.Y + 3),
						new Point(锚点.X + 3, 锚点.Y + 2),
						new Point(锚点.X + 2, 锚点.Y + 3),
						new Point(锚点.X + 4, 锚点.Y + 4),
						new Point(锚点.X + 4, 锚点.Y + 3),
						new Point(锚点.X + 3, 锚点.Y + 4),
						new Point(锚点.X + 5, 锚点.Y + 5),
						new Point(锚点.X + 5, 锚点.Y + 4),
						new Point(锚点.X + 4, 锚点.Y + 5),
						new Point(锚点.X + 6, 锚点.Y + 6),
						new Point(锚点.X + 6, 锚点.Y + 5),
						new Point(锚点.X + 5, 锚点.Y + 6),
						new Point(锚点.X + 7, 锚点.Y + 7),
						new Point(锚点.X + 7, 锚点.Y + 6),
						new Point(锚点.X + 6, 锚点.Y + 7)
					}, 
					游戏方向.左方 => new Point[24]
					{
						锚点,
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X + 2, 锚点.Y),
						new Point(锚点.X + 2, 锚点.Y - 1),
						new Point(锚点.X + 2, 锚点.Y + 1),
						new Point(锚点.X + 3, 锚点.Y),
						new Point(锚点.X + 3, 锚点.Y - 1),
						new Point(锚点.X + 3, 锚点.Y + 1),
						new Point(锚点.X + 4, 锚点.Y),
						new Point(锚点.X + 4, 锚点.Y - 1),
						new Point(锚点.X + 4, 锚点.Y + 1),
						new Point(锚点.X + 5, 锚点.Y),
						new Point(锚点.X + 5, 锚点.Y - 1),
						new Point(锚点.X + 5, 锚点.Y + 1),
						new Point(锚点.X + 6, 锚点.Y),
						new Point(锚点.X + 6, 锚点.Y - 1),
						new Point(锚点.X + 6, 锚点.Y + 1),
						new Point(锚点.X + 7, 锚点.Y),
						new Point(锚点.X + 7, 锚点.Y - 1),
						new Point(锚点.X + 7, 锚点.Y + 1)
					}, 
					游戏方向.右方 => new Point[24]
					{
						锚点,
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X - 2, 锚点.Y),
						new Point(锚点.X - 2, 锚点.Y + 1),
						new Point(锚点.X - 2, 锚点.Y - 1),
						new Point(锚点.X - 3, 锚点.Y),
						new Point(锚点.X - 3, 锚点.Y + 1),
						new Point(锚点.X - 3, 锚点.Y - 1),
						new Point(锚点.X - 4, 锚点.Y),
						new Point(锚点.X - 4, 锚点.Y + 1),
						new Point(锚点.X - 4, 锚点.Y - 1),
						new Point(锚点.X - 5, 锚点.Y),
						new Point(锚点.X - 5, 锚点.Y + 1),
						new Point(锚点.X - 5, 锚点.Y - 1),
						new Point(锚点.X - 6, 锚点.Y),
						new Point(锚点.X - 6, 锚点.Y + 1),
						new Point(锚点.X - 6, 锚点.Y - 1),
						new Point(锚点.X - 7, 锚点.Y),
						new Point(锚点.X - 7, 锚点.Y + 1),
						new Point(锚点.X - 7, 锚点.Y - 1)
					}, 
					游戏方向.右上 => new Point[24]
					{
						锚点,
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X - 2, 锚点.Y + 2),
						new Point(锚点.X - 1, 锚点.Y + 2),
						new Point(锚点.X - 2, 锚点.Y + 1),
						new Point(锚点.X - 3, 锚点.Y + 3),
						new Point(锚点.X - 2, 锚点.Y + 3),
						new Point(锚点.X - 3, 锚点.Y + 2),
						new Point(锚点.X - 4, 锚点.Y + 4),
						new Point(锚点.X - 3, 锚点.Y + 4),
						new Point(锚点.X - 4, 锚点.Y + 3),
						new Point(锚点.X - 5, 锚点.Y + 5),
						new Point(锚点.X - 4, 锚点.Y + 5),
						new Point(锚点.X - 5, 锚点.Y + 4),
						new Point(锚点.X - 6, 锚点.Y + 6),
						new Point(锚点.X - 5, 锚点.Y + 6),
						new Point(锚点.X - 6, 锚点.Y + 5),
						new Point(锚点.X - 7, 锚点.Y + 7),
						new Point(锚点.X - 6, 锚点.Y + 7),
						new Point(锚点.X - 7, 锚点.Y + 6)
					}, 
					游戏方向.左下 => new Point[24]
					{
						锚点,
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X + 2, 锚点.Y - 2),
						new Point(锚点.X + 1, 锚点.Y - 2),
						new Point(锚点.X + 2, 锚点.Y - 1),
						new Point(锚点.X + 3, 锚点.Y - 3),
						new Point(锚点.X + 2, 锚点.Y - 3),
						new Point(锚点.X + 3, 锚点.Y - 2),
						new Point(锚点.X + 4, 锚点.Y - 4),
						new Point(锚点.X + 3, 锚点.Y - 4),
						new Point(锚点.X + 4, 锚点.Y - 3),
						new Point(锚点.X + 5, 锚点.Y - 5),
						new Point(锚点.X + 4, 锚点.Y - 5),
						new Point(锚点.X + 5, 锚点.Y - 4),
						new Point(锚点.X + 6, 锚点.Y - 6),
						new Point(锚点.X + 5, 锚点.Y - 6),
						new Point(锚点.X + 6, 锚点.Y - 5),
						new Point(锚点.X + 7, 锚点.Y - 7),
						new Point(锚点.X + 6, 锚点.Y - 7),
						new Point(锚点.X + 7, 锚点.Y - 6)
					}, 
					游戏方向.下方 => new Point[24]
					{
						锚点,
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y - 2),
						new Point(锚点.X - 1, 锚点.Y - 2),
						new Point(锚点.X + 1, 锚点.Y - 2),
						new Point(锚点.X, 锚点.Y - 3),
						new Point(锚点.X - 1, 锚点.Y - 3),
						new Point(锚点.X + 1, 锚点.Y - 3),
						new Point(锚点.X, 锚点.Y - 4),
						new Point(锚点.X - 1, 锚点.Y - 4),
						new Point(锚点.X + 1, 锚点.Y - 4),
						new Point(锚点.X, 锚点.Y - 5),
						new Point(锚点.X - 1, 锚点.Y - 5),
						new Point(锚点.X + 1, 锚点.Y - 5),
						new Point(锚点.X, 锚点.Y - 6),
						new Point(锚点.X - 1, 锚点.Y - 6),
						new Point(锚点.X + 1, 锚点.Y - 6),
						new Point(锚点.X, 锚点.Y - 7),
						new Point(锚点.X - 1, 锚点.Y - 7),
						new Point(锚点.X + 1, 锚点.Y - 7)
					}, 
					_ => new Point[24]
					{
						锚点,
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 2, 锚点.Y - 2),
						new Point(锚点.X - 2, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y - 2),
						new Point(锚点.X - 3, 锚点.Y - 3),
						new Point(锚点.X - 3, 锚点.Y - 2),
						new Point(锚点.X - 2, 锚点.Y - 3),
						new Point(锚点.X - 4, 锚点.Y - 4),
						new Point(锚点.X - 4, 锚点.Y - 3),
						new Point(锚点.X - 3, 锚点.Y - 4),
						new Point(锚点.X - 5, 锚点.Y - 5),
						new Point(锚点.X - 5, 锚点.Y - 4),
						new Point(锚点.X - 4, 锚点.Y - 5),
						new Point(锚点.X - 6, 锚点.Y - 6),
						new Point(锚点.X - 6, 锚点.Y - 5),
						new Point(锚点.X - 5, 锚点.Y - 6),
						new Point(锚点.X - 7, 锚点.Y - 7),
						new Point(锚点.X - 7, 锚点.Y - 6),
						new Point(锚点.X - 6, 锚点.Y - 7)
					}, 
				}, 
				技能范围类型.菱形3x3 => new Point[5]
				{
					锚点,
					new Point(锚点.X, 锚点.Y + 1),
					new Point(锚点.X, 锚点.Y - 1),
					new Point(锚点.X + 1, 锚点.Y),
					new Point(锚点.X - 1, 锚点.Y)
				}, 
				技能范围类型.线型3x7 => 方向 switch
				{
					游戏方向.上方 => new Point[21]
					{
						锚点,
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y + 2),
						new Point(锚点.X + 1, 锚点.Y + 2),
						new Point(锚点.X - 1, 锚点.Y + 2),
						new Point(锚点.X, 锚点.Y + 3),
						new Point(锚点.X + 1, 锚点.Y + 3),
						new Point(锚点.X - 1, 锚点.Y + 3),
						new Point(锚点.X, 锚点.Y + 4),
						new Point(锚点.X + 1, 锚点.Y + 4),
						new Point(锚点.X - 1, 锚点.Y + 4),
						new Point(锚点.X, 锚点.Y + 5),
						new Point(锚点.X + 1, 锚点.Y + 5),
						new Point(锚点.X - 1, 锚点.Y + 5),
						new Point(锚点.X, 锚点.Y + 6),
						new Point(锚点.X + 1, 锚点.Y + 6),
						new Point(锚点.X - 1, 锚点.Y + 6)
					}, 
					游戏方向.左上 => new Point[21]
					{
						锚点,
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 2, 锚点.Y + 2),
						new Point(锚点.X + 2, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y + 2),
						new Point(锚点.X + 3, 锚点.Y + 3),
						new Point(锚点.X + 3, 锚点.Y + 2),
						new Point(锚点.X + 2, 锚点.Y + 3),
						new Point(锚点.X + 4, 锚点.Y + 4),
						new Point(锚点.X + 4, 锚点.Y + 3),
						new Point(锚点.X + 3, 锚点.Y + 4),
						new Point(锚点.X + 5, 锚点.Y + 5),
						new Point(锚点.X + 5, 锚点.Y + 4),
						new Point(锚点.X + 4, 锚点.Y + 5),
						new Point(锚点.X + 6, 锚点.Y + 6),
						new Point(锚点.X + 6, 锚点.Y + 5),
						new Point(锚点.X + 5, 锚点.Y + 6)
					}, 
					游戏方向.左方 => new Point[21]
					{
						锚点,
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X + 2, 锚点.Y),
						new Point(锚点.X + 2, 锚点.Y - 1),
						new Point(锚点.X + 2, 锚点.Y + 1),
						new Point(锚点.X + 3, 锚点.Y),
						new Point(锚点.X + 3, 锚点.Y - 1),
						new Point(锚点.X + 3, 锚点.Y + 1),
						new Point(锚点.X + 4, 锚点.Y),
						new Point(锚点.X + 4, 锚点.Y - 1),
						new Point(锚点.X + 4, 锚点.Y + 1),
						new Point(锚点.X + 5, 锚点.Y),
						new Point(锚点.X + 5, 锚点.Y - 1),
						new Point(锚点.X + 5, 锚点.Y + 1),
						new Point(锚点.X + 6, 锚点.Y),
						new Point(锚点.X + 6, 锚点.Y - 1),
						new Point(锚点.X + 6, 锚点.Y + 1)
					}, 
					游戏方向.右方 => new Point[21]
					{
						锚点,
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X - 2, 锚点.Y),
						new Point(锚点.X - 2, 锚点.Y + 1),
						new Point(锚点.X - 2, 锚点.Y - 1),
						new Point(锚点.X - 3, 锚点.Y),
						new Point(锚点.X - 3, 锚点.Y + 1),
						new Point(锚点.X - 3, 锚点.Y - 1),
						new Point(锚点.X - 4, 锚点.Y),
						new Point(锚点.X - 4, 锚点.Y + 1),
						new Point(锚点.X - 4, 锚点.Y - 1),
						new Point(锚点.X - 5, 锚点.Y),
						new Point(锚点.X - 5, 锚点.Y + 1),
						new Point(锚点.X - 5, 锚点.Y - 1),
						new Point(锚点.X - 6, 锚点.Y),
						new Point(锚点.X - 6, 锚点.Y + 1),
						new Point(锚点.X - 6, 锚点.Y - 1)
					}, 
					游戏方向.右上 => new Point[21]
					{
						锚点,
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X - 2, 锚点.Y + 2),
						new Point(锚点.X - 1, 锚点.Y + 2),
						new Point(锚点.X - 2, 锚点.Y + 1),
						new Point(锚点.X - 3, 锚点.Y + 3),
						new Point(锚点.X - 2, 锚点.Y + 3),
						new Point(锚点.X - 3, 锚点.Y + 2),
						new Point(锚点.X - 4, 锚点.Y + 4),
						new Point(锚点.X - 3, 锚点.Y + 4),
						new Point(锚点.X - 4, 锚点.Y + 3),
						new Point(锚点.X - 5, 锚点.Y + 5),
						new Point(锚点.X - 4, 锚点.Y + 5),
						new Point(锚点.X - 5, 锚点.Y + 4),
						new Point(锚点.X - 6, 锚点.Y + 6),
						new Point(锚点.X - 5, 锚点.Y + 6),
						new Point(锚点.X - 6, 锚点.Y + 5)
					}, 
					游戏方向.左下 => new Point[21]
					{
						锚点,
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X + 2, 锚点.Y - 2),
						new Point(锚点.X + 1, 锚点.Y - 2),
						new Point(锚点.X + 2, 锚点.Y - 1),
						new Point(锚点.X + 3, 锚点.Y - 3),
						new Point(锚点.X + 2, 锚点.Y - 3),
						new Point(锚点.X + 3, 锚点.Y - 2),
						new Point(锚点.X + 4, 锚点.Y - 4),
						new Point(锚点.X + 3, 锚点.Y - 4),
						new Point(锚点.X + 4, 锚点.Y - 3),
						new Point(锚点.X + 5, 锚点.Y - 5),
						new Point(锚点.X + 4, 锚点.Y - 5),
						new Point(锚点.X + 5, 锚点.Y - 4),
						new Point(锚点.X + 6, 锚点.Y - 6),
						new Point(锚点.X + 5, 锚点.Y - 6),
						new Point(锚点.X + 6, 锚点.Y - 5)
					}, 
					游戏方向.下方 => new Point[21]
					{
						锚点,
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y - 2),
						new Point(锚点.X - 1, 锚点.Y - 2),
						new Point(锚点.X + 1, 锚点.Y - 2),
						new Point(锚点.X, 锚点.Y - 3),
						new Point(锚点.X - 1, 锚点.Y - 3),
						new Point(锚点.X + 1, 锚点.Y - 3),
						new Point(锚点.X, 锚点.Y - 4),
						new Point(锚点.X - 1, 锚点.Y - 4),
						new Point(锚点.X + 1, 锚点.Y - 4),
						new Point(锚点.X, 锚点.Y - 5),
						new Point(锚点.X - 1, 锚点.Y - 5),
						new Point(锚点.X + 1, 锚点.Y - 5),
						new Point(锚点.X, 锚点.Y - 6),
						new Point(锚点.X - 1, 锚点.Y - 6),
						new Point(锚点.X + 1, 锚点.Y - 6)
					}, 
					_ => new Point[21]
					{
						锚点,
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 2, 锚点.Y - 2),
						new Point(锚点.X - 2, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y - 2),
						new Point(锚点.X - 3, 锚点.Y - 3),
						new Point(锚点.X - 3, 锚点.Y - 2),
						new Point(锚点.X - 2, 锚点.Y - 3),
						new Point(锚点.X - 4, 锚点.Y - 4),
						new Point(锚点.X - 4, 锚点.Y - 3),
						new Point(锚点.X - 3, 锚点.Y - 4),
						new Point(锚点.X - 5, 锚点.Y - 5),
						new Point(锚点.X - 5, 锚点.Y - 4),
						new Point(锚点.X - 4, 锚点.Y - 5),
						new Point(锚点.X - 6, 锚点.Y - 6),
						new Point(锚点.X - 6, 锚点.Y - 5),
						new Point(锚点.X - 5, 锚点.Y - 6)
					}, 
				}, 
				技能范围类型.叉型3x3 => new Point[5]
				{
					锚点,
					new Point(锚点.X + 1, 锚点.Y + 1),
					new Point(锚点.X - 1, 锚点.Y + 1),
					new Point(锚点.X + 1, 锚点.Y - 1),
					new Point(锚点.X - 1, 锚点.Y - 1)
				}, 
				技能范围类型.空心5x5 => new Point[24]
				{
					new Point(锚点.X + 1, 锚点.Y + 1),
					new Point(锚点.X, 锚点.Y + 1),
					new Point(锚点.X - 1, 锚点.Y + 1),
					new Point(锚点.X + 1, 锚点.Y),
					new Point(锚点.X - 1, 锚点.Y),
					new Point(锚点.X + 1, 锚点.Y - 1),
					new Point(锚点.X, 锚点.Y - 1),
					new Point(锚点.X - 1, 锚点.Y - 1),
					new Point(锚点.X + 2, 锚点.Y),
					new Point(锚点.X + 2, 锚点.Y + 1),
					new Point(锚点.X + 2, 锚点.Y + 2),
					new Point(锚点.X + 1, 锚点.Y + 2),
					new Point(锚点.X, 锚点.Y + 2),
					new Point(锚点.X - 1, 锚点.Y + 2),
					new Point(锚点.X - 2, 锚点.Y + 2),
					new Point(锚点.X - 2, 锚点.Y + 1),
					new Point(锚点.X - 2, 锚点.Y),
					new Point(锚点.X - 2, 锚点.Y - 1),
					new Point(锚点.X - 2, 锚点.Y - 2),
					new Point(锚点.X - 1, 锚点.Y - 2),
					new Point(锚点.X, 锚点.Y - 2),
					new Point(锚点.X + 1, 锚点.Y - 2),
					new Point(锚点.X + 2, 锚点.Y - 2),
					new Point(锚点.X + 2, 锚点.Y - 1)
				}, 
				技能范围类型.线型1x2 => new Point[2]
				{
					锚点,
					计算类.前方坐标(锚点, 方向, 1)
				}, 
				技能范围类型.前方3x1 => 方向 switch
				{
					游戏方向.上方 => new Point[3]
					{
						锚点,
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y)
					}, 
					游戏方向.左上 => new Point[3]
					{
						锚点,
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y)
					}, 
					游戏方向.左方 => new Point[3]
					{
						锚点,
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y + 1)
					}, 
					游戏方向.右方 => new Point[3]
					{
						锚点,
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y - 1)
					}, 
					游戏方向.右上 => new Point[3]
					{
						锚点,
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 1)
					}, 
					游戏方向.左下 => new Point[3]
					{
						锚点,
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 1)
					}, 
					游戏方向.下方 => new Point[3]
					{
						锚点,
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y)
					}, 
					_ => new Point[3]
					{
						锚点,
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y)
					}, 
				}, 
				技能范围类型.螺旋7x7 => new Point[49]
				{
					锚点,
					new Point(锚点.X - 1, 锚点.Y),
					new Point(锚点.X - 1, 锚点.Y - 1),
					new Point(锚点.X, 锚点.Y - 1),
					new Point(锚点.X + 1, 锚点.Y - 1),
					new Point(锚点.X + 1, 锚点.Y),
					new Point(锚点.X + 1, 锚点.Y + 1),
					new Point(锚点.X, 锚点.Y + 1),
					new Point(锚点.X - 1, 锚点.Y + 1),
					new Point(锚点.X - 2, 锚点.Y + 1),
					new Point(锚点.X - 2, 锚点.Y),
					new Point(锚点.X - 2, 锚点.Y - 1),
					new Point(锚点.X - 2, 锚点.Y - 2),
					new Point(锚点.X - 1, 锚点.Y - 2),
					new Point(锚点.X, 锚点.Y - 2),
					new Point(锚点.X + 1, 锚点.Y - 2),
					new Point(锚点.X + 2, 锚点.Y - 2),
					new Point(锚点.X + 2, 锚点.Y - 1),
					new Point(锚点.X + 2, 锚点.Y),
					new Point(锚点.X + 2, 锚点.Y + 1),
					new Point(锚点.X + 2, 锚点.Y + 2),
					new Point(锚点.X + 1, 锚点.Y + 2),
					new Point(锚点.X, 锚点.Y + 2),
					new Point(锚点.X - 1, 锚点.Y + 2),
					new Point(锚点.X - 2, 锚点.Y + 2),
					new Point(锚点.X - 3, 锚点.Y + 2),
					new Point(锚点.X - 3, 锚点.Y + 1),
					new Point(锚点.X - 3, 锚点.Y),
					new Point(锚点.X - 3, 锚点.Y - 1),
					new Point(锚点.X - 3, 锚点.Y - 2),
					new Point(锚点.X - 3, 锚点.Y - 3),
					new Point(锚点.X - 2, 锚点.Y - 3),
					new Point(锚点.X - 1, 锚点.Y - 3),
					new Point(锚点.X, 锚点.Y - 3),
					new Point(锚点.X + 1, 锚点.Y - 3),
					new Point(锚点.X + 2, 锚点.Y - 3),
					new Point(锚点.X + 3, 锚点.Y - 3),
					new Point(锚点.X + 3, 锚点.Y - 2),
					new Point(锚点.X + 3, 锚点.Y - 1),
					new Point(锚点.X + 3, 锚点.Y),
					new Point(锚点.X + 3, 锚点.Y + 1),
					new Point(锚点.X + 3, 锚点.Y + 2),
					new Point(锚点.X + 3, 锚点.Y + 3),
					new Point(锚点.X + 2, 锚点.Y + 3),
					new Point(锚点.X + 1, 锚点.Y + 3),
					new Point(锚点.X, 锚点.Y + 3),
					new Point(锚点.X - 1, 锚点.Y + 3),
					new Point(锚点.X - 2, 锚点.Y + 3),
					new Point(锚点.X - 3, 锚点.Y + 3)
				}, 
				技能范围类型.炎龙1x2 => 方向 switch
				{
					游戏方向.上方 => new Point[6]
					{
						锚点,
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y + 1)
					}, 
					游戏方向.左上 => new Point[4]
					{
						锚点,
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 1)
					}, 
					游戏方向.左方 => new Point[6]
					{
						锚点,
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y - 1)
					}, 
					游戏方向.右方 => new Point[6]
					{
						锚点,
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y - 1)
					}, 
					游戏方向.右上 => new Point[4]
					{
						锚点,
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y)
					}, 
					游戏方向.左下 => new Point[4]
					{
						锚点,
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y)
					}, 
					游戏方向.下方 => new Point[6]
					{
						锚点,
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y - 1)
					}, 
					_ => new Point[4]
					{
						锚点,
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 1)
					}, 
				}, 
				技能范围类型.线型1x7 => new Point[7]
				{
					锚点,
					计算类.前方坐标(锚点, 方向, 1),
					计算类.前方坐标(锚点, 方向, 2),
					计算类.前方坐标(锚点, 方向, 3),
					计算类.前方坐标(锚点, 方向, 4),
					计算类.前方坐标(锚点, 方向, 5),
					计算类.前方坐标(锚点, 方向, 6)
				}, 
				技能范围类型.螺旋15x15 => new Point[288]
				{
					new Point(锚点.X - 1, 锚点.Y),
					new Point(锚点.X - 1, 锚点.Y - 1),
					new Point(锚点.X, 锚点.Y - 1),
					new Point(锚点.X + 1, 锚点.Y - 1),
					new Point(锚点.X + 1, 锚点.Y),
					new Point(锚点.X + 1, 锚点.Y + 1),
					new Point(锚点.X, 锚点.Y + 1),
					new Point(锚点.X - 1, 锚点.Y + 1),
					new Point(锚点.X - 2, 锚点.Y + 1),
					new Point(锚点.X - 2, 锚点.Y),
					new Point(锚点.X - 2, 锚点.Y - 1),
					new Point(锚点.X - 2, 锚点.Y - 2),
					new Point(锚点.X - 1, 锚点.Y - 2),
					new Point(锚点.X, 锚点.Y - 2),
					new Point(锚点.X + 1, 锚点.Y - 2),
					new Point(锚点.X + 2, 锚点.Y - 2),
					new Point(锚点.X + 2, 锚点.Y - 1),
					new Point(锚点.X + 2, 锚点.Y),
					new Point(锚点.X + 2, 锚点.Y + 1),
					new Point(锚点.X + 2, 锚点.Y + 2),
					new Point(锚点.X + 1, 锚点.Y + 2),
					new Point(锚点.X, 锚点.Y + 2),
					new Point(锚点.X - 1, 锚点.Y + 2),
					new Point(锚点.X - 2, 锚点.Y + 2),
					new Point(锚点.X - 3, 锚点.Y + 2),
					new Point(锚点.X - 3, 锚点.Y + 1),
					new Point(锚点.X - 3, 锚点.Y),
					new Point(锚点.X - 3, 锚点.Y - 1),
					new Point(锚点.X - 3, 锚点.Y - 2),
					new Point(锚点.X - 3, 锚点.Y - 3),
					new Point(锚点.X - 2, 锚点.Y - 3),
					new Point(锚点.X - 1, 锚点.Y - 3),
					new Point(锚点.X, 锚点.Y - 3),
					new Point(锚点.X + 1, 锚点.Y - 3),
					new Point(锚点.X + 2, 锚点.Y - 3),
					new Point(锚点.X + 3, 锚点.Y - 3),
					new Point(锚点.X + 3, 锚点.Y - 2),
					new Point(锚点.X + 3, 锚点.Y - 1),
					new Point(锚点.X + 3, 锚点.Y),
					new Point(锚点.X + 3, 锚点.Y + 1),
					new Point(锚点.X + 3, 锚点.Y + 2),
					new Point(锚点.X + 3, 锚点.Y + 3),
					new Point(锚点.X + 2, 锚点.Y + 3),
					new Point(锚点.X + 1, 锚点.Y + 3),
					new Point(锚点.X, 锚点.Y + 3),
					new Point(锚点.X - 1, 锚点.Y + 3),
					new Point(锚点.X - 2, 锚点.Y + 3),
					new Point(锚点.X - 3, 锚点.Y + 3),
					new Point(锚点.X - 4, 锚点.Y + 3),
					new Point(锚点.X - 4, 锚点.Y + 2),
					new Point(锚点.X - 4, 锚点.Y + 1),
					new Point(锚点.X - 4, 锚点.Y),
					new Point(锚点.X - 4, 锚点.Y - 1),
					new Point(锚点.X - 4, 锚点.Y - 2),
					new Point(锚点.X - 4, 锚点.Y - 3),
					new Point(锚点.X - 4, 锚点.Y - 4),
					new Point(锚点.X - 3, 锚点.Y - 4),
					new Point(锚点.X - 2, 锚点.Y - 4),
					new Point(锚点.X - 1, 锚点.Y - 4),
					new Point(锚点.X, 锚点.Y - 4),
					new Point(锚点.X + 1, 锚点.Y - 4),
					new Point(锚点.X + 2, 锚点.Y - 4),
					new Point(锚点.X + 3, 锚点.Y - 4),
					new Point(锚点.X + 4, 锚点.Y - 4),
					new Point(锚点.X + 4, 锚点.Y - 3),
					new Point(锚点.X + 4, 锚点.Y - 2),
					new Point(锚点.X + 4, 锚点.Y - 1),
					new Point(锚点.X + 4, 锚点.Y),
					new Point(锚点.X + 4, 锚点.Y + 1),
					new Point(锚点.X + 4, 锚点.Y + 2),
					new Point(锚点.X + 4, 锚点.Y + 3),
					new Point(锚点.X + 4, 锚点.Y + 4),
					new Point(锚点.X + 3, 锚点.Y + 4),
					new Point(锚点.X + 2, 锚点.Y + 4),
					new Point(锚点.X + 1, 锚点.Y + 4),
					new Point(锚点.X, 锚点.Y + 4),
					new Point(锚点.X - 1, 锚点.Y + 4),
					new Point(锚点.X - 2, 锚点.Y + 4),
					new Point(锚点.X - 3, 锚点.Y + 4),
					new Point(锚点.X - 4, 锚点.Y + 4),
					new Point(锚点.X - 5, 锚点.Y + 4),
					new Point(锚点.X - 5, 锚点.Y + 3),
					new Point(锚点.X - 5, 锚点.Y + 2),
					new Point(锚点.X - 5, 锚点.Y + 1),
					new Point(锚点.X - 5, 锚点.Y),
					new Point(锚点.X - 5, 锚点.Y - 1),
					new Point(锚点.X - 5, 锚点.Y - 2),
					new Point(锚点.X - 5, 锚点.Y - 3),
					new Point(锚点.X - 5, 锚点.Y - 4),
					new Point(锚点.X - 5, 锚点.Y - 5),
					new Point(锚点.X - 4, 锚点.Y - 5),
					new Point(锚点.X - 3, 锚点.Y - 5),
					new Point(锚点.X - 2, 锚点.Y - 5),
					new Point(锚点.X - 1, 锚点.Y - 5),
					new Point(锚点.X, 锚点.Y - 5),
					new Point(锚点.X + 1, 锚点.Y - 5),
					new Point(锚点.X + 2, 锚点.Y - 5),
					new Point(锚点.X + 3, 锚点.Y - 5),
					new Point(锚点.X + 4, 锚点.Y - 5),
					new Point(锚点.X + 5, 锚点.Y - 5),
					new Point(锚点.X + 5, 锚点.Y - 4),
					new Point(锚点.X + 5, 锚点.Y - 3),
					new Point(锚点.X + 5, 锚点.Y - 2),
					new Point(锚点.X + 5, 锚点.Y - 1),
					new Point(锚点.X + 5, 锚点.Y),
					new Point(锚点.X + 5, 锚点.Y + 1),
					new Point(锚点.X + 5, 锚点.Y + 2),
					new Point(锚点.X + 5, 锚点.Y + 3),
					new Point(锚点.X + 5, 锚点.Y + 4),
					new Point(锚点.X + 5, 锚点.Y + 5),
					new Point(锚点.X + 4, 锚点.Y + 5),
					new Point(锚点.X + 3, 锚点.Y + 5),
					new Point(锚点.X + 2, 锚点.Y + 5),
					new Point(锚点.X + 1, 锚点.Y + 5),
					new Point(锚点.X, 锚点.Y + 5),
					new Point(锚点.X - 1, 锚点.Y + 5),
					new Point(锚点.X - 2, 锚点.Y + 5),
					new Point(锚点.X - 3, 锚点.Y + 5),
					new Point(锚点.X - 4, 锚点.Y + 5),
					new Point(锚点.X - 5, 锚点.Y + 5),
					new Point(锚点.X - 6, 锚点.Y + 5),
					new Point(锚点.X - 6, 锚点.Y + 4),
					new Point(锚点.X - 6, 锚点.Y + 3),
					new Point(锚点.X - 6, 锚点.Y + 2),
					new Point(锚点.X - 6, 锚点.Y + 1),
					new Point(锚点.X - 6, 锚点.Y),
					new Point(锚点.X - 6, 锚点.Y - 1),
					new Point(锚点.X - 6, 锚点.Y - 2),
					new Point(锚点.X - 6, 锚点.Y - 3),
					new Point(锚点.X - 6, 锚点.Y - 4),
					new Point(锚点.X - 6, 锚点.Y - 5),
					new Point(锚点.X - 6, 锚点.Y - 6),
					new Point(锚点.X - 5, 锚点.Y - 6),
					new Point(锚点.X - 4, 锚点.Y - 6),
					new Point(锚点.X - 3, 锚点.Y - 6),
					new Point(锚点.X - 2, 锚点.Y - 6),
					new Point(锚点.X - 1, 锚点.Y - 6),
					new Point(锚点.X, 锚点.Y - 6),
					new Point(锚点.X + 1, 锚点.Y - 6),
					new Point(锚点.X + 2, 锚点.Y - 6),
					new Point(锚点.X + 3, 锚点.Y - 6),
					new Point(锚点.X + 4, 锚点.Y - 6),
					new Point(锚点.X + 5, 锚点.Y - 6),
					new Point(锚点.X + 6, 锚点.Y - 6),
					new Point(锚点.X + 6, 锚点.Y - 5),
					new Point(锚点.X + 6, 锚点.Y - 4),
					new Point(锚点.X + 6, 锚点.Y - 3),
					new Point(锚点.X + 6, 锚点.Y - 2),
					new Point(锚点.X + 6, 锚点.Y - 1),
					new Point(锚点.X + 6, 锚点.Y),
					new Point(锚点.X + 6, 锚点.Y + 1),
					new Point(锚点.X + 6, 锚点.Y + 2),
					new Point(锚点.X + 6, 锚点.Y + 3),
					new Point(锚点.X + 6, 锚点.Y + 4),
					new Point(锚点.X + 6, 锚点.Y + 5),
					new Point(锚点.X + 6, 锚点.Y + 6),
					new Point(锚点.X + 5, 锚点.Y + 6),
					new Point(锚点.X + 4, 锚点.Y + 6),
					new Point(锚点.X + 3, 锚点.Y + 6),
					new Point(锚点.X + 2, 锚点.Y + 6),
					new Point(锚点.X + 1, 锚点.Y + 6),
					new Point(锚点.X, 锚点.Y + 6),
					new Point(锚点.X - 1, 锚点.Y + 6),
					new Point(锚点.X - 2, 锚点.Y + 6),
					new Point(锚点.X - 3, 锚点.Y + 6),
					new Point(锚点.X - 4, 锚点.Y + 6),
					new Point(锚点.X - 5, 锚点.Y + 6),
					new Point(锚点.X - 6, 锚点.Y + 6),
					new Point(锚点.X - 7, 锚点.Y + 6),
					new Point(锚点.X - 7, 锚点.Y + 5),
					new Point(锚点.X - 7, 锚点.Y + 4),
					new Point(锚点.X - 7, 锚点.Y + 3),
					new Point(锚点.X - 7, 锚点.Y + 2),
					new Point(锚点.X - 7, 锚点.Y + 1),
					new Point(锚点.X - 7, 锚点.Y),
					new Point(锚点.X - 7, 锚点.Y - 1),
					new Point(锚点.X - 7, 锚点.Y - 2),
					new Point(锚点.X - 7, 锚点.Y - 3),
					new Point(锚点.X - 7, 锚点.Y - 4),
					new Point(锚点.X - 7, 锚点.Y - 5),
					new Point(锚点.X - 7, 锚点.Y - 6),
					new Point(锚点.X - 7, 锚点.Y - 7),
					new Point(锚点.X - 6, 锚点.Y - 7),
					new Point(锚点.X - 5, 锚点.Y - 7),
					new Point(锚点.X - 4, 锚点.Y - 7),
					new Point(锚点.X - 3, 锚点.Y - 7),
					new Point(锚点.X - 2, 锚点.Y - 7),
					new Point(锚点.X - 1, 锚点.Y - 7),
					new Point(锚点.X, 锚点.Y - 7),
					new Point(锚点.X + 1, 锚点.Y - 7),
					new Point(锚点.X + 2, 锚点.Y - 7),
					new Point(锚点.X + 3, 锚点.Y - 7),
					new Point(锚点.X + 4, 锚点.Y - 7),
					new Point(锚点.X + 5, 锚点.Y - 7),
					new Point(锚点.X + 6, 锚点.Y - 7),
					new Point(锚点.X + 7, 锚点.Y - 7),
					new Point(锚点.X + 7, 锚点.Y - 6),
					new Point(锚点.X + 7, 锚点.Y - 5),
					new Point(锚点.X + 7, 锚点.Y - 4),
					new Point(锚点.X + 7, 锚点.Y - 3),
					new Point(锚点.X + 7, 锚点.Y - 2),
					new Point(锚点.X + 7, 锚点.Y - 1),
					new Point(锚点.X + 7, 锚点.Y),
					new Point(锚点.X + 7, 锚点.Y + 1),
					new Point(锚点.X + 7, 锚点.Y + 2),
					new Point(锚点.X + 7, 锚点.Y + 3),
					new Point(锚点.X + 7, 锚点.Y + 4),
					new Point(锚点.X + 7, 锚点.Y + 5),
					new Point(锚点.X + 7, 锚点.Y + 6),
					new Point(锚点.X + 7, 锚点.Y + 7),
					new Point(锚点.X + 6, 锚点.Y + 7),
					new Point(锚点.X + 5, 锚点.Y + 7),
					new Point(锚点.X + 4, 锚点.Y + 7),
					new Point(锚点.X + 3, 锚点.Y + 7),
					new Point(锚点.X + 2, 锚点.Y + 7),
					new Point(锚点.X + 1, 锚点.Y + 7),
					new Point(锚点.X, 锚点.Y + 7),
					new Point(锚点.X - 1, 锚点.Y + 7),
					new Point(锚点.X - 2, 锚点.Y + 7),
					new Point(锚点.X - 3, 锚点.Y + 7),
					new Point(锚点.X - 4, 锚点.Y + 7),
					new Point(锚点.X - 5, 锚点.Y + 7),
					new Point(锚点.X - 6, 锚点.Y + 7),
					new Point(锚点.X - 7, 锚点.Y + 7),
					new Point(锚点.X - 8, 锚点.Y + 7),
					new Point(锚点.X - 8, 锚点.Y + 6),
					new Point(锚点.X - 8, 锚点.Y + 5),
					new Point(锚点.X - 8, 锚点.Y + 4),
					new Point(锚点.X - 8, 锚点.Y + 3),
					new Point(锚点.X - 8, 锚点.Y + 2),
					new Point(锚点.X - 8, 锚点.Y + 1),
					new Point(锚点.X - 8, 锚点.Y),
					new Point(锚点.X - 8, 锚点.Y - 1),
					new Point(锚点.X - 8, 锚点.Y - 2),
					new Point(锚点.X - 8, 锚点.Y - 3),
					new Point(锚点.X - 8, 锚点.Y - 4),
					new Point(锚点.X - 8, 锚点.Y - 5),
					new Point(锚点.X - 8, 锚点.Y - 6),
					new Point(锚点.X - 8, 锚点.Y - 7),
					new Point(锚点.X - 8, 锚点.Y - 8),
					new Point(锚点.X - 7, 锚点.Y - 8),
					new Point(锚点.X - 6, 锚点.Y - 8),
					new Point(锚点.X - 5, 锚点.Y - 8),
					new Point(锚点.X - 4, 锚点.Y - 8),
					new Point(锚点.X - 3, 锚点.Y - 8),
					new Point(锚点.X - 2, 锚点.Y - 8),
					new Point(锚点.X - 1, 锚点.Y - 8),
					new Point(锚点.X, 锚点.Y - 8),
					new Point(锚点.X + 1, 锚点.Y - 8),
					new Point(锚点.X + 2, 锚点.Y - 8),
					new Point(锚点.X + 3, 锚点.Y - 8),
					new Point(锚点.X + 4, 锚点.Y - 8),
					new Point(锚点.X + 5, 锚点.Y - 8),
					new Point(锚点.X + 6, 锚点.Y - 8),
					new Point(锚点.X + 7, 锚点.Y - 8),
					new Point(锚点.X + 8, 锚点.Y - 8),
					new Point(锚点.X + 8, 锚点.Y - 7),
					new Point(锚点.X + 8, 锚点.Y - 6),
					new Point(锚点.X + 8, 锚点.Y - 5),
					new Point(锚点.X + 8, 锚点.Y - 4),
					new Point(锚点.X + 8, 锚点.Y - 3),
					new Point(锚点.X + 8, 锚点.Y - 2),
					new Point(锚点.X + 8, 锚点.Y - 1),
					new Point(锚点.X + 8, 锚点.Y),
					new Point(锚点.X + 8, 锚点.Y + 1),
					new Point(锚点.X + 8, 锚点.Y + 2),
					new Point(锚点.X + 8, 锚点.Y + 3),
					new Point(锚点.X + 8, 锚点.Y + 4),
					new Point(锚点.X + 8, 锚点.Y + 5),
					new Point(锚点.X + 8, 锚点.Y + 6),
					new Point(锚点.X + 8, 锚点.Y + 7),
					new Point(锚点.X + 8, 锚点.Y + 8),
					new Point(锚点.X + 7, 锚点.Y + 8),
					new Point(锚点.X + 6, 锚点.Y + 8),
					new Point(锚点.X + 5, 锚点.Y + 8),
					new Point(锚点.X + 4, 锚点.Y + 8),
					new Point(锚点.X + 3, 锚点.Y + 8),
					new Point(锚点.X + 2, 锚点.Y + 8),
					new Point(锚点.X + 1, 锚点.Y + 8),
					new Point(锚点.X, 锚点.Y + 8),
					new Point(锚点.X - 1, 锚点.Y + 8),
					new Point(锚点.X - 2, 锚点.Y + 8),
					new Point(锚点.X - 3, 锚点.Y + 8),
					new Point(锚点.X - 4, 锚点.Y + 8),
					new Point(锚点.X - 5, 锚点.Y + 8),
					new Point(锚点.X - 6, 锚点.Y + 8),
					new Point(锚点.X - 7, 锚点.Y + 8),
					new Point(锚点.X - 8, 锚点.Y + 8)
				}, 
				技能范围类型.线型1x6 => new Point[6]
				{
					锚点,
					计算类.前方坐标(锚点, 方向, 1),
					计算类.前方坐标(锚点, 方向, 2),
					计算类.前方坐标(锚点, 方向, 3),
					计算类.前方坐标(锚点, 方向, 4),
					计算类.前方坐标(锚点, 方向, 5)
				}, 
				技能范围类型.线型3x5 => 方向 switch
				{
					游戏方向.上方 => new Point[15]
					{
						锚点,
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y + 2),
						new Point(锚点.X + 1, 锚点.Y + 2),
						new Point(锚点.X - 1, 锚点.Y + 2),
						new Point(锚点.X, 锚点.Y + 3),
						new Point(锚点.X + 1, 锚点.Y + 3),
						new Point(锚点.X - 1, 锚点.Y + 3),
						new Point(锚点.X, 锚点.Y + 4),
						new Point(锚点.X + 1, 锚点.Y + 4),
						new Point(锚点.X - 1, 锚点.Y + 4)
					}, 
					游戏方向.左上 => new Point[15]
					{
						锚点,
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 2, 锚点.Y + 2),
						new Point(锚点.X + 2, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y + 2),
						new Point(锚点.X + 3, 锚点.Y + 3),
						new Point(锚点.X + 3, 锚点.Y + 2),
						new Point(锚点.X + 2, 锚点.Y + 3),
						new Point(锚点.X + 4, 锚点.Y + 4),
						new Point(锚点.X + 4, 锚点.Y + 3),
						new Point(锚点.X + 3, 锚点.Y + 4)
					}, 
					游戏方向.左方 => new Point[15]
					{
						锚点,
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y + 1),
						new Point(锚点.X + 2, 锚点.Y),
						new Point(锚点.X + 2, 锚点.Y - 1),
						new Point(锚点.X + 2, 锚点.Y + 1),
						new Point(锚点.X + 3, 锚点.Y),
						new Point(锚点.X + 3, 锚点.Y - 1),
						new Point(锚点.X + 3, 锚点.Y + 1),
						new Point(锚点.X + 4, 锚点.Y),
						new Point(锚点.X + 4, 锚点.Y - 1),
						new Point(锚点.X + 4, 锚点.Y + 1)
					}, 
					游戏方向.右方 => new Point[15]
					{
						锚点,
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X - 2, 锚点.Y),
						new Point(锚点.X - 2, 锚点.Y + 1),
						new Point(锚点.X - 2, 锚点.Y - 1),
						new Point(锚点.X - 3, 锚点.Y),
						new Point(锚点.X - 3, 锚点.Y + 1),
						new Point(锚点.X - 3, 锚点.Y - 1),
						new Point(锚点.X - 4, 锚点.Y),
						new Point(锚点.X - 4, 锚点.Y + 1),
						new Point(锚点.X - 4, 锚点.Y - 1)
					}, 
					游戏方向.右上 => new Point[15]
					{
						锚点,
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y + 1),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X - 2, 锚点.Y + 2),
						new Point(锚点.X - 1, 锚点.Y + 2),
						new Point(锚点.X - 2, 锚点.Y + 1),
						new Point(锚点.X - 3, 锚点.Y + 3),
						new Point(锚点.X - 2, 锚点.Y + 3),
						new Point(锚点.X - 3, 锚点.Y + 2),
						new Point(锚点.X - 4, 锚点.Y + 4),
						new Point(锚点.X - 3, 锚点.Y + 4),
						new Point(锚点.X - 4, 锚点.Y + 3)
					}, 
					游戏方向.左下 => new Point[15]
					{
						锚点,
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X + 2, 锚点.Y - 2),
						new Point(锚点.X + 1, 锚点.Y - 2),
						new Point(锚点.X + 2, 锚点.Y - 1),
						new Point(锚点.X + 3, 锚点.Y - 3),
						new Point(锚点.X + 2, 锚点.Y - 3),
						new Point(锚点.X + 3, 锚点.Y - 2),
						new Point(锚点.X + 4, 锚点.Y - 4),
						new Point(锚点.X + 3, 锚点.Y - 4),
						new Point(锚点.X + 4, 锚点.Y - 3)
					}, 
					游戏方向.下方 => new Point[15]
					{
						锚点,
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X + 1, 锚点.Y - 1),
						new Point(锚点.X, 锚点.Y - 2),
						new Point(锚点.X - 1, 锚点.Y - 2),
						new Point(锚点.X + 1, 锚点.Y - 2),
						new Point(锚点.X, 锚点.Y - 3),
						new Point(锚点.X - 1, 锚点.Y - 3),
						new Point(锚点.X + 1, 锚点.Y - 3),
						new Point(锚点.X, 锚点.Y - 4),
						new Point(锚点.X - 1, 锚点.Y - 4),
						new Point(锚点.X + 1, 锚点.Y - 4)
					}, 
					_ => new Point[15]
					{
						锚点,
						new Point(锚点.X, 锚点.Y + 1),
						new Point(锚点.X + 1, 锚点.Y),
						new Point(锚点.X - 1, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y),
						new Point(锚点.X, 锚点.Y - 1),
						new Point(锚点.X - 2, 锚点.Y - 2),
						new Point(锚点.X - 2, 锚点.Y - 1),
						new Point(锚点.X - 1, 锚点.Y - 2),
						new Point(锚点.X - 3, 锚点.Y - 3),
						new Point(锚点.X - 3, 锚点.Y - 2),
						new Point(锚点.X - 2, 锚点.Y - 3),
						new Point(锚点.X - 4, 锚点.Y - 4),
						new Point(锚点.X - 4, 锚点.Y - 3),
						new Point(锚点.X - 3, 锚点.Y - 4)
					}, 
				}, 
				技能范围类型.线型1x4 => new Point[4]
				{
					锚点,
					计算类.前方坐标(锚点, 方向, 1),
					计算类.前方坐标(锚点, 方向, 2),
					计算类.前方坐标(锚点, 方向, 3)
				}, 
				技能范围类型.线型1x10 => new Point[10]
				{
					锚点,
					计算类.前方坐标(锚点, 方向, 1),
					计算类.前方坐标(锚点, 方向, 2),
					计算类.前方坐标(锚点, 方向, 3),
					计算类.前方坐标(锚点, 方向, 4),
					计算类.前方坐标(锚点, 方向, 5),
					计算类.前方坐标(锚点, 方向, 6),
					计算类.前方坐标(锚点, 方向, 7),
					计算类.前方坐标(锚点, 方向, 8),
					计算类.前方坐标(锚点, 方向, 9)
				}, 
				_ => new Point[0], 
			};
		}

		static 计算类()
		{
			计算类.crcTable = new uint[256]
			{
				0u, 1996959894u, 3993919788u, 2567524794u, 124634137u, 1886057615u, 3915621685u, 2657392035u, 249268274u, 2044508324u,
				3772115230u, 2547177864u, 162941995u, 2125561021u, 3887607047u, 2428444049u, 498536548u, 1789927666u, 4089016648u, 2227061214u,
				450548861u, 1843258603u, 4107580753u, 2211677639u, 325883990u, 1684777152u, 4251122042u, 2321926636u, 335633487u, 1661365465u,
				4195302755u, 2366115317u, 997073096u, 1281953886u, 3579855332u, 2724688242u, 1006888145u, 1258607687u, 3524101629u, 2768942443u,
				901097722u, 1119000684u, 3686517206u, 2898065728u, 853044451u, 1172266101u, 3705015759u, 2882616665u, 651767980u, 1373503546u,
				3369554304u, 3218104598u, 565507253u, 1454621731u, 3485111705u, 3099436303u, 671266974u, 1594198024u, 3322730930u, 2970347812u,
				795835527u, 1483230225u, 3244367275u, 3060149565u, 1994146192u, 31158534u, 2563907772u, 4023717930u, 1907459465u, 112637215u,
				2680153253u, 3904427059u, 2013776290u, 251722036u, 2517215374u, 3775830040u, 2137656763u, 141376813u, 2439277719u, 3865271297u,
				1802195444u, 476864866u, 2238001368u, 4066508878u, 1812370925u, 453092731u, 2181625025u, 4111451223u, 1706088902u, 314042704u,
				2344532202u, 4240017532u, 1658658271u, 366619977u, 2362670323u, 4224994405u, 1303535960u, 984961486u, 2747007092u, 3569037538u,
				1256170817u, 1037604311u, 2765210733u, 3554079995u, 1131014506u, 879679996u, 2909243462u, 3663771856u, 1141124467u, 855842277u,
				2852801631u, 3708648649u, 1342533948u, 654459306u, 3188396048u, 3373015174u, 1466479909u, 544179635u, 3110523913u, 3462522015u,
				1591671054u, 702138776u, 2966460450u, 3352799412u, 1504918807u, 783551873u, 3082640443u, 3233442989u, 3988292384u, 2596254646u,
				62317068u, 1957810842u, 3939845945u, 2647816111u, 81470997u, 1943803523u, 3814918930u, 2489596804u, 225274430u, 2053790376u,
				3826175755u, 2466906013u, 167816743u, 2097651377u, 4027552580u, 2265490386u, 503444072u, 1762050814u, 4150417245u, 2154129355u,
				426522225u, 1852507879u, 4275313526u, 2312317920u, 282753626u, 1742555852u, 4189708143u, 2394877945u, 397917763u, 1622183637u,
				3604390888u, 2714866558u, 953729732u, 1340076626u, 3518719985u, 2797360999u, 1068828381u, 1219638859u, 3624741850u, 2936675148u,
				906185462u, 1090812512u, 3747672003u, 2825379669u, 829329135u, 1181335161u, 3412177804u, 3160834842u, 628085408u, 1382605366u,
				3423369109u, 3138078467u, 570562233u, 1426400815u, 3317316542u, 2998733608u, 733239954u, 1555261956u, 3268935591u, 3050360625u,
				752459403u, 1541320221u, 2607071920u, 3965973030u, 1969922972u, 40735498u, 2617837225u, 3943577151u, 1913087877u, 83908371u,
				2512341634u, 3803740692u, 2075208622u, 213261112u, 2463272603u, 3855990285u, 2094854071u, 198958881u, 2262029012u, 4057260610u,
				1759359992u, 534414190u, 2176718541u, 4139329115u, 1873836001u, 414664567u, 2282248934u, 4279200368u, 1711684554u, 285281116u,
				2405801727u, 4167216745u, 1634467795u, 376229701u, 2685067896u, 3608007406u, 1308918612u, 956543938u, 2808555105u, 3495958263u,
				1231636301u, 1047427035u, 2932959818u, 3654703836u, 1088359270u, 936918000u, 2847714899u, 3736837829u, 1202900863u, 817233897u,
				3183342108u, 3401237130u, 1404277552u, 615818150u, 3134207493u, 3453421203u, 1423857449u, 601450431u, 3009837614u, 3294710456u,
				1567103746u, 711928724u, 3020668471u, 3272380065u, 1510334235u, 755167117u
			};
			计算类.系统相对时间 = Convert.ToDateTime("1970-01-01 08:00:00");
		}

		public static List<T> RandomSort<T>(List<T> list)
		{
			// Fisher-Yates 原地洗牌(O(n)), 替代原 List.Insert 随机插入(O(n²)); 仍返回新列表、不改入参, 契约不变.
			Random random;
			random = new Random();
			List<T> list2;
			list2 = new List<T>(list);
			for (int i = list2.Count - 1; i > 0; i--)
			{
				int j = random.Next(i + 1);
				T temp = list2[i];
				list2[i] = list2[j];
				list2[j] = temp;
			}
			return list2;
		}

		public static int CRC(byte[] bytes)
		{
			int num;
			num = bytes.Length;
			uint num2;
			num2 = uint.MaxValue;
			for (int i = 0; i < num; i++)
			{
				num2 = ((num2 >> 8) & 0xFFFFFFu) ^ 计算类.crcTable[(num2 ^ bytes[i]) & 0xFF];
			}
			return (int)num2 ^ -1;
		}
	}
}
