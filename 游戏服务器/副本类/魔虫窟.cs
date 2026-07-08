using System.Collections.Generic;
using System.Drawing;
using 游戏服务器.地图类;

namespace 游戏服务器.副本类;

// 借鉴移植(参考引擎): 魔虫窟 独立执行体(净升级原地图实例内的 private 魔虫窟执行 雏形)。
// 进场给 Buff 2554, 清怪 40 后刷 3 只蛇蝎 BOSS。运行时需: 魔虫窟地图/怪物模板(蛇蝎 魔闪/毒钩/赤牙)/Buff2554/Lua scene_instance_run 指向本类.执行。
public static class 魔虫窟
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
			List<玩家实例>.Enumerator enumerator = 地图.获取玩家列表().GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.添加Buff时处理(2554, null);
			}
			地图.副本节点 = 1;
			地图.节点计时 = 主程.当前时间.AddSeconds(3.0);
		}
		else if (地图.副本节点 == 1)
		{
			if (地图.固定怪物总数 - 地图.存活怪物总数 >= 40)
			{
				地图.副本节点 = 2;
			}
			地图.节点计时 = 主程.当前时间.AddSeconds(3.0);
		}
		else if (地图.副本节点 == 255)
		{
			地图.关闭副本();
		}
		else if (地图.副本节点 == 2)
		{
			地图.地图公告("<#T:50607>");
			Point[] 刷新范围 = 地图.获取怪物区域("魔虫窟-BOSS怪物区域").获取刷新范围();
			副本.范围刷新怪物从地图实例("蛇蝎 魔闪", 地图, 0, 刷新范围, 禁止复活: true, 立即刷新: true);
			副本.范围刷新怪物从地图实例("蛇蝎 毒钩", 地图, 0, 刷新范围, 禁止复活: true, 立即刷新: true);
			副本.范围刷新怪物从地图实例("蛇蝎 赤牙", 地图, 0, 刷新范围, 禁止复活: true, 立即刷新: true);
			地图.副本节点 = 3;
		}
	}
}
