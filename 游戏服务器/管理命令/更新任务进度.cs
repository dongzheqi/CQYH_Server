using System.Collections.Generic;
using 游戏服务器.地图类;
using 游戏服务器.网络类;

namespace 游戏服务器.管理命令;

// 借鉴移植(参考引擎): 强制重算某在线玩家指定任务的进度并回推客户端(排查卡任务)。依赖 UpdateQuestProgress(int) 重载(已补)。
public sealed class 更新任务进度 : GM命令
{
	[字段描述(0, 排序 = 0)]
	public string 角色名字;

	[字段描述(0, 排序 = 1)]
	public int 任务编号;

	public override 执行方式 执行方式 => 执行方式.优先后台执行;

	public override void 执行命令()
	{
		foreach (KeyValuePair<int, 玩家实例> item in 地图处理网关.玩家对象表)
		{
			if (item.Value.角色数据.角色名字.V == 角色名字)
			{
				item.Value.UpdateQuestProgress(任务编号);
				item.Value.网络连接?.发送封包(new 同步任务列表
				{
					任务描述 = item.Value.GetQuestProgressData()
				});
				主程.添加命令日志($"<= @{GetType().Name} 已执行, 角色: {角色名字}, 任务编号: {任务编号}");
				return;
			}
		}
		主程.添加命令日志("<= @" + GetType().Name + " 执行失败, 玩家不在线: " + 角色名字);
	}
}
