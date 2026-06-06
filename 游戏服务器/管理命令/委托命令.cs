using System;

namespace 游戏服务器.管理命令
{
	// 通用「在游戏主线程执行一段闭包」的载体。
	// UI(如 GMToolView)不能直接改 角色数据/装备数据(会与服务循环竞态、字典监视器并发读会抛"集合已修改"),
	// 故把要做的事包成 Action 丢进 主程.外部命令 队列, 由主循环单线程 drain 执行(见 主程.循环.cs)。
	public sealed class 委托命令 : GM命令
	{
		private readonly Action 动作;

		// 供反射注册/误输入 @委托命令 时不抛异常; 实际使用走带 Action 的构造。
		public 委托命令()
		{
		}

		public 委托命令(Action 动作)
		{
			this.动作 = 动作;
		}

		public override 执行方式 执行方式 => 执行方式.优先后台执行;

		public override void 执行命令()
		{
			this.动作?.Invoke();
		}
	}
}
