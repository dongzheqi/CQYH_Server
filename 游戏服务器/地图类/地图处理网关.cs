using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using 游戏服务器.模板类;
using 游戏服务器.数据类;
using 游戏服务器.网络类;

namespace 游戏服务器.地图类
{
    public static class 地图处理网关
    {
        public static int 对象表计数;

        public static List<地图对象> 次要对象表;

        public static List<地图对象> 对象备份表;

        public static Dictionary<int, 地图对象> 激活对象表;

        public static Dictionary<int, 地图对象> 地图对象表;

        // 仅此表用 ConcurrentDictionary: 后台「自动战斗」任务(AutoBattleManager, Task.Run 独立线程)每毫秒
        // 枚举 玩家对象表.Values.ToList() / ContainsKey, 与主循环登录登出时的增删并发 —— 普通 Dictionary 会抛
        // 「集合已修改」(被内挂线程 try/catch 吞进 内挂线程异常.txt, 致挂机卡顿). 其余对象表均主循环单线程访问, 保持 Dictionary 避免无谓加锁开销.
        public static ConcurrentDictionary<int, 玩家实例> 玩家对象表;

        public static Dictionary<int, 宠物实例> 宠物对象表;

        public static Dictionary<int, 怪物实例> 怪物对象表;

        public static Dictionary<int, 守卫实例> 守卫对象表;

        public static Dictionary<int, 物品实例> 物品对象表;

        public static Dictionary<int, 陷阱实例> 陷阱对象表;

        public static Dictionary<int, 道具实例> 道具对象表;

        public static Dictionary<int, 地图实例> 地图实例表;

        public static HashSet<地图实例> 副本实例表;

        private static ConcurrentQueue<地图实例> 副本移除表;

        private static ConcurrentQueue<地图对象> 添加激活表;

        private static ConcurrentQueue<地图对象> 移除激活表;

        public static int 对象编号;

        public static int 陷阱编号;

        public static int 物品编号;

        public static int 道具编号;

        public static int 之前的占领行会编号;

        private static DateTime 沙城处理计时;

        public static Point 沙城城门坐标;

        public static Point 皇宫下门坐标;

        public static Point 皇宫下门出口;

        public static Point 皇宫下门入口;

        public static Point 皇宫左门坐标;

        public static Point 皇宫左门出口;

        public static Point 皇宫左门入口;

        public static Point 皇宫上门坐标;

        public static Point 皇宫上门出口;

        public static Point 皇宫上门入口;

        public static Point 皇宫出口点一;

        public static Point 皇宫出口点二;

        public static Point 皇宫正门入口;

        public static Point 皇宫正门出口;

        public static Point 皇宫入口点左;

        public static Point 皇宫入口点中;

        public static Point 皇宫入口点右;

        public static Point 八卦坛坐标上;

        public static Point 八卦坛坐标下;

        public static Point 八卦坛坐标左;

        public static Point 八卦坛坐标右;

        public static Point 八卦坛坐标中;

        public static 地图实例 沙城地图;

        public static 怪物实例 沙城城门;

        public static 怪物实例 下方宫门;

        public static 怪物实例 上方宫门;

        public static 怪物实例 左方宫门;

        public static 守卫实例 上方法阵;

        public static 守卫实例 下方法阵;

        public static 守卫实例 左方法阵;

        public static 守卫实例 右方法阵;

        public static 守卫实例 八卦坛激活法阵;

        public static 行会数据 八卦坛激活行会;

        public static DateTime 八卦坛激活计时;

        public static 地图区域 皇宫随机区域;

        public static 地图区域 外城复活区域;

        public static 地图区域 内城复活区域;

        public static 地图区域 守方传送区域;

        public static byte 沙城节点;

        public static DateTime 通知时间;

        public static HashSet<行会数据> 攻城行会;

        public static DateTime 攻城战结束时间;

        public static DateTime 异常提示时间;

        private static Dictionary<行会数据, byte[]> 待下发行会描述;

        private static Queue<角色数据> 待更新行会角色表;

        public static void 沙巴克攻城战立即开始()
        {
            地图处理网关.攻城行会.Clear();
            foreach (KeyValuePair<int, 游戏数据> item2 in 游戏数据网关.行会数据表.数据表)
            {
                行会数据 item;
                item = item2.Value as 行会数据;
                地图处理网关.攻城行会.Add(item);
            }
            地图处理网关.攻城战结束时间 = 主程.当前时间.AddMinutes((double)Settings.攻城持续时间);
            地图处理网关.沙城节点 = 2;
            地图处理网关.沙城城门.移除Buff时处理(22300);
            地图处理网关.下方宫门.移除Buff时处理(22300);
            地图处理网关.上方宫门.移除Buff时处理(22300);
            地图处理网关.左方宫门.移除Buff时处理(22300);
            地图处理网关.沙巴克行会关系重置();
            网络服务网关.发送公告("沙巴克攻城战开始", 滚动播报: true);
        }

        public static void 沙巴克城门开启()
        {
            if (地图处理网关.沙城城门 != null)
            {
                地图处理网关.沙城城门.当前体力 = 0;
                地图处理网关.沙城城门.自身死亡处理(null, 技能击杀: false);
                地图处理网关.沙城城门?.删除对象();
                地图处理网关.沙城城门.出生地图 = null;
            }
        }

        private static void 沙城处理()
        {
            if (主程.当前时间 < 地图处理网关.沙城处理计时)
            {
                return;
            }
            地图处理网关.沙城处理计时 = 主程.当前时间.AddMilliseconds(50.0);
            if (地图处理网关.沙城地图 == null)
            {
                if (!地图处理网关.地图实例表.TryGetValue(2433, out 地图处理网关.沙城地图) || !游戏Buff.数据表.TryGetValue(22300, out var value) || !游戏怪物.数据表.TryGetValue("沙巴克城门3", out var value2) || !游戏怪物.数据表.TryGetValue("墙07", out var value3) || (地图处理网关.皇宫随机区域 = 地图处理网关.沙城地图.地图区域.FirstOrDefault((地图区域 O) => O.区域名字 == "沙巴克-皇宫随机区域")) == null || (地图处理网关.外城复活区域 = 地图处理网关.沙城地图.地图区域.FirstOrDefault((地图区域 O) => O.区域名字 == "沙巴克-外城复活区域")) == null || (地图处理网关.内城复活区域 = 地图处理网关.沙城地图.地图区域.FirstOrDefault((地图区域 O) => O.区域名字 == "沙巴克-内城复活区域")) == null || (地图处理网关.守方传送区域 = 地图处理网关.沙城地图.地图区域.FirstOrDefault((地图区域 O) => O.区域名字 == "沙巴克-守方传送区域")) == null)
                {
                    if (主程.当前时间 > 地图处理网关.异常提示时间)
                    {
                        主程.添加系统日志("沙城地图[2433]配置读取失败!!!!...");
                        主程.添加系统日志("沙城地图配置读取失败!!!!...");
                        主程.添加系统日志("沙城地图配置读取失败!!!!...");
                        主程.添加系统日志("沙城地图配置读取失败!!!!...");
                        地图处理网关.异常提示时间 = 主程.当前时间.AddSeconds(10.0);
                    }
                    地图处理网关.沙城地图 = null;
                    return;
                }
                地图处理网关.沙城城门 = new 怪物实例(value2, 地图处理网关.沙城地图, int.MaxValue, new Point[1] { 地图处理网关.沙城城门坐标 }, 禁止复活: true, 立即刷新: true)
                {
                    当前方向 = 游戏方向.右上,
                    存活时间 = DateTime.MaxValue
                };
                地图处理网关.上方宫门 = new 怪物实例(value3, 地图处理网关.沙城地图, int.MaxValue, new Point[1] { 地图处理网关.皇宫上门坐标 }, 禁止复活: true, 立即刷新: true)
                {
                    当前方向 = 游戏方向.右下,
                    存活时间 = DateTime.MaxValue
                };
                地图处理网关.下方宫门 = new 怪物实例(value3, 地图处理网关.沙城地图, int.MaxValue, new Point[1] { 地图处理网关.皇宫下门坐标 }, 禁止复活: true, 立即刷新: true)
                {
                    当前方向 = 游戏方向.右下,
                    存活时间 = DateTime.MaxValue
                };
                地图处理网关.左方宫门 = new 怪物实例(value3, 地图处理网关.沙城地图, int.MaxValue, new Point[1] { 地图处理网关.皇宫左门坐标 }, 禁止复活: true, 立即刷新: true)
                {
                    当前方向 = 游戏方向.左下,
                    存活时间 = DateTime.MaxValue
                };
                地图处理网关.沙城城门.添加Buff时处理(value.Buff编号, 地图处理网关.沙城城门);
                地图处理网关.上方宫门.添加Buff时处理(value.Buff编号, 地图处理网关.上方宫门);
                地图处理网关.下方宫门.添加Buff时处理(value.Buff编号, 地图处理网关.下方宫门);
                地图处理网关.左方宫门.添加Buff时处理(value.Buff编号, 地图处理网关.左方宫门);
            }
            foreach (地图对象 item in 地图处理网关.沙城地图[地图处理网关.皇宫下门坐标].ToList())
            {
                if (!item.对象死亡 && item is 玩家实例 玩家实例2 && 主程.当前时间 > 玩家实例2.忙碌时间)
                {
                    玩家实例2.玩家切换地图(地图处理网关.沙城地图, 地图区域类型.未知区域, 地图处理网关.皇宫下门入口);
                }
            }
            foreach (地图对象 item2 in 地图处理网关.沙城地图[地图处理网关.皇宫上门坐标].ToList())
            {
                if (!item2.对象死亡 && item2 is 玩家实例 玩家实例3 && 主程.当前时间 > 玩家实例3.忙碌时间)
                {
                    玩家实例3.玩家切换地图(地图处理网关.沙城地图, 地图区域类型.未知区域, 地图处理网关.皇宫上门入口);
                }
            }
            foreach (地图对象 item3 in 地图处理网关.沙城地图[地图处理网关.皇宫左门坐标].ToList())
            {
                if (!item3.对象死亡 && item3 is 玩家实例 玩家实例4 && 主程.当前时间 > 玩家实例4.忙碌时间)
                {
                    玩家实例4.玩家切换地图(地图处理网关.沙城地图, 地图区域类型.未知区域, 地图处理网关.皇宫左门入口);
                }
            }
            foreach (地图对象 item4 in 地图处理网关.沙城地图[地图处理网关.皇宫出口点一].ToList())
            {
                if (!item4.对象死亡 && item4 is 玩家实例 玩家实例5 && 主程.当前时间 > 玩家实例5.忙碌时间)
                {
                    玩家实例5.玩家切换地图(地图处理网关.沙城地图, 地图区域类型.未知区域, 地图处理网关.皇宫正门出口);
                }
            }
            foreach (地图对象 item5 in 地图处理网关.沙城地图[地图处理网关.皇宫出口点二].ToList())
            {
                if (!item5.对象死亡 && item5 is 玩家实例 玩家实例6 && 主程.当前时间 > 玩家实例6.忙碌时间)
                {
                    玩家实例6.玩家切换地图(地图处理网关.沙城地图, 地图区域类型.未知区域, 地图处理网关.皇宫正门出口);
                }
            }
            foreach (地图对象 item6 in 地图处理网关.沙城地图[地图处理网关.皇宫入口点左].ToList())
            {
                if (!item6.对象死亡 && item6 is 玩家实例 玩家实例7 && 主程.当前时间 > 玩家实例7.忙碌时间 && 玩家实例7.所属行会 != null && 玩家实例7.所属行会 == 系统数据.数据.占领行会.V)
                {
                    玩家实例7.玩家切换地图(地图处理网关.沙城地图, 地图区域类型.未知区域, 地图处理网关.皇宫正门入口);
                }
            }
            foreach (地图对象 item7 in 地图处理网关.沙城地图[地图处理网关.皇宫入口点中].ToList())
            {
                if (!item7.对象死亡 && item7 is 玩家实例 玩家实例8 && 主程.当前时间 > 玩家实例8.忙碌时间 && 玩家实例8.所属行会 != null && 玩家实例8.所属行会 == 系统数据.数据.占领行会.V)
                {
                    玩家实例8.玩家切换地图(地图处理网关.沙城地图, 地图区域类型.未知区域, 地图处理网关.皇宫正门入口);
                }
            }
            foreach (地图对象 item8 in 地图处理网关.沙城地图[地图处理网关.皇宫入口点右].ToList())
            {
                if (!item8.对象死亡 && item8 is 玩家实例 玩家实例9 && 主程.当前时间 > 玩家实例9.忙碌时间 && 玩家实例9.所属行会 != null && 玩家实例9.所属行会 == 系统数据.数据.占领行会.V)
                {
                    玩家实例9.玩家切换地图(地图处理网关.沙城地图, 地图区域类型.未知区域, 地图处理网关.皇宫正门入口);
                }
            }
            if (地图处理网关.沙城节点 == 0)
            {
                DateTime 今日开战 = 主程.当前时间.Date.AddHours(Settings.攻沙开始时间小时).AddMinutes(Settings.攻沙开始时间分钟);
                DateTime 今日预告 = 今日开战.AddMinutes(-10.0);
                if (主程.当前时间.Hour != 今日预告.Hour || 主程.当前时间.Minute != 今日预告.Minute)
                {
                    return;
                }
                foreach (KeyValuePair<DateTime, 行会数据> item9 in 系统数据.数据.申请行会.ToList())
                {
                    if (item9.Key.Date < 主程.当前时间.Date)
                    {
                        系统数据.数据.申请行会.Remove(item9.Key);
                    }
                }
                if (系统数据.数据.申请行会.Count == 0)
                {
                    return;
                }
                {
                    foreach (KeyValuePair<DateTime, 行会数据> item10 in 系统数据.数据.申请行会)
                    {
                        if (item10.Key.Date == 主程.当前时间.Date)
                        {
                            网络服务网关.发送公告("沙巴克攻城战将在十分钟后开始, 请做好准备", 滚动播报: true);
                            地图处理网关.沙城节点++;
                            break;
                        }
                    }
                    return;
                }
            }
            if (地图处理网关.沙城节点 == 1)
            {
                if (主程.当前时间.Hour != Settings.攻沙开始时间小时 || 主程.当前时间.Minute < Settings.攻沙开始时间分钟)
                {
                    return;
                }
                foreach (KeyValuePair<DateTime, 行会数据> item11 in 系统数据.数据.申请行会.ToList())
                {
                    if (item11.Key.Date == 主程.当前时间.Date)
                    {
                        地图处理网关.攻城行会.Add(item11.Value);
                        系统数据.数据.申请行会.Remove(item11.Key);
                    }
                }
                if (地图处理网关.攻城行会.Count == 0)
                {
                    地图处理网关.沙城节点 = 0;
                    return;
                }
                地图处理网关.沙城城门.移除Buff时处理(22300);
                地图处理网关.下方宫门.移除Buff时处理(22300);
                地图处理网关.上方宫门.移除Buff时处理(22300);
                地图处理网关.左方宫门.移除Buff时处理(22300);
                foreach (玩家实例 item12 in 地图处理网关.沙城地图.玩家列表)
                {
                    if (item12.所属行会 == null || item12.所属行会 != 系统数据.数据.占领行会.V)
                    {
                        item12.玩家切换地图(地图处理网关.沙城地图, 地图区域类型.未知区域, 地图处理网关.外城复活区域.随机坐标);
                    }
                }
                if (地图处理网关.地图实例表.TryGetValue(2849, out var value4))
                {
                    foreach (玩家实例 item13 in value4.玩家列表.ToList())
                    {
                        if (item13.所属行会 == null || item13.所属行会 != 系统数据.数据.占领行会.V)
                        {
                            item13.玩家切换地图(item13.复活地图, 地图区域类型.复活区域);
                        }
                    }
                }
                地图处理网关.沙巴克行会关系重置();
                网络服务网关.发送公告("沙巴克攻城战开始", 滚动播报: true);
                地图处理网关.沙城节点++;
            }
            else if (地图处理网关.沙城节点 == 2)
            {
                if (地图处理网关.沙城城门.对象死亡 && 地图处理网关.沙城城门.出生地图 != null)
                {
                    网络服务网关.发送公告("沙巴克城门已经被攻破", 滚动播报: true);
                    地图处理网关.沙城城门.出生地图 = null;
                }
                try
                {
                    if (地图处理网关.八卦坛激活行会 == null)
                    {
                        行会数据 行会数据;
                        行会数据 = null;
                        bool flag;
                        flag = true;
                        if (地图处理网关.沙城地图[地图处理网关.八卦坛坐标上].FirstOrDefault((地图对象 O) => !O.对象死亡 && O is 玩家实例) == null)
                        {
                            flag = false;
                        }
                        if (flag && 地图处理网关.沙城地图[地图处理网关.八卦坛坐标下].FirstOrDefault((地图对象 O) => !O.对象死亡 && O is 玩家实例) == null)
                        {
                            flag = false;
                        }
                        if (flag && 地图处理网关.沙城地图[地图处理网关.八卦坛坐标左].FirstOrDefault((地图对象 O) => !O.对象死亡 && O is 玩家实例) == null)
                        {
                            flag = false;
                        }
                        if (flag && 地图处理网关.沙城地图[地图处理网关.八卦坛坐标右].FirstOrDefault((地图对象 O) => !O.对象死亡 && O is 玩家实例) == null)
                        {
                            flag = false;
                        }
                        if (行会数据 == null && flag)
                        {
                            foreach (地图对象 item14 in 地图处理网关.沙城地图[地图处理网关.八卦坛坐标上])
                            {
                                if (!item14.对象死亡 && item14 is 玩家实例 玩家实例10)
                                {
                                    if (玩家实例10.所属行会 == null)
                                    {
                                        flag = false;
                                        break;
                                    }
                                    if (!地图处理网关.攻城行会.Contains(玩家实例10.所属行会))
                                    {
                                        flag = false;
                                        break;
                                    }
                                    if (行会数据 == null)
                                    {
                                        行会数据 = 玩家实例10.所属行会;
                                    }
                                    else if (行会数据 != 玩家实例10.所属行会)
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (行会数据 != null && flag)
                        {
                            foreach (地图对象 item15 in 地图处理网关.沙城地图[地图处理网关.八卦坛坐标下])
                            {
                                if (!item15.对象死亡 && item15 is 玩家实例 玩家实例11 && (玩家实例11.所属行会 == null || 行会数据 != 玩家实例11.所属行会))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                        if (行会数据 != null && flag)
                        {
                            foreach (地图对象 item16 in 地图处理网关.沙城地图[地图处理网关.八卦坛坐标左])
                            {
                                if (!item16.对象死亡 && item16 is 玩家实例 玩家实例12 && (玩家实例12.所属行会 == null || 行会数据 != 玩家实例12.所属行会))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                        if (行会数据 != null && flag)
                        {
                            foreach (地图对象 item17 in 地图处理网关.沙城地图[地图处理网关.八卦坛坐标右])
                            {
                                if (!item17.对象死亡 && item17 is 玩家实例 玩家实例13 && (玩家实例13.所属行会 == null || 行会数据 != 玩家实例13.所属行会))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                        if (行会数据 != null && flag && 地图处理网关.攻城行会.Contains(行会数据))
                        {
                            if (地图处理网关.八卦坛激活计时 == DateTime.MaxValue)
                            {
                                地图处理网关.八卦坛激活计时 = 主程.当前时间.AddSeconds(10.0);
                            }
                            else if (主程.当前时间 > 地图处理网关.八卦坛激活计时)
                            {
                                地图处理网关.八卦坛激活行会 = 行会数据;
                                地图处理网关.八卦坛激活法阵 = new 守卫实例(地图守卫.数据表[6123], 地图处理网关.沙城地图, 游戏方向.左方, 地图处理网关.八卦坛坐标中);
                                网络服务网关.发送公告($"沙巴克八卦坛传送点已经被行会[{行会数据}]成功激活", 滚动播报: true);
                            }
                        }
                        else
                        {
                            地图处理网关.八卦坛激活计时 = DateTime.MaxValue;
                        }
                    }
                }
                catch (Exception ex)
                {
                    主程.添加系统日志("沙城处理异常!!!!" + ex.Message);
                }
                bool flag2;
                flag2 = true;
                行会数据 行会数据2;
                行会数据2 = null;
                foreach (Point item18 in 地图处理网关.皇宫随机区域.范围坐标)
                {
                    foreach (地图对象 item19 in 地图处理网关.沙城地图[item18])
                    {
                        if (!item19.对象死亡 && item19 is 玩家实例 玩家实例14)
                        {
                            if (玩家实例14.所属行会 == null || !地图处理网关.攻城行会.Contains(玩家实例14.所属行会))
                            {
                                flag2 = false;
                                break;
                            }
                            if (行会数据2 == null)
                            {
                                行会数据2 = 玩家实例14.所属行会;
                            }
                            else if (行会数据2 != 玩家实例14.所属行会)
                            {
                                flag2 = false;
                                break;
                            }
                        }
                    }
                    if (!flag2)
                    {
                        break;
                    }
                }
                if (flag2 && 行会数据2 != null && (系统数据.数据.占领行会.V == null || 系统数据.数据.占领行会.V != 行会数据2))
                {
                    网络服务网关.发送封包(new 同步占领行会
                    {
                        现在行会 = 行会数据2.行会编号
                    });
                    系统数据.数据.占领行会.V = 行会数据2;
                    系统数据.数据.占领时间.V = 主程.当前时间;
                    foreach (KeyValuePair<角色数据, 行会职位> item20 in 行会数据2.行会成员)
                    {
                        item20.Key.攻沙日期.V = 主程.当前时间;
                    }
                    网络服务网关.发送公告($"[{行会数据2}]成为新的沙巴克行会", 滚动播报: true);
                }
                if (地图处理网关.攻城战结束时间 != default(DateTime))
                {
                    if (主程.当前时间 <= 地图处理网关.攻城战结束时间)
                    {
                        return;
                    }
                }
                else if (主程.当前时间.Hour != Settings.攻沙结束时间小时 || 主程.当前时间.Minute < Settings.攻沙结束时间分钟)
                {
                    return;
                }
                if (系统数据.数据.占领行会.V == null)
                {
                    网络服务网关.发送公告("沙巴克攻城战已经结束, 沙巴克仍然为无主之地", 滚动播报: true);
                }
                else
                {
                    foreach (KeyValuePair<角色数据, 行会职位> item21 in 系统数据.数据.占领行会.V.行会成员)
                    {
                        item21.Key.攻沙日期.V = 主程.当前时间;
                    }
                    网络服务网关.发送公告($"沙巴克攻城战已经结束, 恭喜[{系统数据.数据.占领行会.V.行会名字}]成为新的沙巴克行会", 滚动播报: true);
                }
                地图处理网关.八卦坛激活计时 = 主程.当前时间.AddMinutes(5.0);
                地图处理网关.沙城节点++;
            }
            else if (地图处理网关.沙城节点 == 3 && 主程.当前时间 > 地图处理网关.八卦坛激活计时)
            {
                地图处理网关.沙城城门?.删除对象();
                地图处理网关.上方宫门?.删除对象();
                地图处理网关.下方宫门?.删除对象();
                地图处理网关.左方宫门?.删除对象();
                地图处理网关.沙城城门 = new 怪物实例(地图处理网关.沙城城门.对象模板, 地图处理网关.沙城地图, int.MaxValue, new Point[1] { 地图处理网关.沙城城门坐标 }, 禁止复活: true, 立即刷新: true)
                {
                    当前方向 = 游戏方向.右上,
                    存活时间 = DateTime.MaxValue
                };
                地图处理网关.上方宫门 = new 怪物实例(地图处理网关.上方宫门.对象模板, 地图处理网关.沙城地图, int.MaxValue, new Point[1] { 地图处理网关.皇宫上门坐标 }, 禁止复活: true, 立即刷新: true)
                {
                    当前方向 = 游戏方向.右下,
                    存活时间 = DateTime.MaxValue
                };
                地图处理网关.下方宫门 = new 怪物实例(地图处理网关.下方宫门.对象模板, 地图处理网关.沙城地图, int.MaxValue, new Point[1] { 地图处理网关.皇宫下门坐标 }, 禁止复活: true, 立即刷新: true)
                {
                    当前方向 = 游戏方向.右下,
                    存活时间 = DateTime.MaxValue
                };
                地图处理网关.左方宫门 = new 怪物实例(地图处理网关.左方宫门.对象模板, 地图处理网关.沙城地图, int.MaxValue, new Point[1] { 地图处理网关.皇宫左门坐标 }, 禁止复活: true, 立即刷新: true)
                {
                    当前方向 = 游戏方向.左下,
                    存活时间 = DateTime.MaxValue
                };
                地图处理网关.沙城城门.添加Buff时处理(22300, 地图处理网关.沙城城门);
                地图处理网关.上方宫门.添加Buff时处理(22300, 地图处理网关.上方宫门);
                地图处理网关.下方宫门.添加Buff时处理(22300, 地图处理网关.下方宫门);
                地图处理网关.左方宫门.添加Buff时处理(22300, 地图处理网关.左方宫门);
                地图处理网关.八卦坛激活行会 = null;
                地图处理网关.八卦坛激活计时 = DateTime.MaxValue;
                地图处理网关.八卦坛激活法阵?.删除对象();
                地图处理网关.八卦坛激活法阵 = null;
                地图处理网关.攻城行会.Clear();
                地图处理网关.沙城节点 = 0;
                地图处理网关.攻城战结束时间 = default(DateTime);
            }
            if (地图处理网关.待更新行会角色表 != null && 地图处理网关.待更新行会角色表.Any() && 地图处理网关.待更新行会角色表.TryDequeue(out var result) && result != null && result.网络连接 != null)
            {
                if (地图处理网关.待下发行会描述 == null)
                {
                    地图处理网关.待下发行会描述 = new Dictionary<行会数据, byte[]>();
                }
                if (!地图处理网关.待下发行会描述.TryGetValue(result.所属行会.V, out var value5))
                {
                    value5 = result.所属行会.V.行会信息描述();
                    地图处理网关.待下发行会描述.Add(result.所属行会.V, value5);
                }
                result.网络连接?.发送封包(new 行会信息公告
                {
                    字节数据 = value5
                });
                if (!地图处理网关.待更新行会角色表.Any())
                {
                    地图处理网关.待更新行会角色表.Clear();
                    地图处理网关.待下发行会描述.Clear();
                }
            }
        }

        private static void 沙巴克行会关系重置()
        {
            地图处理网关.待更新行会角色表 = new Queue<角色数据>();
            DateTime dateTime;
            dateTime = 主程.当前时间.AddHours(2.0);
            foreach (行会数据 item in 地图处理网关.攻城行会)
            {
                foreach (行会数据 item2 in 地图处理网关.攻城行会)
                {
                    if (item == item2)
                    {
                        continue;
                    }
                    DateTime v2;
                    if (item.结盟行会.TryGetValue(item2, out var v))
                    {
                        if (v < dateTime)
                        {
                            item.结盟行会[item2] = dateTime;
                        }
                        if (item.敌对行会.ContainsKey(item2))
                        {
                            item.敌对行会.Remove(item2);
                        }
                    }
                    else if (!item.敌对行会.TryGetValue(item2, out v2))
                    {
                        item.敌对行会.Add(item2, dateTime);
                    }
                    else if (v2 < dateTime)
                    {
                        item.敌对行会[item2] = dateTime;
                    }
                }
                foreach (角色数据 key in item.行会成员.Keys)
                {
                    地图处理网关.待更新行会角色表.Enqueue(key);
                }
            }
        }

        public static void 处理数据()
        {
            foreach (KeyValuePair<int, 地图对象> item in 地图处理网关.激活对象表)
            {
                item.Value?.处理对象数据();
            }
            if (地图处理网关.对象表计数 >= 地图处理网关.次要对象表.Count)
            {
                地图处理网关.对象表计数 = 0;
                地图处理网关.次要对象表 = 地图处理网关.对象备份表;
                地图处理网关.对象备份表 = new List<地图对象>(2048);
            }
            for (int i = 0; i < 100; i++)
            {
                if (地图处理网关.对象表计数 >= 地图处理网关.次要对象表.Count)
                {
                    break;
                }
                if (地图处理网关.次要对象表[地图处理网关.对象表计数].次要对象)
                {
                    地图处理网关.次要对象表[地图处理网关.对象表计数].处理对象数据();
                    地图处理网关.对象备份表.Add(地图处理网关.次要对象表[地图处理网关.对象表计数]);
                }
                地图处理网关.对象表计数++;
            }
            while (!地图处理网关.移除激活表.IsEmpty)
            {
                if (地图处理网关.移除激活表.TryDequeue(out var result) && !result.激活对象)
                {
                    地图处理网关.激活对象表.Remove(result.地图编号);
                }
            }
            while (!地图处理网关.添加激活表.IsEmpty)
            {
                if (地图处理网关.添加激活表.TryDequeue(out var result2) && result2.激活对象 && !地图处理网关.激活对象表.ContainsKey(result2.地图编号))
                {
                    地图处理网关.激活对象表.Add(result2.地图编号, result2);
                }
            }
            if (主程.当前时间.Minute == 55 && 主程.当前时间.Hour != 地图处理网关.通知时间.Hour)
            {
                if (主程.当前时间.Hour + 1 == Settings.武斗场时间一 || 主程.当前时间.Hour + 1 == Settings.武斗场时间二)
                {
                    网络服务网关.发送公告("经验武斗场将在五分钟后开启, 想要参加的勇士请做好准备", 滚动播报: true);
                }
                地图处理网关.通知时间 = 主程.当前时间;
            }
            foreach (地图实例 item2 in 地图处理网关.副本实例表)
            {
                if (item2.副本关闭)
                {
                    地图处理网关.副本移除表.Enqueue(item2);
                }
                else
                {
                    item2.处理数据();
                }
            }
            while (!地图处理网关.副本移除表.IsEmpty)
            {
                if (地图处理网关.副本移除表.TryDequeue(out var result3))
                {
                    游戏脚本.地图销毁(result3);
                    地图处理网关.副本实例表.Remove(result3);
                }
            }
            地图处理网关.沙城处理();
        }

        public static void 开启地图()
        {
            地图处理网关.次要对象表 = new List<地图对象>();
            地图处理网关.对象备份表 = new List<地图对象>();
            地图处理网关.副本实例表 = new HashSet<地图实例>();
            地图处理网关.副本移除表 = new ConcurrentQueue<地图实例>();
            地图处理网关.添加激活表 = new ConcurrentQueue<地图对象>();
            地图处理网关.移除激活表 = new ConcurrentQueue<地图对象>();
            地图处理网关.激活对象表 = new Dictionary<int, 地图对象>();
            地图处理网关.地图对象表 = new Dictionary<int, 地图对象>();
            地图处理网关.地图实例表 = new Dictionary<int, 地图实例>();
            地图处理网关.玩家对象表 = new ConcurrentDictionary<int, 玩家实例>();
            地图处理网关.怪物对象表 = new Dictionary<int, 怪物实例>();
            地图处理网关.宠物对象表 = new Dictionary<int, 宠物实例>();
            地图处理网关.守卫对象表 = new Dictionary<int, 守卫实例>();
            地图处理网关.物品对象表 = new Dictionary<int, 物品实例>();
            地图处理网关.陷阱对象表 = new Dictionary<int, 陷阱实例>();
            地图处理网关.道具对象表 = new Dictionary<int, 道具实例>();
            foreach (游戏地图 value5 in 游戏地图.数据表.Values)
            {
                地图处理网关.地图实例表.Add(value5.地图编号 * 16 + 1, new 地图实例(value5, 16777217));
            }
            foreach (地形数据 value6 in 地形数据.数据表.Values)
            {
                int key;
                key = value6.地图编号 * 16 + 1;
                if (!地图处理网关.地图实例表.TryGetValue(key, out var value))
                {
                    continue;
                }
                value.地形数据 = value6;
                value.地图对象 = new HashSet<地图对象>[value.地图大小.X, value.地图大小.Y];
                for (int i = 0; i < value.地图大小.X; i++)
                {
                    for (int j = 0; j < value.地图大小.Y; j++)
                    {
                        value.地图对象[i, j] = new HashSet<地图对象>();
                    }
                }
            }
            foreach (地图区域 item in 地图区域.数据表)
            {
                foreach (地图实例 value7 in 地图处理网关.地图实例表.Values)
                {
                    if (value7.地图编号 == item.所处地图)
                    {
                        if (item.区域类型 == 地图区域类型.复活区域)
                        {
                            value7.复活区域 = item;
                        }
                        if (item.区域类型 == 地图区域类型.红名区域)
                        {
                            value7.红名区域 = item;
                        }
                        if (item.区域类型 == 地图区域类型.传送区域)
                        {
                            value7.传送区域 = item;
                        }
                        value7.地图区域.Add(item);
                        break;
                    }
                }
            }
            foreach (传送法阵 item2 in 传送法阵.数据表)
            {
                foreach (地图实例 value8 in 地图处理网关.地图实例表.Values)
                {
                    if (value8.地图编号 == item2.所处地图)
                    {
                        value8.法阵列表.Add(item2.法阵编号, item2);
                    }
                }
            }
            foreach (守卫刷新 item3 in 守卫刷新.数据表)
            {
                if (item3.禁止刷新)
                {
                    continue;
                }
                foreach (地图实例 value9 in 地图处理网关.地图实例表.Values)
                {
                    if (value9.地图编号 == item3.所处地图)
                    {
                        value9.守卫区域.Add(item3);
                    }
                }
            }
            foreach (道具刷新 item4 in 道具刷新.数据表)
            {
                foreach (地图实例 value10 in 地图处理网关.地图实例表.Values)
                {
                    if (value10.地图编号 == item4.地图编号)
                    {
                        value10.道具区域.Add(item4);
                    }
                }
            }
            foreach (怪物刷新 item5 in 怪物刷新.数据表)
            {
                foreach (地图实例 value11 in 地图处理网关.地图实例表.Values)
                {
                    if (value11.地图编号 == item5.所处地图)
                    {
                        value11.怪物区域.Add(item5);
                    }
                }
            }
            List<string> list;
            list = new List<string>();
            List<string> list2;
            list2 = new List<string>();
            foreach (地图实例 value12 in 地图处理网关.地图实例表.Values)
            {
                if (!value12.副本地图)
                {
                    value12.已初始化 = true;
                    foreach (怪物刷新 item6 in value12.怪物区域)
                    {
                        if (item6.刷新列表 == null)
                        {
                            continue;
                        }
                        Point[] 出生范围;
                        出生范围 = item6.范围坐标.ToArray();
                        刷新信息[] 刷新列表;
                        刷新列表 = item6.刷新列表;
                        foreach (刷新信息 刷新信息 in 刷新列表)
                        {
                            if (!游戏怪物.数据表.TryGetValue(刷新信息.怪物名字, out var value2))
                            {
                                continue;
                            }
                            主窗口.添加怪物数据(value2);
                            int 复活间隔;
                            复活间隔 = (刷新信息.按秒复活 ? (刷新信息.复活间隔 * 1000) : (刷新信息.复活间隔 * 60 * 1000));
                            for (int l = 0; l < 刷新信息.刷新数量; l++)
                            {
                                怪物实例 怪物实例2;
                                怪物实例2 = null;
                                怪物实例2 = ((刷新信息.定时刷新 == null || 刷新信息.定时刷新.Count <= 0) ? new 怪物实例(value2, value12, 复活间隔, 出生范围, 禁止复活: false, 立即刷新: true) : new 怪物实例(value2, value12, 刷新信息.定时刷新.ToArray(), 出生范围, 禁止复活: false, 立即刷新: false));
                                怪物实例2.刷新配置信息 = 刷新信息;
                                if (怪物实例2[游戏对象属性.最大体力] == 100 && list.IndexOf(怪物实例2.对象模板.怪物名字) == -1)
                                {
                                    list.Add(怪物实例2.对象模板.怪物名字);
                                }
                                if (怪物实例2.普通攻击技能 == null && 怪物实例2.概率触发技能.Count == 0 && !list2.Contains(怪物实例2.对象模板.怪物名字))
                                {
                                    list2.Add(怪物实例2.对象模板.怪物名字);
                                }
                            }
                        }
                    }
                    foreach (道具刷新 item7 in value12.道具区域)
                    {
                        if (地图道具.数据表.TryGetValue(item7.模板编号, out var value3))
                        {
                            Point[] 范围;
                            范围 = item7.范围坐标.ToArray();
                            for (int m = 0; m < item7.刷新数量; m++)
                            {
                                new 道具实例(value3, value12, item7.所处方向, 范围);
                            }
                        }
                    }
                    foreach (守卫刷新 item8 in value12.守卫区域)
                    {
                        if (!item8.禁止刷新 && 地图守卫.数据表.TryGetValue(item8.守卫编号, out var value4))
                        {
                            new 守卫实例(value4, value12, item8.所处方向, item8.所处坐标);
                        }
                    }
                }
                else
                {
                    value12.固定怪物总数 = (uint)value12.怪物区域.Sum((怪物刷新 O) => O.刷新列表.Sum((刷新信息 X) => X.刷新数量));
                }
                主窗口.添加地图数据(value12);
            }
            for (int n = 0; n < list.Count; n++)
            {
                主程.添加系统日志("[血量]怪物" + list[n] + "不正确");
            }
            for (int num = 0; num < list2.Count; num++)
            {
                主程.添加系统日志("[技能]怪物" + list2[num] + "不正确");
            }
        }

        public static void 重载地图(bool 完全更新 = false)
        {
            主程.重载任务列表.Enqueue(delegate
            {
                地图处理网关.清理传送法阵();
            });
            主程.重载任务列表.Enqueue(delegate
            {
                地图处理网关.加载传送法阵();
            });
            主程.重载任务列表.Enqueue(delegate
            {
                地图处理网关.清理守卫刷新();
            });
            主程.重载任务列表.Enqueue(delegate
            {
                地图处理网关.加载守卫刷新();
            });
            主程.重载任务列表.Enqueue(delegate
            {
                地图处理网关.清理道具刷新();
            });
            主程.重载任务列表.Enqueue(delegate
            {
                地图处理网关.加载道具刷新();
            });
            主程.重载任务列表.Enqueue(delegate
            {
                地图处理网关.清理怪物刷新(完全更新);
            });
            主程.重载任务列表.Enqueue(delegate
            {
                地图处理网关.加载怪物刷新(完全更新);
            });
        }

        public static void 重载道具刷新(bool 完全更新 = false)
        {
            主程.重载任务列表.Enqueue(delegate
            {
                地图处理网关.清理道具刷新();
            });
            主程.重载任务列表.Enqueue(delegate
            {
                地图处理网关.加载道具刷新();
            });
            主程.添加系统日志("重载道具刷新完成...");
        }

        private static void 加载传送法阵()
        {
            foreach (传送法阵 item in 传送法阵.数据表)
            {
                int key;
                key = item.所处地图 * 16 + 1;
                if (地图处理网关.地图实例表.ContainsKey(key))
                {
                    地图实例 地图实例2;
                    地图实例2 = 地图处理网关.地图实例表[key];
                    if (!地图实例2.副本地图)
                    {
                        地图实例2.法阵列表.Add(item.法阵编号, item);
                    }
                }
            }
        }

        private static void 清理传送法阵()
        {
            foreach (地图实例 value in 地图处理网关.地图实例表.Values)
            {
                value.法阵列表.Clear();
            }
        }

        private static void 加载守卫刷新()
        {
            foreach (守卫刷新 item in 守卫刷新.数据表)
            {
                if (item.禁止刷新)
                {
                    continue;
                }
                int key;
                key = item.所处地图 * 16 + 1;
                if (地图处理网关.地图实例表.ContainsKey(key))
                {
                    地图实例 地图实例2;
                    地图实例2 = 地图处理网关.地图实例表[key];
                    地图实例2.守卫区域.Add(item);
                    if (!地图实例2.副本地图 && 地图守卫.数据表.TryGetValue(item.守卫编号, out var value))
                    {
                        new 守卫实例(value, 地图实例2, item.所处方向, item.所处坐标);
                    }
                }
            }
        }

        private static void 清理守卫刷新()
        {
            守卫实例[] array;
            array = 地图处理网关.守卫对象表.Values.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].删除对象();
            }
            foreach (地图实例 value in 地图处理网关.地图实例表.Values)
            {
                value.守卫区域.Clear();
            }
        }

        private static void 加载怪物刷新(bool 完全更新)
        {
            List<地图实例> list;
            list = new List<地图实例>();
            foreach (怪物刷新 item in 怪物刷新.数据表)
            {
                int key;
                key = item.所处地图 * 16 + 1;
                if (地图处理网关.地图实例表.ContainsKey(key))
                {
                    地图实例 地图实例2;
                    地图实例2 = 地图处理网关.地图实例表[key];
                    地图实例2.怪物区域.Add(item);
                    if (!完全更新)
                    {
                        地图实例2.初始化();
                    }
                    if (地图实例2.玩家列表.Count > 0 && !list.Contains(地图实例2))
                    {
                        list.Add(地图实例2);
                    }
                }
            }
            if (!完全更新)
            {
                return;
            }
            foreach (地图实例 item2 in list)
            {
                item2.初始化();
            }
        }

        private static void 清理怪物刷新(bool 完全更新)
        {
            怪物实例[] array;
            array = 地图处理网关.怪物对象表.Values.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].删除对象();
            }
            foreach (地图实例 value in 地图处理网关.地图实例表.Values)
            {
                value.怪物区域.Clear();
                if (完全更新)
                {
                    value.已初始化 = false;
                }
            }
        }

        private static void 加载道具刷新()
        {
            foreach (道具刷新 item in 道具刷新.数据表)
            {
                int key;
                key = item.地图编号 * 16 + 1;
                if (!地图处理网关.地图实例表.ContainsKey(key))
                {
                    continue;
                }
                地图实例 地图实例2;
                地图实例2 = 地图处理网关.地图实例表[key];
                地图实例2.道具区域.Add(item);
                if (!地图实例2.副本地图 && 地图道具.数据表.TryGetValue(item.模板编号, out var value))
                {
                    Point[] 范围;
                    范围 = item.范围坐标.ToArray();
                    for (int i = 0; i < item.刷新数量; i++)
                    {
                        new 道具实例(value, 地图实例2, item.所处方向, 范围);
                    }
                }
            }
        }

        private static void 清理道具刷新()
        {
            道具实例[] array;
            array = 地图处理网关.道具对象表.Values.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].删除对象();
            }
            foreach (地图实例 value in 地图处理网关.地图实例表.Values)
            {
                value.道具区域.Clear();
            }
        }

        public static void 清理物品()
        {
            foreach (物品实例 value in 地图处理网关.物品对象表.Values)
            {
                value.物品数据?.删除数据();
            }
            foreach (KeyValuePair<int, 游戏商店> item in 游戏商店.数据表)
            {
                foreach (物品数据 item2 in item.Value.回购列表)
                {
                    item2.删除数据();
                }
            }
        }

        public static void 添加地图对象(地图对象 当前对象)
        {
            地图处理网关.地图对象表.Add(当前对象.地图编号, 当前对象);
            switch (当前对象.对象类型)
            {
                case 游戏对象类型.Npcc:
                    地图处理网关.守卫对象表.Add(当前对象.地图编号, (守卫实例)当前对象);
                    break;
                case 游戏对象类型.玩家:
                    地图处理网关.玩家对象表[当前对象.地图编号] = (玩家实例)当前对象;
                    break;
                case 游戏对象类型.宠物:
                    地图处理网关.宠物对象表.Add(当前对象.地图编号, (宠物实例)当前对象);
                    break;
                case 游戏对象类型.怪物:
                    地图处理网关.怪物对象表.Add(当前对象.地图编号, (怪物实例)当前对象);
                    break;
                case 游戏对象类型.Chest:
                    地图处理网关.道具对象表.Add(当前对象.地图编号, (道具实例)当前对象);
                    break;
                case 游戏对象类型.陷阱:
                    地图处理网关.陷阱对象表.Add(当前对象.地图编号, (陷阱实例)当前对象);
                    break;
                case 游戏对象类型.物品:
                    地图处理网关.物品对象表.Add(当前对象.地图编号, (物品实例)当前对象);
                    break;
            }
        }

        public static void 移除地图对象(地图对象 当前对象)
        {
            地图处理网关.地图对象表.Remove(当前对象.地图编号);
            switch (当前对象.对象类型)
            {
                case 游戏对象类型.Npcc:
                    地图处理网关.守卫对象表.Remove(当前对象.地图编号);
                    break;
                case 游戏对象类型.玩家:
                    地图处理网关.玩家对象表.TryRemove(当前对象.地图编号, out _);
                    break;
                case 游戏对象类型.宠物:
                    地图处理网关.宠物对象表.Remove(当前对象.地图编号);
                    break;
                case 游戏对象类型.怪物:
                    地图处理网关.怪物对象表.Remove(当前对象.地图编号);
                    break;
                case 游戏对象类型.陷阱:
                    地图处理网关.陷阱对象表.Remove(当前对象.地图编号);
                    break;
                case 游戏对象类型.物品:
                    地图处理网关.物品对象表.Remove(当前对象.地图编号);
                    break;
            }
        }

        public static void 添加激活对象(地图对象 当前对象)
        {
            地图处理网关.添加激活表.Enqueue(当前对象);
        }

        public static void 移除激活对象(地图对象 当前对象)
        {
            地图处理网关.移除激活表.Enqueue(当前对象);
        }

        public static void 添加次要对象(地图对象 当前对象)
        {
            地图处理网关.对象备份表.Add(当前对象);
        }

        public static 地图实例 已分配地图(int 地图编号)
        {
            if (地图处理网关.地图实例表.TryGetValue(地图编号 * 16 + 1, out var value))
            {
                return value;
            }
            return null;
        }

        static 地图处理网关()
        {
            地图处理网关.对象编号 = 268435456;
            地图处理网关.道具编号 = 805306368;
            地图处理网关.陷阱编号 = 1073741824;
            地图处理网关.物品编号 = 1342177280;
            地图处理网关.沙城城门坐标 = new Point(1020, 506);
            地图处理网关.皇宫下门坐标 = new Point(1079, 557);
            地图处理网关.皇宫下门出口 = new Point(1078, 556);
            地图处理网关.皇宫下门入口 = new Point(1265, 773);
            地图处理网关.皇宫左门坐标 = new Point(1082, 557);
            地图处理网关.皇宫左门出口 = new Point(1083, 556);
            地图处理网关.皇宫左门入口 = new Point(1266, 773);
            地图处理网关.皇宫上门坐标 = new Point(1071, 565);
            地图处理网关.皇宫上门出口 = new Point(1070, 564);
            地图处理网关.皇宫上门入口 = new Point(1254, 784);
            地图处理网关.皇宫出口点一 = new Point(1257, 777);
            地图处理网关.皇宫出口点二 = new Point(1258, 776);
            地图处理网关.皇宫正门入口 = new Point(1258, 777);
            地图处理网关.皇宫正门出口 = new Point(1074, 560);
            地图处理网关.皇宫入口点左 = new Point(1076, 560);
            地图处理网关.皇宫入口点中 = new Point(1075, 561);
            地图处理网关.皇宫入口点右 = new Point(1074, 562);
            地图处理网关.八卦坛坐标上 = new Point(1059, 591);
            地图处理网关.八卦坛坐标下 = new Point(1054, 586);
            地图处理网关.八卦坛坐标左 = new Point(1059, 586);
            地图处理网关.八卦坛坐标右 = new Point(1054, 591);
            地图处理网关.八卦坛坐标中 = new Point(1056, 588);
            地图处理网关.八卦坛激活计时 = DateTime.MaxValue;
            地图处理网关.攻城行会 = new HashSet<行会数据>();
        }

        public static void 处理在线玩家天数变更()
        {
            foreach (KeyValuePair<int, 玩家实例> item in 地图处理网关.玩家对象表)
            {
                if (item.Value != null)
                {
                    item.Value.CallDefaultNPC(DefaultNPCType.DayChange, true);
                }
            }
        }
    }
}
