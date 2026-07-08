using System;
using System.Collections.Generic;
using System.Linq;
using 游戏服务器.数据类;

namespace 游戏服务器.管理命令;

// 借鉴移植(参考引擎): 删除玩家指定技能。在线走 玩家移除技能; 离线手动清 技能数据/快捷栏位/关联Buff。
public sealed class 删除技能 : GM命令
{
	[字段描述(0, 排序 = 0)]
	public string 角色名字;

	[字段描述(0, 排序 = 1)]
	public ushort 技能Id;

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
			if (游戏数据网关.角色数据表.检索表.TryGetValue(角色名字, out var value) && value is 角色数据 角色数据)
			{
				if (角色数据.技能数据.TryGetValue(技能Id, out var v))
				{
					if (角色数据.网络连接?.绑定角色 != null)
					{
						角色数据.网络连接?.绑定角色?.玩家移除技能(技能Id);
					}
					else
					{
						foreach (ushort item in 角色数据.技能数据[技能Id].技能Buff)
						{
							角色数据.Buff数据.Remove(item);
						}
						角色数据.技能数据.Remove(技能Id);
						KeyValuePair<byte, 技能数据>[] array = 角色数据.快捷栏位.ToArray();
						for (int i = 0; i < array.Length; i++)
						{
							KeyValuePair<byte, 技能数据> keyValuePair = array[i];
							if (keyValuePair.Value.技能编号.V == 技能Id)
							{
								角色数据.快捷栏位.Remove(keyValuePair.Key);
							}
						}
					}
					角色数据.技能数据.Remove(技能Id);
					主程.添加命令日志($"<= @{GetType().Name} 命令已经执行, 玩家 {角色名字} 技能 {v.铭文模板.技能名字} 已删除");
				}
				else
				{
					主程.添加命令日志($"<= @{GetType().Name} 命令已经执行, 玩家 {角色名字} 技能ID {技能Id} 不存在");
				}
			}
			else
			{
				主程.添加命令日志("<= @" + GetType().Name + " 命令执行失败, 角色不存在");
			}
		}
		catch (Exception ex)
		{
			主程.添加命令日志("<= @" + GetType().Name + " 命令执行异常, " + ex.Message);
		}
	}
}
