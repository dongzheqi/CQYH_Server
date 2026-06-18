using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors.Controls;


namespace 游戏服务器.窗口视图
{
    public partial class ConfigInfoView : RibbonForm
    {

        public ConfigInfoView()
        {
            this.InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.LoadConfig();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            this.SaveConfig();
        }

        private void LoadConfig()
        {
            this.PortEdit.EditValue = Settings.客户连接端口;
            this.AutoSafeTimeEdit.EditValue = Settings.自动保存时间;
            this.DisconnectTimeEdit.EditValue = Settings.掉线判定时间;
            this.ExceptionBanEdit.EditValue = Settings.异常屏蔽时间;
            this.PacketLimitEdit.EditValue = Settings.封包限定数量;
            this.TokenPortEdit.EditValue = Settings.门票接收端口;
            //this.HttpTokenPortEdit.EditValue = Settings.Http门票接收端口;
            //this.textEdit_AccountIp.EditValue = Settings.指定账号服务器IP;
            //this.textEdit2.EditValue = Settings.充值监听端口;
            this.SavePathEdit.EditValue = Settings.数据备份目录;
            this.ServerPathEdit.EditValue = Settings.游戏数据目录;
            this.DebuffLevelEdit.EditValue = Settings.减收益等级差;
            this.ItemOwnerTimeEdit.EditValue = Settings.物品归属时间;
            this.ItemCleanTimeEdit.EditValue = Settings.物品清理时间;
            this.NewbieLevelEdit.EditValue = Settings.新手扶持等级;
            this.HighLevelEdit.EditValue = Settings.游戏开放等级;
            this.SlaveTimeEdit.EditValue = Settings.怪物诱惑时长;
            this.FightSExpEdit.EditValue = Settings.武斗抢点经验;
            this.FightExpEdit.EditValue = Settings.武斗普通经验;
            this.FightTime2Edit.EditValue = Settings.武斗场时间二;
            this.FightTime1Edit.EditValue = Settings.武斗场时间一;
            this.MobDropEdit.EditValue = Settings.怪物额外爆率;
            this.MobExpEdit.EditValue = Settings.怪物经验倍率;
            this.SRepairEdit.EditValue = Settings.装备特修折扣;
            this.DebuffRateEdit.EditValue = Settings.收益减少比率;
            this.StartDateEdit.DateTime = Settings.开服日期;
            this.PacketCheck.Checked = Settings.开启线程发包;
            this.AutoFightCheck.Checked = Settings.开启自动战斗;
            this.EnabledQuestCheck.Checked = Settings.开启任务系统;
            this.EnabledAchievementCheck.Checked = Settings.开启成就系统;
            this.ServerNameEdit.Text = Settings.游戏区服名称;
            ///this.ServerUUIDEdit.Text = Settings.统计UUID代码;
            this.ReBuildItemCheck.Checked = Settings.触发装备重铸;
            this.checkEditResBag.Checked = Settings.资源包只能放材料;
            this.checkEdit可购买玛法特权.Checked = Settings.可购买玛法特权;
            this.textEdit暴击特效ID.EditValue = Settings.暴击特效ID;
            this.checkEditBOSS自动死亡.Checked = Settings.BOSS自动死亡;
            this.checkEdit普通强化不碎武器.Checked = Settings.普通强化不碎武器;
            this.checkEdit屏蔽七天活动.Checked = Settings.屏蔽七天活动;
            this.checkEdit屏蔽战功.Checked = Settings.屏蔽战功;
            this.checkEdit屏蔽威望.Checked = Settings.屏蔽威望;
            this.checkEdit屏蔽日程.Checked = Settings.屏蔽日程;
            this.checkEdit1.Checked = Settings.屏蔽每周特惠;
            this.checkEdit3.Checked = Settings.屏蔽每日签到;
            this.checkEdit4.Checked = Settings.屏蔽传永武技;
            this.radioGroup1.SelectedIndex = Settings.充值货币类型;
            this.checkEdit达最高级后继续加经验.Checked = Settings.达最高级后继续加经验;
            this.JobOpenCheck.Checked = Settings.职业开放[2] && Settings.职业开放[3] && Settings.职业开放[5];
            this.textEdit神佑掉落ID.EditValue = Settings.神佑掉落ID;
            this.textEdit1.EditValue = Settings.商人比例;
            this.AutoFight_New.Checked = Settings.使用新版内挂;
            this.checkEdit5.Checked = Settings.沙巴克掉装备;
            this.checkEdit6.Checked = Settings.限制重要封包间隔时间;
            this.textEdit_zhufuyou_Lv0.EditValue = Settings.祝福油几率0级;
            this.textEdit_zhufuyou_Lv1.EditValue = Settings.祝福油几率1级;
            this.textEdit_zhufuyou_Lv2.EditValue = Settings.祝福油几率2级;
            this.textEdit_zhufuyou_Lv3.EditValue = Settings.祝福油几率3级;
            this.textEdit_zhufuyou_Lv4.EditValue = Settings.祝福油几率4级;
            this.textEdit_zhufuyou_Lv5.EditValue = Settings.祝福油几率5级;
            this.textEdit_zhufuyou_Lv6.EditValue = Settings.祝福油几率6级;
            this.textEdit_Dropweap.EditValue = Settings.死亡掉落剑甲;
            this.textEdit_dropshoushi.EditValue = Settings.死亡掉落首饰;
            this.textEdit_dropbags.EditValue = Settings.死亡掉落背包;
            this.textEdit4.EditValue = Settings.单次死亡限量;
            this.textEdit_Dropweap_red.EditValue = Settings.红名掉落剑甲;
            this.textEdit_dropshoushi_red.EditValue = Settings.红名掉落首饰;
            this.textEdit3.EditValue = Settings.龙卫重铸费用;
            this.textEdit5.EditValue = Settings.锁单重铸费用;
            this.textEdit6.EditValue = Settings.锁半重铸费用;
            this.textEdit7.EditValue = Settings.行会最高人数;
            this.textEdit8.EditValue = Settings.幽冥海底节点开放天数;
            this.textEdit9.EditValue = Settings.白日赤月节点开放天数;
            this.textEdit10.EditValue = Settings.魔龙之城节点开放天数;
            this.textEdit11.EditValue = Settings.苍月惊变节点开放天数;
            this.textEdit12.EditValue = Settings.龙耀雪山节点开放天数;
            this.textEdit13.EditValue = Settings.聊天限制等级;
            this.textEdit14.EditValue = Settings.玛法新秀价格;
            this.textEdit20.EditValue = Settings.玛法名俊价格;
            this.textEdit19.EditValue = Settings.玛法豪杰价格;
            this.textEdit18.EditValue = Settings.玛法战将价格;
            this.textEdit17.EditValue = Settings.玛法至尊价格;
            this.textEdit15.EditValue = Settings.技巧项链倍数;
            //新增
            this.金币入包check.EditValue = Settings.金币自动入包;
            this.银币入包check.EditValue = Settings.银币自动入包;
            this.物品入包check.EditValue = Settings.物品自动入包;
            this.分解装备check.EditValue = Settings.自动分解装备;
            this.不分解极品check.EditValue = Settings.不分解极品装备;
            this.安全区满血check.EditValue = Settings.安全区内满血满蓝;
            this.下线宝宝不死check.EditValue = Settings.下线宝宝不死;
            this.叛变直接死亡check.EditValue = Settings.叛变直接死亡;
            this.借鉴_加载();
        }
        //保存配置
        private void SaveConfig()
        {
            Settings.客户连接端口 = (ushort)this.PortEdit.EditValue;
            Settings.自动保存时间 = (byte)this.AutoSafeTimeEdit.EditValue;
            Settings.掉线判定时间 = (ushort)this.DisconnectTimeEdit.EditValue;
            Settings.异常屏蔽时间 = (ushort)this.ExceptionBanEdit.EditValue;
            Settings.封包限定数量 = (ushort)this.PacketLimitEdit.EditValue;
            Settings.门票接收端口 = (ushort)this.TokenPortEdit.EditValue;
            //Settings.Http门票接收端口 = (ushort)this.HttpTokenPortEdit.EditValue;
            //Settings.指定账号服务器IP = (string)this.textEdit_AccountIp.EditValue;
            //Settings.充值监听端口 = (ushort)this.textEdit2.EditValue;
            Settings.数据备份目录 = (string)this.SavePathEdit.EditValue;
            Settings.游戏数据目录 = (string)this.ServerPathEdit.EditValue;
            Settings.减收益等级差 = (byte)this.DebuffLevelEdit.EditValue;
            Settings.物品归属时间 = (byte)this.ItemOwnerTimeEdit.EditValue;
            Settings.物品清理时间 = (byte)this.ItemCleanTimeEdit.EditValue;
            Settings.新手扶持等级 = (byte)this.NewbieLevelEdit.EditValue;
            Settings.游戏开放等级 = (byte)this.HighLevelEdit.EditValue;
            Settings.怪物诱惑时长 = (ushort)this.SlaveTimeEdit.EditValue;
            Settings.武斗抢点经验 = (int)this.FightSExpEdit.EditValue;
            Settings.武斗普通经验 = (int)this.FightExpEdit.EditValue;
            Settings.武斗场时间二 = (byte)this.FightTime2Edit.EditValue;
            Settings.武斗场时间一 = (byte)this.FightTime1Edit.EditValue;
            Settings.怪物额外爆率 = (decimal)this.MobDropEdit.EditValue;
            Settings.怪物经验倍率 = (decimal)this.MobExpEdit.EditValue;
            Settings.装备特修折扣 = (decimal)this.SRepairEdit.EditValue;
            Settings.收益减少比率 = (decimal)this.DebuffRateEdit.EditValue;
            Settings.开服日期 = this.StartDateEdit.DateTime;
            Settings.开启线程发包 = this.PacketCheck.Checked;
            Settings.开启自动战斗 = this.AutoFightCheck.Checked;
            Settings.使用新版内挂 = this.AutoFight_New.Checked;
            Settings.沙巴克掉装备 = this.checkEdit5.Checked;
            Settings.限制重要封包间隔时间 = this.checkEdit6.Checked;
            Settings.开启任务系统 = this.EnabledQuestCheck.Checked;
            Settings.开启成就系统 = this.EnabledAchievementCheck.Checked;
            Settings.游戏区服名称 = this.ServerNameEdit.Text;
            //Settings.统计UUID代码 = this.ServerUUIDEdit.Text;
            Settings.触发装备重铸 = this.ReBuildItemCheck.Checked;
            Settings.资源包只能放材料 = this.checkEditResBag.Checked;
            Settings.可购买玛法特权 = this.checkEdit可购买玛法特权.Checked;
            Settings.暴击特效ID = (byte)this.textEdit暴击特效ID.EditValue;
            Settings.BOSS自动死亡 = this.checkEditBOSS自动死亡.Checked;
            Settings.普通强化不碎武器 = this.checkEdit普通强化不碎武器.Checked;
            Settings.屏蔽七天活动 = this.checkEdit屏蔽七天活动.Checked;
            Settings.屏蔽战功 = this.checkEdit屏蔽战功.Checked;
            Settings.屏蔽威望 = this.checkEdit屏蔽威望.Checked;
            Settings.屏蔽日程 = this.checkEdit屏蔽日程.Checked;
            Settings.屏蔽每周特惠 = this.checkEdit1.Checked;
            Settings.屏蔽每日签到 = this.checkEdit3.Checked;
            Settings.屏蔽传永武技 = this.checkEdit4.Checked;
            Settings.充值货币类型 = this.radioGroup1.SelectedIndex;
            Settings.神佑掉落ID = (int)this.textEdit神佑掉落ID.EditValue;
            Settings.商人比例 = (int)this.textEdit1.EditValue;
            Settings.达最高级后继续加经验 = this.checkEdit达最高级后继续加经验.Checked;
            Settings.祝福油几率0级 = (int)this.textEdit_zhufuyou_Lv0.EditValue;
            Settings.祝福油几率1级 = (int)this.textEdit_zhufuyou_Lv1.EditValue;
            Settings.祝福油几率2级 = (int)this.textEdit_zhufuyou_Lv2.EditValue;
            Settings.祝福油几率3级 = (int)this.textEdit_zhufuyou_Lv3.EditValue;
            Settings.祝福油几率4级 = (int)this.textEdit_zhufuyou_Lv4.EditValue;
            Settings.祝福油几率5级 = (int)this.textEdit_zhufuyou_Lv5.EditValue;
            Settings.祝福油几率6级 = (int)this.textEdit_zhufuyou_Lv6.EditValue;
            Settings.死亡掉落剑甲 = (float)this.textEdit_Dropweap.EditValue;
            Settings.死亡掉落首饰 = (float)this.textEdit_dropshoushi.EditValue;
            Settings.死亡掉落背包 = (float)this.textEdit_dropbags.EditValue;
            Settings.单次死亡限量 = (int)this.textEdit4.EditValue;
            Settings.红名掉落剑甲 = (float)this.textEdit_Dropweap_red.EditValue;
            Settings.红名掉落首饰 = (float)this.textEdit_dropshoushi_red.EditValue;
            Settings.龙卫重铸费用 = (int)this.textEdit3.EditValue;
            Settings.锁单重铸费用 = (int)this.textEdit5.EditValue;
            Settings.锁半重铸费用 = (int)this.textEdit6.EditValue;
            Settings.行会最高人数 = (int)this.textEdit7.EditValue;
            Settings.幽冥海底节点开放天数 = (byte)this.textEdit8.EditValue;
            Settings.白日赤月节点开放天数 = (byte)this.textEdit9.EditValue;
            Settings.魔龙之城节点开放天数 = (byte)this.textEdit10.EditValue;
            Settings.苍月惊变节点开放天数 = (byte)this.textEdit11.EditValue;
            Settings.龙耀雪山节点开放天数 = (byte)this.textEdit12.EditValue;
            Settings.聊天限制等级 = (byte)this.textEdit13.EditValue;
            Settings.玛法新秀价格 = (int)this.textEdit14.EditValue;
            Settings.玛法名俊价格 = (int)this.textEdit20.EditValue;
            Settings.玛法豪杰价格 = (int)this.textEdit19.EditValue;
            Settings.玛法战将价格 = (int)this.textEdit18.EditValue;
            Settings.玛法至尊价格 = (int)this.textEdit17.EditValue;
            Settings.技巧项链倍数 = (float)this.textEdit15.EditValue;
            //新增
            Settings.金币自动入包 = (bool)this.金币入包check.EditValue;
            Settings.银币自动入包 = (bool)this.银币入包check.EditValue;
            Settings.物品自动入包 = (bool)this.物品入包check.EditValue;
            Settings.自动分解装备 = (bool)this.分解装备check.EditValue;
            Settings.不分解极品装备 = (bool)this.不分解极品check.EditValue;
            Settings.安全区内满血满蓝 = (bool)this.安全区满血check.EditValue;
            Settings.下线宝宝不死 = (bool)this.下线宝宝不死check.EditValue;
            Settings.叛变直接死亡 = (bool)this.叛变直接死亡check.EditValue;

            if (this.JobOpenCheck.Checked)
            {
                bool[] 职业开放;
                职业开放 = Settings.职业开放;
                bool[] 职业开放2;
                职业开放2 = Settings.职业开放;
                bool[] 职业开放3;
                职业开放3 = Settings.职业开放;
                bool[] 职业开放4;
                职业开放4 = Settings.职业开放;
                bool[] 职业开放5;
                职业开放5 = Settings.职业开放;
                Settings.职业开放[5] = true;
                职业开放5[4] = true;
                职业开放4[3] = true;
                职业开放3[2] = true;
                职业开放2[1] = true;
                职业开放[0] = true;
            }
            else
            {
                bool[] 职业开放6;
                职业开放6 = Settings.职业开放;
                bool[] 职业开放7;
                职业开放7 = Settings.职业开放;
                Settings.职业开放[5] = false;
                职业开放7[3] = false;
                职业开放6[2] = false;
            }
            this.借鉴_保存();
            Settings.Save();
        }

        private void SaveDataBaseButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.SaveConfig();
        }

        private void ReLoadDataBaseButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            Settings.Load();
            this.LoadConfig();
        }

        private void VersionPathEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            this.FolderDialog.ShowDialog();
        }

        private void buttonEdit1_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            this.FolderDialog.ShowDialog();
        }

        private void xtraTabPage2_Paint(object sender, PaintEventArgs e)
        {
        }



        private void 分解装备check_CheckedChanged(object sender, EventArgs e)
        {
            if (分解装备check.Checked == false)
            {
                不分解极品check.Checked = false;
                不分解极品check.Enabled = false;
            }
            else
            {
                不分解极品check.Enabled = true;
            }
        }

        private void xtraTabPage3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void DisconnectTimeEdit_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void checkEdit2_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
