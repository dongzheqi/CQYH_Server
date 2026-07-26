using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using 游戏服务器.工具类;
using Newtonsoft.Json;

namespace 游戏服务器.模板类
{
	public sealed class 游戏Buff
	{
		public static Dictionary<ushort, 游戏Buff> 数据表;

		private Dictionary<游戏对象属性, int>[] _基础属性增减;

		[Category("基本属性")]
		public string Buff名字 { get; set; }

		[Category("基本属性")]
		public ushort Buff编号 { get; set; }

		[Category("基本属性")]
		public ushort 分组编号 { get; set; }

		[Category("基本属性")]
		public Buff作用类型 作用类型 { get; set; }

		[Category("基本属性")]
		public Buff叠加类型 叠加类型 { get; set; }

		[Category("基本属性")]
		[Editor(typeof(FlagEnumUIEditor), typeof(UITypeEditor))]
		public Buff效果类型 Buff效果 { get; set; }

		[Category("特殊属性")]
		public bool 同步至客户端 { get; set; }

		[Category("特殊属性")]
		public bool 到期主动消失 { get; set; }

		[Category("特殊属性")]
		public bool 下线计算到期 { get; set; }

		[Category("到期换图")]
		public bool 到期切换地图 { get; set; }

		[Category("到期换图")]
		public ushort 切换地图编号 { get; set; }

		[Category("到期换图")]
		public ushort 检测所在地图 { get; set; }

		[Category("特殊属性")]
		public bool 切换地图消失 { get; set; }

		[Category("特殊属性")]
		public bool 切换武器消失 { get; set; }

		[Category("特殊属性")]
		public bool 角色死亡消失 { get; set; }

		[Category("特殊属性")]
		public bool 角色下线消失 { get; set; }

		[Category("到期减少冷却")]
		public bool 到期减少冷却 { get; set; }

		[Category("到期减少冷却")]
		public ushort 减少冷却技能 { get; set; }

		[Category("到期减少冷却")]
		public ushort 每层减少冷却 { get; set; }

		[Category("技能联动")]
		public ushort 绑定技能等级 { get; set; }

		[Category("技能联动")]
		public bool 移除添加冷却 { get; set; }

		[Category("技能联动")]
		public ushort 技能冷却时间 { get; set; }

		[Category("叠加BUFF")]
		public byte Buff初始层数 { get; set; }

		[Category("叠加BUFF")]
		public byte Buff最大层数 { get; set; }

		[Category("叠加BUFF")]
		public bool Buff允许合成 { get; set; }

		[Category("叠加BUFF")]
		public byte Buff合成层数 { get; set; }

		[Category("叠加BUFF")]
		public ushort Buff合成编号 { get; set; }

		[Category("叠加BUFF")]
		public string 合成触发技能 { get; set; }

		[Category("叠加BUFF")]
		public ushort[] 每层触发Buff { get; set; }

		[Category("时间设置")]
		public int Buff处理间隔 { get; set; }

		[Category("时间设置")]
		public int Buff处理延迟 { get; set; }

		[Category("时间设置")]
		public int Buff持续时间 { get; set; }

		[Category("时间设置")]
		public bool 持续时间延长 { get; set; }

		[Category("时间设置")]
		public bool 技能等级延时 { get; set; }

		[Category("时间设置")]
		public int 每级延长时间 { get; set; }

		[Category("时间设置")]
		public bool 角色属性延时 { get; set; }

		[Category("时间设置")]
		public 游戏对象属性 绑定角色属性 { get; set; }

		[Category("时间设置")]
		public float 属性延时系数 { get; set; }

		[Category("时间设置")]
		public bool 特定铭文延时 { get; set; }

		[Category("时间设置")]
		public int 特定铭文技能 { get; set; }

		[Category("时间设置")]
		public int 铭文延长时间 { get; set; }

		[Category("BUFF联动")]
		public ushort 后接Buff编号 { get; set; }

		[Category("BUFF联动")]
		public ushort[] 后接Buff列表 { get; set; }

		[Category("BUFF联动-连带Buff")]
		public ushort 连带Buff编号 { get; set; }

		[Category("BUFF联动-连带Buff")]
		public ushort[] 连带Buff列表 { get; set; }

		[Category("BUFF联动-连带Buff")]
		public 技能范围类型 连带Buff范围 { get; set; }

		[Category("BUFF联动-连带Buff")]
		public 游戏对象类型 连带目标类型 { get; set; }

		[Category("BUFF联动-连带Buff")]
		public 游戏对象关系 连带目标关系 { get; set; }

		[Category("BUFF联动")]
		public ushort[] 依存Buff列表 { get; set; }

		[Category("BUFF联动")]
		public ushort 删除后接BUFF { get; set; }

		[Category("获得奖励")]
		public int 增加角色经验 { get; set; }

		[Category("获得奖励")]
		public int 增加双倍经验 { get; set; }

		[Category("获得奖励")]
		public int 增加角色金币 { get; set; }

		[Category("获得奖励")]
		public int 增加角色银币 { get; set; }

		[Category("属性设置")]
		[Editor(typeof(FlagEnumUIEditor), typeof(UITypeEditor))]
		public 游戏对象状态 角色所处状态 { get; set; }

		[Category("属性设置")]
		public 铭文属性[] 属性增减 { get; set; }

		[Category("属性设置")]
		public 转换属性[] 属性转换 { get; set; }

		[Category("伤害设置-造成伤害")]
		public 技能伤害类型 Buff伤害类型 { get; set; }

		[Category("伤害设置-造成伤害")]
		public int[] Buff伤害基数 { get; set; }

		[Category("伤害设置-造成伤害")]
		public float[] Buff伤害系数 { get; set; }

		[Category("伤害设置-造成伤害")]
		public ushort 触发技能伤害 { get; set; }

		[Category("伤害设置-造成伤害")]
		public bool 伤害不计神圣 { get; set; }

		[Category("伤害设置-造成伤害")]
		public ushort 判断BUFF增伤 { get; set; }

		[Category("伤害设置-造成伤害")]
		public int BUFF增伤数值 { get; set; }

		[Category("伤害设置-铭文联动")]
		public int 强化铭文编号 { get; set; }

		[Category("伤害设置-铭文联动")]
		public int 铭文强化基数 { get; set; }

		[Category("伤害设置-铭文联动")]
		public float 铭文强化系数 { get; set; }

		[Category("BUFF联动")]
		public bool 效果生效移除 { get; set; }

		[Category("BUFF联动")]
		public sbyte 生效增减层数 { get; set; }

		[Category("BUFF联动")]
		public ushort 生效后接编号 { get; set; }

		[Category("BUFF联动")]
		public bool 后接技能来源 { get; set; }

		[Category("BUFF联动")]
		public int 生效延长时间 { get; set; }

		[Category("BUFF联动")]
		public int 延长限制时间 { get; set; }

		[Category("伤害设置-触发条件")]
		public 指定目标类型 限定目标类型 { get; set; }

		[Category("伤害设置-触发条件")]
		public Buff判定方式 效果判定方式 { get; set; }

		[Category("伤害设置-限定伤害")]
		public bool 限定伤害上限 { get; set; }

		[Category("伤害设置-限定伤害")]
		public int 限定伤害数值 { get; set; }

		[Category("伤害设置-触发条件")]
		public Buff判定类型 效果判定类型 { get; set; }

		[Category("伤害设置-触发条件")]
		public bool 效果判定取反 { get; set; }

		[Category("伤害设置-触发条件")]
		[Editor(typeof(HashSetEditor), typeof(UITypeEditor))]
		public HashSet<ushort> 特定技能编号 { get; set; }

		[Category("伤害设置-触发条件")]
		[Editor(typeof(HashSetEditor), typeof(UITypeEditor))]
		public HashSet<ushort> 特定BUFF编号 { get; set; }

		[Category("伤害设置-触发条件")]
		public float 自身血量比例 { get; set; }

		[Category("伤害设置-触发条件")]
		public float 对象血量比例 { get; set; }

		[Category("伤害设置-伤害增减")]
		public 游戏对象属性 继承主人属性 { get; set; }

		[Category("伤害设置-伤害增减")]
		public int[] 伤害增减基数 { get; set; }

		[Category("伤害设置-伤害增减")]
		public float[] 伤害增减系数 { get; set; }

		[Category("伤害设置-伤害增减")]
		public bool 龙卫宠物基数 { get; set; }

		[Category("伤害设置-伤害增减")]
		public bool 不算BUFF层数 { get; set; }

		[Category("伤害设置-伤害增减")]
		public float 数量衰减系数 { get; set; }

		[Category("伤害设置-伤害转移")]
		public int[] 伤害转移基数 { get; set; }

		[Category("伤害设置-伤害转移")]
		public float[] 伤害转移系数 { get; set; }

		[Category("伤害设置-伤害转移")]
		public byte 伤害转移距离 { get; set; }

		[Category("伤害设置-伤害转移")]
		[Editor(typeof(FlagEnumUIEditor), typeof(UITypeEditor))]
		public 指定目标类型 转移限定类型 { get; set; }

		[Category("伤害设置-伤害转移")]
		public bool 转移不扣伤害 { get; set; }

		[Category("创建陷阱")]
		public string 触发陷阱技能 { get; set; }

		[Category("创建陷阱")]
		public 技能范围类型 触发陷阱数量 { get; set; }

		[Category("回复属性")]
		public byte[] 体力回复基数 { get; set; }

		[Category("回复属性")]
		public float[] 体力回复系数 { get; set; }

		[Category("回复属性")]
		public byte[] 魔力回复基数 { get; set; }

		[Category("回复属性")]
		public float[] 魔力回复系数 { get; set; }

		[Category("诱惑属性")]
		public int 诱惑时长增加 { get; set; }

		[Category("诱惑属性")]
		public float 诱惑概率增加 { get; set; }

		[Category("诱惑属性")]
		public byte 诱惑等级增加 { get; set; }

		[Category("特殊属性")]
		public bool 攻击动作消失 { get; set; }

		[Category("添加BUFF")]
		public ushort 添加BUFF编号 { get; set; }

		[Category("技能联动")]
		public string 释放技能编号 { get; set; }

		[Category("技能联动")]
		public string 移动释放技能 { get; set; }

		[Category("技能联动-受伤触发")]
		public string 受伤触发技能 { get; set; }

		[Category("技能联动-受伤触发")]
		public ushort 受伤触发BUFF { get; set; }

		[Category("技能联动-受伤触发")]
		[Editor(typeof(FlagEnumUIEditor), typeof(UITypeEditor))]
		public 指定目标类型 受伤触发限定 { get; set; }

		[Category("技能联动-受伤触发")]
		public Buff判定类型 受伤类型判定 { get; set; }

		[Category("技能联动-攻击触发")]
		public string 攻击触发技能 { get; set; }

		[Category("技能联动-攻击触发")]
		public ushort 攻击触发BUFF { get; set; }

		[Category("技能联动-攻击触发")]
		[Editor(typeof(FlagEnumUIEditor), typeof(UITypeEditor))]
		public 指定目标类型 攻击触发限定 { get; set; }

		[Category("技能联动-攻击触发")]
		public Buff判定类型 攻击类型判定 { get; set; }

		[Category("技能联动-击杀触发")]
		public string 击杀触发技能 { get; set; }

		[Category("技能联动-击杀触发")]
		public ushort 击杀触发BUFF { get; set; }

		[Category("技能联动-击杀触发")]
		[Editor(typeof(FlagEnumUIEditor), typeof(UITypeEditor))]
		public 指定目标类型 击杀触发限定 { get; set; }

		[Category("技能联动")]
		public bool 成功移除自身 { get; set; }

		[Category("技能联动")]
		public ushort 触发生效间隔 { get; set; }

		[Category("属性设置")]
		public int 继承铭文编号 { get; set; }

		[Category("属性设置")]
		public int 铭文继承基数 { get; set; }

		[Category("属性设置")]
		public float 铭文继承系数 { get; set; }

		[Category("属性设置")]
		public bool 按照层数计算 { get; set; }

		[Category("属性设置")]
		public bool 层数计算系数 { get; set; }

		[Category("护盾属性")]
		public int Buff初始护盾 { get; set; }

		[Category("护盾属性")]
		public bool 护盾数值增加 { get; set; }

		[Category("护盾属性")]
		public bool 技能等级增加 { get; set; }

		[Category("护盾属性")]
		public int 每级增加数值 { get; set; }

		[Category("护盾属性")]
		public bool 角色属性增加 { get; set; }

		[Category("护盾属性")]
		public 游戏对象属性 护盾角色属性 { get; set; }

		[Category("护盾属性")]
		public float 属性增加系数 { get; set; }

		[Category("护盾属性")]
		public bool 特定铭文增加 { get; set; }

		[Category("护盾属性")]
		public int 护盾铭文技能 { get; set; }

		[Category("护盾属性")]
		public int 铭文增加数值 { get; set; }

		public string Buff备注 { get; set; }

		[Category("添加技能")]
		public ushort 添加技能编号 { get; set; }

		[Category("LUA联动")]
		public bool 执行触发LUA { get; set; }

		[Category("LUA联动")]
		public bool 添加触发LUA { get; set; }

		[Category("LUA联动")]
		public bool 移除触发LUA { get; set; }

		[Category("LUA联动")]
		public bool 删除触发LUA { get; set; }

		[Browsable(false)]
		[JsonIgnore]
		public Dictionary<游戏对象属性, int>[] 基础属性增减
		{
			get
			{
				this._基础属性增减 = new Dictionary<游戏对象属性, int>[4]
				{
					new Dictionary<游戏对象属性, int>(),
					new Dictionary<游戏对象属性, int>(),
					new Dictionary<游戏对象属性, int>(),
					new Dictionary<游戏对象属性, int>()
				};
				if (this.属性增减 != null)
				{
					铭文属性[] array;
					array = this.属性增减;
					for (int i = 0; i < array.Length; i++)
					{
						铭文属性 铭文属性2;
						铭文属性2 = array[i];
						this._基础属性增减[0][铭文属性2.属性] = 铭文属性2.零级;
						this._基础属性增减[1][铭文属性2.属性] = 铭文属性2.一级;
						this._基础属性增减[2][铭文属性2.属性] = 铭文属性2.二级;
						this._基础属性增减[3][铭文属性2.属性] = 铭文属性2.三级;
					}
				}
				return this._基础属性增减;
			}
		}

		// 先填局部表、最后一行原子换引用(与 游戏怪物.载入数据 同款范式)。
		// 原写法第一句就把 数据表 换成空 Dictionary, 之后才做真正耗时的 序列化类.反序列化(整目录数千个 .txt),
		// 于是「表是空的/半满的」窗口 = 整个反序列化耗时。而重载入口 BuffInfoView.cs:121 等 SMain 系视图
		// 并不设 主窗口.暂停界面更新(旧 主窗口.加载系统数据 是先停世界再重载的), 系统数据网关 的
		// Task.Run(...).Wait() 也只阻塞 UI 线程, 逻辑主循环照跑 —— 期间 Buff数据.Buff模板 查不到返回 null,
		// 而 地图对象.cs:287 等主循环路径对它无条件解引用, 一次 NRE 就被 主程.循环.cs:205 记成
		// 「发生致命错误」并整帧跳过(含定时存盘)。换引用后主循环任一瞬间读到的要么是旧表要么是新表。
		public static void 载入数据()
		{
			Dictionary<ushort, 游戏Buff> dictionary;
			dictionary = new Dictionary<ushort, 游戏Buff>();
			string text;
			text = Settings.游戏数据目录 + "\\System\\技能数据\\Buff数据\\";
			if (Directory.Exists(text))
			{
				object[] array;
				array = 序列化类.反序列化(text, typeof(游戏Buff));
				foreach (object obj in array)
				{
					dictionary.Add(((游戏Buff)obj).Buff编号, (游戏Buff)obj);
				}
			}
			游戏Buff.数据表 = dictionary;
		}

		public 游戏Buff()
		{
			this.每层触发Buff = new ushort[0];
			this.体力回复基数 = new byte[4];
			this.体力回复系数 = new float[4];
			this.魔力回复基数 = new byte[4];
			this.魔力回复系数 = new float[4];
			this.特定技能编号 = new HashSet<ushort>();
			this.特定BUFF编号 = new HashSet<ushort>();
		}

		public override string ToString()
		{
			return $"{this.Buff编号}-{this.Buff名字}";
		}

		internal static void 保存数据()
		{
			string text;
			text = Settings.游戏数据目录 + "\\System\\技能数据\\Buff数据\\";
			if (!Directory.Exists(text))
			{
				return;
			}
			FileInfo[] files;
			files = new DirectoryInfo(text).GetFiles();
			for (int i = 0; i < files.Length; i++)
			{
				files[i].Delete();
			}
			foreach (KeyValuePair<ushort, 游戏Buff> item in 游戏Buff.数据表)
			{
				StreamWriter streamWriter;
				streamWriter = File.CreateText($"{text}{item.Value.Buff编号}-{item.Value.Buff名字}.txt");
				streamWriter.Write(JsonConvert.SerializeObject(item.Value, Formatting.Indented, 序列化类.全局设置));
				streamWriter.Close();
			}
		}
	}
}
