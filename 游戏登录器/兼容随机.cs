using System;
using System.Security.Cryptography;
using System.Text;

namespace 游戏登录器
{
    // net48 没有 RandomNumberGenerator.GetInt32 (.NET 5+) 与 SHA256.HashData (.NET 5+),
    // 这里提供等价、行为一致的密码学安全实现, 供登录器各处复用.
    internal static class 兼容随机
    {
        // 等价于 RandomNumberGenerator.GetInt32(minInclusive, maxExclusive):
        // 用拒绝采样消除取模偏置, 返回 [minInclusive, maxExclusive) 内均匀分布的整数.
        public static int 整数(int minInclusive, int maxExclusive)
        {
            if (minInclusive >= maxExclusive)
            {
                throw new ArgumentException("minInclusive 必须小于 maxExclusive");
            }
            uint 区间 = (uint)((long)maxExclusive - minInclusive);
            // 落在 [0, 区间) 内, 拒绝超过最大整数倍的尾部以避免偏置
            uint 上界 = uint.MaxValue - (uint.MaxValue % 区间);
            byte[] buf = new byte[4];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                uint 值;
                do
                {
                    rng.GetBytes(buf);
                    值 = BitConverter.ToUInt32(buf, 0);
                }
                while (值 >= 上界);
                return (int)(minInclusive + (值 % 区间));
            }
        }

        // 等价于 SHA256.HashData(data) (.NET 5+)
        public static byte[] SHA256哈希(byte[] data)
        {
            using (SHA256 sha = SHA256.Create())
            {
                return sha.ComputeHash(data);
            }
        }

        // net48 的 ProcessStartInfo 没有 ArgumentList (.NET Core 2.1+), 只能用单一 Arguments 字符串.
        // 这里按 Windows CommandLineToArgvW 的规则手工转义每个参数并空格拼接, 复刻
        // ArgumentList 的自动转义行为: 即便区服名/票据含特殊字符也无法破出参数边界 (纵深防御).
        public static string 拼接命令行(params string[] 参数列表)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 参数列表.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(' ');
                }
                引用单个参数(sb, 参数列表[i]);
            }
            return sb.ToString();
        }

        private static void 引用单个参数(StringBuilder sb, string 参数)
        {
            // 无空白与引号的非空参数无需加引号, 原样输出
            if (!string.IsNullOrEmpty(参数) && 参数.IndexOfAny(new[] { ' ', '\t', '\n', '\v', '"' }) < 0)
            {
                sb.Append(参数);
                return;
            }
            sb.Append('"');
            for (int i = 0; i < 参数.Length; i++)
            {
                int 反斜杠数 = 0;
                while (i < 参数.Length && 参数[i] == '\\')
                {
                    反斜杠数++;
                    i++;
                }
                if (i == 参数.Length)
                {
                    // 结尾的反斜杠要全部翻倍, 以免与收尾引号黏连被解析吞掉
                    sb.Append('\\', 反斜杠数 * 2);
                    break;
                }
                else if (参数[i] == '"')
                {
                    // 引号前的反斜杠翻倍 + 1, 再转义引号本身
                    sb.Append('\\', 反斜杠数 * 2 + 1);
                    sb.Append('"');
                }
                else
                {
                    sb.Append('\\', 反斜杠数);
                    sb.Append(参数[i]);
                }
            }
            sb.Append('"');
        }
    }
}
