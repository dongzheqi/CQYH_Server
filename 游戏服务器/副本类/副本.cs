using System.Collections.Generic;
using System.Drawing;
using 游戏服务器.地图类;
using 游戏服务器.模板类;
using 游戏服务器.数据类;

namespace 游戏服务器.副本类
{
	public static class 副本
	{
		public const int 数字_妖塔祭坛类型 = 0;

		public const int 数字_妖塔陷阱类型 = 1;

		public const int 数字_妖塔开始时间 = 2;

		public const int 数字_妖塔单层时间 = 3;

		public const int 数字_妖塔级别 = 4;

		public const int 数字_魔塔级别 = 5;

		public const int 数字_妖塔每日奖励层数 = 14;

		public const int 数字_妖塔个人层数记录 = 15;

		// 槽位 16/17: 玩家脚本变量(字典监视器<int,int>)专用日计数槽。
		// 原值 1/2 与 const.lua 的 数字_魔虫窟每日进入次数=1、数字_妖塔随机到的层数=2 物理碰撞，
		// 现改到未占用的 16/17 空档(夹在 数字_妖塔个人层数记录=15 与 数字_魔塔个人标准层数记录=20 之间)，
		// 并在 游戏脚本.初始化脚本系统() 中注入为 Lua 全局，供版本脚本 6350/6359.lua 使用。
		public static int 数字_未知一层每日进入次数 = 16;

		public static int 数字_未知二层每日进入次数 = 17;

		public static int 数字_兑换次数 = 10;

		public static int 数字_玛法福利官领取次数 = 11;

		public static int 数字_屠魔令每日清零时间 = 12;

		public static int 数字_屠魔令兑换经验次数 = 13;

		public static int 数字_魔塔个人标准层数记录 = 20;

		public static int 数字_魔塔个人进阶层数记录 = 21;

		public static int 数字_魔塔个人秘境层数记录 = 22;

		public static int 数字_魔塔个人每日领取记录 = 23;

		public static int 数字_屠魔大厅普通层数记录 = 24;

		public static int 数字_屠魔大厅标准层数记录 = 25;

		public static int 数字_屠魔大厅进阶层数记录 = 26;

		public static int 数字_屠魔大厅每日领取记录 = 27;

		public static int 数字_蜘蛛长廊标准层数记录 = 28;

		public static int 数字_蜘蛛长廊进阶层数记录 = 29;

		public static int 数字_蜘蛛长廊困难层数记录 = 30;

		public static int 数字_蜘蛛长廊每日领取记录 = 31;

		public static int 数字_蜘蛛长廊每日进入次数 = 32;

		public static int 数字_副本临时领取开关 = 33;

		public static int 数字_传承明珠兑换次数 = 105;

		public static int 数字_传承明珠清零时间 = 105;

		public static int 数字_每日免费清零时间 = 106;

		public static int 数字_每日收费清零时间 = 107;

		public static int 数字_幽冥海五层进入次数 = 109;

		public static int 数字_赤月群岛阵营冷却 = 110;

		public static int 数字_遗落寺庙每日清零 = 111;

		public static int 数字_遗落寺庙进入次数 = 112;

		public static int 数字_重铸武器装备次数 = 150;

		public static int 数字_重铸头盔装备次数 = 151;

		public static int 数字_重铸衣服装备次数 = 153;

		public static int 数字_重铸项链装备次数 = 158;

		public static int 数字_重铸戒指装备次数 = 159;

		public static int 数字_重铸手镯装备次数 = 160;

		public static int 数字_重铸技能次数 = 152;

		public static int 数字_高级重铸技能次数 = 154;

		public static int 数字_双节节日奖励 = 200;

		public static int 数字_一觉每日任务标志 = 201;

		public static int 数字_二觉每日任务标志 = 202;

		public static int 数字_三觉每日任务标志 = 203;

		public static int 数字_一觉付费任务标志 = 204;

		public static int 数字_二觉付费任务标志 = 205;

		public static int 数字_三觉付费任务标志 = 203;

		public static int 数字_装备精粹购买次数 = 254;

		public static int 数字_特权战神油领取 = 255;

		public static int 数字_屠魔大厅级别 = 6;

		public static int 数字_屠魔大厅普通层数 = 7;

		public static int 数字_屠魔秘境标准层数 = 8;

		public static int 数字_屠魔秘境进阶层数 = 9;

		public static int 数字_蜘蛛长廊级别 = 10;

		public static int 数字_未知暗殿存在时间 = 11;

		public static int 数字_尸王殿开启计数 = 3000;

		public static int 数字_双头血魔计数 = 2000;

		public static int 数字_双头金刚计数 = 2000;

		public static int CONST_PLAYER_SPAWNMAP = 142;

		public static int 数字_妖塔每日进入次数 = 218;

		public static int 数字_魔虫窟每日进入次数 = 232;

		public static int 数字_屠魔大厅每日领取 = 416;

		public static int 怪物攻城刷怪编号 = 0;

		private static int 数字_陷阱刷新间隔 = 500;

		public static string 从表随机取值(string[] strings)
		{
			return strings[主程.随机数.Next(strings.Length)];
		}

		private static int randomNum(int numMin, int numMax)
		{
			return 主程.随机数.Next(numMin, numMax);
		}

		public static 怪物刷新 获取副本怪物区域(地图实例 地图, string 区域名字)
		{
			foreach (怪物刷新 item in 地图.获取怪物区域())
			{
				if (item.区域名字 == 区域名字)
				{
					return item;
				}
			}
			return null;
		}

		public static bool 范围刷新怪物从地图实例(string 怪物名字, 地图实例 地图, int 复活间隔, Point[] 刷新范围, bool 禁止复活, bool 立即刷新)
		{
			return 地图.范围刷新怪物(怪物名字, 复活间隔, 刷新范围, 禁止复活, 立即刷新);
		}

		public static bool 范围刷新怪物从地图实例(string 怪物名字, 地图实例 地图, int 复活间隔, Point 刷新范围, bool 禁止复活, bool 立即刷新)
		{
			return 副本.范围刷新怪物从地图实例(怪物名字, 地图, 复活间隔, new Point[1] { 刷新范围 }, 禁止复活, 立即刷新);
		}

		public static 守卫实例 刷新守卫从地图实例(int 守卫编号, 地图实例 地图, 游戏方向 出生方向, Point 出生坐标)
		{
			if (!地图守卫.数据表.TryGetValue((ushort)守卫编号, out var value))
			{
				return null;
			}
			return new 守卫实例(value, 地图, 出生方向, 出生坐标);
		}

		public static List<玩家实例> 获取在线队友(玩家实例 玩家)
		{
			List<玩家实例> list;
			list = new List<玩家实例>();
			foreach (角色数据 item in 玩家.队友数据)
			{
				if (item.网络连接 != null)
				{
					list.Add(item.网络连接.绑定角色);
				}
			}
			return list;
		}

		public static void 九层妖塔创建(地图实例 妖塔, 玩家实例 玩家)
		{
			妖塔.数字变量[4] = 0;
			妖塔.节点计时 = 主程.当前时间.AddSeconds(5.0);
			玩家.玩家切换地图(妖塔, 地图区域类型.未知区域, new Point(1028, 148));
			玩家.修改角色变量(副本.数字_妖塔每日进入次数, 计算类.时间转换(主程.当前时间.Date));
			if (玩家.开启七天乐)
			{
				玩家.修改七天进度(38, 玩家.角色数据.七天进度[38] + 1);
				玩家.修改七天进度(43, 玩家.角色数据.七天进度[43] + 1);
			}
		}

		public static void 九层魔塔创建(地图实例 魔塔, 玩家实例 玩家, int 副本参数)
		{
			魔塔.数字变量[5] = 副本参数;
			魔塔.节点计时 = 主程.当前时间.AddSeconds(5.0);
			foreach (角色数据 item in 玩家.队友数据)
			{
				if (item.网络连接 != null)
				{
					玩家实例 绑定角色;
					绑定角色 = item.网络连接.绑定角色;
					绑定角色.扣金币(50000u);
					绑定角色.玩家切换地图(魔塔, 地图区域类型.未知区域, new Point(1031, 74));
				}
			}
		}

		public static void 魔塔秘境创建(地图实例 魔塔秘境, 玩家实例 玩家, int 副本参数)
		{
			魔塔秘境.节点计时 = 主程.当前时间.AddSeconds(5.0);
			foreach (玩家实例 item in 副本.获取在线队友(玩家))
			{
				item.扣金币(50000u);
				item.玩家切换地图(魔塔秘境, 地图区域类型.未知区域, new Point(1031, 74));
			}
		}

		public static void 屠魔大厅创建(地图实例 屠魔大厅, 玩家实例 玩家, int 副本参数)
		{
			屠魔大厅.节点计时 = 主程.当前时间.AddSeconds(5.0);
			foreach (玩家实例 item in 副本.获取在线队友(玩家))
			{
				List<物品数据> 物品列表;
				物品列表 = item.查找背包物品(90226, 3);
				if (玩家.对话页面 != 1111 && !((玩家.对话页面 == 1211) | (玩家.对话页面 == 1311)))
				{
					item.消耗背包物品(3, 物品列表);
				}
				else
				{
					item.扣金币(100000u);
				}
				if (玩家.对话页面 != 1111 && 玩家.对话页面 != 1121)
				{
					item.玩家切换地图(屠魔大厅, 地图区域类型.未知区域, new Point(1092, 362));
				}
				else
				{
					item.玩家切换地图(屠魔大厅, 地图区域类型.传送区域);
				}
				if (item.开启七天乐)
				{
					item.修改七天进度(58, 玩家.角色数据.七天进度[58] + 1);
				}
			}
		}

		public static void 蜘蛛长廊创建(地图实例 蜘蛛长廊, 玩家实例 玩家, int 副本参数)
		{
			蜘蛛长廊.数字变量[副本.数字_蜘蛛长廊级别] = 0;
			if (玩家.对话页面 != 311 && 玩家.对话页面 != 312)
			{
				if (玩家.对话页面 != 321 && 玩家.对话页面 != 322)
				{
					if (玩家.对话页面 == 331 || 玩家.对话页面 == 332)
					{
						蜘蛛长廊.数字变量[副本.数字_蜘蛛长廊级别] = 2;
					}
				}
				else
				{
					蜘蛛长廊.数字变量[副本.数字_蜘蛛长廊级别] = 1;
				}
			}
			else
			{
				蜘蛛长廊.数字变量[副本.数字_蜘蛛长廊级别] = 0;
			}
			蜘蛛长廊.节点计时 = 主程.当前时间.AddSeconds(10.0);
			foreach (玩家实例 item in 副本.获取在线队友(玩家))
			{
				List<物品数据> list;
				list = item.查找背包物品(90226, 3);
				if (玩家.对话页面 != 311 && 玩家.对话页面 != 321 && 玩家.对话页面 != 331)
				{
					if (list != null)
					{
						item.消耗背包物品(3, list);
					}
				}
				else
				{
					item.扣金币(100000u);
				}
				item.角色数据.脚本变量[副本.数字_蜘蛛长廊每日进入次数] = 1;
				item.玩家切换地图(蜘蛛长廊, 地图区域类型.未知区域, new Point(887, 284));
			}
		}

		public static void 未知暗殿创建(地图实例 未知暗殿, 玩家实例 玩家, int 副本参数)
		{
			未知暗殿.节点计时 = 主程.当前时间.AddSeconds(1.0);
			玩家.消耗背包物品(1, 玩家.查找背包物品(90235, 1));
			foreach (玩家实例 item in 副本.获取在线队友(玩家))
			{
				玩家.角色数据.脚本变量[副本.数字_未知二层每日进入次数] = 主程.当前时间.DayOfYear;
				item.玩家切换地图(未知暗殿, 地图区域类型.未知区域, new Point(1142, 211));
			}
		}

		public static void 学宫创建(地图实例 学宫, 玩家实例 玩家, int 副本参数)
		{
			玩家.玩家切换地图(学宫, 地图区域类型.未知区域, new Point(635, 504));
		}

		public static void 脚本创建(地图实例 A副本, 玩家实例 玩家, int 副本参数)
		{
		}

		public static bool CallNPCMain(玩家实例 玩家, 守卫实例 对话守卫, out string SAY)
		{
			SAY = string.Empty;
			return false;
		}
	}
}
