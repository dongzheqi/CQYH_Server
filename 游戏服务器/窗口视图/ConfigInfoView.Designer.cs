using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data.Mask;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab;
using DevExpress.XtraVerticalGrid.Rows;

namespace 游戏服务器.窗口视图
{
    partial class ConfigInfoView
    {
        private IContainer components;

        private RibbonControl ribbon;

        private RibbonPage ribbonPage1;

        private RibbonPageGroup ribbonPageGroup1;

        private BarButtonItem SaveDataBaseButton;

        private BarButtonItem barButtonItem1;

        private BarButtonItem ReLoadDataBaseButton;

        private XtraFolderBrowserDialog FolderDialog;

        private EditorRow row;

        private EditorRow row1;

        private EditorRow row2;

        private EditorRow row3;

        private EditorRow row4;

        private EditorRow row5;

        private EditorRow row6;
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
            components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(ConfigInfoView));
            ribbon = new RibbonControl();
            SaveDataBaseButton = new BarButtonItem();
            barButtonItem1 = new BarButtonItem();
            ReLoadDataBaseButton = new BarButtonItem();
            ribbonPage1 = new RibbonPage();
            ribbonPageGroup1 = new RibbonPageGroup();
            FolderDialog = new XtraFolderBrowserDialog(components);
            row = new EditorRow();
            row1 = new EditorRow();
            row2 = new EditorRow();
            row3 = new EditorRow();
            row4 = new EditorRow();
            row5 = new EditorRow();
            row6 = new EditorRow();
            xtraTabPage3 = new XtraTabPage();
            其他分组 = new GroupControl();
            textEdit15 = new TextEdit();
            labelControl44 = new LabelControl();
            textEdit3 = new TextEdit();
            labelControl57 = new LabelControl();
            labelControl45 = new LabelControl();
            textEdit5 = new TextEdit();
            labelControl46 = new LabelControl();
            textEdit6 = new TextEdit();
            labelControl55 = new LabelControl();
            textEdit13 = new TextEdit();
            死亡掉落分组 = new GroupControl();
            labelControl41 = new LabelControl();
            labelControl38 = new LabelControl();
            textEdit_Dropweap = new TextEdit();
            labelControl39 = new LabelControl();
            textEdit_dropshoushi = new TextEdit();
            labelControl40 = new LabelControl();
            textEdit_dropbags = new TextEdit();
            textEdit4 = new TextEdit();
            labelControl42 = new LabelControl();
            textEdit_Dropweap_red = new TextEdit();
            labelControl30 = new LabelControl();
            textEdit_dropshoushi_red = new TextEdit();
            祝福油几率分组 = new GroupControl();
            labelControl37 = new LabelControl();
            labelControl31 = new LabelControl();
            textEdit_zhufuyou_Lv1 = new TextEdit();
            labelControl32 = new LabelControl();
            textEdit_zhufuyou_Lv2 = new TextEdit();
            labelControl34 = new LabelControl();
            textEdit_zhufuyou_Lv3 = new TextEdit();
            labelControl33 = new LabelControl();
            textEdit_zhufuyou_Lv4 = new TextEdit();
            labelControl36 = new LabelControl();
            textEdit_zhufuyou_Lv5 = new TextEdit();
            labelControl35 = new LabelControl();
            textEdit_zhufuyou_Lv6 = new TextEdit();
            textEdit_zhufuyou_Lv0 = new TextEdit();
            游戏地图开放分组 = new GroupControl();
            labelControl51 = new LabelControl();
            textEdit9 = new TextEdit();
            labelControl52 = new LabelControl();
            textEdit10 = new TextEdit();
            labelControl53 = new LabelControl();
            textEdit11 = new TextEdit();
            textEdit8 = new TextEdit();
            labelControl54 = new LabelControl();
            labelControl49 = new LabelControl();
            textEdit12 = new TextEdit();
            自动分解分组 = new GroupControl();
            分解装备check = new CheckEdit();
            不分解极品check = new CheckEdit();
            自动拾取分组 = new GroupControl();
            金币入包check = new CheckEdit();
            银币入包check = new CheckEdit();
            物品入包check = new CheckEdit();
            groupControl1 = new GroupControl();
            textEdit14 = new TextEdit();
            labelControl56 = new LabelControl();
            textEdit17 = new TextEdit();
            labelControl59 = new LabelControl();
            textEdit18 = new TextEdit();
            labelControl60 = new LabelControl();
            textEdit19 = new TextEdit();
            labelControl61 = new LabelControl();
            textEdit20 = new TextEdit();
            labelControl62 = new LabelControl();
            安全区满血check = new CheckEdit();
            xtraTabPage2 = new XtraTabPage();
            checkEdit6 = new CheckEdit();
            checkEdit5 = new CheckEdit();
            textEdit7 = new TextEdit();
            labelControl47 = new LabelControl();
            radioGroup1 = new RadioGroup();
            checkEdit4 = new CheckEdit();
            checkEdit3 = new CheckEdit();
            checkEdit1 = new CheckEdit();
            AutoFight_New = new CheckEdit();
            checkEdit达最高级后继续加经验 = new CheckEdit();
            checkEdit屏蔽日程 = new CheckEdit();
            checkEdit屏蔽战功 = new CheckEdit();
            checkEdit屏蔽威望 = new CheckEdit();
            checkEdit屏蔽七天活动 = new CheckEdit();
            textEdit神佑掉落ID = new TextEdit();
            labelControl28 = new LabelControl();
            checkEdit普通强化不碎武器 = new CheckEdit();
            checkEditBOSS自动死亡 = new CheckEdit();
            textEdit暴击特效ID = new TextEdit();
            labelControl26 = new LabelControl();
            checkEdit可购买玛法特权 = new CheckEdit();
            checkEditResBag = new CheckEdit();
            ReBuildItemCheck = new CheckEdit();
            JobOpenCheck = new CheckEdit();
            PacketCheck = new CheckEdit();
            EnabledAchievementCheck = new CheckEdit();
            EnabledQuestCheck = new CheckEdit();
            AutoFightCheck = new CheckEdit();
            MobDropEdit = new TextEdit();
            labelControl22 = new LabelControl();
            MobExpEdit = new TextEdit();
            SRepairEdit = new TextEdit();
            DebuffRateEdit = new TextEdit();
            labelControl19 = new LabelControl();
            labelControl20 = new LabelControl();
            labelControl21 = new LabelControl();
            SlaveTimeEdit = new TextEdit();
            FightSExpEdit = new TextEdit();
            FightExpEdit = new TextEdit();
            FightTime2Edit = new TextEdit();
            FightTime1Edit = new TextEdit();
            labelControl14 = new LabelControl();
            labelControl15 = new LabelControl();
            labelControl16 = new LabelControl();
            labelControl17 = new LabelControl();
            labelControl18 = new LabelControl();
            DebuffLevelEdit = new TextEdit();
            ItemOwnerTimeEdit = new TextEdit();
            ItemCleanTimeEdit = new TextEdit();
            NewbieLevelEdit = new TextEdit();
            HighLevelEdit = new TextEdit();
            labelControl9 = new LabelControl();
            labelControl10 = new LabelControl();
            labelControl11 = new LabelControl();
            labelControl12 = new LabelControl();
            labelControl13 = new LabelControl();
            xtraTabPage1 = new XtraTabPage();
            groupControl5 = new GroupControl();
            labelControl6 = new LabelControl();
            labelControl7 = new LabelControl();
            ServerPathEdit = new ButtonEdit();
            SavePathEdit = new ButtonEdit();
            groupControl4 = new GroupControl();
            labelControl8 = new LabelControl();
            labelControl4 = new LabelControl();
            labelControl5 = new LabelControl();
            textEdit1 = new TextEdit();
            ExceptionBanEdit = new TextEdit();
            labelControl29 = new LabelControl();
            DisconnectTimeEdit = new TextEdit();
            ServerNameEdit = new TextEdit();
            AutoSafeTimeEdit = new TextEdit();
            labelControl24 = new LabelControl();
            StartDateEdit = new DateEdit();
            labelControl23 = new LabelControl();
            groupControl3 = new GroupControl();
            labelControl1 = new LabelControl();
            labelControl2 = new LabelControl();
            labelControl3 = new LabelControl();
            PortEdit = new TextEdit();
            TokenPortEdit = new TextEdit();
            PacketLimitEdit = new TextEdit();
            xtraTabControl1 = new XtraTabControl();
            xtraTabPage4 = new XtraTabPage();
            宠物下属分组 = new GroupControl();
            下线宝宝不死check = new CheckEdit();
            安全区分组 = new GroupControl();
            groupControl6 = new GroupControl();
            labelControl63 = new LabelControl();
            labelControl58 = new LabelControl();
            richEditControl2 = new DevExpress.XtraRichEdit.RichEditControl();
            richEditControl1 = new DevExpress.XtraRichEdit.RichEditControl();
            radioGroup2 = new DevExpress.XtraEditors.Repository.RepositoryItemRadioGroup();
            ((ISupportInitialize)ribbon).BeginInit();
            xtraTabPage3.SuspendLayout();
            ((ISupportInitialize)其他分组).BeginInit();
            其他分组.SuspendLayout();
            ((ISupportInitialize)textEdit15.Properties).BeginInit();
            ((ISupportInitialize)textEdit3.Properties).BeginInit();
            ((ISupportInitialize)textEdit5.Properties).BeginInit();
            ((ISupportInitialize)textEdit6.Properties).BeginInit();
            ((ISupportInitialize)textEdit13.Properties).BeginInit();
            ((ISupportInitialize)死亡掉落分组).BeginInit();
            死亡掉落分组.SuspendLayout();
            ((ISupportInitialize)textEdit_Dropweap.Properties).BeginInit();
            ((ISupportInitialize)textEdit_dropshoushi.Properties).BeginInit();
            ((ISupportInitialize)textEdit_dropbags.Properties).BeginInit();
            ((ISupportInitialize)textEdit4.Properties).BeginInit();
            ((ISupportInitialize)textEdit_Dropweap_red.Properties).BeginInit();
            ((ISupportInitialize)textEdit_dropshoushi_red.Properties).BeginInit();
            ((ISupportInitialize)祝福油几率分组).BeginInit();
            祝福油几率分组.SuspendLayout();
            ((ISupportInitialize)textEdit_zhufuyou_Lv1.Properties).BeginInit();
            ((ISupportInitialize)textEdit_zhufuyou_Lv2.Properties).BeginInit();
            ((ISupportInitialize)textEdit_zhufuyou_Lv3.Properties).BeginInit();
            ((ISupportInitialize)textEdit_zhufuyou_Lv4.Properties).BeginInit();
            ((ISupportInitialize)textEdit_zhufuyou_Lv5.Properties).BeginInit();
            ((ISupportInitialize)textEdit_zhufuyou_Lv6.Properties).BeginInit();
            ((ISupportInitialize)textEdit_zhufuyou_Lv0.Properties).BeginInit();
            ((ISupportInitialize)游戏地图开放分组).BeginInit();
            游戏地图开放分组.SuspendLayout();
            ((ISupportInitialize)textEdit9.Properties).BeginInit();
            ((ISupportInitialize)textEdit10.Properties).BeginInit();
            ((ISupportInitialize)textEdit11.Properties).BeginInit();
            ((ISupportInitialize)textEdit8.Properties).BeginInit();
            ((ISupportInitialize)textEdit12.Properties).BeginInit();
            ((ISupportInitialize)自动分解分组).BeginInit();
            自动分解分组.SuspendLayout();
            ((ISupportInitialize)分解装备check.Properties).BeginInit();
            ((ISupportInitialize)不分解极品check.Properties).BeginInit();
            ((ISupportInitialize)自动拾取分组).BeginInit();
            自动拾取分组.SuspendLayout();
            ((ISupportInitialize)金币入包check.Properties).BeginInit();
            ((ISupportInitialize)银币入包check.Properties).BeginInit();
            ((ISupportInitialize)物品入包check.Properties).BeginInit();
            ((ISupportInitialize)groupControl1).BeginInit();
            groupControl1.SuspendLayout();
            ((ISupportInitialize)textEdit14.Properties).BeginInit();
            ((ISupportInitialize)textEdit17.Properties).BeginInit();
            ((ISupportInitialize)textEdit18.Properties).BeginInit();
            ((ISupportInitialize)textEdit19.Properties).BeginInit();
            ((ISupportInitialize)textEdit20.Properties).BeginInit();
            ((ISupportInitialize)安全区满血check.Properties).BeginInit();
            xtraTabPage2.SuspendLayout();
            ((ISupportInitialize)checkEdit6.Properties).BeginInit();
            ((ISupportInitialize)checkEdit5.Properties).BeginInit();
            ((ISupportInitialize)textEdit7.Properties).BeginInit();
            ((ISupportInitialize)radioGroup1.Properties).BeginInit();
            ((ISupportInitialize)checkEdit4.Properties).BeginInit();
            ((ISupportInitialize)checkEdit3.Properties).BeginInit();
            ((ISupportInitialize)checkEdit1.Properties).BeginInit();
            ((ISupportInitialize)AutoFight_New.Properties).BeginInit();
            ((ISupportInitialize)checkEdit达最高级后继续加经验.Properties).BeginInit();
            ((ISupportInitialize)checkEdit屏蔽日程.Properties).BeginInit();
            ((ISupportInitialize)checkEdit屏蔽战功.Properties).BeginInit();
            ((ISupportInitialize)checkEdit屏蔽威望.Properties).BeginInit();
            ((ISupportInitialize)checkEdit屏蔽七天活动.Properties).BeginInit();
            ((ISupportInitialize)textEdit神佑掉落ID.Properties).BeginInit();
            ((ISupportInitialize)checkEdit普通强化不碎武器.Properties).BeginInit();
            ((ISupportInitialize)checkEditBOSS自动死亡.Properties).BeginInit();
            ((ISupportInitialize)textEdit暴击特效ID.Properties).BeginInit();
            ((ISupportInitialize)checkEdit可购买玛法特权.Properties).BeginInit();
            ((ISupportInitialize)checkEditResBag.Properties).BeginInit();
            ((ISupportInitialize)ReBuildItemCheck.Properties).BeginInit();
            ((ISupportInitialize)JobOpenCheck.Properties).BeginInit();
            ((ISupportInitialize)PacketCheck.Properties).BeginInit();
            ((ISupportInitialize)EnabledAchievementCheck.Properties).BeginInit();
            ((ISupportInitialize)EnabledQuestCheck.Properties).BeginInit();
            ((ISupportInitialize)AutoFightCheck.Properties).BeginInit();
            ((ISupportInitialize)MobDropEdit.Properties).BeginInit();
            ((ISupportInitialize)MobExpEdit.Properties).BeginInit();
            ((ISupportInitialize)SRepairEdit.Properties).BeginInit();
            ((ISupportInitialize)DebuffRateEdit.Properties).BeginInit();
            ((ISupportInitialize)SlaveTimeEdit.Properties).BeginInit();
            ((ISupportInitialize)FightSExpEdit.Properties).BeginInit();
            ((ISupportInitialize)FightExpEdit.Properties).BeginInit();
            ((ISupportInitialize)FightTime2Edit.Properties).BeginInit();
            ((ISupportInitialize)FightTime1Edit.Properties).BeginInit();
            ((ISupportInitialize)DebuffLevelEdit.Properties).BeginInit();
            ((ISupportInitialize)ItemOwnerTimeEdit.Properties).BeginInit();
            ((ISupportInitialize)ItemCleanTimeEdit.Properties).BeginInit();
            ((ISupportInitialize)NewbieLevelEdit.Properties).BeginInit();
            ((ISupportInitialize)HighLevelEdit.Properties).BeginInit();
            xtraTabPage1.SuspendLayout();
            ((ISupportInitialize)groupControl5).BeginInit();
            groupControl5.SuspendLayout();
            ((ISupportInitialize)ServerPathEdit.Properties).BeginInit();
            ((ISupportInitialize)SavePathEdit.Properties).BeginInit();
            ((ISupportInitialize)groupControl4).BeginInit();
            groupControl4.SuspendLayout();
            ((ISupportInitialize)textEdit1.Properties).BeginInit();
            ((ISupportInitialize)ExceptionBanEdit.Properties).BeginInit();
            ((ISupportInitialize)DisconnectTimeEdit.Properties).BeginInit();
            ((ISupportInitialize)ServerNameEdit.Properties).BeginInit();
            ((ISupportInitialize)AutoSafeTimeEdit.Properties).BeginInit();
            ((ISupportInitialize)StartDateEdit.Properties).BeginInit();
            ((ISupportInitialize)StartDateEdit.Properties.CalendarTimeProperties).BeginInit();
            ((ISupportInitialize)groupControl3).BeginInit();
            groupControl3.SuspendLayout();
            ((ISupportInitialize)PortEdit.Properties).BeginInit();
            ((ISupportInitialize)TokenPortEdit.Properties).BeginInit();
            ((ISupportInitialize)PacketLimitEdit.Properties).BeginInit();
            ((ISupportInitialize)xtraTabControl1).BeginInit();
            xtraTabControl1.SuspendLayout();
            xtraTabPage4.SuspendLayout();
            ((ISupportInitialize)宠物下属分组).BeginInit();
            宠物下属分组.SuspendLayout();
            ((ISupportInitialize)下线宝宝不死check.Properties).BeginInit();
            ((ISupportInitialize)安全区分组).BeginInit();
            安全区分组.SuspendLayout();
            ((ISupportInitialize)groupControl6).BeginInit();
            groupControl6.SuspendLayout();
            ((ISupportInitialize)radioGroup2).BeginInit();
            SuspendLayout();
            // 
            // ribbon
            // 
            ribbon.EmptyAreaImageOptions.ImagePadding = new Padding(35, 39, 35, 39);
            ribbon.ExpandCollapseItem.Id = 0;
            ribbon.Items.AddRange(new BarItem[] { ribbon.ExpandCollapseItem, SaveDataBaseButton, barButtonItem1, ReLoadDataBaseButton });
            ribbon.Location = new Point(0, 0);
            ribbon.Margin = new Padding(4);
            ribbon.MaxItemId = 5;
            ribbon.MdiMergeStyle = RibbonMdiMergeStyle.Always;
            ribbon.Name = "ribbon";
            ribbon.OptionsMenuMinWidth = 385;
            ribbon.Pages.AddRange(new RibbonPage[] { ribbonPage1 });
            ribbon.Size = new Size(1044, 148);
            // 
            // SaveDataBaseButton
            // 
            SaveDataBaseButton.Caption = "保存 设置";
            SaveDataBaseButton.Id = 1;
            SaveDataBaseButton.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("SaveDataBaseButton.ImageOptions.SvgImage");
            SaveDataBaseButton.LargeWidth = 50;
            SaveDataBaseButton.Name = "SaveDataBaseButton";
            SaveDataBaseButton.ItemClick += SaveDataBaseButton_ItemClick;
            // 
            // barButtonItem1
            // 
            barButtonItem1.Caption = "重新加载";
            barButtonItem1.Id = 2;
            barButtonItem1.Name = "barButtonItem1";
            // 
            // ReLoadDataBaseButton
            // 
            ReLoadDataBaseButton.Caption = "重载 设置";
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
            ribbonPageGroup1.ItemLinks.Add(SaveDataBaseButton);
            ribbonPageGroup1.ItemLinks.Add(ReLoadDataBaseButton);
            ribbonPageGroup1.Name = "ribbonPageGroup1";
            ribbonPageGroup1.Text = "动作";
            // 
            // FolderDialog
            // 
            FolderDialog.SelectedPath = "xtraFolderBrowserDialog1";
            // 
            // row
            // 
            row.Name = "row";
            row.Properties.Caption = "row";
            // 
            // row1
            // 
            row1.Name = "row1";
            row1.Properties.Caption = "row1";
            // 
            // row2
            // 
            row2.Name = "row2";
            row2.Properties.Caption = "row2";
            // 
            // row3
            // 
            row3.Name = "row3";
            row3.Properties.Caption = "row3";
            // 
            // row4
            // 
            row4.Name = "row4";
            row4.Properties.Caption = "row4";
            // 
            // row5
            // 
            row5.Name = "row5";
            row5.Properties.Caption = "row5";
            // 
            // row6
            // 
            row6.Name = "row6";
            row6.Properties.Caption = "row6";
            // 
            // xtraTabPage3
            // 
            xtraTabPage3.Controls.Add(其他分组);
            xtraTabPage3.Controls.Add(死亡掉落分组);
            xtraTabPage3.Controls.Add(祝福油几率分组);
            xtraTabPage3.Controls.Add(游戏地图开放分组);
            xtraTabPage3.Controls.Add(自动分解分组);
            xtraTabPage3.Controls.Add(自动拾取分组);
            xtraTabPage3.Controls.Add(groupControl1);
            xtraTabPage3.Name = "xtraTabPage3";
            xtraTabPage3.Size = new Size(1038, 468);
            xtraTabPage3.Text = "设置2";
            xtraTabPage3.Paint += xtraTabPage3_Paint;
            // 
            // 其他分组
            // 
            其他分组.Controls.Add(textEdit15);
            其他分组.Controls.Add(labelControl44);
            其他分组.Controls.Add(textEdit3);
            其他分组.Controls.Add(labelControl57);
            其他分组.Controls.Add(labelControl45);
            其他分组.Controls.Add(textEdit5);
            其他分组.Controls.Add(labelControl46);
            其他分组.Controls.Add(textEdit6);
            其他分组.Controls.Add(labelControl55);
            其他分组.Controls.Add(textEdit13);
            其他分组.Location = new Point(218, 234);
            其他分组.Name = "其他分组";
            其他分组.Size = new Size(200, 197);
            其他分组.TabIndex = 133;
            其他分组.Text = "其他设置";
            // 
            // textEdit15
            // 
            textEdit15.Location = new Point(98, 151);
            textEdit15.Margin = new Padding(4);
            textEdit15.MenuManager = ribbon;
            textEdit15.Name = "textEdit15";
            textEdit15.Properties.Appearance.Options.UseTextOptions = true;
            textEdit15.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit15.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit15.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit15.Properties.MaskSettings.Set("mask", "n3");
            textEdit15.Size = new Size(86, 20);
            textEdit15.TabIndex = 111;
            // 
            // labelControl44
            // 
            labelControl44.Location = new Point(18, 47);
            labelControl44.Margin = new Padding(3, 4, 3, 4);
            labelControl44.Name = "labelControl44";
            labelControl44.Size = new Size(72, 14);
            labelControl44.TabIndex = 101;
            labelControl44.Text = "龙卫重铸费用";
            // 
            // textEdit3
            // 
            textEdit3.EditValue = "10000";
            textEdit3.Location = new Point(98, 43);
            textEdit3.Margin = new Padding(4);
            textEdit3.MenuManager = ribbon;
            textEdit3.Name = "textEdit3";
            textEdit3.Properties.Appearance.Options.UseTextOptions = true;
            textEdit3.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit3.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit3.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit3.Properties.MaskSettings.Set("mask", "n0");
            textEdit3.Size = new Size(85, 20);
            textEdit3.TabIndex = 102;
            // 
            // labelControl57
            // 
            labelControl57.Location = new Point(18, 154);
            labelControl57.Margin = new Padding(3, 4, 3, 4);
            labelControl57.Name = "labelControl57";
            labelControl57.Size = new Size(72, 14);
            labelControl57.TabIndex = 110;
            labelControl57.Text = "技巧项链倍数";
            // 
            // labelControl45
            // 
            labelControl45.Location = new Point(18, 76);
            labelControl45.Margin = new Padding(3, 4, 3, 4);
            labelControl45.Name = "labelControl45";
            labelControl45.Size = new Size(72, 14);
            labelControl45.TabIndex = 103;
            labelControl45.Text = "锁单重铸费用";
            // 
            // textEdit5
            // 
            textEdit5.EditValue = "250000";
            textEdit5.Location = new Point(98, 73);
            textEdit5.Margin = new Padding(4);
            textEdit5.MenuManager = ribbon;
            textEdit5.Name = "textEdit5";
            textEdit5.Properties.Appearance.Options.UseTextOptions = true;
            textEdit5.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit5.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit5.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit5.Properties.MaskSettings.Set("mask", "n0");
            textEdit5.Size = new Size(85, 20);
            textEdit5.TabIndex = 104;
            // 
            // labelControl46
            // 
            labelControl46.Location = new Point(18, 102);
            labelControl46.Margin = new Padding(3, 4, 3, 4);
            labelControl46.Name = "labelControl46";
            labelControl46.Size = new Size(72, 14);
            labelControl46.TabIndex = 105;
            labelControl46.Text = "锁半重铸费用";
            // 
            // textEdit6
            // 
            textEdit6.EditValue = "500000";
            textEdit6.Location = new Point(98, 99);
            textEdit6.Margin = new Padding(4);
            textEdit6.MenuManager = ribbon;
            textEdit6.Name = "textEdit6";
            textEdit6.Properties.Appearance.Options.UseTextOptions = true;
            textEdit6.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit6.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit6.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit6.Properties.MaskSettings.Set("mask", "n0");
            textEdit6.Size = new Size(85, 20);
            textEdit6.TabIndex = 106;
            // 
            // labelControl55
            // 
            labelControl55.Location = new Point(18, 128);
            labelControl55.Margin = new Padding(3, 4, 3, 4);
            labelControl55.Name = "labelControl55";
            labelControl55.Size = new Size(72, 14);
            labelControl55.TabIndex = 120;
            labelControl55.Text = "聊天限制等级";
            // 
            // textEdit13
            // 
            textEdit13.Location = new Point(98, 125);
            textEdit13.Margin = new Padding(4);
            textEdit13.MenuManager = ribbon;
            textEdit13.Name = "textEdit13";
            textEdit13.Properties.Appearance.Options.UseTextOptions = true;
            textEdit13.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit13.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit13.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit13.Properties.MaskSettings.Set("mask", "n0");
            textEdit13.Size = new Size(85, 20);
            textEdit13.TabIndex = 121;
            // 
            // 死亡掉落分组
            // 
            死亡掉落分组.Controls.Add(labelControl41);
            死亡掉落分组.Controls.Add(labelControl38);
            死亡掉落分组.Controls.Add(textEdit_Dropweap);
            死亡掉落分组.Controls.Add(labelControl39);
            死亡掉落分组.Controls.Add(textEdit_dropshoushi);
            死亡掉落分组.Controls.Add(labelControl40);
            死亡掉落分组.Controls.Add(textEdit_dropbags);
            死亡掉落分组.Controls.Add(textEdit4);
            死亡掉落分组.Controls.Add(labelControl42);
            死亡掉落分组.Controls.Add(textEdit_Dropweap_red);
            死亡掉落分组.Controls.Add(labelControl30);
            死亡掉落分组.Controls.Add(textEdit_dropshoushi_red);
            死亡掉落分组.Location = new Point(11, 13);
            死亡掉落分组.Name = "死亡掉落分组";
            死亡掉落分组.Size = new Size(201, 215);
            死亡掉落分组.TabIndex = 132;
            死亡掉落分组.Text = "死亡掉落设置";
            // 
            // labelControl41
            // 
            labelControl41.Location = new Point(22, 128);
            labelControl41.Margin = new Padding(3, 4, 3, 4);
            labelControl41.Name = "labelControl41";
            labelControl41.Size = new Size(72, 14);
            labelControl41.TabIndex = 94;
            labelControl41.Text = "死亡掉装限量";
            // 
            // labelControl38
            // 
            labelControl38.Location = new Point(22, 40);
            labelControl38.Margin = new Padding(3, 4, 3, 4);
            labelControl38.Name = "labelControl38";
            labelControl38.Size = new Size(72, 14);
            labelControl38.TabIndex = 88;
            labelControl38.Text = "死亡掉落剑甲";
            // 
            // textEdit_Dropweap
            // 
            textEdit_Dropweap.Location = new Point(101, 37);
            textEdit_Dropweap.Margin = new Padding(4);
            textEdit_Dropweap.MenuManager = ribbon;
            textEdit_Dropweap.Name = "textEdit_Dropweap";
            textEdit_Dropweap.Properties.Appearance.Options.UseTextOptions = true;
            textEdit_Dropweap.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit_Dropweap.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit_Dropweap.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit_Dropweap.Properties.MaskSettings.Set("mask", "n3");
            textEdit_Dropweap.Size = new Size(75, 20);
            textEdit_Dropweap.TabIndex = 89;
            // 
            // labelControl39
            // 
            labelControl39.Location = new Point(22, 68);
            labelControl39.Margin = new Padding(3, 4, 3, 4);
            labelControl39.Name = "labelControl39";
            labelControl39.Size = new Size(72, 14);
            labelControl39.TabIndex = 90;
            labelControl39.Text = "死亡掉落首饰";
            // 
            // textEdit_dropshoushi
            // 
            textEdit_dropshoushi.Location = new Point(101, 65);
            textEdit_dropshoushi.Margin = new Padding(4);
            textEdit_dropshoushi.MenuManager = ribbon;
            textEdit_dropshoushi.Name = "textEdit_dropshoushi";
            textEdit_dropshoushi.Properties.Appearance.Options.UseTextOptions = true;
            textEdit_dropshoushi.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit_dropshoushi.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit_dropshoushi.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit_dropshoushi.Properties.MaskSettings.Set("mask", "n3");
            textEdit_dropshoushi.Size = new Size(75, 20);
            textEdit_dropshoushi.TabIndex = 91;
            // 
            // labelControl40
            // 
            labelControl40.Location = new Point(22, 96);
            labelControl40.Margin = new Padding(3, 4, 3, 4);
            labelControl40.Name = "labelControl40";
            labelControl40.Size = new Size(72, 14);
            labelControl40.TabIndex = 92;
            labelControl40.Text = "死亡掉落背包";
            // 
            // textEdit_dropbags
            // 
            textEdit_dropbags.Location = new Point(101, 93);
            textEdit_dropbags.Margin = new Padding(4);
            textEdit_dropbags.MenuManager = ribbon;
            textEdit_dropbags.Name = "textEdit_dropbags";
            textEdit_dropbags.Properties.Appearance.Options.UseTextOptions = true;
            textEdit_dropbags.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit_dropbags.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit_dropbags.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit_dropbags.Properties.MaskSettings.Set("mask", "n3");
            textEdit_dropbags.Size = new Size(75, 20);
            textEdit_dropbags.TabIndex = 93;
            // 
            // textEdit4
            // 
            textEdit4.Location = new Point(101, 125);
            textEdit4.Margin = new Padding(4);
            textEdit4.MenuManager = ribbon;
            textEdit4.Name = "textEdit4";
            textEdit4.Properties.Appearance.Options.UseTextOptions = true;
            textEdit4.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit4.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit4.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit4.Properties.MaskSettings.Set("mask", "n0");
            textEdit4.Size = new Size(75, 20);
            textEdit4.TabIndex = 96;
            // 
            // labelControl42
            // 
            labelControl42.Location = new Point(22, 156);
            labelControl42.Margin = new Padding(3, 4, 3, 4);
            labelControl42.Name = "labelControl42";
            labelControl42.Size = new Size(72, 14);
            labelControl42.TabIndex = 97;
            labelControl42.Text = "红名掉落剑甲";
            // 
            // textEdit_Dropweap_red
            // 
            textEdit_Dropweap_red.Location = new Point(101, 153);
            textEdit_Dropweap_red.Margin = new Padding(4);
            textEdit_Dropweap_red.MenuManager = ribbon;
            textEdit_Dropweap_red.Name = "textEdit_Dropweap_red";
            textEdit_Dropweap_red.Properties.Appearance.Options.UseTextOptions = true;
            textEdit_Dropweap_red.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit_Dropweap_red.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit_Dropweap_red.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit_Dropweap_red.Properties.MaskSettings.Set("mask", "n3");
            textEdit_Dropweap_red.Size = new Size(75, 20);
            textEdit_Dropweap_red.TabIndex = 98;
            // 
            // labelControl30
            // 
            labelControl30.Location = new Point(22, 184);
            labelControl30.Margin = new Padding(3, 4, 3, 4);
            labelControl30.Name = "labelControl30";
            labelControl30.Size = new Size(72, 14);
            labelControl30.TabIndex = 99;
            labelControl30.Text = "红名掉落首饰";
            // 
            // textEdit_dropshoushi_red
            // 
            textEdit_dropshoushi_red.Location = new Point(101, 181);
            textEdit_dropshoushi_red.Margin = new Padding(4);
            textEdit_dropshoushi_red.MenuManager = ribbon;
            textEdit_dropshoushi_red.Name = "textEdit_dropshoushi_red";
            textEdit_dropshoushi_red.Properties.Appearance.Options.UseTextOptions = true;
            textEdit_dropshoushi_red.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit_dropshoushi_red.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit_dropshoushi_red.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit_dropshoushi_red.Properties.MaskSettings.Set("mask", "n3");
            textEdit_dropshoushi_red.Size = new Size(75, 20);
            textEdit_dropshoushi_red.TabIndex = 100;
            // 
            // 祝福油几率分组
            // 
            祝福油几率分组.Controls.Add(labelControl37);
            祝福油几率分组.Controls.Add(labelControl31);
            祝福油几率分组.Controls.Add(textEdit_zhufuyou_Lv1);
            祝福油几率分组.Controls.Add(labelControl32);
            祝福油几率分组.Controls.Add(textEdit_zhufuyou_Lv2);
            祝福油几率分组.Controls.Add(labelControl34);
            祝福油几率分组.Controls.Add(textEdit_zhufuyou_Lv3);
            祝福油几率分组.Controls.Add(labelControl33);
            祝福油几率分组.Controls.Add(textEdit_zhufuyou_Lv4);
            祝福油几率分组.Controls.Add(labelControl36);
            祝福油几率分组.Controls.Add(textEdit_zhufuyou_Lv5);
            祝福油几率分组.Controls.Add(labelControl35);
            祝福油几率分组.Controls.Add(textEdit_zhufuyou_Lv6);
            祝福油几率分组.Controls.Add(textEdit_zhufuyou_Lv0);
            祝福油几率分组.Location = new Point(424, 12);
            祝福油几率分组.Name = "祝福油几率分组";
            祝福油几率分组.Size = new Size(233, 156);
            祝福油几率分组.TabIndex = 131;
            祝福油几率分组.Text = "祝福油几率 (%)";
            // 
            // labelControl37
            // 
            labelControl37.Location = new Point(19, 40);
            labelControl37.Margin = new Padding(3, 4, 3, 4);
            labelControl37.Name = "labelControl37";
            labelControl37.Size = new Size(19, 14);
            labelControl37.TabIndex = 86;
            labelControl37.Text = "1级";
            // 
            // labelControl31
            // 
            labelControl31.Location = new Point(19, 68);
            labelControl31.Margin = new Padding(3, 4, 3, 4);
            labelControl31.Name = "labelControl31";
            labelControl31.Size = new Size(19, 14);
            labelControl31.TabIndex = 74;
            labelControl31.Text = "2级";
            // 
            // textEdit_zhufuyou_Lv1
            // 
            textEdit_zhufuyou_Lv1.Location = new Point(45, 66);
            textEdit_zhufuyou_Lv1.Margin = new Padding(4);
            textEdit_zhufuyou_Lv1.MenuManager = ribbon;
            textEdit_zhufuyou_Lv1.Name = "textEdit_zhufuyou_Lv1";
            textEdit_zhufuyou_Lv1.Properties.Appearance.Options.UseTextOptions = true;
            textEdit_zhufuyou_Lv1.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit_zhufuyou_Lv1.Properties.EditFormat.FormatType = FormatType.Numeric;
            textEdit_zhufuyou_Lv1.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit_zhufuyou_Lv1.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit_zhufuyou_Lv1.Properties.MaskSettings.Set("mask", "d");
            textEdit_zhufuyou_Lv1.Properties.MaxLength = 2;
            textEdit_zhufuyou_Lv1.Size = new Size(69, 20);
            textEdit_zhufuyou_Lv1.TabIndex = 75;
            // 
            // labelControl32
            // 
            labelControl32.Location = new Point(19, 93);
            labelControl32.Margin = new Padding(3, 4, 3, 4);
            labelControl32.Name = "labelControl32";
            labelControl32.Size = new Size(19, 14);
            labelControl32.TabIndex = 76;
            labelControl32.Text = "3级";
            // 
            // textEdit_zhufuyou_Lv2
            // 
            textEdit_zhufuyou_Lv2.Location = new Point(45, 94);
            textEdit_zhufuyou_Lv2.Margin = new Padding(4);
            textEdit_zhufuyou_Lv2.MenuManager = ribbon;
            textEdit_zhufuyou_Lv2.Name = "textEdit_zhufuyou_Lv2";
            textEdit_zhufuyou_Lv2.Properties.Appearance.Options.UseTextOptions = true;
            textEdit_zhufuyou_Lv2.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit_zhufuyou_Lv2.Properties.EditFormat.FormatType = FormatType.Numeric;
            textEdit_zhufuyou_Lv2.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit_zhufuyou_Lv2.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit_zhufuyou_Lv2.Properties.MaskSettings.Set("mask", "d");
            textEdit_zhufuyou_Lv2.Properties.MaxLength = 2;
            textEdit_zhufuyou_Lv2.Size = new Size(69, 20);
            textEdit_zhufuyou_Lv2.TabIndex = 77;
            // 
            // labelControl34
            // 
            labelControl34.Location = new Point(19, 121);
            labelControl34.Margin = new Padding(3, 4, 3, 4);
            labelControl34.Name = "labelControl34";
            labelControl34.Size = new Size(19, 14);
            labelControl34.TabIndex = 78;
            labelControl34.Text = "4级";
            // 
            // textEdit_zhufuyou_Lv3
            // 
            textEdit_zhufuyou_Lv3.Location = new Point(45, 122);
            textEdit_zhufuyou_Lv3.Margin = new Padding(4);
            textEdit_zhufuyou_Lv3.MenuManager = ribbon;
            textEdit_zhufuyou_Lv3.Name = "textEdit_zhufuyou_Lv3";
            textEdit_zhufuyou_Lv3.Properties.Appearance.Options.UseTextOptions = true;
            textEdit_zhufuyou_Lv3.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit_zhufuyou_Lv3.Properties.EditFormat.FormatType = FormatType.Numeric;
            textEdit_zhufuyou_Lv3.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit_zhufuyou_Lv3.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit_zhufuyou_Lv3.Properties.MaskSettings.Set("mask", "d");
            textEdit_zhufuyou_Lv3.Properties.MaxLength = 2;
            textEdit_zhufuyou_Lv3.Size = new Size(69, 20);
            textEdit_zhufuyou_Lv3.TabIndex = 79;
            // 
            // labelControl33
            // 
            labelControl33.Location = new Point(121, 40);
            labelControl33.Margin = new Padding(3, 4, 3, 4);
            labelControl33.Name = "labelControl33";
            labelControl33.Size = new Size(19, 14);
            labelControl33.TabIndex = 80;
            labelControl33.Text = "5级";
            // 
            // textEdit_zhufuyou_Lv4
            // 
            textEdit_zhufuyou_Lv4.Location = new Point(147, 38);
            textEdit_zhufuyou_Lv4.Margin = new Padding(4);
            textEdit_zhufuyou_Lv4.MenuManager = ribbon;
            textEdit_zhufuyou_Lv4.Name = "textEdit_zhufuyou_Lv4";
            textEdit_zhufuyou_Lv4.Properties.Appearance.Options.UseTextOptions = true;
            textEdit_zhufuyou_Lv4.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit_zhufuyou_Lv4.Properties.EditFormat.FormatType = FormatType.Numeric;
            textEdit_zhufuyou_Lv4.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit_zhufuyou_Lv4.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit_zhufuyou_Lv4.Properties.MaskSettings.Set("mask", "d");
            textEdit_zhufuyou_Lv4.Properties.MaxLength = 2;
            textEdit_zhufuyou_Lv4.Size = new Size(78, 20);
            textEdit_zhufuyou_Lv4.TabIndex = 81;
            // 
            // labelControl36
            // 
            labelControl36.Location = new Point(121, 68);
            labelControl36.Margin = new Padding(3, 4, 3, 4);
            labelControl36.Name = "labelControl36";
            labelControl36.Size = new Size(19, 14);
            labelControl36.TabIndex = 82;
            labelControl36.Text = "6级";
            // 
            // textEdit_zhufuyou_Lv5
            // 
            textEdit_zhufuyou_Lv5.Location = new Point(147, 66);
            textEdit_zhufuyou_Lv5.Margin = new Padding(4);
            textEdit_zhufuyou_Lv5.MenuManager = ribbon;
            textEdit_zhufuyou_Lv5.Name = "textEdit_zhufuyou_Lv5";
            textEdit_zhufuyou_Lv5.Properties.Appearance.Options.UseTextOptions = true;
            textEdit_zhufuyou_Lv5.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit_zhufuyou_Lv5.Properties.EditFormat.FormatType = FormatType.Numeric;
            textEdit_zhufuyou_Lv5.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit_zhufuyou_Lv5.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit_zhufuyou_Lv5.Properties.MaskSettings.Set("mask", "d");
            textEdit_zhufuyou_Lv5.Properties.MaxLength = 2;
            textEdit_zhufuyou_Lv5.Size = new Size(78, 20);
            textEdit_zhufuyou_Lv5.TabIndex = 83;
            // 
            // labelControl35
            // 
            labelControl35.Location = new Point(121, 96);
            labelControl35.Margin = new Padding(3, 4, 3, 4);
            labelControl35.Name = "labelControl35";
            labelControl35.Size = new Size(19, 14);
            labelControl35.TabIndex = 84;
            labelControl35.Text = "7级";
            // 
            // textEdit_zhufuyou_Lv6
            // 
            textEdit_zhufuyou_Lv6.Location = new Point(145, 94);
            textEdit_zhufuyou_Lv6.Margin = new Padding(4);
            textEdit_zhufuyou_Lv6.MenuManager = ribbon;
            textEdit_zhufuyou_Lv6.Name = "textEdit_zhufuyou_Lv6";
            textEdit_zhufuyou_Lv6.Properties.Appearance.Options.UseTextOptions = true;
            textEdit_zhufuyou_Lv6.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit_zhufuyou_Lv6.Properties.EditFormat.FormatType = FormatType.Numeric;
            textEdit_zhufuyou_Lv6.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit_zhufuyou_Lv6.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit_zhufuyou_Lv6.Properties.MaskSettings.Set("mask", "d");
            textEdit_zhufuyou_Lv6.Properties.MaxLength = 2;
            textEdit_zhufuyou_Lv6.Size = new Size(78, 20);
            textEdit_zhufuyou_Lv6.TabIndex = 85;
            // 
            // textEdit_zhufuyou_Lv0
            // 
            textEdit_zhufuyou_Lv0.Location = new Point(45, 38);
            textEdit_zhufuyou_Lv0.Margin = new Padding(4);
            textEdit_zhufuyou_Lv0.MenuManager = ribbon;
            textEdit_zhufuyou_Lv0.Name = "textEdit_zhufuyou_Lv0";
            textEdit_zhufuyou_Lv0.Properties.Appearance.Options.UseTextOptions = true;
            textEdit_zhufuyou_Lv0.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit_zhufuyou_Lv0.Properties.EditFormat.FormatType = FormatType.Numeric;
            textEdit_zhufuyou_Lv0.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit_zhufuyou_Lv0.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit_zhufuyou_Lv0.Properties.MaskSettings.Set("mask", "d");
            textEdit_zhufuyou_Lv0.Properties.MaxLength = 2;
            textEdit_zhufuyou_Lv0.Size = new Size(69, 20);
            textEdit_zhufuyou_Lv0.TabIndex = 87;
            // 
            // 游戏地图开放分组
            // 
            游戏地图开放分组.Controls.Add(labelControl51);
            游戏地图开放分组.Controls.Add(textEdit9);
            游戏地图开放分组.Controls.Add(labelControl52);
            游戏地图开放分组.Controls.Add(textEdit10);
            游戏地图开放分组.Controls.Add(labelControl53);
            游戏地图开放分组.Controls.Add(textEdit11);
            游戏地图开放分组.Controls.Add(textEdit8);
            游戏地图开放分组.Controls.Add(labelControl54);
            游戏地图开放分组.Controls.Add(labelControl49);
            游戏地图开放分组.Controls.Add(textEdit12);
            游戏地图开放分组.Location = new Point(218, 12);
            游戏地图开放分组.Name = "游戏地图开放分组";
            游戏地图开放分组.Size = new Size(200, 216);
            游戏地图开放分组.TabIndex = 130;
            游戏地图开放分组.Text = "游戏地图开放天数 (天)";
            // 
            // labelControl51
            // 
            labelControl51.Location = new Point(18, 55);
            labelControl51.Margin = new Padding(3, 4, 3, 4);
            labelControl51.Name = "labelControl51";
            labelControl51.Size = new Size(48, 14);
            labelControl51.TabIndex = 110;
            labelControl51.Text = "幽冥海底";
            // 
            // textEdit9
            // 
            textEdit9.Location = new Point(73, 80);
            textEdit9.Margin = new Padding(4);
            textEdit9.MenuManager = ribbon;
            textEdit9.Name = "textEdit9";
            textEdit9.Properties.Appearance.Options.UseTextOptions = true;
            textEdit9.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit9.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit9.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit9.Properties.MaskSettings.Set("mask", "n0");
            textEdit9.Size = new Size(102, 20);
            textEdit9.TabIndex = 111;
            // 
            // labelControl52
            // 
            labelControl52.Location = new Point(18, 83);
            labelControl52.Margin = new Padding(3, 4, 3, 4);
            labelControl52.Name = "labelControl52";
            labelControl52.Size = new Size(48, 14);
            labelControl52.TabIndex = 112;
            labelControl52.Text = "白日赤月";
            // 
            // textEdit10
            // 
            textEdit10.Location = new Point(73, 108);
            textEdit10.Margin = new Padding(4);
            textEdit10.MenuManager = ribbon;
            textEdit10.Name = "textEdit10";
            textEdit10.Properties.Appearance.Options.UseTextOptions = true;
            textEdit10.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit10.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit10.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit10.Properties.MaskSettings.Set("mask", "n0");
            textEdit10.Size = new Size(102, 20);
            textEdit10.TabIndex = 113;
            // 
            // labelControl53
            // 
            labelControl53.Location = new Point(18, 111);
            labelControl53.Margin = new Padding(3, 4, 3, 4);
            labelControl53.Name = "labelControl53";
            labelControl53.Size = new Size(48, 14);
            labelControl53.TabIndex = 114;
            labelControl53.Text = "魔龙之城";
            // 
            // textEdit11
            // 
            textEdit11.Location = new Point(73, 136);
            textEdit11.Margin = new Padding(4);
            textEdit11.MenuManager = ribbon;
            textEdit11.Name = "textEdit11";
            textEdit11.Properties.Appearance.Options.UseTextOptions = true;
            textEdit11.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit11.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit11.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit11.Properties.MaskSettings.Set("mask", "n0");
            textEdit11.Size = new Size(102, 20);
            textEdit11.TabIndex = 115;
            // 
            // textEdit8
            // 
            textEdit8.Location = new Point(73, 52);
            textEdit8.Margin = new Padding(4);
            textEdit8.MenuManager = ribbon;
            textEdit8.Name = "textEdit8";
            textEdit8.Properties.Appearance.Options.UseTextOptions = true;
            textEdit8.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit8.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit8.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit8.Properties.MaskSettings.Set("mask", "n0");
            textEdit8.Size = new Size(102, 20);
            textEdit8.TabIndex = 119;
            // 
            // labelControl54
            // 
            labelControl54.Location = new Point(18, 139);
            labelControl54.Margin = new Padding(3, 4, 3, 4);
            labelControl54.Name = "labelControl54";
            labelControl54.Size = new Size(48, 14);
            labelControl54.TabIndex = 116;
            labelControl54.Text = "苍月惊变";
            // 
            // labelControl49
            // 
            labelControl49.Location = new Point(18, 167);
            labelControl49.Margin = new Padding(3, 4, 3, 4);
            labelControl49.Name = "labelControl49";
            labelControl49.Size = new Size(48, 14);
            labelControl49.TabIndex = 118;
            labelControl49.Text = "龙耀雪山";
            // 
            // textEdit12
            // 
            textEdit12.Location = new Point(73, 164);
            textEdit12.Margin = new Padding(4);
            textEdit12.MenuManager = ribbon;
            textEdit12.Name = "textEdit12";
            textEdit12.Properties.Appearance.Options.UseTextOptions = true;
            textEdit12.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit12.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit12.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit12.Properties.MaskSettings.Set("mask", "n0");
            textEdit12.Size = new Size(102, 20);
            textEdit12.TabIndex = 117;
            // 
            // 自动分解分组
            // 
            自动分解分组.Controls.Add(分解装备check);
            自动分解分组.Controls.Add(不分解极品check);
            自动分解分组.Location = new Point(424, 271);
            自动分解分组.Name = "自动分解分组";
            自动分解分组.Size = new Size(233, 79);
            自动分解分组.TabIndex = 129;
            自动分解分组.Text = "自动分解";
            // 
            // 分解装备check
            // 
            分解装备check.Location = new Point(14, 39);
            分解装备check.MenuManager = ribbon;
            分解装备check.Name = "分解装备check";
            分解装备check.Properties.Caption = "自动分解装备";
            分解装备check.Size = new Size(100, 19);
            分解装备check.TabIndex = 126;
            分解装备check.CheckedChanged += 分解装备check_CheckedChanged;
            // 
            // 不分解极品check
            // 
            不分解极品check.Enabled = false;
            不分解极品check.Location = new Point(120, 39);
            不分解极品check.MenuManager = ribbon;
            不分解极品check.Name = "不分解极品check";
            不分解极品check.Properties.Caption = "不分解极品装备";
            不分解极品check.Size = new Size(110, 19);
            不分解极品check.TabIndex = 127;
            // 
            // 自动拾取分组
            // 
            自动拾取分组.Controls.Add(金币入包check);
            自动拾取分组.Controls.Add(银币入包check);
            自动拾取分组.Controls.Add(物品入包check);
            自动拾取分组.Location = new Point(424, 174);
            自动拾取分组.Name = "自动拾取分组";
            自动拾取分组.Size = new Size(233, 91);
            自动拾取分组.TabIndex = 128;
            自动拾取分组.Text = "自动拾取入包";
            // 
            // 金币入包check
            // 
            金币入包check.Location = new Point(120, 34);
            金币入包check.MenuManager = ribbon;
            金币入包check.Name = "金币入包check";
            金币入包check.Properties.Caption = "金币自动入包";
            金币入包check.Size = new Size(100, 19);
            金币入包check.TabIndex = 123;
            // 
            // 银币入包check
            // 
            银币入包check.Location = new Point(14, 32);
            银币入包check.MenuManager = ribbon;
            银币入包check.Name = "银币入包check";
            银币入包check.Properties.Caption = "银币自动入包";
            银币入包check.Size = new Size(100, 19);
            银币入包check.TabIndex = 124;
            // 
            // 物品入包check
            // 
            物品入包check.Location = new Point(14, 60);
            物品入包check.MenuManager = ribbon;
            物品入包check.Name = "物品入包check";
            物品入包check.Properties.Caption = "物品自动入包(过滤物品除外)";
            物品入包check.Size = new Size(182, 19);
            物品入包check.TabIndex = 125;
            // 
            // groupControl1
            // 
            groupControl1.Controls.Add(textEdit14);
            groupControl1.Controls.Add(labelControl56);
            groupControl1.Controls.Add(textEdit17);
            groupControl1.Controls.Add(labelControl59);
            groupControl1.Controls.Add(textEdit18);
            groupControl1.Controls.Add(labelControl60);
            groupControl1.Controls.Add(textEdit19);
            groupControl1.Controls.Add(labelControl61);
            groupControl1.Controls.Add(textEdit20);
            groupControl1.Controls.Add(labelControl62);
            groupControl1.Location = new Point(11, 234);
            groupControl1.Name = "groupControl1";
            groupControl1.Size = new Size(201, 197);
            groupControl1.TabIndex = 122;
            groupControl1.Text = "玛法特权";
            // 
            // textEdit14
            // 
            textEdit14.Location = new Point(77, 44);
            textEdit14.Margin = new Padding(4);
            textEdit14.MenuManager = ribbon;
            textEdit14.Name = "textEdit14";
            textEdit14.Properties.Appearance.Options.UseTextOptions = true;
            textEdit14.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit14.Properties.EditFormat.FormatType = FormatType.Numeric;
            textEdit14.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit14.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit14.Properties.MaskSettings.Set("mask", "d");
            textEdit14.Properties.MaxLength = 2;
            textEdit14.Size = new Size(107, 20);
            textEdit14.TabIndex = 101;
            // 
            // labelControl56
            // 
            labelControl56.Location = new Point(22, 47);
            labelControl56.Margin = new Padding(3, 4, 3, 4);
            labelControl56.Name = "labelControl56";
            labelControl56.Size = new Size(48, 14);
            labelControl56.TabIndex = 100;
            labelControl56.Text = "玛法新秀";
            // 
            // textEdit17
            // 
            textEdit17.Location = new Point(77, 160);
            textEdit17.Margin = new Padding(4);
            textEdit17.MenuManager = ribbon;
            textEdit17.Name = "textEdit17";
            textEdit17.Properties.Appearance.Options.UseTextOptions = true;
            textEdit17.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit17.Properties.EditFormat.FormatType = FormatType.Numeric;
            textEdit17.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit17.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit17.Properties.MaskSettings.Set("mask", "d");
            textEdit17.Properties.MaxLength = 2;
            textEdit17.Size = new Size(107, 20);
            textEdit17.TabIndex = 95;
            // 
            // labelControl59
            // 
            labelControl59.Location = new Point(22, 163);
            labelControl59.Margin = new Padding(3, 4, 3, 4);
            labelControl59.Name = "labelControl59";
            labelControl59.Size = new Size(48, 14);
            labelControl59.TabIndex = 94;
            labelControl59.Text = "玛法至尊";
            // 
            // textEdit18
            // 
            textEdit18.Location = new Point(77, 132);
            textEdit18.Margin = new Padding(4);
            textEdit18.MenuManager = ribbon;
            textEdit18.Name = "textEdit18";
            textEdit18.Properties.Appearance.Options.UseTextOptions = true;
            textEdit18.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit18.Properties.EditFormat.FormatType = FormatType.Numeric;
            textEdit18.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit18.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit18.Properties.MaskSettings.Set("mask", "d");
            textEdit18.Properties.MaxLength = 2;
            textEdit18.Size = new Size(107, 20);
            textEdit18.TabIndex = 93;
            // 
            // labelControl60
            // 
            labelControl60.Location = new Point(22, 135);
            labelControl60.Margin = new Padding(3, 4, 3, 4);
            labelControl60.Name = "labelControl60";
            labelControl60.Size = new Size(48, 14);
            labelControl60.TabIndex = 92;
            labelControl60.Text = "玛法战将";
            // 
            // textEdit19
            // 
            textEdit19.Location = new Point(77, 100);
            textEdit19.Margin = new Padding(4);
            textEdit19.MenuManager = ribbon;
            textEdit19.Name = "textEdit19";
            textEdit19.Properties.Appearance.Options.UseTextOptions = true;
            textEdit19.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit19.Properties.EditFormat.FormatType = FormatType.Numeric;
            textEdit19.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit19.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit19.Properties.MaskSettings.Set("mask", "d");
            textEdit19.Properties.MaxLength = 2;
            textEdit19.Size = new Size(107, 20);
            textEdit19.TabIndex = 91;
            // 
            // labelControl61
            // 
            labelControl61.Location = new Point(22, 103);
            labelControl61.Margin = new Padding(3, 4, 3, 4);
            labelControl61.Name = "labelControl61";
            labelControl61.Size = new Size(48, 14);
            labelControl61.TabIndex = 90;
            labelControl61.Text = "玛法豪杰";
            // 
            // textEdit20
            // 
            textEdit20.Location = new Point(77, 72);
            textEdit20.Margin = new Padding(4);
            textEdit20.MenuManager = ribbon;
            textEdit20.Name = "textEdit20";
            textEdit20.Properties.Appearance.Options.UseTextOptions = true;
            textEdit20.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit20.Properties.EditFormat.FormatType = FormatType.Numeric;
            textEdit20.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit20.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit20.Properties.MaskSettings.Set("mask", "d");
            textEdit20.Properties.MaxLength = 2;
            textEdit20.Size = new Size(107, 20);
            textEdit20.TabIndex = 89;
            // 
            // labelControl62
            // 
            labelControl62.Location = new Point(22, 75);
            labelControl62.Margin = new Padding(3, 4, 3, 4);
            labelControl62.Name = "labelControl62";
            labelControl62.Size = new Size(48, 14);
            labelControl62.TabIndex = 88;
            labelControl62.Text = "玛法名俊";
            // 
            // 安全区满血check
            // 
            安全区满血check.Location = new Point(5, 37);
            安全区满血check.MenuManager = ribbon;
            安全区满血check.Name = "安全区满血check";
            安全区满血check.Properties.Caption = "安全区内满血满蓝";
            安全区满血check.Size = new Size(125, 19);
            安全区满血check.TabIndex = 128;
            // 
            // xtraTabPage2
            // 
            xtraTabPage2.Controls.Add(checkEdit6);
            xtraTabPage2.Controls.Add(checkEdit5);
            xtraTabPage2.Controls.Add(textEdit7);
            xtraTabPage2.Controls.Add(labelControl47);
            xtraTabPage2.Controls.Add(radioGroup1);
            xtraTabPage2.Controls.Add(checkEdit4);
            xtraTabPage2.Controls.Add(checkEdit3);
            xtraTabPage2.Controls.Add(checkEdit1);
            xtraTabPage2.Controls.Add(AutoFight_New);
            xtraTabPage2.Controls.Add(checkEdit达最高级后继续加经验);
            xtraTabPage2.Controls.Add(checkEdit屏蔽日程);
            xtraTabPage2.Controls.Add(checkEdit屏蔽战功);
            xtraTabPage2.Controls.Add(checkEdit屏蔽威望);
            xtraTabPage2.Controls.Add(checkEdit屏蔽七天活动);
            xtraTabPage2.Controls.Add(textEdit神佑掉落ID);
            xtraTabPage2.Controls.Add(labelControl28);
            xtraTabPage2.Controls.Add(checkEdit普通强化不碎武器);
            xtraTabPage2.Controls.Add(checkEditBOSS自动死亡);
            xtraTabPage2.Controls.Add(textEdit暴击特效ID);
            xtraTabPage2.Controls.Add(labelControl26);
            xtraTabPage2.Controls.Add(checkEdit可购买玛法特权);
            xtraTabPage2.Controls.Add(checkEditResBag);
            xtraTabPage2.Controls.Add(ReBuildItemCheck);
            xtraTabPage2.Controls.Add(JobOpenCheck);
            xtraTabPage2.Controls.Add(PacketCheck);
            xtraTabPage2.Controls.Add(EnabledAchievementCheck);
            xtraTabPage2.Controls.Add(EnabledQuestCheck);
            xtraTabPage2.Controls.Add(AutoFightCheck);
            xtraTabPage2.Controls.Add(MobDropEdit);
            xtraTabPage2.Controls.Add(labelControl22);
            xtraTabPage2.Controls.Add(MobExpEdit);
            xtraTabPage2.Controls.Add(SRepairEdit);
            xtraTabPage2.Controls.Add(DebuffRateEdit);
            xtraTabPage2.Controls.Add(labelControl19);
            xtraTabPage2.Controls.Add(labelControl20);
            xtraTabPage2.Controls.Add(labelControl21);
            xtraTabPage2.Controls.Add(SlaveTimeEdit);
            xtraTabPage2.Controls.Add(FightSExpEdit);
            xtraTabPage2.Controls.Add(FightExpEdit);
            xtraTabPage2.Controls.Add(FightTime2Edit);
            xtraTabPage2.Controls.Add(FightTime1Edit);
            xtraTabPage2.Controls.Add(labelControl14);
            xtraTabPage2.Controls.Add(labelControl15);
            xtraTabPage2.Controls.Add(labelControl16);
            xtraTabPage2.Controls.Add(labelControl17);
            xtraTabPage2.Controls.Add(labelControl18);
            xtraTabPage2.Controls.Add(DebuffLevelEdit);
            xtraTabPage2.Controls.Add(ItemOwnerTimeEdit);
            xtraTabPage2.Controls.Add(ItemCleanTimeEdit);
            xtraTabPage2.Controls.Add(NewbieLevelEdit);
            xtraTabPage2.Controls.Add(HighLevelEdit);
            xtraTabPage2.Controls.Add(labelControl9);
            xtraTabPage2.Controls.Add(labelControl10);
            xtraTabPage2.Controls.Add(labelControl11);
            xtraTabPage2.Controls.Add(labelControl12);
            xtraTabPage2.Controls.Add(labelControl13);
            xtraTabPage2.Margin = new Padding(3, 4, 3, 4);
            xtraTabPage2.Name = "xtraTabPage2";
            xtraTabPage2.Size = new Size(1038, 468);
            xtraTabPage2.Text = "设置";
            xtraTabPage2.Paint += xtraTabPage2_Paint;
            // 
            // checkEdit6
            // 
            checkEdit6.Location = new Point(636, 358);
            checkEdit6.MenuManager = ribbon;
            checkEdit6.Name = "checkEdit6";
            checkEdit6.Properties.Caption = "限制重要封包间隔时间";
            checkEdit6.Size = new Size(148, 19);
            checkEdit6.TabIndex = 109;
            // 
            // checkEdit5
            // 
            checkEdit5.Location = new Point(636, 322);
            checkEdit5.MenuManager = ribbon;
            checkEdit5.Name = "checkEdit5";
            checkEdit5.Properties.Caption = "沙巴克掉装备";
            checkEdit5.Size = new Size(100, 19);
            checkEdit5.TabIndex = 108;
            // 
            // textEdit7
            // 
            textEdit7.EditValue = "40";
            textEdit7.Location = new Point(362, 341);
            textEdit7.Margin = new Padding(4);
            textEdit7.MenuManager = ribbon;
            textEdit7.Name = "textEdit7";
            textEdit7.Properties.Appearance.Options.UseTextOptions = true;
            textEdit7.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit7.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit7.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit7.Properties.MaskSettings.Set("mask", "d");
            textEdit7.Size = new Size(117, 20);
            textEdit7.TabIndex = 89;
            // 
            // labelControl47
            // 
            labelControl47.Location = new Point(283, 344);
            labelControl47.Margin = new Padding(3, 4, 3, 4);
            labelControl47.Name = "labelControl47";
            labelControl47.Size = new Size(72, 14);
            labelControl47.TabIndex = 88;
            labelControl47.Text = "行会最高人数";
            // 
            // radioGroup1
            // 
            radioGroup1.Enabled = false;
            radioGroup1.Location = new Point(38, 338);
            radioGroup1.MenuManager = ribbon;
            radioGroup1.Name = "radioGroup1";
            radioGroup1.Properties.Columns = 2;
            radioGroup1.Properties.Items.AddRange(new RadioGroupItem[] { new RadioGroupItem(null, "充值元宝"), new RadioGroupItem(null, "充值银币") });
            radioGroup1.Size = new Size(196, 28);
            radioGroup1.TabIndex = 87;
            // 
            // checkEdit4
            // 
            checkEdit4.Location = new Point(510, 394);
            checkEdit4.MenuManager = ribbon;
            checkEdit4.Name = "checkEdit4";
            checkEdit4.Properties.Caption = "屏蔽传永武技";
            checkEdit4.Size = new Size(100, 19);
            checkEdit4.TabIndex = 86;
            // 
            // checkEdit3
            // 
            checkEdit3.Location = new Point(510, 358);
            checkEdit3.MenuManager = ribbon;
            checkEdit3.Name = "checkEdit3";
            checkEdit3.Properties.Caption = "屏蔽每日签到";
            checkEdit3.Size = new Size(100, 19);
            checkEdit3.TabIndex = 85;
            // 
            // checkEdit1
            // 
            checkEdit1.Location = new Point(510, 322);
            checkEdit1.MenuManager = ribbon;
            checkEdit1.Name = "checkEdit1";
            checkEdit1.Properties.Caption = "屏蔽每周特惠";
            checkEdit1.Size = new Size(100, 19);
            checkEdit1.TabIndex = 84;
            // 
            // AutoFight_New
            // 
            AutoFight_New.Location = new Point(636, 286);
            AutoFight_New.MenuManager = ribbon;
            AutoFight_New.Name = "AutoFight_New";
            AutoFight_New.Properties.Caption = "使用新内挂";
            AutoFight_New.Size = new Size(100, 19);
            AutoFight_New.TabIndex = 83;
            // 
            // checkEdit达最高级后继续加经验
            // 
            checkEdit达最高级后继续加经验.Location = new Point(636, 250);
            checkEdit达最高级后继续加经验.MenuManager = ribbon;
            checkEdit达最高级后继续加经验.Name = "checkEdit达最高级后继续加经验";
            checkEdit达最高级后继续加经验.Properties.Caption = "达最高级后继续加经验";
            checkEdit达最高级后继续加经验.Size = new Size(138, 19);
            checkEdit达最高级后继续加经验.TabIndex = 82;
            // 
            // checkEdit屏蔽日程
            // 
            checkEdit屏蔽日程.Location = new Point(510, 286);
            checkEdit屏蔽日程.MenuManager = ribbon;
            checkEdit屏蔽日程.Name = "checkEdit屏蔽日程";
            checkEdit屏蔽日程.Properties.Caption = "屏蔽日程";
            checkEdit屏蔽日程.Size = new Size(100, 19);
            checkEdit屏蔽日程.TabIndex = 81;
            // 
            // checkEdit屏蔽战功
            // 
            checkEdit屏蔽战功.Location = new Point(636, 214);
            checkEdit屏蔽战功.MenuManager = ribbon;
            checkEdit屏蔽战功.Name = "checkEdit屏蔽战功";
            checkEdit屏蔽战功.Properties.Caption = "屏蔽战功";
            checkEdit屏蔽战功.Size = new Size(100, 19);
            checkEdit屏蔽战功.TabIndex = 80;
            // 
            // checkEdit屏蔽威望
            // 
            checkEdit屏蔽威望.Location = new Point(510, 250);
            checkEdit屏蔽威望.MenuManager = ribbon;
            checkEdit屏蔽威望.Name = "checkEdit屏蔽威望";
            checkEdit屏蔽威望.Properties.Caption = "屏蔽威望(玛法传说)";
            checkEdit屏蔽威望.Size = new Size(120, 19);
            checkEdit屏蔽威望.TabIndex = 79;
            // 
            // checkEdit屏蔽七天活动
            // 
            checkEdit屏蔽七天活动.Location = new Point(636, 178);
            checkEdit屏蔽七天活动.MenuManager = ribbon;
            checkEdit屏蔽七天活动.Name = "checkEdit屏蔽七天活动";
            checkEdit屏蔽七天活动.Properties.Caption = "屏蔽七天活动";
            checkEdit屏蔽七天活动.Size = new Size(121, 19);
            checkEdit屏蔽七天活动.TabIndex = 78;
            // 
            // textEdit神佑掉落ID
            // 
            textEdit神佑掉落ID.EditValue = "";
            textEdit神佑掉落ID.Location = new Point(362, 304);
            textEdit神佑掉落ID.Margin = new Padding(4);
            textEdit神佑掉落ID.MenuManager = ribbon;
            textEdit神佑掉落ID.Name = "textEdit神佑掉落ID";
            textEdit神佑掉落ID.Properties.Appearance.Options.UseTextOptions = true;
            textEdit神佑掉落ID.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit神佑掉落ID.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit神佑掉落ID.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit神佑掉落ID.Properties.MaskSettings.Set("mask", "d");
            textEdit神佑掉落ID.Size = new Size(117, 20);
            textEdit神佑掉落ID.TabIndex = 77;
            // 
            // labelControl28
            // 
            labelControl28.Location = new Point(283, 307);
            labelControl28.Margin = new Padding(3, 4, 3, 4);
            labelControl28.Name = "labelControl28";
            labelControl28.Size = new Size(60, 14);
            labelControl28.TabIndex = 76;
            labelControl28.Text = "神佑掉落ID";
            // 
            // checkEdit普通强化不碎武器
            // 
            checkEdit普通强化不碎武器.Location = new Point(636, 142);
            checkEdit普通强化不碎武器.MenuManager = ribbon;
            checkEdit普通强化不碎武器.Name = "checkEdit普通强化不碎武器";
            checkEdit普通强化不碎武器.Properties.Caption = "普通强化不碎武器";
            checkEdit普通强化不碎武器.Size = new Size(121, 19);
            checkEdit普通强化不碎武器.TabIndex = 75;
            // 
            // checkEditBOSS自动死亡
            // 
            checkEditBOSS自动死亡.Location = new Point(636, 106);
            checkEditBOSS自动死亡.MenuManager = ribbon;
            checkEditBOSS自动死亡.Name = "checkEditBOSS自动死亡";
            checkEditBOSS自动死亡.Properties.Caption = "BOSS自动死亡";
            checkEditBOSS自动死亡.Size = new Size(121, 19);
            checkEditBOSS自动死亡.TabIndex = 74;
            // 
            // textEdit暴击特效ID
            // 
            textEdit暴击特效ID.Location = new Point(117, 304);
            textEdit暴击特效ID.Margin = new Padding(4);
            textEdit暴击特效ID.MenuManager = ribbon;
            textEdit暴击特效ID.Name = "textEdit暴击特效ID";
            textEdit暴击特效ID.Properties.Appearance.Options.UseTextOptions = true;
            textEdit暴击特效ID.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit暴击特效ID.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit暴击特效ID.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit暴击特效ID.Properties.MaskSettings.Set("mask", "d");
            textEdit暴击特效ID.Size = new Size(117, 20);
            textEdit暴击特效ID.TabIndex = 73;
            // 
            // labelControl26
            // 
            labelControl26.Location = new Point(38, 307);
            labelControl26.Margin = new Padding(3, 4, 3, 4);
            labelControl26.Name = "labelControl26";
            labelControl26.Size = new Size(60, 14);
            labelControl26.TabIndex = 72;
            labelControl26.Text = "暴击特效ID";
            // 
            // checkEdit可购买玛法特权
            // 
            checkEdit可购买玛法特权.Location = new Point(636, 70);
            checkEdit可购买玛法特权.MenuManager = ribbon;
            checkEdit可购买玛法特权.Name = "checkEdit可购买玛法特权";
            checkEdit可购买玛法特权.Properties.Caption = "可购买玛法特权";
            checkEdit可购买玛法特权.Size = new Size(121, 19);
            checkEdit可购买玛法特权.TabIndex = 71;
            // 
            // checkEditResBag
            // 
            checkEditResBag.Location = new Point(636, 34);
            checkEditResBag.MenuManager = ribbon;
            checkEditResBag.Name = "checkEditResBag";
            checkEditResBag.Properties.Caption = "资源包只能放材料";
            checkEditResBag.Size = new Size(121, 19);
            checkEditResBag.TabIndex = 70;
            // 
            // ReBuildItemCheck
            // 
            ReBuildItemCheck.Location = new Point(510, 214);
            ReBuildItemCheck.MenuManager = ribbon;
            ReBuildItemCheck.Name = "ReBuildItemCheck";
            ReBuildItemCheck.Properties.Caption = "触发装备重铸";
            ReBuildItemCheck.Size = new Size(100, 19);
            ReBuildItemCheck.TabIndex = 69;
            // 
            // JobOpenCheck
            // 
            JobOpenCheck.Location = new Point(510, 178);
            JobOpenCheck.MenuManager = ribbon;
            JobOpenCheck.Name = "JobOpenCheck";
            JobOpenCheck.Properties.Caption = "开启六职业";
            JobOpenCheck.Size = new Size(100, 19);
            JobOpenCheck.TabIndex = 68;
            // 
            // PacketCheck
            // 
            PacketCheck.Location = new Point(510, 142);
            PacketCheck.MenuManager = ribbon;
            PacketCheck.Name = "PacketCheck";
            PacketCheck.Properties.Caption = "开启线程发包";
            PacketCheck.Size = new Size(100, 19);
            PacketCheck.TabIndex = 67;
            // 
            // EnabledAchievementCheck
            // 
            EnabledAchievementCheck.Location = new Point(510, 106);
            EnabledAchievementCheck.MenuManager = ribbon;
            EnabledAchievementCheck.Name = "EnabledAchievementCheck";
            EnabledAchievementCheck.Properties.Caption = "开启成就系统";
            EnabledAchievementCheck.Size = new Size(100, 19);
            EnabledAchievementCheck.TabIndex = 66;
            // 
            // EnabledQuestCheck
            // 
            EnabledQuestCheck.Location = new Point(510, 70);
            EnabledQuestCheck.MenuManager = ribbon;
            EnabledQuestCheck.Name = "EnabledQuestCheck";
            EnabledQuestCheck.Properties.Caption = "开启任务系统";
            EnabledQuestCheck.Size = new Size(100, 19);
            EnabledQuestCheck.TabIndex = 65;
            // 
            // AutoFightCheck
            // 
            AutoFightCheck.Location = new Point(510, 34);
            AutoFightCheck.MenuManager = ribbon;
            AutoFightCheck.Name = "AutoFightCheck";
            AutoFightCheck.Properties.Caption = "开启自动战斗";
            AutoFightCheck.Size = new Size(100, 19);
            AutoFightCheck.TabIndex = 64;
            // 
            // MobDropEdit
            // 
            MobDropEdit.Location = new Point(362, 228);
            MobDropEdit.Margin = new Padding(4);
            MobDropEdit.MenuManager = ribbon;
            MobDropEdit.Name = "MobDropEdit";
            MobDropEdit.Properties.Appearance.Options.UseTextOptions = true;
            MobDropEdit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            MobDropEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            MobDropEdit.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            MobDropEdit.Properties.MaskSettings.Set("mask", "n3");
            MobDropEdit.Size = new Size(117, 20);
            MobDropEdit.TabIndex = 63;
            // 
            // labelControl22
            // 
            labelControl22.Location = new Point(283, 231);
            labelControl22.Margin = new Padding(3, 4, 3, 4);
            labelControl22.Name = "labelControl22";
            labelControl22.Size = new Size(72, 14);
            labelControl22.TabIndex = 62;
            labelControl22.Text = "怪物额外爆率";
            // 
            // MobExpEdit
            // 
            MobExpEdit.Location = new Point(362, 267);
            MobExpEdit.Margin = new Padding(4);
            MobExpEdit.MenuManager = ribbon;
            MobExpEdit.Name = "MobExpEdit";
            MobExpEdit.Properties.Appearance.Options.UseTextOptions = true;
            MobExpEdit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            MobExpEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            MobExpEdit.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            MobExpEdit.Properties.MaskSettings.Set("mask", "n3");
            MobExpEdit.Size = new Size(117, 20);
            MobExpEdit.TabIndex = 61;
            // 
            // SRepairEdit
            // 
            SRepairEdit.Location = new Point(117, 267);
            SRepairEdit.Margin = new Padding(4);
            SRepairEdit.MenuManager = ribbon;
            SRepairEdit.Name = "SRepairEdit";
            SRepairEdit.Properties.Appearance.Options.UseTextOptions = true;
            SRepairEdit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            SRepairEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            SRepairEdit.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            SRepairEdit.Properties.MaskSettings.Set("mask", "n3");
            SRepairEdit.Size = new Size(117, 20);
            SRepairEdit.TabIndex = 60;
            // 
            // DebuffRateEdit
            // 
            DebuffRateEdit.Location = new Point(117, 228);
            DebuffRateEdit.Margin = new Padding(4);
            DebuffRateEdit.MenuManager = ribbon;
            DebuffRateEdit.Name = "DebuffRateEdit";
            DebuffRateEdit.Properties.Appearance.Options.UseTextOptions = true;
            DebuffRateEdit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            DebuffRateEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            DebuffRateEdit.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            DebuffRateEdit.Properties.MaskSettings.Set("mask", "n3");
            DebuffRateEdit.Size = new Size(117, 20);
            DebuffRateEdit.TabIndex = 59;
            // 
            // labelControl19
            // 
            labelControl19.Location = new Point(283, 270);
            labelControl19.Margin = new Padding(3, 4, 3, 4);
            labelControl19.Name = "labelControl19";
            labelControl19.Size = new Size(72, 14);
            labelControl19.TabIndex = 58;
            labelControl19.Text = "怪物经验倍率";
            // 
            // labelControl20
            // 
            labelControl20.Location = new Point(38, 270);
            labelControl20.Margin = new Padding(3, 4, 3, 4);
            labelControl20.Name = "labelControl20";
            labelControl20.Size = new Size(72, 14);
            labelControl20.TabIndex = 57;
            labelControl20.Text = "装备特修折扣";
            // 
            // labelControl21
            // 
            labelControl21.Location = new Point(38, 231);
            labelControl21.Margin = new Padding(3, 4, 3, 4);
            labelControl21.Name = "labelControl21";
            labelControl21.Size = new Size(72, 14);
            labelControl21.TabIndex = 56;
            labelControl21.Text = "收益减少比率";
            // 
            // SlaveTimeEdit
            // 
            SlaveTimeEdit.Location = new Point(362, 189);
            SlaveTimeEdit.Margin = new Padding(4);
            SlaveTimeEdit.MenuManager = ribbon;
            SlaveTimeEdit.Name = "SlaveTimeEdit";
            SlaveTimeEdit.Properties.Appearance.Options.UseTextOptions = true;
            SlaveTimeEdit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            SlaveTimeEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            SlaveTimeEdit.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            SlaveTimeEdit.Properties.MaskSettings.Set("mask", "n0");
            SlaveTimeEdit.Size = new Size(117, 20);
            SlaveTimeEdit.TabIndex = 55;
            // 
            // FightSExpEdit
            // 
            FightSExpEdit.Location = new Point(362, 150);
            FightSExpEdit.Margin = new Padding(4);
            FightSExpEdit.MenuManager = ribbon;
            FightSExpEdit.Name = "FightSExpEdit";
            FightSExpEdit.Properties.Appearance.Options.UseTextOptions = true;
            FightSExpEdit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            FightSExpEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            FightSExpEdit.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            FightSExpEdit.Properties.MaskSettings.Set("mask", "n0");
            FightSExpEdit.Size = new Size(117, 20);
            FightSExpEdit.TabIndex = 54;
            // 
            // FightExpEdit
            // 
            FightExpEdit.Location = new Point(362, 111);
            FightExpEdit.Margin = new Padding(4);
            FightExpEdit.MenuManager = ribbon;
            FightExpEdit.Name = "FightExpEdit";
            FightExpEdit.Properties.Appearance.Options.UseTextOptions = true;
            FightExpEdit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            FightExpEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            FightExpEdit.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            FightExpEdit.Properties.MaskSettings.Set("mask", "n0");
            FightExpEdit.Size = new Size(117, 20);
            FightExpEdit.TabIndex = 53;
            // 
            // FightTime2Edit
            // 
            FightTime2Edit.Location = new Point(362, 72);
            FightTime2Edit.Margin = new Padding(4);
            FightTime2Edit.MenuManager = ribbon;
            FightTime2Edit.Name = "FightTime2Edit";
            FightTime2Edit.Properties.Appearance.Options.UseTextOptions = true;
            FightTime2Edit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            FightTime2Edit.Properties.Mask.UseMaskAsDisplayFormat = true;
            FightTime2Edit.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            FightTime2Edit.Properties.MaskSettings.Set("mask", "n0");
            FightTime2Edit.Size = new Size(117, 20);
            FightTime2Edit.TabIndex = 52;
            // 
            // FightTime1Edit
            // 
            FightTime1Edit.Location = new Point(362, 33);
            FightTime1Edit.Margin = new Padding(4);
            FightTime1Edit.MenuManager = ribbon;
            FightTime1Edit.Name = "FightTime1Edit";
            FightTime1Edit.Properties.Appearance.Options.UseTextOptions = true;
            FightTime1Edit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            FightTime1Edit.Properties.Mask.UseMaskAsDisplayFormat = true;
            FightTime1Edit.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            FightTime1Edit.Properties.MaskSettings.Set("mask", "n0");
            FightTime1Edit.Size = new Size(117, 20);
            FightTime1Edit.TabIndex = 51;
            // 
            // labelControl14
            // 
            labelControl14.Location = new Point(283, 192);
            labelControl14.Margin = new Padding(3, 4, 3, 4);
            labelControl14.Name = "labelControl14";
            labelControl14.Size = new Size(72, 14);
            labelControl14.TabIndex = 50;
            labelControl14.Text = "怪物诱惑时长";
            // 
            // labelControl15
            // 
            labelControl15.Location = new Point(283, 153);
            labelControl15.Margin = new Padding(3, 4, 3, 4);
            labelControl15.Name = "labelControl15";
            labelControl15.Size = new Size(72, 14);
            labelControl15.TabIndex = 49;
            labelControl15.Text = "武斗抢点经验";
            // 
            // labelControl16
            // 
            labelControl16.Location = new Point(283, 114);
            labelControl16.Margin = new Padding(3, 4, 3, 4);
            labelControl16.Name = "labelControl16";
            labelControl16.Size = new Size(72, 14);
            labelControl16.TabIndex = 48;
            labelControl16.Text = "武斗普通经验";
            // 
            // labelControl17
            // 
            labelControl17.Location = new Point(283, 75);
            labelControl17.Margin = new Padding(3, 4, 3, 4);
            labelControl17.Name = "labelControl17";
            labelControl17.Size = new Size(72, 14);
            labelControl17.TabIndex = 47;
            labelControl17.Text = "武斗场时间二";
            // 
            // labelControl18
            // 
            labelControl18.Location = new Point(283, 36);
            labelControl18.Margin = new Padding(3, 4, 3, 4);
            labelControl18.Name = "labelControl18";
            labelControl18.Size = new Size(72, 14);
            labelControl18.TabIndex = 46;
            labelControl18.Text = "武斗场时间一";
            // 
            // DebuffLevelEdit
            // 
            DebuffLevelEdit.Location = new Point(117, 189);
            DebuffLevelEdit.Margin = new Padding(4);
            DebuffLevelEdit.MenuManager = ribbon;
            DebuffLevelEdit.Name = "DebuffLevelEdit";
            DebuffLevelEdit.Properties.Appearance.Options.UseTextOptions = true;
            DebuffLevelEdit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            DebuffLevelEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            DebuffLevelEdit.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            DebuffLevelEdit.Properties.MaskSettings.Set("mask", "n0");
            DebuffLevelEdit.Size = new Size(117, 20);
            DebuffLevelEdit.TabIndex = 45;
            // 
            // ItemOwnerTimeEdit
            // 
            ItemOwnerTimeEdit.Location = new Point(117, 150);
            ItemOwnerTimeEdit.Margin = new Padding(4);
            ItemOwnerTimeEdit.MenuManager = ribbon;
            ItemOwnerTimeEdit.Name = "ItemOwnerTimeEdit";
            ItemOwnerTimeEdit.Properties.Appearance.Options.UseTextOptions = true;
            ItemOwnerTimeEdit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            ItemOwnerTimeEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            ItemOwnerTimeEdit.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            ItemOwnerTimeEdit.Properties.MaskSettings.Set("mask", "n0");
            ItemOwnerTimeEdit.Size = new Size(117, 20);
            ItemOwnerTimeEdit.TabIndex = 44;
            // 
            // ItemCleanTimeEdit
            // 
            ItemCleanTimeEdit.Location = new Point(117, 111);
            ItemCleanTimeEdit.Margin = new Padding(4);
            ItemCleanTimeEdit.MenuManager = ribbon;
            ItemCleanTimeEdit.Name = "ItemCleanTimeEdit";
            ItemCleanTimeEdit.Properties.Appearance.Options.UseTextOptions = true;
            ItemCleanTimeEdit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            ItemCleanTimeEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            ItemCleanTimeEdit.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            ItemCleanTimeEdit.Properties.MaskSettings.Set("mask", "n0");
            ItemCleanTimeEdit.Size = new Size(117, 20);
            ItemCleanTimeEdit.TabIndex = 43;
            // 
            // NewbieLevelEdit
            // 
            NewbieLevelEdit.Location = new Point(117, 72);
            NewbieLevelEdit.Margin = new Padding(4);
            NewbieLevelEdit.MenuManager = ribbon;
            NewbieLevelEdit.Name = "NewbieLevelEdit";
            NewbieLevelEdit.Properties.Appearance.Options.UseTextOptions = true;
            NewbieLevelEdit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            NewbieLevelEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            NewbieLevelEdit.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            NewbieLevelEdit.Properties.MaskSettings.Set("mask", "n0");
            NewbieLevelEdit.Size = new Size(117, 20);
            NewbieLevelEdit.TabIndex = 42;
            // 
            // HighLevelEdit
            // 
            HighLevelEdit.Location = new Point(117, 33);
            HighLevelEdit.Margin = new Padding(4);
            HighLevelEdit.MenuManager = ribbon;
            HighLevelEdit.Name = "HighLevelEdit";
            HighLevelEdit.Properties.Appearance.Options.UseTextOptions = true;
            HighLevelEdit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            HighLevelEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            HighLevelEdit.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            HighLevelEdit.Properties.MaskSettings.Set("mask", "n0");
            HighLevelEdit.Size = new Size(117, 20);
            HighLevelEdit.TabIndex = 41;
            // 
            // labelControl9
            // 
            labelControl9.Location = new Point(38, 192);
            labelControl9.Margin = new Padding(3, 4, 3, 4);
            labelControl9.Name = "labelControl9";
            labelControl9.Size = new Size(72, 14);
            labelControl9.TabIndex = 40;
            labelControl9.Text = "减收益等级差";
            // 
            // labelControl10
            // 
            labelControl10.Location = new Point(38, 153);
            labelControl10.Margin = new Padding(3, 4, 3, 4);
            labelControl10.Name = "labelControl10";
            labelControl10.Size = new Size(72, 14);
            labelControl10.TabIndex = 39;
            labelControl10.Text = "物品归属时间";
            // 
            // labelControl11
            // 
            labelControl11.Location = new Point(38, 114);
            labelControl11.Margin = new Padding(3, 4, 3, 4);
            labelControl11.Name = "labelControl11";
            labelControl11.Size = new Size(72, 14);
            labelControl11.TabIndex = 38;
            labelControl11.Text = "物品清理时间";
            // 
            // labelControl12
            // 
            labelControl12.Location = new Point(38, 75);
            labelControl12.Margin = new Padding(3, 4, 3, 4);
            labelControl12.Name = "labelControl12";
            labelControl12.Size = new Size(72, 14);
            labelControl12.TabIndex = 37;
            labelControl12.Text = "新手扶持等级";
            // 
            // labelControl13
            // 
            labelControl13.Location = new Point(38, 36);
            labelControl13.Margin = new Padding(3, 4, 3, 4);
            labelControl13.Name = "labelControl13";
            labelControl13.Size = new Size(72, 14);
            labelControl13.TabIndex = 36;
            labelControl13.Text = "游戏开放等级";
            // 
            // xtraTabPage1
            // 
            xtraTabPage1.Controls.Add(groupControl6);
            xtraTabPage1.Controls.Add(groupControl5);
            xtraTabPage1.Controls.Add(groupControl4);
            xtraTabPage1.Controls.Add(groupControl3);
            xtraTabPage1.Margin = new Padding(3, 4, 3, 4);
            xtraTabPage1.Name = "xtraTabPage1";
            xtraTabPage1.Size = new Size(1038, 468);
            xtraTabPage1.Text = "系统";
            //
            // groupControl5
            // 
            groupControl5.Controls.Add(labelControl6);
            groupControl5.Controls.Add(labelControl7);
            groupControl5.Controls.Add(ServerPathEdit);
            groupControl5.Controls.Add(SavePathEdit);
            groupControl5.Location = new Point(11, 161);
            groupControl5.Name = "groupControl5";
            groupControl5.Size = new Size(587, 100);
            groupControl5.TabIndex = 59;
            groupControl5.Text = "目录数据";
            // 
            // labelControl6
            // 
            labelControl6.Location = new Point(18, 39);
            labelControl6.Margin = new Padding(3, 4, 3, 4);
            labelControl6.Name = "labelControl6";
            labelControl6.Size = new Size(52, 14);
            labelControl6.TabIndex = 5;
            labelControl6.Text = "数据目录:";
            // 
            // labelControl7
            // 
            labelControl7.Location = new Point(19, 67);
            labelControl7.Margin = new Padding(3, 4, 3, 4);
            labelControl7.Name = "labelControl7";
            labelControl7.Size = new Size(52, 14);
            labelControl7.TabIndex = 6;
            labelControl7.Text = "备份目录:";
            // 
            // ServerPathEdit
            // 
            ServerPathEdit.Location = new Point(77, 36);
            ServerPathEdit.Margin = new Padding(4);
            ServerPathEdit.MenuManager = ribbon;
            ServerPathEdit.Name = "ServerPathEdit";
            ServerPathEdit.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton() });
            ServerPathEdit.Size = new Size(477, 20);
            ServerPathEdit.TabIndex = 39;
            ServerPathEdit.ButtonClick += VersionPathEdit_ButtonClick;
            // 
            // SavePathEdit
            // 
            SavePathEdit.Location = new Point(77, 64);
            SavePathEdit.Margin = new Padding(4);
            SavePathEdit.MenuManager = ribbon;
            SavePathEdit.Name = "SavePathEdit";
            SavePathEdit.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton() });
            SavePathEdit.Size = new Size(477, 20);
            SavePathEdit.TabIndex = 40;
            SavePathEdit.ButtonClick += buttonEdit1_ButtonClick;
            // 
            // groupControl4
            // 
            groupControl4.Controls.Add(labelControl8);
            groupControl4.Controls.Add(labelControl4);
            groupControl4.Controls.Add(labelControl5);
            groupControl4.Controls.Add(textEdit1);
            groupControl4.Controls.Add(ExceptionBanEdit);
            groupControl4.Controls.Add(labelControl29);
            groupControl4.Controls.Add(DisconnectTimeEdit);
            groupControl4.Controls.Add(ServerNameEdit);
            groupControl4.Controls.Add(AutoSafeTimeEdit);
            groupControl4.Controls.Add(labelControl24);
            groupControl4.Controls.Add(StartDateEdit);
            groupControl4.Controls.Add(labelControl23);
            groupControl4.Location = new Point(11, 15);
            groupControl4.Name = "groupControl4";
            groupControl4.Size = new Size(361, 140);
            groupControl4.TabIndex = 58;
            groupControl4.Text = "参数配置";
            // 
            // labelControl8
            // 
            labelControl8.Location = new Point(18, 109);
            labelControl8.Margin = new Padding(3, 4, 3, 4);
            labelControl8.Name = "labelControl8";
            labelControl8.Size = new Size(52, 14);
            labelControl8.TabIndex = 7;
            labelControl8.Text = "自动保存:";
            // 
            // labelControl4
            // 
            labelControl4.Location = new Point(189, 109);
            labelControl4.Margin = new Padding(3, 4, 3, 4);
            labelControl4.Name = "labelControl4";
            labelControl4.Size = new Size(52, 14);
            labelControl4.TabIndex = 3;
            labelControl4.Text = "异常屏蔽:";
            // 
            // labelControl5
            // 
            labelControl5.Location = new Point(189, 79);
            labelControl5.Margin = new Padding(3, 4, 3, 4);
            labelControl5.Name = "labelControl5";
            labelControl5.Size = new Size(52, 14);
            labelControl5.TabIndex = 4;
            labelControl5.Text = "超时时间:";
            // 
            // textEdit1
            // 
            textEdit1.EditValue = "110";
            textEdit1.Location = new Point(248, 44);
            textEdit1.Margin = new Padding(4);
            textEdit1.MenuManager = ribbon;
            textEdit1.Name = "textEdit1";
            textEdit1.Properties.Appearance.Options.UseTextOptions = true;
            textEdit1.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            textEdit1.Properties.Mask.UseMaskAsDisplayFormat = true;
            textEdit1.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            textEdit1.Properties.MaskSettings.Set("mask", "n0");
            textEdit1.Size = new Size(95, 20);
            textEdit1.TabIndex = 50;
            // 
            // ExceptionBanEdit
            // 
            ExceptionBanEdit.Location = new Point(248, 106);
            ExceptionBanEdit.Margin = new Padding(4);
            ExceptionBanEdit.MenuManager = ribbon;
            ExceptionBanEdit.Name = "ExceptionBanEdit";
            ExceptionBanEdit.Properties.Appearance.Options.UseTextOptions = true;
            ExceptionBanEdit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            ExceptionBanEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            ExceptionBanEdit.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            ExceptionBanEdit.Properties.MaskSettings.Set("mask", "n0");
            ExceptionBanEdit.Size = new Size(95, 20);
            ExceptionBanEdit.TabIndex = 34;
            // 
            // labelControl29
            // 
            labelControl29.Location = new Point(189, 47);
            labelControl29.Margin = new Padding(3, 4, 3, 4);
            labelControl29.Name = "labelControl29";
            labelControl29.Size = new Size(52, 14);
            labelControl29.TabIndex = 49;
            labelControl29.Text = "商人比例:";
            // 
            // DisconnectTimeEdit
            // 
            DisconnectTimeEdit.Location = new Point(248, 76);
            DisconnectTimeEdit.Margin = new Padding(4);
            DisconnectTimeEdit.MenuManager = ribbon;
            DisconnectTimeEdit.Name = "DisconnectTimeEdit";
            DisconnectTimeEdit.Properties.Appearance.Options.UseTextOptions = true;
            DisconnectTimeEdit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            DisconnectTimeEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            DisconnectTimeEdit.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            DisconnectTimeEdit.Properties.MaskSettings.Set("mask", "n0");
            DisconnectTimeEdit.Size = new Size(95, 20);
            DisconnectTimeEdit.TabIndex = 35;
            DisconnectTimeEdit.EditValueChanged += DisconnectTimeEdit_EditValueChanged;
            // 
            // ServerNameEdit
            // 
            ServerNameEdit.Location = new Point(76, 44);
            ServerNameEdit.Margin = new Padding(4);
            ServerNameEdit.MenuManager = ribbon;
            ServerNameEdit.Name = "ServerNameEdit";
            ServerNameEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            ServerNameEdit.Size = new Size(100, 20);
            ServerNameEdit.TabIndex = 44;
            // 
            // AutoSafeTimeEdit
            // 
            AutoSafeTimeEdit.Location = new Point(76, 106);
            AutoSafeTimeEdit.Margin = new Padding(4);
            AutoSafeTimeEdit.MenuManager = ribbon;
            AutoSafeTimeEdit.Name = "AutoSafeTimeEdit";
            AutoSafeTimeEdit.Properties.Appearance.Options.UseTextOptions = true;
            AutoSafeTimeEdit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            AutoSafeTimeEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            AutoSafeTimeEdit.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            AutoSafeTimeEdit.Properties.MaskSettings.Set("mask", "n0");
            AutoSafeTimeEdit.Size = new Size(100, 20);
            AutoSafeTimeEdit.TabIndex = 38;
            // 
            // labelControl24
            // 
            labelControl24.Location = new Point(6, 47);
            labelControl24.Margin = new Padding(3, 4, 3, 4);
            labelControl24.Name = "labelControl24";
            labelControl24.Size = new Size(64, 14);
            labelControl24.TabIndex = 43;
            labelControl24.Text = "服务器名称:";
            // 
            // StartDateEdit
            // 
            StartDateEdit.EditValue = null;
            StartDateEdit.Location = new Point(76, 76);
            StartDateEdit.MenuManager = ribbon;
            StartDateEdit.Name = "StartDateEdit";
            StartDateEdit.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            StartDateEdit.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            StartDateEdit.Size = new Size(100, 20);
            StartDateEdit.TabIndex = 41;
            // 
            // labelControl23
            // 
            labelControl23.Location = new Point(18, 79);
            labelControl23.Margin = new Padding(3, 4, 3, 4);
            labelControl23.Name = "labelControl23";
            labelControl23.Size = new Size(52, 14);
            labelControl23.TabIndex = 42;
            labelControl23.Text = "开服日期:";
            // 
            // groupControl3
            // 
            groupControl3.Controls.Add(labelControl1);
            groupControl3.Controls.Add(labelControl2);
            groupControl3.Controls.Add(labelControl3);
            groupControl3.Controls.Add(PortEdit);
            groupControl3.Controls.Add(TokenPortEdit);
            groupControl3.Controls.Add(PacketLimitEdit);
            groupControl3.Location = new Point(378, 15);
            groupControl3.Name = "groupControl3";
            groupControl3.Size = new Size(220, 140);
            groupControl3.TabIndex = 57;
            groupControl3.Text = "网络";
            // 
            // labelControl1
            // 
            labelControl1.Location = new Point(7, 47);
            labelControl1.Margin = new Padding(3, 4, 3, 4);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new Size(76, 14);
            labelControl1.TabIndex = 0;
            labelControl1.Text = "客户连接端口:";
            // 
            // labelControl2
            // 
            labelControl2.Location = new Point(7, 79);
            labelControl2.Margin = new Padding(3, 4, 3, 4);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new Size(76, 14);
            labelControl2.TabIndex = 1;
            labelControl2.Text = "门票接收端口:";
            // 
            // labelControl3
            // 
            labelControl3.Location = new Point(7, 109);
            labelControl3.Margin = new Padding(3, 4, 3, 4);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new Size(76, 14);
            labelControl3.TabIndex = 2;
            labelControl3.Text = "封包限定数量:";
            // 
            // PortEdit
            // 
            PortEdit.Location = new Point(93, 44);
            PortEdit.Margin = new Padding(4);
            PortEdit.MenuManager = ribbon;
            PortEdit.Name = "PortEdit";
            PortEdit.Properties.Appearance.Options.UseTextOptions = true;
            PortEdit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            PortEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            PortEdit.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            PortEdit.Properties.MaskSettings.Set("mask", "n0");
            PortEdit.Size = new Size(117, 20);
            PortEdit.TabIndex = 31;
            // 
            // TokenPortEdit
            // 
            TokenPortEdit.Location = new Point(93, 76);
            TokenPortEdit.Margin = new Padding(4);
            TokenPortEdit.MenuManager = ribbon;
            TokenPortEdit.Name = "TokenPortEdit";
            TokenPortEdit.Properties.Appearance.Options.UseTextOptions = true;
            TokenPortEdit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            TokenPortEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            TokenPortEdit.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            TokenPortEdit.Properties.MaskSettings.Set("MaskManagerSignature", "allowNull=False");
            TokenPortEdit.Properties.MaskSettings.Set("mask", "n0");
            TokenPortEdit.Size = new Size(117, 20);
            TokenPortEdit.TabIndex = 32;
            // 
            // PacketLimitEdit
            // 
            PacketLimitEdit.Location = new Point(93, 106);
            PacketLimitEdit.Margin = new Padding(4);
            PacketLimitEdit.MenuManager = ribbon;
            PacketLimitEdit.Name = "PacketLimitEdit";
            PacketLimitEdit.Properties.Appearance.Options.UseTextOptions = true;
            PacketLimitEdit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            PacketLimitEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
            PacketLimitEdit.Properties.MaskSettings.Set("MaskManagerType", typeof(NumericMaskManager));
            PacketLimitEdit.Properties.MaskSettings.Set("mask", "n0");
            PacketLimitEdit.Size = new Size(117, 20);
            PacketLimitEdit.TabIndex = 33;
            // 
            // xtraTabControl1
            // 
            xtraTabControl1.Dock = DockStyle.Fill;
            xtraTabControl1.Location = new Point(0, 148);
            xtraTabControl1.Margin = new Padding(3, 4, 3, 4);
            xtraTabControl1.Name = "xtraTabControl1";
            xtraTabControl1.SelectedTabPage = xtraTabPage1;
            xtraTabControl1.Size = new Size(1044, 497);
            xtraTabControl1.TabIndex = 1;
            xtraTabControl1.TabPages.AddRange(new XtraTabPage[] { xtraTabPage1, xtraTabPage2, xtraTabPage3, xtraTabPage4 });
            // 
            // xtraTabPage4
            // 
            xtraTabPage4.Controls.Add(宠物下属分组);
            xtraTabPage4.Controls.Add(安全区分组);
            xtraTabPage4.Name = "xtraTabPage4";
            xtraTabPage4.Size = new Size(1038, 468);
            xtraTabPage4.Text = "设置3";
            // 
            // 宠物下属分组
            // 
            宠物下属分组.Controls.Add(下线宝宝不死check);
            宠物下属分组.Location = new Point(197, 17);
            宠物下属分组.Name = "宠物下属分组";
            宠物下属分组.Size = new Size(180, 77);
            宠物下属分组.TabIndex = 130;
            宠物下属分组.Text = "宠物下属";
            // 
            // 下线宝宝不死check
            // 
            下线宝宝不死check.Location = new Point(5, 37);
            下线宝宝不死check.MenuManager = ribbon;
            下线宝宝不死check.Name = "下线宝宝不死check";
            下线宝宝不死check.Properties.Caption = "下线宝宝不死";
            下线宝宝不死check.Size = new Size(100, 19);
            下线宝宝不死check.TabIndex = 129;
            // 
            // 安全区分组
            // 
            安全区分组.Controls.Add(安全区满血check);
            安全区分组.Location = new Point(11, 17);
            安全区分组.Name = "安全区分组";
            安全区分组.Size = new Size(180, 77);
            安全区分组.TabIndex = 129;
            安全区分组.Text = "安全区";
            // 
            // groupControl6
            // 
            groupControl6.Controls.Add(labelControl63);
            groupControl6.Controls.Add(labelControl58);
            groupControl6.Controls.Add(richEditControl2);
            groupControl6.Controls.Add(richEditControl1);
            groupControl6.Location = new Point(11, 267);
            groupControl6.Name = "groupControl6";
            groupControl6.Size = new Size(587, 185);
            groupControl6.TabIndex = 61;
            groupControl6.Text = "玩家限制登录";
            // 
            // labelControl63
            // 
            labelControl63.Location = new Point(330, 36);
            labelControl63.Name = "labelControl63";
            labelControl63.Size = new Size(153, 14);
            labelControl63.TabIndex = 58;
            labelControl63.Text = "IP白名单(不受最大人数限制)";
            // 
            // labelControl58
            // 
            labelControl58.Location = new Point(43, 36);
            labelControl58.Name = "labelControl58";
            labelControl58.Size = new Size(166, 14);
            labelControl58.TabIndex = 57;
            labelControl58.Text = "账号白名单(不受最大人数限制)";
            // 
            // richEditControl2
            // 
            richEditControl2.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
            richEditControl2.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
            richEditControl2.Location = new Point(288, 56);
            richEditControl2.MenuManager = ribbon;
            richEditControl2.Name = "richEditControl2";
            richEditControl2.Options.DocumentSaveOptions.CurrentFormat = DevExpress.XtraRichEdit.DocumentFormat.PlainText;
            richEditControl2.Size = new Size(266, 124);
            richEditControl2.TabIndex = 56;
            // 
            // richEditControl1
            // 
            richEditControl1.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
            richEditControl1.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
            richEditControl1.Location = new Point(19, 56);
            richEditControl1.MenuManager = ribbon;
            richEditControl1.Name = "richEditControl1";
            richEditControl1.Options.DocumentSaveOptions.CurrentFormat = DevExpress.XtraRichEdit.DocumentFormat.PlainText;
            richEditControl1.Size = new Size(245, 124);
            richEditControl1.TabIndex = 55;
            // 
            // radioGroup2
            // 
            radioGroup2.Columns = 2;
            radioGroup2.Items.AddRange(new RadioGroupItem[] { new RadioGroupItem(null, "充值元宝"), new RadioGroupItem(null, "充值银币") });
            radioGroup2.Name = "radioGroup2";
            //
            // ConfigInfoView
            // 
            Appearance.Options.UseFont = true;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1044, 645);
            Controls.Add(xtraTabControl1);
            Controls.Add(ribbon);
            Font = new Font("Microsoft YaHei UI", 9F);
            Margin = new Padding(4);
            Name = "ConfigInfoView";
            Ribbon = ribbon;
            Text = "全局设置";
            ((ISupportInitialize)ribbon).EndInit();
            xtraTabPage3.ResumeLayout(false);
            ((ISupportInitialize)其他分组).EndInit();
            其他分组.ResumeLayout(false);
            其他分组.PerformLayout();
            ((ISupportInitialize)textEdit15.Properties).EndInit();
            ((ISupportInitialize)textEdit3.Properties).EndInit();
            ((ISupportInitialize)textEdit5.Properties).EndInit();
            ((ISupportInitialize)textEdit6.Properties).EndInit();
            ((ISupportInitialize)textEdit13.Properties).EndInit();
            ((ISupportInitialize)死亡掉落分组).EndInit();
            死亡掉落分组.ResumeLayout(false);
            死亡掉落分组.PerformLayout();
            ((ISupportInitialize)textEdit_Dropweap.Properties).EndInit();
            ((ISupportInitialize)textEdit_dropshoushi.Properties).EndInit();
            ((ISupportInitialize)textEdit_dropbags.Properties).EndInit();
            ((ISupportInitialize)textEdit4.Properties).EndInit();
            ((ISupportInitialize)textEdit_Dropweap_red.Properties).EndInit();
            ((ISupportInitialize)textEdit_dropshoushi_red.Properties).EndInit();
            ((ISupportInitialize)祝福油几率分组).EndInit();
            祝福油几率分组.ResumeLayout(false);
            祝福油几率分组.PerformLayout();
            ((ISupportInitialize)textEdit_zhufuyou_Lv1.Properties).EndInit();
            ((ISupportInitialize)textEdit_zhufuyou_Lv2.Properties).EndInit();
            ((ISupportInitialize)textEdit_zhufuyou_Lv3.Properties).EndInit();
            ((ISupportInitialize)textEdit_zhufuyou_Lv4.Properties).EndInit();
            ((ISupportInitialize)textEdit_zhufuyou_Lv5.Properties).EndInit();
            ((ISupportInitialize)textEdit_zhufuyou_Lv6.Properties).EndInit();
            ((ISupportInitialize)textEdit_zhufuyou_Lv0.Properties).EndInit();
            ((ISupportInitialize)游戏地图开放分组).EndInit();
            游戏地图开放分组.ResumeLayout(false);
            游戏地图开放分组.PerformLayout();
            ((ISupportInitialize)textEdit9.Properties).EndInit();
            ((ISupportInitialize)textEdit10.Properties).EndInit();
            ((ISupportInitialize)textEdit11.Properties).EndInit();
            ((ISupportInitialize)textEdit8.Properties).EndInit();
            ((ISupportInitialize)textEdit12.Properties).EndInit();
            ((ISupportInitialize)自动分解分组).EndInit();
            自动分解分组.ResumeLayout(false);
            ((ISupportInitialize)分解装备check.Properties).EndInit();
            ((ISupportInitialize)不分解极品check.Properties).EndInit();
            ((ISupportInitialize)自动拾取分组).EndInit();
            自动拾取分组.ResumeLayout(false);
            ((ISupportInitialize)金币入包check.Properties).EndInit();
            ((ISupportInitialize)银币入包check.Properties).EndInit();
            ((ISupportInitialize)物品入包check.Properties).EndInit();
            ((ISupportInitialize)groupControl1).EndInit();
            groupControl1.ResumeLayout(false);
            groupControl1.PerformLayout();
            ((ISupportInitialize)textEdit14.Properties).EndInit();
            ((ISupportInitialize)textEdit17.Properties).EndInit();
            ((ISupportInitialize)textEdit18.Properties).EndInit();
            ((ISupportInitialize)textEdit19.Properties).EndInit();
            ((ISupportInitialize)textEdit20.Properties).EndInit();
            ((ISupportInitialize)安全区满血check.Properties).EndInit();
            xtraTabPage2.ResumeLayout(false);
            xtraTabPage2.PerformLayout();
            ((ISupportInitialize)checkEdit6.Properties).EndInit();
            ((ISupportInitialize)checkEdit5.Properties).EndInit();
            ((ISupportInitialize)textEdit7.Properties).EndInit();
            ((ISupportInitialize)radioGroup1.Properties).EndInit();
            ((ISupportInitialize)checkEdit4.Properties).EndInit();
            ((ISupportInitialize)checkEdit3.Properties).EndInit();
            ((ISupportInitialize)checkEdit1.Properties).EndInit();
            ((ISupportInitialize)AutoFight_New.Properties).EndInit();
            ((ISupportInitialize)checkEdit达最高级后继续加经验.Properties).EndInit();
            ((ISupportInitialize)checkEdit屏蔽日程.Properties).EndInit();
            ((ISupportInitialize)checkEdit屏蔽战功.Properties).EndInit();
            ((ISupportInitialize)checkEdit屏蔽威望.Properties).EndInit();
            ((ISupportInitialize)checkEdit屏蔽七天活动.Properties).EndInit();
            ((ISupportInitialize)textEdit神佑掉落ID.Properties).EndInit();
            ((ISupportInitialize)checkEdit普通强化不碎武器.Properties).EndInit();
            ((ISupportInitialize)checkEditBOSS自动死亡.Properties).EndInit();
            ((ISupportInitialize)textEdit暴击特效ID.Properties).EndInit();
            ((ISupportInitialize)checkEdit可购买玛法特权.Properties).EndInit();
            ((ISupportInitialize)checkEditResBag.Properties).EndInit();
            ((ISupportInitialize)ReBuildItemCheck.Properties).EndInit();
            ((ISupportInitialize)JobOpenCheck.Properties).EndInit();
            ((ISupportInitialize)PacketCheck.Properties).EndInit();
            ((ISupportInitialize)EnabledAchievementCheck.Properties).EndInit();
            ((ISupportInitialize)EnabledQuestCheck.Properties).EndInit();
            ((ISupportInitialize)AutoFightCheck.Properties).EndInit();
            ((ISupportInitialize)MobDropEdit.Properties).EndInit();
            ((ISupportInitialize)MobExpEdit.Properties).EndInit();
            ((ISupportInitialize)SRepairEdit.Properties).EndInit();
            ((ISupportInitialize)DebuffRateEdit.Properties).EndInit();
            ((ISupportInitialize)SlaveTimeEdit.Properties).EndInit();
            ((ISupportInitialize)FightSExpEdit.Properties).EndInit();
            ((ISupportInitialize)FightExpEdit.Properties).EndInit();
            ((ISupportInitialize)FightTime2Edit.Properties).EndInit();
            ((ISupportInitialize)FightTime1Edit.Properties).EndInit();
            ((ISupportInitialize)DebuffLevelEdit.Properties).EndInit();
            ((ISupportInitialize)ItemOwnerTimeEdit.Properties).EndInit();
            ((ISupportInitialize)ItemCleanTimeEdit.Properties).EndInit();
            ((ISupportInitialize)NewbieLevelEdit.Properties).EndInit();
            ((ISupportInitialize)HighLevelEdit.Properties).EndInit();
            xtraTabPage1.ResumeLayout(false);
            ((ISupportInitialize)groupControl5).EndInit();
            groupControl5.ResumeLayout(false);
            groupControl5.PerformLayout();
            ((ISupportInitialize)ServerPathEdit.Properties).EndInit();
            ((ISupportInitialize)SavePathEdit.Properties).EndInit();
            ((ISupportInitialize)groupControl4).EndInit();
            groupControl4.ResumeLayout(false);
            groupControl4.PerformLayout();
            ((ISupportInitialize)textEdit1.Properties).EndInit();
            ((ISupportInitialize)ExceptionBanEdit.Properties).EndInit();
            ((ISupportInitialize)DisconnectTimeEdit.Properties).EndInit();
            ((ISupportInitialize)ServerNameEdit.Properties).EndInit();
            ((ISupportInitialize)AutoSafeTimeEdit.Properties).EndInit();
            ((ISupportInitialize)StartDateEdit.Properties.CalendarTimeProperties).EndInit();
            ((ISupportInitialize)StartDateEdit.Properties).EndInit();
            ((ISupportInitialize)groupControl3).EndInit();
            groupControl3.ResumeLayout(false);
            groupControl3.PerformLayout();
            ((ISupportInitialize)PortEdit.Properties).EndInit();
            ((ISupportInitialize)TokenPortEdit.Properties).EndInit();
            ((ISupportInitialize)PacketLimitEdit.Properties).EndInit();
            ((ISupportInitialize)xtraTabControl1).EndInit();
            xtraTabControl1.ResumeLayout(false);
            xtraTabPage4.ResumeLayout(false);
            ((ISupportInitialize)宠物下属分组).EndInit();
            宠物下属分组.ResumeLayout(false);
            ((ISupportInitialize)下线宝宝不死check.Properties).EndInit();
            ((ISupportInitialize)安全区分组).EndInit();
            安全区分组.ResumeLayout(false);
            ((ISupportInitialize)groupControl6).EndInit();
            groupControl6.ResumeLayout(false);
            groupControl6.PerformLayout();
            ((ISupportInitialize)radioGroup2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private XtraTabPage xtraTabPage3;
        private CheckEdit 不分解极品check;
        private CheckEdit 分解装备check;
        private CheckEdit 物品入包check;
        private CheckEdit 银币入包check;
        private CheckEdit 金币入包check;
        private GroupControl groupControl1;
        private TextEdit textEdit14;
        private LabelControl labelControl56;
        private TextEdit textEdit17;
        private LabelControl labelControl59;
        private TextEdit textEdit18;
        private LabelControl labelControl60;
        private TextEdit textEdit19;
        private LabelControl labelControl61;
        private TextEdit textEdit20;
        private LabelControl labelControl62;
        private TextEdit textEdit13;
        private LabelControl labelControl55;
        private TextEdit textEdit8;
        private LabelControl labelControl49;
        private TextEdit textEdit12;
        private LabelControl labelControl54;
        private TextEdit textEdit11;
        private LabelControl labelControl53;
        private TextEdit textEdit10;
        private LabelControl labelControl52;
        private TextEdit textEdit9;
        private LabelControl labelControl51;
        private TextEdit textEdit6;
        private LabelControl labelControl46;
        private TextEdit textEdit5;
        private LabelControl labelControl45;
        private TextEdit textEdit3;
        private LabelControl labelControl44;
        private TextEdit textEdit_dropshoushi_red;
        private LabelControl labelControl30;
        private TextEdit textEdit_Dropweap_red;
        private LabelControl labelControl42;
        private TextEdit textEdit4;
        private LabelControl labelControl41;
        private TextEdit textEdit_dropbags;
        private LabelControl labelControl40;
        private TextEdit textEdit_dropshoushi;
        private LabelControl labelControl39;
        private TextEdit textEdit_Dropweap;
        private LabelControl labelControl38;
        private TextEdit textEdit_zhufuyou_Lv0;
        private LabelControl labelControl37;
        private TextEdit textEdit_zhufuyou_Lv6;
        private LabelControl labelControl35;
        private TextEdit textEdit_zhufuyou_Lv5;
        private LabelControl labelControl36;
        private TextEdit textEdit_zhufuyou_Lv4;
        private LabelControl labelControl33;
        private TextEdit textEdit_zhufuyou_Lv3;
        private LabelControl labelControl34;
        private TextEdit textEdit_zhufuyou_Lv2;
        private LabelControl labelControl32;
        private TextEdit textEdit_zhufuyou_Lv1;
        private LabelControl labelControl31;
        private XtraTabPage xtraTabPage2;
        private TextEdit textEdit15;
        private LabelControl labelControl57;
        private CheckEdit checkEdit6;
        private CheckEdit checkEdit5;
        private TextEdit textEdit7;
        private LabelControl labelControl47;
        private RadioGroup radioGroup1;
        private CheckEdit checkEdit4;
        private CheckEdit checkEdit3;
        private CheckEdit checkEdit1;
        private CheckEdit AutoFight_New;
        private CheckEdit checkEdit达最高级后继续加经验;
        private CheckEdit checkEdit屏蔽日程;
        private CheckEdit checkEdit屏蔽战功;
        private CheckEdit checkEdit屏蔽威望;
        private CheckEdit checkEdit屏蔽七天活动;
        private TextEdit textEdit神佑掉落ID;
        private LabelControl labelControl28;
        private CheckEdit checkEdit普通强化不碎武器;
        private CheckEdit checkEditBOSS自动死亡;
        private TextEdit textEdit暴击特效ID;
        private LabelControl labelControl26;
        private CheckEdit checkEdit可购买玛法特权;
        private CheckEdit checkEditResBag;
        private CheckEdit ReBuildItemCheck;
        private CheckEdit JobOpenCheck;
        private CheckEdit PacketCheck;
        private CheckEdit EnabledAchievementCheck;
        private CheckEdit EnabledQuestCheck;
        private CheckEdit AutoFightCheck;
        private TextEdit MobDropEdit;
        private LabelControl labelControl22;
        private TextEdit MobExpEdit;
        private TextEdit SRepairEdit;
        private TextEdit DebuffRateEdit;
        private LabelControl labelControl19;
        private LabelControl labelControl20;
        private LabelControl labelControl21;
        private TextEdit SlaveTimeEdit;
        private TextEdit FightSExpEdit;
        private TextEdit FightExpEdit;
        private TextEdit FightTime2Edit;
        private TextEdit FightTime1Edit;
        private LabelControl labelControl14;
        private LabelControl labelControl15;
        private LabelControl labelControl16;
        private LabelControl labelControl17;
        private LabelControl labelControl18;
        private TextEdit DebuffLevelEdit;
        private TextEdit ItemOwnerTimeEdit;
        private TextEdit ItemCleanTimeEdit;
        private TextEdit NewbieLevelEdit;
        private TextEdit HighLevelEdit;
        private LabelControl labelControl9;
        private LabelControl labelControl10;
        private LabelControl labelControl11;
        private LabelControl labelControl12;
        private LabelControl labelControl13;
        private XtraTabPage xtraTabPage1;
        private GroupControl groupControl5;
        private LabelControl labelControl6;
        private LabelControl labelControl7;
        private ButtonEdit ServerPathEdit;
        private ButtonEdit SavePathEdit;
        private GroupControl groupControl4;
        private LabelControl labelControl8;
        private LabelControl labelControl4;
        private LabelControl labelControl5;
        private TextEdit textEdit1;
        private TextEdit ExceptionBanEdit;
        private LabelControl labelControl29;
        private TextEdit DisconnectTimeEdit;
        private TextEdit ServerNameEdit;
        private TextEdit AutoSafeTimeEdit;
        private LabelControl labelControl24;
        private DateEdit StartDateEdit;
        private LabelControl labelControl23;
        private GroupControl groupControl3;
        private LabelControl labelControl1;
        private LabelControl labelControl2;
        private LabelControl labelControl3;
        private TextEdit PortEdit;
        private TextEdit TokenPortEdit;
        private TextEdit PacketLimitEdit;
        private XtraTabControl xtraTabControl1;
        private CheckEdit 安全区满血check;
        private XtraTabPage xtraTabPage4;
        private GroupControl 游戏地图开放分组;
        private GroupControl 自动分解分组;
        private GroupControl 自动拾取分组;
        private GroupControl 安全区分组;
        private GroupControl 死亡掉落分组;
        private GroupControl 祝福油几率分组;
        private GroupControl 其他分组;
        private CheckEdit 下线宝宝不死check;
        private GroupControl 宠物下属分组;
        private GroupControl groupControl6;
        private LabelControl labelControl63;
        private LabelControl labelControl58;
        private DevExpress.XtraRichEdit.RichEditControl richEditControl2;
        private DevExpress.XtraRichEdit.RichEditControl richEditControl1;
        private DevExpress.XtraEditors.Repository.RepositoryItemRadioGroup radioGroup2;
    }
}
