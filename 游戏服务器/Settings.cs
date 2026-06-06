using DevExpress.XtraBars;
using DevExpress.XtraSpreadsheet.Model.History;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace 游戏服务器
{
    internal static class Settings
    {
        public delegate void SaveHandle();

        public delegate void LoadHandle();

        public static ushort 客户连接端口 = 8701;

        public static ushort 门票接收端口 = 6678;

        // PROTO-03: 门票接收 UDP 端口绑定 IPAddress.Any, 任何源 IP 都能注入伪造门票登录任意账号.
        // 此白名单声明可信账号服务器 IP, 以逗号分隔. 空字符串 = 不过滤 (兼容旧部署, 启动时打警告).
        // 建议生产环境设置为账号服务器实际地址, 如 "10.0.0.5,127.0.0.1".
        public static string 门票来源白名单 = "";

        public static ushort 封包限定数量 = 100;

        public static ushort 异常屏蔽时间 = 5;

        public static ushort 掉线判定时间 = 5;

        public static byte 游戏开放等级 = 40;

        public static decimal 装备特修折扣 = 1m;

        public static decimal 怪物额外爆率 = default(decimal);

        public static decimal 怪物经验倍率 = 1m;

        public static byte 减收益等级差 = 10;

        public static decimal 收益减少比率 = 0.1m;

        public static ushort 怪物诱惑时长 = 120;

        public static byte 物品清理时间 = 5;

        public static byte 物品归属时间 = 3;

        public static string 游戏数据目录 = ".\\Database";

        public static string 数据备份目录 = ".\\Backup";

        public static string 系统公告内容 = "";

        public static byte 新手扶持等级 = 0;

        public static byte 自动保存时间 = 5;

        public static byte 武斗场时间一 = 13;

        public static byte 武斗场时间二 = 21;

        public static int 武斗普通经验 = 500;

        public static int 武斗抢点经验 = 2500;

        public static DateTime 开服日期 = DateTime.Now;

        public static bool 开启线程发包 = true;

        public static bool 开启自动战斗 = true;

        public static bool 开启任务系统 = true;

        public static bool 开启成就系统 = true;

        public static bool 开启七天奖励 = true;

        public static bool 使用新版内挂 = false;

        public static bool 沙巴克掉装备 = false;

        // 攻沙(沙巴克攻城)时间, 接入自 参考引擎 Setup.ini. 默认值与原写死行为一致:
        // 开始 20:00 (前 10 分钟即 19:50 预告), 持续 120 分钟, 兜底结束 22:00.
        public static byte 攻沙开始时间小时 = 20;

        public static byte 攻沙开始时间分钟 = 0;

        public static byte 攻沙结束时间小时 = 22;

        public static byte 攻沙结束时间分钟 = 0;

        public static int 攻城持续时间 = 120;

        // 九层妖塔各层获取经验, 接入自 参考引擎 Setup.ini. 默认值与原写死字典一致. [10] 即秘境普通模式.
        public static int[] 妖塔层经验 = new int[10] { 10560, 14080, 18480, 23760, 29920, 40656, 51744, 61600, 79200, 900000 };

        // 悬赏任务次数, 接入自 参考引擎 Setup.ini. 默认值保持 YH 现行行为(接取持有 5, 完成 日10/周15),
        // 未采用 参考引擎 的默认值以免改变现网玩法. 注意: 接取持有数需 <= 可用悬赏池, 否则刷新会死循环.
        public static byte 每日悬赏接取次数 = 5;

        public static byte 每周悬赏接取次数 = 5;

        public static byte 每日悬赏完成次数 = 10;

        public static byte 每周悬赏完成次数 = 15;

        // 寄售行每名玩家最大同时上架数量(原写死 5). 参考引擎 无对应键, 用 YH 命名暴露.
        public static int 寄售上架上限 = 5;

        // 学宫通关奖励(原写死). [学宫] 段. 道具ID + 简单/中等/困难三档各两件数量.
        public static int 学宫奖励道具一 = 91143;

        public static int 学宫奖励道具二 = 91145;

        // 下标 0/1/2 = 简单/中等/困难; [,0]=道具一数量 [,1]=道具二数量
        public static int[,] 学宫奖励数量 = new int[3, 2] { { 6, 2 }, { 60, 20 }, { 600, 200 } };

        // 幸运倍攻(从零原创, 参考配置截图规格): 攻击方玩家幸运值达到某档阈值时, 最终伤害 ×对应倍率(取满足的最高档).
        // 默认关闭, 不影响现网. 阈值/倍率十档对应 参考引擎 Setup.ini 的 幸运额外增伤值一~十 / 幸运增伤倍率值一~十.
        public static bool 开启幸运倍率功能 = false;

        public static int[] 幸运额外增伤值 = new int[10] { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };

        public static float[] 幸运增伤倍率值 = new float[10] { 1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f, 1.8f, 1.9f, 2f };

        // [幸运倍攻]档位中文数字后缀, 用于读写 参考引擎 风格键名.
        private static readonly string[] 十档中文 = new string[10] { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };

        // 职业第一(从零原创, 照 参考引擎 截图规格): 各职业战力榜榜首在线时获得专属 BUFF.
        // 下标按 游戏对象职业 枚举: 0战士 1法师 2刺客 3弓手 4道士 5龙枪. BUFF编号=0 表示该职业不启用. 默认全关.
        public static bool 职业第一机制 = false;

        public static ushort[] 职业第一编号 = new ushort[6] { 0, 0, 0, 0, 0, 0 };

        // 首杀/首爆(从零原创, 照 参考引擎 截图规格): 全服首个击杀某 BOSS / 首个爆出某贵重物品的玩家获奖. 默认全关.
        // 8 个开关沿用 参考引擎 键名; 奖励内容(货币/道具数值)截图未给, 为 YH 命名扩展.
        public static bool 开启怪物首杀功能 = false;

        public static bool 开启怪物首杀货币 = false;

        public static bool 开启怪物首杀道具 = false;

        public static bool 开启首杀奖励邮件 = false;

        public static bool 开启装备首爆功能 = false;

        public static bool 开启装备首爆货币 = false;

        public static bool 开启装备首爆道具 = false;

        public static bool 开启首爆奖励邮件 = false;

        // 货币类型按 游戏货币 枚举(0银币/1金币/3元宝/15绑定元宝...). 数量为 0 即不发该项.
        public static int 首杀货币类型 = 3;

        public static int 首杀货币数量 = 0;

        public static int 首杀道具编号 = 0;

        public static int 首杀道具数量 = 1;

        public static int 首爆货币类型 = 3;

        public static int 首爆货币数量 = 0;

        public static int 首爆道具编号 = 0;

        public static int 首爆道具数量 = 1;

        // 公会等级/职位特效(从零原创, 照 参考引擎 截图规格): 在线行会成员按行会等级/本人职位获得专属特效 BUFF.
        // 等级特效编号[1..6] 按行会等级; 职位特效编号 按 行会职位 枚举(1会长..6执事), 编号=0 即该项不启用. 默认全关.
        public static bool 开启公会等级特效 = false;

        public static bool 开启公会职位特效 = false;

        public static ushort[] 行会等级特效编号 = new ushort[7] { 0, 0, 0, 0, 0, 0, 0 };

        public static ushort[] 行会职位特效编号 = new ushort[7] { 0, 0, 0, 0, 0, 0, 0 };

        public static bool 限制重要封包间隔时间 = true;

        public static bool 开启lua = true;

        public static bool 触发装备重铸 = false;

        public static bool 资源包只能放材料 = true;

        public static bool 可购买玛法特权 = true;

        public static int 玩家出生地图 = 142;

        public static string 游戏区服名称 = "开天辟地";

        public static string 统计UUID代码 = "";

        // HTTP 充值/管理回调接口的签名密钥. 必须由运营方配置为足够长且高熵的随机字符串.
        // 若保持为空, WebApi.WebApiService.Sign() 会拒绝处理所有签名请求.
        public static string 充值签名密钥 = "";

        public static byte 暴击特效ID = 0;

        public static bool BOSS自动死亡 = true;

        public static bool 普通强化不碎武器 = false;

        public static bool 屏蔽七天活动 = false;

        public static bool 屏蔽威望 = false;

        public static bool 屏蔽战功 = false;

        public static bool 屏蔽日程 = false;

        public static bool 屏蔽每周特惠 = false;

        public static bool 屏蔽每日签到 = false;

        public static bool 屏蔽传永武技 = false;

        public static int 充值货币类型 = 0;

        public static bool 达最高级后继续加经验 = true;

        public static int 神佑掉落ID = 90313;

        public static int 商人比例 = 110;

        public static string 充值公告 = "";

        public static string 商人发货公告 = "";

        public static uint 货币异常上限 = 4200000000u;

        public static uint 货币异常归位 = 10000u;

        public static int 祝福油几率0级 = 80;

        public static int 祝福油几率1级 = 10;

        public static int 祝福油几率2级 = 8;

        public static int 祝福油几率3级 = 8;

        public static int 祝福油几率4级 = 5;

        public static int 祝福油几率5级 = 3;

        public static int 祝福油几率6级 = 3;

        public static float 死亡掉落剑甲 = 0.01f;

        public static float 死亡掉落首饰 = 0.05f;

        public static float 死亡掉落背包 = 0.1f;

        public static int 单次死亡限量 = 3;

        public static float 红名掉落剑甲 = 0.01f;

        public static float 红名掉落首饰 = 0.05f;

        public static int 龙卫重铸费用 = 10000;

        public static int 锁单重铸费用 = 250000;

        public static int 锁半重铸费用 = 500000;

        public static int 行会最高人数 = 40;

        public static byte 幽冥海底节点开放天数 = 10;

        public static byte 白日赤月节点开放天数 = 21;

        public static byte 魔龙之城节点开放天数 = 45;

        public static byte 苍月惊变节点开放天数 = 75;

        public static byte 龙耀雪山节点开放天数 = 100;

        public static byte 聊天限制等级 = 20;

        public static int 玛法新秀价格 = 68;

        public static int 玛法名俊价格 = 128;

        public static int 玛法豪杰价格 = 288;

        public static int 玛法战将价格 = 288;

        public static int 玛法至尊价格 = 588;

        public static float 技巧项链倍数 = 2f;

        public static bool 金币自动入包 = true;

        public static bool 银币自动入包 = true;

        public static bool 物品自动入包 = false;

        public static bool 自动分解装备 = false;

        public static bool 不分解极品装备 = false;

        public static bool 安全区内满血满蓝 = true;

        public static string 默认皮肤 = "Office 2010 Blue";

        public static bool 下线宝宝不死 = false;

        public static bool[] 职业开放 = new bool[6] { true, true, true, true, true, true };

        public static string EnvirPath = Path.Combine(Settings.游戏数据目录, "Envir");

        public static string NameListPath = Path.Combine(Settings.EnvirPath, "NameLists");

        public static string NPCPath = Path.Combine(Settings.EnvirPath, "NPCs");

        public static string ValuePath = Path.Combine(Settings.EnvirPath, "Values");

        public static string DefaultNPCFilename = "00-QFunction";

        private static InIReader iniconfig;

        [CompilerGenerated]
        private static SaveHandle _0004_0002_0004_0008_0002_0006_0002_0005_0007_0002;

        [CompilerGenerated]
        private static SaveHandle _0013_0014_0013_0009_0016_0006;

        public static event SaveHandle OnSaved
        {
            [CompilerGenerated]
            add
            {
                SaveHandle saveHandle;
                saveHandle = Settings._0004_0002_0004_0008_0002_0006_0002_0005_0007_0002;
                SaveHandle saveHandle2;
                do
                {
                    saveHandle2 = saveHandle;
                    saveHandle = Interlocked.CompareExchange(ref Settings._0004_0002_0004_0008_0002_0006_0002_0005_0007_0002, (SaveHandle)Delegate.Combine(saveHandle2, value), saveHandle2);
                }
                while ((object)saveHandle != saveHandle2);
            }
            [CompilerGenerated]
            remove
            {
                SaveHandle saveHandle;
                saveHandle = Settings._0004_0002_0004_0008_0002_0006_0002_0005_0007_0002;
                SaveHandle saveHandle2;
                do
                {
                    saveHandle2 = saveHandle;
                    saveHandle = Interlocked.CompareExchange(ref Settings._0004_0002_0004_0008_0002_0006_0002_0005_0007_0002, (SaveHandle)Delegate.Remove(saveHandle2, value), saveHandle2);
                }
                while ((object)saveHandle != saveHandle2);
            }
        }

        public static event SaveHandle OnLoaded
        {
            [CompilerGenerated]
            add
            {
                SaveHandle saveHandle;
                saveHandle = Settings._0013_0014_0013_0009_0016_0006;
                SaveHandle saveHandle2;
                do
                {
                    saveHandle2 = saveHandle;
                    saveHandle = Interlocked.CompareExchange(ref Settings._0013_0014_0013_0009_0016_0006, (SaveHandle)Delegate.Combine(saveHandle2, value), saveHandle2);
                }
                while ((object)saveHandle != saveHandle2);
            }
            [CompilerGenerated]
            remove
            {
                SaveHandle saveHandle;
                saveHandle = Settings._0013_0014_0013_0009_0016_0006;
                SaveHandle saveHandle2;
                do
                {
                    saveHandle2 = saveHandle;
                    saveHandle = Interlocked.CompareExchange(ref Settings._0013_0014_0013_0009_0016_0006, (SaveHandle)Delegate.Remove(saveHandle2, value), saveHandle2);
                }
                while ((object)saveHandle != saveHandle2);
            }
        }

        public static void Load()
        {
            Settings.iniconfig = new InIReader(".\\Setup.ini");
            Settings.客户连接端口 = Settings.iniconfig.ReadUInt16("General", "客户连接端口", Settings.客户连接端口);
            Settings.门票接收端口 = Settings.iniconfig.ReadUInt16("General", "门票接收端口", Settings.门票接收端口);
            Settings.门票来源白名单 = Settings.iniconfig.ReadString("General", "门票来源白名单", Settings.门票来源白名单);
            Settings.封包限定数量 = Settings.iniconfig.ReadUInt16("General", "封包限定数量", Settings.封包限定数量);
            Settings.异常屏蔽时间 = Settings.iniconfig.ReadUInt16("General", "异常屏蔽时间", Settings.异常屏蔽时间);
            Settings.掉线判定时间 = Settings.iniconfig.ReadUInt16("General", "掉线判定时间", Settings.掉线判定时间);
            Settings.游戏开放等级 = Settings.iniconfig.ReadByte("General", "游戏开放等级", Settings.游戏开放等级);
            Settings.装备特修折扣 = Settings.iniconfig.ReadDecimal("General", "装备特修折扣", Settings.装备特修折扣);
            Settings.怪物额外爆率 = Settings.iniconfig.ReadDecimal("General", "怪物额外爆率", Settings.怪物额外爆率);
            Settings.怪物经验倍率 = Settings.iniconfig.ReadDecimal("General", "怪物经验倍率", Settings.怪物经验倍率);
            Settings.减收益等级差 = Settings.iniconfig.ReadByte("General", "减收益等级差", Settings.减收益等级差);
            Settings.收益减少比率 = Settings.iniconfig.ReadDecimal("General", "收益减少比率", Settings.收益减少比率);
            Settings.怪物诱惑时长 = Settings.iniconfig.ReadUInt16("General", "怪物诱惑时长", Settings.怪物诱惑时长);
            Settings.物品清理时间 = Settings.iniconfig.ReadByte("General", "物品清理时间", Settings.物品清理时间);
            Settings.物品归属时间 = Settings.iniconfig.ReadByte("General", "物品归属时间", Settings.物品归属时间);
            Settings.游戏数据目录 = Settings.iniconfig.ReadString("General", "游戏数据目录", Settings.游戏数据目录);
            Settings.数据备份目录 = Settings.iniconfig.ReadString("General", "数据备份目录", Settings.数据备份目录);
            Settings.系统公告内容 = Settings.iniconfig.ReadString("General", "系统公告内容", Settings.系统公告内容);
            Settings.新手扶持等级 = Settings.iniconfig.ReadByte("General", "新手扶持等级", Settings.新手扶持等级);
            Settings.自动保存时间 = Settings.iniconfig.ReadByte("General", "自动保存时间", Settings.自动保存时间);
            Settings.武斗场时间一 = Settings.iniconfig.ReadByte("General", "武斗场时间一", Settings.武斗场时间一);
            Settings.武斗场时间二 = Settings.iniconfig.ReadByte("General", "武斗场时间二", Settings.武斗场时间二);
            Settings.武斗普通经验 = Settings.iniconfig.ReadInt32("General", "武斗普通经验", Settings.武斗普通经验);
            Settings.武斗抢点经验 = Settings.iniconfig.ReadInt32("General", "武斗抢点经验", Settings.武斗抢点经验);
            Settings.开服日期 = 计算类.转换日期(Settings.iniconfig.ReadInt32("General", "开服日期", 0));
            Settings.开启线程发包 = Settings.iniconfig.ReadBoolean("General", "开启线程发包", Settings.开启线程发包);
            Settings.开启自动战斗 = Settings.iniconfig.ReadBoolean("General", "开启自动战斗", Settings.开启自动战斗);
            Settings.开启任务系统 = Settings.iniconfig.ReadBoolean("General", "开启任务系统", Settings.开启任务系统);
            Settings.开启成就系统 = Settings.iniconfig.ReadBoolean("General", "开启成就系统", Settings.开启成就系统);
            Settings.使用新版内挂 = Settings.iniconfig.ReadBoolean("General", "使用新版内挂", Settings.使用新版内挂);
            Settings.沙巴克掉装备 = Settings.iniconfig.ReadBoolean("General", "沙巴克掉装备", Settings.沙巴克掉装备);
            Settings.攻沙开始时间小时 = Settings.iniconfig.ReadByte("General", "攻沙开始时间小时", Settings.攻沙开始时间小时);
            Settings.攻沙开始时间分钟 = Settings.iniconfig.ReadByte("General", "攻沙开始时间分钟", Settings.攻沙开始时间分钟);
            Settings.攻沙结束时间小时 = Settings.iniconfig.ReadByte("General", "攻沙结束时间小时", Settings.攻沙结束时间小时);
            Settings.攻沙结束时间分钟 = Settings.iniconfig.ReadByte("General", "攻沙结束时间分钟", Settings.攻沙结束时间分钟);
            Settings.攻城持续时间 = Settings.iniconfig.ReadInt32("General", "攻城持续时间", Settings.攻城持续时间);
            Settings.妖塔层经验[0] = Settings.iniconfig.ReadInt32("General", "第一层经验值", Settings.妖塔层经验[0]);
            Settings.妖塔层经验[1] = Settings.iniconfig.ReadInt32("General", "第二层经验值", Settings.妖塔层经验[1]);
            Settings.妖塔层经验[2] = Settings.iniconfig.ReadInt32("General", "第三层经验值", Settings.妖塔层经验[2]);
            Settings.妖塔层经验[3] = Settings.iniconfig.ReadInt32("General", "第四层经验值", Settings.妖塔层经验[3]);
            Settings.妖塔层经验[4] = Settings.iniconfig.ReadInt32("General", "第五层经验值", Settings.妖塔层经验[4]);
            Settings.妖塔层经验[5] = Settings.iniconfig.ReadInt32("General", "第六层经验值", Settings.妖塔层经验[5]);
            Settings.妖塔层经验[6] = Settings.iniconfig.ReadInt32("General", "第七层经验值", Settings.妖塔层经验[6]);
            Settings.妖塔层经验[7] = Settings.iniconfig.ReadInt32("General", "第八层经验值", Settings.妖塔层经验[7]);
            Settings.妖塔层经验[8] = Settings.iniconfig.ReadInt32("General", "第九层经验值", Settings.妖塔层经验[8]);
            Settings.妖塔层经验[9] = Settings.iniconfig.ReadInt32("General", "普通妖塔秘境", Settings.妖塔层经验[9]);
            Settings.每日悬赏接取次数 = Settings.iniconfig.ReadByte("General", "每日悬赏接取次数", Settings.每日悬赏接取次数);
            Settings.每周悬赏接取次数 = Settings.iniconfig.ReadByte("General", "每周悬赏接取次数", Settings.每周悬赏接取次数);
            Settings.每日悬赏完成次数 = Settings.iniconfig.ReadByte("General", "每日悬赏完成次数", Settings.每日悬赏完成次数);
            Settings.每周悬赏完成次数 = Settings.iniconfig.ReadByte("General", "每周悬赏完成次数", Settings.每周悬赏完成次数);
            Settings.寄售上架上限 = Settings.iniconfig.ReadInt32("General", "寄售上架上限", Settings.寄售上架上限);
            Settings.学宫奖励道具一 = Settings.iniconfig.ReadInt32("学宫", "学宫奖励道具一", Settings.学宫奖励道具一);
            Settings.学宫奖励道具二 = Settings.iniconfig.ReadInt32("学宫", "学宫奖励道具二", Settings.学宫奖励道具二);
            Settings.学宫奖励数量[0, 0] = Settings.iniconfig.ReadInt32("学宫", "简单奖励道具一数量", Settings.学宫奖励数量[0, 0]);
            Settings.学宫奖励数量[0, 1] = Settings.iniconfig.ReadInt32("学宫", "简单奖励道具二数量", Settings.学宫奖励数量[0, 1]);
            Settings.学宫奖励数量[1, 0] = Settings.iniconfig.ReadInt32("学宫", "中等奖励道具一数量", Settings.学宫奖励数量[1, 0]);
            Settings.学宫奖励数量[1, 1] = Settings.iniconfig.ReadInt32("学宫", "中等奖励道具二数量", Settings.学宫奖励数量[1, 1]);
            Settings.学宫奖励数量[2, 0] = Settings.iniconfig.ReadInt32("学宫", "困难奖励道具一数量", Settings.学宫奖励数量[2, 0]);
            Settings.学宫奖励数量[2, 1] = Settings.iniconfig.ReadInt32("学宫", "困难奖励道具二数量", Settings.学宫奖励数量[2, 1]);
            Settings.开启幸运倍率功能 = Settings.iniconfig.ReadBoolean("General", "开启幸运倍率功能", Settings.开启幸运倍率功能);
            for (int 幸运档 = 0; 幸运档 < 10; 幸运档++)
            {
                Settings.幸运额外增伤值[幸运档] = Settings.iniconfig.ReadInt32("General", "幸运额外增伤值" + Settings.十档中文[幸运档], Settings.幸运额外增伤值[幸运档]);
                Settings.幸运增伤倍率值[幸运档] = Settings.iniconfig.ReadFloat("General", "幸运增伤倍率值" + Settings.十档中文[幸运档], Settings.幸运增伤倍率值[幸运档]);
            }
            Settings.职业第一机制 = Settings.iniconfig.ReadBoolean("General", "职业第一机制", Settings.职业第一机制);
            Settings.职业第一编号[0] = Settings.iniconfig.ReadUInt16("General", "战士职业第一编号", Settings.职业第一编号[0]);
            Settings.职业第一编号[1] = Settings.iniconfig.ReadUInt16("General", "法师职业第一编号", Settings.职业第一编号[1]);
            Settings.职业第一编号[2] = Settings.iniconfig.ReadUInt16("General", "刺客职业第一编号", Settings.职业第一编号[2]);
            Settings.职业第一编号[3] = Settings.iniconfig.ReadUInt16("General", "弓箭职业第一编号", Settings.职业第一编号[3]);
            Settings.职业第一编号[4] = Settings.iniconfig.ReadUInt16("General", "道士职业第一编号", Settings.职业第一编号[4]);
            Settings.职业第一编号[5] = Settings.iniconfig.ReadUInt16("General", "龙枪职业第一编号", Settings.职业第一编号[5]);
            Settings.开启怪物首杀功能 = Settings.iniconfig.ReadBoolean("General", "开启怪物首杀功能", Settings.开启怪物首杀功能);
            Settings.开启怪物首杀货币 = Settings.iniconfig.ReadBoolean("General", "开启怪物首杀货币", Settings.开启怪物首杀货币);
            Settings.开启怪物首杀道具 = Settings.iniconfig.ReadBoolean("General", "开启怪物首杀道具", Settings.开启怪物首杀道具);
            Settings.开启首杀奖励邮件 = Settings.iniconfig.ReadBoolean("General", "开启首杀奖励邮件", Settings.开启首杀奖励邮件);
            Settings.开启装备首爆功能 = Settings.iniconfig.ReadBoolean("General", "开启装备首爆功能", Settings.开启装备首爆功能);
            Settings.开启装备首爆货币 = Settings.iniconfig.ReadBoolean("General", "开启装备首爆货币", Settings.开启装备首爆货币);
            Settings.开启装备首爆道具 = Settings.iniconfig.ReadBoolean("General", "开启装备首爆道具", Settings.开启装备首爆道具);
            Settings.开启首爆奖励邮件 = Settings.iniconfig.ReadBoolean("General", "开启首爆奖励邮件", Settings.开启首爆奖励邮件);
            Settings.首杀货币类型 = Settings.iniconfig.ReadInt32("General", "首杀货币类型", Settings.首杀货币类型);
            Settings.首杀货币数量 = Settings.iniconfig.ReadInt32("General", "首杀货币数量", Settings.首杀货币数量);
            Settings.首杀道具编号 = Settings.iniconfig.ReadInt32("General", "首杀道具编号", Settings.首杀道具编号);
            Settings.首杀道具数量 = Settings.iniconfig.ReadInt32("General", "首杀道具数量", Settings.首杀道具数量);
            Settings.首爆货币类型 = Settings.iniconfig.ReadInt32("General", "首爆货币类型", Settings.首爆货币类型);
            Settings.首爆货币数量 = Settings.iniconfig.ReadInt32("General", "首爆货币数量", Settings.首爆货币数量);
            Settings.首爆道具编号 = Settings.iniconfig.ReadInt32("General", "首爆道具编号", Settings.首爆道具编号);
            Settings.首爆道具数量 = Settings.iniconfig.ReadInt32("General", "首爆道具数量", Settings.首爆道具数量);
            Settings.开启公会等级特效 = Settings.iniconfig.ReadBoolean("General", "开启公会等级特效", Settings.开启公会等级特效);
            Settings.开启公会职位特效 = Settings.iniconfig.ReadBoolean("General", "开启公会职位特效", Settings.开启公会职位特效);
            Settings.行会等级特效编号[1] = Settings.iniconfig.ReadUInt16("General", "一级行会特效编号", Settings.行会等级特效编号[1]);
            Settings.行会等级特效编号[2] = Settings.iniconfig.ReadUInt16("General", "二级行会特效编号", Settings.行会等级特效编号[2]);
            Settings.行会等级特效编号[3] = Settings.iniconfig.ReadUInt16("General", "三级行会特效编号", Settings.行会等级特效编号[3]);
            Settings.行会等级特效编号[4] = Settings.iniconfig.ReadUInt16("General", "四级行会特效编号", Settings.行会等级特效编号[4]);
            Settings.行会等级特效编号[5] = Settings.iniconfig.ReadUInt16("General", "五级行会特效编号", Settings.行会等级特效编号[5]);
            Settings.行会等级特效编号[6] = Settings.iniconfig.ReadUInt16("General", "六级行会特效编号", Settings.行会等级特效编号[6]);
            Settings.行会职位特效编号[1] = Settings.iniconfig.ReadUInt16("General", "行会会长特效编号", Settings.行会职位特效编号[1]);
            Settings.行会职位特效编号[2] = Settings.iniconfig.ReadUInt16("General", "行会副长特效编号", Settings.行会职位特效编号[2]);
            Settings.行会职位特效编号[3] = Settings.iniconfig.ReadUInt16("General", "行会长老特效编号", Settings.行会职位特效编号[3]);
            Settings.行会职位特效编号[4] = Settings.iniconfig.ReadUInt16("General", "行会监事特效编号", Settings.行会职位特效编号[4]);
            Settings.行会职位特效编号[5] = Settings.iniconfig.ReadUInt16("General", "行会理事特效编号", Settings.行会职位特效编号[5]);
            Settings.行会职位特效编号[6] = Settings.iniconfig.ReadUInt16("General", "行会执事特效编号", Settings.行会职位特效编号[6]);
            Settings.限制重要封包间隔时间 = Settings.iniconfig.ReadBoolean("General", "限制重要封包间隔时间", Settings.限制重要封包间隔时间);
            Settings.玩家出生地图 = Settings.iniconfig.ReadInt32("General", "玩家出生地图", Settings.玩家出生地图);
            Settings.游戏区服名称 = Settings.iniconfig.ReadString("General", "游戏区服名称", Settings.游戏区服名称);
            Settings.统计UUID代码 = Settings.iniconfig.ReadString("General", "统计UUID代码", Settings.统计UUID代码);
            Settings.开启lua = Settings.iniconfig.ReadBoolean("General", "开启lua", Settings.开启lua);
            Settings.触发装备重铸 = Settings.iniconfig.ReadBoolean("General", "触发装备重铸", Settings.触发装备重铸);
            Settings.资源包只能放材料 = Settings.iniconfig.ReadBoolean("General", "资源包只能放材料", Settings.资源包只能放材料);
            Settings.可购买玛法特权 = Settings.iniconfig.ReadBoolean("General", "可购买玛法特权", Settings.可购买玛法特权);
            Settings.暴击特效ID = Settings.iniconfig.ReadByte("General", "暴击特效ID", Settings.暴击特效ID);
            Settings.BOSS自动死亡 = Settings.iniconfig.ReadBoolean("General", "BOSS自动死亡", Settings.BOSS自动死亡);
            Settings.普通强化不碎武器 = Settings.iniconfig.ReadBoolean("General", "普通强化不碎武器", Settings.普通强化不碎武器);
            Settings.屏蔽七天活动 = Settings.iniconfig.ReadBoolean("General", "屏蔽七天活动", Settings.屏蔽七天活动);
            Settings.神佑掉落ID = Settings.iniconfig.ReadInt32("General", "神佑掉落ID", Settings.神佑掉落ID);
            Settings.商人比例 = Settings.iniconfig.ReadInt32("General", "商人比例", Settings.商人比例);
            Settings.充值公告 = Settings.iniconfig.ReadString("General", "充值公告", Settings.充值公告);
            Settings.商人发货公告 = Settings.iniconfig.ReadString("General", "商人发货公告", Settings.商人发货公告);
            Settings.屏蔽威望 = Settings.iniconfig.ReadBoolean("General", "屏蔽威望", Settings.屏蔽威望);
            Settings.屏蔽战功 = Settings.iniconfig.ReadBoolean("General", "屏蔽战功", Settings.屏蔽战功);
            Settings.屏蔽日程 = Settings.iniconfig.ReadBoolean("General", "屏蔽日程", Settings.屏蔽日程);
            Settings.屏蔽每周特惠 = Settings.iniconfig.ReadBoolean("General", "屏蔽每周特惠", Settings.屏蔽每周特惠);
            Settings.屏蔽每日签到 = Settings.iniconfig.ReadBoolean("General", "屏蔽每日签到", Settings.屏蔽每日签到);
            Settings.屏蔽传永武技 = Settings.iniconfig.ReadBoolean("General", "屏蔽传永武技", Settings.屏蔽传永武技);
            Settings.充值货币类型 = Settings.iniconfig.ReadInt32("General", "充值货币类型 ", Settings.充值货币类型);
            Settings.达最高级后继续加经验 = Settings.iniconfig.ReadBoolean("General", "达最高级后继续加经验", Settings.达最高级后继续加经验);
            Settings.EnvirPath = Path.Combine(Settings.游戏数据目录, "System\\Envir");
            Settings.NameListPath = Path.Combine(Settings.EnvirPath, "NameLists");
            Settings.NPCPath = Path.Combine(Settings.EnvirPath, "NPCs");
            Settings.ValuePath = Path.Combine(Settings.EnvirPath, "Values");
            Settings.祝福油几率0级 = Settings.iniconfig.ReadInt32("General", "祝福油几率0级 ", Settings.祝福油几率0级);
            Settings.祝福油几率1级 = Settings.iniconfig.ReadInt32("General", "祝福油几率1级 ", Settings.祝福油几率1级);
            Settings.祝福油几率2级 = Settings.iniconfig.ReadInt32("General", "祝福油几率2级 ", Settings.祝福油几率2级);
            Settings.祝福油几率3级 = Settings.iniconfig.ReadInt32("General", "祝福油几率3级 ", Settings.祝福油几率3级);
            Settings.祝福油几率4级 = Settings.iniconfig.ReadInt32("General", "祝福油几率4级 ", Settings.祝福油几率4级);
            Settings.祝福油几率5级 = Settings.iniconfig.ReadInt32("General", "祝福油几率5级 ", Settings.祝福油几率5级);
            Settings.祝福油几率6级 = Settings.iniconfig.ReadInt32("General", "祝福油几率6级 ", Settings.祝福油几率6级);
            Settings.死亡掉落剑甲 = Settings.iniconfig.ReadFloat("General", "死亡掉落剑甲 ", Settings.死亡掉落剑甲);
            Settings.死亡掉落首饰 = Settings.iniconfig.ReadFloat("General", "死亡掉落首饰 ", Settings.死亡掉落首饰);
            Settings.死亡掉落背包 = Settings.iniconfig.ReadFloat("General", "死亡掉落背包 ", Settings.死亡掉落背包);
            Settings.单次死亡限量 = Settings.iniconfig.ReadInt32("General", "单次死亡限量 ", Settings.单次死亡限量);
            Settings.红名掉落剑甲 = Settings.iniconfig.ReadFloat("General", "红名掉落剑甲 ", Settings.红名掉落剑甲);
            Settings.红名掉落首饰 = Settings.iniconfig.ReadFloat("General", "红名掉落首饰 ", Settings.红名掉落首饰);
            Settings.龙卫重铸费用 = Settings.iniconfig.ReadInt32("General", "龙卫重铸费用 ", Settings.龙卫重铸费用);
            Settings.锁单重铸费用 = Settings.iniconfig.ReadInt32("General", "锁单重铸费用 ", Settings.锁单重铸费用);
            Settings.锁半重铸费用 = Settings.iniconfig.ReadInt32("General", "锁半重铸费用 ", Settings.锁半重铸费用);
            Settings.行会最高人数 = Settings.iniconfig.ReadInt32("General", "行会最高人数 ", Settings.行会最高人数);
            Settings.幽冥海底节点开放天数 = Settings.iniconfig.ReadByte("General", "幽冥海底节点开放天数 ", Settings.幽冥海底节点开放天数);
            Settings.白日赤月节点开放天数 = Settings.iniconfig.ReadByte("General", "白日赤月节点开放天数 ", Settings.白日赤月节点开放天数);
            Settings.魔龙之城节点开放天数 = Settings.iniconfig.ReadByte("General", "魔龙之城节点开放天数 ", Settings.魔龙之城节点开放天数);
            Settings.苍月惊变节点开放天数 = Settings.iniconfig.ReadByte("General", "苍月惊变节点开放天数 ", Settings.苍月惊变节点开放天数);
            Settings.龙耀雪山节点开放天数 = Settings.iniconfig.ReadByte("General", "龙耀雪山节点开放天数 ", Settings.龙耀雪山节点开放天数);
            Settings.聊天限制等级 = Settings.iniconfig.ReadByte("General", "聊天限制等级 ", Settings.聊天限制等级);
            Settings.玛法新秀价格 = Settings.iniconfig.ReadInt32("General", "玛法新秀价格 ", Settings.玛法新秀价格);
            Settings.玛法名俊价格 = Settings.iniconfig.ReadInt32("General", "玛法名俊价格 ", Settings.玛法名俊价格);
            Settings.玛法豪杰价格 = Settings.iniconfig.ReadInt32("General", "玛法豪杰价格 ", Settings.玛法豪杰价格);
            Settings.玛法战将价格 = Settings.iniconfig.ReadInt32("General", "玛法战将价格 ", Settings.玛法战将价格);
            Settings.玛法至尊价格 = Settings.iniconfig.ReadInt32("General", "玛法至尊价格 ", Settings.玛法至尊价格);
            Settings.技巧项链倍数 = Settings.iniconfig.ReadFloat("General", "技巧项链倍数 ", Settings.技巧项链倍数);
            //新增
            Settings.金币自动入包 = Settings.iniconfig.ReadBoolean("General", "金币自动入包", Settings.金币自动入包);
            Settings.金币自动入包 = Settings.iniconfig.ReadBoolean("General", "银币自动入包", Settings.银币自动入包);
            Settings.物品自动入包 = Settings.iniconfig.ReadBoolean("General", "物品自动入包", Settings.物品自动入包);
            Settings.自动分解装备 = Settings.iniconfig.ReadBoolean("General", "自动分解装备", Settings.自动分解装备);
            Settings.不分解极品装备 = Settings.iniconfig.ReadBoolean("General", "不分解极品装备", Settings.不分解极品装备);

            Settings.安全区内满血满蓝 = Settings.iniconfig.ReadBoolean("General", "安全区内满血满蓝", Settings.安全区内满血满蓝);
            Settings.下线宝宝不死 = Settings.iniconfig.ReadBoolean("General", "下线宝宝不死", Settings.下线宝宝不死);

            for (int i = 0; i < 6; i++)
            {
                Settings.职业开放[i] = Settings.iniconfig.ReadBoolean("职业开放", "职业" + i, Settings.职业开放[i]);
            }
            Settings._0013_0014_0013_0009_0016_0006?.Invoke();
        }

        public static void Save()
        {
            Settings.iniconfig.Write("General", "客户连接端口", Settings.客户连接端口);
            Settings.iniconfig.Write("General", "门票接收端口", Settings.门票接收端口);
            Settings.iniconfig.Write("General", "门票来源白名单", Settings.门票来源白名单);
            Settings.iniconfig.Write("General", "封包限定数量", Settings.封包限定数量);
            Settings.iniconfig.Write("General", "异常屏蔽时间", Settings.异常屏蔽时间);
            Settings.iniconfig.Write("General", "掉线判定时间", Settings.掉线判定时间);
            Settings.iniconfig.Write("General", "游戏开放等级", Settings.游戏开放等级);
            Settings.iniconfig.Write("General", "装备特修折扣", Settings.装备特修折扣);
            Settings.iniconfig.Write("General", "怪物额外爆率", Settings.怪物额外爆率);
            Settings.iniconfig.Write("General", "怪物经验倍率", Settings.怪物经验倍率);
            Settings.iniconfig.Write("General", "减收益等级差", Settings.减收益等级差);
            Settings.iniconfig.Write("General", "收益减少比率", Settings.收益减少比率);
            Settings.iniconfig.Write("General", "怪物诱惑时长", Settings.怪物诱惑时长);
            Settings.iniconfig.Write("General", "物品清理时间", Settings.物品清理时间);
            Settings.iniconfig.Write("General", "物品归属时间", Settings.物品归属时间);
            Settings.iniconfig.Write("General", "游戏数据目录", Settings.游戏数据目录);
            Settings.iniconfig.Write("General", "数据备份目录", Settings.数据备份目录);
            Settings.iniconfig.Write("General", "系统公告内容", Settings.系统公告内容);
            Settings.iniconfig.Write("General", "新手扶持等级", Settings.新手扶持等级);
            Settings.iniconfig.Write("General", "自动保存时间", Settings.自动保存时间);
            Settings.iniconfig.Write("General", "武斗场时间一", Settings.武斗场时间一);
            Settings.iniconfig.Write("General", "武斗场时间二", Settings.武斗场时间二);
            Settings.iniconfig.Write("General", "武斗普通经验", Settings.武斗普通经验);
            Settings.iniconfig.Write("General", "武斗抢点经验", Settings.武斗抢点经验);
            Settings.iniconfig.Write("General", "开服日期", 计算类.日期转换(Settings.开服日期));
            Settings.iniconfig.Write("General", "开启线程发包", Settings.开启线程发包);
            Settings.iniconfig.Write("General", "开启自动战斗", Settings.开启自动战斗);
            Settings.iniconfig.Write("General", "使用新版内挂", Settings.使用新版内挂);
            Settings.iniconfig.Write("General", "沙巴克掉装备", Settings.沙巴克掉装备);
            Settings.iniconfig.Write("General", "攻沙开始时间小时", Settings.攻沙开始时间小时);
            Settings.iniconfig.Write("General", "攻沙开始时间分钟", Settings.攻沙开始时间分钟);
            Settings.iniconfig.Write("General", "攻沙结束时间小时", Settings.攻沙结束时间小时);
            Settings.iniconfig.Write("General", "攻沙结束时间分钟", Settings.攻沙结束时间分钟);
            Settings.iniconfig.Write("General", "攻城持续时间", Settings.攻城持续时间);
            Settings.iniconfig.Write("General", "第一层经验值", Settings.妖塔层经验[0]);
            Settings.iniconfig.Write("General", "第二层经验值", Settings.妖塔层经验[1]);
            Settings.iniconfig.Write("General", "第三层经验值", Settings.妖塔层经验[2]);
            Settings.iniconfig.Write("General", "第四层经验值", Settings.妖塔层经验[3]);
            Settings.iniconfig.Write("General", "第五层经验值", Settings.妖塔层经验[4]);
            Settings.iniconfig.Write("General", "第六层经验值", Settings.妖塔层经验[5]);
            Settings.iniconfig.Write("General", "第七层经验值", Settings.妖塔层经验[6]);
            Settings.iniconfig.Write("General", "第八层经验值", Settings.妖塔层经验[7]);
            Settings.iniconfig.Write("General", "第九层经验值", Settings.妖塔层经验[8]);
            Settings.iniconfig.Write("General", "普通妖塔秘境", Settings.妖塔层经验[9]);
            Settings.iniconfig.Write("General", "每日悬赏接取次数", Settings.每日悬赏接取次数);
            Settings.iniconfig.Write("General", "每周悬赏接取次数", Settings.每周悬赏接取次数);
            Settings.iniconfig.Write("General", "每日悬赏完成次数", Settings.每日悬赏完成次数);
            Settings.iniconfig.Write("General", "每周悬赏完成次数", Settings.每周悬赏完成次数);
            Settings.iniconfig.Write("General", "寄售上架上限", Settings.寄售上架上限);
            Settings.iniconfig.Write("学宫", "学宫奖励道具一", Settings.学宫奖励道具一);
            Settings.iniconfig.Write("学宫", "学宫奖励道具二", Settings.学宫奖励道具二);
            Settings.iniconfig.Write("学宫", "简单奖励道具一数量", Settings.学宫奖励数量[0, 0]);
            Settings.iniconfig.Write("学宫", "简单奖励道具二数量", Settings.学宫奖励数量[0, 1]);
            Settings.iniconfig.Write("学宫", "中等奖励道具一数量", Settings.学宫奖励数量[1, 0]);
            Settings.iniconfig.Write("学宫", "中等奖励道具二数量", Settings.学宫奖励数量[1, 1]);
            Settings.iniconfig.Write("学宫", "困难奖励道具一数量", Settings.学宫奖励数量[2, 0]);
            Settings.iniconfig.Write("学宫", "困难奖励道具二数量", Settings.学宫奖励数量[2, 1]);
            Settings.iniconfig.Write("General", "开启幸运倍率功能", Settings.开启幸运倍率功能);
            for (int 幸运档 = 0; 幸运档 < 10; 幸运档++)
            {
                Settings.iniconfig.Write("General", "幸运额外增伤值" + Settings.十档中文[幸运档], Settings.幸运额外增伤值[幸运档]);
                Settings.iniconfig.Write("General", "幸运增伤倍率值" + Settings.十档中文[幸运档], Settings.幸运增伤倍率值[幸运档]);
            }
            Settings.iniconfig.Write("General", "职业第一机制", Settings.职业第一机制);
            Settings.iniconfig.Write("General", "战士职业第一编号", Settings.职业第一编号[0]);
            Settings.iniconfig.Write("General", "法师职业第一编号", Settings.职业第一编号[1]);
            Settings.iniconfig.Write("General", "刺客职业第一编号", Settings.职业第一编号[2]);
            Settings.iniconfig.Write("General", "弓箭职业第一编号", Settings.职业第一编号[3]);
            Settings.iniconfig.Write("General", "道士职业第一编号", Settings.职业第一编号[4]);
            Settings.iniconfig.Write("General", "龙枪职业第一编号", Settings.职业第一编号[5]);
            Settings.iniconfig.Write("General", "开启怪物首杀功能", Settings.开启怪物首杀功能);
            Settings.iniconfig.Write("General", "开启怪物首杀货币", Settings.开启怪物首杀货币);
            Settings.iniconfig.Write("General", "开启怪物首杀道具", Settings.开启怪物首杀道具);
            Settings.iniconfig.Write("General", "开启首杀奖励邮件", Settings.开启首杀奖励邮件);
            Settings.iniconfig.Write("General", "开启装备首爆功能", Settings.开启装备首爆功能);
            Settings.iniconfig.Write("General", "开启装备首爆货币", Settings.开启装备首爆货币);
            Settings.iniconfig.Write("General", "开启装备首爆道具", Settings.开启装备首爆道具);
            Settings.iniconfig.Write("General", "开启首爆奖励邮件", Settings.开启首爆奖励邮件);
            Settings.iniconfig.Write("General", "首杀货币类型", Settings.首杀货币类型);
            Settings.iniconfig.Write("General", "首杀货币数量", Settings.首杀货币数量);
            Settings.iniconfig.Write("General", "首杀道具编号", Settings.首杀道具编号);
            Settings.iniconfig.Write("General", "首杀道具数量", Settings.首杀道具数量);
            Settings.iniconfig.Write("General", "首爆货币类型", Settings.首爆货币类型);
            Settings.iniconfig.Write("General", "首爆货币数量", Settings.首爆货币数量);
            Settings.iniconfig.Write("General", "首爆道具编号", Settings.首爆道具编号);
            Settings.iniconfig.Write("General", "首爆道具数量", Settings.首爆道具数量);
            Settings.iniconfig.Write("General", "开启公会等级特效", Settings.开启公会等级特效);
            Settings.iniconfig.Write("General", "开启公会职位特效", Settings.开启公会职位特效);
            Settings.iniconfig.Write("General", "一级行会特效编号", Settings.行会等级特效编号[1]);
            Settings.iniconfig.Write("General", "二级行会特效编号", Settings.行会等级特效编号[2]);
            Settings.iniconfig.Write("General", "三级行会特效编号", Settings.行会等级特效编号[3]);
            Settings.iniconfig.Write("General", "四级行会特效编号", Settings.行会等级特效编号[4]);
            Settings.iniconfig.Write("General", "五级行会特效编号", Settings.行会等级特效编号[5]);
            Settings.iniconfig.Write("General", "六级行会特效编号", Settings.行会等级特效编号[6]);
            Settings.iniconfig.Write("General", "行会会长特效编号", Settings.行会职位特效编号[1]);
            Settings.iniconfig.Write("General", "行会副长特效编号", Settings.行会职位特效编号[2]);
            Settings.iniconfig.Write("General", "行会长老特效编号", Settings.行会职位特效编号[3]);
            Settings.iniconfig.Write("General", "行会监事特效编号", Settings.行会职位特效编号[4]);
            Settings.iniconfig.Write("General", "行会理事特效编号", Settings.行会职位特效编号[5]);
            Settings.iniconfig.Write("General", "行会执事特效编号", Settings.行会职位特效编号[6]);
            Settings.iniconfig.Write("General", "限制重要封包间隔时间", Settings.限制重要封包间隔时间);
            Settings.iniconfig.Write("General", "开启任务系统", Settings.开启任务系统);
            Settings.iniconfig.Write("General", "开启成就系统", Settings.开启成就系统);
            Settings.iniconfig.Write("General", "玩家出生地图", Settings.玩家出生地图);
            Settings.iniconfig.Write("General", "游戏区服名称", Settings.游戏区服名称);
            Settings.iniconfig.Write("General", "统计UUID代码", Settings.统计UUID代码);
            Settings.iniconfig.Write("General", "开启lua", Settings.开启lua);
            Settings.iniconfig.Write("General", "触发装备重铸", Settings.触发装备重铸);
            Settings.iniconfig.Write("General", "资源包只能放材料", Settings.资源包只能放材料);
            Settings.iniconfig.Write("General", "可购买玛法特权", Settings.可购买玛法特权);
            Settings.iniconfig.Write("General", "暴击特效ID", Settings.暴击特效ID);
            Settings.iniconfig.Write("General", "BOSS自动死亡", Settings.BOSS自动死亡);
            Settings.iniconfig.Write("General", "普通强化不碎武器", Settings.普通强化不碎武器);
            Settings.iniconfig.Write("General", "屏蔽七天活动", Settings.屏蔽七天活动);
            Settings.iniconfig.Write("General", "神佑掉落ID", Settings.神佑掉落ID);
            Settings.iniconfig.Write("General", "商人比例", Settings.商人比例);
            Settings.iniconfig.Write("General", "充值公告", Settings.充值公告);
            Settings.iniconfig.Write("General", "商人发货公告", Settings.商人发货公告);
            Settings.iniconfig.Write("General", "屏蔽威望", Settings.屏蔽威望);
            Settings.iniconfig.Write("General", "屏蔽战功", Settings.屏蔽战功);
            Settings.iniconfig.Write("General", "屏蔽日程", Settings.屏蔽日程);
            Settings.iniconfig.Write("General", "屏蔽每周特惠", Settings.屏蔽每周特惠);
            Settings.iniconfig.Write("General", "屏蔽每日签到", Settings.屏蔽每日签到);
            Settings.iniconfig.Write("General", "屏蔽传永武技", Settings.屏蔽传永武技);
            Settings.iniconfig.Write("General", "充值货币类型 ", Settings.充值货币类型);
            Settings.iniconfig.Write("General", "达最高级后继续加经验", Settings.达最高级后继续加经验);
            Settings.iniconfig.Write("General", "祝福油几率0级 ", Settings.祝福油几率0级);
            Settings.iniconfig.Write("General", "祝福油几率1级 ", Settings.祝福油几率1级);
            Settings.iniconfig.Write("General", "祝福油几率2级 ", Settings.祝福油几率2级);
            Settings.iniconfig.Write("General", "祝福油几率3级 ", Settings.祝福油几率3级);
            Settings.iniconfig.Write("General", "祝福油几率4级 ", Settings.祝福油几率4级);
            Settings.iniconfig.Write("General", "祝福油几率5级 ", Settings.祝福油几率5级);
            Settings.iniconfig.Write("General", "祝福油几率6级 ", Settings.祝福油几率6级);
            Settings.iniconfig.Write("General", "死亡掉落剑甲 ", Settings.死亡掉落剑甲);
            Settings.iniconfig.Write("General", "死亡掉落首饰 ", Settings.死亡掉落首饰);
            Settings.iniconfig.Write("General", "死亡掉落背包 ", Settings.死亡掉落背包);
            Settings.iniconfig.Write("General", "单次死亡限量 ", Settings.单次死亡限量);
            Settings.iniconfig.Write("General", "红名掉落剑甲 ", Settings.红名掉落剑甲);
            Settings.iniconfig.Write("General", "红名掉落首饰 ", Settings.红名掉落首饰);
            Settings.iniconfig.Write("General", "龙卫重铸费用 ", Settings.龙卫重铸费用);
            Settings.iniconfig.Write("General", "锁单重铸费用 ", Settings.锁单重铸费用);
            Settings.iniconfig.Write("General", "锁半重铸费用 ", Settings.锁半重铸费用);
            Settings.iniconfig.Write("General", "行会最高人数 ", Settings.行会最高人数);
            Settings.iniconfig.Write("General", "幽冥海底节点开放天数 ", Settings.幽冥海底节点开放天数);
            Settings.iniconfig.Write("General", "白日赤月节点开放天数 ", Settings.白日赤月节点开放天数);
            Settings.iniconfig.Write("General", "魔龙之城节点开放天数 ", Settings.魔龙之城节点开放天数);
            Settings.iniconfig.Write("General", "苍月惊变节点开放天数 ", Settings.苍月惊变节点开放天数);
            Settings.iniconfig.Write("General", "龙耀雪山节点开放天数 ", Settings.龙耀雪山节点开放天数);
            Settings.iniconfig.Write("General", "聊天限制等级 ", Settings.聊天限制等级);
            Settings.iniconfig.Write("General", "玛法新秀价格 ", Settings.玛法新秀价格);
            Settings.iniconfig.Write("General", "玛法名俊价格 ", Settings.玛法名俊价格);
            Settings.iniconfig.Write("General", "玛法豪杰价格 ", Settings.玛法豪杰价格);
            Settings.iniconfig.Write("General", "玛法战将价格 ", Settings.玛法战将价格);
            Settings.iniconfig.Write("General", "玛法至尊价格 ", Settings.玛法至尊价格);
            Settings.iniconfig.Write("General", "技巧项链倍数 ", Settings.技巧项链倍数);
            //新增
            Settings.iniconfig.Write("General", "金币自动入包", Settings.金币自动入包);
            Settings.iniconfig.Write("General", "银币自动入包", Settings.银币自动入包);
            Settings.iniconfig.Write("General", "物品自动入包", Settings.物品自动入包);
            Settings.iniconfig.Write("General", "自动分解装备", Settings.自动分解装备);
            Settings.iniconfig.Write("General", "不分解极品装备", Settings.不分解极品装备);

            Settings.iniconfig.Write("General", "安全区内满血满蓝", Settings.安全区内满血满蓝);
            Settings.iniconfig.Write("General", "下线宝宝不死", Settings.下线宝宝不死);


            for (int i = 0; i < 6; i++)
            {
                Settings.iniconfig.Write("职业开放", "职业" + i, Settings.职业开放[i]);
            }
            Settings._0004_0002_0004_0008_0002_0006_0002_0005_0007_0002?.Invoke();
        }
    }
}
