using System.Collections.Generic;
using System.Drawing;
using 游戏服务器.地图类;

namespace 游戏服务器.副本类;

// 借鉴移植(参考引擎): 魔主之地/妖僧分级Boss房。按随机数出弘化/清远/弘深, 击杀即关。
// 运行时需: 魔主之地地图/怪物模板(妖僧 弘化·清远·弘深)/Lua scene_instance_run 指向本类.执行。
public static class 魔主之地
{
	private static readonly Dictionary<int, string[]> 魔主之地怪物刷新 = new Dictionary<int, string[]> { [1] = new string[3] { "妖僧 弘化", "妖僧 清远", "妖僧 弘深" } };

	public static int 次数;

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
		if (地图.副本节点 == 255)
		{
			地图.关闭副本();
		}
		else if (地图.副本节点 == 0)
		{
			地图.副本节点 = 1;
			地图.节点计时 = 主程.当前时间.AddSeconds(2.0);
		}
		else if (地图.副本节点 == 1)
		{
			地图.地图公告("领主即将出现");
			地图.副本节点 = 2;
			地图.节点计时 = 主程.当前时间.AddSeconds(3.0);
		}
		else
		{
			if (地图.副本节点 <= 1)
			{
				return;
			}
			switch (地图.副本节点 % 2)
			{
			case 0:
			{
				int num = 主程.随机数.Next(100);
				if (num >= 98)
				{
					副本.范围刷新怪物从地图实例("妖僧 弘化", 地图, 0, new Point(906, 230), 禁止复活: true, 立即刷新: true);
					地图.副本节点++;
				}
				else if (num >= 88)
				{
					副本.范围刷新怪物从地图实例("妖僧 清远", 地图, 0, new Point(906, 230), 禁止复活: true, 立即刷新: true);
					地图.副本节点++;
				}
				else if (num >= 1)
				{
					副本.范围刷新怪物从地图实例("妖僧 弘深", 地图, 0, new Point(906, 230), 禁止复活: true, 立即刷新: true);
					地图.副本节点++;
				}
				break;
			}
			case 1:
				if (地图.存活怪物总数 == 0)
				{
					地图.地图公告("恭喜击杀领主，副本即将关闭");
					地图.节点计时 = 主程.当前时间.AddSeconds(10.0);
					地图.副本节点 = 255;
				}
				break;
			}
		}
	}
}
