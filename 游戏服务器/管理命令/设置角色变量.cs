using 游戏服务器.地图类;
using 游戏服务器.数据类;
using 游戏服务器.网络类;

namespace 游戏服务器.管理命令;

// 借鉴移植(参考引擎): 设置角色 Y/F/U/T/J 变量, 修复卡任务/手动发放阵营权限。在线改 Y 变量会推送 同步角色变量 刷新客户端。
// 适配: 原文件反编译 goto 重写为 switch; 参照对象池 ServerPackPool.Take().Reset() 我方无, 改为 new 同步角色变量 直接发送。
public sealed class 设置角色变量 : GM命令
{
	[字段描述(0, 排序 = 0)]
	public string 角色名字;

	[字段描述(0, 排序 = 1)]
	public string 变量类型;

	[字段描述(0, 排序 = 2)]
	public int 变量索引;

	[字段描述(0, 排序 = 3)]
	public string 变量数值;

	public override 执行方式 执行方式 => 执行方式.优先后台执行;

	public override void 执行命令()
	{
		if (!游戏数据网关.角色数据表.检索表.TryGetValue(角色名字, out var value) || !(value is 角色数据 角色数据))
		{
			主程.添加命令日志("<= @" + GetType().Name + " 命令执行失败, 角色不存在: " + 角色名字);
			return;
		}
		if (变量类型 == "脚本字符" || 变量类型 == "T变量")
		{
			string 原值 = (角色数据.脚本字符.ContainsKey(变量索引) ? 角色数据.脚本字符[变量索引] : "(null)");
			角色数据.脚本字符[变量索引] = 变量数值;
			主程.添加命令日志($"<= @{GetType().Name} 已执行, 角色: {角色名字}, T[{变量索引}]: {原值} → {变量数值}");
			return;
		}
		if (!int.TryParse(变量数值, out var 数值))
		{
			主程.添加命令日志("<= @" + GetType().Name + " 命令执行失败, 非数字无法转换: " + 变量数值);
			return;
		}
		字典监视器<int, int> 目标字典 = 获取字典(角色数据, 变量类型);
		if (目标字典 == null)
		{
			主程.添加命令日志("<= @" + GetType().Name + " 命令执行失败, 不支持的变量类型: " + 变量类型);
			主程.添加命令日志("   支持: J变量(零点数字) / T变量(脚本字符) / U变量(脚本数字) / F变量(脚本变量) / Y变量(角色变量)");
			return;
		}
		int 原数值 = (目标字典.ContainsKey(变量索引) ? 目标字典[变量索引] : 0);
		目标字典[变量索引] = 数值;
		主程.添加命令日志($"<= @{GetType().Name} 已执行, 角色: {角色名字}, 类型: {变量类型}, 变量[{变量索引}]: {原数值} → {数值}");
		if (变量类型 != "角色变量" && 变量类型 != "Y变量")
		{
			return;
		}
		if (地图处理网关.玩家对象表.TryGetValue(角色数据.数据索引.V, out var 玩家) && 玩家.网络连接 != null)
		{
			玩家.网络连接.发送封包(new 同步角色变量
			{
				字节描述 = 玩家.获取角色变量()
			});
		}
	}

	private static 字典监视器<int, int> 获取字典(角色数据 角色, string 类型)
	{
		switch (类型)
		{
		case "Y变量":
		case "角色变量":
			return 角色.角色变量;
		case "F变量":
		case "脚本变量":
			return 角色.脚本变量;
		case "U变量":
		case "脚本数字":
			return 角色.脚本数字;
		case "J变量":
		case "零点数字":
			return 角色.零点数字;
		case "任务标识":
			主程.添加命令日志("   提示: 任务标识是 bool 类型，0=未完成 1=已完成");
			return null;
		default:
			return null;
		}
	}
}
