using System;
using System.Collections.Generic;
using System.IO;

namespace 游戏服务器.模板类
{
	public class 游戏天赋
	{
		public static Dictionary<byte, 游戏天赋> 数据表;

		public string 天赋名字;

		public Dictionary<byte, 天赋条件> 条件列表;

		public Dictionary<byte, Dictionary<游戏对象属性, int>>[] 属性列表;

		public Dictionary<byte, 天赋刻印>[] 刻印列表;

		public static void 获取天赋注入物品(byte 天赋位置, byte 天赋等级, out int 物品编号, out int 物品数量)
		{
			物品编号 = 0;
			物品数量 = 0;
			if (游戏天赋.数据表.TryGetValue(天赋位置, out var value) && value.条件列表.TryGetValue(天赋等级, out var value2))
			{
				物品编号 = value2.注入物品;
				物品数量 = value2.注入数量;
			}
		}

		public static void 获取天赋突破物品(byte 天赋位置, byte 天赋等级, out int 物品编号, out int 物品数量)
		{
			物品编号 = 0;
			物品数量 = 0;
			if (游戏天赋.数据表.TryGetValue(天赋位置, out var value) && value.条件列表.TryGetValue(天赋等级, out var value2))
			{
				物品编号 = value2.突破物品;
				物品数量 = value2.突破数量;
			}
		}

		public static int 获取等级经验(byte 天赋位置, byte 天赋等级)
		{
			if (游戏天赋.数据表.TryGetValue(天赋位置, out var value) && value.条件列表.TryGetValue(天赋等级, out var value2))
			{
				return value2.升级经验;
			}
			return 0;
		}

		public static void 载入数据()
		{
			游戏天赋.数据表 = new Dictionary<byte, 游戏天赋>();
			string[] array;
			array = File.ReadAllLines(Settings.游戏数据目录 + "\\System\\玩家成长\\天赋属性.txt");
			int num;
			num = 0;
			string[] array2;
			array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3;
				array3 = array2[i].Split("\t");
				byte key;
				key = Convert.ToByte(array3[0]);
				string text;
				text = array3[1];
				if (!游戏天赋.数据表.TryGetValue(key, out var value))
				{
					value = new 游戏天赋();
					value.天赋名字 = text;
					value.条件列表 = new Dictionary<byte, 天赋条件>();
					value.属性列表 = new Dictionary<byte, Dictionary<游戏对象属性, int>>[6];
					value.刻印列表 = new Dictionary<byte, 天赋刻印>[6];
					游戏天赋.数据表.Add(key, value);
				}
				游戏对象职业 游戏对象职业;
				游戏对象职业 = 计算类.字符转职业(array3[2]);
				游戏对象属性 key2;
				key2 = 计算类.字符转属性(array3[3]);
				string[] array4;
				array4 = array3[5].Split(",");
				num = 0;
				for (int j = 0; j < array4.Length; j++)
				{
					num += Convert.ToInt32(array4[j]);
					if (value.属性列表[(byte)游戏对象职业] == null)
					{
						value.属性列表[(byte)游戏对象职业] = new Dictionary<byte, Dictionary<游戏对象属性, int>>();
					}
					if (!value.属性列表[(byte)游戏对象职业].TryGetValue((byte)j, out var value2))
					{
						value2 = new Dictionary<游戏对象属性, int>();
						value.属性列表[(byte)游戏对象职业].Add((byte)j, value2);
					}
					if (value2.TryGetValue(key2, out var value3))
					{
						value2[key2] = value3 + num;
					}
					else
					{
						value2.Add(key2, num);
					}
				}
				游戏对象属性 key3;
				key3 = 计算类.字符转属性(array3[6]);
				string[] array5;
				array5 = array3[8].Split(",");
				num = 0;
				for (int k = 0; k < array5.Length; k++)
				{
					num += Convert.ToInt32(array5[k]);
					if (value.属性列表[(byte)游戏对象职业] == null)
					{
						value.属性列表[(byte)游戏对象职业] = new Dictionary<byte, Dictionary<游戏对象属性, int>>();
					}
					if (!value.属性列表[(byte)游戏对象职业].TryGetValue((byte)k, out var value4))
					{
						value4 = new Dictionary<游戏对象属性, int>();
						value.属性列表[(byte)游戏对象职业].Add((byte)k, value4);
					}
					if (value4.TryGetValue(key3, out var value5))
					{
						value4[key3] = value5 + num;
					}
					else
					{
						value4.Add(key3, num);
					}
				}
				if (value.刻印列表[(byte)游戏对象职业] == null)
				{
					value.刻印列表[(byte)游戏对象职业] = new Dictionary<byte, 天赋刻印>();
				}
				byte 开启等级;
				开启等级 = Convert.ToByte(array3[9]);
				string 刻印名字;
				刻印名字 = array3[10];
				ushort 刻印BUFF;
				刻印BUFF = Convert.ToUInt16(array3[11]);
				byte 刻印战力;
				刻印战力 = Convert.ToByte(array3[12]);
				value.刻印列表[(byte)游戏对象职业].Add(0, new 天赋刻印
				{
					刻印名字 = 刻印名字,
					刻印BUFF = 刻印BUFF,
					开启等级 = 开启等级,
					刻印战力 = 刻印战力
				});
				byte 开启等级2;
				开启等级2 = Convert.ToByte(array3[13]);
				string 刻印名字2;
				刻印名字2 = array3[14];
				ushort 刻印BUFF2;
				刻印BUFF2 = Convert.ToUInt16(array3[15]);
				byte 刻印战力2;
				刻印战力2 = Convert.ToByte(array3[16]);
				value.刻印列表[(byte)游戏对象职业].Add(1, new 天赋刻印
				{
					刻印名字 = 刻印名字2,
					刻印BUFF = 刻印BUFF2,
					开启等级 = 开启等级2,
					刻印战力 = 刻印战力2
				});
				byte 开启等级3;
				开启等级3 = Convert.ToByte(array3[17]);
				string 刻印名字3;
				刻印名字3 = array3[18];
				ushort 刻印BUFF3;
				刻印BUFF3 = Convert.ToUInt16(array3[19]);
				byte 刻印战力3;
				刻印战力3 = Convert.ToByte(array3[20]);
				value.刻印列表[(byte)游戏对象职业].Add(2, new 天赋刻印
				{
					刻印名字 = 刻印名字3,
					刻印BUFF = 刻印BUFF3,
					开启等级 = 开启等级3,
					刻印战力 = 刻印战力3
				});
			}
			array2 = File.ReadAllLines(Settings.游戏数据目录 + "\\System\\玩家成长\\天赋条件.txt");
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array6;
				array6 = array2[i].Split("\t");
				byte key4;
				key4 = Convert.ToByte(array6[0]);
				byte key5;
				key5 = Convert.ToByte(array6[1]);
				ushort 升级经验;
				升级经验 = Convert.ToUInt16(array6[2]);
				int 注入物品;
				注入物品 = Convert.ToInt32(array6[3]);
				int 注入数量;
				注入数量 = Convert.ToInt32(array6[4]);
				int 突破物品;
				突破物品 = Convert.ToInt32(array6[5]);
				int 突破数量;
				突破数量 = Convert.ToInt32(array6[6]);
				if (游戏天赋.数据表.TryGetValue(key4, out var value6))
				{
					value6.条件列表.Add(key5, new 天赋条件
					{
						升级经验 = 升级经验,
						注入数量 = 注入数量,
						注入物品 = 注入物品,
						突破数量 = 突破数量,
						突破物品 = 突破物品
					});
				}
			}
		}
	}
}
