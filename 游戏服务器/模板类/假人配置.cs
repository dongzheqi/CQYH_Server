using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using 游戏服务器.数据类;

namespace 游戏服务器.模板类
{
    /// <summary>
    /// 单张地图的假人配额。控制该图常驻多少假人、是否打怪、是否 PK。
    /// </summary>
    public class 每图配额
    {
        // 用自动属性(而非字段): DevExpress GridControl 绑定靠 PropertyDescriptor, 字段不会生成列。
        public ushort 地图编号 { get; set; }

        public int 在线数量 { get; set; }

        public bool 是否打怪 { get; set; }

        public bool 是否PK { get; set; }
    }

    /// <summary>
    /// 假人系统全局配置 + 每图配额。从 System\世界其他\ 下三个文件载入:
    /// 假人配置.csv(键,值)、假人地图配额.csv(表)、假人喊话.txt(每行一句)。
    /// 缺文件 = 功能默认关闭(不影响现网); 有文件但格式错 = 响亮报错(对齐仓库"报错优于静默默认"哲学)。
    /// </summary>
    public class 假人配置
    {
        // —— 全局开关与规模 ——
        public bool 启用;

        public int 假人总数;

        public int 最小等级 = 35;

        public int 最大等级 = 45;

        public string 名字前缀 = "游侠";

        // —— 行为节奏 ——
        public int 上下线间隔毫秒 = 3000;

        public int 喊话间隔秒 = 60;

        public int 喝药血量百分比 = 50;

        public int 喝药物品编号;

        public int 巡逻间隔毫秒 = 2000;

        // —— 功能开关 ——
        public bool 开启喊话;

        public bool 开启摆摊;

        public bool 开启PK;

        public bool 真实掉落 = true;

        // —— 内容池 ——
        public List<string> 喊话内容 = new List<string>();

        public List<每图配额> 地图配额 = new List<每图配额>();

        private static string 目录 => Settings.游戏数据目录 + "\\System\\世界其他\\";

        public static 假人配置 载入数据()
        {
            假人配置 配置 = new 假人配置();
            配置.载入全局();
            配置.载入地图配额();
            配置.载入喊话内容();
            return 配置;
        }

        /// <summary>把当前配置写回三个文件(供 GM 图形面板「保存」)。UTF-8 无 BOM, 与 配置读取.打开 兼容。</summary>
        public void 保存数据()
        {
            UTF8Encoding 编码 = new UTF8Encoding(false);
            Directory.CreateDirectory(目录); // 目录不存在时先建, 否则 StreamWriter 抛"找不到路径的一部分"致保存失败
            using (StreamWriter w = new StreamWriter(目录 + "假人配置.csv", append: false, 编码))
            {
                w.WriteLine("# 假人系统全局配置(键,值)。改后服务端 GM 框 @假人 重载 即时生效, 改\"假人总数\"需重启。");
                w.WriteLine($"启用,{(this.启用 ? 1 : 0)}");
                w.WriteLine($"假人总数,{this.假人总数}");
                w.WriteLine($"最小等级,{this.最小等级}");
                w.WriteLine($"最大等级,{this.最大等级}");
                w.WriteLine($"名字前缀,{this.名字前缀}");
                w.WriteLine($"上下线间隔毫秒,{this.上下线间隔毫秒}");
                w.WriteLine($"巡逻间隔毫秒,{this.巡逻间隔毫秒}");
                w.WriteLine($"喊话间隔秒,{this.喊话间隔秒}");
                w.WriteLine($"开启喊话,{(this.开启喊话 ? 1 : 0)}");
                w.WriteLine($"开启摆摊,{(this.开启摆摊 ? 1 : 0)}");
                w.WriteLine($"开启PK,{(this.开启PK ? 1 : 0)}");
                w.WriteLine($"喝药血量百分比,{this.喝药血量百分比}");
                w.WriteLine($"真实掉落,{(this.真实掉落 ? 1 : 0)}");
            }
            using (StreamWriter w = new StreamWriter(目录 + "假人地图配额.csv", append: false, 编码))
            {
                w.WriteLine("# 假人每图配额: 地图编号,在线数量,是否打怪,是否PK (1=是 0=否)");
                w.WriteLine("地图编号,在线数量,是否打怪,是否PK");
                foreach (每图配额 q in this.地图配额)
                {
                    w.WriteLine($"{q.地图编号},{q.在线数量},{(q.是否打怪 ? 1 : 0)},{(q.是否PK ? 1 : 0)}");
                }
            }
            using (StreamWriter w = new StreamWriter(目录 + "假人喊话.txt", append: false, 编码))
            {
                w.WriteLine("# 假人喊话内容池, 每行一句");
                foreach (string 句 in this.喊话内容)
                {
                    w.WriteLine(句);
                }
            }
        }

        private void 载入全局()
        {
            string path = 目录 + "假人配置.csv";
            if (!File.Exists(path))
            {
                return;
            }
            int 行号 = 0;
            using StreamReader reader = 配置读取.打开(path);
            string 行;
            while ((行 = reader.ReadLine()) != null)
            {
                行号++;
                string s = 行.Trim();
                if (s.Length == 0 || s.StartsWith("#") || s.StartsWith("//"))
                {
                    continue;
                }
                int idx = s.IndexOf(',');
                if (idx < 0)
                {
                    主程.添加系统日志($"[假人配置] 假人配置.csv 第{行号}行格式错误(应为 键,值): {s}");
                    continue;
                }
                string 键 = s.Substring(0, idx).Trim();
                string 值 = s.Substring(idx + 1).Trim();
                try
                {
                    应用全局(键, 值);
                }
                catch (Exception ex)
                {
                    主程.添加系统日志($"[假人配置] 假人配置.csv 第{行号}行 键[{键}] 值[{值}] 解析失败: {ex.Message}");
                }
            }
        }

        private void 应用全局(string 键, string 值)
        {
            switch (键)
            {
                case "启用": 启用 = 解析bool(值); break;
                case "假人总数": 假人总数 = int.Parse(值); break;
                case "最小等级": 最小等级 = int.Parse(值); break;
                case "最大等级": 最大等级 = int.Parse(值); break;
                case "名字前缀": 名字前缀 = 值; break;
                case "上下线间隔毫秒": 上下线间隔毫秒 = int.Parse(值); break;
                case "喊话间隔秒": 喊话间隔秒 = int.Parse(值); break;
                case "喝药血量百分比": 喝药血量百分比 = int.Parse(值); break;
                case "喝药物品编号": 喝药物品编号 = int.Parse(值); break;
                case "巡逻间隔毫秒": 巡逻间隔毫秒 = int.Parse(值); break;
                case "开启喊话": 开启喊话 = 解析bool(值); break;
                case "开启摆摊": 开启摆摊 = 解析bool(值); break;
                case "开启PK": 开启PK = 解析bool(值); break;
                case "真实掉落": 真实掉落 = 解析bool(值); break;
                default: 主程.添加系统日志($"[假人配置] 未知配置键: {键}"); break;
            }
        }

        private void 载入地图配额()
        {
            string path = 目录 + "假人地图配额.csv";
            if (!File.Exists(path))
            {
                return;
            }
            int 行号 = 0;
            using StreamReader reader = 配置读取.打开(path);
            string 行;
            while ((行 = reader.ReadLine()) != null)
            {
                行号++;
                string s = 行.Trim();
                if (s.Length == 0 || s.StartsWith("#"))
                {
                    continue;
                }
                string[] 列 = s.Split(',');
                if (列[0].Trim() == "地图编号")
                {
                    continue; // 表头
                }
                if (列.Length < 2)
                {
                    主程.添加系统日志($"[假人地图配额] 第{行号}行列数不足(应为 地图编号,在线数量[,是否打怪,是否PK]): {s}");
                    continue;
                }
                try
                {
                    每图配额 配额 = new 每图配额
                    {
                        地图编号 = ushort.Parse(列[0].Trim()),
                        在线数量 = int.Parse(列[1].Trim()),
                        是否打怪 = 列.Length > 2 && 解析bool(列[2].Trim()),
                        是否PK = 列.Length > 3 && 解析bool(列[3].Trim())
                    };
                    地图配额.Add(配额);
                }
                catch (Exception ex)
                {
                    主程.添加系统日志($"[假人地图配额] 第{行号}行解析失败: {ex.Message} <- {s}");
                }
            }
        }

        private void 载入喊话内容()
        {
            string path = 目录 + "假人喊话.txt";
            if (!File.Exists(path))
            {
                return;
            }
            using StreamReader reader = 配置读取.打开(path);
            string 行;
            while ((行 = reader.ReadLine()) != null)
            {
                string s = 行.Trim();
                if (s.Length > 0 && !s.StartsWith("#"))
                {
                    喊话内容.Add(s);
                }
            }
        }

        private static bool 解析bool(string 值)
        {
            return 值 == "1" || 值 == "是" || 值 == "开" || 值.Equals("true", StringComparison.OrdinalIgnoreCase);
        }
    }
}
