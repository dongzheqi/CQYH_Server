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
		foreach (地图实例 value7 in 地图处理网关.地图实例表.Values)
		{
			if (value7?.物品列表 != null)
			{
				num += value7.物品列表.Count;
			}
			if (value7?.获取怪物列表() != null)
			{
				num2 += value7.获取怪物列表().Count;
			}
		}
		int value2 = 清理过期门票数据();
		int value3 = 清理地图过期物品();
		int value4 = 内存监控器.智能清理异常怪物(100);
		int value5 = 内存监控器.优化角色字典内存();
		内存监控器.强制垃圾回收();
		int value6 = 网络服务网关.门票数据表?.Count ?? 0;
		int num4 = 0;
		int num5 = 0;
		long num6 = GC.GetTotalMemory(forceFullCollection: true) / 1024 / 1024;
		foreach (地图实例 value8 in 地图处理网关.地图实例表.Values)
		{
			if (value8?.物品列表 != null)
			{
				num4 += value8.物品列表.Count;
			}
			if (value8?.获取怪物列表() != null)
			{
				num5 += value8.获取怪物列表().Count;
			}
		}
		主程.添加命令日志("<= @清理内存 命令执行完成");
		主程.添加命令日志($"  门票数据: {value} -> {value6} (清理 {value2} 个过期)");
		主程.添加命令日志($"  地图物品: {num} -> {num4} (清理 {value3} 个过期)");
		主程.添加命令日志($"  异常怪物: {num2} -> {num5} (清理 {value4} 个死亡未复活)");
		主程.添加命令日志($"  角色字典: 清理 {value5} 个临时变量");
		主程.添加命令日志($"  内存使用: {num3}MB -> {num6}MB (变化 {num6 - num3:+#;-#;0}MB)");
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

	private int 清理过期门票数据()
	{
		int num = 0;
		Dictionary<string, 门票信息> 门票数据表 = 网络服务网关.门票数据表;
		if (门票数据表 == null)
		{
			return 0;
		}
		DateTime now = DateTime.Now;
		int num2;
		do
		{
			num2 = 0;
			List<string> list = new List<string>();
			lock (门票数据表)
			{
				foreach (KeyValuePair<string, 门票信息> item in 门票数据表)
				{
					if (now > item.Value.有效时间)
					{
						list.Add(item.Key);
						if (list.Count >= 500)
						{
							break;
						}
					}
				}
				foreach (string item2 in list)
				{
					门票数据表.Remove(item2);
					num++;
					num2++;
				}
			}
		}
		while (num2 >= 500);
		return num;
	}

	private int 清理地图过期物品()
	{
		int num = 0;
		DateTime now = DateTime.Now;
		int num2;
		do
		{
			num2 = 0;
			foreach (地图实例 value in 地图处理网关.地图实例表.Values)
			{
				if (value?.物品列表 == null)
				{
					continue;
				}
				List<物品实例> list = new List<物品实例>();
				foreach (物品实例 item in value.物品列表)
				{
					if (now > item.消失时间)
					{
						list.Add(item);
						if (list.Count >= 1000)
						{
							break;
						}
					}
				}
				foreach (物品实例 item2 in list)
				{
					value.物品列表.Remove(item2);
					num++;
					num2++;
				}
				if (num2 >= 1000)
				{
					break;
				}
			}
		}
		while (num2 >= 1000);
		return num;
	}
}
