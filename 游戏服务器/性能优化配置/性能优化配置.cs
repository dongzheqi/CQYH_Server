using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

// 命名空间刻意不叫「性能优化配置」: 本文件的类就叫 性能优化配置, 同名会让 `性能优化配置.高性能模式`
// 这类引用解析到命名空间而非类型(CS0234)。移植进来时这里原本写的是错别字「性成优化配置」来绕开冲突,
// 现改为去掉「配置」二字的 性能优化, 既无冲突也不是错字。JSON 持久化走 System.Text.Json 的
// Serialize<T>/Deserialize<T>, 文件里不含类型名, 故改命名空间对已有的 配置/性能优化配置.json 无影响。
namespace 游戏服务器.性能优化;

// 借鉴移植(参考引擎): 性能/内存优化的静态配置基座。纯 lock + JSON 持久化, 不碰任何游戏状态。
// 日志调用已由 Logger.SystemLog(带caller/line) 机械替换为 主程.添加系统日志(msg, hardLog, showDiag)。
public static class 性能优化配置
{
	private static readonly object _lockObject = new object();

	private static bool _高性能模式 = false;

	private static int _内存监控间隔 = 1;

	private static bool _启用详细内存日志 = false;

	private static bool _启用自动清理内存 = true;

	private static int _自动清理内存间隔 = 5;

	// 默认阈值从参照的 1548MB 上调到 3072MB: 本引擎满地图满怪的私有字节常态就在 1.5G 上下,
	// 阈值过低会让服务器长期处在"超阈值"状态, 白白反复触发清理(旧版更是直接每秒清一轮把服卡死)
	private static int _内存阈值MB = 3072;

	private static bool _清理过期门票 = true;

	private static bool _清理地图过期物品 = true;

	public static bool 高性能模式
	{
		get
		{
			lock (_lockObject)
			{
				return _高性能模式;
			}
		}
		set
		{
			lock (_lockObject)
			{
				if (_高性能模式 != value)
				{
					_高性能模式 = value;
					On配置变更("高性能模式", value);
				}
			}
		}
	}

	public static int 内存监控间隔
	{
		get
		{
			lock (_lockObject)
			{
				return _内存监控间隔;
			}
		}
		set
		{
			lock (_lockObject)
			{
				if (value < 1 || value > 60)
				{
					throw new ArgumentOutOfRangeException("内存监控间隔", "内存监控间隔必须在1-60分钟之间");
				}
				if (_内存监控间隔 != value)
				{
					_内存监控间隔 = value;
					On配置变更("内存监控间隔", value);
				}
			}
		}
	}

	public static bool 启用详细内存日志
	{
		get
		{
			lock (_lockObject)
			{
				return _启用详细内存日志;
			}
		}
		set
		{
			lock (_lockObject)
			{
				if (_启用详细内存日志 != value)
				{
					_启用详细内存日志 = value;
					On配置变更("启用详细内存日志", value);
				}
			}
		}
	}

	public static bool 启用自动清理内存
	{
		get
		{
			lock (_lockObject)
			{
				return _启用自动清理内存;
			}
		}
		set
		{
			lock (_lockObject)
			{
				if (_启用自动清理内存 != value)
				{
					_启用自动清理内存 = value;
					On配置变更("启用自动清理内存", value);
				}
			}
		}
	}

	public static int 自动清理内存间隔
	{
		get
		{
			lock (_lockObject)
			{
				return _自动清理内存间隔;
			}
		}
		set
		{
			lock (_lockObject)
			{
				if (value < 1 || value > 1440)
				{
					throw new ArgumentOutOfRangeException("自动清理内存间隔", "自动清理内存间隔必须在1-1440分钟之间");
				}
				if (_自动清理内存间隔 != value)
				{
					_自动清理内存间隔 = value;
					On配置变更("自动清理内存间隔", value);
				}
			}
		}
	}

	public static int 内存阈值MB
	{
		get
		{
			lock (_lockObject)
			{
				return _内存阈值MB;
			}
		}
		set
		{
			lock (_lockObject)
			{
				if (value < 512 || value > 8192)
				{
					throw new ArgumentOutOfRangeException("内存阈值MB", "内存阈值必须在512-8192MB之间");
				}
				if (_内存阈值MB != value)
				{
					_内存阈值MB = value;
					On配置变更("内存阈值MB", value);
				}
			}
		}
	}

	public static bool 清理过期门票
	{
		get
		{
			lock (_lockObject)
			{
				return _清理过期门票;
			}
		}
		set
		{
			lock (_lockObject)
			{
				if (_清理过期门票 != value)
				{
					_清理过期门票 = value;
					On配置变更("清理过期门票", value);
				}
			}
		}
	}

	public static bool 清理地图过期物品
	{
		get
		{
			lock (_lockObject)
			{
				return _清理地图过期物品;
			}
		}
		set
		{
			lock (_lockObject)
			{
				if (_清理地图过期物品 != value)
				{
					_清理地图过期物品 = value;
					On配置变更("清理地图过期物品", value);
				}
			}
		}
	}

	public static event Action<string, object> 配置变更;

	private static void On配置变更(string 配置名, object 新值)
	{
		try
		{
			性能优化配置.配置变更?.Invoke(配置名, 新值);
		}
		catch (Exception ex)
		{
			主程.添加系统日志($"配置变更事件处理失败 - 配置: {配置名}, 新值: {新值}, 错误: {ex.Message}", true, true);
		}
	}

	public static string 获取配置摘要()
	{
		lock (_lockObject)
		{
			return $"高性能模式: {_高性能模式}, 内存监控间隔: {_内存监控间隔}分钟, 详细日志: {_启用详细内存日志}, 自动清理: {_启用自动清理内存}, 清理间隔: {_自动清理内存间隔}分钟, 内存阈值: {_内存阈值MB}MB";
		}
	}

	public static void 重置为默认()
	{
		lock (_lockObject)
		{
			bool num = _高性能模式;
			int num2 = _内存监控间隔;
			bool flag = _启用详细内存日志;
			bool flag2 = _启用自动清理内存;
			int num3 = _自动清理内存间隔;
			int num4 = _内存阈值MB;
			bool flag3 = _清理过期门票;
			bool flag4 = _清理地图过期物品;
			_高性能模式 = false;
			_内存监控间隔 = 1;
			_启用详细内存日志 = false;
			_启用自动清理内存 = true;
			_自动清理内存间隔 = 5;
			_内存阈值MB = 3072;
			_清理过期门票 = true;
			_清理地图过期物品 = true;
			if (num)
			{
				On配置变更("高性能模式", false);
			}
			if (num2 != 1)
			{
				On配置变更("内存监控间隔", 5);
			}
			if (!flag)
			{
				On配置变更("启用详细内存日志", true);
			}
			if (!flag2)
			{
				On配置变更("启用自动清理内存", true);
			}
			if (num3 != 5)
			{
				On配置变更("自动清理内存间隔", 5);
			}
			if (num4 != 3072)
			{
				On配置变更("内存阈值MB", 3072);
			}
			if (!flag3)
			{
				On配置变更("清理过期门票", true);
			}
			if (!flag4)
			{
				On配置变更("清理地图过期物品", true);
			}
		}
	}

	public static void 加载配置文件(string 配置文件路径 = "配置/性能优化配置.json")
	{
		try
		{
			if (!File.Exists(配置文件路径))
			{
				主程.添加系统日志("性能优化配置文件不存在: " + 配置文件路径 + "，将创建默认配置", true, true);
				保存配置文件(配置文件路径);
				return;
			}
			性能优化配置数据 性能优化配置数据2 = JsonSerializer.Deserialize<性能优化配置数据>(File.ReadAllText(配置文件路径));
			if (性能优化配置数据2 != null)
			{
				lock (_lockObject)
				{
					高性能模式 = 性能优化配置数据2.高性能模式;
					内存监控间隔 = 性能优化配置数据2.内存监控间隔;
					启用详细内存日志 = 性能优化配置数据2.启用详细内存日志;
					启用自动清理内存 = 性能优化配置数据2.启用自动清理内存;
					自动清理内存间隔 = 性能优化配置数据2.自动清理内存间隔;
					内存阈值MB = 性能优化配置数据2.内存阈值MB;
					清理过期门票 = 性能优化配置数据2.清理过期门票;
					清理地图过期物品 = 性能优化配置数据2.清理地图过期物品;
				}
				主程.添加系统日志("性能优化配置加载成功: " + 获取配置摘要(), true, true);
			}
		}
		catch (Exception ex)
		{
			主程.添加系统日志("加载性能优化配置失败: " + ex.Message + "，将使用默认配置", true, true);
		}
	}

	public static void 保存配置文件(string 配置文件路径 = "配置/性能优化配置.json")
	{
		try
		{
			lock (_lockObject)
			{
				string contents = JsonSerializer.Serialize(new 性能优化配置数据
				{
					高性能模式 = _高性能模式,
					内存监控间隔 = _内存监控间隔,
					启用详细内存日志 = _启用详细内存日志,
					启用自动清理内存 = _启用自动清理内存,
					自动清理内存间隔 = _自动清理内存间隔,
					内存阈值MB = _内存阈值MB,
					清理过期门票 = _清理过期门票,
					清理地图过期物品 = _清理地图过期物品
				}, new JsonSerializerOptions
				{
					WriteIndented = true,
					Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
				});
				string directoryName = Path.GetDirectoryName(配置文件路径);
				if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				File.WriteAllText(配置文件路径, contents);
				主程.添加系统日志("性能优化配置保存成功: " + 配置文件路径, true, true);
			}
		}
		catch (Exception ex)
		{
			主程.添加系统日志("保存性能优化配置失败: " + ex.Message, true, true);
		}
	}
}
