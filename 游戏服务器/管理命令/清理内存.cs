using System;
using System.Collections.Generic;
using 游戏服务器.地图类;
using 游戏服务器.工具类;
using 游戏服务器.网络类;

namespace 游戏服务器.管理命令;

public class 清理内存 : GM命令
{
	[字段描述(0, 可选 = true)]
	public string 参数;

	public override 执行方式 执行方式 => 执行方式.只能后台执行;

	public override void 执行命令()
	{
		try
		{
			switch (参数?.ToLower())
			{
			case "后台":
			case "bg":
				执行后台清理();
				break;
			case "诊断":
			case "info":
				显示诊断报告();
				break;
			case "强制":
			case "force":
				执行强制清理();
				break;
			default:
				执行智能清理();
				break;
			}
		}
		catch (Exception ex)
		{
			主程.添加命令日志("<= @清理内存 命令执行失败: " + ex.Message);
		}
	}

	private void 执行智能清理()
	{
		主程.添加命令日志("<= @清理内存 [智能模式] 开始执行");
		int value = 网络服务网关.门票数据表?.Count ?? 0;
		int num = 0;
		int num2 = 0;
		long num3 = GC.GetTotalMemory(forceFullCollection: false) / 1024 / 1024;
		统计地图物品与怪物(out num, out num2);
		// 门票/地图物品的清理统一走 内存监控器 的实现(单趟+封顶+物品消失处理), 不再在本命令里另写一份
		int value2 = 内存监控器.清理过期门票数据();
		int value3 = 内存监控器.清理地图过期物品();
		int value4 = 内存监控器.智能清理异常怪物(100);
		int value5 = 内存监控器.优化角色字典内存();
		内存监控器.强制垃圾回收();
		int value6 = 网络服务网关.门票数据表?.Count ?? 0;
		int num4 = 0;
		int num5 = 0;
		long num6 = GC.GetTotalMemory(forceFullCollection: true) / 1024 / 1024;
		统计地图物品与怪物(out num4, out num5);
		主程.添加命令日志("<= @清理内存 命令执行完成");
		主程.添加命令日志($"  门票数据: {value} -> {value6} (清理 {value2} 个过期)");
		主程.添加命令日志($"  地图物品: {num} -> {num4} (清理 {value3} 个过期)");
		主程.添加命令日志($"  异常怪物: {num2} -> {num5} (清理 {value4} 个死亡未复活)");
		主程.添加命令日志($"  角色字典: 清理 {value5} 个临时变量");
		主程.添加命令日志($"  内存使用: {num3}MB -> {num6}MB (变化 {num6 - num3:+#;-#;0}MB)");
	}

	// 一次遍历同时统计物品与存活怪物: 获取怪物列表() 每次调用都要新建两个 List, 原来一张图要调两次
	private static void 统计地图物品与怪物(out int 物品数, out int 怪物数)
	{
		物品数 = 0;
		怪物数 = 0;
		if (地图处理网关.地图实例表 == null)
		{
			return;
		}
		foreach (地图实例 地图 in 地图处理网关.地图实例表.Values)
		{
			if (地图 == null)
			{
				continue;
			}
			if (地图.物品列表 != null)
			{
				物品数 += 地图.物品列表.Count;
			}
			if (地图.对象列表 != null)
			{
				foreach (地图对象 对象项 in 地图.对象列表)
				{
					if (对象项 is 怪物实例 { 对象死亡: false })
					{
						怪物数++;
					}
				}
			}
		}
	}

	private void 执行后台清理()
	{
		主程.添加命令日志("<= @清理内存 [后台模式] 启动后台批处理清理...");
		执行智能清理(); // 借鉴适配: 本引擎单逻辑线程, 后台Task.Run清理不安全, 改为同步智能清理
		主程.添加命令日志("  后台清理任务已启动，请稍后查看日志了解进度");
	}

	private void 显示诊断报告()
	{
		主程.添加命令日志("<= @清理内存 [诊断模式] 内存诊断报告:");
		string[] array = 内存监控器.获取内存诊断报告().Split('\n');
		foreach (string text in array)
		{
			主程.添加命令日志("  " + text);
		}
	}

	private void 执行强制清理()
	{
		主程.添加命令日志("<= @清理内存 [强制模式] 执行强制完整垃圾回收...");
		double num = (double)GC.GetTotalMemory(forceFullCollection: false) / 1024.0 / 1024.0;
		GC.Collect(2, GCCollectionMode.Aggressive, blocking: true, compacting: true);
		GC.WaitForPendingFinalizers();
		GC.Collect();
		double num2 = (double)GC.GetTotalMemory(forceFullCollection: true) / 1024.0 / 1024.0;
		主程.添加命令日志($"  强制GC完成: {num:F2}MB -> {num2:F2}MB (释放 {num - num2:F2}MB)");
		主程.添加命令日志("  警告: 频繁使用强制GC会影响性能！");
	}
}
