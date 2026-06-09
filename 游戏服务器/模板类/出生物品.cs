using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using 游戏服务器.数据类;

namespace 游戏服务器.模板类
{
    // 新角色出生物品表: 把原本硬编码在 角色数据 构造函数里的新手装(药水/职业武器/箭袋/护身符/性别衣服)
    // 外置到 Database\System\玩家成长\出生物品.csv, 供运营直接编辑, 无需改码重编译。行为与原硬编码 1:1 等价。
    public class 出生物品
    {
        public static List<出生物品> 数据表;

        public string 职业;

        public string 性别;

        public string 物品;

        public string 容器;

        public byte 槽位;

        public int 持久;

        public bool 绑定;

        public bool 随机属性;

        public static void 载入数据()
        {
            出生物品.数据表 = new List<出生物品>();
            DataTable dataTable = new DataTable();
            using StreamReader reader = 配置读取.打开(Settings.游戏数据目录 + "\\System\\玩家成长\\出生物品.csv");
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                using CsvDataReader reader2 = new CsvDataReader(csv);
                dataTable.Load(reader2);
            }
            foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
            {
                出生物品 出生物品2 = new 出生物品();
                出生物品2.职业 = item["Class"].ToString();
                出生物品2.性别 = item["Gender"].ToString();
                出生物品2.物品 = item["Item"].ToString();
                出生物品2.容器 = item["Container"].ToString();
                出生物品2.槽位 = byte.Parse(item["Slot"].ToString());
                出生物品2.持久 = int.Parse(item["Durability"].ToString());
                出生物品2.绑定 = item["Bound"].ToString() == "1";
                出生物品2.随机属性 = item["RandomAttr"].ToString() == "1";
                出生物品.数据表.Add(出生物品2);
            }
        }

        // 创建角色时调用; 按 职业/性别 过滤并发放出生物品, 保真复现原 角色数据 构造函数的 if/else 逻辑。
        // 装备(武器/衣服)走 装备数据(可随机属性); 普通物(药水/箭袋/护身符)走 物品数据(可绑定);
        // 持久<0 表示用物品模板自带的默认物品持久; 物品缺失则静默跳过(与原 TryGetValue 同语义, 不崩服)。
        public static void 发放(角色数据 角色, 游戏对象职业 职业, 游戏对象性别 性别)
        {
            if (出生物品.数据表 == null)
            {
                return;
            }
            foreach (出生物品 行 in 出生物品.数据表)
            {
                if (行.职业 != "全职业" && 行.职业 != 职业.ToString())
                {
                    continue;
                }
                if (行.性别 != "全性别" && 行.性别 != 性别.ToString())
                {
                    continue;
                }
                游戏物品 模板 = null;
                if (int.TryParse(行.物品, out var 编号))
                {
                    游戏物品.数据表.TryGetValue(编号, out 模板);
                }
                else
                {
                    游戏物品.检索表.TryGetValue(行.物品, out 模板);
                }
                if (模板 == null)
                {
                    continue;
                }
                byte 容器 = ((行.容器 == "装备") ? ((byte)0) : ((byte)1));
                if (模板 is 游戏装备 装备模板)
                {
                    装备数据 装备 = new 装备数据(装备模板, 角色, 容器, 行.槽位, 随机生成: 行.随机属性);
                    if (容器 == 0)
                    {
                        角色.角色装备[行.槽位] = 装备;
                    }
                    else
                    {
                        角色.角色背包[行.槽位] = 装备;
                    }
                }
                else
                {
                    int 持久 = ((行.持久 < 0) ? 模板.物品持久 : 行.持久);
                    角色.角色背包[行.槽位] = new 物品数据(模板, 角色, 容器, 行.槽位, 持久, 绑定: 行.绑定);
                }
            }
        }
    }
}
