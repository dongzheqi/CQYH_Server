using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace 游戏服务器.模板类
{
	public class 铭文洗炼技能
	{
		public static Dictionary<ushort, 铭文洗炼技能> 数据表;

		public static Dictionary<byte, ushort[]> 普通节点铭文 = new Dictionary<byte, ushort[]>
		{
			{
				0,
				new ushort[6] { 10324, 10362, 10361, 10381, 10382, 10422 }
			},
			{
				1,
				new ushort[9] { 25331, 25332, 25363, 25372, 25401, 25402, 25353, 25392, 25503 }
			},
			{
				2,
				new ushort[6] { 15323, 15381, 15354, 15342, 15391, 15411 }
			},
			{
				3,
				new ushort[7] { 20441, 20482, 20502, 20511, 20513, 20442, 20563 }
			},
			{
				4,
				new ushort[8] { 30051, 30032, 30121, 30103, 30082, 30083, 30152, 30104 }
			},
			{
				5,
				new ushort[7] { 12042, 12071, 12081, 12082, 12083, 12092, 12113 }
			}
		};

		public static Dictionary<byte, ushort> 普通最终铭文 = new Dictionary<byte, ushort>
		{
			{ 0, 10361 },
			{ 1, 25363 },
			{ 2, 15381 },
			{ 3, 20482 },
			{ 4, 30103 },
			{ 5, 12081 }
		};

		public static Dictionary<byte, ushort[]> 高级节点铭文 = new Dictionary<byte, ushort[]>
		{
			{
				0,
				new ushort[6] { 10324, 10362, 10361, 10381, 10382, 10422 }
			},
			{
				1,
				new ushort[7] { 25331, 25332, 25363, 25372, 25401, 25402, 25392 }
			},
			{
				2,
				new ushort[6] { 15323, 15381, 15354, 15342, 15391, 15411 }
			},
			{
				3,
				new ushort[7] { 20441, 20482, 20502, 20511, 20513, 20442, 20563 }
			},
			{
				4,
				new ushort[8] { 30051, 30032, 30121, 30103, 30082, 30083, 30152, 30104 }
			},
			{
				5,
				new ushort[7] { 12042, 12071, 12081, 12082, 12083, 12092, 12113 }
			}
		};

		public static Dictionary<byte, ushort[]> 高级最终铭文 = new Dictionary<byte, ushort[]>
		{
			{
				0,
				new ushort[3] { 10324, 10332, 10362 }
			},
			{
				1,
				new ushort[5] { 25331, 25342, 25362, 25402, 25353 }
			},
			{
				2,
				new ushort[4] { 15312, 15352, 15411, 15391 }
			},
			{
				3,
				new ushort[5] { 20431, 20451, 20472, 20502, 20513 }
			},
			{
				4,
				new ushort[4] { 30041, 30033, 30082, 30081 }
			},
			{
				5,
				new ushort[5] { 12042, 12054, 12082, 12092, 12113 }
			}
		};

		public static ushort[] 普通节点次数 = new ushort[3] { 1500, 3000, 4500 };

		public static ushort[] 高级节点次数 = new ushort[6] { 1500, 3000, 4500, 6000, 7500, 9000 };

		public static ushort 普通节点最终次数 = 5000;

		public static ushort 高级节点最终次数 = 10000;

		public static int 普通节点数_ = 1;

		public static int 高级节点数_ = 1;

		public Dictionary<byte, ushort[]> 普通节点;

		public Dictionary<byte, ushort> 普通最终;

		public Dictionary<byte, ushort[]> 高级节点;

		public Dictionary<byte, ushort[]> 高级最终;

		public ushort[] 普通节点触发次数;

		public ushort 普通最终触发次数;

		public ushort[] 高级节点触发次数;

		public ushort 高级最终触发次数;

		public int 普通节点数;

		public int 高级节点数;

		public static void 载入数据()
		{
			铭文洗炼技能.数据表 = new Dictionary<ushort, 铭文洗炼技能>();
			string text;
			text = Settings.游戏数据目录 + "\\System\\装备养成\\铭文洗炼技能.json";
			if (File.Exists(text))
			{
				string value;
				value = File.ReadAllText(text);
				try
				{
					铭文洗炼技能 铭文洗炼技能2;
					铭文洗炼技能2 = (铭文洗炼技能)JsonConvert.DeserializeObject(value, typeof(铭文洗炼技能), 序列化类.全局设置);
					if (铭文洗炼技能2 != null)
					{
						铭文洗炼技能.普通节点铭文 = 铭文洗炼技能2.普通节点;
						铭文洗炼技能.普通最终铭文 = 铭文洗炼技能2.普通最终;
						铭文洗炼技能.高级节点铭文 = 铭文洗炼技能2.高级节点;
						铭文洗炼技能.高级最终铭文 = 铭文洗炼技能2.高级最终;
						铭文洗炼技能.普通节点数_ = 铭文洗炼技能2.普通节点数;
						铭文洗炼技能.高级节点数_ = 铭文洗炼技能2.高级节点数;
						if (铭文洗炼技能2.普通节点触发次数 != null)
						{
							铭文洗炼技能.普通节点次数 = 铭文洗炼技能2.普通节点触发次数;
						}
						if (铭文洗炼技能2.普通最终触发次数 != 0)
						{
							铭文洗炼技能.普通节点最终次数 = 铭文洗炼技能2.普通最终触发次数;
						}
						if (铭文洗炼技能2.高级节点触发次数 != null)
						{
							铭文洗炼技能.高级节点次数 = 铭文洗炼技能2.高级节点触发次数;
						}
						if (铭文洗炼技能2.高级最终触发次数 != 0)
						{
							铭文洗炼技能.高级节点最终次数 = 铭文洗炼技能2.高级最终触发次数;
						}
						铭文洗炼技能.数据表.Add(0, 铭文洗炼技能2);
					}
					return;
				}
				catch (Exception ex)
				{
					主程.添加系统日志(text + " " + ex.Message);
					return;
				}
			}
			try
			{
				铭文洗炼技能 铭文洗炼技能3;
				铭文洗炼技能3 = new 铭文洗炼技能();
				铭文洗炼技能3.普通节点 = 铭文洗炼技能.普通节点铭文;
				铭文洗炼技能3.普通最终 = 铭文洗炼技能.普通最终铭文;
				铭文洗炼技能3.高级节点 = 铭文洗炼技能.高级节点铭文;
				铭文洗炼技能3.高级最终 = 铭文洗炼技能.高级最终铭文;
				铭文洗炼技能3.普通节点数 = 铭文洗炼技能.普通节点数_;
				铭文洗炼技能3.高级节点数 = 铭文洗炼技能.高级节点数_;
				铭文洗炼技能3.普通节点触发次数 = 铭文洗炼技能.普通节点次数;
				铭文洗炼技能3.普通最终触发次数 = 铭文洗炼技能.普通节点最终次数;
				铭文洗炼技能3.高级节点触发次数 = 铭文洗炼技能.高级节点次数;
				铭文洗炼技能3.高级最终触发次数 = 铭文洗炼技能.高级节点最终次数;
				铭文洗炼技能.数据表.Add(0, 铭文洗炼技能3);
				string value2;
				value2 = JsonConvert.SerializeObject(铭文洗炼技能3, typeof(铭文洗炼技能), 序列化类.全局设置);
				StreamWriter streamWriter;
				streamWriter = File.CreateText(text);
				streamWriter.Write(value2);
				streamWriter.Close();
			}
			catch (Exception ex2)
			{
				主程.添加系统日志(text + " " + ex2.Message);
			}
		}
	}
}
