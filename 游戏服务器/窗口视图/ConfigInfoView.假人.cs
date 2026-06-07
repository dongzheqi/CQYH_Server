using System;
using System.Linq;
using 游戏服务器.管理命令;
using 游戏服务器.模板类;

namespace 游戏服务器.窗口视图
{
    // 「特殊功能 / 假人管理」子页的逻辑。假人配置存独立 CSV(非 Settings), 故本页自带 保存/重载/填充/下线/刷新,
    // 不并入 ConfigInfoView 的 借鉴_保存; 载入则由 借鉴_加载() 末尾调用 假人_载入界面()。
    // 所有运行期操作经 假人_在游戏线程(...) 切到主循环线程, 与 GMToolView 同款线程安全范式。
    public partial class ConfigInfoView
    {
        private void 假人_载入界面()
        {
            假人配置 c = 假人配置.载入数据();
            this.chk假人启用.Checked = c.启用;
            this.spn假人总数.Value = c.假人总数;
            this.spn假人最小等级.Value = c.最小等级;
            this.spn假人最大等级.Value = c.最大等级;
            this.txt假人名字前缀.Text = c.名字前缀;
            this.spn假人喝药阈值.Value = c.喝药血量百分比;
            this.spn假人上下线间隔.Value = c.上下线间隔毫秒;
            this.spn假人喊话间隔.Value = c.喊话间隔秒;
            this.chk假人喊话.Checked = c.开启喊话;
            this.chk假人PK.Checked = c.开启PK;
            this.memo假人配额.Text = string.Join("\r\n",
                c.地图配额.Select(q => $"{q.地图编号},{q.在线数量},{(q.是否打怪 ? 1 : 0)},{(q.是否PK ? 1 : 0)}"));
            this.memo假人喊话.Text = string.Join("\r\n", c.喊话内容);
            this.假人_刷新状态();
        }

        private 假人配置 假人_收集配置()
        {
            假人配置 c = new 假人配置();
            c.启用 = this.chk假人启用.Checked;
            c.假人总数 = Convert.ToInt32(this.spn假人总数.Value);
            c.最小等级 = Convert.ToInt32(this.spn假人最小等级.Value);
            c.最大等级 = Convert.ToInt32(this.spn假人最大等级.Value);
            c.名字前缀 = string.IsNullOrWhiteSpace(this.txt假人名字前缀.Text) ? "游侠" : this.txt假人名字前缀.Text.Trim();
            c.喝药血量百分比 = Convert.ToInt32(this.spn假人喝药阈值.Value);
            c.上下线间隔毫秒 = Convert.ToInt32(this.spn假人上下线间隔.Value);
            c.喊话间隔秒 = Convert.ToInt32(this.spn假人喊话间隔.Value);
            c.开启喊话 = this.chk假人喊话.Checked;
            c.开启PK = this.chk假人PK.Checked;
            // 配额: 每行 "地图编号,在线数量,是否打怪,是否PK"
            foreach (string 行 in (this.memo假人配额.Text ?? "").Replace("\r\n", "\n").Split('\n'))
            {
                string s = 行.Trim();
                if (s.Length == 0 || s.StartsWith("#"))
                {
                    continue;
                }
                string[] 列 = s.Split(',');
                if (列.Length < 2 || !ushort.TryParse(列[0].Trim(), out ushort 图))
                {
                    continue;
                }
                int.TryParse(列[1].Trim(), out int 数);
                bool 打 = 列.Length > 2 && (列[2].Trim() == "1" || 列[2].Trim() == "是");
                bool pk = 列.Length > 3 && (列[3].Trim() == "1" || 列[3].Trim() == "是");
                c.地图配额.Add(new 每图配额 { 地图编号 = 图, 在线数量 = 数, 是否打怪 = 打, 是否PK = pk });
            }
            c.喊话内容 = (this.memo假人喊话.Text ?? "").Replace("\r\n", "\n").Split('\n')
                .Select(x => x.Trim()).Where(x => x.Length > 0 && !x.StartsWith("#")).ToList();
            return c;
        }

        private void 假人_在游戏线程(Action 动作)
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

        private void 假人_在UI线程(Action 动作)
        {
            try
            {
                if (!this.IsDisposed && this.IsHandleCreated)
                {
                    this.BeginInvoke(动作);
                }
            }
            catch
            {
            }
        }

        private void btn假人保存_Click(object sender, EventArgs e)
        {
            try
            {
                this.假人_收集配置().保存数据();
                this.lbl假人状态.Text = "已保存到 CSV，正在重载...";
                this.假人_在游戏线程(delegate
                {
                    假人网关.重载配置();
                    string s = 假人网关.状态文本();
                    this.假人_在UI线程(delegate
                    {
                        this.lbl假人状态.Text = "已保存并重载。\r\n" + s;
                    });
                });
            }
            catch (Exception ex)
            {
                this.lbl假人状态.Text = "保存失败: " + ex.Message;
            }
        }

        private void btn假人重载_Click(object sender, EventArgs e)
        {
            this.假人_在游戏线程(delegate
            {
                假人网关.重载配置();
                string s = 假人网关.状态文本();
                this.假人_在UI线程(delegate
                {
                    this.假人_载入界面();
                    this.lbl假人状态.Text = "已从 CSV 重载。\r\n" + s;
                });
            });
        }

        private void btn假人填充_Click(object sender, EventArgs e)
        {
            this.假人_在游戏线程(delegate
            {
                假人网关.立即填满();
                string s = 假人网关.状态文本();
                this.假人_在UI线程(delegate
                {
                    this.lbl假人状态.Text = s;
                });
            });
        }

        private void btn假人下线_Click(object sender, EventArgs e)
        {
            this.假人_在游戏线程(delegate
            {
                假人网关.全部下线();
                string s = 假人网关.状态文本();
                this.假人_在UI线程(delegate
                {
                    this.lbl假人状态.Text = s;
                });
            });
        }

        private void btn假人刷新_Click(object sender, EventArgs e)
        {
            this.假人_刷新状态();
        }

        private void 假人_刷新状态()
        {
            this.假人_在游戏线程(delegate
            {
                string s = 假人网关.状态文本();
                this.假人_在UI线程(delegate
                {
                    this.lbl假人状态.Text = s;
                });
            });
        }
    }
}
