using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using 游戏服务器.地图类;
using 游戏服务器.数据类;

namespace 游戏服务器.模板类
{
    /// <summary>
    /// 假人系统核心。维护一个常驻内存的"假人角色对象池"，按 CSV 每图配额把假人作为
    /// 无网络连接的 玩家实例 投放到世界里自动活动，营造在线人气。
    ///
    /// 设计要点(见实施计划 vast-toasting-toast.md):
    /// 1. 存盘隔离: 假人 角色数据 创建后立即 删除数据() 整株摘出持久表, 永不停留在 角色数据表,
    ///    从而不被 关服重扫/每秒落盘/整理重排 写进 Data.db。
    /// 2. 线程安全: 全部逻辑只在主循环单线程(假人网关.处理数据() 由 主程.循环 调用)执行;
    ///    假人 自动挂机 字段保持 null, 后台 AutoBattleManager 会自动跳过假人, 无并发。
    /// 3. 行为复用: 打怪直接调现成的 玩家实例.玩家自动战斗()(自带节流, 用安全版释放技能)。
    /// </summary>
    public static class 假人网关
    {
        public static 假人配置 配置;

        /// <summary>常驻内存的假人角色池(不在任何持久表中)。</summary>
        public static readonly List<角色数据> 假人池 = new List<角色数据>();

        /// <summary>当前已投放到世界的假人玩家实例。</summary>
        public static readonly List<玩家实例> 在线假人 = new List<玩家实例>();

        private static 账号数据 假人账号;

        private static DateTime 下次调度时间 = DateTime.MinValue;

        private const string 假人账号名 = "JiaRenPool9592";

        public static void 初始化()
        {
            try
            {
                配置 = 假人配置.载入数据();
                if (配置 == null || !配置.启用)
                {
                    主程.添加系统日志("[假人系统] 未启用(假人配置.csv 缺失或 启用!=1)");
                    return;
                }
                生成假人池();
            }
            catch (Exception ex)
            {
                主程.添加系统日志($"[假人系统] 初始化失败: {ex.Message}");
            }
        }

        private static void 生成假人池()
        {
            if (假人池.Count > 0)
            {
                return; // 已生成, 不重复造(改池规模需重启)
            }
            // 假人共用一个账号; new 后立即摘出账号表, 仅保留对象引用。
            假人账号 = new 账号数据(假人账号名);
            假人账号.删除数据();
            int 成功 = 0;
            for (int i = 0; i < 配置.假人总数; i++)
            {
                try
                {
                    角色数据 角色 = 创建假人角色(i);
                    假人池.Add(角色);
                    成功++;
                }
                catch (Exception ex)
                {
                    主程.添加系统日志($"[假人系统] 第{i}个假人创建失败: {ex.Message}");
                }
            }
            主程.添加系统日志($"[假人系统] 假人池已生成, 数量 {成功}/{配置.假人总数}");
        }

        // —— GM 管理入口(经 假人GM命令, 只能后台执行 → 主循环线程 drain, 不与服务循环竞态) ——

        /// <summary>重载 CSV(配额/开关/喊话内容即时生效; 未启用→启用会补建假人池, 改池规模仍需重启)。</summary>
        public static void 重载配置()
        {
            配置 = 假人配置.载入数据();
            if (配置.启用 && 假人池.Count == 0)
            {
                生成假人池();
            }
            主程.添加系统日志($"[假人系统] 配置已重载: 启用={配置.启用}, 池={假人池.Count}, 地图配额{配置.地图配额.Count}条");
        }

        public static void 报告状态()
        {
            主程.添加系统日志($"[假人系统] 启用={(配置 != null && 配置.启用)}, 池={假人池.Count}, 在线={在线假人.Count}");
            if (配置 == null)
            {
                return;
            }
            foreach (每图配额 q in 配置.地图配额)
            {
                int n = 在线假人.Count(p => p.当前地图 != null && p.当前地图.地图编号 == q.地图编号);
                主程.添加系统日志($"  地图{q.地图编号}: 在线{n}/{q.在线数量} 打怪={q.是否打怪} PK={q.是否PK}");
            }
        }

        /// <summary>给 GM 图形面板用的状态摘要字符串(应在主循环线程调用)。</summary>
        public static string 状态文本()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"启用={(配置 != null && 配置.启用)}  池={假人池.Count}  在线={在线假人.Count}");
            if (配置 != null)
            {
                foreach (每图配额 q in 配置.地图配额)
                {
                    int n = 在线假人.Count(p => p.当前地图 != null && p.当前地图.地图编号 == q.地图编号);
                    sb.AppendLine($"地图{q.地图编号}: 在线{n}/{q.在线数量} 打怪={q.是否打怪} PK={q.是否PK}");
                }
            }
            return sb.ToString();
        }

        public static void 全部下线()
        {
            foreach (玩家实例 假人 in 在线假人.ToList())
            {
                假人下线(假人);
            }
            主程.添加系统日志("[假人系统] 假人已全部下线");
        }

        /// <summary>跳过上下线节流, 立即按各图配额补满。</summary>
        public static void 立即填满()
        {
            if (配置 == null || !配置.启用)
            {
                return;
            }
            for (int 轮 = 0; 轮 <= 假人池.Count; 轮++)
            {
                bool 有变化 = false;
                foreach (每图配额 q in 配置.地图配额)
                {
                    int n = 在线假人.Count(p => p.当前地图 != null && p.当前地图.地图编号 == q.地图编号);
                    if (n < q.在线数量)
                    {
                        角色数据 c = 取空闲假人();
                        if (c != null)
                        {
                            假人上线(c, q);
                            有变化 = true;
                        }
                    }
                }
                if (!有变化)
                {
                    break;
                }
            }
            主程.添加系统日志($"[假人系统] 立即填充完成, 在线 {在线假人.Count}");
        }

        public static void 处理数据()
        {
            if (配置 == null || !配置.启用 || 假人池.Count == 0)
            {
                return;
            }
            // 1. 上下线调度(按 上下线间隔毫秒 节流, 每图每次最多调整一个, 平滑逼近配额)
            if (主程.当前时间 >= 下次调度时间)
            {
                下次调度时间 = 主程.当前时间.AddMilliseconds(配置.上下线间隔毫秒);
                try
                {
                    调度上下线();
                }
                catch (Exception ex)
                {
                    主程.添加系统日志($"[假人系统] 调度异常: {ex.Message}");
                }
            }
            // 2. 行为分发(每 tick, 各假人自带节流)
            foreach (玩家实例 假人 in 在线假人.ToList())
            {
                try
                {
                    驱动假人行为(假人);
                }
                catch (Exception ex)
                {
                    主程.添加系统日志($"[假人系统] 假人[{假人.对象名字}]行为异常: {ex.Message}");
                }
            }
        }

        private static void 调度上下线()
        {
            foreach (每图配额 配额 in 配置.地图配额)
            {
                int 现有 = 在线假人.Count(p => p.当前地图 != null && p.当前地图.地图编号 == 配额.地图编号);
                if (现有 < 配额.在线数量)
                {
                    角色数据 角色 = 取空闲假人();
                    if (角色 != null)
                    {
                        假人上线(角色, 配额);
                    }
                }
                else if (现有 > 配额.在线数量)
                {
                    玩家实例 假人 = 在线假人.FirstOrDefault(p => p.当前地图 != null && p.当前地图.地图编号 == 配额.地图编号);
                    if (假人 != null)
                    {
                        假人下线(假人);
                    }
                }
            }
        }

        private static 角色数据 取空闲假人()
        {
            HashSet<角色数据> 在线角色 = new HashSet<角色数据>(在线假人.Select(p => p.角色数据));
            return 假人池.FirstOrDefault(c => !在线角色.Contains(c));
        }

        private static 每图配额 查配额(int 地图编号)
        {
            return 配置.地图配额.FirstOrDefault(q => q.地图编号 == 地图编号);
        }

        private static void 假人上线(角色数据 角色, 每图配额 配额)
        {
            地图实例 图 = 地图处理网关.已分配地图(配额.地图编号);
            if (图 == null)
            {
                return;
            }
            玩家实例 假人 = new 玩家实例(角色, null);
            // 初始化自动战斗参数(供 玩家自动战斗() 使用)
            假人.自动战斗 = 配额.是否打怪;
            假人.释放技能 = 假人.获取最优技能();
            假人.自动拾取 = true;
            假人.拾取范围 = 12;
            假人.优先战斗 = true;
            if (配置.开启PK && 配额.是否PK)
            {
                假人.更改攻击模式(攻击模式.全体); // PK 图: 全体攻击, 对象关系对其他玩家互判敌对
            }
            // 落位到目标图并触发邻居更新(让周围真人看到)。复用 测试命令.cs 的成熟范式。
            Point 点 = 图.地图区域.First().随机坐标;
            假人.玩家切换地图(图, 地图区域类型.未知区域, 点);
            在线假人.Add(假人);
        }

        private static void 假人下线(玩家实例 假人)
        {
            产出清理(假人);
            假人.玩家角色下线();
            在线假人.Remove(假人);
        }

        /// <summary>
        /// 防御性产出清理: 把假人背包/装备里的物品对象摘出持久表, 杜绝落盘污染。
        /// 假人本就只捡金币(物品过滤为空 → 玩家自动战斗只收编号&lt;10 的货币, 走货币不入物品表)、
        /// 初始装备在 创建假人角色 时已随 角色.删除数据() 隔离, 故此处多为幂等;
        /// 作为第二道防线, 确保任何情况下假人产出都不进 Data.db。
        /// </summary>
        private static void 产出清理(玩家实例 假人)
        {
            foreach (物品数据 物品 in 假人.角色背包.Values.ToList())
            {
                物品.删除数据();
            }
            foreach (装备数据 装备 in 假人.角色装备.Values.ToList())
            {
                装备.删除数据();
            }
        }

        /// <summary>每 tick 驱动单个假人的行为。打怪图直接复用现成自动战斗, 其余图随机巡逻。</summary>
        private static void 驱动假人行为(玩家实例 假人)
        {
            if (假人.当前地图 == null)
            {
                return;
            }
            // 死亡 → 立即复活(假人"完全真实掉装"由引擎 自身死亡处理 负责, 这里只负责重新站起)
            if (假人.对象死亡)
            {
                假人.玩家请求复活();
                return;
            }
            // 保活: 血/蓝低于阈值补满(装饰性, 不消耗物品)
            假人.假人维持(配置.喝药血量百分比);
            // 喊话: 附近频道, 内容池随机 + 间隔抖动限流
            if (配置.开启喊话)
            {
                假人.假人尝试喊话(配置.喊话内容, 配置.喊话间隔秒);
            }
            // 行动: PK 图优先互殴(无目标则回退), 打怪图复用现成自动战斗, 其余图随机巡逻
            每图配额 配额 = 查配额(假人.当前地图.地图编号);
            if (配置.开启PK && 配额 != null && 配额.是否PK && 假人.假人尝试PK())
            {
                // 已执行 PK
            }
            else if (假人.自动战斗)
            {
                假人.玩家自动战斗();
            }
            else
            {
                巡逻(假人);
            }
        }

        /// <summary>随机走一步(复用 计算类 方向工具 + 玩家角色走动)。用 挂机间隔 字段节流。</summary>
        private static void 巡逻(玩家实例 假人)
        {
            if (主程.当前时间 < 假人.挂机间隔)
            {
                return;
            }
            假人.挂机间隔 = 主程.当前时间.AddMilliseconds(配置.巡逻间隔毫秒);
            游戏方向 方向 = 计算类.随机方向();
            Point 点 = 计算类.前方坐标(假人.当前坐标, 方向, 1);
            if (假人.当前地图.能否通行(点))
            {
                假人.玩家角色走动(点);
            }
        }

        private static 角色数据 创建假人角色(int 序号)
        {
            string 名字 = 生成名字(序号);
            // 地基阶段统一用战士女(机器人.cs 已验证可创角); 职业/外观随机化留待后续迭代。
            角色数据 角色 = new 角色数据(假人账号, 名字, 游戏对象职业.战士, 游戏对象性别.女性,
                对象发型分类.战士女00, 对象发色分类.发色00, 对象脸型分类.战士女00);
            int 等级 = 主程.随机数.Next(配置.最小等级, Math.Max(配置.最小等级, 配置.最大等级) + 1);
            角色.当前等级.V = (byte)Math.Clamp(等级, 1, 255);
            // 整株摘除出全部持久表(角色 + 初始背包/装备/技能), 仅保留内存引用。这是存盘隔离的核心。
            角色.删除数据();
            主窗口.移除角色数据(角色); // 清掉 加载完成() 时写入的 GM 列表残留行
            return 角色;
        }

        private static string 生成名字(int 序号)
        {
            string 名 = 配置.名字前缀 + (序号 + 1).ToString("D4");
            // 假人不进检索表, 但显示上避免与真实角色撞名(撞名还会让 角色数据 构造期的 检索表.Add 抛异常)
            while (游戏数据网关.角色数据表.检索表.ContainsKey(名))
            {
                名 += 主程.随机数.Next(10);
            }
            return 名;
        }
    }
}
