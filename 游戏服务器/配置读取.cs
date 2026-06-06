using System;
using System.IO;
using System.Text;

namespace 游戏服务器
{
	// 统一的配置/CSV 文本读取入口: 自动识别 UTF-8(带/不带 BOM)、UTF-16 与 GBK/GB18030 编码, 解决两类老问题:
	//   ① File.OpenText 固定按 UTF-8 读取 —— 运营用 Excel/WPS(简体中文默认 GBK) 另存的 csv 里中文会乱码(静默腐蚀, 比崩服更难察觉);
	//   ② 个别加载器硬编码 Encoding.GetEncoding("GB18030") —— 该文件一旦被改存为 UTF-8 又会读乱.
	// .NET Core+ 默认不带代码页编码(936/54936), 静态构造里注册一次 CodePagesEncodingProvider(幂等, 已被本类 GB18030 回退依赖).
	public static class 配置读取
	{
		static 配置读取()
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		}

		// 检测字节序列编码: 优先 BOM(UTF-8 / UTF-16 LE / UTF-16 BE); 无 BOM 则严格按 UTF-8 试解码,
		// 含非法字节(抛 DecoderFallbackException) 即判定非 UTF-8、回退 GB18030(GBK 超集, 可解任意字节序列).
		public static Encoding 检测编码(byte[] 字节)
		{
			if (字节.Length >= 3 && 字节[0] == 0xEF && 字节[1] == 0xBB && 字节[2] == 0xBF)
			{
				return new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
			}
			if (字节.Length >= 2 && 字节[0] == 0xFF && 字节[1] == 0xFE)
			{
				return Encoding.Unicode;
			}
			if (字节.Length >= 2 && 字节[0] == 0xFE && 字节[1] == 0xFF)
			{
				return Encoding.BigEndianUnicode;
			}
			try
			{
				new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true).GetString(字节);
				return new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
			}
			catch (DecoderFallbackException)
			{
				return Encoding.GetEncoding("GB18030");
			}
		}

		// 按识别出的编码打开文本读取器, 替代 File.OpenText / new StreamReader(path, 固定编码).
		// 一次性读全字节(配置文件均不大)、据此选编码, 用 MemoryStream 回放, 单次磁盘 IO; 调用方 using 释放即连带释放内存流.
		public static StreamReader 打开(string 路径)
		{
			byte[] 字节;
			字节 = File.ReadAllBytes(路径);
			return new StreamReader(new MemoryStream(字节), 配置读取.检测编码(字节));
		}

		// 读取整份文本并按识别出的编码解码.
		public static string 读取全文(string 路径)
		{
			byte[] 字节;
			字节 = File.ReadAllBytes(路径);
			return 配置读取.检测编码(字节).GetString(字节);
		}
	}
}
