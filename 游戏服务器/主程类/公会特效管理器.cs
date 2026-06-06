using System.Collections.Generic;
using System;
using 游戏服务器.地图类;
using 游戏服务器.数据类;

namespace 游戏服务器
{
	// 公会等级/职位特效(从零原创, 照配置截图规格复刻):
	// 在线行会成员按"行会等级"获得等级特效 BUFF, 按"本人职位"获得职位特效 BUFF.
	// 由主循环每秒调用, 内部节流每 30 秒对账一次; 无状态对账(移除不该有的、补齐该有的), 自动处理等级提升/职位变动. 默认全关.
	public static class 公会特效管理器
	{
		private static DateTime 下次刷新 = DateTime.MinValue;

		public static void 刷新()
		{
			if ((!Settings.开启公会等级特效 && !Settings.开启公会职位特效) || 主程.当前时间 < 公会特效管理器.下次刷新)
			{
				return;
			}
			公会特效管理器.下次刷新 = 主程.当前时间.AddSeconds(30.0);
			if (游戏数据网关.行会数据表 == null)
			{
				return;
			}
			foreach (KeyValuePair<int, 游戏数据> 项 in 游戏数据网关.行会数据表.数据表)
			{
				if (!(项.Value is 行会数据 行会))
				{
					continue;
				}
				int 等级 = 行会.行会等级.V;
				foreach (角色数据 成员 in new List<角色数据>(行会.行会成员.Keys))
				{
					if (成员 == null || !成员.角色在线(out var 网络) || 网络.绑定角色 == null)
					{
						continue;
					}
					玩家实例 玩家 = 网络.绑定角色;
					if (Settings.开启公会等级特效)
					{
						int 档 = (等级 < 1) ? 1 : ((等级 > 6) ? 6 : 等级);
						公会特效管理器.对账特效(玩家, Settings.行会等级特效编号, Settings.行会等级特效编号[档]);
					}
					if (Settings.开启公会职位特效)
					{
						int 职位 = (int)行会.行会成员[成员];
						ushort 应得 = (职位 >= 1 && 职位 <= 6) ? Settings.行会职位特效编号[职位] : (ushort)0;
						公会特效管理器.对账特效(玩家, Settings.行会职位特效编号, 应得);
					}
				}
			}
		}

		// 无状态对账: 移除该组里玩家不该持有的特效, 补齐应得的(防叠层).
		private static void 对账特效(玩家实例 玩家, ushort[] 特效组, ushort 应得编号)
		{
			for (int i = 1; i < 特效组.Length; i++)
			{
				ushort id = 特效组[i];
				if (id != 0 && id != 应得编号 && 玩家.Buff列表.ContainsKey(id))
				{
					玩家.移除Buff时处理(id);
				}
			}
			if (应得编号 != 0 && !玩家.Buff列表.ContainsKey(应得编号))
			{
				玩家.添加Buff时处理(应得编号, 玩家);
			}
		}
	}
}
