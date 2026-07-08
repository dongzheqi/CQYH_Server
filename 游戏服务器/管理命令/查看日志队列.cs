namespace 游戏服务器.管理命令
{
	// 借鉴移植(参考引擎): 打印各日志队列深度, 排查"队列满则静默丢弃日志"的盲区。
	// 参照原用 Logger.CommandLog(Logger.TotalLogsInfo()) 依赖参照专有 Logger 结构, 我方改用 主程.日志队列摘要()。
	public sealed class 查看日志队列 : GM命令
	{
		public override 执行方式 执行方式 => 执行方式.优先后台执行;

		public override void 执行命令()
		{
			主程.添加命令日志(主程.日志队列摘要());
		}
	}
}
