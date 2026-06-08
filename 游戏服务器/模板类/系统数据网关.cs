using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using 游戏服务器.地图类;
using 游戏服务器.数据类;

namespace 游戏服务器.模板类
{
    public static class 系统数据网关
    {

        public static void 加载数据(int 单独加载 = 0)
        {
            List<Type> 模板列表;
            模板列表 = new List<Type>();
            switch (单独加载)
            {
                case 0:
                    模板列表 = new List<Type>
                {
                    typeof(游戏怪物),
                    typeof(地图守卫),
                    typeof(对话数据),
                    typeof(游戏地图),
                    typeof(地形数据),
                    typeof(地图区域),
                    typeof(传送法阵),
                    typeof(怪物刷新),
                    typeof(守卫刷新),
                    typeof(游戏物品),
                    typeof(随机属性),
                    typeof(装备属性),
                    typeof(游戏商店),
                    typeof(珍宝商品),
                    typeof(游戏称号),
                    typeof(铭文技能),
                    typeof(游戏技能),
                    typeof(技能陷阱),
                    typeof(游戏Buff),
                    typeof(龙卫模板),
                    typeof(地图道具),
                    typeof(道具刷新),
                    typeof(GameQuests),
                    typeof(游戏坐骑),
                    typeof(游戏天赋),
                    typeof(装备精炼),
                    typeof(传奇之力),
                    typeof(高级狩猎),
                    typeof(GameAchievements),
                    typeof(游戏威望),
                    typeof(传永七天),
                    typeof(游戏套装),
                    typeof(战功奖励),
                    typeof(战功任务),
                    typeof(杀怪成就),
                    typeof(主题礼包固定物品模板),
                    typeof(主题礼包商品模板),
                    typeof(锻石炼药),
                    typeof(物品过滤),
                    typeof(首杀怪物名单),
                    typeof(装备升级),
                    typeof(精炼阶段),
                    typeof(掉落分组),
                    typeof(物品分解),
                    typeof(合成公式),
                    typeof(合成系统),
                    typeof(月卡奖励),
                    typeof(武器升级),
                    typeof(每周特惠),
                    typeof(每日签到),
                    typeof(灵石配置),
                    typeof(装备合成),
                    typeof(装备重铸),
                    typeof(系统公告),
                    typeof(机器人),
                    typeof(充值奖励),
                    typeof(全服公告),
                    typeof(铭文洗炼技能),
                    typeof(装备神佑消耗)
                };
                    break;
                case 1:
                    模板列表 = new List<Type>
                {
                    typeof(游戏地图),
                    typeof(地形数据),
                    typeof(地图区域),
                    typeof(传送法阵)
                };
                    break;
                case 2:
                    模板列表 = new List<Type> { typeof(游戏称号) };
                    break;
                case 3:
                    模板列表 = new List<Type>
                {
                    typeof(铭文技能),
                    typeof(游戏技能),
                    typeof(技能陷阱),
                    typeof(游戏Buff)
                };
                    break;
                case 4:
                    模板列表 = new List<Type>
                {
                    typeof(游戏怪物),
                    typeof(地图道具),
                    typeof(地图守卫),
                    typeof(对话数据),
                    typeof(怪物刷新),
                    typeof(道具刷新),
                    typeof(守卫刷新)
                };
                    break;
                case 5:
                    模板列表 = new List<Type>
                {
                    typeof(游戏物品),
                    typeof(随机属性),
                    typeof(装备属性),
                    typeof(游戏套装)
                };
                    break;
                case 6:
                    模板列表 = new List<Type> { typeof(珍宝商品) };
                    break;
                case 7:
                    模板列表 = new List<Type>
                {
                    typeof(GameAchievements),
                    typeof(龙卫模板)
                };
                    break;
                case 8:
                    模板列表 = new List<Type> { typeof(守卫刷新) };
                    break;
                case 9:
                    模板列表 = new List<Type> { typeof(系统公告) };
                    break;
                case 10:
                    模板列表 = new List<Type> { typeof(机器人) };
                    break;
                case 11:
                    模板列表 = new List<Type> { typeof(充值奖励) };
                    break;
                case 12:
                    模板列表 = new List<Type>
                {
                    typeof(地图道具),
                    typeof(道具刷新)
                };
                    break;
                case 13:
                    模板列表 = new List<Type> { typeof(装备神佑消耗) };
                    break;
                case 14:
                    模板列表 = new List<Type> { typeof(游戏怪物) };
                    break;
                case 15:
                    模板列表 = new List<Type> { typeof(全服公告) };
                    break;
                case 16:
                    模板列表 = new List<Type> { typeof(怪物刷新) };
                    break;
                default:
                    模板列表 = new List<Type>
                {
                    typeof(游戏怪物),
                    typeof(地图守卫),
                    typeof(对话数据),
                    typeof(游戏地图),
                    typeof(地形数据),
                    typeof(地图区域),
                    typeof(传送法阵),
                    typeof(怪物刷新),
                    typeof(守卫刷新),
                    typeof(游戏物品),
                    typeof(随机属性),
                    typeof(装备属性),
                    typeof(游戏商店),
                    typeof(珍宝商品),
                    typeof(游戏称号),
                    typeof(铭文技能),
                    typeof(游戏技能),
                    typeof(技能陷阱),
                    typeof(游戏Buff),
                    typeof(龙卫模板),
                    typeof(地图道具),
                    typeof(道具刷新),
                    typeof(GameQuests),
                    typeof(游戏坐骑),
                    typeof(游戏天赋),
                    typeof(装备精炼),
                    typeof(传奇之力),
                    typeof(高级狩猎),
                    typeof(GameAchievements),
                    typeof(游戏威望),
                    typeof(传永七天),
                    typeof(游戏套装),
                    typeof(战功奖励),
                    typeof(战功任务),
                    typeof(杀怪成就),
                    typeof(主题礼包固定物品模板),
                    typeof(主题礼包商品模板),
                    typeof(锻石炼药),
                    typeof(物品过滤),
                    typeof(首杀怪物名单),
                    typeof(装备升级),
                    typeof(精炼阶段),
                    typeof(掉落分组),
                    typeof(物品分解),
                    typeof(合成公式),
                    typeof(合成系统),
                    typeof(月卡奖励),
                    typeof(武器升级),
                    typeof(每周特惠),
                    typeof(每日签到),
                    typeof(灵石配置),
                    typeof(装备合成),
                    typeof(装备重铸),
                    typeof(系统公告),
                    typeof(机器人),
                    typeof(充值奖励),
                    typeof(全服公告),
                    typeof(铭文洗炼技能),
                    typeof(装备神佑消耗)
                };
                    break;
            }
            Task.Run(delegate
            {
                Stopwatch 总计时 = Stopwatch.StartNew();
                foreach (Type item in 模板列表)
                {
                    Stopwatch 单计时 = Stopwatch.StartNew();
                    MethodInfo method;
                    method = item.GetMethod("载入数据", BindingFlags.Static | BindingFlags.Public);
                    try
                    {
                        if (method != null)
                        {
                            method.Invoke(null, null);
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show(item.Name + " 未能找到加载方法, 加载失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        主程.添加系统日志(item.Name + ".模板已经加载异常:: " + ex.Message + ((ex.InnerException != null) ? (".IE:" + ex.InnerException.Message) : "..."));
                        if (SMain.Main != null && SMain.Main.Loaded)
                        {
                            SMain.Main.允许重载();
                        }
                        continue;
                    }
                    单计时.Stop();
                    object obj;
                    obj = item.GetField("数据表", BindingFlags.Static | BindingFlags.Public)?.GetValue(null);
                    object obj3;
                    if (obj == null)
                    {
                        object obj2;
                        obj2 = null;
                    }
                    else
                    {
                        object obj2;
                        obj2 = obj.GetType().GetProperty("Count", BindingFlags.Instance | BindingFlags.Public);
                        if (obj2 != null)
                        {
                            obj3 = ((PropertyInfo)obj2).GetValue(obj);
                            goto IL_0112;
                        }
                    }
                    obj3 = null;
                    goto IL_0112;
                IL_0112:
                    int num;
                    num = ((obj3 != null) ? ((int)obj3) : 0);
                    if (num != 0)
                    {
                        主程.添加系统日志($"{item.Name}.模板已经加载,  数量: {num}  耗时: {单计时.ElapsedMilliseconds}ms");
                    }
                    else
                    {
                        主程.添加系统日志(item.Name + ".模板加载失败, 请注意检查数据目录");
                    }
                    if (SMain.Main != null && SMain.Main.Loaded)
                    {
                        SMain.Main.允许重载();
                    }
                }
                主程.添加系统日志($"[计时] 系统数据全部加载完成, 总耗时: {总计时.ElapsedMilliseconds}ms");
            }).Wait();
            if (单独加载 == 12)
            {
                地图处理网关.重载道具刷新();
            }
        }

        /*
        public static void LoadData()
        {
            var types = new Type[]
    {
                    typeof(游戏怪物),
                    typeof(地图守卫),
                    typeof(对话数据),
                    typeof(游戏地图),
                    typeof(地形数据),
                    typeof(地图区域),
                    typeof(传送法阵),
                    typeof(怪物刷新),
                    typeof(守卫刷新),
                    typeof(游戏物品),
                    typeof(随机属性),
                    typeof(装备属性),
                    typeof(游戏商店),
                    typeof(珍宝商品),
                    typeof(游戏称号),
                    typeof(铭文技能),
                    typeof(游戏技能),
                    typeof(技能陷阱),
                    typeof(游戏Buff),
                    typeof(龙卫模板),
                    typeof(地图道具),
                    typeof(道具刷新),
                    typeof(GameQuests),
                    typeof(游戏坐骑),
                    typeof(游戏天赋),
                    typeof(装备精炼),
                    typeof(传奇之力),
                    typeof(高级狩猎),
                    typeof(GameAchievements),
                    typeof(游戏威望),
                    typeof(传永七天),
                    typeof(游戏套装),
                    typeof(战功奖励),
                    typeof(战功任务),
                    typeof(杀怪成就),
                    typeof(主题礼包固定物品模板),
                    typeof(主题礼包商品模板),
                    typeof(锻石炼药),
                    typeof(物品过滤),
                    typeof(首杀怪物名单),
                    typeof(装备升级),
                    typeof(精炼阶段),
                    typeof(掉落分组),
                    typeof(物品分解),
                    typeof(合成公式),
                    typeof(合成系统),
                    typeof(月卡奖励),
                    typeof(武器升级),
                    typeof(每周特惠),
                    typeof(每日签到),
                    typeof(灵石配置),
                    typeof(装备合成),
                    typeof(装备重铸),
                    typeof(系统公告),
                    typeof(机器人),
                    typeof(充值奖励),
                    typeof(全服公告),
                    typeof(铭文洗炼技能),
                    typeof(装备神佑消耗)

    };
            Parallel.ForEach(types, (type) =>
            {
                LoadDataType(type);
            });
        }
        public static void LoadDataType(Type type)
        {
            MethodInfo method;
            method = type.GetMethod("载入数据", BindingFlags.Static | BindingFlags.Public);
            try
            {
                if (method != null)
                {
                    method.Invoke(null, null);
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(type.Name + " 未能找到加载方法, 加载失败");
                }
            }
            catch (Exception ex)
            {
                主程.添加系统日志(type.Name + ".模板已经加载异常:: " + ex.Message + ((ex.InnerException != null) ? (".IE:" + ex.InnerException.Message) : "..."));
                if (SMain.Main != null && SMain.Main.Loaded)
                {
                    SMain.Main.允许重载();
                }
                //continue;
            }
            object obj;
            obj = type.GetField("数据表", BindingFlags.Static | BindingFlags.Public)?.GetValue(null);
            object obj3;
            if (obj == null)
            {
                object obj2;
                obj2 = null;
            }
            else
            {
                object obj2;
                obj2 = obj.GetType().GetProperty("Count", BindingFlags.Instance | BindingFlags.Public);
                if (obj2 != null)
                {
                    obj3 = ((PropertyInfo)obj2).GetValue(obj);
                    goto IL_0112;
                }
            }
            obj3 = null;
            goto IL_0112;
        IL_0112:
            int num;
            num = ((obj3 != null) ? ((int)obj3) : 0);
            if (num != 0)
            {
                主程.添加系统日志($"{type.Name}.模板已经加载,  数量: {num}");
            }
            else
            {
                主程.添加系统日志(type.Name + ".模板加载失败, 请注意检查数据目录");
            }
            if (SMain.Main != null && SMain.Main.Loaded)
            {
                SMain.Main.允许重载();
            }
        }
        */

    }

}
