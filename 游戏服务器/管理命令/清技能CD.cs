using System;
using System.Collections.Generic;
using System.Linq;
using 游戏服务器.数据类;
using 游戏服务器.网络类;

namespace 游戏服务器.管理命令;

// 借鉴移植(参考引擎): 清空指定玩家(或 * 全服)的技能冷却并下发。适配: 参照对象池 ServerPackPool.Take().Reset() 改为 new 添加技能冷却。
public sealed class 清技能CD : GM命令
{
	[字段描述(0, 排序 = 0)]
	public string 角色名字;

	public override 执行方式 执行方式 => 执行方式.优先后台执行;

	public override void 执行命令()
	{
		if (string.IsNullOrWhiteSpace(角色名字))
		{
			主程.添加命令日志("<= @" + GetType().Name + " 命令执行失败, 角色名不能为空");
			return;
		}
		if (!游戏数据网关.角色数据表.检索表.TryGetValue(角色名字, out var value) && 角色名字 != "*")
		{
			主程.添加命令日志("<= @" + GetType().Name + " 命令执行失败, 角色不存在");
			return;
		}
		List<角色数据> list = ((角色名字 == "*") ? 游戏数据网关.角色数据表.检索表.Values.Select((游戏数据 v) => v as 角色数据).ToList() : new List<角色数据> { value as 角色数据 });
		foreach (角色数据 item in list)
		{
			if (item?.网络连接?.绑定角色 != null)
			{
				foreach (KeyValuePair<int, DateTime> v2 in item.冷却数据)
				{
					item.网络连接?.发送封包(new 添加技能冷却
					{
						冷却编号 = v2.Key,
						冷却时间 = 0
					});
				}
			}
			item?.冷却数据.Clear();
		}
		主程.添加命令日志("<= @" + GetType().Name + $" 命令执行成功, 共清空 {list.Count} 个玩家的技能冷却");
	}
}
