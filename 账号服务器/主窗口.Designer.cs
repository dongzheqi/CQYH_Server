namespace 账号服务器
{
    public partial class 主窗口 : global::System.Windows.Forms.Form
    {
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(主窗口));
            主选项卡 = new System.Windows.Forms.TabControl();
            日志选项卡 = new System.Windows.Forms.TabPage();
            日志文本框 = new System.Windows.Forms.RichTextBox();
            启动服务按钮 = new System.Windows.Forms.Button();
            停止服务按钮 = new System.Windows.Forms.Button();
            已注册账号 = new System.Windows.Forms.Label();
            新注册账号 = new System.Windows.Forms.Label();
            生成门票数 = new System.Windows.Forms.Label();
            已发送字节 = new System.Windows.Forms.Label();
            已接收字节 = new System.Windows.Forms.Label();
            本地监听端口 = new System.Windows.Forms.NumericUpDown();
            本地监听端口标签 = new System.Windows.Forms.Label();
            门票发送端口标签 = new System.Windows.Forms.Label();
            门票发送端口 = new System.Windows.Forms.NumericUpDown();
            最小化到托盘 = new System.Windows.Forms.NotifyIcon(components);
            托盘右键菜单 = new System.Windows.Forms.ContextMenuStrip(components);
            打开ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            打开配置按钮 = new System.Windows.Forms.Button();
            查看账号按钮 = new System.Windows.Forms.Button();
            加载配置按钮 = new System.Windows.Forms.Button();
            加载账号按钮 = new System.Windows.Forms.Button();
            主选项卡.SuspendLayout();
            日志选项卡.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)本地监听端口).BeginInit();
            ((System.ComponentModel.ISupportInitialize)门票发送端口).BeginInit();
            托盘右键菜单.SuspendLayout();
            SuspendLayout();
            // 
            // 主选项卡
            // 
            主选项卡.Controls.Add(日志选项卡);
            主选项卡.ItemSize = new System.Drawing.Size(535, 22);
            主选项卡.Location = new System.Drawing.Point(0, 42);
            主选项卡.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            主选项卡.Name = "主选项卡";
            主选项卡.SelectedIndex = 0;
            主选项卡.Size = new System.Drawing.Size(630, 568);
            主选项卡.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            主选项卡.TabIndex = 0;
            // 
            // 日志选项卡
            // 
            日志选项卡.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            日志选项卡.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            日志选项卡.Controls.Add(日志文本框);
            日志选项卡.Location = new System.Drawing.Point(4, 26);
            日志选项卡.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            日志选项卡.Name = "日志选项卡";
            日志选项卡.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            日志选项卡.Size = new System.Drawing.Size(622, 538);
            日志选项卡.TabIndex = 0;
            日志选项卡.Text = "日志";
            // 
            // 日志文本框
            // 
            日志文本框.BackColor = System.Drawing.Color.Gainsboro;
            日志文本框.BorderStyle = System.Windows.Forms.BorderStyle.None;
            日志文本框.Dock = System.Windows.Forms.DockStyle.Fill;
            日志文本框.Location = new System.Drawing.Point(4, 3);
            日志文本框.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            日志文本框.Name = "日志文本框";
            日志文本框.ReadOnly = true;
            日志文本框.Size = new System.Drawing.Size(610, 528);
            日志文本框.TabIndex = 0;
            日志文本框.Text = "";
            // 
            // 启动服务按钮
            // 
            启动服务按钮.BackColor = System.Drawing.Color.YellowGreen;
            启动服务按钮.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold);
            启动服务按钮.Location = new System.Drawing.Point(632, 405);
            启动服务按钮.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            启动服务按钮.Name = "启动服务按钮";
            启动服务按钮.Size = new System.Drawing.Size(152, 103);
            启动服务按钮.TabIndex = 1;
            启动服务按钮.Text = "启动服务";
            启动服务按钮.UseVisualStyleBackColor = false;
            启动服务按钮.Click += 启动服务_Click;
            // 
            // 停止服务按钮
            // 
            停止服务按钮.BackColor = System.Drawing.Color.Crimson;
            停止服务按钮.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold);
            停止服务按钮.Location = new System.Drawing.Point(631, 507);
            停止服务按钮.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            停止服务按钮.Name = "停止服务按钮";
            停止服务按钮.Size = new System.Drawing.Size(152, 103);
            停止服务按钮.TabIndex = 2;
            停止服务按钮.Text = "停止服务";
            停止服务按钮.UseVisualStyleBackColor = false;
            停止服务按钮.Click += 停止服务_Click;
            // 
            // 已注册账号
            // 
            已注册账号.AutoSize = true;
            已注册账号.Location = new System.Drawing.Point(637, 57);
            已注册账号.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            已注册账号.Name = "已注册账号";
            已注册账号.Size = new System.Drawing.Size(82, 17);
            已注册账号.TabIndex = 3;
            已注册账号.Text = "已注册账号: 0";
            // 
            // 新注册账号
            // 
            新注册账号.AutoSize = true;
            新注册账号.Location = new System.Drawing.Point(637, 89);
            新注册账号.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            新注册账号.Name = "新注册账号";
            新注册账号.Size = new System.Drawing.Size(82, 17);
            新注册账号.TabIndex = 4;
            新注册账号.Text = "新注册账号: 0";
            // 
            // 生成门票数
            // 
            生成门票数.AutoSize = true;
            生成门票数.Location = new System.Drawing.Point(637, 122);
            生成门票数.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            生成门票数.Name = "生成门票数";
            生成门票数.Size = new System.Drawing.Size(82, 17);
            生成门票数.TabIndex = 5;
            生成门票数.Text = "生成门票数: 0";
            // 
            // 已发送字节
            // 
            已发送字节.AutoSize = true;
            已发送字节.Location = new System.Drawing.Point(637, 154);
            已发送字节.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            已发送字节.Name = "已发送字节";
            已发送字节.Size = new System.Drawing.Size(82, 17);
            已发送字节.TabIndex = 6;
            已发送字节.Text = "已发送字节: 0";
            // 
            // 已接收字节
            // 
            已接收字节.AutoSize = true;
            已接收字节.Location = new System.Drawing.Point(637, 187);
            已接收字节.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            已接收字节.Name = "已接收字节";
            已接收字节.Size = new System.Drawing.Size(82, 17);
            已接收字节.TabIndex = 7;
            已接收字节.Text = "已接收字节: 0";
            // 
            // 本地监听端口
            // 
            本地监听端口.Location = new System.Drawing.Point(105, 6);
            本地监听端口.Margin = new System.Windows.Forms.Padding(4);
            本地监听端口.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            本地监听端口.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            本地监听端口.Name = "本地监听端口";
            本地监听端口.Size = new System.Drawing.Size(102, 23);
            本地监听端口.TabIndex = 8;
            本地监听端口.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            本地监听端口.Value = new decimal(new int[] { 8001, 0, 0, 0 });
            // 
            // 本地监听端口标签
            // 
            本地监听端口标签.AutoSize = true;
            本地监听端口标签.Location = new System.Drawing.Point(8, 13);
            本地监听端口标签.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            本地监听端口标签.Name = "本地监听端口标签";
            本地监听端口标签.Size = new System.Drawing.Size(80, 17);
            本地监听端口标签.TabIndex = 9;
            本地监听端口标签.Text = "本地监听端口";
            // 
            // 门票发送端口标签
            // 
            门票发送端口标签.AutoSize = true;
            门票发送端口标签.Location = new System.Drawing.Point(230, 13);
            门票发送端口标签.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            门票发送端口标签.Name = "门票发送端口标签";
            门票发送端口标签.Size = new System.Drawing.Size(80, 17);
            门票发送端口标签.TabIndex = 11;
            门票发送端口标签.Text = "门票发送端口";
            // 
            // 门票发送端口
            // 
            门票发送端口.Location = new System.Drawing.Point(327, 6);
            门票发送端口.Margin = new System.Windows.Forms.Padding(4);
            门票发送端口.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            门票发送端口.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            门票发送端口.Name = "门票发送端口";
            门票发送端口.Size = new System.Drawing.Size(102, 23);
            门票发送端口.TabIndex = 10;
            门票发送端口.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            门票发送端口.Value = new decimal(new int[] { 6678, 0, 0, 0 });
            // 
            // 最小化到托盘
            // 
            最小化到托盘.ContextMenuStrip = 托盘右键菜单;
            最小化到托盘.Icon = (System.Drawing.Icon)resources.GetObject("最小化到托盘.Icon");
            最小化到托盘.Text = "账号服务器";
            最小化到托盘.MouseClick += 恢复窗口_Click;
            // 
            // 托盘右键菜单
            // 
            托盘右键菜单.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { 打开ToolStripMenuItem, 退出ToolStripMenuItem });
            托盘右键菜单.Name = "托盘右键菜单";
            托盘右键菜单.Size = new System.Drawing.Size(101, 48);
            // 
            // 打开ToolStripMenuItem
            // 
            打开ToolStripMenuItem.Name = "打开ToolStripMenuItem";
            打开ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            打开ToolStripMenuItem.Text = "打开";
            打开ToolStripMenuItem.Click += 恢复窗口_Click;
            // 
            // 退出ToolStripMenuItem
            // 
            退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            退出ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            退出ToolStripMenuItem.Text = "退出";
            退出ToolStripMenuItem.Click += 结束进程_Click;
            // 
            // 打开配置按钮
            // 
            打开配置按钮.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
            打开配置按钮.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold);
            打开配置按钮.Location = new System.Drawing.Point(631, 224);
            打开配置按钮.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            打开配置按钮.Name = "打开配置按钮";
            打开配置按钮.Size = new System.Drawing.Size(152, 42);
            打开配置按钮.TabIndex = 12;
            打开配置按钮.Text = "打开服务器配置";
            打开配置按钮.UseVisualStyleBackColor = false;
            打开配置按钮.Click += 打开配置按钮_Click;
            // 
            // 查看账号按钮
            // 
            查看账号按钮.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
            查看账号按钮.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold);
            查看账号按钮.Location = new System.Drawing.Point(631, 314);
            查看账号按钮.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            查看账号按钮.Name = "查看账号按钮";
            查看账号按钮.Size = new System.Drawing.Size(152, 42);
            查看账号按钮.TabIndex = 13;
            查看账号按钮.Text = "打开账号文件夹";
            查看账号按钮.UseVisualStyleBackColor = false;
            查看账号按钮.Click += 查看账号按钮_Click;
            // 
            // 加载配置按钮
            // 
            加载配置按钮.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
            加载配置按钮.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold);
            加载配置按钮.Location = new System.Drawing.Point(631, 269);
            加载配置按钮.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            加载配置按钮.Name = "加载配置按钮";
            加载配置按钮.Size = new System.Drawing.Size(152, 42);
            加载配置按钮.TabIndex = 14;
            加载配置按钮.Text = "加载服务器配置";
            加载配置按钮.UseVisualStyleBackColor = false;
            加载配置按钮.Click += 加载配置按钮_Click;
            // 
            // 加载账号按钮
            // 
            加载账号按钮.BackColor = System.Drawing.Color.FromArgb(192, 255, 192);
            加载账号按钮.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold);
            加载账号按钮.Location = new System.Drawing.Point(632, 360);
            加载账号按钮.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            加载账号按钮.Name = "加载账号按钮";
            加载账号按钮.Size = new System.Drawing.Size(152, 42);
            加载账号按钮.TabIndex = 15;
            加载账号按钮.Text = "加载账号文件夹";
            加载账号按钮.UseVisualStyleBackColor = false;
            加载账号按钮.Click += 加载账号按钮_Click;
            // 
            // 主窗口
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(797, 616);
            Controls.Add(加载账号按钮);
            Controls.Add(加载配置按钮);
            Controls.Add(查看账号按钮);
            Controls.Add(打开配置按钮);
            Controls.Add(门票发送端口标签);
            Controls.Add(门票发送端口);
            Controls.Add(本地监听端口标签);
            Controls.Add(本地监听端口);
            Controls.Add(已接收字节);
            Controls.Add(已发送字节);
            Controls.Add(生成门票数);
            Controls.Add(新注册账号);
            Controls.Add(已注册账号);
            Controls.Add(停止服务按钮);
            Controls.Add(启动服务按钮);
            Controls.Add(主选项卡);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "主窗口";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Elaina Engine - 账号服务器";
            FormClosing += 隐藏窗口_Click;
            Load += 主窗口_Load;
            主选项卡.ResumeLayout(false);
            日志选项卡.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)本地监听端口).EndInit();
            ((System.ComponentModel.ISupportInitialize)门票发送端口).EndInit();
            托盘右键菜单.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private global::System.ComponentModel.IContainer components;


        private global::System.Windows.Forms.TabControl 主选项卡;


        private global::System.Windows.Forms.Button 启动服务按钮;


        private global::System.Windows.Forms.Button 停止服务按钮;


        private global::System.Windows.Forms.TabPage 日志选项卡;

        public global::System.Windows.Forms.RichTextBox 日志文本框;


        private global::System.Windows.Forms.Label 已注册账号;


        private global::System.Windows.Forms.Label 新注册账号;


        private global::System.Windows.Forms.Label 生成门票数;


        private global::System.Windows.Forms.Label 已发送字节;


        private global::System.Windows.Forms.Label 已接收字节;


        private global::System.Windows.Forms.Label 本地监听端口标签;


        private global::System.Windows.Forms.Label 门票发送端口标签;

        public global::System.Windows.Forms.NumericUpDown 本地监听端口;


        public global::System.Windows.Forms.NumericUpDown 门票发送端口;


        private global::System.Windows.Forms.NotifyIcon 最小化到托盘;

        private global::System.Windows.Forms.ContextMenuStrip 托盘右键菜单;


        private global::System.Windows.Forms.ToolStripMenuItem 打开ToolStripMenuItem;


        private global::System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;


        private global::System.Windows.Forms.Button 打开配置按钮;


        private global::System.Windows.Forms.Button 查看账号按钮;


        private global::System.Windows.Forms.Button 加载配置按钮;


        private global::System.Windows.Forms.Button 加载账号按钮;
    }
}
