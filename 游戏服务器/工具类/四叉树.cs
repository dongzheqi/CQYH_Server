using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace 游戏服务器.工具类
{
    // 四叉树空间索引(借鉴同源): 按对象中心坐标组织, 替代地图邻居查找的 625 格(25×25)双重扫描, 把每次移动的邻居更新从 O(625) 降到 O(log n + k)。
    // 用法: 对象进出网格(绑定网格/解绑网格)时 插入/移除; 邻居查找用 查询范围(中心±12 的矩形)。
    // 关键不变量: 移除依赖"对象当前坐标"定位所在节点, 故移动对象必须"先在旧坐标移除→更新坐标→在新坐标插入"。
    //   YH 的移动序列(地图对象.解绑网格()→当前坐标=新→绑定网格())天然满足: 解绑在旧坐标处移除、绑定在新坐标处插入。
    public class 四叉树<T> where T : class
    {
        private readonly 四叉树节点<T> _root;

        private readonly Func<T, Point> _getPosition;

        private readonly Func<T, string> _getDescription;

        private int _objectCount;

        private int _insertFailCount;

        private readonly List<四叉树插入失败详情> _failDetails;

        private const int 最大失败详情数 = 50;

        public int 对象数量 => this._objectCount;

        public int 插入失败数量 => this._insertFailCount;

        public IReadOnlyList<四叉树插入失败详情> 失败详情列表 => this._failDetails;

        public 四叉树(Rectangle bounds, int maxObjects, int maxLevels, Func<T, Point> getPosition, Func<T, string> getDescription = null)
        {
            this._root = new 四叉树节点<T>(0, bounds, maxObjects, maxLevels);
            this._getPosition = getPosition ?? throw new ArgumentNullException(nameof(getPosition));
            this._getDescription = getDescription ?? ((T obj) => obj?.ToString() ?? "null");
            this._objectCount = 0;
            this._insertFailCount = 0;
            this._failDetails = new List<四叉树插入失败详情>();
        }

        public void 插入(T obj)
        {
            if (obj == null)
            {
                return;
            }
            Point pos = this._getPosition(obj);
            if (this._root.插入(obj, pos))
            {
                this._objectCount++;
                return;
            }
            this._insertFailCount++;
            if (this._failDetails.Count < 最大失败详情数)
            {
                this._failDetails.Add(new 四叉树插入失败详情
                {
                    对象描述 = this._getDescription(obj),
                    对象坐标 = pos,
                    树边界 = this._root.边界,
                    失败原因 = this.分析失败原因(pos)
                });
            }
        }

        private string 分析失败原因(Point p)
        {
            Rectangle b = this._root.边界;
            if (p.X < b.X)
            {
                return $"X({p.X})<左界({b.X})";
            }
            if (p.X >= b.X + b.Width)
            {
                return $"X({p.X})>=右界({b.X + b.Width})";
            }
            if (p.Y < b.Y)
            {
                return $"Y({p.Y})<上界({b.Y})";
            }
            if (p.Y >= b.Y + b.Height)
            {
                return $"Y({p.Y})>=下界({b.Y + b.Height})";
            }
            return "未知(坐标在界内但插入失败)";
        }

        public bool 移除(T obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (this._root.移除(obj, this._getPosition(obj)))
            {
                this._objectCount--;
                return true;
            }
            return false;
        }

        public List<T> 查询范围(Rectangle range)
        {
            List<T> result = new List<T>();
            this._root.查询范围(range, result);
            return result;
        }

        public void 清空()
        {
            this._root.清空();
            this._objectCount = 0;
            this._insertFailCount = 0;
            this._failDetails.Clear();
        }

        public string 获取统计信息()
        {
            return $"四叉树[对象数:{this._objectCount}, 插入失败:{this._insertFailCount}, 区域:{this._root.边界}]";
        }

        public string 获取失败详情摘要(int 最大显示数 = 10)
        {
            if (this._failDetails.Count == 0)
            {
                return "无失败记录";
            }
            StringBuilder sb = new StringBuilder();
            int n = Math.Min(this._failDetails.Count, 最大显示数);
            for (int i = 0; i < n; i++)
            {
                sb.AppendLine($"  [{i + 1}] {this._failDetails[i]}");
            }
            if (this._insertFailCount > this._failDetails.Count)
            {
                sb.AppendLine($"  (总失败数:{this._insertFailCount}, 仅记录前{最大失败详情数}条详情)");
            }
            return sb.ToString();
        }
    }
}
