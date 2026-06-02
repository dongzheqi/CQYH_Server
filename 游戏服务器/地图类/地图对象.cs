using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using 游戏服务器.模板类;
using 游戏服务器.数据类;
using 游戏服务器.网络类;

namespace 游戏服务器.地图类
{
    public abstract class 地图对象
    {
        public bool 次要对象;

        public bool 激活对象;

        public HashSet<地图对象> 重要邻居;

        public HashSet<地图对象> 潜行邻居;

        public HashSet<地图对象> 邻居列表;

        public HashSet<技能实例> 技能任务;

        public HashSet<陷阱实例> 陷阱列表;

        public List<地图对象> 转移伤害对象列表;

        public List<int> 转移伤害基数列表;

        public List<float> 转移伤害系数列表;

        public List<bool> 转移伤害免减列表;

        public Dictionary<object, Dictionary<游戏对象属性, int>> 属性加成;

        public Dictionary<object, 转换属性> 属性转换;

        public List<KeyValuePair<string, string>> NPCVar = new List<KeyValuePair<string, string>>();

        public List<DelayedAction> ActionList = new List<DelayedAction>();

        public DateTime 恢复时间 { get; set; }

        public DateTime 治疗时间 { get; set; }

        public DateTime 脱战时间 { get; set; }

        public DateTime 处理计时 { get; set; }

        public DateTime 预约时间 { get; set; }

        public virtual int 处理间隔 { get; }

        public int 治疗次数 { get; set; }

        public int 治疗基数 { get; set; }

        public byte 动作编号 { get; set; }

        public bool 战斗姿态 { get; set; }

        public abstract 游戏对象类型 对象类型 { get; }

        public abstract 技能范围类型 对象体型 { get; }

        public ushort 行走速度 => (ushort)this[游戏对象属性.行走速度];

        public ushort 奔跑速度 => (ushort)this[游戏对象属性.奔跑速度];

        public virtual int 行走耗时 => this.行走速度 * 60;

        public virtual int 奔跑耗时 => this.奔跑速度 * 60;

        public virtual int 地图编号 { get; set; }

        public virtual int 当前体力 { get; set; }

        public virtual int 当前魔力 { get; set; }

        public virtual byte 当前等级 { get; set; }

        public virtual bool 对象死亡 { get; set; }

        public virtual bool 阻塞网格 { get; set; }

        public virtual bool 能被命中 => !this.对象死亡;

        public virtual string 对象名字 { get; set; }

        public virtual string 完整名字 { get; set; }

        public virtual 游戏方向 当前方向 { get; set; }

        public virtual 地图实例 当前地图 { get; set; }

        public virtual Point 当前坐标 { get; set; }

        public virtual ushort 当前高度 => this.当前地图.地形高度(this.当前坐标);

        public virtual DateTime 忙碌时间 { get; set; }

        public virtual DateTime 硬直时间 { get; set; }

        public virtual DateTime 行走时间 { get; set; }

        public virtual DateTime 奔跑时间 { get; set; }

        public virtual int this[游戏对象属性 属性]
        {
            get
            {
                if (!this.当前属性.ContainsKey(属性))
                {
                    return 0;
                }
                return this.当前属性[属性];
            }
            set
            {
                this.当前属性[属性] = value;
                switch (属性)
                {
                    case 游戏对象属性.最大魔力:
                        this.当前魔力 = Math.Min(this.当前魔力, value);
                        break;
                    case 游戏对象属性.最大体力:
                        this.当前体力 = Math.Min(this.当前体力, value);
                        break;
                }
            }
        }

        public virtual Dictionary<游戏对象属性, int> 当前属性 { get; }

        public virtual 字典监视器<int, DateTime> 冷却记录 { get; }

        public virtual 字典监视器<ushort, Buff数据> Buff列表 { get; }

        public abstract void Process(DelayedAction action);

        public override string ToString()
        {
            return this.对象名字;
        }

        public virtual void 更新对象属性()
        {
            int num;
            num = 0;
            int num2;
            num2 = 0;
            int num3;
            num3 = 0;
            int num4;
            num4 = 0;
            int val;
            val = this.当前体力;
            foreach (object value2 in Enum.GetValues(typeof(游戏对象属性)))
            {
                int num5;
                num5 = 0;
                游戏对象属性 游戏对象属性;
                游戏对象属性 = (游戏对象属性)value2;
                if (游戏对象属性 == 游戏对象属性.技能标志)
                {
                    continue;
                }
                foreach (KeyValuePair<object, Dictionary<游戏对象属性, int>> item in this.属性加成)
                {
                    if (item.Value == null || !item.Value.TryGetValue(游戏对象属性, out var value) || value == 0)
                    {
                        continue;
                    }
                    if (item.Key is Buff数据)
                    {
                        switch (游戏对象属性)
                        {
                            default:
                                num5 += value;
                                break;
                            case 游戏对象属性.奔跑速度:
                                num4 = Math.Max(num4, value);
                                num3 = Math.Min(num3, value);
                                break;
                            case 游戏对象属性.行走速度:
                                num2 = Math.Max(num2, value);
                                num = Math.Min(num, value);
                                break;
                        }
                    }
                    else
                    {
                        num5 += value;
                    }
                }
                switch (游戏对象属性)
                {
                    default:
                        this[游戏对象属性] = Math.Max(0, num5);
                        break;
                    case 游戏对象属性.幸运等级:
                        this[游戏对象属性] = num5;
                        break;
                    case 游戏对象属性.奔跑速度:
                        this[游戏对象属性] = Math.Max(1, num5 + num3 + num4);
                        break;
                    case 游戏对象属性.行走速度:
                        this[游戏对象属性] = Math.Max(1, num5 + num + num2);
                        break;
                }
            }
            foreach (游戏对象属性 value3 in Enum.GetValues(typeof(游戏对象属性)))
            {
                if (value3 > (游戏对象属性)200 && value3 < 游戏对象属性.装备免修 && this[value3] > 0)
                {
                    this[value3 - 200] += (int)((double)this[value3 - 200] * ((double)this[value3] / 10000.0));
                }
            }
            foreach (KeyValuePair<object, 转换属性> item2 in this.属性转换)
            {
                if (item2.Value.转换比率 > 0f)
                {
                    this[item2.Value.属性转换] += (int)((float)this[item2.Value.属性来源] * item2.Value.转换比率);
                }
            }
            if (this is 玩家实例 玩家实例2)
            {
                玩家实例2.计算玩家额外暴率();
                foreach (宠物实例 item3 in 玩家实例2.宠物列表)
                {
                    if (item3.对象模板.继承属性 != null)
                    {
                        Dictionary<游戏对象属性, int> dictionary;
                        dictionary = new Dictionary<游戏对象属性, int>();
                        属性继承[] 继承属性;
                        继承属性 = item3.对象模板.继承属性;
                        for (int i = 0; i < 继承属性.Length; i++)
                        {
                            属性继承 属性继承;
                            属性继承 = 继承属性[i];
                            dictionary[属性继承.转换属性] = (int)((float)this[属性继承.继承属性] * 属性继承.继承比例);
                        }
                        item3.属性加成[玩家实例2.角色数据] = dictionary;
                        item3.更新对象属性();
                    }
                }
            }
            this.当前体力 = Math.Min(this[游戏对象属性.最大体力], Math.Max(val, this.当前体力));
        }

        public virtual void 处理对象数据()
        {
            this.处理计时 = 主程.当前时间;
            this.预约时间 = 主程.当前时间.AddMilliseconds(this.处理间隔);
            for (int i = 0; i < this.ActionList.Count; i++)
            {
                if (!(主程.当前时间 < this.ActionList[i].Time))
                {
                    DelayedAction action;
                    action = this.ActionList[i];
                    this.ActionList.RemoveAt(i);
                    this.Process(action);
                }
            }
        }

        public virtual void 自身死亡处理(地图对象 攻击对象, bool 技能击杀, bool 脚本击杀 = false)
        {
            this.发送封包(new 对象角色死亡
            {
                对象编号 = this.地图编号
            });
            this.技能任务.Clear();
            this.对象死亡 = true;
            this.阻塞网格 = false;
            if (攻击对象 != null && 攻击对象.Buff列表 != null)
            {
                foreach (Buff数据 item in 攻击对象.Buff列表.Values.ToList())
                {
                    if (item == null || !item.可以触发)
                    {
                        continue;
                    }
                    if (item.Buff模板.击杀触发技能 != null && 游戏技能.数据表.TryGetValue(item.Buff模板.击杀触发技能, out var value) && this.特定类型(攻击对象, item.Buff模板.击杀触发限定))
                    {
                        if (item.Buff模板.成功移除自身)
                        {
                            攻击对象.移除Buff时处理(item.Buff编号.V);
                        }
                        new 技能实例(攻击对象, value, null, 0, this.当前地图, this.当前坐标, this, this.当前坐标, null);
                    }
                    if (item.Buff模板.击杀触发BUFF != 0 && this.特定类型(攻击对象, item.Buff模板.击杀触发限定))
                    {
                        if (item.Buff模板.成功移除自身)
                        {
                            攻击对象.移除Buff时处理(item.Buff编号.V);
                        }
                        攻击对象.添加Buff时处理(item.Buff模板.击杀触发BUFF, 攻击对象);
                    }
                    if (item.Buff模板.触发生效间隔 > 0)
                    {
                        item.触发时间 = 主程.当前时间.AddSeconds((int)item.Buff模板.触发生效间隔);
                    }
                }
            }
            foreach (地图对象 item2 in this.邻居列表.ToList())
            {
                item2.对象死亡时处理(this);
            }
        }

        public 地图对象()
        {
            this.处理计时 = 主程.当前时间;
            this.技能任务 = new HashSet<技能实例>();
            this.陷阱列表 = new HashSet<陷阱实例>();
            this.重要邻居 = new HashSet<地图对象>();
            this.潜行邻居 = new HashSet<地图对象>();
            this.邻居列表 = new HashSet<地图对象>();
            this.转移伤害对象列表 = new List<地图对象>();
            this.转移伤害基数列表 = new List<int>();
            this.转移伤害系数列表 = new List<float>();
            this.转移伤害免减列表 = new List<bool>();
            this.当前属性 = new Dictionary<游戏对象属性, int>();
            this.冷却记录 = new 字典监视器<int, DateTime>(null);
            this.Buff列表 = new 字典监视器<ushort, Buff数据>(null);
            this.属性加成 = new Dictionary<object, Dictionary<游戏对象属性, int>>();
            this.属性转换 = new Dictionary<object, 转换属性>();
            this.预约时间 = 主程.当前时间.AddMilliseconds(主程.随机数.Next(this.处理间隔));
        }

        public void 解绑网格()
        {
            Point[] array;
            array = 计算类.技能范围(this.当前坐标, this.当前方向, this.对象体型);
            foreach (Point 坐标 in array)
            {
                this.当前地图[坐标].Remove(this);
            }
        }

        public void 绑定网格()
        {
            Point[] array;
            array = 计算类.技能范围(this.当前坐标, this.当前方向, this.对象体型);
            foreach (Point 坐标 in array)
            {
                this.当前地图[坐标].Add(this);
            }
        }

        public void 删除对象(bool 清理列表 = true)
        {
            this.清空邻居时处理();
            this.解绑网格();
            this.次要对象 = false;
            地图处理网关.移除地图对象(this);
            this.激活对象 = false;
            地图处理网关.移除激活对象(this);
        }

        public int 网格距离(Point 坐标)
        {
            return 计算类.网格距离(this.当前坐标, 坐标);
        }

        public int 网格距离(地图对象 对象)
        {
            return 计算类.网格距离(this.当前坐标, 对象.当前坐标);
        }

        public void 发送封包(游戏封包 封包)
        {
            switch (封包.封包类型.Name)
            {
                case "对象角色走动":
                case "对象角色跑动":
                case "对象变换类型":
                case "同步对象行会":
                case "对象移除状态":
                case "对象添加状态":
                case "同步装配称号":
                case "同步宠物等级":
                case "对象角色死亡":
                case "触发命中特效":
                case "对象转动方向":
                case "触发技能正常":
                case "角色等级提升":
                case "摆摊状态改变":
                case "触发状态效果":
                case "触发技能扩展":
                case "开始释放技能":
                case "同步对象容貌":
                case "同步对象惩罚":
                case "玩家名字变灰":
                case "对象状态变动":
                case "变更摊位名字":
                case "同步角色外形":
                case "接收聊天信息":
                case "对象角色停止":
                case "对象被动位移":
                case "陷阱移动位置":
                case "同步对象体力":
                case "玩家骑乘上马":
                case "玩家骑乘下马":
                case "技能释放中断":
                    if (this is 玩家实例 { 隐身模式: not false })
                    {
                        break;
                    }
                    foreach (地图对象 item in this.邻居列表)
                    {
                        if (item is 玩家实例 玩家实例3 && !玩家实例3.潜行邻居.Contains(this))
                        {
                            玩家实例3?.网络连接?.发送封包(封包);
                        }
                    }
                    break;
            }
            if (this is 玩家实例 玩家实例4)
            {
                玩家实例4.网络连接?.发送封包(封包);
            }
        }

        public bool 在视线内(地图对象 对象)
        {
            if (Math.Abs(this.当前坐标.X - 对象.当前坐标.X) <= 20 && Math.Abs(this.当前坐标.Y - 对象.当前坐标.Y) <= 20)
            {
                return true;
            }
            return false;
        }

        public bool 主动攻击(地图对象 对象)
        {
            if (对象.对象死亡)
            {
                return false;
            }
            if (this is 怪物实例 怪物实例2)
            {
                if (怪物实例2.主动攻击目标 && (对象 is 玩家实例 || 对象 is 宠物实例 || 对象 is 守卫实例 { 能否受伤: not false }))
                {
                    return true;
                }
            }
            else if (this is 守卫实例 守卫实例3)
            {
                if (!守卫实例3.主动攻击目标)
                {
                    return false;
                }
                if (对象 is 怪物实例 怪物实例3)
                {
                    if (怪物实例3.主动攻击目标)
                    {
                        return string.Equals(守卫实例3.普攻技能.技能名字, "电脑-大刀守卫普通攻击");
                    }
                    return false;
                }
                if (对象 is 玩家实例 玩家实例2)
                {
                    return 玩家实例2.红名玩家;
                }
                if (对象 is 宠物实例 { 物品召唤: false })
                {
                    return string.Equals(守卫实例3.普攻技能.技能名字, "电脑-大刀守卫普通攻击");
                }
            }
            else if (this is 宠物实例)
            {
                if (对象 is 怪物实例 怪物实例4)
                {
                    return 怪物实例4.主动攻击目标;
                }
                return false;
            }
            return false;
        }

        public bool 邻居类型(地图对象 对象)
        {
            switch (this.对象类型)
            {
                case 游戏对象类型.Npcc:
                    {
                        游戏对象类型 游戏对象类型2;
                        游戏对象类型2 = 对象.对象类型;
                        if ((uint)(游戏对象类型2 - 1) > 1u && 游戏对象类型2 != 游戏对象类型.怪物 && 游戏对象类型2 != 游戏对象类型.陷阱)
                        {
                            return false;
                        }
                        return true;
                    }
                case 游戏对象类型.玩家:
                    return true;
                case 游戏对象类型.宠物:
                case 游戏对象类型.怪物:
                    switch (对象.对象类型)
                    {
                        default:
                            return false;
                        case 游戏对象类型.玩家:
                        case 游戏对象类型.宠物:
                        case 游戏对象类型.怪物:
                        case 游戏对象类型.Npcc:
                        case 游戏对象类型.陷阱:
                            return true;
                    }
                default:
                    return false;
                case 游戏对象类型.Chest:
                    if (对象.对象类型 != 游戏对象类型.玩家)
                    {
                        return false;
                    }
                    return true;
                case 游戏对象类型.陷阱:
                    {
                        游戏对象类型 游戏对象类型;
                        游戏对象类型 = 对象.对象类型;
                        if ((uint)(游戏对象类型 - 1) > 1u && 游戏对象类型 != 游戏对象类型.怪物 && 游戏对象类型 != 游戏对象类型.Npcc)
                        {
                            return false;
                        }
                        return true;
                    }
                case 游戏对象类型.物品:
                    if (对象.对象类型 == 游戏对象类型.玩家)
                    {
                        return true;
                    }
                    return false;
            }
        }

        public bool IsMoney()
        {
            if (this.对象类型 != 游戏对象类型.物品)
            {
                return false;
            }
            string text;
            text = this.对象名字;
            if (!(text == "金币") && !(text == "银币"))
            {
                return false;
            }
            return true;
        }

        public 游戏对象关系 对象关系(地图对象 对象)
        {
            if (对象 is 陷阱实例 陷阱实例2)
            {
                对象 = 陷阱实例2.陷阱来源;
            }
            if (this == 对象)
            {
                return 游戏对象关系.自身;
            }
            if (this is 怪物实例)
            {
                if (!(对象 is 怪物实例))
                {
                    return 游戏对象关系.敌对;
                }
                return 游戏对象关系.友方;
            }
            if (this is 守卫实例)
            {
                if (对象 is 怪物实例 || 对象 is 宠物实例 || 对象 is 玩家实例)
                {
                    return 游戏对象关系.敌对;
                }
            }
            else if (this is 玩家实例 玩家实例2)
            {
                if (对象 is 怪物实例)
                {
                    return 游戏对象关系.敌对;
                }
                if (对象 is 守卫实例)
                {
                    if (玩家实例2.攻击模式 == 攻击模式.全体 && this.当前地图.地图编号 != 80)
                    {
                        return 游戏对象关系.敌对;
                    }
                    return 游戏对象关系.友方;
                }
                if (对象 is 玩家实例 玩家实例3)
                {
                    if (玩家实例2.攻击模式 == 攻击模式.和平)
                    {
                        return 游戏对象关系.友方;
                    }
                    if (玩家实例2.攻击模式 == 攻击模式.行会)
                    {
                        if (玩家实例2.所属行会 != null && 玩家实例3.所属行会 != null && (玩家实例2.所属行会 == 玩家实例3.所属行会 || 玩家实例2.所属行会.结盟行会.ContainsKey(玩家实例3.所属行会)))
                        {
                            return 游戏对象关系.友方;
                        }
                        return 游戏对象关系.敌对;
                    }
                    if (玩家实例2.攻击模式 == 攻击模式.组队)
                    {
                        if (玩家实例2.所属队伍 != null && 玩家实例3.所属队伍 != null && 玩家实例2.所属队伍 == 玩家实例3.所属队伍)
                        {
                            return 游戏对象关系.友方;
                        }
                        return 游戏对象关系.敌对;
                    }
                    if (玩家实例2.攻击模式 == 攻击模式.全体)
                    {
                        return 游戏对象关系.敌对;
                    }
                    if (玩家实例2.攻击模式 == 攻击模式.善恶)
                    {
                        if (!玩家实例3.红名玩家 && !玩家实例3.灰名玩家)
                        {
                            return 游戏对象关系.友方;
                        }
                        return 游戏对象关系.敌对;
                    }
                    if (玩家实例2.攻击模式 == 攻击模式.敌对)
                    {
                        if (玩家实例2.所属行会 != null && 玩家实例3.所属行会 != null && 玩家实例2.所属行会.敌对行会.ContainsKey(玩家实例3.所属行会))
                        {
                            return 游戏对象关系.敌对;
                        }
                        return 游戏对象关系.友方;
                    }
                }
                else if (对象 is 宠物实例 宠物实例2)
                {
                    if (宠物实例2.宠物主人 == 玩家实例2)
                    {
                        if (玩家实例2.攻击模式 != 攻击模式.全体)
                        {
                            return 游戏对象关系.友方;
                        }
                        return 游戏对象关系.友方 | 游戏对象关系.敌对;
                    }
                    if (玩家实例2.攻击模式 == 攻击模式.和平)
                    {
                        return 游戏对象关系.友方;
                    }
                    if (玩家实例2.攻击模式 == 攻击模式.行会)
                    {
                        if (玩家实例2.所属行会 != null && 宠物实例2.宠物主人.所属行会 != null && (玩家实例2.所属行会 == 宠物实例2.宠物主人.所属行会 || 玩家实例2.所属行会.结盟行会.ContainsKey(宠物实例2.宠物主人.所属行会)))
                        {
                            return 游戏对象关系.友方;
                        }
                        return 游戏对象关系.敌对;
                    }
                    if (玩家实例2.攻击模式 == 攻击模式.组队)
                    {
                        if (玩家实例2.所属队伍 != null && 宠物实例2.宠物主人.所属队伍 != null && 玩家实例2.所属队伍 == 宠物实例2.宠物主人.所属队伍)
                        {
                            return 游戏对象关系.友方;
                        }
                        return 游戏对象关系.敌对;
                    }
                    if (玩家实例2.攻击模式 == 攻击模式.全体)
                    {
                        return 游戏对象关系.敌对;
                    }
                    if (玩家实例2.攻击模式 == 攻击模式.善恶)
                    {
                        if (!宠物实例2.宠物主人.红名玩家 && !宠物实例2.宠物主人.灰名玩家)
                        {
                            return 游戏对象关系.友方;
                        }
                        return 游戏对象关系.敌对;
                    }
                    if (玩家实例2.攻击模式 == 攻击模式.敌对)
                    {
                        if (玩家实例2.所属行会 != null && 宠物实例2.宠物主人.所属行会 != null && 玩家实例2.所属行会.敌对行会.ContainsKey(宠物实例2.宠物主人.所属行会))
                        {
                            return 游戏对象关系.敌对;
                        }
                        return 游戏对象关系.友方;
                    }
                }
            }
            else
            {
                if (this is 宠物实例 宠物实例3)
                {
                    if (对象 is 宠物实例 宠物实例4 && 宠物实例3.宠物主人 == 宠物实例4.宠物主人)
                    {
                        return 游戏对象关系.友方;
                    }
                    if (宠物实例3.宠物主人 != 对象)
                    {
                        return 宠物实例3.宠物主人.对象关系(对象);
                    }
                    return 游戏对象关系.友方;
                }
                if (this is 陷阱实例 陷阱实例3)
                {
                    return 陷阱实例3.陷阱来源.对象关系(对象);
                }
            }
            return 游戏对象关系.自身;
        }

        public bool 同一阵营(地图对象 对象)
        {
            if (this is 玩家实例 玩家实例2 && 对象 is 玩家实例 玩家实例3)
            {
                if (玩家实例2.所属行会 != null && 玩家实例3.所属行会 != null && (玩家实例2.所属行会 == 玩家实例3.所属行会 || 玩家实例2.所属行会.结盟行会.ContainsKey(玩家实例3.所属行会)))
                {
                    return true;
                }
                if (玩家实例2.所属队伍 != null && 玩家实例3.所属队伍 != null && 玩家实例2.所属队伍 == 玩家实例3.所属队伍)
                {
                    return true;
                }
            }
            return false;
        }

        public bool 特定类型(地图对象 来源, 指定目标类型 类型)
        {
            地图对象 地图对象2;
            地图对象2 = ((来源 is 陷阱实例 陷阱实例2) ? 陷阱实例2.陷阱来源 : 来源);
            if (this is 怪物实例 怪物实例2)
            {
                if (类型 == 指定目标类型.无)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.低级目标) == 指定目标类型.低级目标 && this.当前等级 < 地图对象2.当前等级)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.所有怪物) == 指定目标类型.所有怪物)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.低级怪物) == 指定目标类型.低级怪物 && this.当前等级 < 地图对象2.当前等级)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.低血怪物) == 指定目标类型.低血怪物 && (float)this.当前体力 / (float)this[游戏对象属性.最大体力] < 0.4f)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.普通怪物) == 指定目标类型.普通怪物 && 怪物实例2.怪物级别 == 怪物级别分类.普通怪物)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.不死生物) == 指定目标类型.不死生物 && 怪物实例2.怪物种族 == 怪物种族分类.不死生物)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.虫族生物) == 指定目标类型.虫族生物 && 怪物实例2.怪物种族 == 怪物种族分类.虫族生物)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.沃玛怪物) == 指定目标类型.沃玛怪物 && 怪物实例2.怪物种族 == 怪物种族分类.沃玛怪物)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.猪类怪物) == 指定目标类型.猪类怪物 && 怪物实例2.怪物种族 == 怪物种族分类.猪类怪物)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.祖玛怪物) == 指定目标类型.祖玛怪物 && 怪物实例2.怪物种族 == 怪物种族分类.祖玛怪物)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.魔龙怪物) == 指定目标类型.魔龙怪物 && 怪物实例2.怪物种族 == 怪物种族分类.魔龙怪物)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.精英怪物) == 指定目标类型.精英怪物 && (怪物实例2.怪物级别 == 怪物级别分类.精英干将 || 怪物实例2.怪物级别 == 怪物级别分类.头目首领))
                {
                    return true;
                }
                if ((类型 & 指定目标类型.背刺目标) == 指定目标类型.背刺目标)
                {
                    游戏方向 游戏方向;
                    游戏方向 = 计算类.计算方向(来源.当前坐标, this.当前坐标);
                    switch (this.当前方向)
                    {
                        case 游戏方向.上方:
                            if (游戏方向 == 游戏方向.左上 || 游戏方向 == 游戏方向.上方 || 游戏方向 == 游戏方向.右上)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.左上:
                            if (游戏方向 == 游戏方向.左方 || 游戏方向 == 游戏方向.左上 || 游戏方向 == 游戏方向.上方)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.左方:
                            if (游戏方向 == 游戏方向.左方 || 游戏方向 == 游戏方向.左上 || 游戏方向 == 游戏方向.左下)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.右方:
                            if (游戏方向 == 游戏方向.右上 || 游戏方向 == 游戏方向.右方 || 游戏方向 == 游戏方向.右下)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.右上:
                            if (游戏方向 == 游戏方向.上方 || 游戏方向 == 游戏方向.右上 || 游戏方向 == 游戏方向.右方)
                            {
                                return true;
                            }
                            break;
                        default:
                            if (游戏方向 == 游戏方向.右方 || 游戏方向 == 游戏方向.右下 || 游戏方向 == 游戏方向.下方)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.左下:
                            if (游戏方向 == 游戏方向.左方 || 游戏方向 == 游戏方向.下方 || 游戏方向 == 游戏方向.左下)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.下方:
                            if (游戏方向 == 游戏方向.右下 || 游戏方向 == 游戏方向.下方 || 游戏方向 == 游戏方向.左下)
                            {
                                return true;
                            }
                            break;
                    }
                }
            }
            else if (this is 守卫实例)
            {
                if (类型 == 指定目标类型.无)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.低级目标) == 指定目标类型.低级目标 && this.当前等级 < 地图对象2.当前等级)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.背刺目标) == 指定目标类型.背刺目标)
                {
                    游戏方向 游戏方向2;
                    游戏方向2 = 计算类.计算方向(来源.当前坐标, this.当前坐标);
                    switch (this.当前方向)
                    {
                        case 游戏方向.上方:
                            if (游戏方向2 == 游戏方向.左上 || 游戏方向2 == 游戏方向.上方 || 游戏方向2 == 游戏方向.右上)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.左上:
                            if (游戏方向2 == 游戏方向.左方 || 游戏方向2 == 游戏方向.左上 || 游戏方向2 == 游戏方向.上方)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.左方:
                            if (游戏方向2 == 游戏方向.左方 || 游戏方向2 == 游戏方向.左上 || 游戏方向2 == 游戏方向.左下)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.右方:
                            if (游戏方向2 == 游戏方向.右上 || 游戏方向2 == 游戏方向.右方 || 游戏方向2 == 游戏方向.右下)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.右上:
                            if (游戏方向2 == 游戏方向.上方 || 游戏方向2 == 游戏方向.右上 || 游戏方向2 == 游戏方向.右方)
                            {
                                return true;
                            }
                            break;
                        default:
                            if (游戏方向2 == 游戏方向.右方 || 游戏方向2 == 游戏方向.右下 || 游戏方向2 == 游戏方向.下方)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.左下:
                            if (游戏方向2 == 游戏方向.左方 || 游戏方向2 == 游戏方向.下方 || 游戏方向2 == 游戏方向.左下)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.下方:
                            if (游戏方向2 == 游戏方向.右下 || 游戏方向2 == 游戏方向.下方 || 游戏方向2 == 游戏方向.左下)
                            {
                                return true;
                            }
                            break;
                    }
                }
            }
            else if (this is 宠物实例 宠物实例2)
            {
                if (类型 == 指定目标类型.无)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.低级目标) == 指定目标类型.低级目标 && this.当前等级 < 地图对象2.当前等级)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.不死生物) == 指定目标类型.不死生物 && 宠物实例2.宠物种族 == 怪物种族分类.不死生物)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.虫族生物) == 指定目标类型.虫族生物 && 宠物实例2.宠物种族 == 怪物种族分类.虫族生物)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.所有宠物) == 指定目标类型.所有宠物)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.背刺目标) == 指定目标类型.背刺目标)
                {
                    游戏方向 游戏方向3;
                    游戏方向3 = 计算类.计算方向(来源.当前坐标, this.当前坐标);
                    switch (this.当前方向)
                    {
                        case 游戏方向.上方:
                            if (游戏方向3 == 游戏方向.左上 || 游戏方向3 == 游戏方向.上方 || 游戏方向3 == 游戏方向.右上)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.左上:
                            if (游戏方向3 == 游戏方向.左方 || 游戏方向3 == 游戏方向.左上 || 游戏方向3 == 游戏方向.上方)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.左方:
                            if (游戏方向3 == 游戏方向.左方 || 游戏方向3 == 游戏方向.左上 || 游戏方向3 == 游戏方向.左下)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.右方:
                            if (游戏方向3 == 游戏方向.右上 || 游戏方向3 == 游戏方向.右方 || 游戏方向3 == 游戏方向.右下)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.右上:
                            if (游戏方向3 == 游戏方向.上方 || 游戏方向3 == 游戏方向.右上 || 游戏方向3 == 游戏方向.右方)
                            {
                                return true;
                            }
                            break;
                        default:
                            if (游戏方向3 == 游戏方向.右方 || 游戏方向3 == 游戏方向.右下 || 游戏方向3 == 游戏方向.下方)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.左下:
                            if (游戏方向3 == 游戏方向.左方 || 游戏方向3 == 游戏方向.下方 || 游戏方向3 == 游戏方向.左下)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.下方:
                            if (游戏方向3 == 游戏方向.右下 || 游戏方向3 == 游戏方向.下方 || 游戏方向3 == 游戏方向.左下)
                            {
                                return true;
                            }
                            break;
                    }
                }
            }
            else if (this is 玩家实例 玩家实例2)
            {
                if (类型 == 指定目标类型.无)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.低级目标) == 指定目标类型.低级目标 && this.当前等级 < 地图对象2.当前等级)
                {
                    return true;
                }
                if ((类型 & 指定目标类型.带盾法师) == 指定目标类型.带盾法师 && 玩家实例2.角色职业 == 游戏对象职业.法师 && (玩家实例2.Buff列表.ContainsKey(25350) || 玩家实例2.Buff列表.ContainsKey(25351) || 玩家实例2.Buff列表.ContainsKey(25352) || 玩家实例2.Buff列表.ContainsKey(25354)))
                {
                    return true;
                }
                if ((类型 & 指定目标类型.物理护盾) == 指定目标类型.物理护盾 && (玩家实例2.Buff列表.ContainsKey(10470) || 玩家实例2.Buff列表.ContainsKey(10471) || 玩家实例2.Buff列表.ContainsKey(10472) || 玩家实例2.Buff列表.ContainsKey(10473) || 玩家实例2.Buff列表.ContainsKey(20520) || 玩家实例2.Buff列表.ContainsKey(20521) || 玩家实例2.Buff列表.ContainsKey(20522) || 玩家实例2.Buff列表.ContainsKey(12090) || 玩家实例2.Buff列表.ContainsKey(12091) || 玩家实例2.Buff列表.ContainsKey(12092) || 玩家实例2.Buff列表.ContainsKey(12093) || 玩家实例2.Buff列表.ContainsKey(12130) || 玩家实例2.Buff列表.ContainsKey(12131) || 玩家实例2.Buff列表.ContainsKey(12132) || 玩家实例2.Buff列表.ContainsKey(12133)))
                {
                    return true;
                }
                if ((类型 & 指定目标类型.魔法护盾) == 指定目标类型.魔法护盾 && (玩家实例2.Buff列表.ContainsKey(25350) || 玩家实例2.Buff列表.ContainsKey(25351) || 玩家实例2.Buff列表.ContainsKey(25352) || 玩家实例2.Buff列表.ContainsKey(25354) || 玩家实例2.Buff列表.ContainsKey(30250) || 玩家实例2.Buff列表.ContainsKey(30252) || 玩家实例2.Buff列表.ContainsKey(30254) || 玩家实例2.Buff列表.ContainsKey(30256) || 玩家实例2.Buff列表.ContainsKey(30270) || 玩家实例2.Buff列表.ContainsKey(30276) || 玩家实例2.Buff列表.ContainsKey(45904)))
                {
                    return true;
                }
                if ((类型 & 指定目标类型.背刺目标) == 指定目标类型.背刺目标)
                {
                    游戏方向 游戏方向4;
                    游戏方向4 = 计算类.计算方向(来源.当前坐标, this.当前坐标);
                    switch (this.当前方向)
                    {
                        case 游戏方向.上方:
                            if (游戏方向4 == 游戏方向.左上 || 游戏方向4 == 游戏方向.上方 || 游戏方向4 == 游戏方向.右上)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.左上:
                            if (游戏方向4 == 游戏方向.左方 || 游戏方向4 == 游戏方向.左上 || 游戏方向4 == 游戏方向.上方)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.左方:
                            if (游戏方向4 == 游戏方向.左方 || 游戏方向4 == 游戏方向.左上 || 游戏方向4 == 游戏方向.左下)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.右方:
                            if (游戏方向4 == 游戏方向.右上 || 游戏方向4 == 游戏方向.右方 || 游戏方向4 == 游戏方向.右下)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.右上:
                            if (游戏方向4 == 游戏方向.上方 || 游戏方向4 == 游戏方向.右上 || 游戏方向4 == 游戏方向.右方)
                            {
                                return true;
                            }
                            break;
                        default:
                            if (游戏方向4 == 游戏方向.右方 || 游戏方向4 == 游戏方向.右下 || 游戏方向4 == 游戏方向.下方)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.左下:
                            if (游戏方向4 == 游戏方向.左方 || 游戏方向4 == 游戏方向.下方 || 游戏方向4 == 游戏方向.左下)
                            {
                                return true;
                            }
                            break;
                        case 游戏方向.下方:
                            if (游戏方向4 == 游戏方向.右下 || 游戏方向4 == 游戏方向.下方 || 游戏方向4 == 游戏方向.左下)
                            {
                                return true;
                            }
                            break;
                    }
                }
                if ((类型 & 指定目标类型.所有玩家) == 指定目标类型.所有玩家)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool 能否走动()
        {
            if (this.对象死亡)
            {
                return false;
            }
            if (主程.当前时间 < this.忙碌时间)
            {
                return false;
            }
            if (主程.当前时间 < this.行走时间)
            {
                return false;
            }
            if (this.检查状态(游戏对象状态.忙绿状态 | 游戏对象状态.定身状态 | 游戏对象状态.麻痹状态 | 游戏对象状态.失神状态))
            {
                return false;
            }
            return true;
        }

        public virtual bool 能否跑动()
        {
            if (this.对象死亡)
            {
                return false;
            }
            if (主程.当前时间 < this.忙碌时间)
            {
                return false;
            }
            if (主程.当前时间 < this.奔跑时间)
            {
                return false;
            }
            if (this.检查状态(游戏对象状态.忙绿状态 | 游戏对象状态.残废状态 | 游戏对象状态.定身状态 | 游戏对象状态.麻痹状态 | 游戏对象状态.失神状态))
            {
                return false;
            }
            return true;
        }

        public virtual bool 能否转动()
        {
            if (this.对象死亡)
            {
                return false;
            }
            if (主程.当前时间 < this.忙碌时间)
            {
                return false;
            }
            if (主程.当前时间 < this.行走时间)
            {
                return false;
            }
            if (this.检查状态(游戏对象状态.忙绿状态 | 游戏对象状态.麻痹状态 | 游戏对象状态.失神状态))
            {
                return false;
            }
            return true;
        }

        public virtual bool 能被推动(地图对象 来源)
        {
            if (this == 来源)
            {
                return true;
            }
            if (this is 守卫实例)
            {
                return false;
            }
            if (this.当前等级 >= 来源.当前等级)
            {
                return false;
            }
            if (this is 怪物实例 { 可被技能推动: false })
            {
                return false;
            }
            if (来源.对象关系(this) != 游戏对象关系.敌对)
            {
                return false;
            }
            return true;
        }

        public virtual bool 能否位移(地图对象 来源, Point 锚点, int 距离, int 数量, bool 穿墙, out Point 终点, out 地图对象[] 目标)
        {
            终点 = this.当前坐标;
            目标 = null;
            if (!(this.当前坐标 == 锚点) && this.能被推动(来源))
            {
                List<地图对象> list;
                list = new List<地图对象>();
                for (int i = 1; i <= 距离; i++)
                {
                    if (穿墙)
                    {
                        Point point;
                        point = 计算类.前方坐标(this.当前坐标, 锚点, i);
                        if (this.当前地图.能否通行(point))
                        {
                            终点 = point;
                        }
                        continue;
                    }
                    游戏方向 方向;
                    方向 = 计算类.计算方向(this.当前坐标, 锚点);
                    Point point2;
                    point2 = 计算类.前方坐标(this.当前坐标, 锚点, i);
                    if (this.当前地图.地形阻塞(point2))
                    {
                        break;
                    }
                    bool flag;
                    flag = false;
                    if (this.当前地图.空间阻塞(point2))
                    {
                        foreach (地图对象 item in this.当前地图[point2].Where((地图对象 O) => O.阻塞网格))
                        {
                            if (list.Count < 数量)
                            {
                                if (item.能否位移(来源, 计算类.前方坐标(item.当前坐标, 方向, 1), 1, 数量 - list.Count - 1, 穿墙: false, out var _, out var 目标2))
                                {
                                    list.Add(item);
                                    list.AddRange(目标2);
                                    continue;
                                }
                                flag = true;
                                break;
                            }
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                    终点 = point2;
                }
                目标 = list.ToArray();
                return 终点 != this.当前坐标;
            }
            return false;
        }

        public virtual bool 检查状态(游戏对象状态 待检状态)
        {
            foreach (Buff数据 value in this.Buff列表.Values)
            {
                if ((value.Buff效果 & Buff效果类型.状态标志) != 0 && (value.Buff模板.角色所处状态 & 待检状态) != 0)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool 检查状态(游戏对象状态 待检状态, out Buff数据 buff)
        {
            buff = null;
            foreach (Buff数据 value in this.Buff列表.Values)
            {
                if ((value.Buff效果 & Buff效果类型.状态标志) != 0 && (value.Buff模板.角色所处状态 & 待检状态) != 0)
                {
                    buff = value;
                    return true;
                }
            }
            return false;
        }

        public Buff数据 添加Buff时处理(ushort 编号, 地图对象 来源)
        {
            // 未知暗殿一层每日进入计数：buff 2628 仅由版本脚本 6350.lua 在一层“当日首次进入”时添加(全引擎唯一来源)，
            // 在此盖戳 脚本变量[16]=当日(主程.当前时间.DayOfYear)，使 6350.lua 的“每日一次”判定真正生效(版本脚本缺该写入)。
            if (编号 == 2628 && this is 玩家实例 当前玩家)
            {
                当前玩家.角色数据.脚本变量[游戏服务器.副本类.副本.数字_未知一层每日进入次数] = 主程.当前时间.DayOfYear;
            }
            if (!(this is 物品实例) && !(this is 陷阱实例) && (!(this is 守卫实例 { 能否受伤: false }) || 来源 == this))
            {
                if (来源 == null)
                {
                    来源 = this;
                }
                if (来源 is 陷阱实例 陷阱实例2)
                {
                    来源 = 陷阱实例2.陷阱来源;
                }
                if (!游戏Buff.数据表.TryGetValue(编号, out var value))
                {
                    return null;
                }
                if ((value.Buff效果 & Buff效果类型.状态标志) != 0)
                {
                    if (((value.角色所处状态 & 游戏对象状态.隐身状态) != 0 || (value.角色所处状态 & 游戏对象状态.潜行状态) != 0) && this.检查状态(游戏对象状态.暴露状态))
                    {
                        return null;
                    }
                    if ((value.角色所处状态 & 游戏对象状态.暴露状态) != 0)
                    {
                        foreach (Buff数据 item in this.Buff列表.Values.ToList())
                        {
                            if ((item.Buff模板.角色所处状态 & 游戏对象状态.隐身状态) != 0 || (item.Buff模板.角色所处状态 & 游戏对象状态.潜行状态) != 0)
                            {
                                this.移除Buff时处理(item.Buff编号.V);
                            }
                        }
                    }
                }
                if ((value.Buff效果 & Buff效果类型.造成伤害) != 0 && value.Buff伤害类型 == 技能伤害类型.灼烧 && this.Buff列表.ContainsKey(25352))
                {
                    return null;
                }
                ushort 分组编号;
                分组编号 = ((value.分组编号 != 0) ? value.分组编号 : value.Buff编号);
                Buff数据 buff数据;
                buff数据 = null;
                switch (value.叠加类型)
                {
                    case Buff叠加类型.禁止叠加:
                        if (this.Buff列表.Values.FirstOrDefault((Buff数据 O) => O.Buff分组 == 分组编号) == null)
                        {
                            buff数据 = (this.Buff列表[value.Buff编号] = new Buff数据(来源, this, value.Buff编号));
                        }
                        break;
                    case Buff叠加类型.同类替换:
                        foreach (Buff数据 item2 in this.Buff列表.Values.Where((Buff数据 O) => O.Buff分组 == 分组编号).ToList())
                        {
                            this.移除Buff时处理(item2.Buff编号.V);
                        }
                        buff数据 = (this.Buff列表[value.Buff编号] = new Buff数据(来源, this, value.Buff编号));
                        break;
                    case Buff叠加类型.同类叠加:
                        {
                            if (this.Buff列表.TryGetValue(编号, out var v2))
                            {
                                v2.当前层数.V = Math.Min((byte)(v2.当前层数.V + 1), v2.最大层数);
                                if (value.每层触发Buff != null && v2.当前层数.V < value.每层触发Buff.Length)
                                {
                                    this.添加Buff时处理(value.每层触发Buff[v2.当前层数.V], 来源);
                                }
                                if (value.Buff允许合成 && v2.当前层数.V >= value.Buff合成层数 && 游戏Buff.数据表.TryGetValue(value.Buff合成编号, out var _))
                                {
                                    this.移除Buff时处理(v2.Buff编号.V);
                                    this.添加Buff时处理(value.Buff合成编号, 来源);
                                    if (value.合成触发技能 != null && 游戏技能.数据表.TryGetValue(value.合成触发技能, out var value3))
                                    {
                                        new 技能实例(this, value3, null, 0, this.当前地图, this.当前坐标, this, this.当前坐标, null);
                                    }
                                    break;
                                }
                                if ((value.Buff效果 & Buff效果类型.属性增减) != 0 && (value.按照层数计算 || value.层数计算系数))
                                {
                                    this.属性加成.Remove(v2);
                                    this.属性加成.Add(v2, v2.属性加成);
                                    this.属性转换.Remove(v2);
                                    this.属性转换.Add(v2, v2.属性转换);
                                    this.更新对象属性();
                                }
                                v2.剩余时间.V = v2.持续时间.V;
                                if (v2.Buff同步)
                                {
                                    this.发送封包(new 对象状态变动
                                    {
                                        对象编号 = this.地图编号,
                                        Buff编号 = v2.Buff编号.V,
                                        Buff索引 = v2.Buff编号.V,
                                        当前层数 = v2.当前层数.V,
                                        剩余时间 = (int)v2.剩余时间.V.TotalMilliseconds,
                                        持续时间 = (int)v2.持续时间.V.TotalMilliseconds
                                    });
                                }
                            }
                            else
                            {
                                buff数据 = (this.Buff列表[value.Buff编号] = new Buff数据(来源, this, value.Buff编号));
                                if (value.每层触发Buff != null && buff数据.当前层数.V < value.每层触发Buff.Length)
                                {
                                    this.添加Buff时处理(value.每层触发Buff[buff数据.当前层数.V], 来源);
                                }
                            }
                            break;
                        }
                    case Buff叠加类型.同类延时:
                        {
                            if (this.Buff列表.TryGetValue(编号, out var v))
                            {
                                v.剩余时间.V += v.持续时间.V;
                                if (v.Buff同步)
                                {
                                    this.发送封包(new 对象状态变动
                                    {
                                        对象编号 = this.地图编号,
                                        Buff编号 = v.Buff编号.V,
                                        Buff索引 = v.Buff编号.V,
                                        当前层数 = v.当前层数.V,
                                        剩余时间 = (int)v.剩余时间.V.TotalMilliseconds,
                                        持续时间 = (int)v.持续时间.V.TotalMilliseconds
                                    });
                                }
                            }
                            else
                            {
                                buff数据 = (this.Buff列表[value.Buff编号] = new Buff数据(来源, this, value.Buff编号));
                            }
                            break;
                        }
                }
                if (buff数据 == null)
                {
                    return null;
                }
                if (buff数据.Buff模板.添加触发LUA && this is 玩家实例 玩家实例2)
                {
                    玩家实例2.CallDefaultNPC(DefaultNPCType.buff_add, true, buff数据.Buff模板.Buff编号);
                }
                if (来源 is 玩家实例 玩家实例3)
                {
                    buff数据.持续时间.V += TimeSpan.FromMilliseconds(玩家实例3.龙卫BUFF持续时间(buff数据.Buff编号.V));
                    buff数据.剩余时间.V = buff数据.持续时间.V;
                    buff数据.护盾数值.V += 玩家实例3.龙卫BUFF增加护盾(buff数据.Buff编号.V);
                    buff数据.增减伤害基数 = 玩家实例3.龙卫BUFF伤害基数(buff数据.Buff编号.V);
                    buff数据.增减伤害系数 = 玩家实例3.龙卫BUFF伤害系数(buff数据.Buff编号.V);
                    if (来源 == this)
                    {
                        buff数据.增减伤害基数 += 玩家实例3.龙卫自身伤害基数(buff数据.Buff编号.V);
                        buff数据.增减伤害系数 += 玩家实例3.龙卫自身伤害系数(buff数据.Buff编号.V);
                    }
                }
                if (buff数据.Buff同步)
                {
                    this.发送封包(new 对象添加状态
                    {
                        对象编号 = this.地图编号,
                        Buff来源 = 来源.地图编号,
                        Buff编号 = buff数据.Buff编号.V,
                        Buff索引 = buff数据.Buff编号.V,
                        Buff层数 = buff数据.当前层数.V,
                        持续时间 = (int)buff数据.持续时间.V.TotalMilliseconds
                    });
                }
                if ((value.Buff效果 & Buff效果类型.属性增减) != 0)
                {
                    Dictionary<游戏对象属性, int> dictionary;
                    dictionary = null;
                    if (来源 is 玩家实例 玩家实例4)
                    {
                        dictionary = 玩家实例4.龙卫BUFF神圣攻击(buff数据.Buff编号.V);
                    }
                    if (dictionary == null)
                    {
                        dictionary = buff数据.属性加成;
                    }
                    this.属性加成.Add(buff数据, dictionary);
                    this.属性转换.Add(buff数据, buff数据.属性转换);
                    this.更新对象属性();
                }
                if ((value.Buff效果 & Buff效果类型.状态标志) != 0)
                {
                    if ((value.角色所处状态 & 游戏对象状态.隐身状态) != 0)
                    {
                        foreach (地图对象 item3 in this.邻居列表.ToList())
                        {
                            item3.对象隐身时处理(this);
                        }
                    }
                    if ((value.角色所处状态 & 游戏对象状态.潜行状态) != 0)
                    {
                        foreach (地图对象 item4 in this.邻居列表.ToList())
                        {
                            item4.对象潜行时处理(this);
                        }
                    }
                    if ((value.Buff效果 & Buff效果类型.坐骑BUFF) != 0 && this is 玩家实例 玩家实例5 && (value.角色所处状态 & 游戏对象状态.坐骑状态) != 0 && 玩家实例5.当前坐骑 > 0)
                    {
                        this.发送封包(new 玩家骑乘上马
                        {
                            对象编号 = this.地图编号,
                            坐骑编号 = 玩家实例5.当前坐骑
                        });
                        if (游戏坐骑.数据表.TryGetValue(玩家实例5.当前坐骑, out var value4))
                        {
                            this.属性加成.Add(buff数据, value4.坐骑属性);
                            this.更新对象属性();
                            for (byte b = 0; b < 4; b++)
                            {
                                if (玩家实例5.御兽列表[b] > 0 && 游戏坐骑.数据表.TryGetValue((ushort)玩家实例5.御兽列表[b], out var value5) && value5.魂兽BUFF > 0)
                                {
                                    玩家实例5.添加Buff时处理(value5.魂兽BUFF, this);
                                }
                            }
                            if (value4.魂兽BUFF > 0)
                            {
                                玩家实例5.添加Buff时处理(value4.魂兽BUFF, this);
                            }
                        }
                    }
                }
                if (value.连带Buff编号 != 0)
                {
                    this.添加Buff时处理(value.连带Buff编号, 来源);
                    if (value.连带Buff范围 != 0)
                    {
                        Point[] array;
                        array = 计算类.技能范围(this.当前坐标, this.当前方向, value.连带Buff范围);
                        foreach (Point 坐标 in array)
                        {
                            foreach (地图对象 item5 in this.当前地图[坐标])
                            {
                                if ((value.连带目标关系 & this.对象关系(item5)) != 0 && (value.连带目标类型 & this.对象类型) != 0)
                                {
                                    item5.添加Buff时处理(value.连带Buff编号, 来源);
                                }
                            }
                        }
                    }
                }
                if (buff数据.连带列表 != null)
                {
                    ushort[] 连带列表;
                    连带列表 = buff数据.连带列表;
                    foreach (ushort num in 连带列表)
                    {
                        if (num != 0)
                        {
                            this.添加Buff时处理(num, buff数据.Buff来源);
                        }
                    }
                }
                if (this is 玩家实例 玩家实例6 && value.添加技能编号 != 0)
                {
                    玩家实例6.玩家学习技能(value.添加技能编号, 0);
                }
                return buff数据;
            }
            return null;
        }

        public void 移除Buff时处理(ushort 编号)
        {
            if (!this.Buff列表.TryGetValue(编号, out var v))
            {
                if (游戏Buff.数据表.TryGetValue(编号, out var value) && value.依存Buff列表 != null)
                {
                    ushort[] 依存Buff列表;
                    依存Buff列表 = value.依存Buff列表;
                    for (int i = 0; i < 依存Buff列表.Length; i++)
                    {
                        this.移除Buff时处理(依存Buff列表[i]);
                    }
                }
                return;
            }
            if (this is 玩家实例 玩家实例2)
            {
                if (v.Buff模板.添加技能编号 != 0)
                {
                    玩家实例2.玩家移除技能(v.Buff模板.添加技能编号);
                }
                if (v.到期换图 && (v.Buff模板.检测所在地图 == 0 || 玩家实例2.当前地图.地图编号 == v.Buff模板.检测所在地图))
                {
                    地图实例 地图实例2;
                    地图实例2 = 地图处理网关.已分配地图(v.切换地图);
                    if (地图实例2 != null)
                    {
                        玩家实例2.玩家切换地图(地图实例2, 地图区域类型.传送区域);
                    }
                }
                if (v.Buff模板.到期减少冷却 && this.冷却记录.TryGetValue(v.Buff模板.减少冷却技能 | 0x1000000, out var v2))
                {
                    v2 -= TimeSpan.FromMilliseconds(v.Buff模板.每层减少冷却 * v.当前层数.V);
                    this.冷却记录[v.Buff模板.减少冷却技能 | 0x1000000] = v2;
                    this.发送封包(new 添加技能冷却
                    {
                        冷却编号 = (v.Buff模板.减少冷却技能 | 0x1000000),
                        冷却时间 = Math.Max(0, (int)(v2 - 主程.当前时间).TotalMilliseconds)
                    });
                }
                if (v.Buff模板.移除触发LUA)
                {
                    玩家实例2.CallDefaultNPC(DefaultNPCType.buff_remove, true, v.Buff模板.Buff编号);
                }
            }
            if (v.Buff来源 == null || (地图处理网关.地图对象表.TryGetValue(v.Buff来源.地图编号, out var value2) && value2 == v.Buff来源))
            {
                if (v.Buff模板.后接Buff编号 != 0)
                {
                    this.添加Buff时处理(v.Buff模板.后接Buff编号, v.Buff来源);
                }
                if (v.后接列表 != null)
                {
                    ushort[] 依存Buff列表;
                    依存Buff列表 = v.后接列表;
                    foreach (ushort num in 依存Buff列表)
                    {
                        if (num != 0)
                        {
                            this.添加Buff时处理(num, v.Buff来源);
                        }
                    }
                }
            }
            if (this is 宠物实例 宠物实例2 && 编号 == 宠物实例2.绑定BUFF)
            {
                宠物实例2.自身死亡处理(null, 技能击杀: false);
            }
            if (v.依存列表 != null)
            {
                ushort[] 依存列表;
                依存列表 = v.依存列表;
                for (int j = 0; j < 依存列表.Length; j++)
                {
                    this.删除Buff时处理(依存列表[j]);
                }
            }
            if (v.添加冷却 && v.绑定技能 != 0 && v.冷却时间 != 0 && this is 玩家实例 玩家实例3 && 玩家实例3.主体技能表.ContainsKey(v.绑定技能))
            {
                DateTime dateTime;
                dateTime = 主程.当前时间.AddMilliseconds((int)v.冷却时间);
                if (dateTime > (this.冷却记录.ContainsKey(v.绑定技能 | 0x1000000) ? this.冷却记录[v.绑定技能 | 0x1000000] : default(DateTime)))
                {
                    this.冷却记录[v.绑定技能 | 0x1000000] = dateTime;
                    this.发送封包(new 添加技能冷却
                    {
                        冷却编号 = (v.绑定技能 | 0x1000000),
                        冷却时间 = v.冷却时间
                    });
                }
            }
            this.Buff列表.Remove(编号);
            v.删除数据();
            if (v.Buff同步)
            {
                this.发送封包(new 对象移除状态
                {
                    对象编号 = this.地图编号,
                    Buff索引 = 编号
                });
            }
            if ((v.Buff效果 & Buff效果类型.属性增减) != 0)
            {
                this.属性加成.Remove(v);
                this.属性转换.Remove(v);
                this.更新对象属性();
            }
            if ((v.Buff效果 & Buff效果类型.状态标志) == 0)
            {
                return;
            }
            if ((v.Buff效果 & Buff效果类型.坐骑BUFF) != 0 && this is 玩家实例 玩家实例4 && (v.Buff模板.角色所处状态 & 游戏对象状态.坐骑状态) != 0)
            {
                this.发送封包(new 玩家骑乘下马
                {
                    对象编号 = this.地图编号
                });
                this.属性加成.Remove(v);
                this.属性转换.Remove(v);
                if (游戏坐骑.数据表.TryGetValue(玩家实例4.当前坐骑, out var value3))
                {
                    for (byte b = 0; b < 4; b++)
                    {
                        if (玩家实例4.御兽列表[b] > 0 && 游戏坐骑.数据表.TryGetValue((ushort)玩家实例4.御兽列表[b], out var value4) && value4.魂兽BUFF > 0)
                        {
                            玩家实例4.移除Buff时处理(value4.魂兽BUFF);
                        }
                    }
                    this.更新对象属性();
                    if (value3.魂兽BUFF > 0)
                    {
                        玩家实例4.移除Buff时处理(value3.魂兽BUFF);
                    }
                }
            }
            if ((v.Buff模板.角色所处状态 & 游戏对象状态.隐身状态) != 0)
            {
                foreach (地图对象 item in this.邻居列表.ToList())
                {
                    item.对象显隐时处理(this);
                }
            }
            if ((v.Buff模板.角色所处状态 & 游戏对象状态.潜行状态) == 0)
            {
                return;
            }
            foreach (地图对象 item2 in this.邻居列表.ToList())
            {
                item2.对象显行时处理(this);
            }
        }

        public void 增加Buff层数(ushort 编号, int 层数)
        {
            if (this.Buff列表.TryGetValue(编号, out var v))
            {
                v.当前层数.V = (byte)Math.Max(0, v.当前层数.V + 层数);
                v.处理计时.V = TimeSpan.FromMilliseconds(v.Buff模板.Buff处理延迟);
                if (v.当前层数.V == 0)
                {
                    this.删除Buff时处理(编号);
                    return;
                }
                this.发送封包(new 对象状态变动
                {
                    对象编号 = this.地图编号,
                    Buff编号 = v.Buff编号.V,
                    Buff索引 = v.Buff编号.V,
                    当前层数 = v.当前层数.V,
                    剩余时间 = (int)v.剩余时间.V.TotalMilliseconds,
                    持续时间 = (int)v.持续时间.V.TotalMilliseconds
                });
            }
        }

        public void 设置Buff时间(ushort 编号, int 时间)
        {
            if (this.Buff列表.TryGetValue(编号, out var v))
            {
                v.剩余时间.V = TimeSpan.FromMilliseconds(时间);
                v.持续时间.V = TimeSpan.FromMilliseconds(时间);
                this.发送封包(new 对象状态变动
                {
                    对象编号 = this.地图编号,
                    Buff编号 = v.Buff编号.V,
                    Buff索引 = v.Buff编号.V,
                    当前层数 = v.当前层数.V,
                    剩余时间 = (int)v.剩余时间.V.TotalMilliseconds,
                    持续时间 = (int)v.持续时间.V.TotalMilliseconds
                });
            }
        }

        public void 删除Buff时处理(ushort 编号, bool 后接BUFF = true)
        {
            if (!this.Buff列表.TryGetValue(编号, out var v))
            {
                if (游戏Buff.数据表.TryGetValue(编号, out var value) && value.依存Buff列表 != null)
                {
                    ushort[] 依存Buff列表;
                    依存Buff列表 = value.依存Buff列表;
                    for (int i = 0; i < 依存Buff列表.Length; i++)
                    {
                        this.删除Buff时处理(依存Buff列表[i], 后接BUFF);
                    }
                }
                return;
            }
            if (this is 玩家实例 玩家实例2 && v.Buff模板.添加技能编号 != 0)
            {
                玩家实例2.玩家移除技能(v.Buff模板.添加技能编号);
            }
            if (v.依存列表 != null)
            {
                ushort[] 依存列表;
                依存列表 = v.依存列表;
                for (int j = 0; j < 依存列表.Length; j++)
                {
                    this.删除Buff时处理(依存列表[j]);
                }
            }
            if (后接BUFF && v.Buff模板.删除后接BUFF != 0)
            {
                this.添加Buff时处理(v.Buff模板.删除后接BUFF, v.Buff来源);
            }
            if (v.Buff模板.删除触发LUA && this is 玩家实例 玩家实例3)
            {
                玩家实例3.CallDefaultNPC(DefaultNPCType.buff_remove, true, v.Buff模板.Buff编号);
            }
            this.Buff列表.Remove(编号);
            v.删除数据();
            if (v.Buff同步)
            {
                this.发送封包(new 对象移除状态
                {
                    对象编号 = this.地图编号,
                    Buff索引 = 编号
                });
            }
            if ((v.Buff效果 & Buff效果类型.属性增减) != 0)
            {
                this.属性加成.Remove(v);
                this.属性转换.Remove(v);
                this.更新对象属性();
            }
            if ((v.Buff效果 & Buff效果类型.状态标志) == 0)
            {
                return;
            }
            if ((v.Buff模板.角色所处状态 & 游戏对象状态.隐身状态) != 0)
            {
                foreach (地图对象 item in this.邻居列表.ToList())
                {
                    item.对象显隐时处理(this);
                }
            }
            if ((v.Buff效果 & Buff效果类型.坐骑BUFF) != 0 && this is 玩家实例 玩家实例4 && (v.Buff模板.角色所处状态 & 游戏对象状态.坐骑状态) != 0)
            {
                this.发送封包(new 玩家骑乘下马
                {
                    对象编号 = this.地图编号
                });
                this.属性加成.Remove(v);
                this.属性转换.Remove(v);
                if (游戏坐骑.数据表.TryGetValue(玩家实例4.当前坐骑, out var value2))
                {
                    for (byte b = 0; b < 4; b++)
                    {
                        if (玩家实例4.御兽列表[b] > 0 && 游戏坐骑.数据表.TryGetValue((ushort)玩家实例4.御兽列表[b], out var value3) && value3.魂兽BUFF > 0)
                        {
                            玩家实例4.删除Buff时处理(value3.魂兽BUFF);
                        }
                    }
                    this.更新对象属性();
                    if (value2.魂兽BUFF > 0)
                    {
                        玩家实例4.删除Buff时处理(value2.魂兽BUFF);
                    }
                }
            }
            if ((v.Buff模板.角色所处状态 & 游戏对象状态.潜行状态) == 0)
            {
                return;
            }
            foreach (地图对象 item2 in this.邻居列表.ToList())
            {
                item2.对象显行时处理(this);
            }
        }

        public void 轮询Buff时处理(Buff数据 数据)
        {
            if (数据.Buff模板.执行触发LUA && this is 玩家实例 玩家实例2)
            {
                玩家实例2.CallDefaultNPC(DefaultNPCType.buff_run, true, 数据.Buff模板.Buff编号);
            }
            if (数据.离线计算 && 数据.到期消失 && 数据.添加时间.V.Add(数据.持续时间.V) < 主程.当前时间)
            {
                this.移除Buff时处理(数据.Buff编号.V);
            }
            else if (数据.到期消失 && (数据.剩余时间.V -= 主程.当前时间 - this.处理计时) < TimeSpan.Zero)
            {
                this.移除Buff时处理(数据.Buff编号.V);
            }
            else
            {
                if (!((数据.处理计时.V -= 主程.当前时间 - this.处理计时) < TimeSpan.Zero))
                {
                    return;
                }
                数据.处理计时.V += TimeSpan.FromMilliseconds(数据.处理间隔);
                if ((数据.Buff效果 & Buff效果类型.造成伤害) != 0)
                {
                    if (数据.触发技能伤害 > 0 && 数据.Buff来源 is 玩家实例 玩家实例3 && 玩家实例3.主体技能表.TryGetValue(数据.触发技能伤害, out var v))
                    {
                        foreach (string item in v.铭文模板.主体技能列表.ToList())
                        {
                            if (游戏技能.数据表.TryGetValue(item, out var value) && value.自身技能编号 == v.技能编号.V && (value.验证已学技能 == 0 || (玩家实例3.主体技能表.TryGetValue(value.验证已学技能, out var v2) && (value.验证技能铭文 == 0 || value.验证技能铭文 == v2.铭文编号))))
                            {
                                if (value.节点列表.Values.FirstOrDefault((技能任务 O) => O is C_02_计算目标伤害) is C_02_计算目标伤害 c_02_计算目标伤害)
                                {
                                    命中详情 命中详情;
                                    命中详情 = new 命中详情(this, 技能命中反馈.后仰);
                                    bool 伤害不计神圣;
                                    伤害不计神圣 = c_02_计算目标伤害.伤害不计神圣;
                                    c_02_计算目标伤害.伤害不计神圣 = 数据.伤害不计神圣;
                                    this.被动受伤时处理(数据.Buff来源, v.技能编号.V, v.铭文编号, v.技能等级.V, c_02_计算目标伤害, 命中详情, 1f, 1);
                                    c_02_计算目标伤害.伤害不计神圣 = 伤害不计神圣;
                                    数据.Buff来源.发送封包(new 触发命中特效
                                    {
                                        对象编号 = 数据.Buff来源.地图编号,
                                        技能编号 = v.技能编号.V,
                                        技能等级 = v.技能等级.V,
                                        技能铭文 = v.铭文编号,
                                        动作编号 = this.动作编号,
                                        目标编号 = this.地图编号,
                                        技能反馈 = (ushort)命中详情.技能反馈,
                                        技能伤害 = -命中详情.技能伤害,
                                        招架伤害 = 命中详情.招架伤害,
                                        附加特效 = c_02_计算目标伤害.附加特效编号
                                    });
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        this.被动受伤时处理(数据);
                    }
                }
                if ((数据.Buff效果 & Buff效果类型.生命回复) != 0)
                {
                    this.被动回复时处理(数据);
                }
                if ((数据.Buff效果 & Buff效果类型.添加BUFF) != 0)
                {
                    this.添加Buff时处理(数据.Buff模板.添加BUFF编号, this);
                }
                if ((数据.Buff效果 & Buff效果类型.释放技能) != 0 && 数据.Buff模板.释放技能编号 != null && 游戏技能.数据表.TryGetValue(数据.Buff模板.释放技能编号, out var value2))
                {
                    new 技能实例(this, value2, null, 0, this.当前地图, this.当前坐标, null, this.当前坐标, null);
                }
                if ((数据.Buff效果 & Buff效果类型.获得奖励) != 0 && this is 玩家实例 玩家实例4)
                {
                    if (数据.Buff模板.增加角色经验 > 0)
                    {
                        玩家实例4.玩家增加经验(null, 数据.Buff模板.增加角色经验);
                    }
                    if (数据.Buff模板.增加双倍经验 > 0)
                    {
                        玩家实例4.双倍经验 += 数据.Buff模板.增加双倍经验;
                    }
                    if (数据.Buff模板.增加角色金币 > 0)
                    {
                        玩家实例4.修改货币("+", 游戏货币.金币, (uint)数据.Buff模板.增加角色金币);
                    }
                    if (数据.Buff模板.增加角色银币 > 0)
                    {
                        玩家实例4.修改货币("+", 游戏货币.银币, (uint)数据.Buff模板.增加角色银币);
                    }
                }
            }
        }

        public void 被技能命中处理(技能实例 技能, C_01_计算命中目标 参数)
        {
            地图对象 地图对象2;
            地图对象2 = ((技能.技能来源 is 陷阱实例 陷阱实例2) ? 陷阱实例2.陷阱来源 : 技能.技能来源);
            if (this is 玩家实例 { 无敌模式: not false } || 技能.命中列表.ContainsKey(this.地图编号) || !this.能被命中 || (this != 地图对象2 && !this.邻居列表.Contains(地图对象2)) || 技能.命中列表.Count >= 参数.限定命中数量 || 技能.命中列表.Count >= 参数.限定命中数量 || (参数.限定目标关系 & 地图对象2.对象关系(this)) == 0 || (参数.限定目标类型 & this.对象类型) == 0 || !this.特定类型(技能.技能来源, 参数.限定特定类型) || ((参数.限定目标关系 & 游戏对象关系.敌对) != 0 && (参数.限定目标关系 & 地图对象2.对象关系(this)) != 游戏对象关系.友方 && (参数.限定目标关系 & 地图对象2.对象关系(this)) != 游戏对象关系.自身 && (this.检查状态(游戏对象状态.无敌状态) || ((this is 玩家实例 || this is 宠物实例) && (地图对象2 is 玩家实例 || 地图对象2 is 宠物实例) && (this.当前地图.安全区内(this.当前坐标) || 地图对象2.当前地图.安全区内(地图对象2.当前坐标))) || (地图对象2 is 怪物实例 && this.当前地图.安全区内(this.当前坐标)))) || (this is 怪物实例 怪物实例2 && (怪物实例2.模板编号 == 8618 || 怪物实例2.模板编号 == 8621) && ((地图对象2 is 玩家实例 { 所属行会: not null } 玩家实例3 && 玩家实例3.所属行会 == 系统数据.数据.占领行会.V) || (地图对象2 is 宠物实例 { 宠物主人: not null } 宠物实例2 && 宠物实例2.宠物主人.所属行会 != null && 宠物实例2.宠物主人.所属行会 == 系统数据.数据.占领行会.V))))
            {
                return;
            }
            int num;
            num = 0;
            float num2;
            num2 = 0f;
            int num3;
            num3 = 0;
            float num4;
            num4 = 0f;
            switch (参数.技能闪避方式)
            {
                case 技能闪避类型.技能无法闪避:
                    num = 1;
                    break;
                case 技能闪避类型.可被物理闪避:
                    num3 = this[游戏对象属性.物理敏捷];
                    num = 地图对象2[游戏对象属性.物理准确];
                    if (this is 怪物实例)
                    {
                        num2 += (float)地图对象2[游戏对象属性.怪物命中] / 10000f;
                    }
                    if (地图对象2 is 怪物实例)
                    {
                        num4 += (float)this[游戏对象属性.怪物闪避] / 10000f;
                    }
                    break;
                case 技能闪避类型.可被魔法闪避:
                    num4 = (float)this[游戏对象属性.魔法闪避] / 10000f;
                    if (this is 怪物实例)
                    {
                        num2 += (float)地图对象2[游戏对象属性.怪物命中] / 10000f;
                    }
                    if (地图对象2 is 怪物实例)
                    {
                        num4 += (float)this[游戏对象属性.怪物闪避] / 10000f;
                    }
                    break;
                case 技能闪避类型.可被中毒闪避:
                    num4 = (float)this[游戏对象属性.中毒躲避] / 10000f;
                    break;
                case 技能闪避类型.非怪物可闪避:
                    if (this is 怪物实例)
                    {
                        num = 1;
                        break;
                    }
                    num3 = this[游戏对象属性.物理敏捷];
                    num = 地图对象2[游戏对象属性.物理准确];
                    break;
            }
            命中详情 命中详情;
            命中详情 = new 命中详情(this)
            {
                技能反馈 = (计算类.计算命中(num, num3, num2, num4) ? 参数.技能命中反馈 : 技能命中反馈.闪避)
            };
            if (命中详情.技能反馈 != 技能命中反馈.闪避)
            {
                if (技能.技能来源 is 玩家实例 玩家实例4 && 玩家实例4.生效龙卫.Count > 0)
                {
                    玩家实例4.龙卫添加目标BUFF(技能.技能编号, 技能.铭文编号, this);
                }
                if (this is 玩家实例 { 操作道具: not false } 玩家实例5)
                {
                    玩家实例5.探索道具?.道具?.Stop(玩家实例5.探索道具);
                }
            }
            技能.命中列表.Add(this.地图编号, 命中详情);
        }

        public void 被动受伤时处理(技能实例 技能, C_02_计算目标伤害 参数, 命中详情 详情, float 伤害系数, int 命中数量)
        {
            this.被动受伤时处理(技能.技能来源, 技能.技能编号, 技能.铭文编号, 技能.技能等级, 参数, 详情, 伤害系数, 命中数量);
        }

        public void 被动受伤时处理(地图对象 技能来源, ushort 技能编号, byte 铭文编号, byte 技能等级, C_02_计算目标伤害 参数, 命中详情 详情, float 伤害系数, int 命中数量)
        {
            foreach (技能实例 item2 in this.技能任务)
            {
                if (item2.动作打断)
                {
                    item2.是否中断 = true;
                }
            }
            int num;
            num = 0;
            地图对象 地图对象2;
            地图对象2 = ((技能来源 is 陷阱实例 陷阱实例2) ? 陷阱实例2.陷阱来源 : 技能来源);
            if (this.对象死亡)
            {
                详情.技能反馈 = 技能命中反馈.丢失;
            }
            else if (!this.邻居列表.Contains(地图对象2))
            {
                详情.技能反馈 = 技能命中反馈.丢失;
            }
            else if (this is 怪物实例 怪物实例2 && (怪物实例2.模板编号 == 8618 || 怪物实例2.模板编号 == 8621) && this.网格距离(地图对象2) >= 4)
            {
                详情.技能反馈 = 技能命中反馈.丢失;
            }
            if ((详情.技能反馈 & 技能命中反馈.免疫) != 0 || (详情.技能反馈 & 技能命中反馈.丢失) != 0)
            {
                return;
            }
            if ((详情.技能反馈 & 技能命中反馈.闪避) == 0)
            {
                if (参数.技能斩杀类型 == 指定目标类型.无 || !计算类.计算概率(参数.技能斩杀概率) || !this.特定类型(地图对象2, 参数.技能斩杀类型))
                {
                    float num2;
                    num2 = 0f;
                    if (地图对象2 is 玩家实例 玩家实例2 && 玩家实例2.龙卫属性.Count > 0)
                    {
                        num2 += 玩家实例2.获取龙卫增伤系数(this, 技能编号, 铭文编号);
                    }
                    if (this is 玩家实例 玩家实例3 && 玩家实例3.龙卫属性.Count > 0)
                    {
                        num2 -= 玩家实例3.获取龙卫减伤系数(地图对象2, 技能编号, 铭文编号);
                    }
                    int[] 技能伤害基数;
                    技能伤害基数 = 参数.技能伤害基数;
                    int num3;
                    num3 = ((技能伤害基数 != null && 技能伤害基数.Length > 技能等级) ? 参数.技能伤害基数[技能等级] : 0);
                    float[] 技能伤害系数;
                    技能伤害系数 = 参数.技能伤害系数;
                    float num4;
                    num4 = ((技能伤害系数 != null && 技能伤害系数.Length > 技能等级) ? 参数.技能伤害系数[技能等级] : 0f);
                    if (this is 怪物实例)
                    {
                        num3 += 地图对象2[游戏对象属性.怪物伤害];
                    }
                    int num5;
                    num5 = 0;
                    float num6;
                    num6 = 0f;
                    if (参数.技能增伤类型 != 0 && this.特定类型(地图对象2, 参数.技能增伤类型))
                    {
                        num5 = 参数.技能增伤基数;
                        num6 = 参数.技能增伤系数;
                    }
                    int num7;
                    num7 = 0;
                    float num8;
                    num8 = 0f;
                    if (参数.技能破防概率 > 0f && 计算类.计算概率(参数.技能破防概率))
                    {
                        num7 = 参数.技能破防基数;
                        num8 = 参数.技能破防系数;
                    }
                    int num9;
                    num9 = 0;
                    int num10;
                    num10 = 0;
                    float num11;
                    num11 = 0f;
                    float num12;
                    num12 = 0f;
                    if (技能来源 is 玩家实例 玩家实例4)
                    {
                        num11 = 玩家实例4.破物防;
                    }
                    if (技能来源 is 玩家实例 玩家实例5)
                    {
                        num12 = 玩家实例5.破魔防;
                    }
                    switch (参数.技能伤害类型)
                    {
                        case 技能伤害类型.攻击:
                            num10 = 计算类.计算防御(this[游戏对象属性.最小防御], this[游戏对象属性.最大防御]);
                            num10 = (int)((float)num10 - (float)num10 * num11);
                            num9 = 计算类.计算攻击(地图对象2[游戏对象属性.最小攻击], 地图对象2[游戏对象属性.最大攻击], 地图对象2[游戏对象属性.幸运等级]);
                            if (this is 玩家实例 玩家实例7)
                            {
                                num2 -= 玩家实例7.获取龙卫特定减伤系数(物理减伤: true, 魔法减伤: false);
                            }
                            break;
                        case 技能伤害类型.魔法:
                            num10 = 计算类.计算防御(this[游戏对象属性.最小魔防], this[游戏对象属性.最大魔防]);
                            num10 = (int)((float)num10 - (float)num10 * num12);
                            num9 = 计算类.计算攻击(地图对象2[游戏对象属性.最小魔法], 地图对象2[游戏对象属性.最大魔法], 地图对象2[游戏对象属性.幸运等级]);
                            if (this is 玩家实例 玩家实例9)
                            {
                                num2 -= 玩家实例9.获取龙卫特定减伤系数(物理减伤: false, 魔法减伤: true);
                            }
                            break;
                        case 技能伤害类型.道术:
                            num10 = 计算类.计算防御(this[游戏对象属性.最小魔防], this[游戏对象属性.最大魔防]);
                            num10 = (int)((float)num10 - (float)num10 * num12);
                            num9 = 计算类.计算攻击(地图对象2[游戏对象属性.最小道术], 地图对象2[游戏对象属性.最大道术], 地图对象2[游戏对象属性.幸运等级]);
                            if (this is 玩家实例 玩家实例10)
                            {
                                num2 -= 玩家实例10.获取龙卫特定减伤系数(物理减伤: false, 魔法减伤: true);
                            }
                            break;
                        case 技能伤害类型.刺术:
                            num10 = 计算类.计算防御(this[游戏对象属性.最小防御], this[游戏对象属性.最大防御]);
                            num10 = (int)((float)num10 - (float)num10 * num11);
                            num9 = 计算类.计算攻击(地图对象2[游戏对象属性.最小刺术], 地图对象2[游戏对象属性.最大刺术], 地图对象2[游戏对象属性.幸运等级]);
                            if (this is 玩家实例 玩家实例8)
                            {
                                num2 -= 玩家实例8.获取龙卫特定减伤系数(物理减伤: true, 魔法减伤: false);
                            }
                            break;
                        case 技能伤害类型.弓术:
                            num10 = 计算类.计算防御(this[游戏对象属性.最小防御], this[游戏对象属性.最大防御]);
                            num10 = (int)((float)num10 - (float)num10 * num11);
                            num9 = 计算类.计算攻击(地图对象2[游戏对象属性.最小弓术], 地图对象2[游戏对象属性.最大弓术], 地图对象2[游戏对象属性.幸运等级]);
                            if (this is 玩家实例 玩家实例6)
                            {
                                num2 -= 玩家实例6.获取龙卫特定减伤系数(物理减伤: true, 魔法减伤: false);
                            }
                            break;
                        case 技能伤害类型.毒性:
                            num9 = 地图对象2[游戏对象属性.最大道术];
                            break;
                        case 技能伤害类型.神圣:
                            num9 = 计算类.计算攻击(地图对象2[游戏对象属性.最小圣伤], 地图对象2[游戏对象属性.最大圣伤], 0);
                            break;
                    }
                    if (!参数.伤害不计神圣)
                    {
                        num = 计算类.计算攻击(地图对象2[游戏对象属性.最小圣伤], 地图对象2[游戏对象属性.最大圣伤], 0);
                        num = ((命中数量 > 1) ? ((int)((float)num / 4f)) : num);
                    }
                    int num13;
                    num13 = 地图对象2[游戏对象属性.暴击概率] - this[游戏对象属性.减暴击];
                    int num14;
                    num14 = 地图对象2[游戏对象属性.暴击伤害] - this[游戏对象属性.减暴伤];
                    if (num13 > 0 && (num14 > 0 || num9 > 0) && 计算类.计算概率(num13 / 10000))
                    {
                        num9 += num9 + num14;
                        详情.技能反馈 |= 技能命中反馈.暴击;
                    }
                    if (this is 怪物实例)
                    {
                        num10 = Math.Max(0, num10 - (int)((float)(num10 * 地图对象2[游戏对象属性.怪物破防]) / 10000f));
                    }
                    this.转移伤害对象列表.Clear();
                    this.转移伤害基数列表.Clear();
                    this.转移伤害免减列表.Clear();
                    this.转移伤害系数列表.Clear();
                    int num15;
                    num15 = 0;
                    float num16;
                    num16 = 0f;
                    int num17;
                    num17 = int.MaxValue;
                    foreach (Buff数据 item3 in 地图对象2.Buff列表.Values.ToList())
                    {
                        if (this.伤害类型校验(参数.技能伤害类型, item3.Buff模板.受伤类型判定, 0))
                        {
                            if (item3.Buff模板.攻击触发技能 != null && 游戏技能.数据表.TryGetValue(item3.Buff模板.攻击触发技能, out var value) && this.特定类型(地图对象2, item3.Buff模板.攻击触发限定))
                            {
                                if (item3.Buff模板.成功移除自身)
                                {
                                    地图对象2.移除Buff时处理(item3.Buff编号.V);
                                }
                                new 技能实例(地图对象2, value, null, 0, this.当前地图, this.当前坐标, this, this.当前坐标, null);
                            }
                            if (item3.Buff模板.攻击触发BUFF != 0 && this.特定类型(地图对象2, item3.Buff模板.攻击触发限定))
                            {
                                if (item3.Buff模板.成功移除自身)
                                {
                                    地图对象2.移除Buff时处理(item3.Buff编号.V);
                                }
                                地图对象2.添加Buff时处理(item3.Buff模板.攻击触发BUFF, 地图对象2);
                            }
                        }
                        if (!this.特定类型(地图对象2, item3.Buff模板.限定目标类型) || (item3.Buff效果 & Buff效果类型.伤害增减) == 0 || (item3.Buff模板.效果判定方式 != 0 && item3.Buff模板.效果判定方式 != Buff判定方式.主动攻击减伤 && item3.Buff模板.效果判定方式 != Buff判定方式.主增基被减系) || (item3.Buff模板.对象血量比例 > 0f && (float)this.当前体力 / (float)this[游戏对象属性.最大体力] > item3.Buff模板.对象血量比例) || (item3.Buff模板.自身血量比例 > 0f && (float)地图对象2.当前体力 / (float)地图对象2[游戏对象属性.最大体力] > item3.Buff模板.自身血量比例))
                        {
                            continue;
                        }
                        bool flag;
                        flag = false;
                        switch (参数.技能伤害类型)
                        {
                            case 技能伤害类型.魔法:
                            case 技能伤害类型.道术:
                                switch (item3.Buff模板.效果判定类型)
                                {
                                    case Buff判定类型.检测对象BUFF:
                                        flag = item3.Buff模板.特定BUFF编号?.Overlaps(this.Buff列表.Keys.ToHashSet()) ?? false;
                                        break;
                                    case Buff判定类型.所有技能伤害:
                                    case Buff判定类型.所有魔法伤害:
                                        flag = true;
                                        break;
                                    case Buff判定类型.所有特定伤害:
                                        flag = item3.Buff模板.特定技能编号?.Contains(技能编号) ?? false;
                                        break;
                                }
                                break;
                            case 技能伤害类型.攻击:
                            case 技能伤害类型.刺术:
                            case 技能伤害类型.弓术:
                                switch (item3.Buff模板.效果判定类型)
                                {
                                    case Buff判定类型.检测对象BUFF:
                                        flag = item3.Buff模板.特定BUFF编号?.Overlaps(this.Buff列表.Keys.ToHashSet()) ?? false;
                                        break;
                                    case Buff判定类型.所有特定伤害:
                                        flag = item3.Buff模板.特定技能编号?.Contains(技能编号) ?? false;
                                        break;
                                    case Buff判定类型.所有技能伤害:
                                    case Buff判定类型.所有物理伤害:
                                        flag = true;
                                        break;
                                }
                                break;
                            case 技能伤害类型.毒性:
                            case 技能伤害类型.神圣:
                            case 技能伤害类型.灼烧:
                            case 技能伤害类型.撕裂:
                                if (item3.Buff模板.效果判定类型 == Buff判定类型.所有特定伤害)
                                {
                                    flag = item3.Buff模板.特定技能编号?.Contains(技能编号) ?? false;
                                }
                                if (item3.Buff模板.效果判定类型 == Buff判定类型.检测对象BUFF)
                                {
                                    flag = item3.Buff模板.特定BUFF编号?.Overlaps(this.Buff列表.Keys.ToHashSet()) ?? false;
                                }
                                break;
                        }
                        if (!flag != item3.Buff模板.效果判定取反)
                        {
                            continue;
                        }
                        int num18;
                        num18 = ((item3.Buff模板.伤害转移距离 == 0 || item3.Buff来源 == null || 计算类.网格距离(this.当前坐标, item3.Buff来源.当前坐标) <= item3.Buff模板.伤害转移距离) ? ((item3.Buff模板.不算BUFF层数 ? 1 : item3.当前层数.V) * ((item3.Buff模板.伤害增减基数?.Length > item3.Buff等级.V) ? item3.Buff模板.伤害增减基数[item3.Buff等级.V] : 0)) : 0);
                        if (地图对象2 is 宠物实例 宠物实例2 && item3.Buff模板.继承主人属性 != 0)
                        {
                            num18 += (int)((float)宠物实例2.宠物主人[item3.Buff模板.继承主人属性] * item3.继承属性比例);
                        }
                        float num19;
                        num19 = ((item3.Buff模板.伤害转移距离 == 0 || item3.Buff来源 == null || 计算类.网格距离(this.当前坐标, item3.Buff来源.当前坐标) <= item3.Buff模板.伤害转移距离) ? ((float)(item3.Buff模板.不算BUFF层数 ? 1 : item3.当前层数.V) * ((item3.Buff模板.伤害增减系数?.Length > item3.Buff等级.V) ? item3.Buff模板.伤害增减系数[item3.Buff等级.V] : 0f)) : 0f);
                        num15 += ((item3.Buff模板.效果判定方式 == Buff判定方式.主动攻击增伤 || item3.Buff模板.效果判定方式 == Buff判定方式.主增基被减系) ? (num18 + item3.增减伤害基数) : (-(num18 + item3.增减伤害基数)));
                        num16 += ((item3.Buff模板.效果判定方式 == Buff判定方式.主动攻击增伤) ? (num19 + item3.增减伤害系数) : (0f - (num19 + item3.增减伤害系数)));
                        if (item3.Buff模板.数量衰减系数 > 0f)
                        {
                            num15 = (int)((float)(num15 / 命中数量) * item3.Buff模板.数量衰减系数);
                            num16 = (int)(num16 / (float)命中数量 * item3.Buff模板.数量衰减系数);
                        }
                        if (item3.Buff模板.效果判定方式 == Buff判定方式.主增基被减系)
                        {
                            num16 = 0f;
                        }
                        if (item3.Buff模板.生效后接编号 != 0 && item3.Buff来源 != null && 地图处理网关.地图对象表.TryGetValue(item3.Buff来源.地图编号, out var value2) && value2 == item3.Buff来源)
                        {
                            if (item3.Buff模板.后接技能来源)
                            {
                                地图对象2.添加Buff时处理(item3.Buff模板.生效后接编号, item3.Buff来源);
                            }
                            else
                            {
                                this.添加Buff时处理(item3.Buff模板.生效后接编号, item3.Buff来源);
                            }
                        }
                        if (item3.Buff模板.生效增减层数 != 0)
                        {
                            地图对象2.增加Buff层数(item3.Buff编号.V, item3.Buff模板.生效增减层数);
                        }
                        if (item3.Buff模板.生效延长时间 != 0)
                        {
                            item3.剩余时间.V += TimeSpan.FromMilliseconds(item3.Buff模板.生效延长时间);
                            if (item3.Buff模板.延长限制时间 > 0 && item3.剩余时间.V > TimeSpan.FromMilliseconds(item3.Buff模板.延长限制时间))
                            {
                                item3.剩余时间.V = TimeSpan.FromMilliseconds(item3.Buff模板.延长限制时间);
                            }
                            item3.持续时间.V = item3.剩余时间.V;
                            地图对象2.发送封包(new 对象状态变动
                            {
                                对象编号 = 地图对象2.地图编号,
                                Buff编号 = item3.Buff编号.V,
                                Buff索引 = item3.Buff编号.V,
                                当前层数 = item3.当前层数.V,
                                剩余时间 = (int)item3.剩余时间.V.TotalMilliseconds,
                                持续时间 = (int)item3.持续时间.V.TotalMilliseconds
                            });
                        }
                        if (item3.Buff模板.效果生效移除)
                        {
                            地图对象2.移除Buff时处理(item3.Buff编号.V);
                        }
                    }
                    foreach (Buff数据 item4 in this.Buff列表.Values.ToList())
                    {
                        if (this.伤害类型校验(参数.技能伤害类型, item4.Buff模板.受伤类型判定, 0))
                        {
                            if (item4.Buff模板.受伤触发技能 != null && 游戏技能.数据表.TryGetValue(item4.Buff模板.受伤触发技能, out var value3) && 地图对象2.特定类型(this, item4.Buff模板.受伤触发限定))
                            {
                                if (item4.Buff模板.成功移除自身)
                                {
                                    this.移除Buff时处理(item4.Buff编号.V);
                                }
                                new 技能实例(this, value3, null, 0, 地图对象2.当前地图, 地图对象2.当前坐标, 地图对象2, 地图对象2.当前坐标, null);
                            }
                            if (item4.Buff模板.受伤触发BUFF != 0 && 地图对象2.特定类型(this, item4.Buff模板.受伤触发限定))
                            {
                                if (item4.Buff模板.成功移除自身)
                                {
                                    this.移除Buff时处理(item4.Buff编号.V);
                                }
                                this.添加Buff时处理(item4.Buff模板.受伤触发BUFF, this);
                            }
                        }
                        if (!地图对象2.特定类型(this, item4.Buff模板.限定目标类型) || (item4.Buff效果 & Buff效果类型.伤害增减) == 0 || (item4.Buff模板.效果判定方式 != Buff判定方式.被动受伤增伤 && item4.Buff模板.效果判定方式 != Buff判定方式.被动受伤减伤 && item4.Buff模板.效果判定方式 != Buff判定方式.主增基被减系) || (item4.Buff模板.自身血量比例 > 0f && (float)this.当前体力 / (float)this[游戏对象属性.最大体力] > item4.Buff模板.自身血量比例) || (item4.Buff模板.对象血量比例 > 0f && (float)地图对象2.当前体力 / (float)地图对象2[游戏对象属性.最大体力] > item4.Buff模板.对象血量比例))
                        {
                            continue;
                        }
                        bool flag2;
                        flag2 = false;
                        switch (参数.技能伤害类型)
                        {
                            case 技能伤害类型.魔法:
                            case 技能伤害类型.道术:
                                switch (item4.Buff模板.效果判定类型)
                                {
                                    case Buff判定类型.所有技能伤害:
                                    case Buff判定类型.所有魔法伤害:
                                        flag2 = true;
                                        break;
                                    case Buff判定类型.所有特定伤害:
                                        flag2 = item4.Buff模板.特定技能编号.Contains(技能编号);
                                        break;
                                    case Buff判定类型.检测对象BUFF:
                                        flag2 = item4.Buff模板.特定BUFF编号?.Overlaps(this.Buff列表.Keys.ToHashSet()) ?? false;
                                        break;
                                    case Buff判定类型.来源特定伤害:
                                        flag2 = 地图对象2 == item4.Buff来源 && (item4.Buff模板.特定技能编号?.Contains(技能编号) ?? false);
                                        break;
                                    case Buff判定类型.来源技能伤害:
                                    case Buff判定类型.来源魔法伤害:
                                        flag2 = 地图对象2 == item4.Buff来源;
                                        break;
                                }
                                break;
                            case 技能伤害类型.攻击:
                            case 技能伤害类型.刺术:
                            case 技能伤害类型.弓术:
                                switch (item4.Buff模板.效果判定类型)
                                {
                                    case Buff判定类型.所有特定伤害:
                                        flag2 = item4.Buff模板.特定技能编号?.Contains(技能编号) ?? false;
                                        break;
                                    case Buff判定类型.所有技能伤害:
                                    case Buff判定类型.所有物理伤害:
                                        flag2 = true;
                                        break;
                                    case Buff判定类型.检测对象BUFF:
                                        flag2 = item4.Buff模板.特定BUFF编号?.Overlaps(this.Buff列表.Keys.ToHashSet()) ?? false;
                                        break;
                                    case Buff判定类型.来源特定伤害:
                                        flag2 = 地图对象2 == item4.Buff来源 && (item4.Buff模板.特定技能编号?.Contains(技能编号) ?? false);
                                        break;
                                    case Buff判定类型.来源技能伤害:
                                    case Buff判定类型.来源物理伤害:
                                        flag2 = 地图对象2 == item4.Buff来源;
                                        break;
                                }
                                break;
                            case 技能伤害类型.毒性:
                            case 技能伤害类型.神圣:
                            case 技能伤害类型.灼烧:
                            case 技能伤害类型.撕裂:
                                switch (item4.Buff模板.效果判定类型)
                                {
                                    case Buff判定类型.检测对象BUFF:
                                        flag2 = item4.Buff模板.特定BUFF编号?.Overlaps(this.Buff列表.Keys.ToHashSet()) ?? false;
                                        break;
                                    case Buff判定类型.来源特定伤害:
                                        flag2 = 地图对象2 == item4.Buff来源 && (item4.Buff模板.特定技能编号?.Contains(技能编号) ?? false);
                                        break;
                                    case Buff判定类型.所有特定伤害:
                                        flag2 = item4.Buff模板.特定技能编号?.Contains(技能编号) ?? false;
                                        break;
                                }
                                break;
                        }
                        if (!flag2 != item4.Buff模板.效果判定取反)
                        {
                            continue;
                        }
                        int num20;
                        num20 = ((item4.Buff模板.伤害转移距离 == 0 || item4.Buff来源 == null || 计算类.网格距离(this.当前坐标, item4.Buff来源.当前坐标) <= item4.Buff模板.伤害转移距离) ? (item4.当前层数.V * ((item4.Buff模板.伤害转移基数?.Length > item4.Buff等级.V) ? item4.Buff模板.伤害转移基数[item4.Buff等级.V] : 0)) : 0);
                        float num21;
                        num21 = ((item4.Buff模板.伤害转移距离 == 0 || item4.Buff来源 == null || 计算类.网格距离(this.当前坐标, item4.Buff来源.当前坐标) <= item4.Buff模板.伤害转移距离) ? ((float)(int)item4.当前层数.V * ((item4.Buff模板.伤害转移系数?.Length > item4.Buff等级.V) ? item4.Buff模板.伤害转移系数[item4.Buff等级.V] : 0f)) : 0f);
                        int num22;
                        num22 = (item4.Buff模板.不算BUFF层数 ? 1 : item4.当前层数.V) * ((item4.Buff模板.伤害增减基数?.Length > item4.Buff等级.V) ? item4.Buff模板.伤害增减基数[item4.Buff等级.V] : 0);
                        if (this is 宠物实例 宠物实例3 && item4.Buff模板.继承主人属性 != 0)
                        {
                            num22 += (int)((float)宠物实例3.宠物主人[item4.Buff模板.继承主人属性] * item4.继承属性比例);
                        }
                        float num23;
                        num23 = (float)(item4.Buff模板.不算BUFF层数 ? 1 : item4.当前层数.V) * ((item4.Buff模板.伤害增减系数?.Length > item4.Buff等级.V) ? item4.Buff模板.伤害增减系数[item4.Buff等级.V] : 0f);
                        num15 += ((item4.Buff模板.效果判定方式 == Buff判定方式.被动受伤增伤) ? (num22 + item4.增减伤害基数) : (-(num22 + item4.增减伤害基数)));
                        num16 += ((item4.Buff模板.效果判定方式 == Buff判定方式.被动受伤增伤) ? (num23 + item4.增减伤害系数) : (0f - (num23 + item4.增减伤害系数)));
                        if (item4.Buff模板.效果判定方式 == Buff判定方式.主增基被减系)
                        {
                            num15 = 0;
                        }
                        if (item4.Buff来源 != null && (num20 > 0 || num21 > 0f) && 地图对象2.特定类型(this, item4.Buff模板.转移限定类型))
                        {
                            地图对象 item;
                            item = 地图对象2;
                            if (item4.Buff来源 is 宠物实例)
                            {
                                item = item4.Buff来源;
                            }
                            this.转移伤害对象列表.Add(item);
                            this.转移伤害基数列表.Add(num20);
                            this.转移伤害系数列表.Add(num21);
                            this.转移伤害免减列表.Add(item4.Buff模板.转移不扣伤害);
                        }
                        if (item4.Buff模板.生效后接编号 != 0 && item4.Buff来源 != null && 地图处理网关.地图对象表.TryGetValue(item4.Buff来源.地图编号, out var value4) && value4 == item4.Buff来源)
                        {
                            if (item4.Buff模板.后接技能来源)
                            {
                                地图对象2.添加Buff时处理(item4.Buff模板.生效后接编号, item4.Buff来源);
                            }
                            else
                            {
                                this.添加Buff时处理(item4.Buff模板.生效后接编号, item4.Buff来源);
                            }
                        }
                        if (item4.Buff模板.效果判定方式 == Buff判定方式.被动受伤减伤 && item4.Buff模板.限定伤害上限)
                        {
                            num17 = Math.Min(num17, item4.Buff模板.限定伤害数值);
                        }
                        if (item4.Buff模板.生效增减层数 != 0)
                        {
                            this.增加Buff层数(item4.Buff编号.V, item4.Buff模板.生效增减层数);
                        }
                        if (item4.Buff模板.生效延长时间 != 0)
                        {
                            item4.剩余时间.V += TimeSpan.FromMilliseconds(item4.Buff模板.生效延长时间);
                            if (item4.Buff模板.延长限制时间 > 0 && item4.剩余时间.V > TimeSpan.FromMilliseconds(item4.Buff模板.延长限制时间))
                            {
                                item4.剩余时间.V = TimeSpan.FromMilliseconds(item4.Buff模板.延长限制时间);
                            }
                            item4.持续时间.V = item4.剩余时间.V;
                            this.发送封包(new 对象状态变动
                            {
                                对象编号 = this.地图编号,
                                Buff编号 = item4.Buff编号.V,
                                Buff索引 = item4.Buff编号.V,
                                当前层数 = item4.当前层数.V,
                                剩余时间 = (int)item4.剩余时间.V.TotalMilliseconds,
                                持续时间 = (int)item4.持续时间.V.TotalMilliseconds
                            });
                        }
                        if (item4.Buff模板.效果生效移除)
                        {
                            this.移除Buff时处理(item4.Buff编号.V);
                        }
                    }
                    详情.技能伤害 = (int)Math.Min(num17, Math.Max(0f, ((num4 + num6) * (float)num9 + (float)num3 + (float)num5 + (float)num15 - Math.Max(0f, (float)(num10 - num7) - (float)num10 * num8)) * (1f + num16 + num2) * 伤害系数));
                    if (地图对象2 != null && 地图对象2 is 玩家实例 玩家实例11 && 玩家实例11.角色装备.TryGetValue(15, out var v) && v.当前持久.V > 0)
                    {
                        switch (v.物品编号)
                        {
                            case 99999105:
                                详情.技能伤害 += 3;
                                玩家实例11.战具损失持久(1);
                                break;
                            case 99999108:
                                详情.技能伤害++;
                                玩家实例11.战具损失持久(1);
                                break;
                            case 99999104:
                            case 99999109:
                            case 99999116:
                                详情.技能伤害 += 2;
                                玩家实例11.战具损失持久(1);
                                break;
                        }
                    }
                    for (int i = 0; i < this.转移伤害对象列表.Count; i++)
                    {
                        int num24;
                        num24 = this.转移伤害基数列表[i] + (int)(this.转移伤害系数列表[i] * (float)详情.技能伤害);
                        if (!this.转移伤害免减列表[i])
                        {
                            详情.技能伤害 = Math.Max(0, 详情.技能伤害 - num24);
                        }
                        this.转移伤害对象列表[i].发送封包(new 体力变动飘字
                        {
                            血量变化 = -num24,
                            对象编号 = this.转移伤害对象列表[i].地图编号
                        });
                        if ((this.转移伤害对象列表[i].当前体力 = Math.Max(0, this.转移伤害对象列表[i].当前体力 - num24)) == 0)
                        {
                            this.转移伤害对象列表[i].自身死亡处理(地图对象2, 技能击杀: false);
                        }
                        if (详情.技能伤害 == 0 && num == 0)
                        {
                            return;
                        }
                    }
                    int[] 技能吸血基数;
                    技能吸血基数 = 参数.技能吸血基数;
                    if (技能吸血基数 == null || 技能吸血基数.Length <= 技能等级)
                    {
                        float[] 技能吸血系数;
                        技能吸血系数 = 参数.技能吸血系数;
                        if (技能吸血系数 == null || 技能吸血系数.Length <= 技能等级)
                        {
                            goto IL_1b3a;
                        }
                    }
                    int[] 技能吸血基数2;
                    技能吸血基数2 = 参数.技能吸血基数;
                    int num26;
                    num26 = ((技能吸血基数2 != null && 技能吸血基数2.Length > 技能等级) ? 参数.技能吸血基数[技能等级] : 0);
                    float num27;
                    num27 = 详情.技能伤害;
                    float[] 技能吸血系数2;
                    技能吸血系数2 = 参数.技能吸血系数;
                    int num28;
                    num28 = num26 + (int)(num27 * ((技能吸血系数2 != null && 技能吸血系数2.Length > 技能等级) ? 参数.技能吸血系数[技能等级] : 0f));
                    if (num28 > 0)
                    {
                        地图对象2.当前体力 += num28;
                        地图对象2.发送封包(new 体力变动飘字
                        {
                            血量变化 = num28,
                            对象编号 = 地图对象2.地图编号
                        });
                    }
                    goto IL_1b3a;
                }
                详情.技能伤害 = this.当前体力;
            }
            goto IL_1c2b;
        IL_1c2b:
            this.脱战时间 = 主程.当前时间.AddSeconds(10.0);
            地图对象2.脱战时间 = 主程.当前时间.AddSeconds(10.0);
            if ((详情.技能反馈 & 技能命中反馈.闪避) == 0)
            {
                foreach (Buff数据 item5 in this.Buff列表.Values.ToList())
                {
                    if ((item5.Buff效果 & Buff效果类型.状态标志) != 0 && (item5.Buff模板.角色所处状态 & 游戏对象状态.失神状态) != 0)
                    {
                        this.移除Buff时处理(item5.Buff编号.V);
                    }
                    if ((详情.技能伤害 > 0 && item5.Buff分组 == 2535) || item5.Buff分组 == 38400 || item5.Buff分组 == 4238)
                    {
                        if ((item5.剩余时间.V -= TimeSpan.FromSeconds(3.0)) < TimeSpan.Zero)
                        {
                            this.删除Buff时处理(item5.Buff编号.V);
                            continue;
                        }
                        this.发送封包(new 对象状态变动
                        {
                            对象编号 = this.地图编号,
                            Buff编号 = item5.Buff编号.V,
                            Buff索引 = item5.Buff编号.V,
                            当前层数 = item5.当前层数.V,
                            剩余时间 = (int)item5.剩余时间.V.TotalMilliseconds,
                            持续时间 = (int)item5.持续时间.V.TotalMilliseconds
                        });
                    }
                }
            }
            if (this is 怪物实例 怪物实例3)
            {
                if (怪物实例3.怪物级别 != 怪物级别分类.头目首领)
                {
                    怪物实例3.硬直时间 = 主程.当前时间.AddMilliseconds(参数.目标硬直时间);
                }
                if (地图对象2 is 玩家实例 || 地图对象2 is 宠物实例)
                {
                    怪物实例3.对象仇恨.添加仇恨(地图对象2, 主程.当前时间.AddMilliseconds(怪物实例3.仇恨时长), 详情.技能伤害);
                }
            }
            else if (this is 玩家实例 玩家实例12)
            {
                if (详情.技能伤害 > 0)
                {
                    玩家实例12.装备损失持久(详情.技能伤害);
                }
                if (玩家实例12.对象关系(地图对象2) == 游戏对象关系.敌对)
                {
                    foreach (宠物实例 item6 in 玩家实例12.宠物列表.ToList())
                    {
                        if (item6.邻居列表.Contains(地图对象2) && !地图对象2.检查状态(游戏对象状态.隐身状态 | 游戏对象状态.潜行状态))
                        {
                            item6.对象仇恨.添加仇恨(地图对象2, 主程.当前时间.AddMilliseconds(item6.仇恨时长), 0);
                        }
                    }
                }
                if (地图对象2 is 玩家实例 玩家实例13 && !this.当前地图.自由区内(this.当前坐标) && !玩家实例12.灰名玩家 && !玩家实例12.红名玩家)
                {
                    if (玩家实例13.红名玩家)
                    {
                        玩家实例13.减PK时间 = TimeSpan.FromMinutes(1.0);
                    }
                    else if (玩家实例12.所属行会 == null || 玩家实例13.所属行会 == null || !玩家实例12.所属行会.敌对行会.ContainsKey(玩家实例13.所属行会))
                    {
                        玩家实例13.灰名时间 = TimeSpan.FromMinutes(1.0);
                    }
                }
                else if (地图对象2 is 宠物实例 宠物实例4 && !this.当前地图.自由区内(this.当前坐标) && !玩家实例12.灰名玩家 && !玩家实例12.红名玩家)
                {
                    if (宠物实例4.宠物主人.红名玩家)
                    {
                        宠物实例4.宠物主人.减PK时间 = TimeSpan.FromMinutes(1.0);
                    }
                    else if (玩家实例12.所属行会 == null || 宠物实例4.宠物主人.所属行会 == null || !玩家实例12.所属行会.敌对行会.ContainsKey(宠物实例4.宠物主人.所属行会))
                    {
                        宠物实例4.宠物主人.灰名时间 = TimeSpan.FromMinutes(1.0);
                    }
                }
            }
            else if (this is 宠物实例 宠物实例5)
            {
                if (地图对象2 != 宠物实例5.宠物主人 && 宠物实例5.对象关系(地图对象2) == 游戏对象关系.敌对)
                {
                    foreach (宠物实例 item7 in 宠物实例5.宠物主人?.宠物列表.ToList())
                    {
                        if (item7.邻居列表.Contains(地图对象2) && !地图对象2.检查状态(游戏对象状态.隐身状态 | 游戏对象状态.潜行状态))
                        {
                            item7.对象仇恨.添加仇恨(地图对象2, 主程.当前时间.AddMilliseconds(item7.仇恨时长), 0);
                        }
                    }
                }
                if (地图对象2 != 宠物实例5.宠物主人 && 地图对象2 is 玩家实例 玩家实例14 && !this.当前地图.自由区内(this.当前坐标) && !宠物实例5.宠物主人.灰名玩家 && !宠物实例5.宠物主人.红名玩家 && (宠物实例5.宠物主人.所属行会 == null || 玩家实例14.所属行会 == null || !宠物实例5.宠物主人.所属行会.敌对行会.ContainsKey(玩家实例14.所属行会)))
                {
                    玩家实例14.灰名时间 = TimeSpan.FromMinutes(1.0);
                }
            }
            else if (this is 守卫实例 守卫实例2 && 守卫实例2.对象关系(地图对象2) == 游戏对象关系.敌对)
            {
                守卫实例2.对象仇恨.添加仇恨(地图对象2, default(DateTime), 0);
            }
            if (地图对象2 is 玩家实例 玩家实例15)
            {
                if (玩家实例15.对象关系(this) == 游戏对象关系.敌对 && !this.检查状态(游戏对象状态.隐身状态 | 游戏对象状态.潜行状态))
                {
                    foreach (宠物实例 item8 in 玩家实例15.宠物列表.ToList())
                    {
                        if (item8.邻居列表.Contains(this))
                        {
                            item8.对象仇恨.添加仇恨(this, 主程.当前时间.AddMilliseconds(item8.仇恨时长), 参数.增加宠物仇恨 ? 详情.技能伤害 : 0);
                        }
                    }
                }
                if (主程.当前时间 > 玩家实例15.战具计时 && !玩家实例15.对象死亡 && 玩家实例15.当前体力 < 玩家实例15[游戏对象属性.最大体力] && 玩家实例15.角色装备.TryGetValue(15, out var v2) && v2.当前持久.V > 0 && (v2.物品编号 == 99999106 || v2.物品编号 == 99999107))
                {
                    玩家实例15.当前体力 += ((this is 怪物实例) ? 20 : 10);
                    玩家实例15.战具损失持久(1);
                    玩家实例15.战具计时 = 主程.当前时间.AddMilliseconds(1000.0);
                }
            }
            if (this.检查状态(游戏对象状态.招架状态) && (参数.技能伤害类型 == 技能伤害类型.攻击 || 参数.技能伤害类型 == 技能伤害类型.刺术))
            {
                详情.技能反馈 = 技能命中反馈.招架;
            }
            if (this.检查状态(游戏对象状态.护盾状态, out var buff))
            {
                if (buff.护盾数值.V > 详情.技能伤害)
                {
                    buff.护盾数值.V -= 详情.技能伤害;
                    详情.招架伤害 = (ushort)详情.技能伤害;
                    详情.技能伤害 = 0;
                }
                else
                {
                    详情.招架伤害 = (ushort)(详情.技能伤害 - buff.护盾数值.V);
                    buff.护盾数值.V = 0;
                    详情.技能伤害 -= 详情.招架伤害;
                    this.移除Buff时处理(buff.Buff编号.V);
                }
                详情.技能反馈 |= 技能命中反馈.护盾;
            }
            详情.技能伤害 += num;
            if (技能来源 is 玩家实例 玩家实例16 && 主程.当前时间 > 玩家实例16.物理击回间隔 && (参数.技能伤害类型 == 技能伤害类型.攻击 || 参数.技能伤害类型 == 技能伤害类型.刺术 || 参数.技能伤害类型 == 技能伤害类型.弓术) && 玩家实例16.物理击回 > 0)
            {
                玩家实例16.物理击回间隔 = 主程.当前时间.AddSeconds(1.0);
                玩家实例16.当前体力 += 玩家实例16.物理击回;
                玩家实例16.发送封包(new 体力变动飘字
                {
                    血量变化 = 玩家实例16.物理击回,
                    对象编号 = 玩家实例16.地图编号
                });
            }
            if (技能来源 is 玩家实例 玩家实例17 && 主程.当前时间 > 玩家实例17.魔法击回间隔 && (参数.技能伤害类型 == 技能伤害类型.魔法 || 参数.技能伤害类型 == 技能伤害类型.道术) && 玩家实例17.魔法击回 > 0)
            {
                玩家实例17.魔法击回间隔 = 主程.当前时间.AddSeconds(1.0);
                玩家实例17.当前体力 += 玩家实例17.魔法击回;
                玩家实例17.发送封包(new 体力变动飘字
                {
                    血量变化 = 玩家实例17.魔法击回,
                    对象编号 = 玩家实例17.地图编号
                });
            }
            if ((this.当前体力 = Math.Max(0, this.当前体力 - 详情.技能伤害)) == 0)
            {
                详情.技能反馈 |= 技能命中反馈.死亡;
                this.自身死亡处理(地图对象2, 技能击杀: true);
            }
            return;
        IL_1b3a:
            int[] 技能吸蓝基数;
            技能吸蓝基数 = 参数.技能吸蓝基数;
            if (技能吸蓝基数 == null || 技能吸蓝基数.Length <= 技能等级)
            {
                float[] 技能吸蓝系数;
                技能吸蓝系数 = 参数.技能吸蓝系数;
                if (技能吸蓝系数 == null || 技能吸蓝系数.Length <= 技能等级)
                {
                    goto IL_1c2b;
                }
            }
            int[] 技能吸蓝基数2;
            技能吸蓝基数2 = 参数.技能吸蓝基数;
            int num30;
            num30 = ((技能吸蓝基数2 != null && 技能吸蓝基数2.Length > 技能等级) ? 参数.技能吸蓝基数[技能等级] : 0);
            float num31;
            num31 = 详情.技能伤害;
            float[] 技能吸蓝系数2;
            技能吸蓝系数2 = 参数.技能吸蓝系数;
            int num32;
            num32 = num30 + (int)(num31 * ((技能吸蓝系数2 != null && 技能吸蓝系数2.Length > 技能等级) ? 参数.技能吸蓝系数[技能等级] : 0f));
            if (num32 > 0)
            {
                if (!参数.不吸目标蓝量)
                {
                    地图对象2.当前魔力 += num32;
                }
                if (!参数.不扣目标蓝量)
                {
                    if (参数.蓝不足转伤害 && this.当前魔力 < num32)
                    {
                        详情.技能伤害 += num32 - this.当前魔力;
                        this.当前魔力 = 0;
                    }
                    else
                    {
                        this.当前魔力 -= num32;
                    }
                }
            }
            goto IL_1c2b;
        }

        public bool 伤害类型校验(技能伤害类型 技能伤害类型, Buff判定类型 效果判定类型, ushort 技能编号 = 0, HashSet<ushort> 特定技能编号 = null, HashSet<ushort> 特定BUFF编号 = null)
        {
            switch (技能伤害类型)
            {
                case 技能伤害类型.魔法:
                case 技能伤害类型.道术:
                    switch (效果判定类型)
                    {
                        case Buff判定类型.检测对象BUFF:
                            return 特定BUFF编号?.Overlaps(this.Buff列表.Keys.ToHashSet()) ?? false;
                        case Buff判定类型.所有技能伤害:
                        case Buff判定类型.所有魔法伤害:
                            return true;
                        case Buff判定类型.所有特定伤害:
                            return 特定技能编号?.Contains(技能编号) ?? false;
                    }
                    break;
                case 技能伤害类型.攻击:
                case 技能伤害类型.刺术:
                case 技能伤害类型.弓术:
                    switch (效果判定类型)
                    {
                        case Buff判定类型.检测对象BUFF:
                            return 特定BUFF编号?.Overlaps(this.Buff列表.Keys.ToHashSet()) ?? false;
                        case Buff判定类型.所有特定伤害:
                            return 特定技能编号?.Contains(技能编号) ?? false;
                        case Buff判定类型.所有技能伤害:
                        case Buff判定类型.所有物理伤害:
                            return true;
                    }
                    break;
                case 技能伤害类型.毒性:
                case 技能伤害类型.神圣:
                case 技能伤害类型.灼烧:
                case 技能伤害类型.撕裂:
                    switch (效果判定类型)
                    {
                        case Buff判定类型.所有特定伤害:
                            return 特定技能编号?.Contains(技能编号) ?? false;
                        case Buff判定类型.检测对象BUFF:
                            return 特定BUFF编号?.Overlaps(this.Buff列表.Keys.ToHashSet()) ?? false;
                    }
                    break;
            }
            return false;
        }

        public void 被动受伤时处理(Buff数据 数据)
        {
            foreach (技能实例 item in this.技能任务)
            {
                if (item.动作打断)
                {
                    item.是否中断 = true;
                }
            }
            int num;
            num = 0;
            switch (数据.伤害类型)
            {
                case 技能伤害类型.魔法:
                case 技能伤害类型.道术:
                    num = 计算类.计算防御(this[游戏对象属性.最小魔防], this[游戏对象属性.最大魔防]);
                    break;
                case 技能伤害类型.攻击:
                case 技能伤害类型.刺术:
                case 技能伤害类型.弓术:
                    num = 计算类.计算防御(this[游戏对象属性.最小防御], this[游戏对象属性.最大防御]);
                    break;
            }
            int num2;
            num2 = Math.Max(0, 数据.伤害基数.V * 数据.当前层数.V - num);
            if (数据.Buff模板.判断BUFF增伤 > 0 && 数据.Buff来源 != null && 数据.Buff来源.Buff列表.ContainsKey(数据.Buff模板.判断BUFF增伤))
            {
                num2 += 数据.Buff模板.BUFF增伤数值;
            }
            this.当前体力 = Math.Max(0, this.当前体力 - num2);
            this.发送封包(new 触发状态效果
            {
                Buff编号 = 数据.Buff编号.V,
                Buff来源 = (数据.Buff来源?.地图编号 ?? 0),
                Buff目标 = this.地图编号,
                血量变化 = -num2
            });
            if (this.当前体力 == 0)
            {
                this.自身死亡处理(数据.Buff来源, 技能击杀: false);
            }
        }

        public void 被动回复时处理(技能实例 技能, C_05_计算目标回复 参数)
        {
            if (this.对象死亡 || this.当前地图 != 技能.技能来源.当前地图 || (this != 技能.技能来源 && !this.邻居列表.Contains(技能.技能来源)))
            {
                return;
            }
            地图对象 地图对象2;
            地图对象2 = ((技能.技能来源 is 陷阱实例 陷阱实例2) ? 陷阱实例2.陷阱来源 : 技能.技能来源);
            int num;
            num = ((参数.体力回复次数?.Length > 技能.技能等级) ? 参数.体力回复次数[技能.技能等级] : 0);
            int num2;
            num2 = ((参数.体力回复基数?.Length > 技能.技能等级) ? 参数.体力回复基数[技能.技能等级] : 0);
            float num3;
            num3 = ((参数.道术叠加次数?.Length > 技能.技能等级) ? 参数.道术叠加次数[技能.技能等级] : 0f);
            float num4;
            num4 = ((参数.道术叠加基数?.Length > 技能.技能等级) ? 参数.道术叠加基数[技能.技能等级] : 0f);
            int num5;
            num5 = ((参数.立即回复基数?.Length > 技能.技能等级 && 地图对象2 == this) ? 参数.立即回复基数[技能.技能等级] : 0);
            float num6;
            num6 = ((!(参数.立即回复系数?.Length > 技能.技能等级) || 地图对象2 != this) ? 0f : 参数.立即回复系数[技能.技能等级]);
            if (num3 > 0f)
            {
                num += (int)(num3 * (float)计算类.计算攻击(地图对象2[游戏对象属性.最小道术], 地图对象2[游戏对象属性.最大道术], 地图对象2[游戏对象属性.幸运等级]));
            }
            if (num4 > 0f)
            {
                num2 += (int)(num4 * (float)计算类.计算攻击(地图对象2[游戏对象属性.最小道术], 地图对象2[游戏对象属性.最大道术], 地图对象2[游戏对象属性.幸运等级]));
            }
            if (num5 > 0)
            {
                this.当前体力 += num5;
            }
            if (num6 > 0f)
            {
                this.当前体力 += (int)((float)this[游戏对象属性.最大体力] * num6);
            }
            if (num > this.治疗次数 && num2 > 0)
            {
                this.治疗次数 = (byte)num;
                if (技能.技能来源 is 玩家实例 玩家实例2)
                {
                    this.治疗基数 = (int)((float)(num2 * num) * (1f + 玩家实例2.获取龙卫增伤系数(this, 技能.技能编号, 技能.铭文编号)) / (float)num);
                }
                else
                {
                    this.治疗基数 = num2;
                }
                this.治疗时间 = 主程.当前时间.AddMilliseconds(500.0);
            }
        }

        public void 被动回复时处理(Buff数据 数据)
        {
            if (数据.Buff模板.体力回复基数 != null && 数据.Buff模板.体力回复基数.Length > 数据.Buff等级.V)
            {
                byte b;
                b = 数据.Buff模板.体力回复基数[数据.Buff等级.V];
                this.当前体力 += b;
                this.发送封包(new 触发状态效果
                {
                    Buff编号 = 数据.Buff编号.V,
                    Buff来源 = (数据.Buff来源?.地图编号 ?? 0),
                    Buff目标 = this.地图编号,
                    血量变化 = b
                });
            }
            if (数据.Buff模板.体力回复系数 != null && 数据.Buff模板.体力回复系数.Length > 数据.Buff等级.V)
            {
                int num;
                num = (int)((float)this[游戏对象属性.最大体力] * 数据.Buff模板.体力回复系数[数据.Buff等级.V]);
                this.当前体力 += num;
                this.发送封包(new 触发状态效果
                {
                    Buff编号 = 数据.Buff编号.V,
                    Buff来源 = (数据.Buff来源?.地图编号 ?? 0),
                    Buff目标 = this.地图编号,
                    血量变化 = num
                });
            }
            if (数据.Buff模板.魔力回复基数 != null && 数据.Buff模板.魔力回复基数.Length > 数据.Buff等级.V)
            {
                byte b2;
                b2 = 数据.Buff模板.魔力回复基数[数据.Buff等级.V];
                this.当前魔力 += b2;
            }
            if (数据.Buff模板.魔力回复系数 != null && 数据.Buff模板.魔力回复系数.Length > 数据.Buff等级.V)
            {
                int num2;
                num2 = (int)((float)this[游戏对象属性.最大魔力] * 数据.Buff模板.魔力回复系数[数据.Buff等级.V]);
                this.当前魔力 += num2;
            }
        }

        public void 扣除护盾时间(int 技能伤害)
        {
            foreach (Buff数据 item in this.Buff列表.Values.ToList())
            {
                if (item.Buff分组 == 2535 || item.Buff分组 == 38400 || item.Buff分组 == 4238)
                {
                    if ((item.剩余时间.V -= TimeSpan.FromSeconds(3.0)) < TimeSpan.Zero)
                    {
                        this.删除Buff时处理(item.Buff编号.V);
                        continue;
                    }
                    this.发送封包(new 对象状态变动
                    {
                        对象编号 = this.地图编号,
                        Buff编号 = item.Buff编号.V,
                        Buff索引 = item.Buff编号.V,
                        当前层数 = item.当前层数.V,
                        剩余时间 = (int)item.剩余时间.V.TotalMilliseconds,
                        持续时间 = (int)item.持续时间.V.TotalMilliseconds
                    });
                }
            }
        }

        public void 自身移动时处理(Point 坐标)
        {
            if (this is 玩家实例 玩家实例2)
            {
                foreach (技能实例 item in this.技能任务)
                {
                    if (item.动作打断)
                    {
                        item.是否中断 = true;
                    }
                }
                if (玩家实例2.操作道具)
                {
                    玩家实例2.探索道具.道具.Stop(玩家实例2.探索道具);
                }
                玩家实例2.当前交易?.结束交易();
                foreach (Buff数据 item2 in this.Buff列表.Values.ToList())
                {
                    if ((item2.Buff效果 & Buff效果类型.释放技能) != 0 && item2.Buff模板.移动释放技能 != null && 游戏技能.数据表.TryGetValue(item2.Buff模板.移动释放技能, out var value))
                    {
                        new 技能实例(this, value, null, 0, this.当前地图, this.当前坐标, null, this.当前坐标, null);
                    }
                    if ((item2.Buff效果 & Buff效果类型.创建陷阱) != 0 && 技能陷阱.数据表.TryGetValue(item2.Buff模板.触发陷阱技能, out var 陷阱模板2))
                    {
                        int num;
                        num = 0;
                        while (true)
                        {
                            Point point;
                            point = 计算类.前方坐标(this.当前坐标, 坐标, num);
                            if (point == 坐标)
                            {
                                break;
                            }
                            Point[] array;
                            array = 计算类.技能范围(point, this.当前方向, item2.Buff模板.触发陷阱数量);
                            foreach (Point 坐标2 in array)
                            {
                                if (!this.当前地图.地形阻塞(坐标2) && this.当前地图[坐标2].FirstOrDefault((地图对象 O) => O is 陷阱实例 { 陷阱分组编号: not 0 } 陷阱实例3 && 陷阱实例3.陷阱分组编号 == 陷阱模板2.分组编号) == null)
                                {
                                    this.陷阱列表.Add(new 陷阱实例(this, 陷阱模板2, this.当前地图, 坐标2));
                                }
                            }
                            num++;
                        }
                    }
                    if ((item2.Buff效果 & Buff效果类型.状态标志) != 0 && (item2.Buff模板.角色所处状态 & 游戏对象状态.隐身状态) != 0)
                    {
                        this.移除Buff时处理(item2.Buff编号.V);
                    }
                }
            }
            else if (this is 宠物实例)
            {
                foreach (Buff数据 item3 in this.Buff列表.Values.ToList())
                {
                    if ((item3.Buff效果 & Buff效果类型.创建陷阱) != 0 && 技能陷阱.数据表.TryGetValue(item3.Buff模板.触发陷阱技能, out var 陷阱模板))
                    {
                        int num2;
                        num2 = 0;
                        while (true)
                        {
                            Point point2;
                            point2 = 计算类.前方坐标(this.当前坐标, 坐标, num2);
                            if (point2 == 坐标)
                            {
                                break;
                            }
                            Point[] array;
                            array = 计算类.技能范围(point2, this.当前方向, item3.Buff模板.触发陷阱数量);
                            foreach (Point 坐标3 in array)
                            {
                                if (!this.当前地图.地形阻塞(坐标3) && this.当前地图[坐标3].FirstOrDefault((地图对象 O) => O is 陷阱实例 { 陷阱分组编号: not 0 } 陷阱实例2 && 陷阱实例2.陷阱分组编号 == 陷阱模板.分组编号) == null)
                                {
                                    this.陷阱列表.Add(new 陷阱实例(this, 陷阱模板, this.当前地图, 坐标3));
                                }
                            }
                            num2++;
                        }
                    }
                    if ((item3.Buff效果 & Buff效果类型.状态标志) != 0 && (item3.Buff模板.角色所处状态 & 游戏对象状态.隐身状态) != 0)
                    {
                        this.移除Buff时处理(item3.Buff编号.V);
                    }
                }
            }
            this.解绑网格();
            this.当前坐标 = 坐标;
            this.绑定网格();
            this.更新邻居时处理();
            foreach (地图对象 item4 in this.邻居列表.ToList())
            {
                item4.对象移动时处理(this);
            }
        }

        public void 清空邻居时处理()
        {
            foreach (地图对象 item in this.邻居列表.ToList())
            {
                item.对象消失时处理(this);
            }
            this.邻居列表.Clear();
            this.重要邻居.Clear();
            this.潜行邻居.Clear();
        }

        public void 更新邻居时处理()
        {
            foreach (地图对象 item in this.邻居列表.ToList())
            {
                if (this.当前地图 != item.当前地图 || !this.在视线内(item))
                {
                    item.对象消失时处理(this);
                    this.对象消失时处理(item);
                }
            }
            for (int i = -12; i <= 12; i++)
            {
                for (int j = -12; j <= 12; j++)
                {
                    try
                    {
                        foreach (地图对象 item2 in this.当前地图[new Point(this.当前坐标.X + i, this.当前坐标.Y + j)])
                        {
                            if (item2 != this)
                            {
                                if (!this.邻居列表.Contains(item2) && this.邻居类型(item2))
                                {
                                    this.对象出现时处理(item2);
                                }
                                if (!item2.邻居列表.Contains(this) && item2.邻居类型(this))
                                {
                                    item2.对象出现时处理(this);
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        public void 对象移动时处理(地图对象 对象)
        {
            if (!(this is 物品实例))
            {
                if (this is 玩家实例 && 对象 is 玩家实例 玩家实例2 && this.网格距离(对象) < 3 && !this.同一阵营(对象))
                {
                    if (玩家实例2.角色职业 == 游戏对象职业.刺客)
                    {
                        foreach (Buff数据 item in 玩家实例2.Buff列表.Values.ToList())
                        {
                            if ((item.Buff效果 & Buff效果类型.状态标志) != 0 && (item.Buff模板.角色所处状态 & 游戏对象状态.潜行状态) != 0)
                            {
                                玩家实例2.移除Buff时处理(item.Buff编号.V);
                            }
                        }
                    }
                }
                else if (this is 宠物实例 宠物实例2)
                {
                    对象仇恨.仇恨详情 value;
                    if (宠物实例2.主动攻击(对象) && this.网格距离(对象) <= 宠物实例2.仇恨范围 && !对象.检查状态(游戏对象状态.隐身状态 | 游戏对象状态.潜行状态))
                    {
                        宠物实例2.对象仇恨.添加仇恨(对象, default(DateTime), 0);
                    }
                    else if (this.网格距离(对象) > 宠物实例2.仇恨范围 && 宠物实例2.对象仇恨.仇恨列表.TryGetValue(对象, out value) && value.仇恨时间 < 主程.当前时间)
                    {
                        宠物实例2.对象仇恨.移除仇恨(对象);
                    }
                }
                else if (this is 怪物实例 怪物实例2)
                {
                    对象仇恨.仇恨详情 value2;
                    if (this.网格距离(对象) <= 怪物实例2.仇恨范围 && 怪物实例2.主动攻击(对象) && (怪物实例2.可见隐身目标 || !对象.检查状态(游戏对象状态.隐身状态 | 游戏对象状态.潜行状态)))
                    {
                        怪物实例2.对象仇恨.添加仇恨(对象, default(DateTime), 0);
                    }
                    else if (this.网格距离(对象) > 怪物实例2.仇恨范围 && 怪物实例2.对象仇恨.仇恨列表.TryGetValue(对象, out value2) && value2.仇恨时间 < 主程.当前时间)
                    {
                        怪物实例2.对象仇恨.移除仇恨(对象);
                    }
                }
                else if (this is 陷阱实例 陷阱实例2)
                {
                    if (计算类.技能范围(陷阱实例2.当前坐标, 陷阱实例2.当前方向, 陷阱实例2.对象体型).Contains(对象.当前坐标))
                    {
                        陷阱实例2.被动触发陷阱(对象);
                    }
                }
                else if (this is 守卫实例 守卫实例2)
                {
                    if (守卫实例2.主动攻击(对象) && this.网格距离(对象) <= 守卫实例2.仇恨范围)
                    {
                        守卫实例2.对象仇恨.添加仇恨(对象, default(DateTime), 0);
                    }
                    else if (this.网格距离(对象) > 守卫实例2.仇恨范围)
                    {
                        守卫实例2.对象仇恨.移除仇恨(对象);
                    }
                }
            }
            if (对象 is 物品实例)
            {
                return;
            }
            if (对象 is 宠物实例 宠物实例3)
            {
                对象仇恨.仇恨详情 value3;
                if (宠物实例3.网格距离(this) <= 宠物实例3.仇恨范围 && 宠物实例3.主动攻击(this) && !this.检查状态(游戏对象状态.隐身状态 | 游戏对象状态.潜行状态))
                {
                    宠物实例3.对象仇恨.添加仇恨(this, default(DateTime), 0);
                }
                else if (宠物实例3.网格距离(this) > 宠物实例3.仇恨范围 && 宠物实例3.对象仇恨.仇恨列表.TryGetValue(this, out value3) && value3.仇恨时间 < 主程.当前时间)
                {
                    宠物实例3.对象仇恨.移除仇恨(this);
                }
            }
            else if (对象 is 怪物实例 怪物实例3)
            {
                对象仇恨.仇恨详情 value4;
                if (怪物实例3.网格距离(this) <= 怪物实例3.仇恨范围 && 怪物实例3.主动攻击(this) && (怪物实例3.可见隐身目标 || !this.检查状态(游戏对象状态.隐身状态 | 游戏对象状态.潜行状态)))
                {
                    怪物实例3.对象仇恨.添加仇恨(this, default(DateTime), 0);
                }
                else if (怪物实例3.网格距离(this) > 怪物实例3.仇恨范围 && 怪物实例3.对象仇恨.仇恨列表.TryGetValue(this, out value4) && value4.仇恨时间 < 主程.当前时间)
                {
                    怪物实例3.对象仇恨.移除仇恨(this);
                }
            }
            else if (对象 is 陷阱实例 陷阱实例3)
            {
                if (计算类.技能范围(陷阱实例3.当前坐标, 陷阱实例3.当前方向, 陷阱实例3.对象体型).Contains(this.当前坐标))
                {
                    陷阱实例3.被动触发陷阱(this);
                }
            }
            else if (对象 is 守卫实例 守卫实例3)
            {
                if (守卫实例3.主动攻击(this) && 守卫实例3.网格距离(this) <= 守卫实例3.仇恨范围)
                {
                    守卫实例3.对象仇恨.添加仇恨(this, default(DateTime), 0);
                }
                else if (守卫实例3.网格距离(this) > 守卫实例3.仇恨范围)
                {
                    守卫实例3.对象仇恨.移除仇恨(this);
                }
            }
        }

        public void 对象出现时处理(地图对象 对象)
        {
            if (this.潜行邻居.Remove(对象))
            {
                if (this is 物品实例)
                {
                    return;
                }
                if (this is 玩家实例 玩家实例2)
                {
                    switch (对象.对象类型)
                    {
                        case 游戏对象类型.宠物:
                            玩家实例2.网络连接?.发送封包(new 对象角色停止
                            {
                                对象编号 = 对象.地图编号,
                                对象坐标 = 对象.当前坐标,
                                对象高度 = 对象.当前高度
                            });
                            玩家实例2.网络连接?.发送封包(new 对象进入视野
                            {
                                出现方式 = 1,
                                对象编号 = 对象.地图编号,
                                现身坐标 = 对象.当前坐标,
                                现身高度 = 对象.当前高度,
                                现身方向 = (ushort)对象.当前方向,
                                现身姿态 = (byte)((!对象.对象死亡) ? 1u : 13u),
                                体力比例 = (byte)(对象.当前体力 * 100 / 对象[游戏对象属性.最大体力])
                            });
                            玩家实例2.网络连接?.发送封包(new 同步对象体力
                            {
                                对象编号 = 对象.地图编号,
                                当前体力 = 对象.当前体力,
                                体力上限 = 对象[游戏对象属性.最大体力]
                            });
                            玩家实例2.网络连接?.发送封包(new 对象变换类型
                            {
                                改变类型 = 2,
                                对象编号 = 对象.地图编号
                            });
                            break;
                        case 游戏对象类型.玩家:
                        case 游戏对象类型.怪物:
                        case 游戏对象类型.Npcc:
                            if (对象 is 玩家实例 { 隐身模式: not false })
                            {
                                return;
                            }
                            玩家实例2.网络连接?.发送封包(new 对象角色停止
                            {
                                对象编号 = 对象.地图编号,
                                对象坐标 = 对象.当前坐标,
                                对象高度 = 对象.当前高度
                            });
                            玩家实例2.网络连接?.发送封包(new 对象进入视野
                            {
                                出现方式 = 1,
                                对象编号 = 对象.地图编号,
                                现身坐标 = 对象.当前坐标,
                                现身高度 = 对象.当前高度,
                                现身方向 = (ushort)对象.当前方向,
                                现身姿态 = (byte)((!对象.对象死亡) ? 1u : 13u),
                                体力比例 = (byte)(对象.当前体力 * 100 / 对象[游戏对象属性.最大体力]),
                                补充参数 = (byte)((对象 is 玩家实例 { 灰名玩家: not false }) ? 2u : 0u),
                                坐骑编号 = (byte)((对象 is 玩家实例 玩家实例5 && 玩家实例5.Buff列表.ContainsKey(2555)) ? 玩家实例5.当前坐骑 : 0),
                                传承之力 = (byte)((对象 is 玩家实例 玩家实例6) ? 玩家实例6.传承之力外观 : 0)
                            });
                            玩家实例2.网络连接?.发送封包(new 同步对象体力
                            {
                                对象编号 = 对象.地图编号,
                                当前体力 = 对象.当前体力,
                                体力上限 = 对象[游戏对象属性.最大体力]
                            });
                            break;
                        case 游戏对象类型.陷阱:
                            玩家实例2.网络连接?.发送封包(new 陷阱进入视野
                            {
                                地图编号 = 对象.地图编号,
                                陷阱坐标 = 对象.当前坐标,
                                陷阱高度 = 对象.当前高度,
                                来源编号 = (对象 as 陷阱实例).陷阱来源.地图编号,
                                陷阱编号 = (对象 as 陷阱实例).陷阱编号,
                                持续时间 = (对象 as 陷阱实例).陷阱剩余时间,
                                陷阱方向 = (ushort)(对象 as 陷阱实例).当前方向,
                                目的坐标 = (对象 as 陷阱实例).目的坐标,
                                目的高度 = (对象 as 陷阱实例).目的高度
                            });
                            break;
                        case 游戏对象类型.物品:
                            玩家实例2.网络连接?.发送封包(new 对象掉落物品
                            {
                                对象编号 = (对象 as 物品实例).掉落对象编号,
                                地图编号 = 对象.地图编号,
                                掉落坐标 = 对象.当前坐标,
                                掉落高度 = 对象.当前高度,
                                物品编号 = (对象 as 物品实例).物品编号,
                                物品数量 = (对象 as 物品实例).堆叠数量
                            });
                            break;
                    }
                    if (对象.Buff列表.Count > 0)
                    {
                        玩家实例2.网络连接?.发送封包(new 同步对象Buff
                        {
                            字节描述 = 对象.对象Buff简述()
                        });
                    }
                }
                else if (this is 陷阱实例 陷阱实例2)
                {
                    if (计算类.技能范围(陷阱实例2.当前坐标, 陷阱实例2.当前方向, 陷阱实例2.对象体型).Contains(对象.当前坐标))
                    {
                        陷阱实例2.被动触发陷阱(对象);
                    }
                }
                else if (this is 宠物实例 宠物实例2)
                {
                    对象仇恨.仇恨详情 value;
                    if (this.网格距离(对象) <= 宠物实例2.仇恨范围 && 宠物实例2.主动攻击(对象) && !对象.检查状态(游戏对象状态.隐身状态 | 游戏对象状态.潜行状态))
                    {
                        宠物实例2.对象仇恨.添加仇恨(对象, default(DateTime), 0);
                    }
                    else if (this.网格距离(对象) > 宠物实例2.仇恨范围 && 宠物实例2.对象仇恨.仇恨列表.TryGetValue(对象, out value) && value.仇恨时间 < 主程.当前时间)
                    {
                        宠物实例2.对象仇恨.移除仇恨(对象);
                    }
                }
                else if (this is 怪物实例 怪物实例2)
                {
                    对象仇恨.仇恨详情 value2;
                    if (this.网格距离(对象) <= 怪物实例2.仇恨范围 && 怪物实例2.主动攻击(对象) && (怪物实例2.可见隐身目标 || !对象.检查状态(游戏对象状态.隐身状态 | 游戏对象状态.潜行状态)))
                    {
                        怪物实例2.对象仇恨.添加仇恨(对象, default(DateTime), 0);
                    }
                    else if (this.网格距离(对象) > 怪物实例2.仇恨范围 && 怪物实例2.对象仇恨.仇恨列表.TryGetValue(对象, out value2) && value2.仇恨时间 < 主程.当前时间)
                    {
                        怪物实例2.对象仇恨.移除仇恨(对象);
                    }
                }
            }
            else
            {
                if (!this.邻居列表.Add(对象))
                {
                    return;
                }
                if (对象 is 玩家实例 || 对象 is 宠物实例)
                {
                    this.重要邻居.Add(对象);
                }
                if (this is 物品实例)
                {
                    return;
                }
                if (this is 玩家实例 玩家实例7)
                {
                    switch (对象.对象类型)
                    {
                        case 游戏对象类型.宠物:
                            玩家实例7.网络连接?.发送封包(new 对象角色停止
                            {
                                对象编号 = 对象.地图编号,
                                对象坐标 = 对象.当前坐标,
                                对象高度 = 对象.当前高度
                            });
                            玩家实例7.网络连接?.发送封包(new 对象进入视野
                            {
                                出现方式 = 1,
                                对象编号 = 对象.地图编号,
                                现身坐标 = 对象.当前坐标,
                                现身高度 = 对象.当前高度,
                                现身方向 = (ushort)对象.当前方向,
                                现身姿态 = (byte)((!对象.对象死亡) ? 1u : 13u),
                                体力比例 = (byte)(对象.当前体力 * 100 / 对象[游戏对象属性.最大体力])
                            });
                            玩家实例7.网络连接?.发送封包(new 同步对象体力
                            {
                                对象编号 = 对象.地图编号,
                                当前体力 = 对象.当前体力,
                                体力上限 = 对象[游戏对象属性.最大体力]
                            });
                            玩家实例7.网络连接?.发送封包(new 对象变换类型
                            {
                                改变类型 = 2,
                                对象编号 = 对象.地图编号
                            });
                            break;
                        case 游戏对象类型.玩家:
                        case 游戏对象类型.怪物:
                        case 游戏对象类型.Npcc:
                            if (对象 is 玩家实例 { 隐身模式: not false })
                            {
                                return;
                            }
                            玩家实例7.网络连接?.发送封包(new 对象角色停止
                            {
                                对象编号 = 对象.地图编号,
                                对象坐标 = 对象.当前坐标,
                                对象高度 = 对象.当前高度
                            });
                            玩家实例7.网络连接?.发送封包(new 对象进入视野
                            {
                                出现方式 = 1,
                                对象编号 = 对象.地图编号,
                                现身坐标 = 对象.当前坐标,
                                现身高度 = 对象.当前高度,
                                现身方向 = (ushort)对象.当前方向,
                                现身姿态 = (byte)((!对象.对象死亡) ? 1u : 13u),
                                体力比例 = (byte)(对象.当前体力 * 100 / 对象[游戏对象属性.最大体力]),
                                补充参数 = (byte)((对象 is 玩家实例 { 灰名玩家: not false }) ? 2u : 0u),
                                坐骑编号 = (byte)((对象 is 玩家实例 玩家实例10 && 玩家实例10.Buff列表.ContainsKey(2555)) ? 玩家实例10.当前坐骑 : 0),
                                传承之力 = (byte)((对象 is 玩家实例 玩家实例11) ? 玩家实例11.传承之力外观 : 0)
                            });
                            玩家实例7.网络连接?.发送封包(new 同步对象体力
                            {
                                对象编号 = 对象.地图编号,
                                当前体力 = 对象.当前体力,
                                体力上限 = 对象[游戏对象属性.最大体力]
                            });
                            break;
                        case 游戏对象类型.Chest:
                            玩家实例7.网络连接?.发送封包(new 同步道具列表
                            {
                                ObjectId = 对象.地图编号,
                                Position = 对象.当前坐标,
                                Direction = (ushort)对象.当前方向,
                                Altitude = 对象.当前高度,
                                NPCTemplateId = (对象 as 道具实例).Template.道具编号
                            });
                            (对象 as 道具实例).ActivateObject();
                            break;
                        case 游戏对象类型.陷阱:
                            玩家实例7.网络连接?.发送封包(new 陷阱进入视野
                            {
                                地图编号 = 对象.地图编号,
                                陷阱坐标 = 对象.当前坐标,
                                陷阱高度 = 对象.当前高度,
                                来源编号 = (对象 as 陷阱实例).陷阱来源.地图编号,
                                陷阱编号 = (对象 as 陷阱实例).陷阱编号,
                                持续时间 = (对象 as 陷阱实例).陷阱剩余时间,
                                陷阱方向 = (ushort)(对象 as 陷阱实例).当前方向,
                                目的坐标 = (对象 as 陷阱实例).目的坐标,
                                目的高度 = (对象 as 陷阱实例).目的高度
                            });
                            break;
                        case 游戏对象类型.物品:
                            玩家实例7.网络连接?.发送封包(new 对象掉落物品
                            {
                                对象编号 = (对象 as 物品实例).掉落对象编号,
                                地图编号 = 对象.地图编号,
                                掉落坐标 = 对象.当前坐标,
                                掉落高度 = 对象.当前高度,
                                物品编号 = (对象 as 物品实例).物品编号,
                                物品数量 = (对象 as 物品实例).堆叠数量
                            });
                            break;
                    }
                    if (对象.Buff列表.Count > 0)
                    {
                        玩家实例7.网络连接?.发送封包(new 同步对象Buff
                        {
                            字节描述 = 对象.对象Buff简述()
                        });
                    }
                }
                else if (this is 陷阱实例 陷阱实例3)
                {
                    if (计算类.技能范围(陷阱实例3.当前坐标, 陷阱实例3.当前方向, 陷阱实例3.对象体型).Contains(对象.当前坐标))
                    {
                        陷阱实例3.被动触发陷阱(对象);
                    }
                }
                else if (this is 宠物实例 宠物实例3 && !this.对象死亡)
                {
                    对象仇恨.仇恨详情 value3;
                    if (this.网格距离(对象) <= 宠物实例3.仇恨范围 && 宠物实例3.主动攻击(对象) && !对象.检查状态(游戏对象状态.隐身状态 | 游戏对象状态.潜行状态))
                    {
                        宠物实例3.对象仇恨.添加仇恨(对象, default(DateTime), 0);
                    }
                    else if (this.网格距离(对象) > 宠物实例3.仇恨范围 && 宠物实例3.对象仇恨.仇恨列表.TryGetValue(对象, out value3) && value3.仇恨时间 < 主程.当前时间)
                    {
                        宠物实例3.对象仇恨.移除仇恨(对象);
                    }
                }
                else if (this is 怪物实例 怪物实例3 && !this.对象死亡)
                {
                    对象仇恨.仇恨详情 value4;
                    if (this.网格距离(对象) <= 怪物实例3.仇恨范围 && 怪物实例3.主动攻击(对象) && (怪物实例3.可见隐身目标 || !对象.检查状态(游戏对象状态.隐身状态 | 游戏对象状态.潜行状态)))
                    {
                        怪物实例3.对象仇恨.添加仇恨(对象, default(DateTime), 0);
                    }
                    else if (this.网格距离(对象) > 怪物实例3.仇恨范围 && 怪物实例3.对象仇恨.仇恨列表.TryGetValue(对象, out value4) && value4.仇恨时间 < 主程.当前时间)
                    {
                        怪物实例3.对象仇恨.移除仇恨(对象);
                    }
                    if (this.重要邻居.Count != 0)
                    {
                        怪物实例3.怪物激活处理();
                    }
                }
                else if (this is 守卫实例 守卫实例2 && !this.对象死亡)
                {
                    if (守卫实例2.主动攻击(对象) && this.网格距离(对象) <= 守卫实例2.仇恨范围)
                    {
                        守卫实例2.对象仇恨.添加仇恨(对象, default(DateTime), 0);
                    }
                    else if (this.网格距离(对象) > 守卫实例2.仇恨范围)
                    {
                        守卫实例2.对象仇恨.移除仇恨(对象);
                    }
                    if (this.重要邻居.Count != 0)
                    {
                        守卫实例2.守卫激活处理();
                    }
                }
            }
        }

        public void 对象消失时处理(地图对象 对象)
        {
            if (!this.邻居列表.Remove(对象))
            {
                return;
            }
            this.潜行邻居.Remove(对象);
            this.重要邻居.Remove(对象);
            if (this is 物品实例)
            {
                return;
            }
            if (this is 玩家实例 玩家实例2)
            {
                玩家实例2.网络连接?.发送封包(new 对象离开视野
                {
                    对象编号 = 对象.地图编号
                });
            }
            else if (this is 宠物实例 宠物实例2)
            {
                宠物实例2.对象仇恨.移除仇恨(对象);
            }
            else if (this is 怪物实例 怪物实例2)
            {
                if (!this.对象死亡)
                {
                    怪物实例2.对象仇恨.移除仇恨(对象);
                    if (怪物实例2.重要邻居.Count == 0)
                    {
                        怪物实例2.怪物沉睡处理();
                    }
                }
            }
            else if (this is 守卫实例 守卫实例2 && !this.对象死亡)
            {
                守卫实例2.对象仇恨.移除仇恨(对象);
                if (守卫实例2.重要邻居.Count == 0)
                {
                    守卫实例2.守卫沉睡处理();
                }
            }
        }

        public void 对象死亡时处理(地图对象 对象)
        {
            if (this is 怪物实例 怪物实例2)
            {
                怪物实例2.对象仇恨.移除仇恨(对象);
            }
            else if (this is 宠物实例 宠物实例2)
            {
                宠物实例2.对象仇恨.移除仇恨(对象);
            }
            else if (this is 守卫实例 守卫实例2)
            {
                守卫实例2.对象仇恨.移除仇恨(对象);
            }
        }

        public void 对象隐身时处理(地图对象 对象)
        {
            if (this is 宠物实例 宠物实例2 && 宠物实例2.对象仇恨.仇恨列表.ContainsKey(对象))
            {
                宠物实例2.对象仇恨.移除仇恨(对象);
            }
            if (this is 怪物实例 怪物实例2 && 怪物实例2.对象仇恨.仇恨列表.ContainsKey(对象) && !怪物实例2.可见隐身目标)
            {
                怪物实例2.对象仇恨.移除仇恨(对象);
            }
        }

        public void 对象潜行时处理(地图对象 对象)
        {
            if (this is 宠物实例 宠物实例2)
            {
                if (宠物实例2.对象仇恨.仇恨列表.ContainsKey(对象))
                {
                    宠物实例2.对象仇恨.移除仇恨(对象);
                }
                this.潜行邻居.Add(对象);
            }
            if (this is 怪物实例 { 可见隐身目标: false } 怪物实例2)
            {
                if (怪物实例2.对象仇恨.仇恨列表.ContainsKey(对象))
                {
                    怪物实例2.对象仇恨.移除仇恨(对象);
                }
                this.潜行邻居.Add(对象);
            }
            if (this is 玩家实例 玩家实例2)
            {
                this.潜行邻居.Add(对象);
                玩家实例2.网络连接?.发送封包(new 对象离开视野
                {
                    对象编号 = 对象.地图编号
                });
            }
        }

        public void 对象显隐时处理(地图对象 对象)
        {
            if (this is 宠物实例 宠物实例2)
            {
                对象仇恨.仇恨详情 value;
                if (this.网格距离(对象) <= 宠物实例2.仇恨范围 && 宠物实例2.主动攻击(对象) && !对象.检查状态(游戏对象状态.隐身状态 | 游戏对象状态.潜行状态))
                {
                    宠物实例2.对象仇恨.添加仇恨(对象, default(DateTime), 0);
                }
                else if (this.网格距离(对象) > 宠物实例2.仇恨范围 && 宠物实例2.对象仇恨.仇恨列表.TryGetValue(对象, out value) && value.仇恨时间 < 主程.当前时间)
                {
                    宠物实例2.对象仇恨.移除仇恨(对象);
                }
            }
            if (this is 怪物实例 怪物实例2)
            {
                对象仇恨.仇恨详情 value2;
                if (this.网格距离(对象) <= 怪物实例2.仇恨范围 && 怪物实例2.主动攻击(对象) && (怪物实例2.可见隐身目标 || !对象.检查状态(游戏对象状态.隐身状态 | 游戏对象状态.潜行状态)))
                {
                    怪物实例2.对象仇恨.添加仇恨(对象, default(DateTime), 0);
                }
                else if (this.网格距离(对象) > 怪物实例2.仇恨范围 && 怪物实例2.对象仇恨.仇恨列表.TryGetValue(对象, out value2) && value2.仇恨时间 < 主程.当前时间)
                {
                    怪物实例2.对象仇恨.移除仇恨(对象);
                }
            }
        }

        public void 对象显行时处理(地图对象 对象)
        {
            if (this.潜行邻居.Contains(对象))
            {
                this.对象出现时处理(对象);
            }
        }

        public byte[] 对象Buff详述()
        {
            using MemoryStream memoryStream = new MemoryStream(34);
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write((byte)this.Buff列表.Count);
            foreach (KeyValuePair<ushort, Buff数据> item in this.Buff列表)
            {
                binaryWriter.Write(item.Value.Buff编号.V);
                binaryWriter.Write((int)item.Value.Buff编号.V);
                binaryWriter.Write(item.Value.当前层数.V);
                binaryWriter.Write((int)item.Value.剩余时间.V.TotalMilliseconds);
                binaryWriter.Write((int)item.Value.持续时间.V.TotalMilliseconds);
            }
            return memoryStream.ToArray();
        }

        public byte[] 对象Buff简述()
        {
            using MemoryStream memoryStream = new MemoryStream(34);
            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(this.地图编号);
            int num;
            num = 0;
            foreach (KeyValuePair<ushort, Buff数据> item in this.Buff列表)
            {
                binaryWriter.Write(item.Value.Buff编号.V);
                binaryWriter.Write((int)item.Value.Buff编号.V);
                if (++num >= 5)
                {
                    break;
                }
            }
            return memoryStream.ToArray();
        }

        public void 发送聊天信息(string 消息)
        {
            byte[] 字节描述;
            字节描述 = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                binaryWriter.Write(2415919105u);
                binaryWriter.Write(this.地图编号);
                binaryWriter.Write(1);
                binaryWriter.Write(0);
                binaryWriter.Write(Encoding.UTF8.GetBytes(消息 + "\0"));
                binaryWriter.Write(Encoding.UTF8.GetBytes(this.对象名字));
                binaryWriter.Write((byte)0);
                字节描述 = memoryStream.ToArray();
            }
            this.发送封包(new 接收聊天信息
            {
                字节描述 = 字节描述
            });
        }

        public bool 碰撞陷阱(ushort 陷阱编号)
        {
            return this.当前地图[this.当前坐标].FirstOrDefault((地图对象 x) => x is 陷阱实例 陷阱实例2 && 陷阱实例2.陷阱编号 == 陷阱编号) != null;
        }

        public void 掉落(List<怪物掉落> 列表, 玩家实例 归属玩家, decimal 爆率, 游戏怪物 怪物模板, ushort 模板编号, string Src)
        {
            HashSet<角色数据> 物品归属;
            物品归属 = ((归属玩家.所属队伍 == null) ? new HashSet<角色数据> { 归属玩家.角色数据 } : new HashSet<角色数据>(归属玩家.所属队伍.队伍成员));
            float num;
            num = 计算类.收益衰减(归属玩家.当前等级, this.当前等级);
            int num2;
            num2 = 0;
            int num3;
            num3 = 0;
            List<int> list;
            list = new List<int>();
            Dictionary<掉落条件分组, bool> dictionary;
            dictionary = new Dictionary<掉落条件分组, bool>();
            if (num < 1f)
            {
                foreach (怪物掉落 item in 计算类.RandomSort(列表))
                {
                    if (item.暴率分组 > 0 && list.Contains(item.暴率分组))
                    {
                        continue;
                    }
                    if (item.条件分组 != null)
                    {
                        bool value;
                        value = false;
                        if (!dictionary.TryGetValue(item.条件分组, out value))
                        {
                            value = item.条件分组.检测(归属玩家);
                            dictionary.Add(item.条件分组, value);
                        }
                        if (!value)
                        {
                            continue;
                        }
                    }
                    if (!游戏物品.检索表.TryGetValue(item.物品名字, out var value2))
                    {
                        continue;
                    }
                    int num4;
                    num4 = Math.Max(1, item.掉落概率 - (int)Math.Round((decimal)item.掉落概率 * (Settings.怪物额外爆率 + 爆率)));
                    if (num4 != 1 && 主程.随机数.Next(num4) != num4 / 2)
                    {
                        continue;
                    }
                    int num5;
                    num5 = 主程.随机数.Next(item.最小数量, item.最大数量 + 1);
                    if (num5 == 0)
                    {
                        continue;
                    }
                    if (item.暴率分组 > 0)
                    {
                        list.Add(item.暴率分组);
                    }
                    if (value2.物品持久 == 0)
                    {
                        new 物品实例(value2, null, this.当前地图, this.当前坐标, 物品归属, num5);
                        if (value2.物品编号 == 1)
                        {
                            this.当前地图.金币掉落总数 += num5;
                            num2 = num5;
                        }
                        if (怪物模板 != null)
                        {
                            怪物模板.掉落统计[value2] = (怪物模板.掉落统计.ContainsKey(value2) ? 怪物模板.掉落统计[value2] : 0L) + num5;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < num5; i++)
                        {
                            new 物品实例(value2, null, this.当前地图, this.当前坐标, 物品归属, 1);
                        }
                        this.当前地图.怪物掉落次数 += num5;
                        num3++;
                        if (怪物模板 != null)
                        {
                            怪物模板.掉落统计[value2] = (怪物模板.掉落统计.ContainsKey(value2) ? 怪物模板.掉落统计[value2] : 0L) + num5;
                        }
                    }
                    if (item.公告ID > 0)
                    {
                        if (Src == "")
                        {
                            Src = item.怪物名字;
                        }
                        全服公告.发送(item.公告ID, value2.物品名字, 归属玩家.ToString(), Src);
                    }
                    if (value2.贵重物品 && !value2.爆不通知 && 模板编号 > 0)
                    {
                        网络服务网关.发送公告($"<#P0:<#N:{模板编号}>><#P1:<#PN:{归属玩家.对象名字}>><#P2:<#I:{value2.物品编号}>><#T:MMOGame.TvShowNeedSysNtf.ITEM.10>");
                    }
                }
            }
            if (num2 > 0)
            {
                主窗口.更新地图数据(this.当前地图, "金币掉落总数", num2);
            }
            if (num3 > 0)
            {
                主窗口.更新地图数据(this.当前地图, "怪物掉落次数", num3);
            }
            if ((num2 > 0 || num3 > 0) && 怪物模板 != null)
            {
                主窗口.更新掉落统计(怪物模板, 怪物模板.掉落统计.ToList());
            }
        }
    }
}
