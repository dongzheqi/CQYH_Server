using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using _001D_000F_0007_0013_0011_0015;
using 游戏服务器.窗口视图;
using 游戏服务器.地图类;
using 游戏服务器.模板类;
using 游戏服务器.网络类;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MethodInvoker = System.Windows.Forms.MethodInvoker;

namespace 游戏服务器.数据类
{
    public static class 游戏数据网关
    {
        // 串行化"写文件/改结构"的数据操作(导出/整理/清理), 防 GM 管理操作与后台自动保存(保存数据库 → Task.Run(导出数据))
        // 并发写同一批数据文件而损坏. 仅锁后台/管理路径, 不锁主循环每帧调用的内存态 保存数据(), 故不阻塞主循环.
        // lock(Monitor) 可重入: 整理/清理 内部再调 导出数据 不会自死锁.
        private static readonly object 数据锁 = new object();

        // 数据是否已完成首次加载. 供后续管理操作前置判断, 避免在未加载完成时误整理/合并而清空数据.
        public static bool 数据已加载 { get; private set; }

        private static bool 数据修改;

        private static byte[] 表头描述;

        public static 数据表实例<账号数据> 账号数据表;

        public static 数据表实例<角色数据> 角色数据表;

        public static 数据表实例<宠物数据> 宠物数据表;

        public static 数据表实例<物品数据> 物品数据表;

        public static 数据表实例<装备数据> 装备数据表;

        public static 数据表实例<技能数据> 技能数据表;

        public static 数据表实例<Buff数据> Buff数据表;

        public static 数据表实例<队伍数据> 队伍数据表;

        public static 数据表实例<行会数据> 行会数据表;

        public static 数据表实例<师门数据> 师门数据表;

        public static 数据表实例<邮件数据> 邮件数据表;

        public static 数据表实例<龙卫数据> 龙卫数据表;

        public static 数据表实例<CharacterQuest> CharacterQuestDataTable;

        public static 数据表实例<CharacterQuestMission> CharacterQuestConstraintDataTable;

        public static 数据表实例<寄售数据> 寄售数据表;

        public static 数据表实例<AchievementData> 成就数据表;

        public static Dictionary<Type, 数据表基类> 数据类型表;

        public static DateTime 寄售处理;

        public static bool 已经修改
        {
            get
            {
                return 游戏数据网关.数据修改;
            }
            set
            {
                游戏数据网关.数据修改 = value;
                if (游戏数据网关.数据修改 && !主程.已经启动 && (主程.主线程 == null || !主程.主线程.IsAlive))
                {
                    主窗口.主界面?.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate
                    {
                        主窗口.主界面.保存按钮.Enabled = true;
                    });
                }
            }
        }

        public static string 数据目录 => Settings.游戏数据目录 + "\\User";

        public static string 备份目录 => Settings.数据备份目录;

        public static string 数据文件 => Settings.游戏数据目录 + "\\User\\Data.db";

        public static string 缓存文件 => Settings.游戏数据目录 + "\\User\\Temp.db";

        public static string 备份文件 => $"{Settings.数据备份目录}\\User-{主程.当前时间:yyyy-MM-dd-HH-mm-ss-ffff}.db.gz";

        public static void 加载数据()
        {
            游戏数据网关.数据类型表 = new Dictionary<Type, 数据表基类>();
            Type[] types;
            types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type type in types)
            {
                if (type.IsSubclassOf(typeof(游戏数据)))
                {
                    游戏数据网关.数据类型表[type] = (数据表基类)Activator.CreateInstance(typeof(数据表实例<>).MakeGenericType(type));
                }
            }
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                binaryWriter.Write(游戏数据网关.数据类型表.Count);
                foreach (KeyValuePair<Type, 数据表基类> item in 游戏数据网关.数据类型表)
                {
                    item.Value.当前映射.保存映射描述(binaryWriter);
                }
                游戏数据网关.表头描述 = memoryStream.ToArray();
            }
            if (File.Exists(游戏数据网关.数据文件))
            {
                using BinaryReader binaryReader = new BinaryReader(File.OpenRead(游戏数据网关.数据文件));
                List<数据映射> list;
                list = new List<数据映射>();
                int num;
                num = binaryReader.ReadInt32();
                for (int j = 0; j < num; j++)
                {
                    list.Add(new 数据映射(binaryReader));
                }
                List<Task> list2;
                list2 = new List<Task>();
                foreach (数据映射 当前历史映射 in list)
                {
                    byte[] 历史映射数据;
                    历史映射数据 = binaryReader.ReadBytes(binaryReader.ReadInt32());
                    if (!(当前历史映射.数据类型 == null) && 游戏数据网关.数据类型表.TryGetValue(当前历史映射.数据类型, out var 存表实例))
                    {
                        list2.Add(Task.Run(delegate
                        {
                            存表实例.加载数据(历史映射数据, 当前历史映射);
                        }));
                    }
                }
                if (list2.Count > 0)
                {
                    Task.WaitAll(list2.ToArray());
                }
            }
            if (游戏数据网关.数据类型表[typeof(系统数据)].数据表.Count == 0)
            {
                new 系统数据(1);
            }
            游戏数据网关.账号数据表 = 游戏数据网关.数据类型表[typeof(账号数据)] as 数据表实例<账号数据>;
            游戏数据网关.角色数据表 = 游戏数据网关.数据类型表[typeof(角色数据)] as 数据表实例<角色数据>;
            游戏数据网关.宠物数据表 = 游戏数据网关.数据类型表[typeof(宠物数据)] as 数据表实例<宠物数据>;
            游戏数据网关.物品数据表 = 游戏数据网关.数据类型表[typeof(物品数据)] as 数据表实例<物品数据>;
            游戏数据网关.装备数据表 = 游戏数据网关.数据类型表[typeof(装备数据)] as 数据表实例<装备数据>;
            游戏数据网关.技能数据表 = 游戏数据网关.数据类型表[typeof(技能数据)] as 数据表实例<技能数据>;
            游戏数据网关.Buff数据表 = 游戏数据网关.数据类型表[typeof(Buff数据)] as 数据表实例<Buff数据>;
            游戏数据网关.队伍数据表 = 游戏数据网关.数据类型表[typeof(队伍数据)] as 数据表实例<队伍数据>;
            游戏数据网关.行会数据表 = 游戏数据网关.数据类型表[typeof(行会数据)] as 数据表实例<行会数据>;
            游戏数据网关.师门数据表 = 游戏数据网关.数据类型表[typeof(师门数据)] as 数据表实例<师门数据>;
            游戏数据网关.邮件数据表 = 游戏数据网关.数据类型表[typeof(邮件数据)] as 数据表实例<邮件数据>;
            游戏数据网关.龙卫数据表 = 游戏数据网关.数据类型表[typeof(龙卫数据)] as 数据表实例<龙卫数据>;
            游戏数据网关.CharacterQuestDataTable = 游戏数据网关.数据类型表[typeof(CharacterQuest)] as 数据表实例<CharacterQuest>;
            游戏数据网关.CharacterQuestConstraintDataTable = 游戏数据网关.数据类型表[typeof(CharacterQuestMission)] as 数据表实例<CharacterQuestMission>;
            游戏数据网关.寄售数据表 = 游戏数据网关.数据类型表[typeof(寄售数据)] as 数据表实例<寄售数据>;
            游戏数据网关.成就数据表 = 游戏数据网关.数据类型表[typeof(AchievementData)] as 数据表实例<AchievementData>;
            数据关联表.处理任务();
            foreach (KeyValuePair<int, 游戏数据> item2 in 游戏数据网关.角色数据表.数据表)
            {
                item2.Value.加载完成();
            }
            系统数据.数据.加载完成();
            游戏数据网关.数据已加载 = true;
        }

        public static void 保存数据()
        {
            Parallel.ForEach(游戏数据网关.数据类型表.Values, delegate (数据表基类 x)
            {
                x.保存数据();
            });
        }

        public static void 强制保存()
        {
            Parallel.ForEach(游戏数据网关.数据类型表.Values, delegate (数据表基类 x)
            {
                x.强制保存();
            });
        }

        public static void 强制保存_线程内()
        {
            foreach (数据表基类 value in 游戏数据网关.数据类型表.Values)
            {
                value.强制保存();
            }
        }

        public static void 导出数据()
        {
            lock (游戏数据网关.数据锁)
            {
            if (!Directory.Exists(游戏数据网关.数据目录))
            {
                Directory.CreateDirectory(游戏数据网关.数据目录);
            }
            using (BinaryWriter binaryWriter = new BinaryWriter(File.Create(游戏数据网关.缓存文件)))
            {
                binaryWriter.Write(游戏数据网关.表头描述);
                foreach (KeyValuePair<Type, 数据表基类> item in 游戏数据网关.数据类型表)
                {
                    byte[] array;
                    array = item.Value.存表数据();
                    binaryWriter.Write(array.Length);
                    binaryWriter.Write(array);
                }
            }
            if (!Directory.Exists(Settings.数据备份目录))
            {
                Directory.CreateDirectory(Settings.数据备份目录);
            }
            if (File.Exists(游戏数据网关.数据文件))
            {
                using (FileStream fileStream = File.OpenRead(游戏数据网关.数据文件))
                {
                    using FileStream stream = File.Create(游戏数据网关.备份文件);
                    using GZipStream destination = new GZipStream(stream, CompressionMode.Compress);
                    fileStream.CopyTo(destination);
                }
                File.Delete(游戏数据网关.数据文件);
            }
            File.Move(游戏数据网关.缓存文件, 游戏数据网关.数据文件);
            游戏数据网关.清理旧备份(30);
            if (游戏数据网关.已经修改)
            {
                游戏数据网关.已经修改 = false;
            }
            }
        }

        // 滚动清理备份目录: 仅保留最近 保留份数 个 User-*.db.gz, 防自动保存产生的历史备份无限堆积撑爆磁盘.
        // 在 导出数据 写完新备份后调用; 任何异常都吞掉, 备份清理失败不得影响主存档落盘.
        private static void 清理旧备份(int 保留份数)
        {
            try
            {
                if (!Directory.Exists(Settings.数据备份目录))
                {
                    return;
                }
                FileInfo[] 备份列表;
                备份列表 = new DirectoryInfo(Settings.数据备份目录).GetFiles("User-*.db.gz");
                if (备份列表.Length <= 保留份数)
                {
                    return;
                }
                Array.Sort(备份列表, (FileInfo a, FileInfo b) => b.LastWriteTimeUtc.CompareTo(a.LastWriteTimeUtc));
                for (int i = 保留份数; i < 备份列表.Length; i++)
                {
                    try
                    {
                        备份列表[i].Delete();
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                主程.添加系统日志("清理旧备份失败: " + ex.Message);
            }
        }

        public static void 整理数据(bool 保存数据)
        {
            lock (游戏数据网关.数据锁)
            {
            int num;
            num = 0;
            foreach (KeyValuePair<Type, 数据表基类> item in 游戏数据网关.数据类型表)
            {
                int num2;
                num2 = 0;
                if (item.Value.数据类型 == typeof(行会数据))
                {
                    num2 = 1610612736;
                }
                if (item.Value.数据类型 == typeof(队伍数据))
                {
                    num2 = 1879048192;
                }
                List<游戏数据> list;
                list = item.Value.数据表.Values.OrderBy((游戏数据 O) => O.数据索引.V).ToList();
                int num3;
                num3 = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    int num4;
                    num4 = num2 + i + 1;
                    游戏数据 游戏数据2;
                    游戏数据2 = list[i];
                    if (游戏数据2.数据索引.V == num4)
                    {
                        continue;
                    }
                    if (游戏数据2 is 角色数据)
                    {
                        foreach (KeyValuePair<int, 游戏数据> item2 in 游戏数据网关.行会数据表.数据表)
                        {
                            foreach (行会事记 item3 in ((行会数据)item2.Value).行会事记)
                            {
                                switch (item3.事记类型)
                                {
                                    case 事记类型.创建公会:
                                    case 事记类型.加入公会:
                                    case 事记类型.离开公会:
                                        if (item3.第一参数 == 游戏数据2.数据索引.V)
                                        {
                                            item3.第一参数 = num4;
                                        }
                                        break;
                                    case 事记类型.逐出公会:
                                    case 事记类型.变更职位:
                                    case 事记类型.会长传位:
                                        if (item3.第一参数 == 游戏数据2.数据索引.V)
                                        {
                                            item3.第一参数 = num4;
                                        }
                                        if (item3.第二参数 == 游戏数据2.数据索引.V)
                                        {
                                            item3.第二参数 = num4;
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    else if (游戏数据2 is 行会数据)
                    {
                        foreach (KeyValuePair<int, 游戏数据> item4 in 游戏数据网关.行会数据表.数据表)
                        {
                            foreach (行会事记 item5 in ((行会数据)item4.Value).行会事记)
                            {
                                事记类型 事记类型2;
                                事记类型2 = item5.事记类型;
                                if ((uint)(事记类型2 - 9) <= 1u || (uint)(事记类型2 - 21) <= 1u)
                                {
                                    if (item5.第一参数 == 游戏数据2.数据索引.V)
                                    {
                                        item5.第一参数 = num4;
                                    }
                                    if (item5.第二参数 == 游戏数据2.数据索引.V)
                                    {
                                        item5.第二参数 = num4;
                                    }
                                }
                            }
                        }
                    }
                    游戏数据2.数据索引.V = num4;
                    num3++;
                }
                item.Value.当前索引 = list.Count + num2;
                num += num3;
                item.Value.数据表 = item.Value.数据表.ToDictionary((KeyValuePair<int, 游戏数据> x) => x.Value.数据索引.V, (KeyValuePair<int, 游戏数据> x) => x.Value);
                if (num3 > 0)
                {
                    主程.添加命令日志($"{item.Key.Name}已经整理完毕, 整理数量:{num3}");
                }
            }
            if (num > 0)
            {
                主程.添加命令日志($"客户数据已经整理完毕, 整理总数:{num}");
            }
            if (num > 0 && 保存数据)
            {
                主程.添加命令日志("正在重新保存整理后的客户数据, 可能花费较长时间, 请稍后...");
                游戏数据网关.强制保存();
                游戏数据网关.导出数据();
                主程.添加命令日志("数据已经保存到磁盘");
                MessageBox.Show("客户数据已经整理完毕, 应用程序需要重启");
                Environment.Exit(0);
            }
            }
        }

        public static void 清理角色(int 限制等级, int 限制天数)
        {
            lock (游戏数据网关.数据锁)
            {
            主程.添加命令日志("开始清理角色数据...");
            DateTime dateTime;
            dateTime = DateTime.Now.AddDays(-限制天数);
            int num;
            num = 0;
            foreach (游戏数据 item in 游戏数据网关.角色数据表.数据表.Values.ToList())
            {
                if (item is 角色数据 角色数据2 && 角色数据2.当前等级.V < 限制等级 && !(角色数据2.离线日期.V > dateTime))
                {
                    if (角色数据2.当前排名.Count > 0)
                    {
                        主程.添加命令日志($"[{角色数据2}]({角色数据2.当前等级}/{(int)(DateTime.Now - 角色数据2.离线日期.V).TotalDays}) 在排行榜单上, 已跳过清理");
                    }
                    else if (角色数据2.元宝数量 != 0)
                    {
                        主程.添加命令日志($"[{角色数据2}]({角色数据2.当前等级}/{(int)(DateTime.Now - 角色数据2.离线日期.V).TotalDays}) 有未消费元宝, 已跳过清理");
                    }
                    else if (角色数据2.当前行会?.会长数据 == 角色数据2)
                    {
                        主程.添加命令日志($"[{角色数据2}]({角色数据2.当前等级}/{(int)(DateTime.Now - 角色数据2.离线日期.V).TotalDays}) 是行会的会长, 已跳过清理");
                    }
                    else
                    {
                        主程.添加命令日志($"开始清理[{角色数据2}]({角色数据2.当前等级}/{(int)(DateTime.Now - 角色数据2.离线日期.V).TotalDays})...");
                        角色数据2.删除数据();
                        num++;
                        主窗口.移除角色数据(角色数据2);
                    }
                }
            }
            主程.添加命令日志($"角色数据已经清理完成, 清理总数:{num}");
            if (num > 0)
            {
                主程.添加命令日志("正在重新保存清理后的客户数据, 可能花费较长时间, 请稍后...");
                游戏数据网关.保存数据();
                游戏数据网关.导出数据();
                游戏数据网关.加载数据();
                主程.添加命令日志("数据已经保存到磁盘");
            }
            }
        }

        public static string ModifyString2(string input)
        {
            int length;
            length = input.Length;
            int num;
            num = length - 1;
            string text;
            text = input;
            while (num >= 0)
            {
                char c;
                c = input[num];
                if (c != 'z' && (char.IsLetter(c) || length >= 24))
                {
                    if (char.IsLetter(c))
                    {
                        char c2;
                        c2 = (char)(c + 1);
                        if (c2 > 'z')
                        {
                            num--;
                            continue;
                        }
                        input = input.Substring(0, num) + c2 + input.Substring(num + 1);
                        break;
                    }
                    input = ((length < 24) ? (input + "a") : (input.Substring(0, num) + "a" + input.Substring(num + 1)));
                    break;
                }
                input += "a";
                break;
            }
            if (text == input)
            {
                return input + "a";
            }
            return input;
        }

        public static string ModifyString(string input, bool firstTime = false)
        {
            List<char> list;
            list = input.ToList();
            if (firstTime && list.Count < 26)
            {
                list.Add('a');
                return new string(list.ToArray());
            }
            char c;
            c = list.Last();
            if (list.Count >= 26 && !Regex.IsMatch(c.ToString(), "[a-zA-Z]"))
            {
                主程.添加系统日志(input + " 账号名字长度超过24个字符,并且,末尾字符将被清除,替换为字母");
                list[list.Count - 1] = 'a';
                return new string(list.ToArray());
            }
            if (!Regex.IsMatch(c.ToString(), "[a-zA-Z]"))
            {
                list.Add('a');
            }
            else if (c != 'z' && c != 'Z')
            {
                list[list.Count - 1] = (char)(c + 1);
            }
            else
            {
                list.Add('a');
            }
            return new string(list.ToArray());
        }

        public static void 合并数据(string 数据文件, string 账号配置路径)
        {
            Control UI;
            UI = 主窗口.主界面;
            if (UI == null)
            {
                UI = SMain.Main;
            }
            UI?.BeginInvoke((MethodInvoker)delegate
            {
                if (UI == 主窗口.主界面)
                {
                    TabPage 设置页面;
                    设置页面 = 主窗口.主界面.设置页面;
                    主窗口.主界面.下方控件页.Enabled = false;
                    设置页面.Enabled = false;
                    主窗口.主界面.主选项卡.SelectedIndex = 0;
                    主窗口.主界面.日志选项卡.SelectedIndex = 2;
                }
                else if (SMain.Main.Loaded)
                {
                    SMain.Main.ShowView(typeof(CommandLogView));
                }
                Dictionary<string, JObject> dictionary;
                dictionary = new Dictionary<string, JObject>();
                if (!Directory.Exists(账号配置路径))
                {
                    MessageBox.Show("账号服务器文件夹不存在");
                }
                else
                {
                    string[] files;
                    files = Directory.GetFiles(账号配置路径);
                    for (int j = 0; j < files.Length; j++)
                    {
                        string value;
                        value = File.ReadAllText(files[j]);
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            JObject jObject;
                            jObject = (JObject)JsonConvert.DeserializeObject(value);
                            if (jObject.ContainsKey("账号名字"))
                            {
                                dictionary.Add(jObject["账号名字"].ToString(), jObject);
                            }
                        }
                    }
                    if (dictionary.Count == 0)
                    {
                        MessageBox.Show("未找到任何账号信息,请选择账号信息文件夹");
                    }
                    else
                    {
                        主程.添加命令日志("开始整理当前客户数据...");
                        游戏数据网关.整理数据(保存数据: false);
                        Dictionary<Type, 数据表基类> dictionary2;
                        dictionary2 = 游戏数据网关.数据类型表;
                        主程.添加命令日志("开始加载指定客户数据...");
                        游戏数据网关.数据类型表 = new Dictionary<Type, 数据表基类>();
                        Type[] types;
                        types = Assembly.GetExecutingAssembly().GetTypes();
                        foreach (Type type in types)
                        {
                            if (type.IsSubclassOf(typeof(游戏数据)))
                            {
                                游戏数据网关.数据类型表[type] = (数据表基类)Activator.CreateInstance(typeof(数据表实例<>).MakeGenericType(type));
                            }
                        }
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
                            binaryWriter.Write(游戏数据网关.数据类型表.Count);
                            foreach (KeyValuePair<Type, 数据表基类> item in 游戏数据网关.数据类型表)
                            {
                                item.Value.当前映射.保存映射描述(binaryWriter);
                            }
                            游戏数据网关.表头描述 = memoryStream.ToArray();
                        }
                        if (File.Exists(数据文件))
                        {
                            using BinaryReader binaryReader = new BinaryReader(File.OpenRead(数据文件));
                            List<数据映射> list;
                            list = new List<数据映射>();
                            int num;
                            num = binaryReader.ReadInt32();
                            for (int l = 0; l < num; l++)
                            {
                                list.Add(new 数据映射(binaryReader));
                            }
                            List<Task> list2;
                            list2 = new List<Task>();
                            foreach (数据映射 当前历史映射 in list)
                            {
                                byte[] 历史映射数据;
                                历史映射数据 = binaryReader.ReadBytes(binaryReader.ReadInt32());
                                if (!(当前历史映射.数据类型 == null) && 游戏数据网关.数据类型表.TryGetValue(当前历史映射.数据类型, out var 存表实例))
                                {
                                    list2.Add(Task.Run(delegate
                                    {
                                        存表实例.加载数据(历史映射数据, 当前历史映射);
                                    }));
                                }
                            }
                            if (list2.Count > 0)
                            {
                                Task.WaitAll(list2.ToArray());
                            }
                        }
                        主程.添加命令日志("开始整理指定客户数据...");
                        数据关联表.处理任务();
                        游戏数据网关.整理数据(保存数据: false);
                        Dictionary<Type, 数据表基类> dictionary3;
                        dictionary3 = 游戏数据网关.数据类型表;
                        Dictionary<string, List<string>> dictionary4;
                        dictionary4 = new Dictionary<string, List<string>>();
                        foreach (KeyValuePair<Type, 数据表基类> item2 in dictionary2)
                        {
                            if (dictionary3.ContainsKey(item2.Key))
                            {
                                if (item2.Key == typeof(账号数据))
                                {
                                    数据表实例<账号数据> 数据表实例2;
                                    数据表实例2 = dictionary2[item2.Key] as 数据表实例<账号数据>;
                                    Dictionary<string, List<账号数据>> dictionary5;
                                    dictionary5 = (from d in (from 账号数据 acc in 数据表实例2.数据表.Values
                                                              group acc by acc.账号名字.V.ToUpperInvariant()).ToDictionary((IGrouping<string, 账号数据> k) => k.Key, (IGrouping<string, 账号数据> v) => v.ToList())
                                                   where d.Value.Count > 1
                                                   select d).ToDictionary((KeyValuePair<string, List<账号数据>> k) => k.Key, (KeyValuePair<string, List<账号数据>> v) => v.Value);
                                    Dictionary<string, List<账号数据>> dictionary6;
                                    dictionary6 = (from d in (from 账号数据 acc in dictionary3[item2.Key].数据表.Values
                                                              group acc by acc.账号名字.V.ToUpperInvariant()).ToDictionary((IGrouping<string, 账号数据> k) => k.Key.ToUpperInvariant(), (IGrouping<string, 账号数据> v) => v.ToList())
                                                   where d.Value.Count > 1
                                                   select d).ToDictionary((KeyValuePair<string, List<账号数据>> k) => k.Key, (KeyValuePair<string, List<账号数据>> v) => v.Value);
                                    foreach (string item3 in dictionary5.Keys.ToList())
                                    {
                                        if (dictionary6.TryGetValue(item3, out var value2))
                                        {
                                            dictionary5[item3].AddRange(value2);
                                        }
                                        bool flag;
                                        flag = false;
                                        foreach (账号数据 i in dictionary5[item3].ToList())
                                        {
                                            if (dictionary.TryGetValue(i.账号名字.V, out var value3))
                                            {
                                                if (!flag)
                                                {
                                                    flag = true;
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                value3 = dictionary.FirstOrDefault((KeyValuePair<string, JObject> acc) => acc.Key.Equals(i.账号名字.V, StringComparison.OrdinalIgnoreCase)).Value;
                                                if (value3 == null)
                                                {
                                                    MessageBox.Show("异常账号,游戏数据有账号,但账号文件完全没有对应文件(不区分大小写)\r\n即将结束合区", "即将结束合区");
                                                    Environment.Exit(0);
                                                    return;
                                                }
                                            }
                                            string NewName;
                                            NewName = 游戏数据网关.ModifyString(i.账号名字.V, firstTime: true);
                                            while (dictionary5[item3].Any((账号数据 a) => a.账号名字.V.Equals(NewName, StringComparison.OrdinalIgnoreCase)) || dictionary.Any((KeyValuePair<string, JObject> a) => a.Key.Equals(NewName, StringComparison.OrdinalIgnoreCase)))
                                            {
                                                NewName = 游戏数据网关.ModifyString(NewName);
                                            }
                                            i.账号名字.V = NewName;
                                            JObject jObject2;
                                            jObject2 = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value3));
                                            jObject2["账号名字"] = NewName;
                                            dictionary.Add(NewName, jObject2);
                                        }
                                    }
                                    foreach (KeyValuePair<int, 游戏数据> item4 in (dictionary3[item2.Key] as 数据表实例<账号数据>).数据表)
                                    {
                                        账号数据 账号数据2;
                                        账号数据2 = item4.Value as 账号数据;
                                        if (数据表实例2.检索表.TryGetValue(账号数据2.账号名字.V, out var value4))
                                        {
                                            账号数据 原始账号;
                                            原始账号 = value4 as 账号数据;
                                            if (原始账号 != null)
                                            {
                                                if (原始账号.账号名字.V == 机器人.默认账号名)
                                                {
                                                    continue;
                                                }
                                                string text;
                                                text = 游戏数据网关.ModifyString(原始账号.账号名字.V, firstTime: true);
                                                while (数据表实例2.检索表.ContainsKey(text) || dictionary.ContainsKey(text))
                                                {
                                                    text = 游戏数据网关.ModifyString(text);
                                                }
                                                if (!dictionary.TryGetValue(原始账号.账号名字.V, out var value5))
                                                {
                                                    value5 = dictionary.FirstOrDefault((KeyValuePair<string, JObject> d) => d.Key.Equals(原始账号.账号名字.V, StringComparison.OrdinalIgnoreCase)).Value;
                                                    if (value5 == null)
                                                    {
                                                        MessageBox.Show("两个区有重复账号 " + 原始账号.账号名字.V + ",但未找到对应的账号文件,请检查!", "即将结束合区");
                                                        Environment.Exit(0);
                                                        return;
                                                    }
                                                }
                                                JObject jObject3;
                                                jObject3 = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(value5));
                                                jObject3["账号名字"] = text;
                                                dictionary.Add(text, jObject3);
                                                if (!dictionary4.ContainsKey("账号合并"))
                                                {
                                                    dictionary4.Add("账号合并", new List<string>());
                                                }
                                                dictionary4["账号合并"].Add("[账号合并]" + 账号数据2.账号名字.V + " 更名为 " + text);
                                                账号数据2.账号名字.V = text;
                                            }
                                        }
                                        item2.Value.添加数据(账号数据2, 分配索引: true);
                                    }
                                }
                                else if (item2.Key == typeof(角色数据))
                                {
                                    数据表实例<角色数据> 数据表实例3;
                                    数据表实例3 = dictionary2[item2.Key] as 数据表实例<角色数据>;
                                    foreach (KeyValuePair<int, 游戏数据> item5 in (dictionary3[item2.Key] as 数据表实例<角色数据>).数据表)
                                    {
                                        角色数据 角色数据2;
                                        角色数据2 = item5.Value as 角色数据;
                                        if (数据表实例3.检索表.TryGetValue(角色数据2.角色名字.V, out var value6) && value6 is 角色数据 角色数据3)
                                        {
                                            string text2;
                                            text2 = 游戏数据网关.ModifyString(角色数据3.角色名字.V, firstTime: true);
                                            while (数据表实例3.检索表.ContainsKey(text2))
                                            {
                                                text2 = 游戏数据网关.ModifyString(text2);
                                            }
                                            foreach (KeyValuePair<int, 游戏数据> item6 in (dictionary3[typeof(行会数据)] as 数据表实例<行会数据>).数据表)
                                            {
                                                if (item6.Value is 行会数据 行会数据2 && 行会数据2.创建人名.V == 角色数据2.角色名字.V)
                                                {
                                                    if (!dictionary4.ContainsKey("角色合并"))
                                                    {
                                                        dictionary4.Add("角色合并", new List<string>());
                                                    }
                                                    dictionary4["角色合并"].Add("[角色合并]行会会长 " + 行会数据2.创建人名.V + " 更名为 " + text2);
                                                    行会数据2.创建人名.V = text2;
                                                }
                                            }
                                            if (!dictionary4.ContainsKey("角色合并"))
                                            {
                                                dictionary4.Add("角色合并", new List<string>());
                                            }
                                            dictionary4["角色合并"].Add("[角色合并]" + 角色数据2.角色名字.V + " 更名位 " + text2);
                                            角色数据2.角色名字.V = text2;
                                        }
                                        item2.Value.添加数据(角色数据2, 分配索引: true);
                                    }
                                }
                                else if (item2.Key == typeof(行会数据))
                                {
                                    数据表实例<行会数据> 数据表实例4;
                                    数据表实例4 = dictionary2[item2.Key] as 数据表实例<行会数据>;
                                    foreach (KeyValuePair<int, 游戏数据> item7 in (dictionary3[item2.Key] as 数据表实例<行会数据>).数据表)
                                    {
                                        行会数据 行会数据3;
                                        行会数据3 = item7.Value as 行会数据;
                                        if (数据表实例4.检索表.TryGetValue(行会数据3.行会名字.V, out var value7) && value7 is 行会数据 行会数据4)
                                        {
                                            string text3;
                                            text3 = 游戏数据网关.ModifyString(行会数据4.行会名字.V, firstTime: true);
                                            while (数据表实例4.检索表.ContainsKey(text3))
                                            {
                                                text3 = 游戏数据网关.ModifyString(text3);
                                            }
                                            if (!dictionary4.ContainsKey("行会合并"))
                                            {
                                                dictionary4.Add("行会合并", new List<string>());
                                            }
                                            dictionary4["行会合并"].Add("[行会合并]" + 行会数据3.行会名字.V + " 更名为 " + text3);
                                            行会数据3.行会名字.V = text3;
                                        }
                                        item2.Value.添加数据(行会数据3, 分配索引: true);
                                    }
                                }
                                else
                                {
                                    foreach (KeyValuePair<int, 游戏数据> item8 in dictionary3[item2.Key].数据表)
                                    {
                                        item2.Value.添加数据(item8.Value, 分配索引: true);
                                    }
                                }
                            }
                        }
                        foreach (KeyValuePair<int, 游戏数据> item9 in dictionary2[typeof(行会数据)].数据表)
                        {
                            ((行会数据)item9.Value).行会排名.V = 0;
                        }
                        foreach (KeyValuePair<int, 游戏数据> item10 in dictionary2[typeof(角色数据)].数据表)
                        {
                            角色数据 obj;
                            obj = (角色数据)item10.Value;
                            obj.历史排名.Clear();
                            obj.当前排名.Clear();
                        }
                        系统数据 obj2;
                        obj2 = dictionary3[typeof(系统数据)].数据表[1] as 系统数据;
                        系统数据 系统数据2;
                        系统数据2 = dictionary2[typeof(系统数据)].数据表[1] as 系统数据;
                        Dictionary<string, DateTime> dictionary7;
                        dictionary7 = obj2.网络封禁.ToDictionary((KeyValuePair<string, DateTime> k) => k.Key, (KeyValuePair<string, DateTime> v) => v.Value);
                        Dictionary<string, DateTime> dictionary8;
                        dictionary8 = 系统数据2.网络封禁.ToDictionary((KeyValuePair<string, DateTime> k) => k.Key, (KeyValuePair<string, DateTime> v) => v.Value);
                        Dictionary<string, DateTime> dictionary9;
                        dictionary9 = obj2.网卡封禁.ToDictionary((KeyValuePair<string, DateTime> k) => k.Key, (KeyValuePair<string, DateTime> v) => v.Value);
                        Dictionary<string, DateTime> dictionary10;
                        dictionary10 = 系统数据2.网卡封禁.ToDictionary((KeyValuePair<string, DateTime> k) => k.Key, (KeyValuePair<string, DateTime> v) => v.Value);
                        obj2.脚本数字.ToDictionary((KeyValuePair<int, int> k) => k.Key, (KeyValuePair<int, int> v) => v.Value);
                        Dictionary<int, int> dictionary11;
                        dictionary11 = 系统数据2.脚本数字.ToDictionary((KeyValuePair<int, int> k) => k.Key, (KeyValuePair<int, int> v) => v.Value);
                        obj2.脚本字符.ToDictionary((KeyValuePair<int, string> k) => k.Key, (KeyValuePair<int, string> v) => v.Value);
                        Dictionary<int, string> dictionary12;
                        dictionary12 = 系统数据2.脚本字符.ToDictionary((KeyValuePair<int, string> k) => k.Key, (KeyValuePair<int, string> v) => v.Value);
                        obj2.排序角色ID.ToDictionary((KeyValuePair<int, string> k) => k.Key, (KeyValuePair<int, string> v) => v.Value);
                        Dictionary<int, string> dictionary13;
                        dictionary13 = 系统数据2.排序角色ID.ToDictionary((KeyValuePair<int, string> k) => k.Key, (KeyValuePair<int, string> v) => v.Value);
                        dictionary3[typeof(系统数据)].数据表.Clear();
                        dictionary2[typeof(系统数据)].数据表.Clear();
                        dictionary2[typeof(系统数据)].数据表[1] = new 系统数据(1);
                        系统数据2 = dictionary2[typeof(系统数据)].数据表[1] as 系统数据;
                        foreach (KeyValuePair<string, DateTime> item11 in dictionary8)
                        {
                            系统数据2.封禁网络(item11.Key, item11.Value);
                        }
                        foreach (KeyValuePair<string, DateTime> item12 in dictionary7)
                        {
                            if (!系统数据2.网络封禁.ContainsKey(item12.Key))
                            {
                                系统数据2.封禁网络(item12.Key, item12.Value);
                            }
                        }
                        foreach (KeyValuePair<string, DateTime> item13 in dictionary10)
                        {
                            系统数据2.封禁网卡(item13.Key, item13.Value);
                        }
                        foreach (KeyValuePair<string, DateTime> item14 in dictionary9)
                        {
                            if (!系统数据2.网卡封禁.ContainsKey(item14.Key))
                            {
                                系统数据2.封禁网卡(item14.Key, item14.Value);
                            }
                        }
                        foreach (KeyValuePair<int, int> item15 in dictionary11)
                        {
                            系统数据2.脚本数字.QuietlySet(item15.Key, item15.Value);
                        }
                        foreach (KeyValuePair<int, string> item16 in dictionary12)
                        {
                            系统数据2.脚本字符.QuietlySet(item16.Key, item16.Value);
                        }
                        foreach (KeyValuePair<int, string> item17 in dictionary13)
                        {
                            系统数据2.排序角色ID.QuietlySet(item17.Key, item17.Value);
                        }
                        foreach (KeyValuePair<int, 游戏数据> item18 in dictionary2[typeof(角色数据)].数据表)
                        {
                            角色数据 角色;
                            角色 = item18.Value as 角色数据;
                            系统数据2.更新战力(角色);
                            系统数据2.更新等级(角色);
                            系统数据2.更新声望(角色);
                            系统数据2.更新PK值(角色);
                        }
                        foreach (KeyValuePair<int, 游戏数据> item19 in dictionary2[typeof(行会数据)].数据表)
                        {
                            系统数据2.更新行会(item19.Value as 行会数据);
                        }
                        游戏数据网关.数据类型表 = dictionary2;
                        游戏数据网关.强制保存();
                        游戏数据网关.导出数据();
                        Path.GetFileName(账号配置路径);
                        foreach (KeyValuePair<string, JObject> item20 in dictionary)
                        {
                            string text4;
                            text4 = Path.Combine(账号配置路径, "合区后");
                            if (!Directory.Exists(text4))
                            {
                                Directory.CreateDirectory(text4);
                            }
                            File.WriteAllText(Path.Combine(text4, item20.Key + ".txt"), JsonConvert.SerializeObject(item20.Value, Formatting.Indented));
                        }
                        主程.添加系统日志(string.Join("\r\n", dictionary4.Values.SelectMany((List<string> v) => v)));
                        主程.添加命令日志("客户数据已经合并完成");
                        主程.WriteLogs();
                        MessageBox.Show("客户数据已经合并完毕, 应用程序需要重启");
                        Environment.Exit(0);
                    }
                }
            });
        }

        public static void 处理数据()
        {
            foreach (KeyValuePair<int, 游戏数据> item in 游戏数据网关.队伍数据表.数据表)
            {
                if (item.Value is 队伍数据 队伍数据2)
                {
                    队伍数据2.处理数据();
                }
            }
            if (主程.当前时间 > 游戏数据网关.寄售处理)
            {
                游戏数据网关.寄售处理 = 主程.当前时间.AddMinutes(1.0);
                foreach (KeyValuePair<int, 游戏数据> item2 in 游戏数据网关.寄售数据表.数据表)
                {
                    if (item2.Value is 寄售数据 寄售数据2)
                    {
                        寄售数据2.处理数据();
                    }
                }
            }
            if (!(主程.当前时间.Date != 系统数据.数据.每日处理.V.Date))
            {
                return;
            }
            系统数据.数据.每日处理.V = 主程.当前时间.Date;
            foreach (KeyValuePair<int, 游戏数据> item3 in 游戏数据网关.角色数据表.数据表)
            {
                角色数据 角色数据2;
                角色数据2 = item3.Value as 角色数据;
                玩家实例 玩家实例;
                玩家实例 = 角色数据2.网络连接?.绑定角色;
                角色数据2.零点数字.Clear();
                DateTime date;
                date = 主程.当前时间.AddDays(-1.0).Date;
                foreach (KeyValuePair<ushort, ushort> item4 in 角色数据2.找回奖励)
                {
                    switch (item4.Key)
                    {
                        case 2:
                            角色数据2.找回奖励[item4.Key] = (ushort)Math.Max(角色数据2.找回奖励[item4.Key] - (5 - 角色数据2.紧急任务.V), 0);
                            break;
                        case 1:
                            if (计算类.转换时间(角色数据2.角色变量[218]).Date == date)
                            {
                                角色数据2.找回奖励[item4.Key] = (ushort)Math.Max(角色数据2.找回奖励[item4.Key] - 1, 0);
                            }
                            break;
                        case 14:
                            if (计算类.转换时间(角色数据2.角色变量[629]).Date == date)
                            {
                                角色数据2.找回奖励[item4.Key] = (ushort)Math.Max(角色数据2.找回奖励[item4.Key] - 1, 0);
                            }
                            break;
                        case 7:
                            if (计算类.转换时间(角色数据2.角色变量[416]).Date == date)
                            {
                                角色数据2.找回奖励[item4.Key] = (ushort)Math.Max(角色数据2.找回奖励[item4.Key] - 1, 0);
                            }
                            break;
                        case 9:
                            角色数据2.找回奖励[item4.Key] = (ushort)Math.Max(角色数据2.找回奖励[item4.Key] - 1, 0);
                            break;
                        case 11:
                            if (计算类.转换时间(角色数据2.角色变量[220]).Date == date)
                            {
                                角色数据2.找回奖励[item4.Key] = (ushort)Math.Max(角色数据2.找回奖励[item4.Key] - 1, 0);
                            }
                            break;
                        case 21:
                            角色数据2.找回奖励[item4.Key] = (ushort)Math.Max(角色数据2.找回奖励[item4.Key] - 1, 0);
                            break;
                        case 20:
                            角色数据2.找回奖励[item4.Key] = (ushort)Math.Max(角色数据2.找回奖励[item4.Key] - 1, 0);
                            break;
                        case 31:
                            角色数据2.找回奖励[item4.Key] = (ushort)Math.Max(角色数据2.找回奖励[item4.Key] - 1, 0);
                            break;
                        case 27:
                            角色数据2.找回奖励[item4.Key] = (ushort)Math.Max(角色数据2.找回奖励[item4.Key] - 1, 0);
                            break;
                    }
                }
                角色数据2.今日充值.V = 0;
                角色数据2.紧急任务.V = 0;
                if (玩家实例 != null)
                {
                    玩家实例.发送封包(new 同步签到信息
                    {
                        签到次数 = 玩家实例.每日签到,
                        能否签到 = ((主程.当前时间.Date.AddDays(1.0) - 玩家实例.签到日期.Date).TotalDays > 0.0),
                        开启签到 = true
                    });
                    玩家实例.发送封包(new 传永武技签到
                    {
                        签到次数 = 玩家实例.传永武技,
                        能否签到 = ((玩家实例.本期特权 == 4 || 玩家实例.本期特权 == 5 || 玩家实例.本期特权 == 7) && (主程.当前时间.Date.AddDays(1.0) - 玩家实例.武技日期.Date).TotalDays > 0.0)
                    });
                }
                foreach (int value2 in Enum.GetValues(typeof(日程变量)))
                {
                    if (角色数据2.角色变量.ContainsKey(value2))
                    {
                        角色数据2.角色变量[value2] = 0;
                    }
                }
                角色数据2.网络连接?.发送封包(new 同步角色变量
                {
                    字节描述 = 角色数据2.网络连接.绑定角色.获取角色变量()
                });
                角色数据2.日程进度.V = 0;
                角色数据2.日程奖励.V = 0;
                角色数据2.网络连接?.绑定角色.发送日程详情();
                if (!Settings.屏蔽战功)
                {
                    foreach (KeyValuePair<ushort, ushort> item5 in 角色数据2.战功任务)
                    {
                        if (战功任务.数据表.TryGetValue(item5.Key, out var value))
                        {
                            if (value.任务分类 == QuestResetType.Daily)
                            {
                                角色数据2.战功任务[item5.Key] = 0;
                            }
                            else if (value.任务分类 == QuestResetType.Weekly && 主程.当前时间.Date.DayOfWeek == DayOfWeek.Monday)
                            {
                                角色数据2.战功任务[item5.Key] = 0;
                            }
                        }
                    }
                }
                角色数据2.网络连接?.绑定角色.发送战功详情();
                游戏脚本.每日清空(角色数据2);
            }
        }

        public static void 检测物品数据(bool 添加到数据表)
        {
            foreach (角色数据 item in 游戏数据网关.角色数据表.数据表.Values.ToList().Cast<角色数据>().ToList())
            {
                List<物品数据> list;
                list = new List<物品数据>();
                list.AddRange(item.角色装备.Values);
                list.AddRange(item.角色背包.Values);
                list.AddRange(item.角色仓库.Values);
                list.AddRange(item.角色资源包.Values);
                foreach (物品数据 item2 in list)
                {
                    byte v;
                    v = item2.物品容器.V;
                    byte v2;
                    v2 = item2.物品位置.V;
                    switch (v)
                    {
                        case 0:
                            {
                                装备数据 v4;
                                if (v2 > 15)
                                {
                                    主窗口.添加系统日志($"<= @ 命令执行失败, {item.角色名字} 装备穿戴部位 {v2.ToString()} 不正确");
                                }
                                else if (item.角色装备.TryGetValue(v2, out v4) && !游戏数据网关.装备数据表.数据表.ContainsKey(v4.数据索引.V))
                                {
                                    if (添加到数据表)
                                    {
                                        游戏数据网关.装备数据表.添加数据(v4);
                                        主窗口.添加系统日志($"<= @ 命令执行成功, {item.角色名字} 角色部位 {v2.ToString()} 的装备 {v4.物品名字} 已添加到数据表");
                                    }
                                    else
                                    {
                                        主窗口.添加系统日志($"<= @ 命令执行成功, {item.角色名字} 装备数据表 不存在 {v4.物品名字}");
                                    }
                                }
                                break;
                            }
                        case 1:
                        case 2:
                        case 7:
                            {
                                物品数据 v3;
                                v3 = null;
                                string value;
                                value = "";
                                switch (v)
                                {
                                    case 1:
                                        if (v2 >= item.背包大小.V)
                                        {
                                            主窗口.添加系统日志($"<= @ 命令执行失败, {item.角色名字} 背包位置 {v2.ToString()} 大于背包大小");
                                        }
                                        else if (item.角色背包.TryGetValue(v2, out v3))
                                        {
                                            value = "背包";
                                            break;
                                        }
                                        goto end_IL_00a7;
                                    case 2:
                                        if (v2 >= item.仓库大小.V)
                                        {
                                            主窗口.添加系统日志($"<= @ 命令执行失败, {item.角色名字} 仓库位置 {v2.ToString()} 大于仓库大小");
                                        }
                                        else if (item.角色仓库.TryGetValue(v2, out v3))
                                        {
                                            value = "仓库";
                                            break;
                                        }
                                        goto end_IL_00a7;
                                    case 7:
                                        if (v2 >= item.资源包大小.V)
                                        {
                                            主窗口.添加系统日志($"<= @ 命令执行失败, {item.角色名字} 资源背包位置 {v2.ToString()} 大于仓库大小");
                                        }
                                        else if (item.角色资源包.TryGetValue(v2, out v3))
                                        {
                                            value = "资源背包";
                                            break;
                                        }
                                        goto end_IL_00a7;
                                }
                                if (v3 is 装备数据 装备数据2)
                                {
                                    if (!游戏数据网关.装备数据表.数据表.ContainsKey(装备数据2.数据索引.V))
                                    {
                                        if (添加到数据表)
                                        {
                                            游戏数据网关.装备数据表.添加数据(装备数据2);
                                            主窗口.添加系统日志($"<= @ 命令执行成功, {item.角色名字} 角色{value} {v2.ToString()} 的装备 {装备数据2.物品名字} 已添加到 装备数据表");
                                        }
                                        else
                                        {
                                            主窗口.添加系统日志($"<= @ 命令执行成功, {item.角色名字} 装备数据表 不存在 {装备数据2.物品名字}");
                                        }
                                    }
                                }
                                else if (!游戏数据网关.物品数据表.数据表.ContainsKey(v3.数据索引.V))
                                {
                                    if (添加到数据表)
                                    {
                                        游戏数据网关.物品数据表.添加数据(v3);
                                        主窗口.添加系统日志($"<= @ 命令执行成功, {item.角色名字} 角色{value} {v2.ToString()} 的物品 {v3.物品名字} 已添加到 物品数据表");
                                    }
                                    else
                                    {
                                        主窗口.添加系统日志($"<= @ 命令执行成功, {item.角色名字} 物品数据表 不存在 {v3.物品名字}");
                                    }
                                }
                                break;
                            }
                        end_IL_00a7:
                            break;
                    }
                }
            }
        }
    }
}
