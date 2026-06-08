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
	public sealed class 怪物实例 : 地图对象
	{
		public byte 宠物等级;

		public 游戏怪物 对象模板;

		public int 复活间隔;

		public 定时刷新[] 定时复活;

		public 对象仇恨 对象仇恨;

		public Point[] 出生范围;

		public 地图实例 出生地图;

		public 游戏技能 普通攻击技能;

		public List<游戏技能> 概率触发技能;

		public 游戏技能 进入战斗技能;

		public 游戏技能 退出战斗技能;

		public 游戏技能 死亡释放技能;

		public 游戏技能 移动释放技能;

		public 游戏技能 出生释放技能;

		public 游戏技能 狂暴释放技能;

		public 刷新信息 刷新配置信息;

		public byte 无归属;

		public List<怪物实例> 马仔列表;

		public 怪物实例 主人对象;

		public bool 禁止复活 { get; set; }

		public bool 尸体消失 { get; set; }

		public DateTime 攻击时间 { get; set; }

		public DateTime 漫游时间 { get; set; }

		public DateTime 复活时间 { get; set; }

		public DateTime 消失时间 { get; set; }

		public DateTime 存活时间 { get; set; }

		public override int 处理间隔 => 10;

		public byte 复活提示阶段 { get; set; }

		public override bool 能被命中
		{
			get
			{
				if (this.对象模板.能被命中)
				{
					return !this.对象死亡;
				}
				return false;
			}
		}

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
				return base.当前体力;
			}
			set
			{
				value = 计算类.数值限制(0, value, this[游戏对象属性.最大体力]);
				if (base.当前体力 != value)
				{
					base.当前体力 = value;
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
						对象朝向 = (ushort)value
					});
				}
			}
		}

		public override byte 当前等级 => this.对象模板.怪物等级;

		public override string 对象名字 => Regex.Replace(this.对象模板.怪物名字, "[\\d-]", string.Empty);

		public override string 完整名字 => this.对象模板.怪物名字;

		public override 游戏对象类型 对象类型 => 游戏对象类型.怪物;

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

		public 怪物种族分类 怪物种族 => this.对象模板.怪物分类;

		public 怪物级别分类 怪物级别 => this.对象模板.怪物级别;

		public List<怪物掉落> 怪物掉落 => this.对象模板.怪物掉落物品;

		public ushort 模板编号 => this.对象模板.怪物编号;

		public int 怪物经验 => this.对象模板.怪物提供经验;

		public int 仇恨范围
		{
			get
			{
				if (this.当前地图.地图编号 != 80)
				{
					return this.对象模板.怪物仇恨范围;
				}
				return 25;
			}
		}

		public int 移动间隔 => this.对象模板.怪物移动间隔;

		public int 切换间隔 => 5000;

		public int 漫游间隔 => this.对象模板.怪物漫游间隔;

		public int 仇恨时长 => this.对象模板.怪物仇恨时间;

		public int 尸体保留 => this.对象模板.尸体保留时长;

		public bool 怪物禁止移动 => this.对象模板.怪物禁止移动;

		public bool 可被技能推动 => this.对象模板.可被技能推动;

		public bool 可见隐身目标 => this.对象模板.可见隐身目标;

		public bool 可被技能控制 => this.对象模板.可被技能控制;

		public bool 可被技能诱惑 => this.对象模板.可被技能诱惑;

		public float 基础诱惑概率 => this.对象模板.基础诱惑概率;

		public ushort 狂暴状态编号 => this.对象模板.狂暴状态编号;

		public float 狂暴状态血量 => this.对象模板.狂暴状态血量;

		public bool 主动攻击目标 => this.对象模板.主动攻击目标;

		public bool 执行脚本 => this.对象模板.执行脚本;

		public bool 触发lua => this.对象模板.触发lua;

		public 怪物实例()
		{
			this.地图编号 = ++地图处理网关.对象编号;
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
			string text8;
			text8 = this.对象模板.狂暴释放技能;
			if (text8 != null && text8.Length > 0)
			{
				游戏技能.数据表.TryGetValue(this.对象模板.狂暴释放技能, out this.狂暴释放技能);
			}
			if (this.对象模板.刷新通知)
			{
				this.存活时间 = 主程.当前时间.AddMinutes(90.0);
			}
		}

		public 怪物实例(宠物实例 对应宠物)
		{
			this.地图编号 = ++地图处理网关.对象编号;
			this.对象模板 = 对应宠物.对象模板;
			this.当前地图 = 对应宠物.当前地图;
			this.当前坐标 = 对应宠物.当前坐标;
			this.当前方向 = 对应宠物.当前方向;
			this.宠物等级 = 对应宠物.宠物等级;
			this.禁止复活 = true;
			this.对象仇恨 = new 对象仇恨();
			this.存活时间 = 主程.当前时间.AddHours(2.0);
			base.恢复时间 = 主程.当前时间.AddSeconds(5.0);
			this.攻击时间 = 主程.当前时间.AddSeconds(1.0);
			this.漫游时间 = 主程.当前时间.AddMilliseconds(this.漫游间隔);
			base.属性加成[this] = 对应宠物.基础属性;
			this.更新对象属性();
			this.当前体力 = Math.Min(对应宠物.当前体力, this[游戏对象属性.最大体力]);
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
			string text8;
			text8 = this.对象模板.狂暴释放技能;
			if (text8 != null && text8.Length > 0)
			{
				游戏技能.数据表.TryGetValue(this.对象模板.狂暴释放技能, out this.狂暴释放技能);
			}
			对应宠物.自身死亡处理(null, 技能击杀: false);
			对应宠物.删除对象();
			对应宠物.当前地图?.移除对象(this);
			this.对象死亡 = false;
			base.战斗姿态 = false;
			this.阻塞网格 = this.对象模板.阻塞网格;
			base.绑定网格();
			base.更新邻居时处理();
			地图处理网关.添加地图对象(this);
		}

		public 怪物实例(游戏怪物 对应模板, 地图实例 出生地图, int 复活间隔, Point 出生范围, bool 禁止复活, bool 立即刷新)
		{
			this.对象模板 = 对应模板;
			this.出生地图 = 出生地图;
			this.当前地图 = 出生地图;
			this.复活间隔 = 复活间隔;
			this.出生范围 = new Point[1] { 出生范围 };
			this.禁止复活 = 禁止复活;
			this.地图编号 = ++地图处理网关.对象编号;
			base.属性加成[this] = 对应模板.基础属性;
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
			string text8;
			text8 = this.对象模板.狂暴释放技能;
			if (text8 != null && text8.Length > 0)
			{
				游戏技能.数据表.TryGetValue(this.对象模板.狂暴释放技能, out this.狂暴释放技能);
			}
			地图处理网关.添加地图对象(this);
			if (!禁止复活)
			{
				this.当前地图.固定怪物总数++;
				主窗口.更新地图数据(this.当前地图, "固定怪物总数", this.当前地图.固定怪物总数);
			}
			if (this.对象模板.刷新通知)
			{
				this.存活时间 = 主程.当前时间.AddMinutes(90.0);
			}
			if (立即刷新)
			{
				this.怪物复活处理(计算复活: false);
				return;
			}
			this.复活时间 = 主程.当前时间.AddMilliseconds(复活间隔);
			this.阻塞网格 = false;
			this.尸体消失 = true;
			this.对象死亡 = true;
			base.次要对象 = true;
			地图处理网关.添加次要对象(this);
		}

		public 怪物实例(游戏怪物 对应模板, 地图实例 出生地图, int 复活间隔, Point[] 出生范围, bool 禁止复活, bool 立即刷新, 怪物实例 主人 = null)
		{
			this.对象模板 = 对应模板;
			this.出生地图 = 出生地图;
			this.当前地图 = 出生地图;
			this.复活间隔 = 复活间隔;
			this.出生范围 = 出生范围;
			this.禁止复活 = 禁止复活;
			this.地图编号 = ++地图处理网关.对象编号;
			base.属性加成[this] = 对应模板.基础属性;
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
			string text8;
			text8 = this.对象模板.狂暴释放技能;
			if (text8 != null && text8.Length > 0)
			{
				游戏技能.数据表.TryGetValue(this.对象模板.狂暴释放技能, out this.狂暴释放技能);
			}
			地图处理网关.添加地图对象(this);
			if (!禁止复活)
			{
				this.当前地图.固定怪物总数++;
				主窗口.更新地图数据(this.当前地图, "固定怪物总数", this.当前地图.固定怪物总数);
			}
			if (主人 != null)
			{
				if (主人.马仔列表 == null)
				{
					主人.马仔列表 = new List<怪物实例>();
				}
				主人.马仔列表.Add(this);
				this.主人对象 = 主人;
			}
			if (this.对象模板.刷新通知)
			{
				this.存活时间 = 主程.当前时间.AddMinutes(90.0);
			}
			if (立即刷新)
			{
				this.怪物复活处理(计算复活: false);
				return;
			}
			this.复活时间 = 主程.当前时间.AddMilliseconds(复活间隔);
			this.阻塞网格 = false;
			this.尸体消失 = true;
			this.对象死亡 = true;
			base.次要对象 = true;
			地图处理网关.添加次要对象(this);
		}

		public 怪物实例(游戏怪物 对应模板, 地图实例 出生地图, 定时刷新[] 定时复活, Point[] 出生范围, bool 禁止复活, bool 立即刷新)
		{
			this.对象模板 = 对应模板;
			this.出生地图 = 出生地图;
			this.当前地图 = 出生地图;
			this.复活间隔 = 0;
			this.定时复活 = 定时复活;
			this.出生范围 = 出生范围;
			this.禁止复活 = 禁止复活;
			this.地图编号 = ++地图处理网关.对象编号;
			base.属性加成[this] = 对应模板.基础属性;
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
			string text8;
			text8 = this.对象模板.狂暴释放技能;
			if (text8 != null && text8.Length > 0)
			{
				游戏技能.数据表.TryGetValue(this.对象模板.狂暴释放技能, out this.狂暴释放技能);
			}
			地图处理网关.添加地图对象(this);
			if (!禁止复活)
			{
				this.当前地图.固定怪物总数++;
				主窗口.更新地图数据(this.当前地图, "固定怪物总数", this.当前地图.固定怪物总数);
			}
			if (this.对象模板.刷新通知)
			{
				this.存活时间 = 主程.当前时间.AddMinutes(90.0);
			}
			if (立即刷新)
			{
				this.怪物复活处理(计算复活: false);
				return;
			}
			this.复活时间 = this.获取复活时间();
			this.阻塞网格 = false;
			this.尸体消失 = true;
			this.对象死亡 = true;
			base.次要对象 = true;
			地图处理网关.添加次要对象(this);
		}

		public DateTime 获取复活时间(int 尸体保留 = 0)
		{
			if (this.定时复活 != null && this.定时复活.Length != 0)
			{
				DateTime now;
				now = 主程.当前时间;
				List<DateTime> list;
				list = new List<DateTime>();
				for (int i = 0; i < this.定时复活.Length; i++)
				{
					list.Add(new DateTime(now.Year, now.Month, now.Day, this.定时复活[i].小时, this.定时复活[i].分钟, 0));
					list.Add(new DateTime(now.Year, now.Month, now.Day, this.定时复活[i].小时, this.定时复活[i].分钟, 0).AddDays(1.0));
				}
				list.Sort((DateTime x, DateTime y) => DateTime.Compare(x, y));
				DateTime dateTime;
				dateTime = list.FirstOrDefault((DateTime t) => t > now);
				主程.添加系统日志($"[复活]{this.完整名字} {this.地图编号} 下次复活时间：{dateTime:yyyy-MM-dd HH:mm:ss}");
				return dateTime;
			}
			return 主程.当前时间.AddMilliseconds(this.复活间隔 + 尸体保留);
		}

		public override void 处理对象数据()
		{
			if (主程.当前时间 < base.预约时间)
			{
				return;
			}
			if (!this.对象死亡 && this.对象模板.刷新通知 && Settings.BOSS自动死亡 && !base.战斗姿态 && 主程.当前时间 >= this.存活时间)
			{
				if (this.禁止复活)
				{
					base.删除对象();
					this.当前地图?.移除对象(this);
				}
				else
				{
					this.对象死亡 = true;
					this.尸体消失 = true;
					base.清空邻居时处理();
					base.解绑网格();
					this.复活时间 = this.获取复活时间();
					this.复活提示阶段 = 0;
					主程.添加系统日志($"{this.完整名字} {this.地图编号} 超过存活时间 {this.存活时间},下次复活 {this.复活时间.ToString()}");
				}
			}
			else if (this.禁止复活 && 主程.当前时间 >= this.存活时间)
			{
				base.删除对象();
				this.当前地图?.移除对象(this);
			}
			else if (this.对象死亡)
			{
				if (!this.尸体消失 && 主程.当前时间 >= this.消失时间)
				{
					if (this.禁止复活)
					{
						base.删除对象();
						this.当前地图.移除对象(this);
					}
					else
					{
						this.尸体消失 = true;
						base.清空邻居时处理();
						base.解绑网格();
					}
				}
				if (!this.禁止复活 && 主程.当前时间 >= this.复活时间)
				{
					base.清空邻居时处理();
					base.解绑网格();
					this.怪物复活处理(计算复活: true);
				}
				if (!this.禁止复活 && this.对象模板.刷新通知 && this.复活提示阶段 == 0 && 主程.当前时间.AddMinutes(30.0) >= this.复活时间)
				{
					this.复活提示阶段 = 1;
					网络服务网关.发送公告($"30分钟后，{this.对象名字}将出现在{this.当前地图.地图模板.地图名字}，请勇士们做好讨伐准备！", 滚动播报: true);
				}
				if (!this.禁止复活 && this.对象模板.刷新通知 && this.复活提示阶段 == 1 && 主程.当前时间.AddMinutes(10.0) >= this.复活时间)
				{
					this.复活提示阶段 = 2;
					网络服务网关.发送公告($"10分钟后，{this.对象名字}将出现在{this.当前地图.地图模板.地图名字}，请勇士们做好讨伐准备！", 滚动播报: true);
				}
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
				if (主程.当前时间 > this.忙碌时间 && 主程.当前时间 > this.硬直时间)
				{
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
					else if (this.对象模板.脱战自动石化 && !base.战斗姿态 && this.对象仇恨.仇恨列表.Count != 0)
					{
						base.战斗姿态 = true;
						base.移除Buff时处理(this.对象模板.石化状态编号);
						base.脱战时间 = 主程.当前时间.AddSeconds(10.0);
					}
					else if (this.对象模板.脱战自动石化 && base.战斗姿态 && this.对象仇恨.仇恨列表.Count == 0 && 主程.当前时间 > base.脱战时间)
					{
						base.战斗姿态 = false;
						base.添加Buff时处理(this.对象模板.石化状态编号, this);
					}
					else if (this.对象模板.仇恨锁定主人 && (this.对象仇恨.当前目标 = this.主人对象) != null)
					{
						this.怪物智能攻击();
					}
					else if ((this.怪物级别 == 怪物级别分类.头目首领) ? this.更新最近仇恨() : this.更新对象仇恨())
					{
						if (this.狂暴状态编号 > 0 && this.狂暴状态血量 > 0f && this.对象仇恨.仇恨列表.Count != 0 && !this.Buff列表.ContainsKey(this.狂暴状态编号) && this.狂暴状态血量 > (float)this.当前体力 / (float)this[游戏对象属性.最大体力])
						{
							base.添加Buff时处理(this.狂暴状态编号, this);
							if (this.狂暴释放技能 != null)
							{
								new 技能实例(this, this.狂暴释放技能, null, base.动作编号++, this.当前地图, this.当前坐标, this.对象仇恨.当前目标, this.对象仇恨.当前目标.当前坐标, null);
								this.攻击时间 = 主程.当前时间.AddMilliseconds(计算类.数值限制(0, 10 - this[游戏对象属性.攻击速度], 10) * 500);
							}
						}
						else
						{
							this.怪物智能攻击();
						}
					}
					else
					{
						this.怪物随机漫游();
					}
				}
			}
			base.处理对象数据();
		}

		public override void 自身死亡处理(地图对象 对象, bool 技能击杀, bool 脚本击杀 = false)
		{
			foreach (技能实例 item in base.技能任务)
			{
				item.技能中断();
			}
			if (this.马仔列表 != null && this.马仔列表.Count > 0)
			{
				foreach (怪物实例 item2 in this.马仔列表)
				{
					if (item2 == null)
					{
						if (item2 == null)
						{
							continue;
						}
					}
					else
					{
						item2.清空怪物仇恨();
						if (item2 == null)
						{
							continue;
						}
					}
					item2.自身死亡处理(this, 技能击杀: false);
				}
				this.马仔列表.Clear();
			}
			if (this.触发lua)
			{
				游戏脚本.怪物死亡(this, 对象, 技能击杀);
				
			}
			base.自身死亡处理(对象, 技能击杀);
			if (this.死亡释放技能 != null && 对象 != null)
			{
				new 技能实例(this, this.死亡释放技能, null, base.动作编号++, this.当前地图, this.当前坐标, null, this.当前坐标, null).处理任务();
			}
			if ((this.当前地图.副本地图 || !this.禁止复活) && !this.对象模板.不计存活)
			{
				this.当前地图.存活怪物总数--;
				主窗口.更新地图数据(this.当前地图, "存活怪物总数", -1);
			}
			this.复活提示阶段 = 0;
			this.尸体消失 = false;
			this.消失时间 = 主程.当前时间.AddMilliseconds(this.尸体保留);
			this.复活时间 = this.获取复活时间();
			if (对象 is 宠物实例 宠物实例2)
			{
				宠物实例2.宠物经验增加();
			}
			if (this.更新怪物归属(out var 归属玩家))
			{
				if (this.怪物级别 == 怪物级别分类.精英干将 || this.怪物级别 == 怪物级别分类.头目首领 || this.对象模板.刷新通知 || 首杀怪物名单.数据表.Contains(this.模板编号))
				{
					主程.添加系统日志($"{this.完整名字} {this.地图编号} 被 {归属玩家.角色数据.角色名字.V} 击杀,下次复活 {this.复活时间.ToString()}");
					// 首杀(从零原创): BOSS 级(精英/首领/刷新通知) 或 首杀怪物名单.csv 显式列入的怪 纳入全服首杀统计
					首杀首爆管理器.尝试怪物首杀(this.模板编号, this.对象模板.怪物名字, 归属玩家);
				}
				if (!this.触发lua && this.执行脚本)
				{
					归属玩家.最后杀死的怪物编号 = this.模板编号;
					归属玩家.CallDefaultNPC(DefaultNPCType.MonDie, true, this.模板编号);
				}
				decimal 额外爆率;
				额外爆率 = 归属玩家.额外爆率;
				额外爆率 += (decimal)(归属玩家.脚本爆率 / 100000);
				HashSet<角色数据> hashSet;
				hashSet = ((归属玩家.所属队伍 == null) ? new HashSet<角色数据> { 归属玩家.角色数据 } : new HashSet<角色数据>(归属玩家.所属队伍.队伍成员));
				if (!Settings.屏蔽威望)
				{
					switch (this.当前地图.地图编号)
					{
					case 102:
						归属玩家.更改玩家威望(25, 归属玩家.获取玩家威望(25) + 1);
						break;
					case 105:
					case 106:
						归属玩家.更改玩家威望(21, 归属玩家.获取玩家威望(21) + 1);
						break;
					case 113:
					case 114:
					case 115:
					case 116:
					case 117:
					case 118:
					case 119:
					case 125:
					case 126:
						归属玩家.更改玩家威望(23, 归属玩家.获取玩家威望(23) + 1);
						break;
					case 142:
						归属玩家.更改玩家威望(30, 归属玩家.获取玩家威望(30) + 1);
						break;
					case 145:
						归属玩家.更改玩家威望(33, 归属玩家.获取玩家威望(33) + 1);
						break;
					case 147:
						归属玩家.更改玩家威望(36, 归属玩家.获取玩家威望(36) + 1);
						break;
					case 144:
					case 153:
					case 154:
						归属玩家.更改玩家威望(39, 归属玩家.获取玩家威望(39) + 1);
						break;
					case 149:
					case 159:
					case 160:
					case 161:
					case 162:
					case 163:
						归属玩家.更改玩家威望(41, 归属玩家.获取玩家威望(41) + 1);
						break;
					case 167:
					case 168:
					case 169:
					case 170:
					case 171:
					case 172:
					case 173:
					case 180:
						归属玩家.更改玩家威望(51, 归属玩家.获取玩家威望(51) + 1);
						break;
					case 187:
					case 188:
					case 189:
					case 190:
					case 191:
					case 193:
					case 194:
					case 195:
					case 196:
					case 198:
					case 199:
					case 200:
						归属玩家.更改玩家威望(57, 归属玩家.获取玩家威望(57) + 1);
						break;
					case 148:
					case 157:
					case 158:
					case 201:
					case 202:
					case 203:
					case 208:
					case 209:
					case 210:
						归属玩家.更改玩家威望(42, 归属玩家.获取玩家威望(42) + 1);
						break;
					case 146:
					case 155:
					case 156:
					case 211:
					case 212:
					case 213:
					case 214:
					case 215:
						归属玩家.更改玩家威望(40, 归属玩家.获取玩家威望(40) + 1);
						break;
					case 231:
					case 232:
					case 233:
					case 234:
					case 235:
					case 236:
					case 237:
					case 238:
					case 239:
					case 240:
					case 241:
					case 242:
					case 243:
					case 244:
					case 245:
						归属玩家.更改玩家威望(58, 归属玩家.获取玩家威望(58) + 1);
						break;
					case 50:
					case 51:
					case 52:
					case 53:
					case 54:
					case 55:
					case 56:
					case 57:
					case 58:
					case 59:
					case 60:
					case 61:
					case 62:
					case 63:
					case 64:
					case 65:
						归属玩家.更改玩家威望(63, 归属玩家.获取玩家威望(63) + 1);
						break;
					}
				}
				if (Settings.开启成就系统 || Settings.开启任务系统)
				{
					foreach (角色数据 item3 in hashSet)
					{
						if (Settings.开启任务系统)
						{
							CharacterQuest[] inProgressQuests;
							inProgressQuests = item3.GetInProgressQuests();
							bool flag;
							flag = false;
							CharacterQuest[] array;
							array = inProgressQuests;
							foreach (CharacterQuest characterQuest in array)
							{
								CharacterQuestMission[] missionsOfType;
								missionsOfType = characterQuest.GetMissionsOfType(QuestMissionType.KillMob);
								foreach (CharacterQuestMission characterQuestMission in missionsOfType)
								{
									if (!(characterQuestMission.CompletedDate.V != DateTime.MinValue) && characterQuestMission.Info.V.Id == this.模板编号)
									{
										int num;
										num = Array.IndexOf(characterQuest.Missions.ToArray(), characterQuestMission);
										characterQuestMission.Count.V = (byte)(characterQuestMission.Count.V + 1);
										item3.网络连接?.绑定角色.发送封包(new 同步补充变量
										{
											变量类型 = 6,
											变量索引 = (ushort)num,
											对象编号 = characterQuest.Info.V.Id,
											变量内容 = characterQuestMission.Count.V
										});
										flag = true;
									}
								}
								missionsOfType = characterQuest.GetMissionsOfType(QuestMissionType.KillMobGroup);
								foreach (CharacterQuestMission characterQuestMission2 in missionsOfType)
								{
									if (!(characterQuestMission2.CompletedDate.V != DateTime.MinValue) && characterQuestMission2.Info.V.Id == this.对象模板.分组编号)
									{
										int num2;
										num2 = Array.IndexOf(characterQuest.Missions.ToArray(), characterQuestMission2);
										characterQuestMission2.Count.V = (byte)(characterQuestMission2.Count.V + 1);
										item3.网络连接?.绑定角色.发送封包(new 同步补充变量
										{
											变量类型 = 6,
											变量索引 = (ushort)num2,
											对象编号 = characterQuest.Info.V.Id,
											变量内容 = characterQuestMission2.Count.V
										});
										flag = true;
									}
								}
								if (flag)
								{
									item3.网络连接?.绑定角色.UpdateQuestProgress(characterQuest);
								}
							}
						}
						if (!Settings.开启成就系统)
						{
							continue;
						}
						item3.网络连接?.绑定角色.成就变量变更(AchievementVariables.KillNpcTotalCount, 1);
						if (this.怪物级别 == 怪物级别分类.头目首领)
						{
							item3.网络连接?.绑定角色.成就变量变更(AchievementVariables.KillNpcBoss, 1);
						}
						foreach (杀怪成就 item4 in 杀怪成就.数据表.Values.Where(delegate(杀怪成就 x)
						{
							if (x.怪物编号 > 0 && x.怪物编号 == this.对象模板.怪物编号)
							{
								return true;
							}
							return x.分组编号 > 0 && x.分组编号 == this.对象模板.分组编号;
						}))
						{
							item3.网络连接?.绑定角色.杀怪成就变更(item4.成就编号, 1);
						}
					}
				}
				float num3;
				num3 = 计算类.收益衰减(归属玩家.当前等级, this.当前等级);
				int num4;
				num4 = 0;
				List<int> list;
				list = new List<int>();
				Dictionary<掉落条件分组, bool> dictionary;
				dictionary = new Dictionary<掉落条件分组, bool>();
				if (num3 < 1f)
				{
					foreach (怪物掉落 item5 in 计算类.RandomSort(this.怪物掉落))
					{
						if (item5.暴率分组 > 0 && list.Contains(item5.暴率分组))
						{
							continue;
						}
						if (item5.条件分组 != null)
						{
							bool value;
							value = false;
							if (!dictionary.TryGetValue(item5.条件分组, out value))
							{
								value = item5.条件分组.检测(归属玩家);
								dictionary.Add(item5.条件分组, value);
							}
							if (!value)
							{
								continue;
							}
						}
						if (!游戏物品.检索表.TryGetValue(item5.物品名字, out var value2) || 计算类.计算概率(num3))
						{
							continue;
						}
						int num5;
						num5 = Math.Max(1, item5.掉落概率 - (int)Math.Round((decimal)item5.掉落概率 * (Settings.怪物额外爆率 + 额外爆率)));
						if (num5 != 1 && 主程.随机数.Next(num5) != num5 / 2)
						{
							continue;
						}
						int num6;
						num6 = 主程.随机数.Next(item5.最小数量, Math.Max(item5.最小数量, item5.最大数量 + 1));
						if (num6 == 0)
						{
							continue;
						}
						if (item5.暴率分组 > 0)
						{
							list.Add(item5.暴率分组);
						}
						if (value2.物品持久 == 0)
						{
							new 物品实例(value2, null, this.当前地图, this.当前坐标, ((归属玩家.所属队伍 == null) ? hashSet : 归属玩家.所属队伍.计算掉落归属(value2.物品编号)), num6, 物品绑定: false, this);
							if (value2.物品编号 == 1)
							{
								this.当前地图.金币掉落总数 += num6;
							}
							this.对象模板.掉落统计[value2] = (this.对象模板.掉落统计.ContainsKey(value2) ? this.对象模板.掉落统计[value2] : 0L) + num6;
						}
						else
						{
							for (int k = 0; k < num6; k++)
							{
								new 物品实例(value2, null, this.当前地图, this.当前坐标, ((归属玩家.所属队伍 == null) ? hashSet : 归属玩家.所属队伍.计算掉落归属(value2.物品编号)), 1, 物品绑定: false, this);
							}
							this.当前地图.怪物掉落次数 += num6;
							num4++;
							this.对象模板.掉落统计[value2] = (this.对象模板.掉落统计.ContainsKey(value2) ? this.对象模板.掉落统计[value2] : 0L) + num6;
						}
						if (value2.贵重物品 && !value2.爆不通知)
						{
							网络服务网关.发送公告($"<#P0:<#N:{this.模板编号}>><#P1:<#PN:{归属玩家.对象名字}>><#P2:<#I:{value2.物品编号}>><#T:MMOGame.TvShowNeedSysNtf.ITEM.10>", 滚动播报: false, saveLog: false);
							主程.添加系统日志($"{this.当前地图.地图模板?.地图名字} 的 {this.对象模板?.怪物名字} 被玩家 {归属玩家.对象名字}击杀,掉落 {value2.物品名字}", hardLog: true, showDiag: false);
							// 首爆(从零原创): 仅贵重物品纳入全服首爆统计
							首杀首爆管理器.尝试装备首爆(value2.物品编号, value2.物品名字, 归属玩家);
						}
					}
				}
				if (!脚本击杀)
				{
					if (归属玩家.所属队伍 == null)
					{
						归属玩家.玩家增加经验(this, this.怪物经验);
					}
					else
					{
						List<玩家实例> list2;
						list2 = new List<玩家实例> { 归属玩家 };
						foreach (地图对象 item6 in base.重要邻居)
						{
							if (item6 != 归属玩家 && item6 is 玩家实例 玩家实例2 && 玩家实例2.所属队伍 == 归属玩家.所属队伍)
							{
								list2.Add(玩家实例2);
							}
						}
						float num7;
						num7 = (float)this.怪物经验 * (1f + (float)(list2.Count - 1) * 0.2f);
						float num8;
						num8 = list2.Sum((玩家实例 x) => x.当前等级);
						foreach (玩家实例 item7 in list2)
						{
							item7.玩家增加经验(this, (int)(num7 * (float)(int)item7.当前等级 / num8));
						}
					}
				}
			}
			this.Buff列表.Clear();
			base.次要对象 = true;
			地图处理网关.添加次要对象(this);
			if (base.激活对象)
			{
				base.激活对象 = false;
				地图处理网关.移除激活对象(this);
			}
		}

		public void 怪物随机漫游()
		{
			if (this.怪物禁止移动 || 主程.当前时间 < this.漫游时间)
			{
				return;
			}
			if (this.能否走动())
			{
				Point point;
				point = 计算类.前方坐标(this.当前坐标, 计算类.随机方向(), 1);
				if (this.当前地图.能否通行(point))
				{
					this.忙碌时间 = 主程.当前时间.AddMilliseconds(this.行走耗时);
					this.行走时间 = 主程.当前时间.AddMilliseconds(this.行走耗时 + this.移动间隔);
					this.当前方向 = 计算类.计算方向(this.当前坐标, point);
					base.自身移动时处理(point);
					if (!this.对象死亡)
					{
						base.发送封包(new 对象角色走动
						{
							对象编号 = this.地图编号,
							移动坐标 = this.当前坐标,
							移动速度 = base.行走速度
						});
					}
					if (this.移动释放技能 != null)
					{
						new 技能实例(this, this.移动释放技能, null, base.动作编号++, this.当前地图, this.当前坐标, null, this.当前坐标, null).处理任务();
					}
				}
			}
			this.漫游时间 = 主程.当前时间.AddMilliseconds(this.漫游间隔 + 主程.随机数.Next(5000));
		}

		public void 怪物智能攻击()
		{
			base.脱战时间 = 主程.当前时间.AddSeconds(10.0);
			游戏技能 游戏技能;
			游戏技能 = null;
			if (this.概率触发技能 != null)
			{
				foreach (游戏技能 item in this.概率触发技能)
				{
					if ((!this.冷却记录.ContainsKey(item.自身技能编号 | 0x1000000) || 主程.当前时间 > this.冷却记录[item.自身技能编号 | 0x1000000]) && 计算类.计算概率(item.计算幸运概率 ? 计算类.计算幸运(this[游戏对象属性.幸运等级]) : item.计算触发概率))
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
			if (游戏技能 == null || (游戏技能.验证自身血量 && ((游戏技能.自身血量低于 > 0f) ? (游戏技能.自身血量低于 > (float)this.当前体力 / (float)this[游戏对象属性.最大体力]) : (游戏技能.自身血量高于 > (float)this.当前体力 / (float)this[游戏对象属性.最大体力]))) || (游戏技能.验证目标血量 && ((游戏技能.目标血量低于 > 0f) ? (游戏技能.目标血量低于 > (float)this.对象仇恨.当前目标.当前体力 / (float)this.对象仇恨.当前目标[游戏对象属性.最大体力]) : (游戏技能.目标血量高于 > (float)this.对象仇恨.当前目标.当前体力 / (float)this.对象仇恨.当前目标[游戏对象属性.最大体力]))) || (游戏技能.验证角色Buff != 0 && !this.Buff列表.ContainsKey(游戏技能.验证角色Buff)) || this.检查状态(游戏对象状态.忙绿状态 | 游戏对象状态.麻痹状态 | 游戏对象状态.失神状态))
			{
				return;
			}
			if (base.网格距离(this.对象仇恨.当前目标) > 游戏技能.技能最远距离)
			{
				if (this.怪物禁止移动 || !this.能否走动())
				{
					return;
				}
				游戏方向 方向;
				方向 = 计算类.计算方向(this.当前坐标, this.对象仇恨.当前目标.当前坐标);
				Point point;
				point = default(Point);
				for (int i = 0; i < 8; i++)
				{
					if (!this.当前地图.能否通行(point = 计算类.前方坐标(this.当前坐标, 方向, 1)))
					{
						方向 = 计算类.旋转方向(方向, (主程.随机数.Next(2) != 0) ? 1 : (-1));
						continue;
					}
					this.忙碌时间 = 主程.当前时间.AddMilliseconds(this.行走耗时);
					this.行走时间 = 主程.当前时间.AddMilliseconds(this.行走耗时 + this.移动间隔);
					this.当前方向 = 计算类.计算方向(this.当前坐标, point);
					base.发送封包(new 对象角色走动
					{
						对象编号 = this.地图编号,
						移动坐标 = point,
						移动速度 = base.行走速度
					});
					base.自身移动时处理(point);
					if (this.移动释放技能 != null)
					{
						new 技能实例(this, this.移动释放技能, null, base.动作编号++, this.当前地图, this.当前坐标, null, this.当前坐标, null).处理任务();
					}
					break;
				}
			}
			else if (游戏技能.需要正向走位 && !计算类.直线方向(this.当前坐标, this.对象仇恨.当前目标.当前坐标))
			{
				if (this.怪物禁止移动 || !this.能否走动())
				{
					return;
				}
				游戏方向 方向2;
				方向2 = 计算类.正向方向(this.当前坐标, this.对象仇恨.当前目标.当前坐标);
				Point point2;
				point2 = default(Point);
				for (int j = 0; j < 8; j++)
				{
					if (!this.当前地图.能否通行(point2 = 计算类.前方坐标(this.当前坐标, 方向2, 1)))
					{
						方向2 = 计算类.旋转方向(方向2, (主程.随机数.Next(2) != 0) ? 1 : (-1));
						continue;
					}
					this.当前方向 = 计算类.计算方向(this.当前坐标, point2);
					this.忙碌时间 = 主程.当前时间.AddMilliseconds(this.行走耗时);
					this.行走时间 = 主程.当前时间.AddMilliseconds(this.行走耗时 + this.移动间隔);
					base.自身移动时处理(point2);
					if (!this.对象死亡)
					{
						base.发送封包(new 对象角色走动
						{
							对象编号 = this.地图编号,
							移动坐标 = point2,
							移动速度 = base.行走速度
						});
					}
					if (this.移动释放技能 != null)
					{
						new 技能实例(this, this.移动释放技能, null, base.动作编号++, this.当前地图, this.当前坐标, null, this.当前坐标, null).处理任务();
					}
					break;
				}
			}
			else if (主程.当前时间 > this.攻击时间)
			{
				foreach (Buff数据 item2 in this.Buff列表.Values.ToList())
				{
					if (item2.攻击消失 && (item2.Buff来源 == null || item2.Buff来源.地图编号 == this.地图编号))
					{
						base.删除Buff时处理(item2.Buff编号.V);
					}
				}
				new 技能实例(this, 游戏技能, null, base.动作编号++, this.当前地图, this.当前坐标, this.对象仇恨.当前目标, this.对象仇恨.当前目标.当前坐标, null);
				this.攻击时间 = 主程.当前时间.AddMilliseconds(计算类.数值限制(0, 10 - this[游戏对象属性.攻击速度], 10) * 500);
			}
			else if (!this.怪物禁止移动 && this.能否转动())
			{
				this.当前方向 = 计算类.计算方向(this.当前坐标, this.对象仇恨.当前目标.当前坐标);
			}
		}

		public void 怪物复活处理(bool 计算复活)
		{
			if ((this.当前地图.副本地图 || !this.禁止复活) && !this.对象模板.不计存活)
			{
				this.当前地图.存活怪物总数++;
				主窗口.更新地图数据(this.当前地图, "存活怪物总数", 1);
				if (计算复活)
				{
					this.当前地图.怪物复活次数++;
					主窗口.更新地图数据(this.当前地图, "怪物复活次数", 1);
				}
			}
			if (this.对象模板.刷新通知)
			{
				this.存活时间 = 主程.当前时间.AddMinutes(90.0);
				网络服务网关.发送公告($"[{this.对象名字}]出现在[{this.当前地图.地图模板.地图名字}]，请勇士们速速前往击杀！", 滚动播报: true);
			}
			this.更新对象属性();
			this.当前地图 = this.出生地图;
			this.当前方向 = 计算类.随机方向();
			this.当前体力 = this[游戏对象属性.最大体力];
			this.当前坐标 = this.出生范围[主程.随机数.Next(0, this.出生范围.Length)];
			Point point;
			point = this.当前坐标;
			for (int i = 0; i < 120; i++)
			{
				if (this.当前地图.能否通行(point = 计算类.螺旋坐标(this.当前坐标, i)))
				{
					this.当前坐标 = point;
					break;
				}
			}
			this.攻击时间 = 主程.当前时间.AddSeconds(1.0);
			base.恢复时间 = 主程.当前时间.AddMilliseconds(主程.随机数.Next(5000));
			this.漫游时间 = 主程.当前时间.AddMilliseconds(主程.随机数.Next(5000) + this.漫游间隔);
			this.对象仇恨 = new 对象仇恨();
			base.次要对象 = this.对象模板.刷新通知;
			if (this.对象模板.刷新通知)
			{
				地图处理网关.添加次要对象(this);
				if (base.激活对象)
				{
					base.激活对象 = false;
					地图处理网关.移除激活对象(this);
				}
			}
			this.对象死亡 = false;
			base.战斗姿态 = false;
			this.阻塞网格 = this.对象模板.阻塞网格;
			base.绑定网格();
			base.更新邻居时处理();
			if (!base.激活对象)
			{
				if (this.对象模板.脱战自动石化)
				{
					base.添加Buff时处理(this.对象模板.石化状态编号, this);
				}
				if (this.退出战斗技能 != null)
				{
					new 技能实例(this, this.退出战斗技能, null, base.动作编号++, this.当前地图, this.当前坐标, null, this.当前坐标, null).处理任务();
				}
			}
			if (this.对象模板.刷新通知)
			{
				主程.添加系统日志($"[{this.对象名字}]出现在[{this.当前地图.地图模板.地图名字}]，{this.当前坐标}！");
			}
		}

		public void 怪物诱惑处理()
		{
			this.Buff列表.Clear();
			this.尸体消失 = true;
			this.对象死亡 = true;
			this.阻塞网格 = false;
			if (this.禁止复活)
			{
				base.删除对象();
				this.当前地图?.移除对象(this);
				return;
			}
			base.清空邻居时处理();
			base.解绑网格();
			this.复活时间 = this.获取复活时间();
			base.次要对象 = true;
			地图处理网关.添加次要对象(this);
			base.激活对象 = false;
			地图处理网关.移除激活对象(this);
		}

		public void 怪物沉睡处理()
		{
			if (base.激活对象)
			{
				base.激活对象 = false;
				base.技能任务.Clear();
				地图处理网关.移除激活对象(this);
			}
			if (this.禁止复活 && !base.次要对象)
			{
				base.次要对象 = true;
				base.技能任务.Clear();
				地图处理网关.添加次要对象(this);
			}
		}

		public void 怪物激活处理()
		{
			if (!base.激活对象)
			{
				base.次要对象 = false;
				base.激活对象 = true;
				地图处理网关.添加激活对象(this);
				int num;
				num = (int)Math.Max(0.0, (主程.当前时间 - base.恢复时间).TotalSeconds / 5.0);
				base.当前体力 = Math.Min(this[游戏对象属性.最大体力], this.当前体力 + num * this[游戏对象属性.体力恢复]);
				base.恢复时间 = base.恢复时间.AddSeconds(5.0);
				this.攻击时间 = 主程.当前时间.AddSeconds(1.0);
				this.漫游时间 = 主程.当前时间.AddMilliseconds(主程.随机数.Next(5000) + this.漫游间隔);
				if (this.出生释放技能 != null)
				{
					new 技能实例(this, this.出生释放技能, null, 0, this.当前地图, this.当前坐标, this, this.当前坐标, null);
				}
			}
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

		public bool 更新最近仇恨()
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
			else if (base.网格距离(this.对象仇恨.当前目标) > this.仇恨范围 && 主程.当前时间 > this.对象仇恨.仇恨列表[this.对象仇恨.当前目标].仇恨时间)
			{
				this.对象仇恨.移除仇恨(this.对象仇恨.当前目标);
			}
			else if (base.网格距离(this.对象仇恨.当前目标) <= this.仇恨范围)
			{
				this.对象仇恨.仇恨列表[this.对象仇恨.当前目标].仇恨时间 = 主程.当前时间.AddMilliseconds(this.仇恨时长);
			}
			if (this.对象仇恨.切换时间 < 主程.当前时间 && this.对象仇恨.最近仇恨(this))
			{
				this.对象仇恨.切换时间 = 主程.当前时间.AddMilliseconds(this.切换间隔);
			}
			if (this.对象仇恨.当前目标 == null)
			{
				return this.更新对象仇恨();
			}
			return true;
		}

		public void 清空怪物仇恨()
		{
			this.对象仇恨.当前目标 = null;
			this.对象仇恨.仇恨列表.Clear();
		}

		public bool 更新怪物归属(out 玩家实例 归属玩家)
		{
			地图对象 地图对象2;
			地图对象2 = null;
			if (this.无归属 == 0)
			{
				foreach (KeyValuePair<地图对象, 对象仇恨.仇恨详情> item in this.对象仇恨.仇恨列表.ToList())
				{
					if (item.Key is 宠物实例 宠物实例2)
					{
						if (item.Value.仇恨数值 > 0)
						{
							this.对象仇恨.添加仇恨(宠物实例2.宠物主人, item.Value.仇恨时间, item.Value.仇恨数值);
						}
						this.对象仇恨.移除仇恨(item.Key);
					}
					else if (!(item.Key is 玩家实例))
					{
						this.对象仇恨.移除仇恨(item.Key);
					}
				}
				地图对象2 = (from x in this.对象仇恨.仇恨列表.Keys.ToList()
					orderby this.对象仇恨.仇恨列表[x].仇恨数值 descending
					select x).FirstOrDefault();
			}
			归属玩家 = ((地图对象2 == null || !(地图对象2 is 玩家实例 玩家实例2)) ? null : 玩家实例2);
			return 归属玩家 != null;
		}

		public override void Process(DelayedAction action)
		{
		}
	}
}
