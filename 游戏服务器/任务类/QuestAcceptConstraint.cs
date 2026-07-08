namespace 游戏服务器.任务类
{
	public enum QuestAcceptConstraint
	{
		QuestCompleted = 0,
		MinLevel = 1,
		MaxLevel = 2,
		AcceptStartTime = 3,
		AcceptEndTime = 4,
		Job = 5,
		Gender = 6,
		// 借鉴移植(参考引擎): 补齐缺失的第8个约束值。缺此值时, 带 SelfVar 约束的任务模板在 CanAcceptQuest 走 default 抛
		// NotImplementedException 直接崩服。末尾追加=7与参照数值一致, 向后兼容存档/任务JSON。
		SelfVar = 7
	}
}
