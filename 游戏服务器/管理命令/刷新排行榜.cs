using System.Collections.Generic;
using System.Diagnostics;
using 游戏服务器.数据类;

namespace 游戏服务器.管理命令;

// 借鉴移植(参考引擎): 遍历全部角色/行会数据重算全部排行榜。零缺依赖(系统数据.数据 五个更新方法我方已具备)。
// 用途: 批量改档/数据导入/加机器人后一键重建全部榜单, 无此命令只能等在线增量刷新。
public sealed class 刷新排行榜 : GM命令
{
	public override 执行方式 执行方式 => 执行方式.优先后台执行;

	public override void 执行命令()
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		主程.添加命令日志("开始刷新排行榜");
		foreach (KeyValuePair<int, 游戏数据> item in 游戏数据网关.角色数据表.数据表)
		{
			角色数据 角色 = item.Value as 角色数据;
			系统数据.数据.更新战力(角色);
			系统数据.数据.更新等级(角色);
			系统数据.数据.更新声望(角色);
			系统数据.数据.更新PK值(角色);
		}
		foreach (KeyValuePair<int, 游戏数据> item2 in 游戏数据网关.行会数据表.数据表)
		{
			系统数据.数据.更新行会(item2.Value as 行会数据);
		}
		主程.添加命令日志("<= @" + GetType().Name + $" 刷新排行榜成功,耗时:{stopwatch.ElapsedMilliseconds}");
	}
}
