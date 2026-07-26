using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using 游戏服务器.地图类;
using 游戏服务器.性能优化;
using 游戏服务器.数据类;
using 游戏服务器.模板类;
using 游戏服务器.网络类;

// 借鉴移植(参考引擎): 进程内存看门狗 + 泄漏清扫器。适配要点:
//  - Logger.SystemLog(带caller/line) -> 主程.添加系统日志(msg,hardLog,showDiag)
//  - 我方 怪物对象表/地图实例表/物品对象表 是普通 Dictionary(单逻辑线程), 参照 ConcurrentDictionary.TryRemove -> Dictionary.Remove
//  - 删除 Task.Run 后台清理路径(会与逻辑线程并发改 Dictionary), 仅保留逻辑线程同步清理(尝试自动清理/执行自动清理)
namespace 游戏服务器.工具类;

public static class 内存监控器
{
	private static DateTime _上次检查时间 = DateTime.Now;

	private static long _上次内存使用量 = 0L;

	private static bool _已记录启动内存 = false;

	private static DateTime _上次自动清理时间 = DateTime.Now;

	private static bool _自动清理进行中 = false;

	private static readonly object _自动清理锁 = new object();

	// ===== 以下为"内存看门狗卡服"修复引入的限频/上限参数 =====

	// 单轮清理上限: 清理全程跑在逻辑线程上, 必须封顶, 否则一次清理就是几十万次迭代的停服级卡顿
	private const int 单轮清理上限_门票 = 5000;

	private const int 单轮清理上限_物品 = 2000;

	private const int 单轮清理上限_副本 = 50;

	// 进程内存采样缓存: Process.PrivateMemorySize64 在 Windows 上会做一次全系统进程/线程快照
	// (NtQuerySystemInformation), 单次可达数十毫秒。原实现每秒在逻辑线程取一次 => 周期性卡顿。
	private const int 内存采样间隔秒 = 10;

	private static DateTime _下次内存采样 = DateTime.MinValue;

	private static long _内存采样缓存 = 0L;

	// 超阈值强制清理的最小间隔与退避: 私有字节清理后一般不会立刻回落到阈值以下,
	// 原实现 强制清理=true 直接跳过间隔判断 => 每秒一轮全表扫描 + 两次阻塞 Full GC, 服务端卡死。
	private const int 强制清理最小间隔秒 = 60;

	private const double 有效清理阈值MB = 32.0;

	private const int 最大退避档位 = 3;

	private static int _无效清理次数 = 0;

	private static long _上轮清理前内存 = 0L;

	private static DateTime _下次超阈值诊断 = DateTime.MinValue;

	// 角色字典优化是全表扫描, 最重的一项, 单独限频
	private const int 角色字典优化间隔分钟 = 30;

	private static DateTime _上次角色字典优化 = DateTime.MinValue;

	// 状态型日志去重(原实现在这两种状态下每秒刷一行, 把日志队列冲满)
	private static bool _已提示高性能模式 = false;

	private static bool _已提示清理禁用 = false;

	// 代码里直接用索引器读取(缺键即抛 KeyNotFoundException)的角色变量, 绝不能被"优化"删掉:
	//   50            玩家实例.五零变量 取值器
	//   218/629/416/220  游戏数据网关.每日处理 的找回奖励结算
	// 另加 日程变量/角色变量 两个枚举的全部索引(登录同步会整表回填, 删了纯属白删还要重建)
	private static readonly HashSet<int> 保护角色变量 = 构建保护角色变量();

	private static HashSet<int> 构建保护角色变量()
	{
		HashSet<int> hashSet = new HashSet<int> { 50, 218, 220, 416, 629 };
		try
		{
			foreach (int item in Enum.GetValues(typeof(日程变量)))
			{
				hashSet.Add(item);
			}
			foreach (int item2 in Enum.GetValues(typeof(角色变量)))
			{
				hashSet.Add(item2);
			}
		}
		catch
		{
		}
		return hashSet;
	}

	// 带缓存的进程私有内存读取, 见 内存采样间隔秒 注释
	private static long 取进程私有内存(DateTime 当前时间, bool 强制刷新 = false)
	{
		if (!强制刷新 && _内存采样缓存 > 0 && 当前时间 < _下次内存采样)
		{
			return _内存采样缓存;
		}
		using (Process process = Process.GetCurrentProcess())
		{
			_内存采样缓存 = process.PrivateMemorySize64;
		}
		_下次内存采样 = 当前时间.AddSeconds(内存采样间隔秒);
		return _内存采样缓存;
	}

	public static void 记录启动内存()
	{
		if (_已记录启动内存)
		{
			return;
		}
		_已记录启动内存 = true;
		try
		{
			long privateMemorySize = 取进程私有内存(DateTime.Now, 强制刷新: true);
			double num = (double)privateMemorySize / 1024.0 / 1024.0;
			主程.添加系统日志($"[内存基线] 服务器启动内存: {num:F2}MB", true, true);
			if (num > 1024.0)
			{
				主程.添加系统日志("[内存基线警告] 启动内存过高，开始记录各集合状态...", true, true);
				记录详细集合状态();
			}
			_上次内存使用量 = privateMemorySize;
		}
		catch (Exception ex)
		{
			主程.添加系统日志("[内存基线] 记录时发生错误: " + ex.Message, true, true);
		}
	}

	public static void 检查内存使用()
	{
		if (性能优化配置.高性能模式)
		{
			// 状态不变时不重复刷日志(本方法每秒被主循环调一次)
			if (!_已提示高性能模式)
			{
				_已提示高性能模式 = true;
				主程.添加系统日志("[内存监控] 高性能模式已启用，跳过内存检查", true, true);
			}
			return;
		}
		_已提示高性能模式 = false;
		DateTime now = DateTime.Now;
		try
		{
			long privateMemorySize = 取进程私有内存(now);
			double num = (double)privateMemorySize / 1024.0 / 1024.0;
			int 内存阈值MB = 性能优化配置.内存阈值MB;
			if (num > (double)内存阈值MB)
			{
				// 超阈值只提高清理频率, 不再每秒"打日志 + 全表统计 + 触发清理"。
				// 原实现在这里每秒跑 记录集合状态()+建议()+强制清理(绕过间隔), 一旦内存过线就再也回不来 => 卡服。
				if (now >= _下次超阈值诊断)
				{
					_下次超阈值诊断 = now.AddMinutes(Math.Max(1, 性能优化配置.内存监控间隔));
					主程.添加系统日志($"[内存警告] 内存使用过高: {num:F2}MB (阈值: {内存阈值MB}MB)", true, true);
					记录集合状态();
					建议();
				}
				尝试自动清理(强制清理: true);
			}
			else
			{
				_无效清理次数 = 0;
				_下次超阈值诊断 = DateTime.MinValue;
				尝试自动清理(强制清理: false);
			}
			TimeSpan timeSpan = TimeSpan.FromMinutes(性能优化配置.内存监控间隔);
			if (now - _上次检查时间 >= timeSpan)
			{
				double num2 = (double)(privateMemorySize - _上次内存使用量) / 1024.0 / 1024.0;
				if (num2 > 100.0)
				{
					主程.添加系统日志($"[内存监控] 内存增长异常: 当前使用 {num:F2}MB, 增长了 {num2:F2}MB", true, true);
					记录集合状态();
				}
				if (性能优化配置.启用详细内存日志)
				{
					主程.添加系统日志($"[内存监控] 当前内存: {num:F2}MB, 阈值: {内存阈值MB}MB, 自动清理: {(性能优化配置.启用自动清理内存 ? "启用" : "禁用")}", true, true);
				}
				_上次内存使用量 = privateMemorySize;
				_上次检查时间 = now;
			}
		}
		catch (Exception ex)
		{
			主程.添加系统日志("[内存监控] 检查时发生错误: " + ex.Message, true, true);
		}
	}

	private static void 尝试自动清理(bool 强制清理)
	{
		if (!性能优化配置.启用自动清理内存)
		{
			if (!_已提示清理禁用)
			{
				_已提示清理禁用 = true;
				主程.添加系统日志("[自动清理] 自动清理已禁用，跳过执行", true, true);
			}
			return;
		}
		_已提示清理禁用 = false;
		DateTime now = DateTime.Now;
		lock (_自动清理锁)
		{
			if (_自动清理进行中)
			{
				if (性能优化配置.启用详细内存日志)
				{
					主程.添加系统日志("[自动清理] 清理正在进行中，跳过本次", true, true);
				}
				return;
			}
			TimeSpan timeSpan;
			if (强制清理)
			{
				// 超阈值也必须限频。清理无效(基本没降内存)时逐级退避 1→2→4→8 分钟,
				// 避免"内存本来就该这么大"的服务器被清理循环永久占住逻辑线程。
				timeSpan = TimeSpan.FromSeconds((double)(强制清理最小间隔秒 * (1 << Math.Min(_无效清理次数, 最大退避档位))));
			}
			else
			{
				timeSpan = TimeSpan.FromMinutes(性能优化配置.自动清理内存间隔);
			}
			TimeSpan timeSpan2 = now - _上次自动清理时间;
			if (timeSpan2 < timeSpan)
			{
				if (性能优化配置.启用详细内存日志)
				{
					主程.添加系统日志($"[自动清理] 跳过清理，距离下次清理还有 {(timeSpan - timeSpan2).TotalSeconds:F0} 秒(强制={强制清理})", true, true);
				}
				return;
			}
			主程.添加系统日志($"[自动清理] 触发清理，强制={强制清理}，距离上次清理={timeSpan2.TotalMinutes:F1}分钟", true, true);
			_自动清理进行中 = true;
			// 先占位: 清理本身耗时较长时, 防止下一秒的调用又排进来
			_上次自动清理时间 = now;
		}
		try
		{
			long 清理前内存 = 取进程私有内存(now, 强制刷新: true);
			double num = (double)清理前内存 / 1024.0 / 1024.0;
			// 用"两轮清理之间的内存变化"判定上一轮是否有效(非阻塞GC回收在本轮之后才体现, 当场量是量不出来的)
			if (强制清理 && _上轮清理前内存 > 0)
			{
				double num3 = (double)(_上轮清理前内存 - 清理前内存) / 1024.0 / 1024.0;
				_无效清理次数 = ((num3 < 有效清理阈值MB) ? Math.Min(_无效清理次数 + 1, 最大退避档位) : 0);
			}
			_上轮清理前内存 = 清理前内存;
			主程.添加系统日志("[自动清理] ====== 开始执行自动内存清理 ======", true, true);
			主程.添加系统日志($"[自动清理] 当前内存: {num:F2}MB", true, true);
			主程.添加系统日志($"[自动清理] 配置: 间隔={性能优化配置.自动清理内存间隔}分钟, 阈值={性能优化配置.内存阈值MB}MB", true, true);
			执行自动清理();
			// 只发一次非阻塞后台 Gen2 回收。原实现连做两次 blocking Full GC + WaitForPendingFinalizers,
			// 1.5G+ 堆上单次停顿就有数百毫秒~数秒, 逻辑线程全程冻结(玩家表现就是"服务端卡住")。
			主程.添加系统日志("[自动清理] 已请求后台GC(非阻塞)...", true, true);
			GC.Collect(2, GCCollectionMode.Optimized, blocking: false);
			主程.添加系统日志("[自动清理] ====== 清理完成 ======", true, true);
			_上次自动清理时间 = DateTime.Now;
		}
		catch (Exception ex)
		{
			主程.添加系统日志("[自动清理] 执行时发生错误: " + ex.Message, true, true);
		}
		finally
		{
			lock (_自动清理锁)
			{
				_自动清理进行中 = false;
			}
		}
	}

	public static int 智能清理异常怪物(int 最大清理数量 = 200)
	{
		int num = 0;
		if (地图处理网关.地图实例表 == null || 最大清理数量 <= 0)
		{
			return 0;
		}
		DateTime now = DateTime.Now;
		try
		{
			// 单趟扫描, 清满上限就收工, 下一轮清理接着来。
			// 原实现套了一层 do/while: 每清满一批就从第一张地图重头再扫一遍全服(且每张图的对象列表还要 ToList 拷一份),
			// 死怪一多就是 O(n²) 级的逻辑线程阻塞 + 海量临时分配 —— 号称清内存, 实际既卡服又制造垃圾。
			List<怪物实例> list = new List<怪物实例>();
			foreach (地图实例 item in 地图处理网关.地图实例表.Values.ToList())
			{
				if (item?.对象列表 == null)
				{
					continue;
				}
				list.Clear();
				// 我方怪物存于 地图实例.对象列表(HashSet<地图对象>), 无独立怪物列表, 故遍历对象列表按 is 怪物实例 过滤(含已死亡怪物)
				// 待删项先收集、循环结束后再删, 故此处无需对 对象列表 做 ToList 快照
				foreach (地图对象 对象项 in item.对象列表)
				{
					if (!(对象项 is 怪物实例 item2))
					{
						continue;
					}
					if (!item2.对象死亡 || item2.怪物级别 >= 怪物级别分类.精英干将)
					{
						continue;
					}
					游戏怪物 对象模板 = item2.对象模板;
					if (对象模板 == null || !对象模板.刷新通知)
					{
						if (item2.复活时间 > DateTime.MinValue && now > item2.复活时间.AddMinutes(30.0))
						{
							list.Add(item2);
						}
						else if (item2.消失时间 > DateTime.MinValue && now > item2.消失时间.AddHours(1.0))
						{
							list.Add(item2);
						}
						if (num + list.Count >= 最大清理数量)
						{
							break;
						}
					}
				}
				foreach (怪物实例 item3 in list)
				{
					try
					{
						item.对象列表.Remove(item3);
						item3.删除对象();
						num++;
					}
					catch (Exception ex)
					{
						主程.添加系统日志($"[智能清理] 清理怪物 {item3.地图编号} 时发生错误: {ex.Message}", true, true);
					}
				}
				if (num >= 最大清理数量)
				{
					break;
				}
			}
			if (num > 0)
			{
				主程.添加系统日志($"[智能清理] 成功清理 {num} 个异常怪物（死亡未复活超过30分钟）", true, true);
			}
		}
		catch (Exception ex2)
		{
			主程.添加系统日志("[智能清理] 执行异常怪物清理时发生错误: " + ex2.Message, true, true);
		}
		return num;
	}

	private static void 执行自动清理()
	{
		int value = 0;
		int value2 = 0;
		int value3 = 0;
		int value4 = 0;
		int value5 = 0;
		int value6 = 0;
		if (性能优化配置.清理过期门票)
		{
			try
			{
				value = 清理过期门票数据();
			}
			catch (Exception ex)
			{
				主程.添加系统日志("[自动清理] 清理门票数据时发生错误: " + ex.Message, true, true);
			}
		}
		if (性能优化配置.清理地图过期物品)
		{
			try
			{
				value2 = 清理地图过期物品();
			}
			catch (Exception ex2)
			{
				主程.添加系统日志("[自动清理] 清理地图物品时发生错误: " + ex2.Message, true, true);
			}
		}
		try
		{
			value3 = 智能清理异常怪物();
		}
		catch (Exception ex3)
		{
			主程.添加系统日志("[自动清理] 智能清理异常怪物时发生错误: " + ex3.Message, true, true);
		}
		try
		{
			value4 = 清理空地图实例();
		}
		catch (Exception ex4)
		{
			主程.添加系统日志("[自动清理] 清理空地图实例时发生错误: " + ex4.Message, true, true);
		}
		// 角色字典优化要扫全服角色表(含所有离线角色), 是这里最重的一项, 单独限频到 角色字典优化间隔分钟
		if (DateTime.Now - _上次角色字典优化 >= TimeSpan.FromMinutes(角色字典优化间隔分钟))
		{
			_上次角色字典优化 = DateTime.Now;
			try
			{
				value5 = 优化角色字典内存();
			}
			catch (Exception ex5)
			{
				主程.添加系统日志("[自动清理] 优化角色字典内存时发生错误: " + ex5.Message, true, true);
			}
		}
		try
		{
			value6 = 清理已关闭副本();
		}
		catch (Exception ex6)
		{
			主程.添加系统日志("[自动清理] 清理已关闭副本时发生错误: " + ex6.Message, true, true);
		}
		主程.添加系统日志($"[自动清理] 清理完成: 门票 {value} 个, 物品 {value2} 个, 异常怪物 {value3} 个, 空地图 {value4} 个, 字典优化 {value5} 个, 副本 {value6} 个", true, true);
	}

	private static void 记录集合状态()
	{
		try
		{
			主程.添加系统日志($"[内存监控] 当前怪物总数: {地图处理网关.怪物对象表?.Count ?? 0}", true, true);
			主程.添加系统日志($"[内存监控] 当前玩家数量: {地图处理网关.玩家对象表?.Count ?? 0}", true, true);
			主程.添加系统日志($"[内存监控] 当前地图实例: {地图处理网关.地图实例表?.Count ?? 0}", true, true);
			int num = 地图处理网关.副本实例表?.Count ?? 0;
			int num2 = 0;
			if (地图处理网关.副本实例表 != null)
			{
				foreach (地图实例 item in 地图处理网关.副本实例表)
				{
					if (item.副本关闭)
					{
						num2++;
					}
				}
			}
			主程.添加系统日志($"[内存监控] 当前副本实例: {num} (已关闭: {num2})", true, true);
			if (num > 50)
			{
				主程.添加系统日志("[内存监控] ⚠ 警告: 副本实例数量过多，建议检查副本清理机制", true, true);
			}
		}
		catch (Exception ex)
		{
			主程.添加系统日志("[内存监控] 记录集合状态时发生错误: " + ex.Message, true, true);
		}
	}

	private static int 清理已关闭副本()
	{
		int num = 0;
		if (地图处理网关.副本实例表 == null)
		{
			return 0;
		}
		try
		{
			List<地图实例> list = new List<地图实例>();
			foreach (地图实例 item in 地图处理网关.副本实例表)
			{
				if (item.副本关闭)
				{
					list.Add(item);
				}
			}
			int num2 = 0;
			foreach (地图实例 item2 in list)
			{
				if (num2 >= 单轮清理上限_副本)
				{
					break;
				}
				try
				{
					游戏脚本.地图销毁(item2);
					if (地图处理网关.副本实例表.Remove(item2))
					{
						num++;
						num2++;
					}
				}
				catch (Exception ex)
				{
					主程.添加系统日志("[自动清理] 清理副本 " + item2.地图模板?.地图名字 + " 时发生错误: " + ex.Message, true, true);
				}
			}
			if (num > 0)
			{
				主程.添加系统日志($"[自动清理] 成功清理 {num} 个已关闭副本", true, true);
			}
			if (list.Count > 100)
			{
				主程.添加系统日志($"[自动清理] ⚠ 警告: 有 {list.Count} 个已关闭副本等待清理，副本清理可能跟不上创建速度", true, true);
			}
		}
		catch (Exception ex2)
		{
			主程.添加系统日志("[自动清理] 清理已关闭副本时发生错误: " + ex2.Message, true, true);
		}
		return num;
	}

	// public: GM命令 @清理内存 复用同一份实现, 避免两处各写一遍再各自出错
	public static int 清理过期门票数据()
	{
		Dictionary<string, 门票信息> 门票数据表 = 网络服务网关.门票数据表;
		if (门票数据表 == null)
		{
			return 0;
		}
		DateTime now = DateTime.Now;
		// 单趟扫描 + 封顶。原实现每删满 500 条就把整张表从头再枚举一遍(还是在 lock 里), 过期票一多直接 O(n²) 锁死
		List<string> list = new List<string>();
		lock (门票数据表)
		{
			foreach (KeyValuePair<string, 门票信息> item in 门票数据表)
			{
				if (now > item.Value.有效时间)
				{
					list.Add(item.Key);
					if (list.Count >= 单轮清理上限_门票)
					{
						break;
					}
				}
			}
			foreach (string item2 in list)
			{
				门票数据表.Remove(item2);
			}
		}
		return list.Count;
	}

	// public: 同上, 供 GM命令 @清理内存 复用
	public static int 清理地图过期物品()
	{
		int num = 0;
		if (地图处理网关.地图实例表 == null)
		{
			return 0;
		}
		DateTime now = DateTime.Now;
		// 同样改为单趟 + 封顶(原实现 do/while 重扫全服地图)
		List<物品实例> list = new List<物品实例>();
		foreach (地图实例 value in 地图处理网关.地图实例表.Values)
		{
			if (value?.物品列表 == null)
			{
				continue;
			}
			list.Clear();
			foreach (物品实例 item in value.物品列表)
			{
				if (now > item.消失时间)
				{
					list.Add(item);
					if (num + list.Count >= 单轮清理上限_物品)
					{
						break;
					}
				}
			}
			foreach (物品实例 item2 in list)
			{
				try
				{
					// 走引擎自身的消失流程(删物品数据 + 解绑网格 + 移出各对象表 + 移出地图物品列表)。
					// 原实现只从 地图.物品列表 里 Remove: 物品仍留在 地图处理网关.物品对象表/地图对象表 和网格中,
					// 结果是谁都回收不掉的幽灵物品 —— 越"清"漏得越多。
					item2.物品消失处理();
					// 兜底: 物品消失处理 走的是 当前地图.移除对象, 若 当前地图 为空则列表里会残留, 下轮又被重复处理
					value.物品列表.Remove(item2);
					num++;
				}
				catch (Exception ex)
				{
					主程.添加系统日志("[自动清理] 清理地图物品时发生错误: " + ex.Message, true, true);
				}
			}
			if (num >= 单轮清理上限_物品)
			{
				break;
			}
		}
		return num;
	}

	// 【本引擎不适用, 整体停用, 恒返回 0】
	// 参照引擎的世界地图是「用到才建、空了可弃」的池化实例, 删掉能省内存、下次进图会重建。本引擎不是:
	// 地图处理网关.初始化(地图处理网关.cs:737-739) 在起服时就把 游戏地图.数据表 里每一张地图 new 成常驻
	// 地图实例 塞进 地图实例表, 随后 :741-758 给它挂 地形数据 与逐格 地图对象 网格、:759-781 挂
	// 复活区域/安全区域 等 地图区域。全仓再无第二处往 地图实例表 写入——删掉即永久丢失, 不会重建。
	// 后果: 已分配地图(编号) 从此返回 null, 在该图下线的角色登录、传送/换图、NPC 脚本查人数全部 NRE,
	// 怪物也不再在该图刷新。
	// 而原判定恰好专挑常驻图下手, 三道保险全都不成立:
	//   ① !value.副本地图 —— 候选集正好是常驻世界图(副本实例另有 清理已关闭副本() 负责);
	//   ② 地图实例表.Count <= 150 才跳过 —— 本引擎约 2695 张地图模板, 这道闸门恒开;
	//   ③ 30 分钟空闲保护形同虚设 —— 执行计时 只在 地图实例.处理数据(地图实例.cs:202-205) 的
	//      `if (this.副本地图 && ...)` 分支里推进, 普通地图的 执行计时 自初始化后再没更新过,
	//      故 now - 执行计时 恒远大于 30 分钟, 判定恒真。
	// 即: 任何一张暂时没人、没地面物品、没怪的普通图, 都会被永久删除。
	// 保留空实现而非删掉函数, 是为了让 执行智能清理() 的调用点与「空地图 N 个」日志格式不变, 并给
	// 后续再从参照引擎移植同名功能的人留下这段说明。随之失效的 主城地图编号 / 单轮清理上限_空地图
	// 两个私有成员已一并移除。
	private static int 清理空地图实例()
	{
		return 0;
	}

	public static int 优化角色字典内存()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		try
		{
			if (游戏数据网关.角色数据表?.数据表 == null)
			{
				return 0;
			}
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, 游戏数据> item in 游戏数据网关.角色数据表.数据表.ToList())
			{
				if (!(item.Value is 角色数据 角色数据))
				{
					continue;
				}
				if (角色数据.角色在线(out _))
				{
					num2++;
				}
				else
				{
					num3++;
				}
				if (角色数据.角色变量 == null || 角色数据.角色变量.Count == 0)
				{
					continue;
				}
				list.Clear();
				foreach (KeyValuePair<int, int> item2 in 角色数据.角色变量)
				{
					// 只回收"值为 0 且没人用索引器直读"的变量键 —— 值为 0 时 TryGetValue 取不到与取到 0 等价, 删了无副作用。
					// 原实现另有一条"离线满7天就把该角色所有非日程变量全删"的分支: 那会把任务进度/计数等
					// 非零业务数据一并抹掉(角色变量是任务约束 SelfVar 的载体), 属于纯数据损坏, 已删除。
					// 保护键判定也从 Enum.IsDefined(反射+装箱, 百万次量级极慢) 换成预建 HashSet。
					if (item2.Value == 0 && !保护角色变量.Contains(item2.Key))
					{
						list.Add(item2.Key);
					}
				}
				foreach (int item3 in list)
				{
					角色数据.角色变量.Remove(item3);
					num++;
				}
			}
			if (num > 0 || num2 > 0)
			{
				主程.添加系统日志($"[字典优化] 处理角色: 在线{num2}个, 离线{num3}个, 清理空变量: {num}个", true, true);
			}
		}
		catch (Exception ex)
		{
			主程.添加系统日志("[字典优化] 优化时发生错误: " + ex.Message, true, true);
		}
		return num;
	}

	private static void 记录详细集合状态()
	{
		try
		{
			主程.添加系统日志($"[详细状态] 玩家对象表: {地图处理网关.玩家对象表?.Count ?? 0}", true, true);
			主程.添加系统日志($"[详细状态] 怪物对象表: {地图处理网关.怪物对象表?.Count ?? 0}", true, true);
			主程.添加系统日志($"[详细状态] 宠物对象表: {地图处理网关.宠物对象表?.Count ?? 0}", true, true);
			主程.添加系统日志($"[详细状态] 物品对象表: {地图处理网关.物品对象表?.Count ?? 0}", true, true);
			主程.添加系统日志($"[详细状态] 守卫对象表: {地图处理网关.守卫对象表?.Count ?? 0}", true, true);
			主程.添加系统日志($"[详细状态] 地图实例表: {地图处理网关.地图实例表?.Count ?? 0}", true, true);
			主程.添加系统日志($"[详细状态] 网络连接表: {网络服务网关.网络连接表?.Count ?? 0}", true, true);
			主程.添加系统日志($"[详细状态] 门票数据表: {网络服务网关.门票数据表?.Count ?? 0}", true, true);
			记录配置表状态();
			long num = 0L;
			long num2 = 0L;
			if (地图处理网关.地图实例表 != null)
			{
				foreach (地图实例 value in 地图处理网关.地图实例表.Values)
				{
					if (value != null)
					{
						num += value.获取怪物列表().Count;
					}
					if (value?.物品列表 != null)
					{
						num2 += value.物品列表.Count;
					}
				}
			}
			主程.添加系统日志($"[详细状态] 地图怪物总数: {num}", true, true);
			主程.添加系统日志($"[详细状态] 地图物品总数: {num2}", true, true);
		}
		catch (Exception ex)
		{
			主程.添加系统日志("[详细状态] 记录时发生错误: " + ex.Message, true, true);
		}
	}

	private static void 记录配置表状态()
	{
		try
		{
			FieldInfo[] fields = typeof(系统数据网关).GetFields(BindingFlags.Static | BindingFlags.Public);
			foreach (FieldInfo fieldInfo in fields)
			{
				try
				{
					if (!fieldInfo.FieldType.Name.Contains("Dictionary") && !fieldInfo.FieldType.Name.Contains("ConcurrentDictionary") && !fieldInfo.FieldType.Name.Contains("List"))
					{
						continue;
					}
					object value = fieldInfo.GetValue(null);
					if (value != null)
					{
						PropertyInfo property = value.GetType().GetProperty("Count");
						if (property != null)
						{
							object value2 = property.GetValue(value);
							主程.添加系统日志($"[配置表] {fieldInfo.Name}: {value2}", true, true);
						}
					}
				}
				catch
				{
				}
			}
		}
		catch (Exception ex)
		{
			主程.添加系统日志("[配置表] 记录时发生错误: " + ex.Message, true, true);
		}
	}

	private static void 建议()
	{
		try
		{
			List<string> list = new List<string>();
			Dictionary<string, 门票信息> 门票数据表 = 网络服务网关.门票数据表;
			if (门票数据表 != null && 门票数据表.Count > 5000)
			{
				list.Add("考虑清理更多过期的门票数据");
			}
			HashSet<客户网络> 网络连接表 = 网络服务网关.网络连接表;
			if (网络连接表 != null && 网络连接表.Count > 1000)
			{
				list.Add("检查是否有僵尸连接未被清理");
			}
			Dictionary<int, 物品实例> 物品对象表 = 地图处理网关.物品对象表;
			if (物品对象表 != null && 物品对象表.Count > 10000)
			{
				list.Add("考虑清理地面上的过期物品");
			}
			long num = 0L;
			if (地图处理网关.地图实例表 != null)
			{
				foreach (地图实例 value in 地图处理网关.地图实例表.Values)
				{
					if (value?.物品列表 != null)
					{
						num += value.物品列表.Count;
					}
				}
			}
			if (num > 5000)
			{
				list.Add("考虑清理地图上的过期物品");
			}
			if (list.Count > 0)
			{
				主程.添加系统日志("[优化建议] " + string.Join("; ", list), true, true);
			}
		}
		catch (Exception ex)
		{
			主程.添加系统日志("[优化建议] 生成时发生错误: " + ex.Message, true, true);
		}
	}

	// 注意: 本方法内含阻塞式 Full GC(会冻结逻辑线程), 只允许 GM 手动命令 @清理内存 调用, 不要挂进任何定时逻辑
	public static void 强制垃圾回收()
	{
		try
		{
			long privateMemorySize = 取进程私有内存(DateTime.Now, 强制刷新: true);
			double num = (double)privateMemorySize / 1024.0 / 1024.0;
			if (num > 3072.0)
			{
				GC.Collect(2, GCCollectionMode.Aggressive, blocking: true, compacting: true);
				GC.WaitForPendingFinalizers();
				GC.Collect(2, GCCollectionMode.Aggressive, blocking: true, compacting: true);
			}
			else if (num > 1536.0)
			{
				GC.Collect(0, GCCollectionMode.Optimized, blocking: true);
				GC.Collect(1, GCCollectionMode.Optimized, blocking: true);
				GC.Collect(2, GCCollectionMode.Optimized, blocking: true);
				GC.WaitForPendingFinalizers();
				GC.Collect(2, GCCollectionMode.Aggressive, blocking: true);
			}
			else
			{
				GC.Collect(0, GCCollectionMode.Optimized, blocking: true);
				GC.Collect(1, GCCollectionMode.Optimized, blocking: false);
			}
			GC.WaitForPendingFinalizers();
			long privateMemorySize2 = 取进程私有内存(DateTime.Now, 强制刷新: true);
			double value = (double)privateMemorySize2 / 1024.0 / 1024.0;
			double num2 = (double)(privateMemorySize - privateMemorySize2) / 1024.0 / 1024.0;
			if (num2 < 10.0 && num > 1024.0)
			{
				主程.添加系统日志($"[内存监控] 首次GC释放较少({num2:F2}MB),尝试堆压缩...", true, true);
				GC.Collect(2, GCCollectionMode.Forced, blocking: true);
				GC.WaitForPendingFinalizers();
				GC.Collect(2, GCCollectionMode.Forced, blocking: true);
				long privateMemorySize3 = 取进程私有内存(DateTime.Now, 强制刷新: true);
				double value2 = (double)privateMemorySize3 / 1024.0 / 1024.0;
				double value3 = (double)(privateMemorySize - privateMemorySize3) / 1024.0 / 1024.0;
				主程.添加系统日志($"[内存监控] 垃圾回收完成，当前内存: {value2:F2}MB, 总释放: {value3:F2}MB", true, true);
			}
			else
			{
				主程.添加系统日志($"[内存监控] 垃圾回收完成，当前内存: {value:F2}MB, 释放: {num2:F2}MB", true, true);
			}
		}
		catch (Exception ex)
		{
			主程.添加系统日志("[内存监控] 强制垃圾回收时发生错误: " + ex.Message, true, true);
		}
	}

	public static string 获取内存诊断报告()
	{
		try
		{
			using Process currentProcess = Process.GetCurrentProcess();
			GCMemoryInfo gCMemoryInfo = GC.GetGCMemoryInfo();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("===== 内存诊断报告 =====");
			stringBuilder.AppendLine($"工作集: {(double)currentProcess.WorkingSet64 / 1024.0 / 1024.0:F2} MB");
			stringBuilder.AppendLine($"私有内存: {(double)currentProcess.PrivateMemorySize64 / 1024.0 / 1024.0:F2} MB");
			stringBuilder.AppendLine($"托管堆大小: {(double)GC.GetTotalMemory(forceFullCollection: false) / 1024.0 / 1024.0:F2} MB");
			stringBuilder.AppendLine($"Gen0 回收次数: {GC.CollectionCount(0)}");
			stringBuilder.AppendLine($"Gen1 回收次数: {GC.CollectionCount(1)}");
			stringBuilder.AppendLine($"Gen2 回收次数: {GC.CollectionCount(2)}");
			stringBuilder.AppendLine($"堆总大小: {(double)gCMemoryInfo.HeapSizeBytes / 1024.0 / 1024.0:F2} MB");
			stringBuilder.AppendLine($"可用内存: {(double)gCMemoryInfo.TotalAvailableMemoryBytes / 1024.0 / 1024.0:F2} MB");
			stringBuilder.AppendLine($"内存负载: {(double)gCMemoryInfo.MemoryLoadBytes / 1024.0 / 1024.0:F2} MB");
			stringBuilder.AppendLine("========================");
			return stringBuilder.ToString();
		}
		catch (Exception ex)
		{
			return "获取内存诊断报告失败: " + ex.Message;
		}
	}
}
