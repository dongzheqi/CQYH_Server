using System.Collections.Generic;
using System.Drawing;
using 游戏服务器.地图类;

namespace 游戏服务器.副本类
{
	public static class 学宫
	{
		public static void 执行(地图实例 地图)
		{
			if (!(主程.当前时间 > 地图.节点计时))
			{
				return;
			}
			if (地图.玩家列表.Count == 0)
			{
				地图.副本节点 = 255;
			}
			if (地图.副本节点 == 0)
			{
				地图.节点计时 = 主程.当前时间.AddSeconds(1.0);
			}
			else if (地图.副本节点 == 1)
			{
				地图.副本节点 = 2;
				地图.节点计时 = 主程.当前时间.AddSeconds(1.0);
			}
			else if (地图.副本节点 == 2)
			{
				副本.范围刷新怪物从地图实例("白野猪98", 地图, 0, new Point(633, 506), 禁止复活: true, 立即刷新: true);
				副本.范围刷新怪物从地图实例("白野猪98", 地图, 0, new Point(631, 504), 禁止复活: true, 立即刷新: true);
				地图.副本节点 = 3;
				地图.节点计时 = 主程.当前时间.AddSeconds(1.0);
			}
			else if (地图.副本节点 == 3 && 地图.存活怪物总数 == 0)
			{
				地图.副本节点 = 254;
				if (地图.玩家列表.Count > 0)
				{
					List<玩家实例> list;
					list = 地图.获取玩家列表();
					if (list != null && list.Count > 0)
					{
						list[0].玩家获得物品(Settings.学宫奖励道具一, Settings.学宫奖励数量[0, 0], "挑战学宫给予");
						list[0].玩家获得物品(Settings.学宫奖励道具二, Settings.学宫奖励数量[0, 1], "挑战学宫给予");
					}
				}
				副本.刷新守卫从地图实例(6297, 地图, 游戏方向.下方, new Point(636, 503));
				地图.节点计时 = 主程.当前时间.AddSeconds(60.0);
			}
			else if (地图.副本节点 == 10)
			{
				地图.副本节点 = 11;
				地图.节点计时 = 主程.当前时间.AddSeconds(1.0);
			}
			else if (地图.副本节点 == 11)
			{
				副本.范围刷新怪物从地图实例("白野猪98", 地图, 0, new Point(633, 506), 禁止复活: true, 立即刷新: true);
				副本.范围刷新怪物从地图实例("白野猪98", 地图, 0, new Point(631, 504), 禁止复活: true, 立即刷新: true);
				副本.范围刷新怪物从地图实例("白野猪98", 地图, 0, new Point(632, 505), 禁止复活: true, 立即刷新: true);
				地图.副本节点 = 12;
				地图.节点计时 = 主程.当前时间.AddSeconds(1.0);
			}
			else if (地图.副本节点 == 12 && 地图.存活怪物总数 == 0)
			{
				地图.副本节点 = 254;
				if (地图.玩家列表.Count > 0)
				{
					List<玩家实例> list2;
					list2 = 地图.获取玩家列表();
					if (list2 != null && list2.Count > 0)
					{
						list2[0].玩家获得物品(Settings.学宫奖励道具一, Settings.学宫奖励数量[1, 0], "挑战学宫给予");
						list2[0].玩家获得物品(Settings.学宫奖励道具二, Settings.学宫奖励数量[1, 1], "挑战学宫给予");
					}
				}
				副本.刷新守卫从地图实例(6297, 地图, 游戏方向.下方, new Point(636, 503));
				地图.节点计时 = 主程.当前时间.AddSeconds(60.0);
			}
			else if (地图.副本节点 == 100)
			{
				地图.副本节点 = 101;
				地图.节点计时 = 主程.当前时间.AddSeconds(1.0);
			}
			else if (地图.副本节点 == 101)
			{
				副本.范围刷新怪物从地图实例("白野猪98", 地图, 0, new Point(633, 506), 禁止复活: true, 立即刷新: true);
				副本.范围刷新怪物从地图实例("白野猪98", 地图, 0, new Point(631, 504), 禁止复活: true, 立即刷新: true);
				副本.范围刷新怪物从地图实例("白野猪98", 地图, 0, new Point(633, 505), 禁止复活: true, 立即刷新: true);
				副本.范围刷新怪物从地图实例("白野猪98", 地图, 0, new Point(632, 505), 禁止复活: true, 立即刷新: true);
				地图.副本节点 = 102;
				地图.节点计时 = 主程.当前时间.AddSeconds(1.0);
			}
			else if (地图.副本节点 == 102 && 地图.存活怪物总数 == 0)
			{
				地图.副本节点 = 254;
				if (地图.玩家列表.Count > 0)
				{
					List<玩家实例> list3;
					list3 = 地图.获取玩家列表();
					if (list3 != null && list3.Count > 0)
					{
						list3[0].玩家获得物品(Settings.学宫奖励道具一, Settings.学宫奖励数量[2, 0], "挑战学宫给予");
						list3[0].玩家获得物品(Settings.学宫奖励道具二, Settings.学宫奖励数量[2, 1], "挑战学宫给予");
					}
				}
				副本.刷新守卫从地图实例(6297, 地图, 游戏方向.下方, new Point(636, 503));
				地图.节点计时 = 主程.当前时间.AddSeconds(60.0);
			}
			else if (地图.副本节点 == 254)
			{
				List<玩家实例> list4;
				list4 = 地图.获取玩家列表();
				if (list4 != null && list4.Count > 0)
				{
					foreach (玩家实例 item in list4)
					{
						item.玩家切换地图(147, 0, 0, 1);
					}
				}
				地图.副本节点 = 255;
				地图.节点计时 = 主程.当前时间.AddSeconds(1.0);
			}
			else if (地图.副本节点 == 255)
			{
				地图.关闭副本();
			}
			else
			{
				地图.节点计时 = 主程.当前时间.AddSeconds(1.0);
			}
		}
	}
}
