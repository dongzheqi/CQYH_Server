using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace 游戏服务器.模板类
{
	public class 游戏坐骑
	{
		public static Dictionary<ushort, 游戏坐骑> 数据表;

		public static Dictionary<byte, Dictionary<游戏对象属性, int>> 御兽之力属性;

		public static Dictionary<byte, byte> 御兽之力栏数;

		public ushort 坐骑编号;

		public string 坐骑名字;

		public short 御兽之力;

		public ushort 魂兽BUFF;

		public byte 坐骑品质;

		public byte 需要等级;

		public int 速度加成;

		public Dictionary<游戏对象属性, int> 基础属性;

		[JsonIgnore]
		public Dictionary<游戏对象属性, int> 坐骑属性;

		public static void 载入数据()
		{
			Dictionary<ushort, 游戏坐骑> dictionary;
			dictionary = new Dictionary<ushort, 游戏坐骑>();
			string text;
			text = Settings.游戏数据目录 + "\\System\\游戏坐骑\\";
			if (Directory.Exists(text))
			{
				游戏坐骑[] array;
				array = 序列化类.反序列化<游戏坐骑>(text);
				foreach (游戏坐骑 游戏坐骑2 in array)
				{
					游戏坐骑2.坐骑属性 = new Dictionary<游戏对象属性, int>
					{
						{
							游戏对象属性.行走速度,
							游戏坐骑2.速度加成 / 800
						},
						{
							游戏对象属性.奔跑速度,
							游戏坐骑2.速度加成 / 800
						}
					};
					dictionary.Add(游戏坐骑2.坐骑编号, 游戏坐骑2);
				}
			}
			Dictionary<byte, Dictionary<游戏对象属性, int>> dictionary2;
			dictionary2 = new Dictionary<byte, Dictionary<游戏对象属性, int>>();
			Dictionary<byte, byte> dictionary3;
			dictionary3 = new Dictionary<byte, byte>();
			string[] array2;
			array2 = Regex.Split(File.ReadAllText(Settings.游戏数据目录 + "\\System\\玩家成长\\御兽之力.txt").Trim('\r', '\n', '\r'), "\r\n", RegexOptions.IgnoreCase);
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3;
				array3 = Regex.Split(array2[i], "\t", RegexOptions.IgnoreCase);
				Dictionary<游戏对象属性, int> dictionary4;
				dictionary4 = new Dictionary<游戏对象属性, int>();
				int num;
				num = Convert.ToInt32(array3[4]);
				int num2;
				num2 = Convert.ToInt32(array3[6]);
				int num3;
				num3 = Convert.ToInt32(array3[8]);
				int num4;
				num4 = Convert.ToInt32(array3[10]);
				int num5;
				num5 = Convert.ToInt32(array3[12]);
				if (num > 0)
				{
					dictionary4.Add(游戏对象属性.最大体力, num);
				}
				if (num2 > 0)
				{
					dictionary4.Add(游戏对象属性.最大防御, num2);
				}
				if (num3 > 0)
				{
					dictionary4.Add(游戏对象属性.最大魔防, num3);
				}
				if (num4 > 0)
				{
					dictionary4.Add(游戏对象属性.最小防御, num4);
				}
				if (num5 > 0)
				{
					dictionary4.Add(游戏对象属性.最小魔防, num5);
				}
				dictionary2.Add(Convert.ToByte(array3[0]), dictionary4);
				dictionary3.Add(Convert.ToByte(array3[0]), Convert.ToByte(array3[2]));
			}
			游戏坐骑.数据表 = dictionary;
			游戏坐骑.御兽之力属性 = dictionary2;
			游戏坐骑.御兽之力栏数 = dictionary3;
		}
	}
}
