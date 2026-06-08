using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using 游戏服务器.模板类;
using 游戏服务器.网络类;

namespace 游戏服务器.地图类
{
	public sealed class 地图实例
	{
		public readonly int 路线编号;

		public readonly 游戏地图 地图模板;

		public uint 固定怪物总数;

		public uint 存活怪物总数;

		public uint 怪物复活次数;

		public long 怪物掉落次数;

		public long 金币掉落总数;

		public bool 副本关闭;

		public int 副本节点;

		public 守卫实例 副本守卫;

		public DateTime 节点计时;

		public int 刷怪记录;

		public List<怪物刷新> 怪物波数;

		public HashSet<地图对象>[,] 地图对象;

		public 地形数据 地形数据;

		public 地图区域 复活区域;

		public 地图区域 红名区域;

		public 地图区域 传送区域;

		public HashSet<地图区域> 地图区域;

		public HashSet<怪物刷新> 怪物区域;

		public HashSet<守卫刷新> 守卫区域;

		public HashSet<道具刷新> 道具区域;

		public HashSet<玩家实例> 玩家列表;

		public HashSet<宠物实例> 宠物列表;

		public HashSet<物品实例> 物品列表;

		public HashSet<地图对象> 对象列表;

		public Dictionary<byte, 传送法阵> 法阵列表;

		public Dictionary<int, int> 数字变量;

		public Dictionary<int, string> 字符变量;

		public DateTime 执行计时;

		public DateTime 关闭时间;

		public int 副本主人;

		public bool 已初始化 { get; set; }

		public byte 地图状态
		{
			get
			{
				if (this.玩家列表.Count < 200)
				{
					return 1;
				}
				if (this.玩家列表.Count < 500)
				{
					return 2;
				}
				return 3;
			}
		}

		public int 地图编号 => this.地图模板.地图编号;

		public byte 限制等级 => this.地图模板.限制等级;

		public byte 分线数量 => 1;

		public bool 下线传送 => this.地图模板.下线传送;

		public byte 传送地图 => this.地图模板.传送地图;

		public bool 副本地图 => this.地图模板.副本地图;

		public Point 地图起点 => this.地形数据.地图起点;

		public Point 地图终点 => this.地形数据.地图终点;

		public Point 地图大小 => this.地形数据.地图大小;

		public HashSet<地图对象> this[Point 坐标]
		{
			get
			{
				if (this.坐标越界(坐标))
				{
					return new HashSet<地图对象>();
				}
				if (this.地图对象[坐标.X - this.地图起点.X, 坐标.Y - this.地图起点.Y] == null)
				{
					return this.地图对象[坐标.X - this.地图起点.X, 坐标.Y - this.地图起点.Y] = new HashSet<地图对象>();
				}
				return this.地图对象[坐标.X - this.地图起点.X, 坐标.Y - this.地图起点.Y];
			}
		}

		// 四叉树空间索引(性能优化, 默认关): 按对象中心坐标组织, 供 更新邻居时处理 的四叉树快路径用; 由 绑定网格/解绑网格 维护增删。
		public 游戏服务器.工具类.四叉树<地图对象> 空间索引;

		// lazy-init: 在地形数据就绪后按地图边界建树(构造期 地形数据 可能尚未赋值, 故不在构造函数建)。仅 Settings.开启四叉树邻居 时由 绑定网格 触发。
		public void 确保空间索引()
		{
			if (this.空间索引 == null && this.地形数据 != null)
			{
				Point 起点 = this.地图起点;
				Point 大小 = this.地图大小;
				this.空间索引 = new 游戏服务器.工具类.四叉树<地图对象>(new System.Drawing.Rectangle(起点.X, 起点.Y, 大小.X, 大小.Y), 10, 5, (地图对象 o) => o.当前坐标);
			}
		}

		public 地图实例(游戏地图 地图模板, int 路线编号 = 1)
		{
			this.关闭时间 = DateTime.MaxValue;
			this.地图区域 = new HashSet<地图区域>();
			this.怪物区域 = new HashSet<怪物刷新>();
			this.守卫区域 = new HashSet<守卫刷新>();
			this.道具区域 = new HashSet<道具刷新>();
			this.玩家列表 = new HashSet<玩家实例>();
			this.宠物列表 = new HashSet<宠物实例>();
			this.物品列表 = new HashSet<物品实例>();
			this.对象列表 = new HashSet<地图对象>();
			this.法阵列表 = new Dictionary<byte, 传送法阵>();
			this.数字变量 = new Dictionary<int, int>();
			this.字符变量 = new Dictionary<int, string>();
			this.地图模板 = 地图模板;
			this.路线编号 = 路线编号;
			游戏脚本.地图创建(this);
		}

		public 地图实例(游戏地图 地图模板, 地图实例 复制实例, int 路线编号 = 1)
		{
			this.关闭时间 = DateTime.MaxValue;
			this.地图区域 = new HashSet<地图区域>();
			this.怪物区域 = new HashSet<怪物刷新>();
			this.守卫区域 = new HashSet<守卫刷新>();
			this.道具区域 = new HashSet<道具刷新>();
			this.玩家列表 = new HashSet<玩家实例>();
			this.宠物列表 = new HashSet<宠物实例>();
			this.物品列表 = new HashSet<物品实例>();
			this.对象列表 = new HashSet<地图对象>();
			this.法阵列表 = new Dictionary<byte, 传送法阵>();
			this.数字变量 = new Dictionary<int, int>();
			this.字符变量 = new Dictionary<int, string>();
			this.地图模板 = 地图模板;
			this.路线编号 = 路线编号;
			this.地形数据 = 复制实例.地形数据;
			this.地图区域 = 复制实例.地图区域;
			this.怪物区域 = 复制实例.怪物区域;
			this.守卫区域 = 复制实例.守卫区域;
			this.传送区域 = 复制实例.传送区域;
			this.地图对象 = new HashSet<地图对象>[复制实例.地图大小.X, 复制实例.地图大小.Y];
			游戏脚本.地图创建(this);
		}

		public bool 范围刷新怪物(string 怪物名字, int 复活间隔, Point[] 刷新范围, bool 禁止复活, bool 立即刷新)
		{
			if (游戏怪物.数据表.TryGetValue(怪物名字, out var value))
			{
				new 怪物实例(value, this, 复活间隔, 刷新范围, 禁止复活, 立即刷新).存活时间 = 主程.当前时间.AddHours(2.0);
				return true;
			}
			return false;
		}

		private void 魔虫窟执行()
		{
			if (this.副本节点 == 0 && this.固定怪物总数 - this.存活怪物总数 >= 60)
			{
				this.副本节点 = 1;
				this.节点计时 = 主程.当前时间.AddHours(2.0);
				this.地图公告("<#T:50607>");
				Point[] 刷新范围;
				刷新范围 = this.怪物区域.FirstOrDefault((怪物刷新 o) => o.区域名字 == "魔虫窟-BOSS怪物区域")?.范围坐标.ToArray();
				this.范围刷新怪物("蛇蝎 魔闪", 0, 刷新范围, 禁止复活: true, 立即刷新: true);
				this.范围刷新怪物("蛇蝎 毒钩", 0, 刷新范围, 禁止复活: true, 立即刷新: true);
				this.范围刷新怪物("蛇蝎 赤牙", 0, 刷新范围, 禁止复活: true, 立即刷新: true);
			}
		}

		public void 处理数据()
		{
			if (this.副本地图 && 主程.当前时间 > this.执行计时)
			{
				游戏脚本.地图执行(this);
				this.执行计时 = 主程.当前时间.AddMilliseconds(500.0);
				if (主程.当前时间 > this.关闭时间)
				{
					this.关闭副本();
				}
			}
		}

		public void 初始化副本守卫()
		{
			foreach (守卫刷新 item in this.守卫区域)
			{
				if (!item.禁止刷新 && 地图守卫.数据表.TryGetValue(item.守卫编号, out var value))
				{
					new 守卫实例(value, this, item.所处方向, item.所处坐标);
				}
			}
		}

		public void 初始化副本怪物(int 存活时间)
		{
			this.已初始化 = true;
			foreach (怪物刷新 item in this.怪物区域)
			{
				if (item.刷新列表 == null)
				{
					continue;
				}
				Point[] 出生范围;
				出生范围 = item.范围坐标.ToArray();
				刷新信息[] 刷新列表;
				刷新列表 = item.刷新列表;
				foreach (刷新信息 刷新信息 in 刷新列表)
				{
					if (游戏怪物.数据表.TryGetValue(刷新信息.怪物名字, out var value))
					{
						int num;
						num = (刷新信息.按秒复活 ? (刷新信息.复活间隔 * 1000) : (刷新信息.复活间隔 * 60 * 1000));
						for (int j = 0; j < 刷新信息.刷新数量; j++)
						{
							new 怪物实例(value, this, num, 出生范围, num == 0, 立即刷新: true).存活时间 = 主程.当前时间.AddMinutes(存活时间);
						}
					}
				}
			}
			this.固定怪物总数 = (uint)this.怪物区域.Sum((怪物刷新 O) => O.刷新列表.Sum((刷新信息 X) => X.刷新数量));
		}

		public void 关闭副本()
		{
			if (!this.副本地图)
			{
				return;
			}
			foreach (玩家实例 item in this.玩家列表.ToList())
			{
				if (item.对象死亡)
				{
					item.玩家请求复活();
				}
				else
				{
					item.玩家切换地图(地图处理网关.已分配地图(item.重生地图), 地图区域类型.复活区域);
				}
			}
			foreach (宠物实例 item2 in this.宠物列表.ToList())
			{
				if (item2.对象死亡)
				{
					item2.删除对象();
				}
				else
				{
					item2.宠物召回处理();
				}
			}
			foreach (物品实例 item3 in this.物品列表)
			{
				item3.物品消失处理();
			}
			foreach (地图对象 item4 in this.对象列表)
			{
				item4.删除对象();
			}
			this.副本关闭 = true;
		}

		public void 添加对象(地图对象 对象)
		{
			switch (对象.对象类型)
			{
			default:
				this.对象列表.Add(对象);
				break;
			case 游戏对象类型.物品:
				this.物品列表.Add(对象 as 物品实例);
				break;
			case 游戏对象类型.宠物:
				this.宠物列表.Add(对象 as 宠物实例);
				break;
			case 游戏对象类型.玩家:
				if (!this.已初始化)
				{
					this.初始化();
				}
				this.玩家列表.Add(对象 as 玩家实例);
				break;
			}
		}

		public void 初始化()
		{
			this.已初始化 = true;
			this.固定怪物总数 = 0u;
			this.存活怪物总数 = 0u;
			this.怪物复活次数 = 0u;
			this.怪物掉落次数 = 0L;
			if (this.副本地图)
			{
				return;
			}
			foreach (怪物刷新 item in this.怪物区域)
			{
				if (item.刷新列表 == null)
				{
					continue;
				}
				Point[] 出生范围;
				出生范围 = item.范围坐标.ToArray();
				刷新信息[] 刷新列表;
				刷新列表 = item.刷新列表;
				foreach (刷新信息 刷新信息 in 刷新列表)
				{
					if (游戏怪物.数据表.TryGetValue(刷新信息.怪物名字, out var value))
					{
						主窗口.添加怪物数据(value);
						int 复活间隔;
						复活间隔 = (刷新信息.按秒复活 ? (刷新信息.复活间隔 * 1000) : (刷新信息.复活间隔 * 60 * 1000));
						for (int j = 0; j < 刷新信息.刷新数量; j++)
						{
							怪物实例 怪物实例2;
							怪物实例2 = null;
							((刷新信息.定时刷新 == null || 刷新信息.定时刷新.Count <= 0) ? new 怪物实例(value, this, 复活间隔, 出生范围, 禁止复活: false, 立即刷新: true) : new 怪物实例(value, this, 刷新信息.定时刷新.ToArray(), 出生范围, 禁止复活: false, 立即刷新: false)).刷新配置信息 = 刷新信息;
						}
					}
				}
			}
			this.固定怪物总数 = (uint)this.怪物区域.Sum((怪物刷新 O) => O.刷新列表.Sum((刷新信息 X) => X.刷新数量));
			主窗口.添加地图数据(this);
		}

		public void 移除对象(地图对象 对象)
		{
			switch (对象.对象类型)
			{
			default:
				this.对象列表.Remove(对象);
				break;
			case 游戏对象类型.物品:
				this.物品列表.Remove(对象 as 物品实例);
				break;
			case 游戏对象类型.宠物:
				this.宠物列表.Remove(对象 as 宠物实例);
				break;
			case 游戏对象类型.玩家:
				this.玩家列表.Remove(对象 as 玩家实例);
				break;
			}
		}

		public void 地图公告(string 内容)
		{
			if (this.玩家列表.Count == 0)
			{
				return;
			}
			using MemoryStream memoryStream = new MemoryStream();
			using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(0);
			binaryWriter.Write(2415919107u);
			binaryWriter.Write(3);
			binaryWriter.Write(0);
			binaryWriter.Write(Encoding.UTF8.GetBytes(内容 + "\0"));
			byte[] 字节描述;
			字节描述 = memoryStream.ToArray();
			foreach (玩家实例 item in this.玩家列表)
			{
				item.网络连接?.发送封包(new 接收聊天消息
				{
					字节描述 = 字节描述
				});
			}
		}

		public override string ToString()
		{
			return this.地图模板.ToString();
		}

		public Point 随机坐标(地图区域类型 区域)
		{
			return 区域 switch
			{
				地图区域类型.复活区域 => this.复活区域?.随机坐标 ?? this.随机传送(), 
				地图区域类型.红名区域 => this.红名区域?.随机坐标 ?? this.随机传送(), 
				地图区域类型.传送区域 => this.传送区域?.随机坐标 ?? this.随机传送(), 
				地图区域类型.随机区域 => this.地图区域.FirstOrDefault((地图区域 O) => O.区域类型 == 地图区域类型.随机区域)?.随机坐标 ?? this.随机传送(), 
				_ => default(Point), 
			};
		}

		public Point 随机传送(Point 坐标)
		{
			foreach (地图区域 item in this.地图区域)
			{
				if (item.范围坐标.Contains(坐标) && item.区域类型 == 地图区域类型.随机区域)
				{
					return item.随机坐标;
				}
			}
			return default(Point);
		}

		public Point 随机传送()
		{
			Random 随机数;
			随机数 = 主程.随机数;
			int num;
			num = 随机数.Next(50);
			int num2;
			num2 = 随机数.Next(50);
			for (int i = this.地形数据.地图起点.X + num; i < this.地形数据.地图起点.X + this.地形数据.地图大小.X; i++)
			{
				for (int j = this.地形数据.地图起点.Y + num2; j < this.地形数据.地图起点.Y + this.地形数据.地图大小.Y; j++)
				{
					Point point;
					point = new Point(i, j);
					if (this.能否通行(point))
					{
						return point;
					}
				}
			}
			return default(Point);
		}

		public bool 坐标越界(Point 坐标)
		{
			if (坐标.X >= this.地图起点.X && 坐标.Y >= this.地图起点.Y && 坐标.X < this.地图终点.X)
			{
				return 坐标.Y >= this.地图终点.Y;
			}
			return true;
		}

		public bool 空间阻塞(Point 坐标)
		{
			if (this.安全区内(坐标))
			{
				return false;
			}
			foreach (地图对象 item in this[坐标])
			{
				if (item.阻塞网格)
				{
					return true;
				}
			}
			return false;
		}

		public int 阻塞数量(Point 坐标)
		{
			int num;
			num = 0;
			foreach (地图对象 item in this[坐标])
			{
				if (item.阻塞网格)
				{
					num++;
				}
			}
			return num;
		}

		public bool 地形阻塞(Point 坐标)
		{
			if (!this.坐标越界(坐标))
			{
				return (this.地形数据[坐标] & 0x10000000) != 268435456;
			}
			return true;
		}

		public bool 能否通行(Point 坐标)
		{
			if (!this.地形阻塞(坐标))
			{
				return !this.空间阻塞(坐标);
			}
			return false;
		}

		public ushort 地形高度(Point 坐标)
		{
			if (this.坐标越界(坐标))
			{
				return 0;
			}
			return (ushort)((this.地形数据[坐标] & 0xFFFF) - 30);
		}

		public bool 地形遮挡(Point 起点, Point 终点)
		{
			int num;
			num = 计算类.网格距离(起点, 终点);
			for (int i = 1; i < num; i++)
			{
				if (this.地形阻塞(计算类.前方坐标(起点, 终点, i)))
				{
					return true;
				}
			}
			return false;
		}

		public bool 自由区内(Point 坐标)
		{
			if (!this.坐标越界(坐标))
			{
				return (this.地形数据[坐标] & 0x20000) == 131072;
			}
			return false;
		}

		public bool 安全区内(Point 坐标)
		{
			if (!this.坐标越界(坐标))
			{
				if ((this.地形数据[坐标] & 0x40000) != 262144)
				{
					return (this.地形数据[坐标] & 0x100000) == 1048576;
				}
				return true;
			}
			return false;
		}

		public bool 摆摊区内(Point 坐标)
		{
			if (!this.坐标越界(坐标))
			{
				return (this.地形数据[坐标] & 0x100000) == 1048576;
			}
			return false;
		}

		public bool 掉落装备(Point 坐标, bool 红名)
		{
			if (!Settings.沙巴克掉装备 && (this.地图编号 == 152 || this.地图编号 == 178))
			{
				return false;
			}
			if (this.坐标越界(坐标))
			{
				return false;
			}
			if ((this.地形数据[坐标] & 0x400000) == 4194304)
			{
				return true;
			}
			if ((this.地形数据[坐标] & 0x800000) == 8388608 && 红名)
			{
				return true;
			}
			return false;
		}

		public List<玩家实例> 获取玩家列表()
		{
			return this.玩家列表.ToList();
		}

		public List<地图区域> 获取地图区域()
		{
			return this.地图区域.ToList();
		}

		public 怪物刷新 获取怪物区域(string 区域名)
		{
			return this.怪物区域.ToList().FirstOrDefault((怪物刷新 m) => m.区域名字 == 区域名);
		}

		public List<怪物刷新> 获取怪物区域()
		{
			return this.怪物区域.ToList();
		}

		public List<守卫刷新> 获取守卫区域()
		{
			return this.守卫区域.ToList();
		}

		public List<宠物实例> 获取宠物列表()
		{
			return this.宠物列表.ToList();
		}

		public List<物品实例> 获取物品列表()
		{
			return this.物品列表.ToList();
		}

		public List<守卫实例> 获取守卫列表()
		{
			List<守卫实例> list;
			list = new List<守卫实例>();
			foreach (地图对象 item2 in this.对象列表)
			{
				if (item2 is 守卫实例 item)
				{
					list.Add(item);
				}
			}
			return list;
		}

		public List<怪物实例> 获取怪物列表(string 名字 = "")
		{
			List<怪物实例> list;
			list = new List<怪物实例>();
			foreach (地图对象 item in this.对象列表)
			{
				if (item is 怪物实例 { 对象死亡: false } 怪物实例2 && (名字 == "" || 名字 == 怪物实例2.对象名字))
				{
					list.Add(怪物实例2);
				}
			}
			return list.ToList();
		}

		public List<地图对象> 获取对象列表()
		{
			return this.对象列表.ToList();
		}
	}
}
