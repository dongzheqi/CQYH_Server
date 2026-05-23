using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using _001D_000F_0007_0013_0011_0015;
using 游戏服务器.副本类;
using 游戏服务器.管理命令;
using 游戏服务器.模板类;
using 游戏服务器.数据类;
using 游戏服务器.网络类;
using DevExpress.XtraBars;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace 游戏服务器.地图类
{
    public sealed partial class 玩家实例 : 地图对象
    {
        public void 自动挂机状态变更(暂停自动战斗 P)
        {
            if (this.自动挂机 != null)
            {
                this.自动挂机.自动战斗 = P.自动战斗 == 1;
            }
        }

        public void 玩家开始自动挂机(开始自动战斗 p)
        {
            if (!Settings.开启自动战斗)
            {
                this.发送顶部公告("服务器未开启自动挂机功能。");
            }
            else if (!p.自动战斗)
            {
                if (this.自动挂机 != null && this.自动挂机.自动战斗)
                {
                    this.重置挂机参数();
                    this.发送顶部公告((!this.自动挂机.自动战斗) ? "<#T:1000100>" : "<#T:1000101>");
                    this.自动挂机 = null;
                }
            }
            else if (this.本期特权 >= 3 && !(主程.当前时间 > this.特权时间))
            {
                if (this.当前地图.地图编号 == 152)
                {
                    this.发送顶部公告("沙巴克不能挂机");
                    return;
                }
                this.自动挂机 = p;
                this.重置挂机参数();
                this.发送顶部公告(this.自动挂机.自动战斗 ? "<#T:1000100>" : "<#T:1000101>");
            }
            else
            {
                this.发送顶部公告("只有玛法特权才可以使用");
            }
        }

        private void 重置挂机参数()
        {
            开始自动战斗 开始自动战斗;
            开始自动战斗 = this.自动挂机;
            if (开始自动战斗 != null && 开始自动战斗.自动战斗)
            {
                this.挂机_目标_黑名单.Clear();
                this.挂机_目标 = null;
                this.挂机_地图 = this.当前地图.地图编号;
                this.挂机_下一个坐标 = default(Point);
                this.挂机_状态 = 挂机状态.寻路;
                this.挂机_寻路队列 = new Queue<Point>();
                this.挂机_范围 = new Rectangle(this.当前坐标.X - this.自动挂机.战斗范围, this.当前坐标.Y - this.自动挂机.战斗范围, this.自动挂机.战斗范围 * 2, this.自动挂机.战斗范围 * 2);
                this.远攻技能数 = -1;
                this.获取远攻技能数();
                this.下次经验判断 = 主程.当前时间.AddSeconds(this.自动挂机.空闲时间);
                this.挂机已分解物品 = new HashSet<int>();
            }
            else
            {
                this.挂机_结束时间 = default(DateTime);
            }
        }

        private void 开始挂机()
        {
            if (this.自动挂机 == null || !this.自动挂机.自动战斗 || this.对象死亡 || this.摆摊状态 > 0 || this.交易状态 >= 3 || (this.无收益跳转中 && this.无收益跳转延时 != default(DateTime) && 主程.当前时间 < this.无收益跳转延时))
            {
                return;
            }
            if (this.无收益跳转中)
            {
                this.无收益跳转中 = false;
                this.无收益跳转延时 = default(DateTime);
                this.重置挂机参数();
            }
            if (主程.当前时间 < this.行走时间 || 主程.当前时间 < this.奔跑时间 || 主程.当前时间 < this.忙碌时间 || 主程.当前时间 < this.硬直时间 || this.检查状态(游戏对象状态.忙绿状态 | 游戏对象状态.残废状态 | 游戏对象状态.定身状态 | 游戏对象状态.麻痹状态 | 游戏对象状态.失神状态))
            {
                return;
            }
            if (this.自动挂机.开启空闲使用道具 && this.自动挂机.空闲时间 > 0 && this.自动挂机.道具ID > 0 && 主程.当前时间 > this.下次经验判断)
            {
                if (this.当前经验 == this.上次经验值 && this.查找背包物品(this.自动挂机.道具ID, out var 物品))
                {
                    if (物品.触发lua)
                    {
                        if (游戏脚本.玩家使用物品(this, 物品) == 0L)
                        {
                            this.ProcessConsumableItem(物品);
                        }
                    }
                    else if (!this.ProcessConsumableItem(物品))
                    {
                        this.CallDefaultNPC(DefaultNPCType.UseItem, true, 物品.物品编号);
                    }
                    this.无收益跳转中 = true;
                    this.无收益跳转延时 = 主程.当前时间.AddSeconds(2.0);
                }
                this.下次经验判断 = 主程.当前时间.AddSeconds(this.自动挂机.空闲时间);
                this.上次经验值 = this.当前经验;
            }
            if (主程.当前时间 > this.下次分解时间 && (!this.分解完成 || this.最大负重 - this.背包重量 < 5 || (this.自动挂机.开启预留背包 && this.背包剩余 <= this.自动挂机.预留格数) || this.背包剩余 < 5))
            {
                this.挂机自动分解();
                this.下次分解时间 = ((!this.分解完成) ? 主程.当前时间.AddSeconds(3.0) : ((this.背包剩余 == 0) ? 主程.当前时间.AddSeconds(30.0) : 主程.当前时间.AddSeconds(10.0)));
            }
            if (this.挂机_状态 == 挂机状态.寻路)
            {
                return;
            }
            if (this.挂机_状态 != 挂机状态.移动中 && (this.挂机_目标 == null || this.挂机_目标.对象死亡))
            {
                this.挂机_下一个坐标 = default(Point);
                this.挂机_状态 = 挂机状态.寻路;
                this.挂机_目标 = null;
                return;
            }
            if (this.挂机_目标 != null && 主程.当前时间 > this.挂机_目标_超时时间)
            {
                if (this.挂机_目标_黑名单.ContainsKey(this.挂机_目标))
                {
                    this.挂机_目标_黑名单[this.挂机_目标] = 主程.当前时间.AddSeconds(30.0);
                }
                else
                {
                    this.挂机_目标_黑名单.Add(this.挂机_目标, 主程.当前时间.AddSeconds(30.0));
                }
                this.挂机_目标 = null;
                this.挂机_状态 = 挂机状态.寻路;
                return;
            }
            地图对象 obj;
            obj = this.挂机_目标;
            if (obj != null && obj.对象类型 == 游戏对象类型.怪物)
            {
                switch (this.角色职业)
                {
                    case 游戏对象职业.法师:
                    case 游戏对象职业.弓手:
                    case 游戏对象职业.道士:
                        {
                            int num2;
                            num2 = 计算类.网格距离(this.挂机_目标.当前坐标, this.当前坐标);
                            if (num2 > 5)
                            {
                                if (this.挂机_状态 == 挂机状态.战斗中)
                                {
                                    this.挂机_下一个坐标 = default(Point);
                                    this.挂机_状态 = 挂机状态.寻路;
                                    return;
                                }
                            }
                            else if (this.获取远攻技能数() >= 1)
                            {
                                if (this.当前魔力 > 10 || num2 == 1)
                                {
                                    if (this.挂机_状态 == 挂机状态.移动中)
                                    {
                                        this.网络连接.发送封包(new 对象角色停止
                                        {
                                            对象编号 = this.地图编号,
                                            对象坐标 = ((this.挂机_下一个坐标 != default(Point)) ? this.挂机_下一个坐标 : this.当前坐标),
                                            对象高度 = this.当前高度
                                        });
                                    }
                                    this.挂机_状态 = 挂机状态.战斗中;
                                }
                            }
                            else
                            {
                                this.挂机_状态 = ((num2 > 1) ? 挂机状态.移动中 : 挂机状态.战斗中);
                            }
                            break;
                        }
                    case 游戏对象职业.战士:
                    case 游戏对象职业.刺客:
                    case 游戏对象职业.龙枪:
                        {
                            int num;
                            num = 计算类.网格距离(this.挂机_目标.当前坐标, this.当前坐标);
                            if (this.挂机_状态 == 挂机状态.战斗中)
                            {
                                if (num > 1)
                                {
                                    this.挂机_下一个坐标 = default(Point);
                                    this.挂机_状态 = 挂机状态.寻路;
                                    return;
                                }
                            }
                            else if (num == 0 || (this.挂机_状态 == 挂机状态.移动中 && num <= 1))
                            {
                                this.网络连接?.发送封包(new 对象角色停止
                                {
                                    对象编号 = this.地图编号,
                                    对象坐标 = ((this.挂机_下一个坐标 != default(Point)) ? this.挂机_下一个坐标 : this.当前坐标),
                                    对象高度 = this.当前高度
                                });
                                this.挂机_状态 = 挂机状态.战斗中;
                            }
                            break;
                        }
                }
            }
            else
            {
                地图对象 obj2;
                obj2 = this.挂机_目标;
                if (obj2 != null && obj2.对象类型 == 游戏对象类型.物品)
                {
                    if ((this.背包剩余 <= 0 || !base.邻居列表.Contains(this.挂机_目标) || this.背包重量 >= this[游戏对象属性.最大负重]) && !this.挂机_目标.IsMoney())
                    {
                        this.挂机_下一个坐标 = default(Point);
                        this.挂机_状态 = 挂机状态.寻路;
                        this.挂机_目标 = null;
                        return;
                    }
                    if (this.挂机_目标.当前坐标 == this.当前坐标)
                    {
                        if (!base.邻居列表.Contains(this.挂机_目标))
                        {
                            this.挂机_下一个坐标 = default(Point);
                            this.挂机_状态 = 挂机状态.寻路;
                            this.挂机_目标 = null;
                        }
                        return;
                    }
                    if (this.当前地图.空间阻塞(this.挂机_目标.当前坐标))
                    {
                        this.挂机_目标 = null;
                        this.挂机_状态 = 挂机状态.寻路;
                        return;
                    }
                    this.挂机_状态 = 挂机状态.移动中;
                }
            }
            switch (this.挂机_状态)
            {
                case 挂机状态.寻路:
                    if (this.挂机_寻路队列.Count > 0)
                    {
                        this.挂机_状态 = 挂机状态.移动中;
                    }
                    break;
                case 挂机状态.移动中:
                    {
                        if (this.挂机_下一个坐标 == default(Point))
                        {
                            if (this.挂机_寻路队列.Count == 0)
                            {
                                this.挂机_下一个坐标 = default(Point);
                                this.挂机_状态 = 挂机状态.寻路;
                                break;
                            }
                            this.挂机_下一个坐标 = this.挂机_寻路队列.Dequeue();
                        }
                        if (计算类.网格距离(this.挂机_下一个坐标, this.当前坐标) >= 4)
                        {
                            this.挂机_下一个坐标 = default(Point);
                            this.挂机_状态 = 挂机状态.寻路;
                            break;
                        }
                        if (!this.当前地图.能否通行(this.挂机_下一个坐标) && this.挂机_下一个坐标 != this.当前坐标)
                        {
                            this.挂机_下一个坐标 = default(Point);
                            this.挂机_状态 = 挂机状态.寻路;
                            break;
                        }
                        Point point;
                        point = ((this.挂机_寻路队列.Count > 0) ? this.挂机_寻路队列.Peek() : default(Point));
                        if (point != default(Point) && this.当前地图.能否通行(point))
                        {
                            int num3;
                            num3 = this.当前坐标.X - this.挂机_下一个坐标.X;
                            int num4;
                            num4 = this.当前坐标.Y - this.挂机_下一个坐标.Y;
                            int num5;
                            num5 = this.挂机_下一个坐标.X - point.X;
                            int num6;
                            num6 = this.挂机_下一个坐标.Y - point.Y;
                            if (num3 == num5 && num4 == num6)
                            {
                                if (this.挂机_寻路队列.Count > 0)
                                {
                                    this.挂机_寻路队列.Dequeue();
                                }
                                this.挂机_下一个坐标 = point;
                                this.玩家角色跑动(this.挂机_下一个坐标);
                                this.奔跑时间 = this.奔跑时间.AddMilliseconds(150.0);
                                this.挂机_下一个坐标 = default(Point);
                                break;
                            }
                        }
                        this.玩家角色走动(this.挂机_下一个坐标);
                        this.行走时间 = this.行走时间.AddMilliseconds(100.0);
                        this.挂机_下一个坐标 = default(Point);
                        break;
                    }
                case 挂机状态.战斗中:
                    if (!(主程.当前时间 < this.攻击间隔))
                    {
                        switch (this.角色职业)
                        {
                            case 游戏对象职业.战士:
                                this.战士挂机();
                                break;
                            case 游戏对象职业.法师:
                                this.法师挂机();
                                break;
                            case 游戏对象职业.刺客:
                                this.刺客挂机();
                                break;
                            case 游戏对象职业.弓手:
                                this.弓手挂机();
                                break;
                            case 游戏对象职业.道士:
                                this.道士挂机();
                                break;
                            case 游戏对象职业.龙枪:
                                this.龙枪挂机();
                                break;
                        }
                    }
                    break;
            }
        }

        private bool 开关技能(技能数据 技能)
        {
            if (技能.铭文模板.开关技能列表 != null)
            {
                foreach (string item in 技能.铭文模板.开关技能列表)
                {
                    if (!游戏技能.数据表.TryGetValue(item, out var value))
                    {
                        continue;
                    }
                    if (this.冷却记录.TryGetValue(value.自身技能编号 | 0x1000000, out var v) && 主程.当前时间 < v)
                    {
                        return false;
                    }
                    foreach (KeyValuePair<int, 技能任务> item2 in value.节点列表)
                    {
                        if (item2.Value is B_00_技能切换通知 b_00_技能切换通知 && this.冷却记录.TryGetValue(b_00_技能切换通知.技能标记编号 | 0x1000000, out v) && 主程.当前时间 < v)
                        {
                            return false;
                        }
                    }
                    if (this.主体技能表.TryGetValue(value.绑定等级编号, out var v2) && value.需要消耗魔法?.Length > v2.技能等级.V && this.当前魔力 < value.需要消耗魔法[v2.技能等级.V] && this is 玩家实例 { 无敌模式: false })
                    {
                        return false;
                    }
                    this.玩家开关技能(value.自身技能编号);
                    return true;
                }
            }
            return true;
        }

        public int 获取远攻技能数()
        {
            if (this.远攻技能数 != -1)
            {
                return this.远攻技能数;
            }
            switch (this.角色职业)
            {
                case 游戏对象职业.法师:
                    this.远攻技能数 = this.主体技能表.Keys.ToList().Count((ushort k) => k == 2559 || k == 2540 || k == 2537 || k == 2533 || k == 2531);
                    break;
                case 游戏对象职业.弓手:
                    this.远攻技能数 = this.主体技能表.Count - 1;
                    break;
                case 游戏对象职业.道士:
                    this.远攻技能数 = this.主体技能表.Keys.ToList().Count((ushort k) => k == 3005 || k == 3010);
                    break;
            }
            return this.远攻技能数;
        }

        private void 刺客挂机()
        {
            foreach (游戏基础技能 item in 玩家实例.技能使用顺序[游戏对象职业.刺客])
            {
                if (!this.主体技能表.TryGetValue((ushort)item, out var v))
                {
                    continue;
                }
                List<ushort> list;
                list = this.Buff列表.Keys.ToList();
                switch (item)
                {
                    case 游戏基础技能.献祭:
                        if (!list.Any((ushort b) => b == 15450 || b == 15452 || b == 15454) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.觉醒暗影守卫:
                        if (!list.Any((ushort b) => b == 15460 || b == 15461 || b == 15462 || b == 15463) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.觉醒猎命宣告:
                        if (!list.Any((ushort b) => b == 15430 || b == 15400 || b == 15442 || b == 15480) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.火镰狂舞:
                        if (list.Contains(15390) && this.玩家释放技能2(1932, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.刺客普攻:
                        if (计算类.网格距离(this.当前坐标, this.挂机_目标.当前坐标) <= 1 && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.暴击之术:
                        if (list.Contains(15310) && this.玩家释放技能2(1930, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.致残毒药:
                        if (!list.Any((ushort b) => b == 15330 || b == 15332 || b == 15334 || b == 15335 || b == 15338 || b == 15300) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.炎龙啸波:
                        if (list.Contains(15350) && this.玩家释放技能2(1931, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                }
            }
        }

        private void 战士挂机()
        {
            foreach (游戏基础技能 item in 玩家实例.技能使用顺序[游戏对象职业.战士])
            {
                if (!this.主体技能表.TryGetValue((ushort)item, out var v))
                {
                    continue;
                }
                List<ushort> list;
                list = this.Buff列表.Keys.ToList();
                switch (item)
                {
                    case 游戏基础技能.爆炎剑诀:
                        if (!list.Contains(10420))
                        {
                            if (this.开关技能(v))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                                return;
                            }
                        }
                        else if (this.被动技能.TryGetValue(1435, out v) && this.玩家释放技能2(1435, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.神威盾甲:
                        if (!list.Any((ushort b) => b == 10460 || b == 10461 || b == 10462 || b == 10463) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.觉醒金钟罩:
                        if (!list.Any((ushort b) => b == 10470 || b == 10471 || b == 10472 || b == 10473) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.觉醒雷霆剑法:
                        if (!list.Contains(10490))
                        {
                            if (this.开关技能(v))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                                return;
                            }
                        }
                        else if (this.被动技能.TryGetValue(1437, out v) && this.玩家释放技能2(1437, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.觉醒天威剑法:
                        if (!list.Contains(10500))
                        {
                            if (this.开关技能(v))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                                return;
                            }
                        }
                        else if (this.被动技能.TryGetValue(1450, out v) && this.玩家释放技能2(1450, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.战士普攻:
                        if (计算类.网格距离(this.当前坐标, this.挂机_目标.当前坐标) <= 1 && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.刺杀剑术:
                        if (!list.Contains(10330))
                        {
                            if (this.开关技能(v))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                                return;
                            }
                        }
                        else if (this.被动技能.TryGetValue(1431, out v) && this.玩家释放技能2(1431, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.半月弯刀:
                        if (base.邻居列表.Count((地图对象 o) => o.对象类型 == 游戏对象类型.怪物 && !o.对象死亡 && 计算类.网格距离(o.当前坐标, this.挂机_目标.当前坐标) < 3) < 2)
                        {
                            if (list.Contains(10340))
                            {
                                this.开关技能(v);
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                                return;
                            }
                        }
                        else if (list.Contains(10340))
                        {
                            if (this.玩家释放技能2(1432, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                                return;
                            }
                        }
                        else if (this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.烈火剑法:
                        if (!list.Contains(10360))
                        {
                            if (this.开关技能(v))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                                return;
                            }
                        }
                        else if (this.被动技能.TryGetValue(1433, out v) && this.玩家释放技能2(1433, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.逐日剑法:
                        if (!list.Contains(10380))
                        {
                            if (this.开关技能(v))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                                return;
                            }
                        }
                        else if (this.被动技能.TryGetValue(1434, out v) && this.玩家释放技能2(1434, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                }
            }
        }

        private void 龙枪挂机()
        {
            foreach (游戏基础技能 item in 玩家实例.技能使用顺序[游戏对象职业.龙枪])
            {
                if (!this.主体技能表.TryGetValue((ushort)item, out var v))
                {
                    continue;
                }
                List<ushort> list;
                list = this.Buff列表.Keys.ToList();
                switch (item)
                {
                    case 游戏基础技能.龙枪普攻:
                        if (计算类.网格距离(this.当前坐标, this.挂机_目标.当前坐标) <= 1 && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.横扫六合:
                        if ((list.Any((ushort b) => b == 12140 || b == 12141 || b == 12142 || b == 12143) && base.邻居列表.Count((地图对象 o) => o.对象类型 == 游戏对象类型.怪物 && !o.对象死亡 && 计算类.网格距离(o.当前坐标, this.挂机_目标.当前坐标) < 3) < 2) || !this.被动技能.TryGetValue(1600, out v) || !this.被动技能.TryGetValue(1601, out v))
                        {
                            break;
                        }
                        if (!list.Any((ushort b) => b == 12030 || b == 12031))
                        {
                            if (this.开关技能(v))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                                return;
                            }
                        }
                        else if (list.Contains(12031))
                        {
                            if (this.玩家释放技能2(1601, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                                return;
                            }
                        }
                        else if (this.玩家释放技能2(1600, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.狂飙突刺:
                        if (this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.神威镇域:
                        if (!list.Any((ushort b) => b == 12070 || b == 12071 || b == 12072 || b == 12073) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.枪出如龙:
                        if (!list.Any((ushort b) => b == 12080 || b == 12082 || b == 12150))
                        {
                            if (this.开关技能(v))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                                return;
                            }
                        }
                        else if (this.被动技能.TryGetValue(1602, out v) && this.玩家释放技能2(1602, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.御龙晶甲:
                        if (!list.Any((ushort b) => b == 12090 || b == 12091 || b == 12092 || b == 12093) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.凌云枪法:
                        if (!list.Any((ushort b) => b == 12100 || b == 12101))
                        {
                            if (this.开关技能(v))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                                return;
                            }
                        }
                        else if (list.Contains(12060) && this.被动技能.TryGetValue(1603, out v))
                        {
                            if (this.玩家释放技能2(1603, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                                return;
                            }
                        }
                        else if (list.Contains(12061) && !this.被动技能.TryGetValue(1604, out v) && this.玩家释放技能2(1604, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.盘龙枪势:
                        if (!list.Any((ushort b) => b == 12130 || b == 12131 || b == 12132 || b == 12133) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.百战军魂:
                        if (!list.Any((ushort b) => b == 12140 || b == 12141 || b == 12142 || b == 12143) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.龙啸千里:
                        if (this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                }
            }
        }

        private void 弓手挂机()
        {
            foreach (游戏基础技能 item in 玩家实例.技能使用顺序[游戏对象职业.弓手])
            {
                if (!this.主体技能表.TryGetValue((ushort)item, out var v))
                {
                    continue;
                }
                List<ushort> source;
                source = this.Buff列表.Keys.ToList();
                switch (item)
                {
                    case 游戏基础技能.弓手普攻:
                        if (计算类.网格距离(this.当前坐标, this.挂机_目标.当前坐标) <= 1 && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.基础射击:
                        if (this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.战术标记:
                        if (!this.挂机_目标.Buff列表.Keys.ToList().Any((ushort b) => b == 20440 || b == 20441 || b == 20442 || b == 20443) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.三发散射:
                        if (this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.强袭之箭:
                        if (this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.觉醒羿神庇佑:
                        if (!source.Any((ushort b) => b == 20490 || b == 20492 || b == 20494 || b == 20496) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.穿刺射击:
                        if (this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.守护箭羽:
                        if (!source.Any((ushort b) => b == 20520 || b == 20521 || b == 20522) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.回避射击:
                        if (this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.觉醒万箭穿心:
                        if (!source.Any((ushort b) => b == 20520 || b == 20521 || b == 20522) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                }
            }
        }

        private void 法师挂机()
        {
            foreach (游戏基础技能 item in 玩家实例.技能使用顺序[游戏对象职业.法师])
            {
                if (!this.主体技能表.TryGetValue((ushort)item, out var v))
                {
                    continue;
                }
                List<ushort> source;
                source = this.Buff列表.Keys.ToList();
                switch (item)
                {
                    case 游戏基础技能.觉醒魔能星陨:
                        {
                            地图对象 obj3;
                            obj3 = this.挂机_目标;
                            if ((obj3 == null || !(obj3.邻居列表?.Count((地图对象 o) => o.对象类型 == 游戏对象类型.怪物 && !o.对象死亡 && 计算类.网格距离(o.当前坐标, this.挂机_目标.当前坐标) < 5) < 2)) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                                return;
                            }
                            break;
                        }
                    case 游戏基础技能.觉醒法神奥义:
                        if (!source.Any((ushort b) => b == 25560 || b == 25564 || b == 25570 || b == 25577) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.法师普攻:
                        if (计算类.网格距离(this.当前坐标, this.挂机_目标.当前坐标) <= 1 && this.当前魔力 <= 30 && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.小火球术:
                        if (!this.主体技能表.ContainsKey(2533) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.雷电之术:
                        if (this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.魔法护盾:
                        if (!source.Any((ushort b) => b == 25350 || b == 25352 || b == 25354 || b == 25356) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.疾光电影:
                        {
                            if (this.主体技能表.TryGetValue(2536, out var v3) && v3.铭文编号 == 3)
                            {
                                地图对象 obj;
                                obj = this.挂机_目标;
                                if ((obj == null || !(obj.邻居列表?.Count((地图对象 o) => o.对象类型 == 游戏对象类型.怪物 && !o.对象死亡 && 计算类.网格距离(o.当前坐标, this.挂机_目标.当前坐标) < 5) < 2)) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                                {
                                    this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                                    return;
                                }
                            }
                            break;
                        }
                    case 游戏基础技能.冰咆哮:
                        {
                            地图对象 obj2;
                            obj2 = this.挂机_目标;
                            if ((obj2 == null || !(obj2.邻居列表?.Count((地图对象 o) => o.对象类型 == 游戏对象类型.怪物 && !o.对象死亡 && 计算类.网格距离(o.当前坐标, this.挂机_目标.当前坐标) < 5) < 2)) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                                return;
                            }
                            break;
                        }
                    case 游戏基础技能.流星火雨:
                        {
                            if (this.主体技能表.TryGetValue(2540, out var _) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                            {
                                this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                                return;
                            }
                            break;
                        }
                }
            }
        }

        private void 道士挂机()
        {
            foreach (游戏基础技能 item in 玩家实例.技能使用顺序[游戏对象职业.道士])
            {
                if (!this.主体技能表.TryGetValue((ushort)item, out var v))
                {
                    continue;
                }
                List<ushort> source;
                source = this.Buff列表.Keys.ToList();
                switch (item)
                {
                    case 游戏基础技能.觉醒阴阳道盾:
                        if (!source.Any((ushort b) => b == 30250 || b == 30252 || b == 30254 || b == 30256) && this.开关技能(v))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.道士普攻:
                        if (计算类.网格距离(this.当前坐标, this.挂机_目标.当前坐标) <= 1 && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.施毒术:
                        {
                            List<ushort> source2;
                            source2 = this.挂机_目标.Buff列表.Keys.ToList();
                            if (this.挂机_目标 == null || source2.Any((ushort b) => b == 30041 || b == 30045 || b == 30047 || b == 34002) || !this.玩家释放技能2(3004, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                            {
                                if (this.挂机_目标 != null && !source2.Any((ushort b) => b == 30040 || b == 30043 || b == 30048 || b == 34002) && this.玩家释放技能2(3400, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                                {
                                    this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                                    return;
                                }
                                break;
                            }
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                    case 游戏基础技能.灵魂火符:
                        if (this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间 + 200);
                            return;
                        }
                        break;
                    case 游戏基础技能.幽灵之盾:
                        if (!source.Any((ushort b) => b == 30060 || b == 30061 || b == 30062 || b == 30063 || b == 30064) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.神圣战甲:
                        if (!source.Any((ushort b) => b == 30070 || b == 30071 || b == 30072 || b == 30073) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.召唤骷髅:
                    case 游戏基础技能.召唤神兽:
                    case 游戏基础技能.觉醒召唤月灵:
                        if (this.宠物列表.Count < ((!this.主体技能表.ContainsKey(3022)) ? 1 : 2) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.隐身之术:
                        if (!source.Any((ushort b) => b == 30090 || b == 30091) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间);
                            return;
                        }
                        break;
                    case 游戏基础技能.噬血术:
                        if (this.主体技能表.ContainsKey(3010) && this.玩家释放技能2((ushort)item, base.动作编号++, this.挂机_目标.地图编号, this.挂机_目标.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.攻击间隔时间 + 200);
                            return;
                        }
                        break;
                    case 游戏基础技能.无极真气:
                        if (!source.Any((ushort b) => b == 30150 || b == 30152 || b == 30153 || b == 30154) && this.玩家释放技能2((ushort)item, base.动作编号++, this.地图编号, this.当前坐标))
                        {
                            this.攻击间隔 = 主程.当前时间.AddMilliseconds(this.开关间隔时间);
                            return;
                        }
                        break;
                }
            }
        }

        private void 挂机自动分解()
        {
            this.分解完成 = false;
            int num;
            num = 0;
            foreach (KeyValuePair<byte, 物品数据> item in this.角色背包.ToList())
            {
                byte key;
                key = item.Key;
                物品数据 value;
                value = item.Value;
                if (!value.是否上锁 && 物品分解.数据表.ContainsKey(value.物品编号) && (!(value is 装备数据 装备数据) || 装备数据.随机属性.Count <= 0))
                {
                    this.玩家分解物品(1, key, 1);
                    num++;
                    if (num > 5)
                    {
                        return;
                    }
                }
            }
            this.分解完成 = true;
        }
    }
}
