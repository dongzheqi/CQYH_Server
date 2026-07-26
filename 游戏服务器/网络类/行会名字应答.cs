using System;

namespace 游戏服务器.网络类
{
	[封包信息描述(来源 = 封包来源.服务器, 编号 = 583, 长度 = 48, 注释 = "查询行会名字")]
	public sealed class 行会名字应答 : 游戏封包
	{
		[封包字段描述(下标 = 2, 长度 = 4)]
		public int 行会编号;

		[封包字段描述(下标 = 6, 长度 = 25)]
		public string 行会名字;

		[封包字段描述(下标 = 31, 长度 = 4)]
		public int 会长编号;

		// DateTime → int: 游戏封包.封包字段写入表 只注册了 bool/byte/sbyte/byte[]/short/ushort/int/uint/
		// long/ulong/string/Point 十二种类型, 没有 DateTime; 而 取字节()(游戏封包.cs:290) 用的是
		// TryGetValue —— 未注册类型**静默跳过、不报错**。于是本字段从来没被写进过封包, 客户端读到的
		// 第 35~38 字节恒为 0, 行会创建时间永远显示不出来。
		// 全封包层所有时间字段都是 int(长度 4), 本字段是唯一的 DateTime; 且 行会数据.cs:69 早就备好了
		// `public int 创建时间 => 计算类.时间转换(创建日期.V)`, 与全仓 `事记时间 = 计算类.时间转换(...)`
		// 同一编码约定 —— 只是调用点(玩家实例.公会师门.cs)错传了未转换的 创建日期.V, 一并改正。
		[封包字段描述(下标 = 35, 长度 = 4)]
		public int 创建时间;

		[封包字段描述(下标 = 39, 长度 = 1)]
		public byte 行会人数;

		[封包字段描述(下标 = 40, 长度 = 1)]
		public byte 行会等级;

		[封包字段描述(下标 = 41, 长度 = 1)]
		public byte 建筑等级;

		[封包字段描述(下标 = 46, 长度 = 1)]
		public byte 未知参数 = 1;

		public override ushort 封包编号 => 583;
	}
}
