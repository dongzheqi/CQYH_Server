using System.Collections.Generic;
using System.Text;
using 游戏服务器.数据类;

namespace 游戏服务器.管理命令;

// 借鉴移植(参考引擎): 打印角色的 Y/F/U/T/J/Q 各类脚本/任务变量, 排查卡任务/日程/阵营/副本进度。
// 原文件为反编译 goto 分支, 移植时重写为可读 switch; 与 设置角色变量 成对。
public sealed class 查看角色变量 : GM命令
{
	[字段描述(0, 排序 = 0)]
	public string 角色名字;

	[字段描述(0, 排序 = 1, 可选 = true)]
	public string 变量类型;

	public override 执行方式 执行方式 => 执行方式.优先后台执行;

	public override void 执行命令()
	{
		if (!游戏数据网关.角色数据表.检索表.TryGetValue(角色名字, out var value) || !(value is 角色数据 角色数据))
		{
			主程.添加命令日志("<= @" + GetType().Name + " 命令执行失败, 角色不存在: " + 角色名字);
			return;
		}
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("━━━━━━━━━━━━━ 角色 [" + 角色名字 + "] 数据总览 ━━━━━━━━━━━━━");
		if (string.IsNullOrEmpty(变量类型))
		{
			输出字典(sb, "Y变量-角色变量 (日程/阵营/副本)", 角色数据.角色变量);
			输出字典(sb, "F变量-脚本变量", 角色数据.脚本变量);
			输出字典(sb, "U变量-脚本数字", 角色数据.脚本数字);
			输出字典(sb, "T变量-脚本字符", 角色数据.脚本字符);
			输出字典(sb, "J变量-零点数字", 角色数据.零点数字);
			输出字典(sb, "Q变量-任务标识 (0=未完成/1=已完成)", 角色数据.任务标识);
		}
		else
		{
			switch (变量类型)
			{
			case "Y变量":
			case "角色变量":
				输出字典(sb, "Y变量-角色变量", 角色数据.角色变量);
				break;
			case "F变量":
			case "脚本变量":
				输出字典(sb, "F变量-脚本变量", 角色数据.脚本变量);
				break;
			case "U变量":
			case "脚本数字":
				输出字典(sb, "U变量-脚本数字", 角色数据.脚本数字);
				break;
			case "T变量":
			case "脚本字符":
				输出字典(sb, "T变量-脚本字符", 角色数据.脚本字符);
				break;
			case "J变量":
			case "零点数字":
				输出字典(sb, "J变量-零点数字", 角色数据.零点数字);
				break;
			case "任务标识":
				输出字典(sb, "任务标识", 角色数据.任务标识);
				break;
			default:
				主程.添加命令日志("<= @" + GetType().Name + " 不支持的变量类型: " + 变量类型);
				return;
			}
		}
		sb.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
		sb.AppendLine("常用: 706=是否加入阵营  707=染血之锋  708=神树守护");
		主程.添加命令日志(sb.ToString());
	}

	private static void 输出字典<T>(StringBuilder sb, string 标题, 字典监视器<int, T> 字典)
	{
		if (字典 == null || 字典.Count == 0)
		{
			return;
		}
		string 前缀 = "";
		if (标题.Contains("J变量") || 标题.Contains("零点数字")) 前缀 = "J";
		else if (标题.Contains("T变量") || 标题.Contains("脚本字符")) 前缀 = "T";
		else if (标题.Contains("U变量") || 标题.Contains("脚本数字")) 前缀 = "U";
		else if (标题.Contains("F变量") || 标题.Contains("脚本变量")) 前缀 = "F";
		else if (标题.Contains("Y变量") || 标题.Contains("角色变量") || 标题.Contains("日程") || 标题.Contains("阵营")) 前缀 = "Y";
		else if (标题.Contains("任务标识")) 前缀 = "Q";
		sb.AppendLine();
		sb.AppendLine($"  ▸ {标题} ({字典.Count} 项)");
		sb.AppendLine("  " + new string('─', 60));
		int num = 0;
		foreach (KeyValuePair<int, T> item in 字典)
		{
			sb.Append($"{前缀}{item.Key}={item.Value,-8}");
			num++;
			if (num % 5 == 0)
			{
				sb.AppendLine();
				sb.Append("  ");
			}
		}
		if (num % 5 != 0)
		{
			sb.AppendLine();
		}
	}
}
