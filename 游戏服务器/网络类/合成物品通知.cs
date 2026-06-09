namespace 游戏服务器.网络类
{
	[封包信息描述(来源 = 封包来源.客户端, 编号 = 261, 长度 = 6, 注释 = "合成物品通知")]
	public sealed class 合成物品通知 : 游戏封包
	{
		public override ushort 封包编号 => 261;
	}
}
