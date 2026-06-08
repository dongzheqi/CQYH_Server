using System.Drawing;

namespace 游戏服务器.工具类
{
    // 四叉树插入失败诊断: 记录坐标越界等导致插入失败的对象, 便于核对坐标系/边界配置(借鉴同源)。
    public class 四叉树插入失败详情
    {
        public string 对象描述 { get; set; }

        public Point 对象坐标 { get; set; }

        public Rectangle 树边界 { get; set; }

        public string 失败原因 { get; set; }

        public override string ToString()
        {
            return $"对象={对象描述}, 坐标=({对象坐标.X},{对象坐标.Y}), 边界={树边界}, 原因={失败原因}";
        }
    }
}
