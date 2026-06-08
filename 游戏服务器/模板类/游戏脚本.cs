using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using 游戏服务器.地图类;
using 游戏服务器.数据类;
using NLua;

namespace 游戏服务器.模板类
{
	public sealed class 游戏脚本
	{
		public static long scriptMemoryUseage;

		public static Lua 状态机;

		public static LuaFunction 玩家进入游戏;

		public static LuaFunction reload;

		public static LuaFunction 玩家对话NPC;

		public static LuaFunction 玩家点击NPC;

		public static LuaFunction 使用物品;

		public static LuaFunction 合成物品;

		public static LuaFunction 合成装备;

		public static LuaFunction 装备重铸;

		public static LuaFunction 技能重铸;

		public static LuaFunction 勋章洗练;

		public static LuaFunction 武器铸魂;

		public static LuaFunction 武器祈祷;

		public static LuaFunction 武器升级;

		public static LuaFunction 防具升级;

		public static LuaFunction 装备神佑;

		public static LuaFunction 装备分解;

		public static LuaFunction 合无相石;

		public static LuaFunction 装备精炼;

		public static LuaFunction 获取灵石配置;

		public static LuaFunction 地图创建事件;

		public static LuaFunction 地图执行事件;

		public static LuaFunction 地图进入事件;

		public static LuaFunction 地图退出事件;

		public static LuaFunction 地图销毁事件;

		public static LuaFunction 守卫创建事件;

		public static LuaFunction 守卫执行事件;

		public static LuaFunction 月卡奖励领取;

		public static LuaFunction 怪物死亡事件;

		public static LuaFunction 玩家死亡事件;

		public static LuaFunction 玩家执行事件;

		public static LuaFunction 玩家升级事件;

		public static LuaFunction 玩家穿卸事件;

		public static LuaFunction 玩家属性事件;

		public static LuaFunction 玩家充值事件;

		public static LuaFunction 特惠礼包事件;

		public static LuaFunction 玩家拾取事件;

		public static LuaFunction 签到奖励领取;

		public static LuaFunction 每日清空事件;

		public static LuaFunction 执行触发BUFF;

		public static LuaFunction 添加触发BUFF;

		public static LuaFunction 移除触发BUFF;

		public static LuaFunction 删除触发BUFF;

		public static LuaFunction 主动GC;

		private static DateTime 垃圾收集时间;

		private static LuaFunction CallLua;

		public static int 出生地图;

		public static void 测试(Point[] a)
		{
		}

		// 供 Lua 搜索器调用: 用 .NET 文件 API 读取脚本源码, 避开 Lua 原生 fopen 对中文/非 ANSI 代码页路径无法打开的问题
		public static string 加载lua模块(string 模块名)
		{
			try
			{
				if (string.IsNullOrEmpty(模块名))
				{
					return null;
				}
				string text = Path.Combine(Settings.游戏数据目录, "System", "lua", 模块名.Replace('/', '\\') + ".lua");
				if (File.Exists(text))
				{
					return File.ReadAllText(text);
				}
			}
			catch
			{
			}
			return null;
		}

		public static void 初始化脚本系统()
		{
			if (!Settings.开启lua)
			{
				return;
			}
			try
			{
				if (游戏脚本.状态机 == null)
				{
					游戏脚本.状态机 = new Lua();
					游戏脚本.状态机.State.Encoding = Encoding.UTF8;
					主程.添加系统日志("开始加载lua系统脚本");
				}
				游戏脚本.状态机.LoadCLRPackage();
				游戏脚本.状态机.DoString("import ('System.Drawing') ");
				游戏脚本.状态机.DoString("import ('游戏服务器') ");
				游戏脚本.状态机.RegisterFunction("print", null, typeof(主程).GetMethod("添加系统日志"));
				游戏脚本.状态机.RegisterFunction("GetMonster", null, typeof(游戏怪物).GetMethod("获取游戏怪物"));
				游戏脚本.状态机.RegisterFunction("__clr_loadmodule", null, typeof(游戏脚本).GetMethod("加载lua模块"));
				游戏脚本.状态机.DoString("package.path = \"" + Settings.游戏数据目录.Replace("\\", "/") + "/System/lua/?.lua;\" .. package.path");
				// 注册一个走 .NET 文件 API 的 Lua 模块搜索器, 优先于默认 path 搜索器, 从而支持中文/非 ASCII 安装路径
				游戏脚本.状态机.DoString("table.insert(package.searchers, 2, function(name) local src = __clr_loadmodule(name) if src then local f, e = load(src, '@'..name) if f then return f end error(e) end return '\\n\\t[CLR] no lua file for module '..name end)");
				游戏脚本.状态机.DoString(File.ReadAllText(Settings.游戏数据目录 + "\\System\\lua\\main.lua"), "@main.lua");
				// 注入未知暗殿一层/二层每日进入计数常量为 Lua 全局：版本脚本 6350/6359.lua 未定义这些名字，
				// 否则索引会塌缩到 脚本变量[0] 与其它数据互相覆盖。引擎侧统一来源，避免改动版本数据。
				游戏脚本.状态机["数字_未知一层每日进入次数"] = 游戏服务器.副本类.副本.数字_未知一层每日进入次数;
				游戏脚本.状态机["数字_未知二层每日进入次数"] = 游戏服务器.副本类.副本.数字_未知二层每日进入次数;
				// 尸王殿入口玩法: 比奇矿区三层击杀阈值 + 临时传送门存续秒数, 注入为 Lua 全局供 monster.lua 读取
				游戏脚本.状态机["尸王殿开启击杀数"] = Settings.尸王殿开启击杀数;
				游戏脚本.状态机["尸王殿门存续秒数"] = Settings.尸王殿门存续秒数;
				主程.添加系统日志("正在绑定玩家触发脚本");
				主程.添加系统日志("正在绑定技能触发脚本");
				主程.添加系统日志("正在绑定技能NPC对话脚本");
				游戏脚本.玩家对话NPC = 游戏脚本.状态机["npctalk"] as LuaFunction;
				游戏脚本.玩家点击NPC = 游戏脚本.状态机["npclick"] as LuaFunction;
				游戏脚本.使用物品 = 游戏脚本.状态机["player_useitem"] as LuaFunction;
				游戏脚本.合成物品 = 游戏脚本.状态机["player_compose"] as LuaFunction;
				游戏脚本.合成装备 = 游戏脚本.状态机["player_equip_compose"] as LuaFunction;
				游戏脚本.装备重铸 = 游戏脚本.状态机["player_equip_create"] as LuaFunction;
				游戏脚本.技能重铸 = 游戏脚本.状态机["player_skill_create"] as LuaFunction;
				游戏脚本.武器铸魂 = 游戏脚本.状态机["player_equip_levelup_advanced"] as LuaFunction;
				游戏脚本.武器升级 = 游戏脚本.状态机["player_equip_levelup_advanced_check"] as LuaFunction;
				游戏脚本.武器祈祷 = 游戏脚本.状态机["player_weapon_pray"] as LuaFunction;
				游戏脚本.防具升级 = 游戏脚本.状态机["player_armor_levelup"] as LuaFunction;
				游戏脚本.装备神佑 = 游戏脚本.状态机["player_shenyou"] as LuaFunction;
				游戏脚本.装备分解 = 游戏脚本.状态机["player_decompose"] as LuaFunction;
				游戏脚本.合无相石 = 游戏脚本.状态机["player_compound_formula"] as LuaFunction;
				游戏脚本.装备精炼 = 游戏脚本.状态机["player_equip_refine"] as LuaFunction;
				游戏脚本.获取灵石配置 = 游戏脚本.状态机["server_get_soul"] as LuaFunction;
				游戏脚本.地图创建事件 = 游戏脚本.状态机["scene_instance_create"] as LuaFunction;
				游戏脚本.地图执行事件 = 游戏脚本.状态机["scene_instance_run"] as LuaFunction;
				游戏脚本.地图进入事件 = 游戏脚本.状态机["scene_instance_player_into"] as LuaFunction;
				游戏脚本.地图退出事件 = 游戏脚本.状态机["scene_instance_player_leave"] as LuaFunction;
				游戏脚本.地图销毁事件 = 游戏脚本.状态机["scene_instance_destroy"] as LuaFunction;
				游戏脚本.守卫创建事件 = 游戏脚本.状态机["npc_create"] as LuaFunction;
				游戏脚本.守卫执行事件 = 游戏脚本.状态机["npc_run"] as LuaFunction;
				游戏脚本.月卡奖励领取 = 游戏脚本.状态机["monthly_card_reward"] as LuaFunction;
				游戏脚本.怪物死亡事件 = 游戏脚本.状态机["monster_die"] as LuaFunction;
				游戏脚本.玩家死亡事件 = 游戏脚本.状态机["player_die"] as LuaFunction;
				游戏脚本.玩家执行事件 = 游戏脚本.状态机["player_run"] as LuaFunction;
				游戏脚本.玩家升级事件 = 游戏脚本.状态机["player_levelup"] as LuaFunction;
				游戏脚本.玩家穿卸事件 = 游戏脚本.状态机["player_wear"] as LuaFunction;
				游戏脚本.玩家属性事件 = 游戏脚本.状态机["player_abil"] as LuaFunction;
				游戏脚本.玩家充值事件 = 游戏脚本.状态机["player_payment"] as LuaFunction;
				游戏脚本.特惠礼包事件 = 游戏脚本.状态机["player_week"] as LuaFunction;
				游戏脚本.玩家拾取事件 = 游戏脚本.状态机["player_pickup"] as LuaFunction;
				游戏脚本.签到奖励领取 = 游戏脚本.状态机["player_checkin"] as LuaFunction;
				游戏脚本.每日清空事件 = 游戏脚本.状态机["player_clear_everyday"] as LuaFunction;
				游戏脚本.添加触发BUFF = 游戏脚本.状态机["buff_add"] as LuaFunction;
				游戏脚本.移除触发BUFF = 游戏脚本.状态机["buff_remove"] as LuaFunction;
				游戏脚本.删除触发BUFF = 游戏脚本.状态机["buff_delete"] as LuaFunction;
				游戏脚本.执行触发BUFF = 游戏脚本.状态机["buff_run"] as LuaFunction;
				游戏脚本.勋章洗练 = 游戏脚本.状态机["player_randomize"] as LuaFunction;
				游戏脚本.主动GC = 游戏脚本.状态机["engine_gc"] as LuaFunction;
				游戏脚本.CallLua = 游戏脚本.状态机["call_lua"] as LuaFunction;
				游戏脚本.垃圾收集时间 = DateTime.Now.AddSeconds(5.0);
				游戏脚本.出生地图 = ((游戏脚本.状态机["CONST_PLAYER_SPAWNMAP"] != null) ? Convert.ToInt32(游戏脚本.状态机["CONST_PLAYER_SPAWNMAP"]) : 142);
				主程.添加系统日志("加载lua系统脚本完成");
			}
			catch (Exception ex)
			{
				主程.添加系统日志("加载lua脚本出现异常:" + ex.Message);
			}
		}

		public static void 进入游戏(玩家实例 玩家)
		{
			if (Settings.开启lua && 游戏脚本.玩家进入游戏 != null && 玩家 != null)
			{
				游戏脚本.玩家进入游戏.Call(玩家);
			}
		}

		public static void 重新加载()
		{
			if (Settings.开启lua && 游戏脚本.reload != null)
			{
				游戏脚本.reload.Call(游戏脚本.reload);
				游戏脚本.初始化脚本系统();
			}
		}

		public static string 对话NPC(玩家实例 玩家, 守卫实例 守卫)
		{
			try
			{
				if (游戏脚本.玩家对话NPC != null && 玩家 != null && 守卫 != null)
				{
					object[] array;
					array = 游戏脚本.玩家对话NPC.Call(玩家, 守卫);
					return (array == null || array.Count() == 0) ? string.Empty : (array[0] as string);
				}
				return string.Empty;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("对话NPC出现异常:" + ex.Message);
				return string.Empty;
			}
		}

		public static string 点击NPC(玩家实例 玩家, 守卫实例 对话守卫, int 选项编号)
		{
			try
			{
				if (游戏脚本.玩家点击NPC != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.玩家点击NPC.Call(玩家, 对话守卫, 选项编号);
					return (array == null || array.Count() == 0) ? string.Empty : (array[0] as string);
				}
				return string.Empty;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("点击NPC出现异常:" + ex.Message);
				return string.Empty;
			}
		}

		public static long 玩家使用物品(玩家实例 玩家, 物品数据 物品)
		{
			try
			{
				if (游戏脚本.使用物品 != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.使用物品.Call(玩家, 物品);
					return (array == null || array.Count() == 0) ? 0L : ((long)array[0]);
				}
				return 0L;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("使用物品出现异常:" + ex.Message);
				return 0L;
			}
		}

		public static long 玩家合成物品(玩家实例 玩家, int 模板编号)
		{
			try
			{
				if (游戏脚本.合成物品 != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.合成物品.Call(玩家, 模板编号);
					return (array == null || array.Count() == 0) ? 0L : ((long)array[0]);
				}
				return 0L;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("合成物品出现异常:" + ex.Message);
				return 0L;
			}
		}

		public static long 玩家重铸装备(玩家实例 玩家, int 部位, int 未知)
		{
			try
			{
				if (游戏脚本.装备重铸 != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.装备重铸.Call(玩家, 部位, 未知);
					return (array == null || array.Count() == 0) ? 0L : ((long)array[0]);
				}
				return 0L;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("重铸装备出现异常:" + ex.Message);
				return 0L;
			}
		}

		public static long 玩家重铸技能(玩家实例 玩家)
		{
			try
			{
				if (游戏脚本.技能重铸 != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.技能重铸.Call(玩家);
					return (array == null || array.Count() == 0) ? 0L : ((long)array[0]);
				}
				return 0L;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("重铸技能出现异常:" + ex.Message);
				return 0L;
			}
		}

		public static long 玩家合成装备(玩家实例 玩家, bool 合成勋章, int 模板编号, byte[] 未知参数, byte[] 合成参数)
		{
			try
			{
				if (游戏脚本.合成装备 != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.合成装备.Call(玩家, 合成勋章, 模板编号, 未知参数, 合成参数);
					return (array == null || array.Count() == 0) ? 0L : ((long)array[0]);
				}
				return 0L;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("合成装备出现异常:" + ex.Message);
				return 0L;
			}
		}

		public static long 玩家武器铸魂(玩家实例 玩家)
		{
			try
			{
				if (游戏脚本.武器铸魂 != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.武器铸魂.Call(玩家);
					return (array == null || array.Count() == 0) ? 0L : ((long)array[0]);
				}
				return 0L;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("武器铸魂出现异常:" + ex.Message);
				return 0L;
			}
		}

		public static long 玩家武器升级(玩家实例 玩家)
		{
			try
			{
				if (游戏脚本.武器升级 != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.武器升级.Call(玩家);
					return (array == null || array.Count() == 0) ? 0L : ((long)array[0]);
				}
				return 0L;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("武器升级出现异常:" + ex.Message);
				return 0L;
			}
		}

		public static long 玩家武器祈祷(玩家实例 玩家, int 未知参数)
		{
			try
			{
				if (游戏脚本.武器祈祷 != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.武器祈祷.Call(玩家, 未知参数);
					return (array == null || array.Count() == 0) ? 0L : ((long)array[0]);
				}
				return 0L;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("武器祈祷出现异常:" + ex.Message);
				return 0L;
			}
		}

		public static long 玩家防具升级(玩家实例 玩家, int 装备部位)
		{
			try
			{
				if (游戏脚本.防具升级 != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.防具升级.Call(玩家, 装备部位);
					return (array == null || array.Count() == 0) ? 0L : ((long)array[0]);
				}
				return 0L;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("防具升级出现异常:" + ex.Message);
				return 0L;
			}
		}

		public static long 玩家装备神佑(玩家实例 玩家, int 装备部位)
		{
			try
			{
				if (游戏脚本.装备神佑 != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.装备神佑.Call(玩家, 装备部位);
					return (array == null || array.Count() == 0) ? 0L : ((long)array[0]);
				}
				return 0L;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("装备神佑出现异常:" + ex.Message);
				return 0L;
			}
		}

		public static int[] 玩家装备分解(玩家实例 玩家, int 背包类型, int 物品位置, int 分解数量)
		{
			try
			{
				if (游戏脚本.装备分解 != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.装备分解.Call(玩家, 背包类型, 物品位置, 分解数量);
					if (array != null && array[0] is LuaTable luaTable)
					{
						int[] array2;
						array2 = new int[7];
						for (int i = 0; i < luaTable.Keys.Count; i++)
						{
							array2[i] = (int)(long)luaTable[i + 1];
						}
						return array2;
					}
				}
				return null;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("装备分解出现异常:" + ex.Message);
				return null;
			}
		}

		public static long 玩家合无相石(玩家实例 玩家, int 物品位置, int 一键合成)
		{
			try
			{
				if (游戏脚本.合无相石 != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.合无相石.Call(玩家, 物品位置, 一键合成);
					return (array == null || array.Count() == 0) ? 0L : ((long)array[0]);
				}
				return 0L;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("合无相石出现异常:" + ex.Message);
				return 0L;
			}
		}

		public static long 玩家装备精炼(玩家实例 玩家, int 背包类型, int 背包位置)
		{
			try
			{
				if (游戏脚本.装备精炼 != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.装备精炼.Call(玩家, 背包类型, 背包位置);
					return (array == null || array.Count() == 0) ? 0L : ((long)array[0]);
				}
				return 0L;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("装备精炼出现异常:" + ex.Message);
				return 0L;
			}
		}

		public static void 地图创建(地图实例 地图)
		{
			try
			{
				if (游戏脚本.地图创建事件 != null && 地图 != null)
				{
					游戏脚本.地图创建事件.Call(地图);
				}
			}
			catch (Exception ex)
			{
				主程.添加系统日志("地图创建出现异常:" + ex.Message);
			}
		}

		public static void 地图执行(地图实例 地图)
		{
			try
			{
				if (游戏脚本.地图执行事件 != null && 地图 != null)
				{
					Stopwatch stopwatch;
					stopwatch = Stopwatch.StartNew();
					游戏脚本.地图执行事件.Call(地图);
					stopwatch.Stop();
					if (stopwatch.ElapsedMilliseconds > 500L)
					{
						主程.添加系统日志($"Lua 地图执行 , 耗时:{stopwatch.ElapsedMilliseconds} 地图编号:{地图.地图编号}");
					}
				}
			}
			catch (Exception ex)
			{
				主程.添加系统日志("地图执行出现异常:" + ex.Message);
			}
		}

		public static void 地图进入(地图实例 地图, 玩家实例 玩家)
		{
			try
			{
				if (游戏脚本.地图进入事件 != null && 地图 != null && 玩家 != null)
				{
					游戏脚本.地图进入事件.Call(地图, 玩家);
				}
			}
			catch (Exception ex)
			{
				主程.添加系统日志("地图进入出现异常:" + ex.Message);
			}
		}

		public static void 地图退出(地图实例 地图, 玩家实例 玩家)
		{
			try
			{
				if (游戏脚本.地图退出事件 != null && 地图 != null && 玩家 != null)
				{
					游戏脚本.地图退出事件.Call(地图, 玩家);
				}
			}
			catch (Exception ex)
			{
				主程.添加系统日志("地图退出出现异常:" + ex.Message);
			}
		}

		public static void 地图销毁(地图实例 地图)
		{
			try
			{
				if (游戏脚本.地图销毁事件 != null && 地图 != null)
				{
					游戏脚本.地图销毁事件.Call(地图);
				}
			}
			catch (Exception ex)
			{
				主程.添加系统日志("地图销毁出现异常:" + ex.Message);
			}
		}

		public static void 守卫创建(守卫实例 守卫)
		{
			try
			{
				if (游戏脚本.守卫创建事件 != null && 守卫 != null)
				{
					游戏脚本.守卫创建事件.Call(守卫);
				}
			}
			catch (Exception ex)
			{
				主程.添加系统日志("守卫创建出现异常:" + ex.Message);
			}
		}

		public static void 守卫执行(守卫实例 守卫)
		{
			try
			{
				if (游戏脚本.守卫执行事件 != null && 守卫 != null)
				{
					游戏脚本.守卫执行事件.Call(守卫);
				}
			}
			catch (Exception ex)
			{
				主程.添加系统日志("守卫执行出现异常:" + ex.Message);
			}
		}

		public static bool 月卡奖励(玩家实例 玩家, int 特权类型, int 礼包位置)
		{
			try
			{
				if (游戏脚本.月卡奖励领取 != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.月卡奖励领取.Call(玩家, 特权类型, 礼包位置);
					return array != null && array.Count() != 0 && (bool)array[0];
				}
				return false;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("月卡奖励出现异常:" + ex.Message);
				return false;
			}
		}

		public static void 怪物死亡(怪物实例 怪物, 地图对象 击杀者, bool 技能击杀)
		{
			try
			{
				if (游戏脚本.怪物死亡事件 != null && 怪物 != null)
				{
					游戏脚本.怪物死亡事件.Call(怪物, 击杀者, 技能击杀);
				}
			}
			catch (Exception ex)
			{
				主程.添加系统日志("怪物死亡出现异常:" + ex.Message);
			}
		}

		public static void 玩家死亡(玩家实例 玩家, 地图对象 击杀者, bool 技能击杀)
		{
			try
			{
				if (游戏脚本.玩家死亡事件 != null && 玩家 != null)
				{
					游戏脚本.玩家死亡事件.Call(玩家, 击杀者, 技能击杀);
				}
			}
			catch (Exception ex)
			{
				主程.添加系统日志("玩家死亡出现异常:" + ex.Message);
			}
		}

		public static void 玩家执行(玩家实例 玩家)
		{
			try
			{
				if (游戏脚本.玩家执行事件 != null && 玩家 != null)
				{
					游戏脚本.玩家执行事件.Call(玩家);
				}
			}
			catch (Exception ex)
			{
				主程.添加系统日志("玩家执行出现异常:" + ex.Message);
			}
		}

		public static void 玩家升级(玩家实例 玩家)
		{
			try
			{
				if (游戏脚本.玩家升级事件 != null && 玩家 != null)
				{
					游戏脚本.玩家升级事件.Call(玩家);
				}
			}
			catch (Exception ex)
			{
				主程.添加系统日志("玩家升级出现异常:" + ex.Message);
			}
		}

		public static void 玩家穿卸(玩家实例 玩家, 装备穿戴部位 装备部位, 装备数据 原有装备, 装备数据 现有装备)
		{
			try
			{
				if (游戏脚本.玩家穿卸事件 != null && 玩家 != null)
				{
					游戏脚本.玩家穿卸事件.Call(玩家, 装备部位, 原有装备, 现有装备);
				}
			}
			catch (Exception ex)
			{
				主程.添加系统日志("玩家穿卸出现异常:" + ex.Message);
			}
		}

		public static void 玩家属性(玩家实例 玩家)
		{
			try
			{
				if (游戏脚本.玩家属性事件 != null && 玩家 != null)
				{
					游戏脚本.玩家属性事件.Call(玩家);
				}
			}
			catch (Exception ex)
			{
				主程.添加系统日志("玩家属性出现异常:" + ex.Message);
			}
		}

		public static void 玩家充值(角色数据 玩家, int 充值金额)
		{
			try
			{
				if (游戏脚本.玩家充值事件 != null && 玩家 != null)
				{
					游戏脚本.玩家充值事件.Call(玩家, 充值金额);
				}
			}
			catch (Exception ex)
			{
				主程.添加系统日志("玩家充值出现异常:" + ex.Message);
			}
		}

		public static bool 特惠礼包(玩家实例 玩家, int 礼包编号)
		{
			try
			{
				if (游戏脚本.特惠礼包事件 != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.特惠礼包事件.Call(玩家, 礼包编号);
					return array != null && array.Count() != 0 && (bool)array[0];
				}
				return false;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("特惠礼包出现异常:" + ex.Message);
				return false;
			}
		}

		public static void 玩家拾取(玩家实例 玩家, 物品数据 物品)
		{
			try
			{
				if (游戏脚本.玩家拾取事件 != null && 玩家 != null)
				{
					游戏脚本.玩家拾取事件.Call(玩家, 物品);
				}
			}
			catch (Exception ex)
			{
				主程.添加系统日志("玩家拾取出现异常:" + ex.Message);
			}
		}

		public static long 签到领取(玩家实例 玩家, int 类型, int 位置)
		{
			try
			{
				if (游戏脚本.签到奖励领取 != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.签到奖励领取.Call(玩家, 类型, 位置);
					return (array == null || array.Count() == 0) ? 0L : ((long)array[0]);
				}
				return 0L;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("签到领取出现异常:" + ex.Message);
				return 0L;
			}
		}

		public static long 每日清空(角色数据 玩家)
		{
			try
			{
				if (游戏脚本.每日清空事件 != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.每日清空事件.Call(玩家);
					return (array == null || array.Count() == 0) ? 0L : ((long)array[0]);
				}
				return 0L;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("每日清空出现异常:" + ex.Message);
				return 0L;
			}
		}

		public static long 执行BUFF(地图对象 玩家, Buff数据 数据)
		{
			try
			{
				if (游戏脚本.执行触发BUFF != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.执行触发BUFF.Call(玩家, 数据);
					return (array == null || array.Count() == 0) ? 0L : ((long)array[0]);
				}
				return 0L;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("执行BUFF:" + ex.Message);
				return 0L;
			}
		}

		public static long 添加BUFF(地图对象 玩家, Buff数据 数据)
		{
			try
			{
				if (游戏脚本.添加触发BUFF != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.添加触发BUFF.Call(玩家, 数据);
					return (array == null || array.Count() == 0) ? 0L : ((long)array[0]);
				}
				return 0L;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("添加BUFF:" + ex.Message);
				return 0L;
			}
		}

		public static long 移除BUFF(地图对象 玩家, Buff数据 数据)
		{
			try
			{
				if (游戏脚本.移除触发BUFF != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.移除触发BUFF.Call(玩家, 数据);
					return (array == null || array.Count() == 0) ? 0L : ((long)array[0]);
				}
				return 0L;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("移除BUFF:" + ex.Message);
				return 0L;
			}
		}

		public static long 删除BUFF(地图对象 玩家, Buff数据 数据)
		{
			try
			{
				if (游戏脚本.删除触发BUFF != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.删除触发BUFF.Call(玩家, 数据);
					return (array == null || array.Count() == 0) ? 0L : ((long)array[0]);
				}
				return 0L;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("删除BUFF:" + ex.Message);
				return 0L;
			}
		}

		public static long 玩家勋章洗练(玩家实例 玩家, 装备数据 勋章)
		{
			try
			{
				if (游戏脚本.勋章洗练 != null && 玩家 != null)
				{
					object[] array;
					array = 游戏脚本.勋章洗练.Call(玩家, 勋章);
					return (array == null || array.Count() == 0) ? 0L : ((long)array[0]);
				}
				return 0L;
			}
			catch (Exception ex)
			{
				主程.添加系统日志("勋章洗练:" + ex.Message);
				return 0L;
			}
		}

		public static void 垃圾收集()
		{
			try
			{
				if (!Settings.开启lua || 游戏脚本.状态机 == null || DateTime.Now <= 游戏脚本.垃圾收集时间)
				{
					return;
				}
				游戏脚本.垃圾收集时间 = DateTime.Now.AddSeconds(5.0);
				if (游戏脚本.主动GC != null)
				{
					object[] array;
					array = 游戏脚本.主动GC.Call();
					if (array != null && array.Count() != 0)
					{
						游戏脚本.scriptMemoryUseage = Convert.ToInt64((double)array[0]);
					}
				}
			}
			catch (Exception ex)
			{
				主程.添加系统日志("垃圾收集:" + ex.Message);
			}
		}

		public static void 调用Lua(地图对象 对象, string[] 参数)
		{
			try
			{
				if (Settings.开启lua && 游戏脚本.CallLua != null)
				{
					object[] array;
					array = new object[参数.Length + 1];
					array[0] = 对象;
					Array.Copy(参数, 0, array, 1, 参数.Length);
					游戏脚本.CallLua.Call(array);
				}
			}
			catch (Exception ex)
			{
				主程.添加系统日志("调用Lua:" + ex.Message);
			}
		}
	}
}
