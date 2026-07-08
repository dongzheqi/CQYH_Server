using System.Text.Json.Serialization;

namespace 游戏服务器.性成优化配置;

// 借鉴移植(参考引擎): 性能/内存优化的持久化配置数据对象
public class 性能优化配置数据
{
	[JsonPropertyName("高性能模式")]
	public bool 高性能模式 { get; set; }

	[JsonPropertyName("内存监控间隔")]
	public int 内存监控间隔 { get; set; } = 1;


	[JsonPropertyName("启用详细内存日志")]
	public bool 启用详细内存日志 { get; set; }

	[JsonPropertyName("启用自动清理内存")]
	public bool 启用自动清理内存 { get; set; } = true;


	[JsonPropertyName("自动清理内存间隔")]
	public int 自动清理内存间隔 { get; set; } = 5;


	[JsonPropertyName("内存阈值MB")]
	public int 内存阈值MB { get; set; } = 1548;


	[JsonPropertyName("清理过期门票")]
	public bool 清理过期门票 { get; set; } = true;


	[JsonPropertyName("清理地图过期物品")]
	public bool 清理地图过期物品 { get; set; } = true;

}
