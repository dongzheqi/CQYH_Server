using System;
using 游戏服务器.地图类;
using 游戏服务器.数据类;
using 游戏服务器.网络类;

namespace 游戏服务器.管理命令;

// 借鉴移植(参考引擎): 按容器(0装备/1背包/2仓库/5挂载/7资源包)删除玩家物品并下发。
// 适配: 参照对象池 ServerPackPool.Take<删除玩家物品>().Reset() 改为 new 删除玩家物品; 参照 删除数据(名,备注,caller,line) 我方为无参 删除数据()。
public sealed class 删除物品 : GM命令
{
	[字段描述(0, 排序 = 0)]
	public string 角色名字;

	[字段描述(0, 排序 = 1)]
	public byte 装备容器类型;

	[字段描述(0, 排序 = 2)]
	public byte 装备位置;

	public override 执行方式 执行方式 => 执行方式.前台立即执行;

	public override void 执行命令()
	{
		if (string.IsNullOrWhiteSpace(角色名字))
		{
			主程.添加命令日志("<= @" + GetType().Name + " 命令执行失败, 角色名不能为空");
			return;
		}
		try
		{
			if (游戏数据网关.角色数据表.检索表.TryGetValue(角色名字, out var value))
			{
				角色数据 角色数据 = value as 角色数据;
				if (角色数据 != null)
				{
					玩家实例 value2 = null;
					地图处理网关.玩家对象表?.TryGetValue(角色数据.角色编号, out value2);
					物品数据 装备 = null;
					装备数据 装备2 = null;
					switch (装备容器类型)
					{
					case 0:
						if (!角色数据.角色装备.TryGetValue(装备位置, out 装备2))
						{
							break;
						}
						角色数据.角色装备.Remove(装备位置);
						if (value2 != null)
						{
							value2.玩家穿卸装备((装备穿戴部位)装备2.物品位置.V, 装备2, null);
							value2.网络连接?.发送封包(new 删除玩家物品
							{
								背包类型 = 装备2.物品容器.V,
								物品位置 = 装备2.物品位置.V
							});
						}
						装备2.删除数据();
						break;
					case 1:
						if (角色数据.角色背包.TryGetValue(装备位置, out 装备))
						{
							角色数据.角色背包.Remove(装备位置);
							value2?.网络连接?.发送封包(new 删除玩家物品
							{
								背包类型 = 装备.物品容器.V,
								物品位置 = 装备.物品位置.V
							});
							装备.删除数据();
						}
						break;
					case 2:
						if (角色数据.角色仓库.TryGetValue(装备位置, out 装备))
						{
							角色数据.角色仓库.Remove(装备位置);
							value2?.网络连接?.发送封包(new 删除玩家物品
							{
								背包类型 = 装备.物品容器.V,
								物品位置 = 装备.物品位置.V
							});
							装备.删除数据();
						}
						break;
					case 5:
						if (角色数据.挂载物品.V != null)
						{
							物品数据 挂载 = 角色数据.挂载物品.V;
							角色数据.挂载物品.V = null;
							value2?.网络连接?.发送封包(new 删除玩家物品
							{
								背包类型 = 挂载.物品容器.V,
								物品位置 = 0
							});
							挂载.删除数据();
						}
						break;
					case 7:
						if (角色数据.角色资源包.TryGetValue(装备位置, out 装备))
						{
							角色数据.角色资源包.Remove(装备位置);
							value2?.网络连接?.发送封包(new 删除玩家物品
							{
								背包类型 = 装备.物品容器.V,
								物品位置 = 装备.物品位置.V
							});
							装备.删除数据();
						}
						break;
					}
					if (装备2 != null)
					{
						主程.添加命令日志($"<= @{GetType().Name} 命令已经执行, 玩家 {角色名字} 装备 {装备2?.对应模板?.V?.物品名字} 已删除");
					}
					if (装备 != null)
					{
						主程.添加命令日志($"<= @{GetType().Name} 命令已经执行, 玩家 {角色名字} 物品 {装备?.对应模板?.V?.物品名字} 已删除");
					}
					return;
				}
			}
			主程.添加命令日志("<= @" + GetType().Name + " 命令执行失败, 角色不存在");
		}
		catch (Exception ex)
		{
			主程.添加命令日志("<= @" + GetType().Name + " 命令执行异常, " + ex.Message);
		}
	}
}
