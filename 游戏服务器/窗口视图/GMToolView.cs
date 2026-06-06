using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using 游戏服务器.管理命令;
using 游戏服务器.数据类;
using 游戏服务器.网络类;
using 游戏服务器.模板类;

namespace 游戏服务器.窗口视图
{
	// GM 工具面板: 图形化发放(元宝/金币/物品/装备/邮件) + 浏览并修改角色装备属性。
	// 所有对游戏数据的读写都通过 在游戏线程执行(...) 切到主循环线程, 避免与服务循环竞态。
	public partial class GMToolView : RibbonForm
	{
		// 装备列表行(纯值快照): UI 永不持有 live 装备数据; 来源+位置用于回写时在游戏线程重新定位。
		public sealed class 装备行
		{
			public string 来源 { get; set; }      // "穿戴" / "背包"

			[Browsable(false)]
			public byte 位置 { get; set; }          // 穿戴部位字节 或 背包格号(原始 key)

			public string 位置名 { get; set; }      // 部位中文名 或 背包[格号]

			public string 名称 { get; set; }

			public int 强化 { get; set; }

			public int 幸运 { get; set; }

			public bool 绑定 { get; set; }

			public int 当前持久 { get; set; }

			public int 最大持久 { get; set; }

			public string 随机属性 { get; set; }

			public string 进阶 { get; set; }

			[Browsable(false)]
			public List<int> 随机属性编号 { get; set; }

			[Browsable(false)]
			public int 开启精炼 { get; set; }

			[Browsable(false)]
			public int 精炼一 { get; set; }

			[Browsable(false)]
			public int 精炼二 { get; set; }

			[Browsable(false)]
			public int 精炼三 { get; set; }

			[Browsable(false)]
			public int 精炼次数 { get; set; }

			[Browsable(false)]
			public List<int> 孔颜色 { get; set; }

			[Browsable(false)]
			public string[] 镶嵌名 { get; set; }
		}

		// 词条下拉项: ToString 显示描述, 携带属性编号用于回写。
		private sealed class 词条项
		{
			public int 编号;

			public string 文本;

			public override string ToString()
			{
				return this.文本;
			}
		}

		private IContainer components;

		private RibbonControl ribbon;

		private RibbonPage ribbonPage1;

		private XtraTabControl 主页签;

		private XtraTabPage 发放页;

		private XtraTabPage 装备页;

		// —— 发放页控件 ——
		private GroupControl 发放分组;

		private TextEdit 发放角色名;

		private ComboBoxEdit 发放类型;

		private ComboBoxEdit 发放物品名;

		private SpinEdit 发放数量;

		private CheckEdit 发放绑定;

		private SimpleButton 发放按钮;

		private LabelControl 发放状态;

		// —— 装备页控件 ——
		private TextEdit 查询角色名;

		private SimpleButton 加载按钮;

		private LabelControl 装备状态;

		private GridControl 装备表格;

		private GridView 装备表格视图;

		private GroupControl 编辑分组;

		private LabelControl 选中信息;

		private SpinEdit 强化编辑;

		private SpinEdit 幸运编辑;

		private SpinEdit 当前持久编辑;

		private SpinEdit 最大持久编辑;

		private CheckEdit 绑定编辑;

		private SimpleButton 应用基础按钮;

		// —— 随机属性词条 ——
		private GroupControl 词条分组;

		private ComboBoxEdit[] 词条框;

		private SimpleButton 应用词条按钮;

		// —— 精炼 / 孔洞 / 镶嵌 ——
		private GroupControl 进阶分组;

		private SpinEdit 开启精炼编辑;

		private SpinEdit 精炼一编辑;

		private SpinEdit 精炼二编辑;

		private SpinEdit 精炼三编辑;

		private SpinEdit 精炼次数编辑;

		private SpinEdit 孔数编辑;

		private ComboBoxEdit[] 孔颜色框;

		private ComboBoxEdit[] 镶嵌框;

		private SimpleButton 应用进阶按钮;

		private bool 物品名已填充;

		private bool 词条已填充;

		private bool 镶嵌已填充;

		private 词条项 词条无;

		private readonly Dictionary<int, 词条项> 词条索引 = new Dictionary<int, 词条项>();

		public GMToolView()
		{
			this.InitializeComponent();
			this.填充物品名列表();
		}

		// 把一段闭包送到游戏主线程执行: 开服时入队由主循环 drain; 未开服时无并发, 直接前台跑。
		internal static void 在游戏线程执行(Action 动作)
		{
			if (动作 == null)
			{
				return;
			}
			if (主程.已经启动)
			{
				主程.外部命令.Enqueue(new 委托命令(动作));
			}
			else
			{
				动作();
			}
		}

		// 从游戏线程把结果安全推回 UI 线程(面板可能已关闭)。
		private void 在UI线程(Action 动作)
		{
			try
			{
				if (this.IsDisposed || !this.IsHandleCreated)
				{
					return;
				}
				this.BeginInvoke(动作);
			}
			catch
			{
			}
		}

		// 现成的发放类 GM 命令本身就是 优先后台执行, 直接入队/前台执行复用其逻辑。
		private static void 派发命令(GM命令 命令)
		{
			if (主程.已经启动)
			{
				主程.外部命令.Enqueue(命令);
			}
			else
			{
				命令.执行命令();
			}
		}

		// 在游戏线程按 来源+位置 重新定位 live 装备数据(找不到返回 null)。
		private static 装备数据 定位装备(角色数据 角色数据, string 来源, byte 位置)
		{
			if (来源 == "穿戴")
			{
				角色数据.角色装备.TryGetValue(位置, out var v);
				return v;
			}
			角色数据.角色背包.TryGetValue(位置, out var v2);
			return v2 as 装备数据;
		}

		// 灵石与孔色匹配规则: 原样照搬游戏 玩家镶嵌灵石(玩家实例.cs) 的按名校验, 只有同色灵石才能镶。
		private static bool 灵石匹配孔色(装备孔洞颜色 孔色, string 灵石名)
		{
			if (string.IsNullOrEmpty(灵石名))
			{
				return false;
			}
			switch (孔色)
			{
			case 装备孔洞颜色.绿色:
				return 灵石名.Contains("精绿灵石") || 灵石名.Contains("盈绿灵石");
			case 装备孔洞颜色.黄色:
				return 灵石名.Contains("守阳灵石") || 灵石名.Contains("新阳灵石");
			case 装备孔洞颜色.蓝色:
				return 灵石名.Contains("蔚蓝灵石") || 灵石名.Contains("透蓝灵石");
			case 装备孔洞颜色.紫色:
				return 灵石名.Contains("纯紫灵石") || 灵石名.Contains("韧紫灵石");
			case 装备孔洞颜色.灰色:
				return 灵石名.Contains("深灰灵石");
			case 装备孔洞颜色.橙色:
				return 灵石名.Contains("橙黄灵石");
			case 装备孔洞颜色.红色:
				return 灵石名.Contains("驭朱灵石") || 灵石名.Contains("命朱灵石");
			case 装备孔洞颜色.褐色:
				return 灵石名.Contains("赤褐灵石");
			case 装备孔洞颜色.多彩:
				return 灵石名.Contains("幻彩灵石");
			default:
				return false;
			}
		}

		private static string[] 取镶嵌名(装备数据 装备)
		{
			string[] 数组 = new string[3];
			foreach (KeyValuePair<byte, 游戏物品> kv in 装备.镶嵌灵石)
			{
				if (kv.Key < 3)
				{
					数组[kv.Key] = kv.Value?.物品名字;
				}
			}
			return 数组;
		}

		// 改完装备后在线推送、记审计、刷新列表。在游戏线程内调用。
		private void 同步并刷新(角色数据 角色数据, 装备数据 装备, string 日志)
		{
			角色数据.网络连接?.发送封包(new 玩家物品变动
			{
				物品描述 = 装备.字节描述()
			});
			主程.添加命令日志(日志);
			this.在UI线程(delegate
			{
				this.加载按钮_Click(null, null);
			});
		}

		// 物品/装备模板在启动时一次性加载、运行期不变, 读取键安全。可能在未加载数据时为 null。
		private void 填充物品名列表()
		{
			if (this.物品名已填充 || 游戏物品.检索表 == null || 游戏物品.检索表.Count == 0)
			{
				return;
			}
			this.发放物品名.Properties.Items.Clear();
			this.发放物品名.Properties.Items.AddRange(游戏物品.检索表.Keys.OrderBy((string x) => x).ToArray());
			this.物品名已填充 = true;
		}

		// 随机属性词条池(随机属性.数据表)启动时加载、运行期不变。
		private void 填充词条列表()
		{
			if (this.词条已填充 || 随机属性.数据表 == null || 随机属性.数据表.Count == 0)
			{
				return;
			}
			this.词条无 = new 词条项 { 编号 = 0, 文本 = "（无）" };
			List<object> 列表 = new List<object> { this.词条无 };
			this.词条索引.Clear();
			foreach (随机属性 属性 in from a in 随机属性.数据表.Values orderby a.属性编号 select a)
			{
				词条项 项 = new 词条项
				{
					编号 = 属性.属性编号,
					文本 = $"[{属性.属性编号}] {属性.属性描述}"
				};
				this.词条索引[属性.属性编号] = 项;
				列表.Add(项);
			}
			object[] 数组 = 列表.ToArray();
			foreach (ComboBoxEdit 框 in this.词条框)
			{
				框.Properties.Items.Clear();
				框.Properties.Items.AddRange(数组);
				框.SelectedItem = this.词条无;
			}
			this.词条已填充 = true;
		}

		// 镶嵌下拉只列「合法灵石」: 取 灵石配置.数据表 的模板编号, 反查 游戏物品.数据表 拿名称。
		// 真实规则(玩家镶嵌灵石): 只有 灵石配置 里的物品能镶, 且孔位必须 < 孔洞颜色.Count, 一孔一灵石。
		private void 填充镶嵌列表()
		{
			if (this.镶嵌已填充 || 灵石配置.数据表 == null || 灵石配置.数据表.Count == 0 || 游戏物品.数据表 == null)
			{
				return;
			}
			List<string> 名单 = new List<string>();
			foreach (int 编号 in 灵石配置.数据表.Keys)
			{
				if (游戏物品.数据表.TryGetValue(编号, out var 模板) && 模板 != null)
				{
					名单.Add(模板.物品名字);
				}
			}
			object[] 数组 = (from n in 名单.Distinct() orderby n select n).Cast<object>().ToArray();
			foreach (ComboBoxEdit 框 in this.镶嵌框)
			{
				框.Properties.Items.Clear();
				框.Properties.Items.AddRange(数组);
			}
			this.镶嵌已填充 = true;
		}

		private void 发放物品名_Enter(object sender, EventArgs e)
		{
			this.填充物品名列表();
		}

		private void 镶嵌框_Enter(object sender, EventArgs e)
		{
			this.填充镶嵌列表();
		}

		private void 发放按钮_Click(object sender, EventArgs e)
		{
			string 角色名 = (this.发放角色名.Text ?? string.Empty).Trim();
			if (角色名.Length == 0)
			{
				this.设置发放状态("请填写角色名");
				return;
			}
			int 数量 = (int)this.发放数量.Value;
			if (数量 < 1)
			{
				数量 = 1;
			}
			bool 绑定 = this.发放绑定.Checked;
			string 类型 = this.发放类型.Text;
			string 物品名 = (this.发放物品名.Text ?? string.Empty).Trim();

			switch (类型)
			{
			case "元宝":
				派发命令(new 添加元宝 { 角色名字 = 角色名, 元宝数量 = (uint)数量 });
				break;
			case "金币":
				派发命令(new 添加金币 { 角色名字 = 角色名, 金币数量 = (uint)数量 });
				break;
			case "物品":
				if (物品名.Length == 0)
				{
					this.设置发放状态("请选择/填写物品名");
					return;
				}
				派发命令(new 添加物品 { 角色名字 = 角色名, 物品名字 = 物品名, 物品数量 = 数量, 是否绑定 = 绑定 });
				break;
			case "装备":
				if (物品名.Length == 0)
				{
					this.设置发放状态("请选择/填写装备名");
					return;
				}
				// 制造物品: 装备会随机生成属性、进背包空格
				派发命令(new 制造物品 { 角色名字 = 角色名, 物品名字 = 物品名 });
				break;
			case "邮件物品":
				if (物品名.Length == 0)
				{
					this.设置发放状态("请选择/填写物品名");
					return;
				}
				this.邮件发放(角色名, 物品名, 数量, 绑定);
				break;
			default:
				this.设置发放状态("未知发放类型");
				return;
			}
			主程.添加命令日志($"=> GM工具 发放 [{类型}] 给 {角色名} (数量 {数量}{(绑定 ? ", 绑定" : string.Empty)})");
			this.设置发放状态($"已提交: {类型} -> {角色名} x{数量}; 详见命令日志");
		}

		// 邮件发放(在线离线通用): 在游戏线程定位角色, 走 角色数据.发送邮件(物品编号)。
		private void 邮件发放(string 角色名, string 物品名, int 数量, bool 绑定)
		{
			在游戏线程执行(delegate
			{
				if (!游戏数据网关.角色数据表.检索表.TryGetValue(角色名, out var value) || !(value is 角色数据 角色数据))
				{
					主程.添加命令日志("<= GM工具 邮件发放失败, 角色不存在: " + 角色名);
					return;
				}
				if (!游戏物品.检索表.TryGetValue(物品名, out var 模板))
				{
					主程.添加命令日志("<= GM工具 邮件发放失败, 物品不存在: " + 物品名);
					return;
				}
				角色数据.发送邮件(null, "GM发放", "管理员为您发放物品，请查收。", 模板.物品编号, 数量, 绑定);
				主程.添加命令日志($"<= GM工具 已向 {角色名} 邮件发放 {物品名} x{数量}");
			});
		}

		private void 设置发放状态(string 文本)
		{
			this.发放状态.Text = 文本;
		}

		// —— 装备浏览 ——
		private void 加载按钮_Click(object sender, EventArgs e)
		{
			string 角色名 = (this.查询角色名.Text ?? string.Empty).Trim();
			if (角色名.Length == 0)
			{
				this.装备状态.Text = "请填写角色名";
				return;
			}
			this.装备状态.Text = "加载中...";
			在游戏线程执行(delegate
			{
				if (!游戏数据网关.角色数据表.检索表.TryGetValue(角色名, out var value) || !(value is 角色数据 角色数据))
				{
					this.在UI线程(delegate
					{
						this.装备表格.DataSource = null;
						this.装备状态.Text = "角色不存在: " + 角色名;
					});
					return;
				}
				List<装备行> 行列表 = new List<装备行>();
				foreach (KeyValuePair<byte, 装备数据> kv in 角色数据.角色装备)
				{
					if (kv.Value != null)
					{
						行列表.Add(this.构造装备行("穿戴", kv.Key, ((装备穿戴部位)kv.Key).ToString(), kv.Value));
					}
				}
				foreach (KeyValuePair<byte, 物品数据> kv2 in 角色数据.角色背包)
				{
					if (kv2.Value is 装备数据 装备数据)
					{
						行列表.Add(this.构造装备行("背包", kv2.Key, $"背包[{kv2.Key}]", 装备数据));
					}
				}
				bool 在线 = 角色数据.网络连接 != null;
				this.在UI线程(delegate
				{
					this.装备表格.DataSource = 行列表;
					this.装备表格视图.BestFitColumns();
					this.装备状态.Text = $"{角色名}（{(在线 ? "在线" : "离线")}）: 穿戴+背包装备 {行列表.Count} 件";
				});
			});
		}

		private 装备行 构造装备行(string 来源, byte 位置, string 位置名, 装备数据 装备)
		{
			List<string> 进阶 = new List<string>();
			if (装备.开启精炼.V > 0)
			{
				进阶.Add($"精炼{装备.开启精炼.V}");
			}
			int 孔数 = 装备.孔洞颜色.Count;
			if (孔数 > 0)
			{
				进阶.Add($"孔×{孔数}");
			}
			int 镶嵌数 = 装备.镶嵌灵石.Count;
			if (镶嵌数 > 0)
			{
				进阶.Add($"镶×{镶嵌数}");
			}
			return new 装备行
			{
				来源 = 来源,
				位置 = 位置,
				位置名 = 位置名,
				名称 = 装备.物品名字,
				强化 = 装备.升级次数.V,
				幸运 = 装备.幸运等级.V,
				绑定 = 装备.绑定物品.V,
				当前持久 = 装备.当前持久.V,
				最大持久 = 装备.最大持久.V,
				随机属性 = string.Join(" | ", from a in 装备.随机属性 select a.属性描述),
				进阶 = string.Join(" ", 进阶),
				随机属性编号 = (from a in 装备.随机属性 select a.属性编号).ToList(),
				开启精炼 = 装备.开启精炼.V,
				精炼一 = 装备.精炼值一.V,
				精炼二 = 装备.精炼值二.V,
				精炼三 = 装备.精炼值三.V,
				精炼次数 = 装备.精炼次数.V,
				孔颜色 = (from c in 装备.孔洞颜色 select (int)c).ToList(),
				镶嵌名 = 取镶嵌名(装备)
			};
		}

		private 装备行 当前选中行()
		{
			return this.装备表格视图.GetFocusedRow() as 装备行;
		}

		private void 装备表格视图_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
		{
			装备行 行 = this.当前选中行();
			if (行 == null)
			{
				this.选中信息.Text = "未选中装备";
				return;
			}
			this.选中信息.Text = $"选中: {行.位置名}  {行.名称}";
			this.强化编辑.Value = 行.强化;
			this.幸运编辑.Value = 行.幸运;
			this.当前持久编辑.Value = 行.当前持久;
			this.最大持久编辑.Value = 行.最大持久;
			this.绑定编辑.Checked = 行.绑定;
			// 词条
			this.填充词条列表();
			List<int> 编号列表 = 行.随机属性编号 ?? new List<int>();
			for (int i = 0; i < this.词条框.Length; i++)
			{
				int 编号 = ((i < 编号列表.Count) ? 编号列表[i] : 0);
				this.词条框[i].SelectedItem = ((编号 > 0 && this.词条索引.TryGetValue(编号, out var 项)) ? 项 : this.词条无);
			}
			// 精炼
			this.开启精炼编辑.Value = 行.开启精炼;
			this.精炼一编辑.Value = 行.精炼一;
			this.精炼二编辑.Value = 行.精炼二;
			this.精炼三编辑.Value = 行.精炼三;
			this.精炼次数编辑.Value = 行.精炼次数;
			// 孔洞
			List<int> 颜色列表 = 行.孔颜色 ?? new List<int>();
			this.孔数编辑.Value = 颜色列表.Count;
			for (int j = 0; j < this.孔颜色框.Length; j++)
			{
				// 空孔位默认黄色(下拉已无"无孔"项, 用无孔会显示空白)
				this.孔颜色框[j].SelectedItem = ((j < 颜色列表.Count) ? (装备孔洞颜色)颜色列表[j] : 装备孔洞颜色.黄色);
			}
			// 镶嵌
			this.填充镶嵌列表();
			string[] 镶嵌列表 = 行.镶嵌名 ?? new string[4];
			for (int k = 0; k < this.镶嵌框.Length; k++)
			{
				this.镶嵌框[k].Text = ((k < 镶嵌列表.Length) ? (镶嵌列表[k] ?? string.Empty) : string.Empty);
			}
		}

		// 应用前的公共检查: 返回选中行与角色名; 失败返回 false。
		private bool 取选中(out 装备行 行, out string 角色名)
		{
			行 = this.当前选中行();
			角色名 = (this.查询角色名.Text ?? string.Empty).Trim();
			if (行 == null)
			{
				this.装备状态.Text = "请先在表格中选中一件装备";
				return false;
			}
			if (角色名.Length == 0)
			{
				this.装备状态.Text = "请填写角色名";
				return false;
			}
			return true;
		}

		// —— Phase 4: 强化 / 幸运 / 持久 / 绑定 ——
		private void 应用基础按钮_Click(object sender, EventArgs e)
		{
			if (!this.取选中(out var 行, out var 角色名))
			{
				return;
			}
			byte 强化 = (byte)this.强化编辑.Value;
			if (强化 > 9)
			{
				this.装备状态.Text = "强化等级只能 0-9";
				return;
			}
			sbyte 幸运 = (sbyte)this.幸运编辑.Value;
			bool 绑定 = this.绑定编辑.Checked;
			int 当前持久 = (int)this.当前持久编辑.Value;
			int 最大持久 = (int)this.最大持久编辑.Value;
			string 来源 = 行.来源;
			byte 位置 = 行.位置;
			string 位置名 = 行.位置名;

			在游戏线程执行(delegate
			{
				if (!游戏数据网关.角色数据表.检索表.TryGetValue(角色名, out var value) || !(value is 角色数据 角色数据))
				{
					主程.添加命令日志("<= GM工具 改装备失败, 角色不存在: " + 角色名);
					return;
				}
				装备数据 装备 = 定位装备(角色数据, 来源, 位置);
				if (装备 == null)
				{
					主程.添加命令日志($"<= GM工具 改装备失败, {位置名} 已无装备(可能已被移动)");
					return;
				}
				装备.升级次数.V = 强化;
				switch (角色数据.角色职业.V)
				{
				case 游戏对象职业.法师:
					装备.升级魔法.V = 强化;
					break;
				case 游戏对象职业.刺客:
					装备.升级刺术.V = 强化;
					break;
				case 游戏对象职业.弓手:
					装备.升级弓术.V = 强化;
					break;
				case 游戏对象职业.道士:
					装备.升级道术.V = 强化;
					break;
				case 游戏对象职业.战士:
				case 游戏对象职业.龙枪:
					装备.升级攻击.V = 强化;
					break;
				}
				装备.幸运等级.V = 幸运;
				装备.绑定物品.V = 绑定;
				if (最大持久 > 0)
				{
					装备.最大持久.V = 最大持久;
					装备.当前持久.V = ((当前持久 > 最大持久) ? 最大持久 : 当前持久);
				}
				this.同步并刷新(角色数据, 装备, $"<= GM工具 改装备 {角色名}/{位置名}: 强化={强化} 幸运={幸运} 绑定={绑定} 持久={装备.当前持久.V}/{装备.最大持久.V}");
			});
		}

		// —— Phase 5: 随机属性词条 ——
		private void 应用词条按钮_Click(object sender, EventArgs e)
		{
			if (!this.取选中(out var 行, out var 角色名))
			{
				return;
			}
			List<int> 选中编号 = new List<int>();
			foreach (ComboBoxEdit 框 in this.词条框)
			{
				if (框.SelectedItem is 词条项 项 && 项.编号 > 0)
				{
					选中编号.Add(项.编号);
				}
			}
			string 来源 = 行.来源;
			byte 位置 = 行.位置;
			string 位置名 = 行.位置名;

			在游戏线程执行(delegate
			{
				if (!游戏数据网关.角色数据表.检索表.TryGetValue(角色名, out var value) || !(value is 角色数据 角色数据))
				{
					主程.添加命令日志("<= GM工具 改词条失败, 角色不存在: " + 角色名);
					return;
				}
				装备数据 装备 = 定位装备(角色数据, 来源, 位置);
				if (装备 == null)
				{
					主程.添加命令日志($"<= GM工具 改词条失败, {位置名} 已无装备");
					return;
				}
				List<随机属性> 新词条 = new List<随机属性>();
				foreach (int 编号 in 选中编号)
				{
					if (随机属性.数据表.TryGetValue(编号, out var 属性))
					{
						新词条.Add(属性);
					}
				}
				// 共享模板引用即可(与装备生成时一致), 一次性 SetValue 触发持久化。
				装备.随机属性.SetValue(新词条);
				this.同步并刷新(角色数据, 装备, $"<= GM工具 改词条 {角色名}/{位置名}: 共 {新词条.Count} 条 [{string.Join(",", 选中编号)}]");
			});
		}

		// —— Phase 6: 精炼 / 孔洞 / 镶嵌 ——
		private void 应用进阶按钮_Click(object sender, EventArgs e)
		{
			if (!this.取选中(out var 行, out var 角色名))
			{
				return;
			}
			byte 开启精炼 = (byte)this.开启精炼编辑.Value;
			ushort 精炼一 = (ushort)this.精炼一编辑.Value;
			ushort 精炼二 = (ushort)this.精炼二编辑.Value;
			ushort 精炼三 = (ushort)this.精炼三编辑.Value;
			ushort 精炼次数 = (ushort)this.精炼次数编辑.Value;
			int 孔数 = (int)this.孔数编辑.Value;
			List<装备孔洞颜色> 颜色列表 = new List<装备孔洞颜色>();
			for (int i = 0; i < 孔数 && i < this.孔颜色框.Length; i++)
			{
				颜色列表.Add((this.孔颜色框[i].SelectedItem is 装备孔洞颜色 颜色) ? 颜色 : 装备孔洞颜色.无孔);
			}
			string[] 镶嵌名 = (from 框 in this.镶嵌框 select (框.Text ?? string.Empty).Trim()).ToArray();
			string 来源 = 行.来源;
			byte 位置 = 行.位置;
			string 位置名 = 行.位置名;

			在游戏线程执行(delegate
			{
				if (!游戏数据网关.角色数据表.检索表.TryGetValue(角色名, out var value) || !(value is 角色数据 角色数据))
				{
					主程.添加命令日志("<= GM工具 改进阶失败, 角色不存在: " + 角色名);
					return;
				}
				装备数据 装备 = 定位装备(角色数据, 来源, 位置);
				if (装备 == null)
				{
					主程.添加命令日志($"<= GM工具 改进阶失败, {位置名} 已无装备");
					return;
				}
				装备.开启精炼.V = 开启精炼;
				装备.精炼值一.V = 精炼一;
				装备.精炼值二.V = 精炼二;
				装备.精炼值三.V = 精炼三;
				装备.精炼次数.V = 精炼次数;
				装备.孔洞颜色.SetValue(颜色列表);
				for (byte 孔 = 0; 孔 < 3; 孔++)
				{
					string 名 = ((孔 < 镶嵌名.Length) ? 镶嵌名[孔] : string.Empty);
					if (名.Length == 0)
					{
						装备.镶嵌灵石.Remove(孔); // 留空 = 清除该孔镶嵌
						continue;
					}
					if (孔 >= 孔数)
					{
						装备.镶嵌灵石.Remove(孔); // 孔数减少后, 清掉不存在孔上残留的灵石, 避免数据不一致
						continue;
					}
					if (!游戏物品.检索表.TryGetValue(名, out var 灵石) || 灵石配置.数据表 == null || !灵石配置.数据表.ContainsKey(灵石.物品编号))
					{
						主程.添加命令日志($"<= GM工具 镶嵌跳过 孔{孔 + 1}: '{名}' 不是有效灵石");
						continue;
					}
					装备孔洞颜色 孔色 = ((孔 < 颜色列表.Count) ? 颜色列表[孔] : 装备孔洞颜色.无孔);
					if (!灵石匹配孔色(孔色, 名))
					{
						// 对齐官方: 灵石颜色必须与孔颜色一致才能镶。
						主程.添加命令日志($"<= GM工具 镶嵌跳过 孔{孔 + 1}: 灵石[{名}]与孔色[{孔色}]不匹配");
						continue;
					}
					装备.镶嵌灵石[孔] = 灵石; // 与游戏一致: 镶嵌灵石[孔位] = 灵石的物品模板
				}
				this.同步并刷新(角色数据, 装备, $"<= GM工具 改进阶 {角色名}/{位置名}: 精炼{开启精炼}({精炼一}/{精炼二}/{精炼三}) 孔×{颜色列表.Count}");
			});
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new Container();
			this.ribbon = new RibbonControl();
			this.ribbonPage1 = new RibbonPage();
			this.主页签 = new XtraTabControl();
			this.发放页 = new XtraTabPage();
			this.装备页 = new XtraTabPage();
			this.发放分组 = new GroupControl();
			this.发放角色名 = new TextEdit();
			this.发放类型 = new ComboBoxEdit();
			this.发放物品名 = new ComboBoxEdit();
			this.发放数量 = new SpinEdit();
			this.发放绑定 = new CheckEdit();
			this.发放按钮 = new SimpleButton();
			this.发放状态 = new LabelControl();
			this.查询角色名 = new TextEdit();
			this.加载按钮 = new SimpleButton();
			this.装备状态 = new LabelControl();
			this.装备表格 = new GridControl();
			this.装备表格视图 = new GridView();
			this.编辑分组 = new GroupControl();
			this.选中信息 = new LabelControl();
			this.强化编辑 = new SpinEdit();
			this.幸运编辑 = new SpinEdit();
			this.当前持久编辑 = new SpinEdit();
			this.最大持久编辑 = new SpinEdit();
			this.绑定编辑 = new CheckEdit();
			this.应用基础按钮 = new SimpleButton();
			this.词条分组 = new GroupControl();
			this.应用词条按钮 = new SimpleButton();
			this.词条框 = new ComboBoxEdit[4] { new ComboBoxEdit(), new ComboBoxEdit(), new ComboBoxEdit(), new ComboBoxEdit() };
			this.进阶分组 = new GroupControl();
			this.开启精炼编辑 = new SpinEdit();
			this.精炼一编辑 = new SpinEdit();
			this.精炼二编辑 = new SpinEdit();
			this.精炼三编辑 = new SpinEdit();
			this.精炼次数编辑 = new SpinEdit();
			this.孔数编辑 = new SpinEdit();
			this.应用进阶按钮 = new SimpleButton();
			this.孔颜色框 = new ComboBoxEdit[3] { new ComboBoxEdit(), new ComboBoxEdit(), new ComboBoxEdit() };
			this.镶嵌框 = new ComboBoxEdit[3] { new ComboBoxEdit(), new ComboBoxEdit(), new ComboBoxEdit() };
			((ISupportInitialize)this.ribbon).BeginInit();
			((ISupportInitialize)this.主页签).BeginInit();
			this.主页签.SuspendLayout();
			this.发放页.SuspendLayout();
			this.装备页.SuspendLayout();
			((ISupportInitialize)this.发放分组).BeginInit();
			this.发放分组.SuspendLayout();
			((ISupportInitialize)this.发放角色名.Properties).BeginInit();
			((ISupportInitialize)this.发放类型.Properties).BeginInit();
			((ISupportInitialize)this.发放物品名.Properties).BeginInit();
			((ISupportInitialize)this.发放数量.Properties).BeginInit();
			((ISupportInitialize)this.发放绑定.Properties).BeginInit();
			((ISupportInitialize)this.查询角色名.Properties).BeginInit();
			((ISupportInitialize)this.装备表格).BeginInit();
			((ISupportInitialize)this.装备表格视图).BeginInit();
			((ISupportInitialize)this.编辑分组).BeginInit();
			this.编辑分组.SuspendLayout();
			((ISupportInitialize)this.强化编辑.Properties).BeginInit();
			((ISupportInitialize)this.幸运编辑.Properties).BeginInit();
			((ISupportInitialize)this.当前持久编辑.Properties).BeginInit();
			((ISupportInitialize)this.最大持久编辑.Properties).BeginInit();
			((ISupportInitialize)this.绑定编辑.Properties).BeginInit();
			((ISupportInitialize)this.词条分组).BeginInit();
			this.词条分组.SuspendLayout();
			((ISupportInitialize)this.进阶分组).BeginInit();
			this.进阶分组.SuspendLayout();
			((ISupportInitialize)this.开启精炼编辑.Properties).BeginInit();
			((ISupportInitialize)this.精炼一编辑.Properties).BeginInit();
			((ISupportInitialize)this.精炼二编辑.Properties).BeginInit();
			((ISupportInitialize)this.精炼三编辑.Properties).BeginInit();
			((ISupportInitialize)this.精炼次数编辑.Properties).BeginInit();
			((ISupportInitialize)this.孔数编辑.Properties).BeginInit();
			foreach (ComboBoxEdit 框 in this.词条框)
			{
				((ISupportInitialize)框.Properties).BeginInit();
			}
			foreach (ComboBoxEdit 框 in this.孔颜色框)
			{
				((ISupportInitialize)框.Properties).BeginInit();
			}
			foreach (ComboBoxEdit 框 in this.镶嵌框)
			{
				((ISupportInitialize)框.Properties).BeginInit();
			}
			this.SuspendLayout();
			//
			// ribbon
			//
			this.ribbon.ExpandCollapseItem.Id = 0;
			this.ribbon.Items.AddRange(new DevExpress.XtraBars.BarItem[] { this.ribbon.ExpandCollapseItem });
			this.ribbon.Location = new Point(0, 0);
			this.ribbon.MaxItemId = 1;
			this.ribbon.MdiMergeStyle = RibbonMdiMergeStyle.Always;
			this.ribbon.Name = "ribbon";
			this.ribbon.Pages.AddRange(new RibbonPage[] { this.ribbonPage1 });
			this.ribbon.Size = new Size(1360, 32);
			//
			// ribbonPage1
			//
			this.ribbonPage1.Name = "ribbonPage1";
			this.ribbonPage1.Text = "主页";
			//
			// 主页签
			//
			this.主页签.Dock = DockStyle.Fill;
			this.主页签.Location = new Point(0, 32);
			this.主页签.Name = "主页签";
			this.主页签.SelectedTabPage = this.发放页;
			this.主页签.Size = new Size(1360, 628);
			this.主页签.TabIndex = 0;
			this.主页签.TabPages.AddRange(new XtraTabPage[] { this.发放页, this.装备页 });
			//
			// 发放页
			//
			this.发放页.Controls.Add(this.发放状态);
			this.发放页.Controls.Add(this.发放分组);
			this.发放页.Name = "发放页";
			this.发放页.Size = new Size(1354, 599);
			this.发放页.Text = "发放";
			//
			// 装备页
			//
			this.装备页.Controls.Add(this.进阶分组);
			this.装备页.Controls.Add(this.词条分组);
			this.装备页.Controls.Add(this.编辑分组);
			this.装备页.Controls.Add(this.装备表格);
			this.装备页.Controls.Add(this.装备状态);
			this.装备页.Controls.Add(this.加载按钮);
			this.装备页.Controls.Add(this.查询角色名);
			this.装备页.Name = "装备页";
			this.装备页.Size = new Size(1354, 599);
			this.装备页.Text = "装备 / 改属性";
			//
			// 发放分组
			//
			this.发放分组.Controls.Add(this.发放按钮);
			this.发放分组.Controls.Add(this.发放绑定);
			this.发放分组.Controls.Add(this.发放数量);
			this.发放分组.Controls.Add(this.发放物品名);
			this.发放分组.Controls.Add(this.发放类型);
			this.发放分组.Controls.Add(this.发放角色名);
			this.发放分组.Controls.Add(this.制作标签("角色名:", 18, 38, 70));
			this.发放分组.Controls.Add(this.制作标签("类型:", 18, 68, 70));
			this.发放分组.Controls.Add(this.制作标签("物品名:", 18, 98, 70));
			this.发放分组.Controls.Add(this.制作标签("数量:", 18, 128, 70));
			this.发放分组.Location = new Point(12, 12);
			this.发放分组.Name = "发放分组";
			this.发放分组.Size = new Size(460, 230);
			this.发放分组.Text = "发放 元宝 / 金币 / 物品 / 装备 / 邮件";
			//
			// 发放角色名
			//
			this.发放角色名.Location = new Point(95, 35);
			this.发放角色名.Name = "发放角色名";
			this.发放角色名.Properties.NullValuePrompt = "角色名";
			this.发放角色名.Size = new Size(330, 20);
			this.发放角色名.TabIndex = 0;
			//
			// 发放类型
			//
			this.发放类型.Location = new Point(95, 65);
			this.发放类型.Name = "发放类型";
			this.发放类型.Properties.Items.AddRange(new object[] { "元宝", "金币", "物品", "装备", "邮件物品" });
			this.发放类型.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.发放类型.SelectedIndex = 0;
			this.发放类型.Size = new Size(150, 20);
			this.发放类型.TabIndex = 1;
			//
			// 发放物品名
			//
			this.发放物品名.Location = new Point(95, 95);
			this.发放物品名.Name = "发放物品名";
			this.发放物品名.Properties.NullValuePrompt = "物品/装备名（物品/装备/邮件 时填，可搜索）";
			this.发放物品名.Size = new Size(330, 20);
			this.发放物品名.TabIndex = 2;
			this.发放物品名.Enter += this.发放物品名_Enter;
			//
			// 发放数量
			//
			this.发放数量.EditValue = new decimal(new int[] { 1, 0, 0, 0 });
			this.发放数量.Location = new Point(95, 125);
			this.发放数量.Name = "发放数量";
			this.发放数量.Properties.IsFloatValue = false;
			this.发放数量.Properties.MaxValue = new decimal(new int[] { 1000000000, 0, 0, 0 });
			this.发放数量.Properties.MinValue = new decimal(new int[] { 1, 0, 0, 0 });
			this.发放数量.Size = new Size(150, 20);
			this.发放数量.TabIndex = 3;
			//
			// 发放绑定
			//
			this.发放绑定.Location = new Point(260, 125);
			this.发放绑定.Name = "发放绑定";
			this.发放绑定.Properties.Caption = "绑定";
			this.发放绑定.Size = new Size(75, 20);
			this.发放绑定.TabIndex = 4;
			//
			// 发放按钮
			//
			this.发放按钮.Location = new Point(95, 165);
			this.发放按钮.Name = "发放按钮";
			this.发放按钮.Size = new Size(120, 32);
			this.发放按钮.TabIndex = 5;
			this.发放按钮.Text = "发放";
			this.发放按钮.Click += this.发放按钮_Click;
			//
			// 发放状态
			//
			this.发放状态.Location = new Point(15, 252);
			this.发放状态.Name = "发放状态";
			this.发放状态.Size = new Size(0, 14);
			this.发放状态.TabIndex = 1;
			//
			// 查询角色名
			//
			this.查询角色名.Location = new Point(12, 12);
			this.查询角色名.Name = "查询角色名";
			this.查询角色名.Properties.NullValuePrompt = "角色名";
			this.查询角色名.Size = new Size(200, 20);
			this.查询角色名.TabIndex = 0;
			//
			// 加载按钮
			//
			this.加载按钮.Location = new Point(220, 10);
			this.加载按钮.Name = "加载按钮";
			this.加载按钮.Size = new Size(90, 24);
			this.加载按钮.TabIndex = 1;
			this.加载按钮.Text = "加载装备";
			this.加载按钮.Click += this.加载按钮_Click;
			//
			// 装备状态
			//
			this.装备状态.Location = new Point(322, 16);
			this.装备状态.Name = "装备状态";
			this.装备状态.Size = new Size(0, 14);
			this.装备状态.TabIndex = 2;
			//
			// 装备表格
			//
			this.装备表格.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			this.装备表格.Location = new Point(12, 42);
			this.装备表格.MainView = this.装备表格视图;
			this.装备表格.Name = "装备表格";
			this.装备表格.Size = new Size(610, 545);
			this.装备表格.TabIndex = 3;
			this.装备表格.ViewCollection.AddRange(new BaseView[] { this.装备表格视图 });
			//
			// 装备表格视图
			//
			this.装备表格视图.GridControl = this.装备表格;
			this.装备表格视图.Name = "装备表格视图";
			this.装备表格视图.OptionsBehavior.Editable = false;
			this.装备表格视图.OptionsView.ColumnAutoWidth = false;
			this.装备表格视图.FocusedRowChanged += this.装备表格视图_FocusedRowChanged;
			//
			// 编辑分组
			//
			this.编辑分组.Controls.Add(this.应用基础按钮);
			this.编辑分组.Controls.Add(this.绑定编辑);
			this.编辑分组.Controls.Add(this.最大持久编辑);
			this.编辑分组.Controls.Add(this.当前持久编辑);
			this.编辑分组.Controls.Add(this.幸运编辑);
			this.编辑分组.Controls.Add(this.强化编辑);
			this.编辑分组.Controls.Add(this.选中信息);
			this.编辑分组.Location = new Point(632, 42);
			this.编辑分组.Name = "编辑分组";
			this.编辑分组.Size = new Size(350, 285);
			this.编辑分组.Text = "改属性（先在左侧选中一件装备）";
			//
			// 选中信息
			//
			this.选中信息.Location = new Point(15, 30);
			this.选中信息.Name = "选中信息";
			this.选中信息.Size = new Size(48, 14);
			this.选中信息.TabIndex = 0;
			this.选中信息.Text = "未选中装备";
			//
			// 强化编辑
			//
			this.强化编辑.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
			this.强化编辑.Location = new Point(120, 60);
			this.强化编辑.Name = "强化编辑";
			this.强化编辑.Properties.IsFloatValue = false;
			this.强化编辑.Properties.MaxValue = new decimal(new int[] { 9, 0, 0, 0 });
			this.强化编辑.Size = new Size(120, 20);
			this.强化编辑.TabIndex = 1;
			//
			// 幸运编辑
			//
			this.幸运编辑.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
			this.幸运编辑.Location = new Point(120, 90);
			this.幸运编辑.Name = "幸运编辑";
			this.幸运编辑.Properties.IsFloatValue = false;
			this.幸运编辑.Properties.MaxValue = new decimal(new int[] { 7, 0, 0, 0 });
			this.幸运编辑.Size = new Size(120, 20);
			this.幸运编辑.TabIndex = 2;
			//
			// 当前持久编辑
			//
			this.当前持久编辑.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
			this.当前持久编辑.Location = new Point(120, 120);
			this.当前持久编辑.Name = "当前持久编辑";
			this.当前持久编辑.Properties.IsFloatValue = false;
			this.当前持久编辑.Properties.MaxValue = new decimal(new int[] { 1000000000, 0, 0, 0 });
			this.当前持久编辑.Size = new Size(120, 20);
			this.当前持久编辑.TabIndex = 3;
			//
			// 最大持久编辑
			//
			this.最大持久编辑.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
			this.最大持久编辑.Location = new Point(120, 150);
			this.最大持久编辑.Name = "最大持久编辑";
			this.最大持久编辑.Properties.IsFloatValue = false;
			this.最大持久编辑.Properties.MaxValue = new decimal(new int[] { 1000000000, 0, 0, 0 });
			this.最大持久编辑.Size = new Size(120, 20);
			this.最大持久编辑.TabIndex = 4;
			//
			// 绑定编辑
			//
			this.绑定编辑.Location = new Point(15, 180);
			this.绑定编辑.Name = "绑定编辑";
			this.绑定编辑.Properties.Caption = "绑定";
			this.绑定编辑.Size = new Size(120, 20);
			this.绑定编辑.TabIndex = 5;
			//
			// 应用基础按钮
			//
			this.应用基础按钮.Location = new Point(15, 215);
			this.应用基础按钮.Name = "应用基础按钮";
			this.应用基础按钮.Size = new Size(225, 32);
			this.应用基础按钮.TabIndex = 6;
			this.应用基础按钮.Text = "应用 强化 / 幸运 / 持久 / 绑定";
			this.应用基础按钮.Click += this.应用基础按钮_Click;
			//
			// 编辑分组标签
			//
			this.编辑分组.Controls.Add(this.制作标签("强化(0-9):", 15, 63));
			this.编辑分组.Controls.Add(this.制作标签("幸运(0-7):", 15, 93));
			this.编辑分组.Controls.Add(this.制作标签("当前持久:", 15, 123));
			this.编辑分组.Controls.Add(this.制作标签("最大持久:", 15, 153));
			//
			// 词条分组
			//
			this.词条分组.Controls.Add(this.应用词条按钮);
			this.词条分组.Location = new Point(632, 335);
			this.词条分组.Name = "词条分组";
			this.词条分组.Size = new Size(350, 200);
			this.词条分组.Text = "随机属性词条（最多 4 条，留“（无）”即清除）";
			//
			// 词条框 1-4
			//
			for (int i = 0; i < this.词条框.Length; i++)
			{
				ComboBoxEdit 框 = this.词条框[i];
				框.Location = new Point(70, 30 + i * 30);
				框.Name = "词条框" + (i + 1);
				框.Size = new Size(265, 20);
				框.TabIndex = i;
				框.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
				this.词条分组.Controls.Add(框);
				this.词条分组.Controls.Add(this.制作标签("词条" + (i + 1), 15, 33 + i * 30, 50));
			}
			//
			// 应用词条按钮
			//
			this.应用词条按钮.Location = new Point(70, 155);
			this.应用词条按钮.Name = "应用词条按钮";
			this.应用词条按钮.Size = new Size(265, 30);
			this.应用词条按钮.TabIndex = 4;
			this.应用词条按钮.Text = "应用 随机属性词条";
			this.应用词条按钮.Click += this.应用词条按钮_Click;
			//
			// 进阶分组
			//
			this.进阶分组.Controls.Add(this.应用进阶按钮);
			this.进阶分组.Controls.Add(this.孔数编辑);
			this.进阶分组.Controls.Add(this.精炼次数编辑);
			this.进阶分组.Controls.Add(this.精炼三编辑);
			this.进阶分组.Controls.Add(this.精炼二编辑);
			this.进阶分组.Controls.Add(this.精炼一编辑);
			this.进阶分组.Controls.Add(this.开启精炼编辑);
			this.进阶分组.Location = new Point(992, 42);
			this.进阶分组.Name = "进阶分组";
			this.进阶分组.Size = new Size(350, 545);
			this.进阶分组.Text = "精炼 / 孔洞 / 镶嵌";
			//
			// 开启精炼编辑
			//
			this.开启精炼编辑.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
			this.开启精炼编辑.Location = new Point(150, 27);
			this.开启精炼编辑.Name = "开启精炼编辑";
			this.开启精炼编辑.Properties.IsFloatValue = false;
			this.开启精炼编辑.Properties.MaxValue = new decimal(new int[] { 3, 0, 0, 0 });
			this.开启精炼编辑.Size = new Size(180, 20);
			this.开启精炼编辑.TabIndex = 0;
			//
			// 精炼一编辑
			//
			this.精炼一编辑.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
			this.精炼一编辑.Location = new Point(150, 57);
			this.精炼一编辑.Name = "精炼一编辑";
			this.精炼一编辑.Properties.IsFloatValue = false;
			this.精炼一编辑.Properties.MaxValue = new decimal(new int[] { 65535, 0, 0, 0 });
			this.精炼一编辑.Size = new Size(180, 20);
			this.精炼一编辑.TabIndex = 1;
			//
			// 精炼二编辑
			//
			this.精炼二编辑.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
			this.精炼二编辑.Location = new Point(150, 87);
			this.精炼二编辑.Name = "精炼二编辑";
			this.精炼二编辑.Properties.IsFloatValue = false;
			this.精炼二编辑.Properties.MaxValue = new decimal(new int[] { 65535, 0, 0, 0 });
			this.精炼二编辑.Size = new Size(180, 20);
			this.精炼二编辑.TabIndex = 2;
			//
			// 精炼三编辑
			//
			this.精炼三编辑.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
			this.精炼三编辑.Location = new Point(150, 117);
			this.精炼三编辑.Name = "精炼三编辑";
			this.精炼三编辑.Properties.IsFloatValue = false;
			this.精炼三编辑.Properties.MaxValue = new decimal(new int[] { 65535, 0, 0, 0 });
			this.精炼三编辑.Size = new Size(180, 20);
			this.精炼三编辑.TabIndex = 3;
			//
			// 精炼次数编辑
			//
			this.精炼次数编辑.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
			this.精炼次数编辑.Location = new Point(150, 147);
			this.精炼次数编辑.Name = "精炼次数编辑";
			this.精炼次数编辑.Properties.IsFloatValue = false;
			this.精炼次数编辑.Properties.MaxValue = new decimal(new int[] { 65535, 0, 0, 0 });
			this.精炼次数编辑.Size = new Size(180, 20);
			this.精炼次数编辑.TabIndex = 4;
			//
			// 孔数编辑
			//
			this.孔数编辑.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
			this.孔数编辑.Location = new Point(150, 182);
			this.孔数编辑.Name = "孔数编辑";
			this.孔数编辑.Properties.IsFloatValue = false;
			this.孔数编辑.Properties.MaxValue = new decimal(new int[] { 3, 0, 0, 0 });
			this.孔数编辑.Size = new Size(180, 20);
			this.孔数编辑.TabIndex = 5;
			//
			// 进阶分组标签
			//
			this.进阶分组.Controls.Add(this.制作标签("开启精炼(0-3):", 15, 30, 130));
			this.进阶分组.Controls.Add(this.制作标签("精炼值一:", 15, 60, 130));
			this.进阶分组.Controls.Add(this.制作标签("精炼值二:", 15, 90, 130));
			this.进阶分组.Controls.Add(this.制作标签("精炼值三:", 15, 120, 130));
			this.进阶分组.Controls.Add(this.制作标签("精炼次数:", 15, 150, 130));
			this.进阶分组.Controls.Add(this.制作标签("孔数(0-3):", 15, 185, 130));
			//
			// 孔颜色框 1-4
			//
			// 孔色下拉去掉「无孔」(打孔后的孔必有真实颜色); 红黄蓝绿紫灰橙为官方7色, 褐/多彩为本引擎扩展。
			object[] 颜色值 = (from 装备孔洞颜色 c in Enum.GetValues(typeof(装备孔洞颜色)) where c != 装备孔洞颜色.无孔 select (object)c).ToArray();
			for (int j = 0; j < this.孔颜色框.Length; j++)
			{
				ComboBoxEdit 框2 = this.孔颜色框[j];
				框2.Location = new Point(90, 215 + j * 30);
				框2.Name = "孔颜色框" + (j + 1);
				框2.Size = new Size(240, 20);
				框2.TabIndex = 6 + j;
				框2.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
				框2.Properties.Items.AddRange(颜色值);
				框2.SelectedItem = 装备孔洞颜色.黄色;
				this.进阶分组.Controls.Add(框2);
				this.进阶分组.Controls.Add(this.制作标签("孔" + (j + 1) + "颜色:", 15, 218 + j * 30, 70));
			}
			//
			// 镶嵌框 1-4
			//
			this.进阶分组.Controls.Add(this.制作标签("孔镶嵌灵石（留空=清除；下拉仅合法灵石；孔位须<孔数）:", 15, 338, 320));
			for (int k = 0; k < this.镶嵌框.Length; k++)
			{
				ComboBoxEdit 框3 = this.镶嵌框[k];
				框3.Location = new Point(90, 360 + k * 30);
				框3.Name = "镶嵌框" + (k + 1);
				框3.Size = new Size(240, 20);
				框3.TabIndex = 10 + k;
				框3.Enter += this.镶嵌框_Enter;
				this.进阶分组.Controls.Add(框3);
				this.进阶分组.Controls.Add(this.制作标签("孔" + (k + 1) + "镶嵌:", 15, 363 + k * 30, 70));
			}
			//
			// 应用进阶按钮
			//
			this.应用进阶按钮.Location = new Point(90, 488);
			this.应用进阶按钮.Name = "应用进阶按钮";
			this.应用进阶按钮.Size = new Size(240, 32);
			this.应用进阶按钮.TabIndex = 14;
			this.应用进阶按钮.Text = "应用 精炼 / 孔洞 / 镶嵌";
			this.应用进阶按钮.Click += this.应用进阶按钮_Click;
			//
			// GMToolView
			//
			this.AutoScaleDimensions = new SizeF(7f, 14f);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.ClientSize = new Size(1360, 660);
			this.Controls.Add(this.主页签);
			this.Controls.Add(this.ribbon);
			this.Name = "GMToolView";
			this.Ribbon = this.ribbon;
			this.Text = "GM工具";
			((ISupportInitialize)this.ribbon).EndInit();
			((ISupportInitialize)this.主页签).EndInit();
			this.主页签.ResumeLayout(false);
			this.发放页.ResumeLayout(false);
			this.发放页.PerformLayout();
			this.装备页.ResumeLayout(false);
			this.装备页.PerformLayout();
			((ISupportInitialize)this.发放分组).EndInit();
			this.发放分组.ResumeLayout(false);
			((ISupportInitialize)this.发放角色名.Properties).EndInit();
			((ISupportInitialize)this.发放类型.Properties).EndInit();
			((ISupportInitialize)this.发放物品名.Properties).EndInit();
			((ISupportInitialize)this.发放数量.Properties).EndInit();
			((ISupportInitialize)this.发放绑定.Properties).EndInit();
			((ISupportInitialize)this.查询角色名.Properties).EndInit();
			((ISupportInitialize)this.装备表格).EndInit();
			((ISupportInitialize)this.装备表格视图).EndInit();
			((ISupportInitialize)this.编辑分组).EndInit();
			this.编辑分组.ResumeLayout(false);
			((ISupportInitialize)this.强化编辑.Properties).EndInit();
			((ISupportInitialize)this.幸运编辑.Properties).EndInit();
			((ISupportInitialize)this.当前持久编辑.Properties).EndInit();
			((ISupportInitialize)this.最大持久编辑.Properties).EndInit();
			((ISupportInitialize)this.绑定编辑.Properties).EndInit();
			((ISupportInitialize)this.词条分组).EndInit();
			this.词条分组.ResumeLayout(false);
			((ISupportInitialize)this.进阶分组).EndInit();
			this.进阶分组.ResumeLayout(false);
			((ISupportInitialize)this.开启精炼编辑.Properties).EndInit();
			((ISupportInitialize)this.精炼一编辑.Properties).EndInit();
			((ISupportInitialize)this.精炼二编辑.Properties).EndInit();
			((ISupportInitialize)this.精炼三编辑.Properties).EndInit();
			((ISupportInitialize)this.精炼次数编辑.Properties).EndInit();
			((ISupportInitialize)this.孔数编辑.Properties).EndInit();
			foreach (ComboBoxEdit 框4 in this.词条框)
			{
				((ISupportInitialize)框4.Properties).EndInit();
			}
			foreach (ComboBoxEdit 框4 in this.孔颜色框)
			{
				((ISupportInitialize)框4.Properties).EndInit();
			}
			foreach (ComboBoxEdit 框4 in this.镶嵌框)
			{
				((ISupportInitialize)框4.Properties).EndInit();
			}
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		// 生成只读说明标签的小工具。
		private LabelControl 制作标签(string 文本, int x, int y, int 宽 = 95)
		{
			return new LabelControl
			{
				Text = 文本,
				Location = new Point(x, y),
				AutoSizeMode = LabelAutoSizeMode.None,
				Size = new Size(宽, 16)
			};
		}
	}
}
