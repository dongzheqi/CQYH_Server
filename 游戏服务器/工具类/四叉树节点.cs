using System.Collections.Generic;
using System.Drawing;

namespace 游戏服务器.工具类
{
    // 四叉树节点(借鉴同源): 超过 maxObjects 且未达 maxLevels 时分裂为四个子节点。对象按中心坐标归入子节点, 跨界对象留在本节点。
    internal class 四叉树节点<T> where T : class
    {
        private readonly int _level;

        private readonly Rectangle _bounds;

        private readonly int _maxObjects;

        private readonly int _maxLevels;

        private readonly List<(T obj, Point pos)> _objects;

        private 四叉树节点<T>[] _nodes;

        public Rectangle 边界 => this._bounds;

        public 四叉树节点(int level, Rectangle bounds, int maxObjects, int maxLevels)
        {
            this._level = level;
            this._bounds = bounds;
            this._maxObjects = maxObjects;
            this._maxLevels = maxLevels;
            this._objects = new List<(T, Point)>();
            this._nodes = null;
        }

        private void 分裂()
        {
            int 半宽 = this._bounds.Width / 2;
            int 半高 = this._bounds.Height / 2;
            int x = this._bounds.X;
            int y = this._bounds.Y;
            int 右宽 = this._bounds.Width - 半宽;
            int 下高 = this._bounds.Height - 半高;
            this._nodes = new 四叉树节点<T>[4];
            this._nodes[0] = new 四叉树节点<T>(this._level + 1, new Rectangle(x + 半宽, y, 右宽, 半高), this._maxObjects, this._maxLevels);
            this._nodes[1] = new 四叉树节点<T>(this._level + 1, new Rectangle(x, y, 半宽, 半高), this._maxObjects, this._maxLevels);
            this._nodes[2] = new 四叉树节点<T>(this._level + 1, new Rectangle(x, y + 半高, 半宽, 下高), this._maxObjects, this._maxLevels);
            this._nodes[3] = new 四叉树节点<T>(this._level + 1, new Rectangle(x + 半宽, y + 半高, 右宽, 下高), this._maxObjects, this._maxLevels);
        }

        private int 获取索引(Point position)
        {
            if (this._nodes == null)
            {
                return -1;
            }
            int 中X = this._bounds.X + this._bounds.Width / 2;
            int 中Y = this._bounds.Y + this._bounds.Height / 2;
            bool 上 = position.Y < 中Y;
            bool 下 = position.Y >= 中Y;
            if (position.X < 中X)
            {
                if (上)
                {
                    return 1;
                }
                if (下)
                {
                    return 2;
                }
            }
            else
            {
                if (上)
                {
                    return 0;
                }
                if (下)
                {
                    return 3;
                }
            }
            return -1;
        }

        public bool 插入(T obj, Point position)
        {
            if (!this._bounds.Contains(position))
            {
                return false;
            }
            if (this._nodes != null)
            {
                int idx = this.获取索引(position);
                if (idx != -1)
                {
                    return this._nodes[idx].插入(obj, position);
                }
            }
            this._objects.Add((obj, position));
            if (this._objects.Count > this._maxObjects && this._level < this._maxLevels)
            {
                if (this._nodes == null)
                {
                    this.分裂();
                }
                int i = 0;
                while (i < this._objects.Count)
                {
                    (T obj, Point pos) 项 = this._objects[i];
                    int idx = this.获取索引(项.pos);
                    if (idx != -1)
                    {
                        this._nodes[idx].插入(项.obj, 项.pos);
                        this._objects.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            return true;
        }

        public bool 移除(T obj, Point position)
        {
            if (!this._bounds.Contains(position))
            {
                return false;
            }
            if (this._nodes != null)
            {
                int idx = this.获取索引(position);
                if (idx != -1 && this._nodes[idx].移除(obj, position))
                {
                    return true;
                }
            }
            for (int i = 0; i < this._objects.Count; i++)
            {
                if (this._objects[i].obj == obj)
                {
                    this._objects.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public void 查询范围(Rectangle range, List<T> result)
        {
            if (!this._bounds.IntersectsWith(range))
            {
                return;
            }
            foreach ((T obj, Point pos) 项 in this._objects)
            {
                if (range.Contains(项.pos))
                {
                    result.Add(项.obj);
                }
            }
            if (this._nodes != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    this._nodes[i].查询范围(range, result);
                }
            }
        }

        public void 清空()
        {
            this._objects.Clear();
            if (this._nodes != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    this._nodes[i].清空();
                }
                this._nodes = null;
            }
        }
    }
}
