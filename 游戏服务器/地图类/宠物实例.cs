using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using 游戏服务器.模板类;
using 游戏服务器.数据类;
using 游戏服务器.网络类;

namespace 游戏服务器.地图类
{
    public sealed class 宠物实例 : 地图对象
    {
        public 玩家实例 宠物主人;

        public 游戏怪物 对象模板;

        public 对象仇恨 对象仇恨;

        public 游戏技能 普通攻击技能;

        public List<游戏技能> 概率触发技能;

        public 游戏技能 进入战斗技能;

        public 游戏技能 退出战斗技能;

        public 游戏技能 死亡释放技能;

        public 游戏技能 移动释放技能;

        public 游戏技能 出生释放技能;

        public 宠物数据 宠物数据;

        public bool 尸体消失 { get; set; }

        public DateTime 攻击时间 { get; set; }

        public DateTime 漫游时间 { get; set; }

        public DateTime 复活时间 { get; set; }

        public DateTime 消失时间 { get; set; }

        public int 宠物经验
        {
            get
            {
                return this.宠物数据.当前经验.V;
            }
            set
            {
                if (this.宠物数据.当前经验.V != value)
                {
                    this.宠物数据.当前经验.V = value;
                }
            }
        }

        public byte 宠物等级
        {
            get
            {
                return this.宠物数据.当前等级.V;
            }
            set
            {
                if (this.宠物数据.当前等级.V != value)
                {
                    this.宠物数据.当前等级.V = value;
                }
            }
        }

        public byte 等级上限
        {
            get
            {
                return this.宠物数据.等级上限.V;
            }
            set
            {
                if (this.宠物数据.等级上限.V != value)
                {
                    this.宠物数据.等级上限.V = value;
                }
            }
        }

        public bool 绑定武器
        {
            get
            {
                return this.宠物数据.绑定武器.V;
            }
            set
            {
                if (this.宠物数据.绑定武器.V != value)
                {
                    this.宠物数据.绑定武器.V = value;
                }
            }
        }

        public ushort 绑定BUFF
        {
            get
            {
                return this.宠物数据.绑定BUFF.V;
            }
            set
            {
                if (this.宠物数据.绑定BUFF.V != value)
                {
                    this.宠物数据.绑定BUFF.V = value;
                }
            }
        }

        public bool 物品召唤
        {
            get
            {
                return this.宠物数据.物品召唤.V;
            }
            set
            {
                if (this.宠物数据.物品召唤.V != value)
                {
                    this.宠物数据.物品召唤.V = value;
                }
            }
        }

        public DateTime 叛变时间
        {
            get
            {
                return this.宠物数据.叛变时间.V;
            }
            set
            {
                if (this.宠物数据.叛变时间.V != value)
                {
                    this.宠物数据.叛变时间.V = value;
                }
            }
        }

        public DateTime 存活时间
        {
            get
            {
                return this.宠物数据.存活时间.V;
            }
            set
            {
                if (this.宠物数据.存活时间.V != value)
                {
                    this.宠物数据.存活时间.V = value;
                }
            }
        }

        public override int 处理间隔 => 10;

        public override DateTime 忙碌时间
        {
            get
            {
                return base.忙碌时间;
            }
            set
            {
                if (base.忙碌时间 < value)
                {
                    base.忙碌时间 = (this.硬直时间 = value);
                }
            }
        }

        public override DateTime 硬直时间
        {
            get
            {
                return base.硬直时间;
            }
            set
            {
                if (base.硬直时间 < value)
                {
                    base.硬直时间 = value;
                }
            }
        }

        public override int 当前体力
        {
            get
            {
                return this.宠物数据.当前体力.V;
            }
            set
            {
                value = 计算类.数值限制(0, value, this[游戏对象属性.最大体力]);
                if (this.宠物数据.当前体力.V != value)
                {
                    this.宠物数据.当前体力.V = value;
                    base.发送封包(new 同步对象体力
                    {
                        对象编号 = this.地图编号,
                        当前体力 = this.当前体力,
                        体力上限 = this[游戏对象属性.最大体力]
                    });
                }
            }
        }

        public override 地图实例 当前地图
        {
            get
            {
                return base.当前地图;
            }
            set
            {
                if (this.当前地图 != value)
                {
                    base.当前地图?.移除对象(this);
                    base.当前地图 = value;
                    base.当前地图.添加对象(this);
                }
            }
        }

        public override 游戏方向 当前方向
        {
            get
            {
                return base.当前方向;
            }
            set
            {
                if (this.当前方向 != value)
                {
                    base.当前方向 = value;
                    base.发送封包(new 对象转动方向
                    {
                        转向耗时 = 100,
                        对象编号 = this.地图编号,
                        对象朝向 = (ushort)this.当前方向
                    });
                }
            }
        }

        public override byte 当前等级 => this.对象模板.怪物等级;

        public override string 对象名字 => Regex.Replace(this.对象模板.怪物名字, "[\\d-]", string.Empty);

        public override string 完整名字 => this.对象模板.怪物名字;

        public override 游戏对象类型 对象类型 => 游戏对象类型.宠物;

        public override 技能范围类型 对象体型 => this.对象模板.怪物体型;

        public override int this[游戏对象属性 属性]
        {
            get
            {
                return base[属性];
            }
            set
            {
                base[属性] = value;
            }
        }

        public int 仇恨范围 => 4;

        public int 仇恨时长 => 15000;

        public int 切换间隔 => 5000;

        public ushort 模板编号 => this.对象模板.怪物编号;

        public ushort 升级经验
        {
            get
            {
                if (!(角色成长.宠物升级经验?.Length > this.宠物等级))
                {
                    return 10;
                }
                return 角色成长.宠物升级经验[this.宠物等级];
            }
        }

        public int 移动间隔 => this.对象模板.怪物移动间隔;

        public int 漫游间隔 => this.对象模板.怪物漫游间隔;

        public int 尸体保留 => this.对象模板.尸体保留时长;

        public bool 可被技能诱惑 => this.对象模板.可被技能诱惑;

        public float 基础诱惑概率 => this.对象模板.基础诱惑概率;

        public 怪物种族分类 宠物种族 => this.对象模板.怪物分类;

        public 怪物级别分类 宠物级别 => this.对象模板.怪物级别;

        public bool 执行脚本 => this.对象模板.执行脚本;

        public Dictionary<游戏对象属性, int> 基础属性
        {
            get
            {
                if (!(this.对象模板.成长属性?.Length > this.宠物等级))
                {
                    return this.对象模板.基础属性;
                }
                return this.对象模板.成长属性[this.宠物等级];
            }
        }

        public override 字典监视器<ushort, Buff数据> Buff列表 => this.宠物数据.Buff数据;

        public 宠物实例(玩家实例 宠物主人, 宠物数据 对象数据)
        {
            this.宠物主人 = 宠物主人;
            this.宠物数据 = 对象数据;
            this.对象模板 = 游戏怪物.数据表[对象数据.宠物名字.V];
            this.当前坐标 = 宠物主人.当前坐标;
            this.当前地图 = 宠物主人.当前地图;
            this.当前方向 = 计算类.随机方向();
            base.属性加成[this] = this.基础属性;
            base.属性加成[宠物主人.角色数据] = new Dictionary<游戏对象属性, int>();
            if (this.对象模板.继承属性 != null)
            {
                属性继承[] 继承属性;
                继承属性 = this.对象模板.继承属性;
                for (int i = 0; i < 继承属性.Length; i++)
                {
                    属性继承 属性继承;
                    属性继承 = 继承属性[i];
                    base.属性加成[宠物主人.角色数据][属性继承.转换属性] = (int)((float)宠物主人[属性继承.继承属性] * 属性继承.继承比例);
                }
            }
            foreach (Buff数据 value2 in this.Buff列表.Values)
            {
                if ((value2.Buff效果 & Buff效果类型.属性增减) != 0)
                {
                    base.属性加成.Add(value2, value2.属性加成);
                }
            }
            this.更新对象属性();
            base.恢复时间 = 主程.当前时间.AddSeconds(5.0);
            this.攻击时间 = 主程.当前时间.AddSeconds(1.0);
            this.漫游时间 = 主程.当前时间.AddMilliseconds(主程.随机数.Next(5000) + this.漫游间隔);
            this.对象仇恨 = new 对象仇恨();
            string text;
            text = this.对象模板.普通攻击技能;
            if (text != null && text.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.普通攻击技能, out this.普通攻击技能);
            }
            this.概率触发技能 = new List<游戏技能>();
            string[] array;
            array = this.对象模板.概率触发技能;
            foreach (string text2 in array)
            {
                if (text2 != null && text2.Length > 0)
                {
                    游戏技能.数据表.TryGetValue(text2, out var value);
                    this.概率触发技能.Add(value);
                }
            }
            string text3;
            text3 = this.对象模板.进入战斗技能;
            if (text3 != null && text3.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.进入战斗技能, out this.进入战斗技能);
            }
            string text4;
            text4 = this.对象模板.退出战斗技能;
            if (text4 != null && text4.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.退出战斗技能, out this.退出战斗技能);
            }
            string text5;
            text5 = this.对象模板.死亡释放技能;
            if (text5 != null && text5.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.死亡释放技能, out this.死亡释放技能);
            }
            string text6;
            text6 = this.对象模板.移动释放技能;
            if (text6 != null && text6.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.移动释放技能, out this.移动释放技能);
            }
            string text7;
            text7 = this.对象模板.出生释放技能;
            if (text7 != null && text7.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.出生释放技能, out this.出生释放技能);
            }
            this.地图编号 = ++地图处理网关.对象编号;
            地图处理网关.添加地图对象(this);
            base.激活对象 = true;
            地图处理网关.添加激活对象(this);
            this.阻塞网格 = true;
            this.宠物召回处理();
            if (this.出生释放技能 != null)
            {
                new 技能实例(this, this.出生释放技能, null, 0, this.当前地图, this.当前坐标, 宠物主人, this.当前坐标, null);
            }
        }

        public 宠物实例(玩家实例 宠物主人, 游戏怪物 召唤宠物, byte 初始等级, byte 等级上限, bool 绑定武器, ushort 绑定BUFF, int 存活时长)
        {
            this.宠物主人 = 宠物主人;
            this.对象模板 = 召唤宠物;
            this.宠物数据 = new 宠物数据(召唤宠物.怪物名字, 初始等级, 等级上限, 绑定武器, 绑定BUFF, DateTime.MaxValue, 主程.当前时间.AddSeconds(存活时长));
            this.当前坐标 = 宠物主人.当前坐标;
            this.当前地图 = 宠物主人.当前地图;
            this.当前方向 = 计算类.随机方向();
            this.地图编号 = ++地图处理网关.对象编号;
            base.属性加成[this] = this.基础属性;
            base.属性加成[宠物主人.角色数据] = new Dictionary<游戏对象属性, int>();
            if (this.对象模板.继承属性 != null)
            {
                属性继承[] 继承属性;
                继承属性 = this.对象模板.继承属性;
                for (int i = 0; i < 继承属性.Length; i++)
                {
                    属性继承 属性继承;
                    属性继承 = 继承属性[i];
                    base.属性加成[宠物主人.角色数据][属性继承.转换属性] = (int)((float)宠物主人[属性继承.继承属性] * 属性继承.继承比例);
                }
            }
            this.更新对象属性();
            this.当前体力 = this[游戏对象属性.最大体力];
            base.恢复时间 = 主程.当前时间.AddSeconds(5.0);
            this.攻击时间 = 主程.当前时间.AddSeconds(1.0);
            this.漫游时间 = 主程.当前时间.AddMilliseconds(主程.随机数.Next(5000) + this.漫游间隔);
            this.对象仇恨 = new 对象仇恨();
            string text;
            text = this.对象模板.普通攻击技能;
            if (text != null && text.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.普通攻击技能, out this.普通攻击技能);
            }
            this.概率触发技能 = new List<游戏技能>();
            string[] array;
            array = this.对象模板.概率触发技能;
            foreach (string text2 in array)
            {
                if (text2 != null && text2.Length > 0)
                {
                    游戏技能.数据表.TryGetValue(text2, out var value);
                    this.概率触发技能.Add(value);
                }
            }
            string text3;
            text3 = this.对象模板.进入战斗技能;
            if (text3 != null && text3.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.进入战斗技能, out this.进入战斗技能);
            }
            string text4;
            text4 = this.对象模板.退出战斗技能;
            if (text4 != null && text4.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.退出战斗技能, out this.退出战斗技能);
            }
            string text5;
            text5 = this.对象模板.死亡释放技能;
            if (text5 != null && text5.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.死亡释放技能, out this.死亡释放技能);
            }
            string text6;
            text6 = this.对象模板.移动释放技能;
            if (text6 != null && text6.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.移动释放技能, out this.移动释放技能);
            }
            string text7;
            text7 = this.对象模板.出生释放技能;
            if (text7 != null && text7.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.出生释放技能, out this.出生释放技能);
            }
            地图处理网关.添加地图对象(this);
            base.激活对象 = true;
            地图处理网关.添加激活对象(this);
            this.阻塞网格 = true;
            this.宠物召回处理();
            if (this.出生释放技能 != null)
            {
                new 技能实例(this, this.出生释放技能, null, 0, this.当前地图, this.当前坐标, 宠物主人, this.当前坐标, null);
            }
        }

        public 宠物实例(玩家实例 宠物主人, 怪物实例 诱惑怪物, byte 初始等级, bool 绑定武器, int 宠物时长, ushort 绑定BUFF, int 存活时长)
        {
            this.宠物主人 = 宠物主人;
            this.对象模板 = 诱惑怪物.对象模板;
            this.宠物数据 = new 宠物数据(诱惑怪物.对象模板.怪物名字, 初始等级, 7, 绑定武器, 绑定BUFF, 主程.当前时间.AddMinutes(宠物时长), 主程.当前时间.AddSeconds(存活时长));
            this.当前坐标 = 诱惑怪物.当前坐标;
            this.当前地图 = 诱惑怪物.当前地图;
            this.当前方向 = 诱惑怪物.当前方向;
            base.属性加成[this] = this.基础属性;
            this.更新对象属性();
            this.当前体力 = Math.Min(诱惑怪物.当前体力, this[游戏对象属性.最大体力]);
            base.恢复时间 = 主程.当前时间.AddSeconds(5.0);
            this.攻击时间 = 主程.当前时间.AddSeconds(1.0);
            this.忙碌时间 = 主程.当前时间.AddSeconds(1.0);
            this.漫游时间 = 主程.当前时间.AddMilliseconds(this.漫游间隔);
            this.对象仇恨 = new 对象仇恨();
            string text;
            text = this.对象模板.普通攻击技能;
            if (text != null && text.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.普通攻击技能, out this.普通攻击技能);
            }
            this.概率触发技能 = new List<游戏技能>();
            string[] array;
            array = this.对象模板.概率触发技能;
            foreach (string text2 in array)
            {
                if (text2 != null && text2.Length > 0)
                {
                    游戏技能.数据表.TryGetValue(text2, out var value);
                    this.概率触发技能.Add(value);
                }
            }
            string text3;
            text3 = this.对象模板.进入战斗技能;
            if (text3 != null && text3.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.进入战斗技能, out this.进入战斗技能);
            }
            string text4;
            text4 = this.对象模板.退出战斗技能;
            if (text4 != null && text4.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.退出战斗技能, out this.退出战斗技能);
            }
            string text5;
            text5 = this.对象模板.死亡释放技能;
            if (text5 != null && text5.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.死亡释放技能, out this.死亡释放技能);
            }
            string text6;
            text6 = this.对象模板.移动释放技能;
            if (text6 != null && text6.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.移动释放技能, out this.移动释放技能);
            }
            string text7;
            text7 = this.对象模板.出生释放技能;
            if (text7 != null && text7.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.出生释放技能, out this.出生释放技能);
            }
            诱惑怪物.怪物诱惑处理();
            this.地图编号 = ++地图处理网关.对象编号;
            this.阻塞网格 = true;
            base.绑定网格();
            地图处理网关.添加地图对象(this);
            base.激活对象 = true;
            地图处理网关.添加激活对象(this);
            base.更新邻居时处理();
            if (this.出生释放技能 != null)
            {
                new 技能实例(this, this.出生释放技能, null, 0, this.当前地图, this.当前坐标, 宠物主人, this.当前坐标, null);
            }
        }

        public 宠物实例(玩家实例 宠物主人, 宠物实例 诱惑宠物, byte 初始等级, bool 绑定武器, int 宠物时长, ushort 绑定BUFF, int 存活时长)
        {
            this.宠物主人 = 宠物主人;
            this.对象模板 = 诱惑宠物.对象模板;
            this.宠物数据 = new 宠物数据(诱惑宠物.对象名字, 初始等级, 7, 绑定武器, 绑定BUFF, 主程.当前时间.AddMinutes(宠物时长), 主程.当前时间.AddSeconds(存活时长));
            this.当前坐标 = 诱惑宠物.当前坐标;
            this.当前地图 = 诱惑宠物.当前地图;
            this.当前方向 = 诱惑宠物.当前方向;
            base.属性加成[this] = this.基础属性;
            this.更新对象属性();
            this.当前体力 = Math.Min(诱惑宠物.当前体力, this[游戏对象属性.最大体力]);
            base.恢复时间 = 主程.当前时间.AddSeconds(5.0);
            this.攻击时间 = 主程.当前时间.AddSeconds(1.0);
            this.忙碌时间 = 主程.当前时间.AddSeconds(1.0);
            this.漫游时间 = 主程.当前时间.AddMilliseconds(this.漫游间隔);
            this.对象仇恨 = new 对象仇恨();
            string text;
            text = this.对象模板.普通攻击技能;
            if (text != null && text.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.普通攻击技能, out this.普通攻击技能);
            }
            this.概率触发技能 = new List<游戏技能>();
            string[] array;
            array = this.对象模板.概率触发技能;
            foreach (string text2 in array)
            {
                if (text2 != null && text2.Length > 0)
                {
                    游戏技能.数据表.TryGetValue(text2, out var value);
                    this.概率触发技能.Add(value);
                }
            }
            string text3;
            text3 = this.对象模板.进入战斗技能;
            if (text3 != null && text3.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.进入战斗技能, out this.进入战斗技能);
            }
            string text4;
            text4 = this.对象模板.退出战斗技能;
            if (text4 != null && text4.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.退出战斗技能, out this.退出战斗技能);
            }
            string text5;
            text5 = this.对象模板.死亡释放技能;
            if (text5 != null && text5.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.死亡释放技能, out this.死亡释放技能);
            }
            string text6;
            text6 = this.对象模板.移动释放技能;
            if (text6 != null && text6.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.移动释放技能, out this.移动释放技能);
            }
            string text7;
            text7 = this.对象模板.出生释放技能;
            if (text7 != null && text7.Length > 0)
            {
                游戏技能.数据表.TryGetValue(this.对象模板.出生释放技能, out this.出生释放技能);
            }
            诱惑宠物.自身死亡处理(null, 技能击杀: false);
            this.阻塞网格 = true;
            base.绑定网格();
            this.地图编号 = ++地图处理网关.对象编号;
            地图处理网关.添加地图对象(this);
            base.激活对象 = true;
            地图处理网关.添加激活对象(this);
            base.更新邻居时处理();
            if (this.出生释放技能 != null)
            {
                new 技能实例(this, this.出生释放技能, null, 0, this.当前地图, this.当前坐标, 宠物主人, this.当前坐标, null);
            }
        }

        public override void 处理对象数据()
        {
            if (主程.当前时间 < base.预约时间)
            {
                return;
            }
            if (this.对象死亡)
            {
                if (!this.尸体消失 && 主程.当前时间 >= this.消失时间)
                {
                    base.删除对象();
                    this.当前地图?.移除对象(this);
                }
            }
            else if (this.存活时间 != default(DateTime) && 主程.当前时间 > this.存活时间)
            {
                this.自身死亡处理(null, 技能击杀: false);
            }
            else if (this.叛变时间 != default(DateTime) && 主程.当前时间 > this.叛变时间)
            {
                if (Settings.叛变直接死亡)
                    this.自身死亡处理(null, 技能击杀: false);
                else
                    new 怪物实例(this);
            }
            else
            {
                foreach (KeyValuePair<ushort, Buff数据> item in this.Buff列表.ToList())
                {
                    base.轮询Buff时处理(item.Value);
                }
                foreach (技能实例 item2 in base.技能任务.ToList())
                {
                    item2.处理任务();
                }
                if (主程.当前时间 > base.恢复时间)
                {
                    if (!this.检查状态(游戏对象状态.中毒状态))
                    {
                        this.当前体力 += this[游戏对象属性.体力恢复];
                    }
                    base.恢复时间 = 主程.当前时间.AddSeconds(5.0);
                }
                if (主程.当前时间 > base.治疗时间 && base.治疗次数 > 0)
                {
                    base.治疗次数--;
                    base.治疗时间 = 主程.当前时间.AddMilliseconds(500.0);
                    this.当前体力 += base.治疗基数;
                }
                if (this.进入战斗技能 != null && !base.战斗姿态 && this.对象仇恨.仇恨列表.Count != 0)
                {
                    new 技能实例(this, this.进入战斗技能, null, base.动作编号++, this.当前地图, this.当前坐标, null, this.当前坐标, null);
                    base.战斗姿态 = true;
                    base.脱战时间 = 主程.当前时间.AddSeconds(10.0);
                }
                else if (this.退出战斗技能 != null && base.战斗姿态 && this.对象仇恨.仇恨列表.Count == 0 && 主程.当前时间 > base.脱战时间)
                {
                    new 技能实例(this, this.退出战斗技能, null, base.动作编号++, this.当前地图, this.当前坐标, null, this.当前坐标, null);
                    base.战斗姿态 = false;
                }
                else if (this.宠物主人.宠物模式 == 宠物模式.攻击 && 主程.当前时间 > this.忙碌时间 && 主程.当前时间 > this.硬直时间)
                {
                    if (this.对象仇恨.当前目标 == null && !base.邻居列表.Contains(this.宠物主人))
                    {
                        this.宠物智能跟随();
                    }
                    else if (this.更新对象仇恨())
                    {
                        this.宠物智能攻击();
                    }
                    else
                    {
                        this.宠物智能跟随();
                    }
                }
            }
            base.处理对象数据();
        }

        public override void 自身死亡处理(地图对象 对象, bool 技能击杀, bool 脚本击杀 = false)
        {
            if (this.死亡释放技能 != null && 对象 != null)
            {
                new 技能实例(this, this.死亡释放技能, null, base.动作编号++, this.当前地图, this.当前坐标, null, this.当前坐标, null).处理任务();
            }
            if (this.对象模板.死亡主人BUFF > 0 && this.宠物主人 != null)
            {
                this.宠物主人.添加Buff时处理(this.对象模板.死亡主人BUFF, this);
            }
            base.自身死亡处理(对象, 技能击杀);
            this.消失时间 = 主程.当前时间.AddMilliseconds(this.尸体保留);
            this.清空宠物仇恨();
            this.宠物主人?.宠物死亡处理(this);
            this.宠物主人?.宠物数据.Remove(this.宠物数据);
            this.宠物主人?.宠物列表.Remove(this);
            int? num;
            num = this.宠物主人?.宠物数量;
            if ((num.GetValueOrDefault() == 0) & num.HasValue)
            {
                this.宠物主人?.网络连接?.发送封包(new 游戏错误提示
                {
                    错误代码 = 9473
                });
            }
            this.Buff列表.Clear();
            this.宠物数据?.删除数据();
            base.次要对象 = true;
            地图处理网关.添加次要对象(this);
            base.激活对象 = false;
            地图处理网关.移除激活对象(this);
        }

        public void 宠物智能跟随()
        {
            if (!this.能否走动())
            {
                return;
            }
            if (base.邻居列表.Contains(this.宠物主人))
            {
                Point point;
                point = 计算类.前方坐标(this.宠物主人.当前坐标, 计算类.旋转方向(this.宠物主人.当前方向, 4), 2);
                if (base.网格距离(this.宠物主人) <= 2 && base.网格距离(point) <= 2)
                {
                    if (主程.当前时间 > this.漫游时间)
                    {
                        this.漫游时间 = 主程.当前时间.AddMilliseconds(this.漫游间隔 + 主程.随机数.Next(5000));
                        Point point2;
                        point2 = 计算类.前方坐标(this.当前坐标, 计算类.随机方向(), 1);
                        if (this.当前地图.能否通行(point2))
                        {
                            this.忙碌时间 = 主程.当前时间.AddMilliseconds(this.行走耗时);
                            this.行走时间 = 主程.当前时间.AddMilliseconds(this.行走耗时 + this.移动间隔);
                            this.当前方向 = 计算类.计算方向(this.当前坐标, point2);
                            base.自身移动时处理(point2);
                            base.发送封包(new 对象角色走动
                            {
                                对象编号 = this.地图编号,
                                移动坐标 = this.当前坐标,
                                移动速度 = base.行走速度
                            });
                        }
                    }
                    return;
                }
                游戏方向 方向;
                方向 = 计算类.计算方向(this.当前坐标, point);
                for (int i = 0; i < 8; i++)
                {
                    Point point3;
                    point3 = 计算类.前方坐标(this.当前坐标, 方向, 1);
                    if (!this.当前地图.能否通行(point3))
                    {
                        方向 = 计算类.旋转方向(方向, (主程.随机数.Next(2) != 0) ? 1 : (-1));
                        continue;
                    }
                    this.忙碌时间 = 主程.当前时间.AddMilliseconds(this.行走耗时);
                    this.行走时间 = 主程.当前时间.AddMilliseconds(this.行走耗时 + this.移动间隔);
                    this.当前方向 = 计算类.计算方向(this.当前坐标, point3);
                    base.自身移动时处理(point3);
                    base.发送封包(new 对象角色走动
                    {
                        对象编号 = this.地图编号,
                        移动坐标 = this.当前坐标,
                        移动速度 = base.行走速度
                    });
                    break;
                }
            }
            else
            {
                this.宠物召回处理();
            }
        }

        public void 宠物智能攻击()
        {
            if (this.检查状态(游戏对象状态.麻痹状态 | 游戏对象状态.失神状态))
            {
                return;
            }
            游戏技能 游戏技能;
            游戏技能 = null;
            if (this.概率触发技能 != null)
            {
                foreach (游戏技能 item in this.概率触发技能)
                {
                    if (item != null && (!this.冷却记录.ContainsKey(item.自身技能编号 | 0x1000000) || 主程.当前时间 > this.冷却记录[item.自身技能编号 | 0x1000000]) && 计算类.计算概率(this.宠物主人.龙卫技能触发概率(item.自身技能编号) + (item.计算幸运概率 ? 计算类.计算幸运(this[游戏对象属性.幸运等级]) : item.计算触发概率)))
                    {
                        游戏技能 = item;
                        break;
                    }
                }
            }
            if (游戏技能 == null)
            {
                if (this.普通攻击技能 == null || (this.冷却记录.ContainsKey(this.普通攻击技能.自身技能编号 | 0x1000000) && !(主程.当前时间 > this.冷却记录[this.普通攻击技能.自身技能编号 | 0x1000000])))
                {
                    return;
                }
                游戏技能 = this.普通攻击技能;
            }
            if (游戏技能 == null)
            {
                return;
            }
            if (base.网格距离(this.对象仇恨.当前目标) > 游戏技能.技能最远距离)
            {
                if (!this.能否走动())
                {
                    return;
                }
                游戏方向 方向;
                方向 = 计算类.计算方向(this.当前坐标, this.对象仇恨.当前目标.当前坐标);
                bool flag;
                flag = false;
                for (int i = 0; i < 10; i++)
                {
                    Point point;
                    point = 计算类.前方坐标(this.当前坐标, 方向, 1);
                    if (!this.当前地图.能否通行(point))
                    {
                        方向 = 计算类.旋转方向(方向, (主程.随机数.Next(2) != 0) ? 1 : (-1));
                        continue;
                    }
                    this.忙碌时间 = 主程.当前时间.AddMilliseconds(this.行走耗时);
                    this.行走时间 = 主程.当前时间.AddMilliseconds(this.行走耗时 + this.移动间隔);
                    this.当前方向 = 计算类.计算方向(this.当前坐标, point);
                    base.自身移动时处理(point);
                    base.发送封包(new 对象角色走动
                    {
                        对象编号 = this.地图编号,
                        移动坐标 = point,
                        移动速度 = base.行走速度
                    });
                    flag = true;
                    break;
                }
                if (!flag)
                {
                    this.对象仇恨.移除仇恨(this.对象仇恨.当前目标);
                }
            }
            else if (主程.当前时间 > this.攻击时间)
            {
                new 技能实例(this, 游戏技能, null, base.动作编号++, this.当前地图, this.当前坐标, this.对象仇恨.当前目标, this.对象仇恨.当前目标.当前坐标, null);
                this.攻击时间 = 主程.当前时间.AddMilliseconds(计算类.数值限制(0, 10 - this[游戏对象属性.攻击速度], 10) * 500);
            }
            else if (this.能否转动())
            {
                this.当前方向 = 计算类.计算方向(this.当前坐标, this.对象仇恨.当前目标.当前坐标);
            }
        }

        public void 宠物经验增加()
        {
            if (this.宠物等级 < this.等级上限 && ++this.宠物经验 >= this.升级经验)
            {
                this.宠物等级++;
                this.宠物经验 = 0;
                base.属性加成[this] = this.基础属性;
                this.更新对象属性();
                this.当前体力 = this[游戏对象属性.最大体力];
                base.发送封包(new 对象变换类型
                {
                    改变类型 = 2,
                    对象编号 = this.地图编号
                });
                base.发送封包(new 同步宠物等级
                {
                    宠物编号 = this.地图编号,
                    宠物等级 = this.宠物等级
                });
            }
        }

        public void 宠物召回处理(bool 重叠 = false)
        {
            Point 原点;
            原点 = this.宠物主人.当前坐标;
            if (!重叠)
            {
                for (int i = 1; i <= 120; i++)
                {
                    Point point;
                    point = 计算类.螺旋坐标(原点, i);
                    if (this.宠物主人.当前地图.能否通行(point))
                    {
                        原点 = point;
                        break;
                    }
                }
            }
            this.清空宠物仇恨();
            base.清空邻居时处理();
            base.解绑网格();
            this.当前坐标 = 原点;
            玩家实例 玩家实例2;
            玩家实例2 = this.宠物主人;
            object obj;
            if (玩家实例2 == null)
            {
                obj = null;
            }
            else
            {
                obj = 玩家实例2.当前地图;
                if (obj != null)
                {
                    goto IL_0071;
                }
            }
            obj = null;
            goto IL_0071;
        IL_0071:
            this.当前地图 = (地图实例)obj;
            base.绑定网格();
            base.更新邻居时处理();
        }

        public void 宠物沉睡处理()
        {
            base.技能任务.Clear();
            foreach (Buff数据 item in this.Buff列表.Values.ToList())
            {
                if (item.下线消失)
                {
                    base.删除Buff时处理(item.Buff编号.V, 后接BUFF: false);
                }
            }
            this.对象死亡 = true;
            base.删除对象();
            this.当前地图?.移除对象(this);
        }

        public bool 更新对象仇恨()
        {
            if (this.对象仇恨.仇恨列表.Count == 0)
            {
                return false;
            }
            if (this.对象仇恨.当前目标 == null)
            {
                this.对象仇恨.切换时间 = default(DateTime);
            }
            else if (this.对象仇恨.当前目标.对象死亡)
            {
                this.对象仇恨.移除仇恨(this.对象仇恨.当前目标);
            }
            else if (!base.邻居列表.Contains(this.对象仇恨.当前目标))
            {
                this.对象仇恨.移除仇恨(this.对象仇恨.当前目标);
            }
            else if (!this.对象仇恨.仇恨列表.ContainsKey(this.对象仇恨.当前目标))
            {
                this.对象仇恨.移除仇恨(this.对象仇恨.当前目标);
            }
            else if (this.宠物主人.对象关系(this.对象仇恨.当前目标) != 游戏对象关系.敌对)
            {
                this.对象仇恨.移除仇恨(this.对象仇恨.当前目标);
            }
            else if (base.网格距离(this.对象仇恨.当前目标) > this.仇恨范围 && 主程.当前时间 > this.对象仇恨.仇恨列表[this.对象仇恨.当前目标].仇恨时间)
            {
                this.对象仇恨.移除仇恨(this.对象仇恨.当前目标);
            }
            else if (base.网格距离(this.对象仇恨.当前目标) <= this.仇恨范围)
            {
                this.对象仇恨.仇恨列表[this.对象仇恨.当前目标].仇恨时间 = 主程.当前时间.AddMilliseconds(this.仇恨时长);
            }
            if (this.对象仇恨.切换时间 < 主程.当前时间 && this.对象仇恨.切换仇恨(this))
            {
                this.对象仇恨.切换时间 = 主程.当前时间.AddMilliseconds(this.切换间隔);
            }
            if (this.对象仇恨.当前目标 == null)
            {
                return this.更新对象仇恨();
            }
            return true;
        }

        public void 清空宠物仇恨()
        {
            this.对象仇恨.当前目标 = null;
            this.对象仇恨.仇恨列表.Clear();
        }

        public override void Process(DelayedAction action)
        {
        }
    }
}
