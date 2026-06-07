using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using 游戏服务器.模板类;
using 游戏服务器.网络类;

namespace 游戏服务器.地图类
{
    /// <summary>
    /// 假人系统专用的 玩家实例 扩展(partial)。假人是无网络连接的装饰对象,
    /// 这里只放它需要、而真人路径不便直接复用的薄封装。
    /// </summary>
    public partial class 玩家实例
    {
        private DateTime 假人下次维持;

        private DateTime 假人下次喊话;

        /// <summary>
        /// 假人保活: 血/蓝低于阈值时补满(每秒最多一次, 避免刷屏)。
        /// 假人不走真实喝药物品消耗链路 —— 那会 new 出需要清理的物品/污染物品表,
        /// 且假人喝药对真人玩法零影响, 故直接回复属性即可(死亡掉装的"真实"由引擎自身死亡处理保证, 与此无关)。
        /// </summary>
        internal void 假人维持(int 血量阈值百分比)
        {
            if (主程.当前时间 < this.假人下次维持)
            {
                return;
            }
            this.假人下次维持 = 主程.当前时间.AddSeconds(1.0);
            int 最大体力 = this[游戏对象属性.最大体力];
            if (最大体力 > 0 && this.当前体力 < (long)最大体力 * 血量阈值百分比 / 100L)
            {
                this.当前体力 = 最大体力;
            }
            int 最大魔力 = this[游戏对象属性.最大魔力];
            if (最大魔力 > 0 && this.当前魔力 < 最大魔力 / 2)
            {
                this.当前魔力 = 最大魔力;
            }
        }

        /// <summary>从内容池随机挑一句喊话(附近频道), 带间隔抖动限流, 避免整齐刷屏。</summary>
        internal void 假人尝试喊话(List<string> 内容池, int 间隔秒)
        {
            if (内容池 == null || 内容池.Count == 0 || 主程.当前时间 < this.假人下次喊话)
            {
                return;
            }
            int 基准 = Math.Max(1, 间隔秒);
            this.假人下次喊话 = 主程.当前时间.AddSeconds(基准 + 主程.随机数.Next(基准));
            this.假人喊话(内容池[主程.随机数.Next(内容池.Count)], 全服: false);
        }

        /// <summary>
        /// 让假人主动喊一句话。直接构造与 玩家发送广播 一致的"字节描述"并广播,
        /// 绕过扣费/等级/禁言校验(假人没有客户端, 不该走那套)。全服=广播频道, 否则附近频道。
        /// </summary>
        internal void 假人喊话(string 内容, bool 全服)
        {
            if (string.IsNullOrEmpty(内容))
            {
                return;
            }
            byte[] 内容字节;
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter w = new BinaryWriter(ms))
            {
                w.Write(Encoding.UTF8.GetBytes(内容));
                w.Write((byte)0);
                内容字节 = ms.ToArray();
            }
            byte[] 字节描述;
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter w = new BinaryWriter(ms))
            {
                if (全服)
                {
                    w.Write(this.地图编号);
                    w.Write(2415919107u);
                    w.Write(1);
                    w.Write((int)this.当前等级);
                    w.Write(内容字节);
                    w.Write(Encoding.UTF8.GetBytes(this.对象名字));
                    w.Write((byte)0);
                }
                else
                {
                    w.Write(2415919105u);
                    w.Write(this.地图编号);
                    w.Write(1);
                    w.Write((int)this.当前等级);
                    w.Write(内容字节);
                    w.Write(Encoding.UTF8.GetBytes(this.对象名字));
                    w.Write((byte)0);
                }
                字节描述 = ms.ToArray();
            }
            if (全服)
            {
                网络服务网关.发送封包(new 接收聊天消息 { 字节描述 = 字节描述 });
            }
            else
            {
                base.发送封包(new 接收聊天信息 { 字节描述 = 字节描述 });
            }
        }

        /// <summary>
        /// 假人 PK: 在 PK 图选最近的、敌对的"其他假人"追击并放技能。
        /// 只打假人(不主动骚扰真人); 假人攻击模式设为全体后 对象关系 判定互为敌对。
        /// 配合 假人维持 每秒补血, 假人间打不死(表演赛); 真人高爆发仍能击穿补血击杀 → 真实掉装。
        /// 返回 false 表示当前无 PK 目标, 交由调用方回退到打怪/巡逻。
        /// </summary>
        internal bool 假人尝试PK()
        {
            if (this.攻击目标 == null || this.攻击目标.对象死亡 || !base.邻居列表.Contains(this.攻击目标) || !(this.攻击目标 is 玩家实例))
            {
                this.攻击目标 = 选PK目标();
            }
            if (this.攻击目标 == null)
            {
                return false;
            }
            if (主程.当前时间 < this.忙碌时间 || 主程.当前时间 < this.硬直时间 || this.检查状态(游戏对象状态.麻痹状态 | 游戏对象状态.失神状态))
            {
                return true;
            }
            if (base.网格距离(this.攻击目标) <= this.获取职业技能距离())
            {
                this.当前方向 = 计算类.计算方向(this.当前坐标, this.攻击目标.当前坐标);
                this.玩家释放技能(this.获取职业特定技能((ushort)this.释放技能), ++base.动作编号, this.攻击目标.地图编号, this.攻击目标.当前坐标);
            }
            else if (this.能否跑动())
            {
                游戏方向 方向 = 计算类.计算方向(this.当前坐标, this.攻击目标.当前坐标);
                Point 点 = 计算类.前方坐标(this.当前坐标, 方向, 1);
                if (this.当前地图.能否通行(点))
                {
                    this.玩家角色跑动(点);
                }
            }
            return true;
        }

        private 玩家实例 选PK目标()
        {
            玩家实例 最近 = null;
            foreach (地图对象 邻居 in base.邻居列表)
            {
                if (邻居 is 玩家实例 目标 && 目标 != this && 目标.是否假人 && !目标.对象死亡
                    && this.对象关系(目标) == 游戏对象关系.敌对
                    && !this.当前地图.地形遮挡(this.当前坐标, 目标.当前坐标))
                {
                    if (最近 == null || base.网格距离(最近) > base.网格距离(目标))
                    {
                        最近 = 目标;
                    }
                }
            }
            return 最近;
        }
    }
}
