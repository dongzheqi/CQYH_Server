using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace 游戏服务器
{
    internal static class Program
    {
        // 持有进程生命周期、防同目录重复启动的单实例锁; 必须存为静态字段, 否则被 GC 回收即释放锁.
        private static Mutex 单实例锁;

        [STAThread]
        private static void Main(string[] str1)
        {
            if (!Program.尝试取得单实例锁())
            {
                MessageBox.Show("本目录的游戏服务器已在运行, 不能在同一目录重复启动 —— 两个进程同时写同一份客户数据(Data.db)会导致存档损坏。\r\n如需多开, 请把整个服务端复制到不同目录分别运行。", "重复启动已被阻止", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.SetCompatibleTextRenderingDefault(defaultValue: false);
            Application.ThreadException += _0010_0013_0005_0007_0001_0009;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += _0001_0003_0009_000B_000D_0008_000E_000D;
            Settings.Load();
            if (str1.Length != 0 && str1[0] == "old")
            {
                主程.OldForm = true;
                Application.Run(new 主窗口());
            }
            else
            {
                Application.Run(new SMain());
            }
        }

        // 防同一目录重复启动: 以运行目录哈希为键的全局命名 Mutex —— 不同目录键不同、互不阻塞(允许多目录多开),
        // 同一目录的第二个实例取不到锁即被拦下。进程退出(含崩溃)由 OS 自动释放命名对象, 不会卡死下次启动。
        // 取锁机制本身一旦异常(如 Global 命名空间 ACL 问题), 一律放行启动 —— 宁可不拦也别因锁机制本身打不开服。
        private static bool 尝试取得单实例锁()
        {
            try
            {
                byte[] 哈希;
                哈希 = System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\').ToLowerInvariant()));
                Program.单实例锁 = new Mutex(initiallyOwned: true, "Global\\YH_GameServer_Instance_" + Convert.ToHexString(哈希), out var createdNew);
                return createdNew;
            }
            catch
            {
                return true;
            }
        }

        private static void _0010_0013_0005_0007_0001_0009(object _000C_000A_000E_0008_0003_000A_0002_0003, ThreadExceptionEventArgs _0005_0008_0005_000B_0009_0003_000B_0002)
        {
            Program._0005_0007_0012_0013_0006_0007_0003_0003("Form1_UIThreadException:\r\n" + _0005_0008_0005_000B_0009_0003_000B_0002.Exception.ToString() + "\r\n");
            MessageBox.Show("An application error occurred.\r\n" + _0005_0008_0005_000B_0009_0003_000B_0002.Exception.ToString(), "UIThreadException", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        private static void _0001_0003_0009_000B_000D_0008_000E_000D(object _000C_000A_000E_0008_0003_000A_0002_0003, UnhandledExceptionEventArgs _000F_0004_000E_0003_0014_000A)
        {
            string text;
            text = _000F_0004_000E_0003_0014_000A.ExceptionObject.ToString();
            Program._0005_0007_0012_0013_0006_0007_0003_0003("CurrentDomain_UnhandledException:\r\n" + text + "\r\n");
            MessageBox.Show("An application error occurred.Terminating=" + _000F_0004_000E_0003_0014_000A.IsTerminating + "\r\n" + text, "UnhandledException", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        private static void _0005_0007_0012_0013_0006_0007_0003_0003(string _000F_0011_0001_000F_000C_0012)
        {
            try
            {
                string text;
                text = Path.Combine(Application.StartupPath, "Logs");
                if (!Directory.Exists(text))
                {
                    Directory.CreateDirectory(text);
                }
                string path;
                path = Path.Combine(text, "unhandleEx.txt");
                if (!File.Exists(path))
                {
                    File.WriteAllText(path, "");
                }
                File.AppendAllText(path, _000F_0011_0001_000F_000C_0012);
            }
            catch (Exception)
            {
            }
        }
    }
}
