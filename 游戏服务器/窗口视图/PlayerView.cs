using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using 游戏服务器.模板类;
using 游戏服务器.数据类;
using DevExpress.Data;
using DevExpress.Data.Mask;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace 游戏服务器.窗口视图
{
	public class PlayerView : RibbonForm
	{
		private class 角色数据视图
		{
			private 角色数据 角色;

			public string 角色名字 => this.角色.角色名字.V;

			public bool 是否管理
			{
				get
				{
					return this.角色.管理员角色.V;
				}
				set
				{
					this.角色.管理员角色.V = value;
				}
			}

			public bool 商人角色
			{
				get
				{
					return this.角色.商人角色.V;
				}
				set
				{
					this.角色.商人角色.V = value;
				}
			}

			public bool 内挂挂机中
			{
				get
				{
					if (!Settings.使用新版内挂)
					{
						return (this.角色.网络连接?.绑定角色?.自动战斗).GetValueOrDefault();
					}
					return (this.角色.网络连接?.绑定角色?.自动挂机?.自动战斗).GetValueOrDefault();
				}
			}

			public string 封禁日期
			{
				get
				{
					if (!(this.角色.封禁日期.V != default(DateTime)))
					{
						return "未封禁";
					}
					return this.角色.封禁日期.V.ToString();
				}
			}

			public string 所属账号 => this.角色.所属账号.V.ToString();

			public string 账号封禁
			{
				get
				{
					if (!(this.角色.所属账号.V.封禁日期.V != default(DateTime)))
					{
						return "未封禁";
					}
					return this.角色.所属账号.V.封禁日期.V.ToString();
				}
			}

			public string 冻结日期
			{
				get
				{
					if (!(this.角色.冻结日期.V != default(DateTime)))
					{
						return "未冻结";
					}
					return this.角色.冻结日期.V.ToString();
				}
			}

			public string 删除日期
			{
				get
				{
					if (!(this.角色.删除日期.V != default(DateTime)))
					{
						return "未删除";
					}
					return this.角色.删除日期.V.ToString();
				}
			}

			public DateTime 登录日期 => this.角色.登录日期.V;

			public DateTime 离线日期 => this.角色.离线日期.V;

			public string 网络地址 => this.角色.网络地址.V;

			public string 物理地址 => this.角色.物理地址.V;

			public string 角色职业 => this.角色.角色职业.V.ToString();

			public string 角色性别 => this.角色.角色性别.ToString();

			public string 所属行会 => this.角色.所属行会.ToString();

			public string 行会职位 => this.角色.所属行会?.V?.行会成员[this.角色].ToString();

			public uint 元宝数量
			{
				get
				{
					return this.角色.元宝数量;
				}
				set
				{
					this.角色.元宝数量 = value;
				}
			}

			public long 消耗元宝 => this.角色.消耗元宝.V;

			public uint 金币数量 => this.角色.金币数量;

			public long 转出金币 => this.角色.转出金币.V;

			public uint 银币数量 => this.角色.银币数量;

			public byte 背包大小 => this.角色.背包大小.V;

			public byte 仓库大小 => this.角色.仓库大小.V;

			public uint 师门声望 => this.角色.师门声望;

			public string 仓库锁密码
			{
				get
				{
					return this.角色.动态密码.V;
				}
				set
				{
					this.角色.动态密码.V = value;
				}
			}

			public byte 本期特权 => this.角色.本期特权.V;

			public DateTime 本期日期 => this.角色.本期日期.V;

			public byte 上期特权 => this.角色.上期特权.V;

			public string 上期日期 => this.角色.上期日期.ToString();

			public byte 剩余特权 => 0;

			public byte 当前等级 => this.角色.当前等级.V;

			public long 当前经验 => this.角色.当前经验.V;

			public string 所在地图 => this.当前地图;

			public int 当前战力 => this.角色.当前战力.V;

			public string 当前地图
			{
				get
				{
					if (!游戏地图.数据表.TryGetValue((byte)this.角色.当前地图.V, out var value))
					{
						return this.角色.当前地图.ToString();
					}
					return value.地图名字;
				}
			}

			public string 当前坐标 => $"{this.角色.当前坐标.V.X}, {this.角色.当前坐标.V.Y}";

			public int 角色编号 => this.角色.角色编号;

			public int 当前PK值 => this.角色.当前PK值.V;

			public 角色数据视图(角色数据 A角色)
			{
				this.角色 = A角色;
			}
		}

		private int 选择的角色编号;

		private IContainer components;

		private RibbonControl ribbon;

		private RibbonPage ribbonPage1;

		private RibbonPageGroup ribbonPageGroup1;

		private BarButtonItem ReLoadDataBaseButton;

		private RepositoryItemTextEdit repositoryItemTextEdit1;

		private RepositoryItemTextEdit repositoryItemTextEdit2;

		private RepositoryItemTextEdit repositoryItemTextEdit3;

		private GridColumn 管理员角色;

		private XtraTabControl xtraTabControl1;

		private XtraTabPage 仓库页;

		private GridControl grid仓库;

		private GridView gridView2;

		private GridColumn 仓库位置;

		private XtraTabPage 技能页;

		private GridControl grid技能;

		private GridView gridView3;

		private GridColumn 技能名字;

		private XtraTabPage 装备页;

		private GridControl grid装备;

		private GridView gridView4;

		private GridColumn 穿戴部位;

		private XtraTabPage 背包页;

		private GridControl grid背包;

		private GridView gridView5;

		private GridColumn 背包位置;

		private SplitterControl splitterControl1;

		private GridColumn 技能编号;

		private GridColumn 技能等级;

		private GridColumn 技能经验;

		private GridColumn 穿戴装备;

		private GridColumn 仓库物品;

		private GridColumn 背包物品;

		private GroupControl groupControl1;

		private GridControl PlayerViewGrid;

		private GridView gridView1;

		private GridColumn 角色名字;

		private GridColumn 所属账号;

		private GridColumn 是否管理;

		private RepositoryItemCheckEdit repositoryItemCheckEdit1;

		private GridColumn 商人角色;

		private RepositoryItemCheckEdit repositoryItemCheckEdit2;

		private GridColumn 封禁日期;

		private GridColumn 冻结日期;

		private GridColumn 删除日期;

		private GridColumn 登录日期;

		private GridColumn 元宝数量;

		private GridColumn 金币数量;

		private GridColumn 角色职业;

		private GridColumn 角色性别;

		private GridColumn 所属行会;

		private GridColumn 当前等级;

		private GridColumn 当前经验;

		private GridColumn 背包大小;

		private GridColumn 角色编号;

		private GridColumn 消耗元宝;

		private GridColumn gridColumn2;

		private GridColumn 网络地址;

		private GridColumn 物理地址;

		private GridColumn 转出金币;

		private GridColumn 银币数量;

		private GridColumn 本期特权;

		private GridColumn 行会职位;

		private GridColumn 仓库锁密码;

		private GridColumn 离线日期;

		private GridColumn 当前PK值;

		private GridColumn 内挂挂机中;

		private GridColumn 掉落怪物_背包;

		private GridColumn 掉落怪物_仓库;

		private GridColumn 掉落怪物_身上;

		private GridColumn 掉落地图_仓库;

		private GridColumn 物品归属_仓库;

		private GridColumn 掉落时间_仓库;

		private GridColumn 掉落地图_背包;

		private GridColumn 物品归属_背包;

		private GridColumn 掉落时间_背包;

		private GridColumn 物品归属_身上;

		private GridColumn 掉落地图_身上;

		private GridColumn 掉落时间_身上;

		private GridColumn 数量_背包;

		private GridColumn 持久_装备;
        private GridColumn 所在地图;
        private GridColumn 当前坐标;
        private GridColumn 数量_仓库;

		public PlayerView()
		{
			this.InitializeComponent();
			this.Init();
		}

		public static void 界面更新处理(角色数据 角色)
		{
			SMain.技能数据表.Rows.Clear();
			SMain.装备数据表.Rows.Clear();
			SMain.背包数据表.Rows.Clear();
			SMain.仓库数据表.Rows.Clear();
			if (角色 == null)
			{
				return;
			}
			foreach (KeyValuePair<ushort, 技能数据> item in 角色.技能数据)
			{
				DataRow dataRow;
				dataRow = SMain.技能数据表.NewRow();
				dataRow["技能名字"] = item.Value.铭文模板.技能名字;
				dataRow["技能编号"] = item.Value.技能编号;
				dataRow["当前等级"] = item.Value.技能等级;
				dataRow["当前经验"] = item.Value.技能经验;
				SMain.技能数据表.Rows.Add(dataRow);
			}
			foreach (KeyValuePair<byte, 装备数据> item2 in 角色.角色装备)
			{
				DataRow dataRow2;
				dataRow2 = SMain.装备数据表.NewRow();
				dataRow2["穿戴部位"] = (装备穿戴部位)item2.Key;
				dataRow2["穿戴装备"] = item2.Value;
				dataRow2["持久"] = item2.Value.当前持久.V;
				dataRow2["物品归属"] = item2.Value.生成来源.V;
				dataRow2["掉落怪物"] = item2.Value.掉落怪物.V;
				dataRow2["掉落地图"] = item2.Value.掉落地图.V;
				dataRow2["掉落时间"] = item2.Value.生成时间.V;
				SMain.装备数据表.Rows.Add(dataRow2);
			}
			foreach (KeyValuePair<byte, 物品数据> item3 in 角色.角色背包)
			{
				DataRow dataRow3;
				dataRow3 = SMain.背包数据表.NewRow();
				dataRow3["背包位置"] = item3.Key;
				dataRow3["背包物品"] = item3.Value;
				dataRow3["数量"] = ((item3.Value.持久类型 != 物品持久分类.堆叠 && (item3.Value.持久类型 != 物品持久分类.容器 || item3.Value.物品类型 != 物品使用分类.经验容器)) ? 1 : item3.Value.当前持久.V);
				dataRow3["物品归属"] = item3.Value.生成来源.V;
				dataRow3["掉落怪物"] = item3.Value.掉落怪物.V;
				dataRow3["掉落地图"] = item3.Value.掉落地图.V;
				dataRow3["掉落时间"] = item3.Value.生成时间.V;
				SMain.背包数据表.Rows.Add(dataRow3);
			}
			foreach (KeyValuePair<byte, 物品数据> item4 in 角色.角色仓库)
			{
				DataRow dataRow4;
				dataRow4 = SMain.仓库数据表.NewRow();
				dataRow4["仓库位置"] = item4.Key;
				dataRow4["仓库物品"] = item4.Value;
				dataRow4["数量"] = ((item4.Value.持久类型 != 物品持久分类.堆叠 && (item4.Value.持久类型 != 物品持久分类.容器 || item4.Value.物品类型 != 物品使用分类.经验容器)) ? 1 : item4.Value.当前持久.V);
				dataRow4["物品归属"] = item4.Value.生成来源.V;
				dataRow4["掉落怪物"] = item4.Value.掉落怪物.V;
				dataRow4["掉落地图"] = item4.Value.掉落地图.V;
				dataRow4["掉落时间"] = item4.Value.生成时间.V;
				SMain.仓库数据表.Rows.Add(dataRow4);
			}
		}

		private void Init()
		{
			List<角色数据视图> list;
			list = new List<角色数据视图>();
			foreach (KeyValuePair<int, 游戏数据> item in 游戏数据网关.角色数据表.数据表)
			{
				if (item.Value is 角色数据 角色数据 && 机器人.默认账号名 != 角色数据.所属账号.V.ToString())
				{
					list.Add(new 角色数据视图(角色数据));
				}
			}
			this.PlayerViewGrid.DataSource = list;
			this.grid技能.DataSource = SMain.技能数据表;
			this.grid装备.DataSource = SMain.装备数据表;
			this.grid背包.DataSource = SMain.背包数据表;
			this.grid仓库.DataSource = SMain.仓库数据表;
			this.ColumnWidth(this.gridView5);
			this.ColumnWidth(this.gridView2);
			this.ColumnWidth(this.gridView4);
			this.ColumnWidth(this.gridView1);
			this.初始化右键菜单();
		}

		private void 初始化右键菜单()
		{
			// 技能右键删除
			var 技能菜单 = new ContextMenuStrip();
			var 技能删除项 = new ToolStripMenuItem("删除技能");
			技能删除项.Click += (s, e) => this.删除选中技能();
			技能菜单.Items.Add(技能删除项);
			技能菜单.Opening += (s, e) => { e.Cancel = this.gridView3.FocusedRowHandle < 0; };
			this.grid技能.ContextMenuStrip = 技能菜单;
			this.gridView3.MouseDown += (s, e) =>
			{
				if (e.Button == MouseButtons.Right)
				{
					var info = this.gridView3.CalcHitInfo(e.Location);
					if (info.InRow) this.gridView3.FocusedRowHandle = info.RowHandle;
				}
			};

			// 背包右键删除
			var 背包菜单 = new ContextMenuStrip();
			var 背包删除项 = new ToolStripMenuItem("删除物品");
			背包删除项.Click += (s, e) => this.删除选中背包物品();
			背包菜单.Items.Add(背包删除项);
			背包菜单.Opening += (s, e) => { e.Cancel = this.gridView5.FocusedRowHandle < 0; };
			this.grid背包.ContextMenuStrip = 背包菜单;
			this.gridView5.MouseDown += (s, e) =>
			{
				if (e.Button == MouseButtons.Right)
				{
					var info = this.gridView5.CalcHitInfo(e.Location);
					if (info.InRow) this.gridView5.FocusedRowHandle = info.RowHandle;
				}
			};
		}

		private void 删除选中技能()
		{
			if (!游戏数据网关.角色数据表.数据表.TryGetValue(this.选择的角色编号, out var value) || !(value is 角色数据 角色))
				return;
			int row = this.gridView3.FocusedRowHandle;
			if (row < 0) return;
			ushort 编号 = Convert.ToUInt16(this.gridView3.GetRowCellValue(row, "技能编号"));
			string 名字 = this.gridView3.GetRowCellValue(row, "技能名字")?.ToString() ?? 编号.ToString();
			if (MessageBox.Show($"确认删除技能【{名字}】？", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
			{
				角色.技能数据.Remove(编号);
				PlayerView.界面更新处理(角色);
			}
		}

		private void 删除选中背包物品()
		{
			if (!游戏数据网关.角色数据表.数据表.TryGetValue(this.选择的角色编号, out var value) || !(value is 角色数据 角色))
				return;
			int row = this.gridView5.FocusedRowHandle;
			if (row < 0) return;
			byte 位置 = Convert.ToByte(this.gridView5.GetRowCellValue(row, "背包位置"));
			var 物品 = this.gridView5.GetRowCellValue(row, "背包物品") as 物品数据;
			string 名字 = 物品?.ToString() ?? 位置.ToString();
			if (MessageBox.Show($"确认删除物品【{名字}】（背包格{位置}）？", "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
			{
				角色.角色背包.Remove(位置);
				PlayerView.界面更新处理(角色);
			}
		}

		private void ColumnWidth(GridView gridView)
		{
			gridView.OptionsView.ColumnAutoWidth = false;
			gridView.OptionsView.BestFitMode = GridBestFitMode.Fast;
			gridView.BestFitColumns();
		}

		private void ReLoadDataBaseButton_ItemClick(object sender, ItemClickEventArgs e)
		{
			this.Init();
		}

		private void gridView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			int[] selectedRows;
			selectedRows = this.gridView1.GetSelectedRows();
			if (selectedRows == null || selectedRows.Length == 0)
			{
				return;
			}
			int num;
			num = 0;
			while (true)
			{
				if (num < this.gridView1.Columns.Count)
				{
					if (this.gridView1.Columns[num].Name == "角色编号")
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			object rowCellValue;
			rowCellValue = this.gridView1.GetRowCellValue(selectedRows[0], this.gridView1.Columns[num]);
			int key;
			key = ((rowCellValue != null) ? ((int)rowCellValue) : 0);
			if (游戏数据网关.角色数据表.数据表.TryGetValue(key, out var value) && value is 角色数据 角色)
			{
				this.选择的角色编号 = key;
				PlayerView.界面更新处理(角色);
			}
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(PlayerView));
            ribbon = new RibbonControl();
            ReLoadDataBaseButton = new BarButtonItem();
            ribbonPage1 = new RibbonPage();
            ribbonPageGroup1 = new RibbonPageGroup();
            repositoryItemTextEdit1 = new RepositoryItemTextEdit();
            repositoryItemTextEdit2 = new RepositoryItemTextEdit();
            repositoryItemTextEdit3 = new RepositoryItemTextEdit();
            xtraTabControl1 = new XtraTabControl();
            背包页 = new XtraTabPage();
            grid背包 = new GridControl();
            gridView5 = new GridView();
            背包位置 = new GridColumn();
            背包物品 = new GridColumn();
            数量_背包 = new GridColumn();
            物品归属_背包 = new GridColumn();
            掉落怪物_背包 = new GridColumn();
            掉落地图_背包 = new GridColumn();
            掉落时间_背包 = new GridColumn();
            技能页 = new XtraTabPage();
            grid技能 = new GridControl();
            gridView3 = new GridView();
            技能名字 = new GridColumn();
            技能编号 = new GridColumn();
            技能等级 = new GridColumn();
            技能经验 = new GridColumn();
            装备页 = new XtraTabPage();
            grid装备 = new GridControl();
            gridView4 = new GridView();
            穿戴部位 = new GridColumn();
            穿戴装备 = new GridColumn();
            持久_装备 = new GridColumn();
            物品归属_身上 = new GridColumn();
            掉落怪物_身上 = new GridColumn();
            掉落地图_身上 = new GridColumn();
            掉落时间_身上 = new GridColumn();
            仓库页 = new XtraTabPage();
            grid仓库 = new GridControl();
            gridView2 = new GridView();
            仓库位置 = new GridColumn();
            仓库物品 = new GridColumn();
            数量_仓库 = new GridColumn();
            物品归属_仓库 = new GridColumn();
            掉落怪物_仓库 = new GridColumn();
            掉落地图_仓库 = new GridColumn();
            掉落时间_仓库 = new GridColumn();
            splitterControl1 = new SplitterControl();
            groupControl1 = new GroupControl();
            PlayerViewGrid = new GridControl();
            gridView1 = new GridView();
            角色编号 = new GridColumn();
            角色名字 = new GridColumn();
            角色性别 = new GridColumn();
            角色职业 = new GridColumn();
            当前等级 = new GridColumn();
            所属账号 = new GridColumn();
            是否管理 = new GridColumn();
            repositoryItemCheckEdit1 = new RepositoryItemCheckEdit();
            商人角色 = new GridColumn();
            repositoryItemCheckEdit2 = new RepositoryItemCheckEdit();
            内挂挂机中 = new GridColumn();
            登录日期 = new GridColumn();
            离线日期 = new GridColumn();
            元宝数量 = new GridColumn();
            消耗元宝 = new GridColumn();
            金币数量 = new GridColumn();
            转出金币 = new GridColumn();
            银币数量 = new GridColumn();
            当前经验 = new GridColumn();
            所属行会 = new GridColumn();
            行会职位 = new GridColumn();
            仓库锁密码 = new GridColumn();
            所在地图 = new GridColumn();
            当前坐标 = new GridColumn();
            本期特权 = new GridColumn();
            背包大小 = new GridColumn();
            封禁日期 = new GridColumn();
            冻结日期 = new GridColumn();
            删除日期 = new GridColumn();
            物理地址 = new GridColumn();
            网络地址 = new GridColumn();
            当前PK值 = new GridColumn();
            ((ISupportInitialize)ribbon).BeginInit();
            ((ISupportInitialize)repositoryItemTextEdit1).BeginInit();
            ((ISupportInitialize)repositoryItemTextEdit2).BeginInit();
            ((ISupportInitialize)repositoryItemTextEdit3).BeginInit();
            ((ISupportInitialize)xtraTabControl1).BeginInit();
            xtraTabControl1.SuspendLayout();
            背包页.SuspendLayout();
            ((ISupportInitialize)grid背包).BeginInit();
            ((ISupportInitialize)gridView5).BeginInit();
            技能页.SuspendLayout();
            ((ISupportInitialize)grid技能).BeginInit();
            ((ISupportInitialize)gridView3).BeginInit();
            装备页.SuspendLayout();
            ((ISupportInitialize)grid装备).BeginInit();
            ((ISupportInitialize)gridView4).BeginInit();
            仓库页.SuspendLayout();
            ((ISupportInitialize)grid仓库).BeginInit();
            ((ISupportInitialize)gridView2).BeginInit();
            ((ISupportInitialize)groupControl1).BeginInit();
            groupControl1.SuspendLayout();
            ((ISupportInitialize)PlayerViewGrid).BeginInit();
            ((ISupportInitialize)gridView1).BeginInit();
            ((ISupportInitialize)repositoryItemCheckEdit1).BeginInit();
            ((ISupportInitialize)repositoryItemCheckEdit2).BeginInit();
            SuspendLayout();
            // 
            // ribbon
            // 
            ribbon.EmptyAreaImageOptions.ImagePadding = new Padding(35, 32, 35, 32);
            ribbon.ExpandCollapseItem.Id = 0;
            ribbon.Items.AddRange(new BarItem[] { ribbon.ExpandCollapseItem, ReLoadDataBaseButton });
            ribbon.Location = new Point(0, 0);
            ribbon.Margin = new Padding(4, 3, 4, 3);
            ribbon.MaxItemId = 12;
            ribbon.MdiMergeStyle = RibbonMdiMergeStyle.Always;
            ribbon.Name = "ribbon";
            ribbon.OptionsMenuMinWidth = 385;
            ribbon.Pages.AddRange(new RibbonPage[] { ribbonPage1 });
            ribbon.RepositoryItems.AddRange(new RepositoryItem[] { repositoryItemTextEdit1, repositoryItemTextEdit2, repositoryItemTextEdit3 });
            ribbon.Size = new Size(999, 160);
            // 
            // ReLoadDataBaseButton
            // 
            ReLoadDataBaseButton.Caption = "重载 数据";
            ReLoadDataBaseButton.Id = 4;
            ReLoadDataBaseButton.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("ReLoadDataBaseButton.ImageOptions.SvgImage");
            ReLoadDataBaseButton.LargeWidth = 50;
            ReLoadDataBaseButton.Name = "ReLoadDataBaseButton";
            ReLoadDataBaseButton.ItemClick += ReLoadDataBaseButton_ItemClick;
            // 
            // ribbonPage1
            // 
            ribbonPage1.Groups.AddRange(new RibbonPageGroup[] { ribbonPageGroup1 });
            ribbonPage1.Name = "ribbonPage1";
            ribbonPage1.Text = "主页";
            // 
            // ribbonPageGroup1
            // 
            ribbonPageGroup1.AllowTextClipping = false;
            ribbonPageGroup1.CaptionButtonVisible = DefaultBoolean.False;
            ribbonPageGroup1.ItemLinks.Add(ReLoadDataBaseButton);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "动作";
            // 
            // repositoryItemTextEdit1
            // 
            repositoryItemTextEdit1.AutoHeight = false;
            repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
            // 
            // repositoryItemTextEdit2
            // 
            repositoryItemTextEdit2.AutoHeight = false;
            repositoryItemTextEdit2.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            repositoryItemTextEdit2.MaskSettings.Set("MaskManagerSignature", "allowNull=False");
            repositoryItemTextEdit2.MaskSettings.Set("mask", "n0");
            repositoryItemTextEdit2.Name = "repositoryItemTextEdit2";
            // 
            // repositoryItemTextEdit3
            // 
            repositoryItemTextEdit3.AutoHeight = false;
            repositoryItemTextEdit3.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            repositoryItemTextEdit3.MaskSettings.Set("MaskManagerSignature", "allowNull=False");
            repositoryItemTextEdit3.MaskSettings.Set("mask", "n0");
            repositoryItemTextEdit3.Name = "repositoryItemTextEdit3";
            // 
            // xtraTabControl1
            // 
            xtraTabControl1.Dock = DockStyle.Right;
            xtraTabControl1.Location = new Point(699, 160);
            xtraTabControl1.Name = "xtraTabControl1";
            xtraTabControl1.SelectedTabPage = 背包页;
            xtraTabControl1.Size = new Size(300, 502);
            xtraTabControl1.TabIndex = 32;
            xtraTabControl1.TabPages.AddRange(new XtraTabPage[] { 技能页, 装备页, 背包页, 仓库页 });
            // 
            // 背包页
            // 
            背包页.Controls.Add(grid背包);
            背包页.Name = "背包页";
            背包页.Size = new Size(298, 476);
            背包页.Text = "背包";
            // 
            // grid背包
            // 
            grid背包.AccessibleDescription = "";
            grid背包.Dock = DockStyle.Fill;
            grid背包.Location = new Point(0, 0);
            grid背包.MainView = gridView5;
            grid背包.MenuManager = ribbon;
            grid背包.Name = "grid背包";
            grid背包.Size = new Size(298, 476);
            grid背包.TabIndex = 27;
            grid背包.ViewCollection.AddRange(new BaseView[] { gridView5 });
            // 
            // gridView5
            // 
            gridView5.Columns.AddRange(new GridColumn[] { 背包位置, 背包物品, 数量_背包, 物品归属_背包, 掉落怪物_背包, 掉落地图_背包, 掉落时间_背包 });
            gridView5.GridControl = grid背包;
            gridView5.Name = "gridView5";
            gridView5.OptionsView.ShowGroupPanel = false;
            // 
            // 背包位置
            // 
            背包位置.Caption = "背包位置";
            背包位置.FieldName = "背包位置";
            背包位置.Name = "背包位置";
            背包位置.Visible = true;
            背包位置.VisibleIndex = 0;
            // 
            // 背包物品
            // 
            背包物品.Caption = "背包物品";
            背包物品.FieldName = "背包物品";
            背包物品.Name = "背包物品";
            背包物品.Visible = true;
            背包物品.VisibleIndex = 1;
            // 
            // 数量_背包
            // 
            数量_背包.Caption = "数量";
            数量_背包.FieldName = "数量";
            数量_背包.Name = "数量_背包";
            数量_背包.Visible = true;
            数量_背包.VisibleIndex = 2;
            // 
            // 物品归属_背包
            // 
            物品归属_背包.Caption = "物品归属";
            物品归属_背包.FieldName = "物品归属";
            物品归属_背包.Name = "物品归属_背包";
            物品归属_背包.Visible = true;
            物品归属_背包.VisibleIndex = 3;
            // 
            // 掉落怪物_背包
            // 
            掉落怪物_背包.Caption = "掉落怪物";
            掉落怪物_背包.FieldName = "掉落怪物";
            掉落怪物_背包.Name = "掉落怪物_背包";
            掉落怪物_背包.Visible = true;
            掉落怪物_背包.VisibleIndex = 4;
            // 
            // 掉落地图_背包
            // 
            掉落地图_背包.Caption = "掉落地图";
            掉落地图_背包.FieldName = "掉落地图";
            掉落地图_背包.Name = "掉落地图_背包";
            掉落地图_背包.Visible = true;
            掉落地图_背包.VisibleIndex = 5;
            // 
            // 掉落时间_背包
            // 
            掉落时间_背包.Caption = "掉落时间";
            掉落时间_背包.FieldName = "掉落时间";
            掉落时间_背包.Name = "掉落时间_背包";
            掉落时间_背包.Visible = true;
            掉落时间_背包.VisibleIndex = 6;
            // 
            // 技能页
            // 
            技能页.Controls.Add(grid技能);
            技能页.Name = "技能页";
            技能页.Size = new Size(298, 488);
            技能页.Text = "技能";
            // 
            // grid技能
            // 
            grid技能.Dock = DockStyle.Fill;
            grid技能.Location = new Point(0, 0);
            grid技能.MainView = gridView3;
            grid技能.MenuManager = ribbon;
            grid技能.Name = "grid技能";
            grid技能.Size = new Size(298, 488);
            grid技能.TabIndex = 25;
            grid技能.ViewCollection.AddRange(new BaseView[] { gridView3 });
            // 
            // gridView3
            // 
            gridView3.Columns.AddRange(new GridColumn[] { 技能名字, 技能编号, 技能等级, 技能经验 });
            gridView3.GridControl = grid技能;
            gridView3.Name = "gridView3";
            gridView3.OptionsView.ShowGroupPanel = false;
            // 
            // 技能名字
            // 
            技能名字.Caption = "技能名字";
            技能名字.FieldName = "技能名字";
            技能名字.Name = "技能名字";
            技能名字.Visible = true;
            技能名字.VisibleIndex = 0;
            // 
            // 技能编号
            // 
            技能编号.Caption = "技能编号";
            技能编号.FieldName = "技能编号";
            技能编号.Name = "技能编号";
            技能编号.Visible = true;
            技能编号.VisibleIndex = 1;
            // 
            // 技能等级
            // 
            技能等级.Caption = "技能等级";
            技能等级.FieldName = "当前等级";
            技能等级.Name = "技能等级";
            技能等级.Visible = true;
            技能等级.VisibleIndex = 2;
            // 
            // 技能经验
            // 
            技能经验.Caption = "当前经验";
            技能经验.FieldName = "当前经验";
            技能经验.Name = "技能经验";
            技能经验.Visible = true;
            技能经验.VisibleIndex = 3;
            // 
            // 装备页
            // 
            装备页.Controls.Add(grid装备);
            装备页.Name = "装备页";
            装备页.Size = new Size(298, 488);
            装备页.Text = "装备";
            // 
            // grid装备
            // 
            grid装备.Dock = DockStyle.Fill;
            grid装备.Location = new Point(0, 0);
            grid装备.MainView = gridView4;
            grid装备.MenuManager = ribbon;
            grid装备.Name = "grid装备";
            grid装备.Size = new Size(298, 488);
            grid装备.TabIndex = 26;
            grid装备.ViewCollection.AddRange(new BaseView[] { gridView4 });
            // 
            // gridView4
            // 
            gridView4.Columns.AddRange(new GridColumn[] { 穿戴部位, 穿戴装备, 持久_装备, 物品归属_身上, 掉落怪物_身上, 掉落地图_身上, 掉落时间_身上 });
            gridView4.GridControl = grid装备;
            gridView4.Name = "gridView4";
            gridView4.OptionsView.ShowGroupPanel = false;
            // 
            // 穿戴部位
            // 
            穿戴部位.Caption = "穿戴部位";
            穿戴部位.FieldName = "穿戴部位";
            穿戴部位.Name = "穿戴部位";
            穿戴部位.Visible = true;
            穿戴部位.VisibleIndex = 0;
            // 
            // 穿戴装备
            // 
            穿戴装备.Caption = "穿戴装备";
            穿戴装备.FieldName = "穿戴装备";
            穿戴装备.Name = "穿戴装备";
            穿戴装备.Visible = true;
            穿戴装备.VisibleIndex = 1;
            // 
            // 持久_装备
            // 
            持久_装备.Caption = "持久";
            持久_装备.FieldName = "持久";
            持久_装备.Name = "持久_装备";
            持久_装备.Visible = true;
            持久_装备.VisibleIndex = 2;
            // 
            // 物品归属_身上
            // 
            物品归属_身上.Caption = "物品归属";
            物品归属_身上.FieldName = "物品归属";
            物品归属_身上.Name = "物品归属_身上";
            物品归属_身上.Visible = true;
            物品归属_身上.VisibleIndex = 3;
            // 
            // 掉落怪物_身上
            // 
            掉落怪物_身上.Caption = "掉落怪物";
            掉落怪物_身上.FieldName = "掉落怪物";
            掉落怪物_身上.Name = "掉落怪物_身上";
            掉落怪物_身上.Visible = true;
            掉落怪物_身上.VisibleIndex = 4;
            // 
            // 掉落地图_身上
            // 
            掉落地图_身上.Caption = "掉落地图";
            掉落地图_身上.FieldName = "掉落地图";
            掉落地图_身上.Name = "掉落地图_身上";
            掉落地图_身上.Visible = true;
            掉落地图_身上.VisibleIndex = 5;
            // 
            // 掉落时间_身上
            // 
            掉落时间_身上.Caption = "掉落时间";
            掉落时间_身上.FieldName = "掉落时间";
            掉落时间_身上.Name = "掉落时间_身上";
            掉落时间_身上.Visible = true;
            掉落时间_身上.VisibleIndex = 6;
            // 
            // 仓库页
            // 
            仓库页.Controls.Add(grid仓库);
            仓库页.Name = "仓库页";
            仓库页.Size = new Size(298, 488);
            仓库页.Text = "仓库";
            // 
            // grid仓库
            // 
            grid仓库.Dock = DockStyle.Fill;
            grid仓库.Location = new Point(0, 0);
            grid仓库.MainView = gridView2;
            grid仓库.MenuManager = ribbon;
            grid仓库.Name = "grid仓库";
            grid仓库.Size = new Size(298, 488);
            grid仓库.TabIndex = 25;
            grid仓库.ViewCollection.AddRange(new BaseView[] { gridView2 });
            // 
            // gridView2
            // 
            gridView2.Columns.AddRange(new GridColumn[] { 仓库位置, 仓库物品, 数量_仓库, 物品归属_仓库, 掉落怪物_仓库, 掉落地图_仓库, 掉落时间_仓库 });
            gridView2.GridControl = grid仓库;
            gridView2.Name = "gridView2";
            gridView2.OptionsView.ShowGroupPanel = false;
            // 
            // 仓库位置
            // 
            仓库位置.Caption = "仓库位置";
            仓库位置.FieldName = "仓库位置";
            仓库位置.Name = "仓库位置";
            仓库位置.Visible = true;
            仓库位置.VisibleIndex = 0;
            // 
            // 仓库物品
            // 
            仓库物品.Caption = "仓库物品";
            仓库物品.FieldName = "仓库物品";
            仓库物品.Name = "仓库物品";
            仓库物品.Visible = true;
            仓库物品.VisibleIndex = 1;
            // 
            // 数量_仓库
            // 
            数量_仓库.Caption = "数量";
            数量_仓库.FieldName = "数量";
            数量_仓库.Name = "数量_仓库";
            数量_仓库.Visible = true;
            数量_仓库.VisibleIndex = 2;
            // 
            // 物品归属_仓库
            // 
            物品归属_仓库.Caption = "物品归属";
            物品归属_仓库.FieldName = "物品归属";
            物品归属_仓库.Name = "物品归属_仓库";
            物品归属_仓库.Visible = true;
            物品归属_仓库.VisibleIndex = 3;
            // 
            // 掉落怪物_仓库
            // 
            掉落怪物_仓库.Caption = "掉落怪物";
            掉落怪物_仓库.FieldName = "掉落怪物";
            掉落怪物_仓库.Name = "掉落怪物_仓库";
            掉落怪物_仓库.Visible = true;
            掉落怪物_仓库.VisibleIndex = 4;
            // 
            // 掉落地图_仓库
            // 
            掉落地图_仓库.Caption = "掉落地图";
            掉落地图_仓库.FieldName = "掉落地图";
            掉落地图_仓库.Name = "掉落地图_仓库";
            掉落地图_仓库.Visible = true;
            掉落地图_仓库.VisibleIndex = 5;
            // 
            // 掉落时间_仓库
            // 
            掉落时间_仓库.Caption = "掉落时间";
            掉落时间_仓库.FieldName = "掉落时间";
            掉落时间_仓库.Name = "掉落时间_仓库";
            掉落时间_仓库.Visible = true;
            掉落时间_仓库.VisibleIndex = 6;
            // 
            // splitterControl1
            // 
            splitterControl1.Dock = DockStyle.Right;
            splitterControl1.Location = new Point(689, 160);
            splitterControl1.Name = "splitterControl1";
            splitterControl1.Size = new Size(10, 502);
            splitterControl1.TabIndex = 33;
            splitterControl1.TabStop = false;
            // 
            // groupControl1
            // 
            groupControl1.Controls.Add(PlayerViewGrid);
            groupControl1.Dock = DockStyle.Fill;
            groupControl1.Location = new Point(0, 160);
            groupControl1.Name = "groupControl1";
            groupControl1.ShowCaption = false;
            groupControl1.Size = new Size(689, 502);
            groupControl1.TabIndex = 36;
            groupControl1.Text = "角色列表";
            // 
            // PlayerViewGrid
            // 
            PlayerViewGrid.Dock = DockStyle.Fill;
            PlayerViewGrid.Location = new Point(2, 2);
            PlayerViewGrid.MainView = gridView1;
            PlayerViewGrid.MenuManager = ribbon;
            PlayerViewGrid.Name = "PlayerViewGrid";
            PlayerViewGrid.RepositoryItems.AddRange(new RepositoryItem[] { repositoryItemCheckEdit1, repositoryItemCheckEdit2 });
            PlayerViewGrid.Size = new Size(685, 498);
            PlayerViewGrid.TabIndex = 31;
            PlayerViewGrid.ViewCollection.AddRange(new BaseView[] { gridView1 });
            // 
            // gridView1
            // 
            gridView1.Columns.AddRange(new GridColumn[] { 角色编号, 角色名字, 角色性别, 角色职业, 当前等级, 所属账号, 是否管理, 商人角色, 内挂挂机中, 登录日期, 离线日期, 元宝数量, 消耗元宝, 金币数量, 转出金币, 银币数量, 当前经验, 所属行会, 行会职位, 仓库锁密码, 所在地图, 当前坐标, 本期特权, 背包大小, 封禁日期, 冻结日期, 删除日期, 物理地址, 网络地址, 当前PK值 });
            gridView1.GridControl = PlayerViewGrid;
            gridView1.Name = "gridView1";
            gridView1.OptionsView.BestFitMode = GridBestFitMode.Full;
            gridView1.OptionsView.ColumnAutoWidth = false;
            gridView1.OptionsView.ShowGroupPanel = false;
            gridView1.SelectionChanged += gridView1_SelectionChanged;
            // 
            // 角色编号
            // 
            角色编号.Caption = "角色编号";
            角色编号.FieldName = "角色编号";
            角色编号.Name = "角色编号";
            角色编号.Visible = true;
            角色编号.VisibleIndex = 0;
            角色编号.Width = 60;
            // 
            // 角色名字
            // 
            角色名字.Caption = "角色名字";
            角色名字.FieldName = "角色名字";
            角色名字.MinWidth = 75;
            角色名字.Name = "角色名字";
            角色名字.Visible = true;
            角色名字.VisibleIndex = 1;
            // 
            // 角色性别
            // 
            角色性别.Caption = "性别";
            角色性别.FieldName = "角色性别";
            角色性别.Name = "角色性别";
            角色性别.Visible = true;
            角色性别.VisibleIndex = 2;
            角色性别.Width = 20;
            // 
            // 角色职业
            // 
            角色职业.Caption = "职业";
            角色职业.FieldName = "角色职业";
            角色职业.Name = "角色职业";
            角色职业.Visible = true;
            角色职业.VisibleIndex = 3;
            角色职业.Width = 20;
            // 
            // 当前等级
            // 
            当前等级.Caption = "等级";
            当前等级.FieldName = "当前等级";
            当前等级.Name = "当前等级";
            当前等级.Visible = true;
            当前等级.VisibleIndex = 4;
            当前等级.Width = 20;
            // 
            // 所属账号
            // 
            所属账号.Caption = "所属账号";
            所属账号.FieldName = "所属账号";
            所属账号.GroupInterval = ColumnGroupInterval.DateRange;
            所属账号.MinWidth = 75;
            所属账号.Name = "所属账号";
            所属账号.Visible = true;
            所属账号.VisibleIndex = 5;
            // 
            // 是否管理
            // 
            是否管理.Caption = "是否管理";
            是否管理.ColumnEdit = repositoryItemCheckEdit1;
            是否管理.FieldName = "是否管理";
            是否管理.Name = "是否管理";
            是否管理.OptionsColumn.ShowInExpressionEditor = false;
            是否管理.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
            是否管理.UnboundDataType = typeof(bool);
            是否管理.Visible = true;
            是否管理.VisibleIndex = 6;
            是否管理.Width = 20;
            // 
            // repositoryItemCheckEdit1
            // 
            repositoryItemCheckEdit1.AutoHeight = false;
            repositoryItemCheckEdit1.Caption = "";
            repositoryItemCheckEdit1.CheckBoxOptions.Style = CheckBoxStyle.CheckBox;
            repositoryItemCheckEdit1.ContentAlignment = HorzAlignment.Near;
            repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            repositoryItemCheckEdit1.NullStyle = StyleIndeterminate.Unchecked;
            // 
            // 商人角色
            // 
            商人角色.Caption = "商人角色";
            商人角色.ColumnEdit = repositoryItemCheckEdit2;
            商人角色.FieldName = "商人角色";
            商人角色.Name = "商人角色";
            商人角色.Visible = true;
            商人角色.VisibleIndex = 7;
            商人角色.Width = 20;
            // 
            // repositoryItemCheckEdit2
            // 
            repositoryItemCheckEdit2.AutoHeight = false;
            repositoryItemCheckEdit2.CheckBoxOptions.Style = CheckBoxStyle.CheckBox;
            repositoryItemCheckEdit2.ContentAlignment = HorzAlignment.Near;
            repositoryItemCheckEdit2.Name = "repositoryItemCheckEdit2";
            repositoryItemCheckEdit2.NullStyle = StyleIndeterminate.Unchecked;
            // 
            // 内挂挂机中
            // 
            内挂挂机中.Caption = "内挂挂机中";
            内挂挂机中.FieldName = "内挂挂机中";
            内挂挂机中.Name = "内挂挂机中";
            内挂挂机中.Visible = true;
            内挂挂机中.VisibleIndex = 8;
            // 
            // 登录日期
            // 
            登录日期.Caption = "登录日期";
            登录日期.FieldName = "登录日期";
            登录日期.Name = "登录日期";
            登录日期.Visible = true;
            登录日期.VisibleIndex = 9;
            登录日期.Width = 20;
            // 
            // 离线日期
            // 
            离线日期.Caption = "离线日期";
            离线日期.FieldName = "离线日期";
            离线日期.Name = "离线日期";
            离线日期.Visible = true;
            离线日期.VisibleIndex = 10;
            // 
            // 元宝数量
            // 
            元宝数量.Caption = "元宝数量";
            元宝数量.FieldName = "元宝数量";
            元宝数量.Name = "元宝数量";
            元宝数量.Visible = true;
            元宝数量.VisibleIndex = 11;
            元宝数量.Width = 50;
            // 
            // 消耗元宝
            // 
            消耗元宝.Caption = "消耗元宝";
            消耗元宝.FieldName = "消耗元宝";
            消耗元宝.MinWidth = 21;
            消耗元宝.Name = "消耗元宝";
            消耗元宝.Visible = true;
            消耗元宝.VisibleIndex = 12;
            消耗元宝.Width = 78;
            // 
            // 金币数量
            // 
            金币数量.Caption = "金币数量";
            金币数量.FieldName = "金币数量";
            金币数量.Name = "金币数量";
            金币数量.Visible = true;
            金币数量.VisibleIndex = 13;
            金币数量.Width = 20;
            // 
            // 转出金币
            // 
            转出金币.Caption = "转出金币";
            转出金币.FieldName = "转出金币";
            转出金币.MinWidth = 21;
            转出金币.Name = "转出金币";
            转出金币.Visible = true;
            转出金币.VisibleIndex = 14;
            转出金币.Width = 78;
            // 
            // 银币数量
            // 
            银币数量.Caption = "银币数量";
            银币数量.FieldName = "银币数量";
            银币数量.MinWidth = 21;
            银币数量.Name = "银币数量";
            银币数量.Visible = true;
            银币数量.VisibleIndex = 15;
            银币数量.Width = 78;
            // 
            // 当前经验
            // 
            当前经验.Caption = "当前经验";
            当前经验.FieldName = "当前经验";
            当前经验.Name = "当前经验";
            当前经验.Visible = true;
            当前经验.VisibleIndex = 16;
            当前经验.Width = 20;
            // 
            // 所属行会
            // 
            所属行会.Caption = "所属行会";
            所属行会.FieldName = "所属行会";
            所属行会.Name = "所属行会";
            所属行会.Visible = true;
            所属行会.VisibleIndex = 17;
            所属行会.Width = 20;
            // 
            // 行会职位
            // 
            行会职位.Caption = "行会职位";
            行会职位.FieldName = "行会职位";
            行会职位.Name = "行会职位";
            行会职位.Visible = true;
            行会职位.VisibleIndex = 18;
            行会职位.Width = 52;
            // 
            // 仓库锁密码
            // 
            仓库锁密码.Caption = "仓库锁密码";
            仓库锁密码.FieldName = "仓库锁密码";
            仓库锁密码.Name = "仓库锁密码";
            仓库锁密码.Visible = true;
            仓库锁密码.VisibleIndex = 19;
            // 
            // 所在地图
            // 
            所在地图.Caption = "所在地图";
            所在地图.FieldName = "所在地图";
            所在地图.Name = "所在地图";
            所在地图.Visible = true;
            所在地图.VisibleIndex = 20;
            所在地图.Width = 20;
            // 
            // 当前坐标
            // 
            当前坐标.Caption = "当前坐标";
            当前坐标.FieldName = "当前坐标";
            当前坐标.MinWidth = 21;
            当前坐标.Name = "当前坐标";
            当前坐标.Visible = true;
            当前坐标.VisibleIndex = 21;
            // 
            // 本期特权
            // 
            本期特权.Caption = "本期特权";
            本期特权.FieldName = "本期特权";
            本期特权.MinWidth = 21;
            本期特权.Name = "本期特权";
            本期特权.Visible = true;
            本期特权.VisibleIndex = 22;
            本期特权.Width = 78;
            // 
            // 背包大小
            // 
            背包大小.Caption = "背包大小";
            背包大小.FieldName = "背包大小";
            背包大小.Name = "背包大小";
            背包大小.Visible = true;
            背包大小.VisibleIndex = 23;
            背包大小.Width = 20;
            // 
            // 封禁日期
            // 
            封禁日期.Caption = "封禁日期";
            封禁日期.FieldName = "封禁日期";
            封禁日期.Name = "封禁日期";
            封禁日期.Visible = true;
            封禁日期.VisibleIndex = 24;
            封禁日期.Width = 20;
            // 
            // 冻结日期
            // 
            冻结日期.Caption = "冻结日期";
            冻结日期.FieldName = "冻结日期";
            冻结日期.Name = "冻结日期";
            冻结日期.Visible = true;
            冻结日期.VisibleIndex = 25;
            冻结日期.Width = 20;
            // 
            // 删除日期
            // 
            删除日期.Caption = "删除日期";
            删除日期.FieldName = "删除日期";
            删除日期.Name = "删除日期";
            删除日期.Visible = true;
            删除日期.VisibleIndex = 26;
            删除日期.Width = 20;
            // 
            // 物理地址
            // 
            物理地址.Caption = "物理地址";
            物理地址.FieldName = "物理地址";
            物理地址.Name = "物理地址";
            物理地址.Visible = true;
            物理地址.VisibleIndex = 27;
            物理地址.Width = 20;
            // 
            // 网络地址
            // 
            网络地址.Caption = "网络地址";
            网络地址.FieldName = "网络地址";
            网络地址.MinWidth = 70;
            网络地址.Name = "网络地址";
            网络地址.Visible = true;
            网络地址.VisibleIndex = 28;
            网络地址.Width = 70;
            // 
            // 当前PK值
            // 
            当前PK值.Caption = "当前PK值";
            当前PK值.FieldName = "当前PK值";
            当前PK值.Name = "当前PK值";
            当前PK值.Visible = true;
            当前PK值.VisibleIndex = 29;
            // 
            // PlayerView
            // 
            AutoScaleDimensions = new SizeF(7F, 14F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(999, 662);
            Controls.Add(groupControl1);
            Controls.Add(splitterControl1);
            Controls.Add(xtraTabControl1);
            Controls.Add(ribbon);
            Margin = new Padding(4, 3, 4, 3);
            Name = "PlayerView";
            Ribbon = ribbon;
            Text = "角色数据";
            ((ISupportInitialize)ribbon).EndInit();
            ((ISupportInitialize)repositoryItemTextEdit1).EndInit();
            ((ISupportInitialize)repositoryItemTextEdit2).EndInit();
            ((ISupportInitialize)repositoryItemTextEdit3).EndInit();
            ((ISupportInitialize)xtraTabControl1).EndInit();
            xtraTabControl1.ResumeLayout(false);
            背包页.ResumeLayout(false);
            ((ISupportInitialize)grid背包).EndInit();
            ((ISupportInitialize)gridView5).EndInit();
            技能页.ResumeLayout(false);
            ((ISupportInitialize)grid技能).EndInit();
            ((ISupportInitialize)gridView3).EndInit();
            装备页.ResumeLayout(false);
            ((ISupportInitialize)grid装备).EndInit();
            ((ISupportInitialize)gridView4).EndInit();
            仓库页.ResumeLayout(false);
            ((ISupportInitialize)grid仓库).EndInit();
            ((ISupportInitialize)gridView2).EndInit();
            ((ISupportInitialize)groupControl1).EndInit();
            groupControl1.ResumeLayout(false);
            ((ISupportInitialize)PlayerViewGrid).EndInit();
            ((ISupportInitialize)gridView1).EndInit();
            ((ISupportInitialize)repositoryItemCheckEdit1).EndInit();
            ((ISupportInitialize)repositoryItemCheckEdit2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
