using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using 游戏服务器.地图类;
using 游戏服务器.性成优化配置;
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

	private const int 每批清理数量 = 100;

	private static bool _后台清理进行中 = false;

	private static readonly object _后台清理锁 = new object();

	private static readonly HashSet<int> 主城地图编号 = new HashSet<int> { 143, 147, 152, 178 };

	public static void 记录启动内存()
	{
		if (_已记录启动内存)
		{
			return;
		}
		_已记录启动内存 = true;
		try
		{
			long privateMemorySize = Process.GetCurrentProcess().PrivateMemorySize64;
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
			主程.添加系统日志("[内存监控] 高性能模式已启用，跳过内存检查", true, true);
			return;
		}
		DateTime now = DateTime.Now;
		try
		{
			long privateMemorySize = Process.GetCurrentProcess().PrivateMemorySize64;
			double num = (double)privateMemorySize / 1024.0 / 1024.0;
			int 内存阈值MB = 性能优化配置.内存阈值MB;
			if (num > (double)内存阈值MB)
			{
				主程.添加系统日志($"[内存警告] 内存使用过高: {num:F2}MB (阈值: {内存阈值MB}MB)", true, true);
				记录集合状态();
				建议();
				尝试自动清理(强制清理: true);
			}
			else
			{
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
			主程.添加系统日志("[自动清理] 自动清理已禁用，跳过执行", true, true);
			return;
		}
		lock (_自动清理锁)
		{
			if (_自动清理进行中)
			{
				主程.添加系统日志("[自动清理] 清理正在进行中，跳过本次", true, true);
				return;
			}
			DateTime now = DateTime.Now;
			TimeSpan timeSpan = TimeSpan.FromMinutes(性能优化配置.自动清理内存间隔);
			TimeSpan timeSpan2 = now - _上次自动清理时间;
			if (!强制清理 && timeSpan2 < timeSpan)
			{
				if (性能优化配置.启用详细内存日志)
				{
					主程.添加系统日志($"[自动清理] 跳过清理，距离上次清理还有 {(timeSpan - timeSpan2).TotalSeconds:F0} 秒", true, true);
				}
				return;
			}
			主程.添加系统日志($"[自动清理] 触发清理，强制={强制清理}，距离上次清理={timeSpan2.TotalMinutes:F1}分钟", true, true);
			_自动清理进行中 = true;
		}
		try
		{
			double num = (double)Process.GetCurrentProcess().PrivateMemorySize64 / 1024.0 / 1024.0;
			主程.添加系统日志("[自动清理] ====== 开始执行自动内存清理 ======", true, true);
			主程.添加系统日志($"[自动清理] 当前内存: {num:F2}MB", true, true);
			主程.添加系统日志($"[自动清理] 配置: 间隔={性能优化配置.自动清理内存间隔}分钟, 阈值={性能优化配置.内存阈值MB}MB", true, true);
			执行自动清理();
			主程.添加系统日志("[自动清理] 执行GC...", true, true);
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
			double num2 = (double)Process.GetCurrentProcess().PrivateMemorySize64 / 1024.0 / 1024.0;
			double value = num - num2;
			主程.添加系统日志("[自动清理] ====== 清理完成 ======", true, true);
			主程.添加系统日志($"[自动清理] 当前内存: {num2:F2}MB，释放: {value:F2}MB", true, true);
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
		if (地图处理网关.地图实例表 == null)
		{
			return 0;
		}
		DateTime now = DateTime.Now;
		try
		{
			int num2;
			do
			{
				num2 = 0;
				foreach (地图实例 item in 地图处理网关.地图实例表.Values.ToList())
				{
					if (item?.对象列表 == null)
					{
						continue;
					}
					List<怪物实例> list = new List<怪物实例>();
					// 我方怪物存于 地图实例.对象列表(HashSet<地图对象>), 无独立怪物列表, 故遍历对象列表按 is 怪物实例 过滤(含已死亡怪物)
					foreach (地图对象 对象项 in item.对象列表.ToList())
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
							if (list.Count >= 最大清理数量)
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
							num2++;
						}
						catch (Exception ex)
						{
							主程.添加系统日志($"[智能清理] 清理怪物 {item3.地图编号} 时发生错误: {ex.Message}", true, true);
						}
					}
					if (num2 >= 最大清理数量)
					{
						break;
					}
				}
			}
			while (num2 >= 最大清理数量);
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
		try
		{
			value5 = 优化角色字典内存();
		}
		catch (Exception ex5)
		{
			主程.添加系统日志("[自动清理] 优化角色字典内存时发生错误: " + ex5.Message, true, true);
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
				if (num2 >= 50)
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

	private static int 清理过期门票数据()
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

	private static int 清理地图过期物品()
	{
		int num = 0;
		if (地图处理网关.地图实例表 == null)
		{
			return 0;
		}
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
						if (list.Count >= 500)
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
				if (num2 >= 500)
				{
					break;
				}
			}
		}
		while (num2 >= 500);
		return num;
	}

	private static int 清理空地图实例()
	{
		int num = 0;
		if (地图处理网关.地图实例表 == null)
		{
			return 0;
		}
		List<int> list = new List<int>();
		DateTime now = DateTime.Now;
		if (地图处理网关.地图实例表.Count <= 150)
		{
			return 0;
		}
		TimeSpan timeSpan = TimeSpan.FromMinutes(30.0);
		foreach (KeyValuePair<int, 地图实例> item in 地图处理网关.地图实例表)
		{
			地图实例 value = item.Value;
			// 我方怪物在 对象列表 内, "对象列表<=0" 已隐含"无怪物", 故删去参照冗余的 怪物列表 子判定
			if (value != null && !value.副本地图 && !主城地图编号.Contains(value.地图编号) && (value.玩家列表 == null || value.玩家列表.Count <= 0) && (value.物品列表 == null || value.物品列表.Count <= 0) && (value.对象列表 == null || value.对象列表.Count <= 0) && value.已初始化 && !(now - value.执行计时 < timeSpan))
			{
				list.Add(item.Key);
			}
		}
		int num2 = Math.Min(list.Count, 5);
		for (int i = 0; i < num2; i++)
		{
			try
			{
				int num3 = list[i];
				if (地图处理网关.地图实例表.TryGetValue(num3, out var value2) && value2 != null && value2.玩家列表?.Count == 0 && value2 != null && value2.获取怪物列表().Count == 0 && 地图处理网关.地图实例表.Remove(num3, out _))
				{
					num++;
					主程.添加系统日志($"[空地图清理] 清理空地图: {value2?.地图模板?.地图名字}(ID:{num3})", true, true);
				}
			}
			catch (Exception ex)
			{
				主程.添加系统日志("[自动清理] 清理地图时发生错误: " + ex.Message, true, true);
			}
		}
		if (num > 0)
		{
			主程.添加系统日志($"[空地图清理] 本轮清理 {num} 个空地图，剩余待清理: {Math.Max(0, list.Count - 5)} 个，当前总实例: {地图处理网关.地图实例表.Count}", true, true);
		}
		return num;
	}

	public static int 优化角色字典内存()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		try
		{
			if (游戏数据网关.角色数据表?.数据表 == null)
			{
				return 0;
			}
			DateTime now = DateTime.Now;
			int num5 = 7;
			foreach (KeyValuePair<int, 游戏数据> item in 游戏数据网关.角色数据表.数据表.ToList())
			{
				if (!(item.Value is 角色数据 角色数据))
				{
					continue;
				}
				客户网络 网络;
				bool flag = 角色数据.角色在线(out 网络);
				if (flag)
				{
					num2++;
				}
				else
				{
					num3++;
				}
				if (角色数据.角色变量 != null && 角色数据.角色变量.Count > 0)
				{
					List<int> list = new List<int>();
					foreach (KeyValuePair<int, int> item2 in 角色数据.角色变量)
					{
						if (item2.Value == 0 && !Is日程变量(item2.Key))
						{
							list.Add(item2.Key);
							num4++;
						}
						else if (!flag && !Is日程变量(item2.Key) && (now - 角色数据.离线日期.V).TotalDays >= (double)num5)
						{
							list.Add(item2.Key);
							num4++;
						}
					}
					if (list.Count > 0)
					{
						foreach (int item3 in list)
						{
							角色数据.角色变量.Remove(item3);
							num++;
						}
					}
				}
				if (!flag && 角色数据.角色变量 != null)
				{
					_ = 角色数据.角色变量.Count;
				}
			}
			if (num > 0 || num2 > 0)
			{
				主程.添加系统日志($"[字典优化] 处理角色: 在线{num2}个, 离线{num3}个, 清理变量: {num}个(临时{num4})", true, true);
			}
		}
		catch (Exception ex)
		{
			主程.添加系统日志("[字典优化] 优化时发生错误: " + ex.Message, true, true);
		}
		return num;
	}

	private static bool Is日程变量(int 变量索引)
	{
		try
		{
			if (Enum.IsDefined(typeof(日程变量), 变量索引))
			{
				return true;
			}
		}
		catch
		{
		}
		return false;
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

	public static void 强制垃圾回收()
	{
		try
		{
			long privateMemorySize = Process.GetCurrentProcess().PrivateMemorySize64;
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
			long privateMemorySize2 = Process.GetCurrentProcess().PrivateMemorySize64;
			double value = (double)privateMemorySize2 / 1024.0 / 1024.0;
			double num2 = (double)(privateMemorySize - privateMemorySize2) / 1024.0 / 1024.0;
			if (num2 < 10.0 && num > 1024.0)
			{
				主程.添加系统日志($"[内存监控] 首次GC释放较少({num2:F2}MB),尝试堆压缩...", true, true);
				GC.Collect(2, GCCollectionMode.Forced, blocking: true);
				GC.WaitForPendingFinalizers();
				GC.Collect(2, GCCollectionMode.Forced, blocking: true);
				long privateMemorySize3 = Process.GetCurrentProcess().PrivateMemorySize64;
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
			Process currentProcess = Process.GetCurrentProcess();
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
