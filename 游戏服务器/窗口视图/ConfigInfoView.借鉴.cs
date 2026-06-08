using System;

namespace 游戏服务器.窗口视图
{
	// 「特殊功能」配置页的 控件↔Settings 双向绑定.
	// 控件本身在 ConfigInfoView.Designer.cs 的 InitializeComponent 里以标准设计器代码创建(可在 VS 设计器编辑).
	// 由 ConfigInfoView.cs 的 LoadConfig 末尾调用 借鉴_加载(), SaveConfig 中 Settings.Save() 前调用 借鉴_保存().
	public partial class ConfigInfoView
	{
		private void 借鉴_加载()
		{
			this.chk幸运倍率.Checked = Settings.开启幸运倍率功能;
			this.spn幸运0.Value = Settings.幸运额外增伤值[0]; this.spn倍率0.Value = (decimal)Settings.幸运增伤倍率值[0];
			this.spn幸运1.Value = Settings.幸运额外增伤值[1]; this.spn倍率1.Value = (decimal)Settings.幸运增伤倍率值[1];
			this.spn幸运2.Value = Settings.幸运额外增伤值[2]; this.spn倍率2.Value = (decimal)Settings.幸运增伤倍率值[2];
			this.spn幸运3.Value = Settings.幸运额外增伤值[3]; this.spn倍率3.Value = (decimal)Settings.幸运增伤倍率值[3];
			this.spn幸运4.Value = Settings.幸运额外增伤值[4]; this.spn倍率4.Value = (decimal)Settings.幸运增伤倍率值[4];
			this.spn幸运5.Value = Settings.幸运额外增伤值[5]; this.spn倍率5.Value = (decimal)Settings.幸运增伤倍率值[5];
			this.spn幸运6.Value = Settings.幸运额外增伤值[6]; this.spn倍率6.Value = (decimal)Settings.幸运增伤倍率值[6];
			this.spn幸运7.Value = Settings.幸运额外增伤值[7]; this.spn倍率7.Value = (decimal)Settings.幸运增伤倍率值[7];
			this.spn幸运8.Value = Settings.幸运额外增伤值[8]; this.spn倍率8.Value = (decimal)Settings.幸运增伤倍率值[8];
			this.spn幸运9.Value = Settings.幸运额外增伤值[9]; this.spn倍率9.Value = (decimal)Settings.幸运增伤倍率值[9];

			this.chk职业第一.Checked = Settings.职业第一机制;
			this.spn职业0.Value = Settings.职业第一编号[0];
			this.spn职业1.Value = Settings.职业第一编号[1];
			this.spn职业2.Value = Settings.职业第一编号[2];
			this.spn职业3.Value = Settings.职业第一编号[3];
			this.spn职业4.Value = Settings.职业第一编号[4];
			this.spn职业5.Value = Settings.职业第一编号[5];

			this.chk怪物首杀.Checked = Settings.开启怪物首杀功能;
			this.chk首杀货币.Checked = Settings.开启怪物首杀货币;
			this.chk首杀道具.Checked = Settings.开启怪物首杀道具;
			this.chk首杀邮件.Checked = Settings.开启首杀奖励邮件;
			this.spn首杀货币类型.Value = Settings.首杀货币类型;
			this.spn首杀货币数量.Value = Settings.首杀货币数量;
			this.spn首杀道具编号.Value = Settings.首杀道具编号;
			this.spn首杀道具数量.Value = Settings.首杀道具数量;

			this.chk装备首爆.Checked = Settings.开启装备首爆功能;
			this.chk首爆货币.Checked = Settings.开启装备首爆货币;
			this.chk首爆道具.Checked = Settings.开启装备首爆道具;
			this.chk首爆邮件.Checked = Settings.开启首爆奖励邮件;
			this.spn首爆货币类型.Value = Settings.首爆货币类型;
			this.spn首爆货币数量.Value = Settings.首爆货币数量;
			this.spn首爆道具编号.Value = Settings.首爆道具编号;
			this.spn首爆道具数量.Value = Settings.首爆道具数量;

			this.chk公会等级特效.Checked = Settings.开启公会等级特效;
			this.chk公会职位特效.Checked = Settings.开启公会职位特效;
			this.spn等级1.Value = Settings.行会等级特效编号[1];
			this.spn等级2.Value = Settings.行会等级特效编号[2];
			this.spn等级3.Value = Settings.行会等级特效编号[3];
			this.spn等级4.Value = Settings.行会等级特效编号[4];
			this.spn等级5.Value = Settings.行会等级特效编号[5];
			this.spn等级6.Value = Settings.行会等级特效编号[6];
			this.spn职位1.Value = Settings.行会职位特效编号[1];
			this.spn职位2.Value = Settings.行会职位特效编号[2];
			this.spn职位3.Value = Settings.行会职位特效编号[3];
			this.spn职位4.Value = Settings.行会职位特效编号[4];
			this.spn职位5.Value = Settings.行会职位特效编号[5];
			this.spn职位6.Value = Settings.行会职位特效编号[6];

			this.spn攻沙开始时.Value = Settings.攻沙开始时间小时;
			this.spn攻沙开始分.Value = Settings.攻沙开始时间分钟;
			this.spn攻沙结束时.Value = Settings.攻沙结束时间小时;
			this.spn攻沙结束分.Value = Settings.攻沙结束时间分钟;
			this.spn攻城持续.Value = Settings.攻城持续时间;

			this.spn尸王殿击杀数.Value = Settings.尸王殿开启击杀数;
			this.spn尸王殿存续秒.Value = Settings.尸王殿门存续秒数;

			// 网络·安全页
			this.spn最大连接数.Value = Settings.最大连接数;
			this.spn单IP连接上限.Value = Settings.单IP连接上限;
			this.richEditControl2.Text = Settings.连接IP白名单.Replace(",", "\r\n"); // 复用「玩家限制登录」组现成的 IP白名单框, 逗号→换行多行显示
			this.chk禁止创建角色.Checked = Settings.禁止创建角色;
			this.txt门票来源白名单.Text = Settings.门票来源白名单;
			this.spn货币异常上限.Value = Settings.货币异常上限;
			this.spn货币异常归位.Value = Settings.货币异常归位;
			this.spn日志保留天数.Value = Settings.日志保留天数;

			// 假人系统(存独立 CSV, 不走 Settings): 载入到「特殊功能 / 假人管理」子页
			this.假人_载入界面();
		}

		private void 借鉴_保存()
		{
			Settings.开启幸运倍率功能 = this.chk幸运倍率.Checked;
			Settings.幸运额外增伤值[0] = Convert.ToInt32(this.spn幸运0.Value); Settings.幸运增伤倍率值[0] = Convert.ToSingle(this.spn倍率0.Value);
			Settings.幸运额外增伤值[1] = Convert.ToInt32(this.spn幸运1.Value); Settings.幸运增伤倍率值[1] = Convert.ToSingle(this.spn倍率1.Value);
			Settings.幸运额外增伤值[2] = Convert.ToInt32(this.spn幸运2.Value); Settings.幸运增伤倍率值[2] = Convert.ToSingle(this.spn倍率2.Value);
			Settings.幸运额外增伤值[3] = Convert.ToInt32(this.spn幸运3.Value); Settings.幸运增伤倍率值[3] = Convert.ToSingle(this.spn倍率3.Value);
			Settings.幸运额外增伤值[4] = Convert.ToInt32(this.spn幸运4.Value); Settings.幸运增伤倍率值[4] = Convert.ToSingle(this.spn倍率4.Value);
			Settings.幸运额外增伤值[5] = Convert.ToInt32(this.spn幸运5.Value); Settings.幸运增伤倍率值[5] = Convert.ToSingle(this.spn倍率5.Value);
			Settings.幸运额外增伤值[6] = Convert.ToInt32(this.spn幸运6.Value); Settings.幸运增伤倍率值[6] = Convert.ToSingle(this.spn倍率6.Value);
			Settings.幸运额外增伤值[7] = Convert.ToInt32(this.spn幸运7.Value); Settings.幸运增伤倍率值[7] = Convert.ToSingle(this.spn倍率7.Value);
			Settings.幸运额外增伤值[8] = Convert.ToInt32(this.spn幸运8.Value); Settings.幸运增伤倍率值[8] = Convert.ToSingle(this.spn倍率8.Value);
			Settings.幸运额外增伤值[9] = Convert.ToInt32(this.spn幸运9.Value); Settings.幸运增伤倍率值[9] = Convert.ToSingle(this.spn倍率9.Value);

			Settings.职业第一机制 = this.chk职业第一.Checked;
			Settings.职业第一编号[0] = (ushort)Convert.ToInt32(this.spn职业0.Value);
			Settings.职业第一编号[1] = (ushort)Convert.ToInt32(this.spn职业1.Value);
			Settings.职业第一编号[2] = (ushort)Convert.ToInt32(this.spn职业2.Value);
			Settings.职业第一编号[3] = (ushort)Convert.ToInt32(this.spn职业3.Value);
			Settings.职业第一编号[4] = (ushort)Convert.ToInt32(this.spn职业4.Value);
			Settings.职业第一编号[5] = (ushort)Convert.ToInt32(this.spn职业5.Value);

			Settings.开启怪物首杀功能 = this.chk怪物首杀.Checked;
			Settings.开启怪物首杀货币 = this.chk首杀货币.Checked;
			Settings.开启怪物首杀道具 = this.chk首杀道具.Checked;
			Settings.开启首杀奖励邮件 = this.chk首杀邮件.Checked;
			Settings.首杀货币类型 = Convert.ToInt32(this.spn首杀货币类型.Value);
			Settings.首杀货币数量 = Convert.ToInt32(this.spn首杀货币数量.Value);
			Settings.首杀道具编号 = Convert.ToInt32(this.spn首杀道具编号.Value);
			Settings.首杀道具数量 = Convert.ToInt32(this.spn首杀道具数量.Value);

			Settings.开启装备首爆功能 = this.chk装备首爆.Checked;
			Settings.开启装备首爆货币 = this.chk首爆货币.Checked;
			Settings.开启装备首爆道具 = this.chk首爆道具.Checked;
			Settings.开启首爆奖励邮件 = this.chk首爆邮件.Checked;
			Settings.首爆货币类型 = Convert.ToInt32(this.spn首爆货币类型.Value);
			Settings.首爆货币数量 = Convert.ToInt32(this.spn首爆货币数量.Value);
			Settings.首爆道具编号 = Convert.ToInt32(this.spn首爆道具编号.Value);
			Settings.首爆道具数量 = Convert.ToInt32(this.spn首爆道具数量.Value);

			Settings.开启公会等级特效 = this.chk公会等级特效.Checked;
			Settings.开启公会职位特效 = this.chk公会职位特效.Checked;
			Settings.行会等级特效编号[1] = (ushort)Convert.ToInt32(this.spn等级1.Value);
			Settings.行会等级特效编号[2] = (ushort)Convert.ToInt32(this.spn等级2.Value);
			Settings.行会等级特效编号[3] = (ushort)Convert.ToInt32(this.spn等级3.Value);
			Settings.行会等级特效编号[4] = (ushort)Convert.ToInt32(this.spn等级4.Value);
			Settings.行会等级特效编号[5] = (ushort)Convert.ToInt32(this.spn等级5.Value);
			Settings.行会等级特效编号[6] = (ushort)Convert.ToInt32(this.spn等级6.Value);
			Settings.行会职位特效编号[1] = (ushort)Convert.ToInt32(this.spn职位1.Value);
			Settings.行会职位特效编号[2] = (ushort)Convert.ToInt32(this.spn职位2.Value);
			Settings.行会职位特效编号[3] = (ushort)Convert.ToInt32(this.spn职位3.Value);
			Settings.行会职位特效编号[4] = (ushort)Convert.ToInt32(this.spn职位4.Value);
			Settings.行会职位特效编号[5] = (ushort)Convert.ToInt32(this.spn职位5.Value);
			Settings.行会职位特效编号[6] = (ushort)Convert.ToInt32(this.spn职位6.Value);

			Settings.攻沙开始时间小时 = (byte)Convert.ToInt32(this.spn攻沙开始时.Value);
			Settings.攻沙开始时间分钟 = (byte)Convert.ToInt32(this.spn攻沙开始分.Value);
			Settings.攻沙结束时间小时 = (byte)Convert.ToInt32(this.spn攻沙结束时.Value);
			Settings.攻沙结束时间分钟 = (byte)Convert.ToInt32(this.spn攻沙结束分.Value);
			Settings.攻城持续时间 = Convert.ToInt32(this.spn攻城持续.Value);

			Settings.尸王殿开启击杀数 = Convert.ToInt32(this.spn尸王殿击杀数.Value);
			Settings.尸王殿门存续秒数 = Convert.ToInt32(this.spn尸王殿存续秒.Value);

			// 网络·安全页
			Settings.最大连接数 = Convert.ToInt32(this.spn最大连接数.Value);
			Settings.单IP连接上限 = Convert.ToInt32(this.spn单IP连接上限.Value);
			Settings.连接IP白名单 = string.Join(",", this.richEditControl2.Text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)); // 多行 IP→逗号分隔单行存 INI
			Settings.禁止创建角色 = this.chk禁止创建角色.Checked;
			Settings.门票来源白名单 = this.txt门票来源白名单.Text;
			Settings.货币异常上限 = Convert.ToUInt32(this.spn货币异常上限.Value);
			Settings.货币异常归位 = Convert.ToUInt32(this.spn货币异常归位.Value);
			Settings.日志保留天数 = Convert.ToInt32(this.spn日志保留天数.Value);
		}
	}
}
