using System;
using System.IO;
using _0005_0005_0004_0004_0001_0005_0003_0001_0002_0005_0005_0002_0001_0002;
using 游戏服务器.地图类;
using 游戏服务器.模板类;
using 游戏服务器.网络类;

namespace 游戏服务器.数据类
{
	public class 物品数据 : 游戏数据
	{
		public static byte 数据版本;

		public readonly 数据监视器<游戏物品> 对应模板;

		public readonly 数据监视器<DateTime> 生成时间;

		public readonly 数据监视器<角色数据> 生成来源;

		public readonly 数据监视器<int> 当前持久;

		public readonly 数据监视器<int> 最大持久;

		public readonly 数据监视器<byte> 物品容器;

		public readonly 数据监视器<byte> 物品位置;

		public readonly 数据监视器<bool> 绑定物品;

		public readonly 数据监视器<uint> 上锁时间;

		public readonly 字典监视器<byte, int> 特殊属性;

		public readonly 数据监视器<string> 掉落怪物;

		public readonly 数据监视器<string> 掉落地图;

		public int 回购编号;

		// 卖给 NPC 时记下卖家的 角色数据.数据索引, 使回购清单只对卖家本人可见(见 玩家实例.请求回购清单)。
		// 与 回购编号 一样是不落库的运行期字段: 回购物仅存活到关服, 关服时 地图处理网关.cs:1115-1120
		// 会把各商店 回购列表 里的物品逐个 删除数据()。0 表示来源未知(理论上不会出现)。
		public int 回购卖家索引;

		public 游戏物品 物品模板 => this.对应模板.V;

		public 物品出售分类 出售类型 => this.物品模板.商店类型;

		public 物品使用分类 物品类型 => this.物品模板.物品分类;

		public 物品持久分类 持久类型 => this.物品模板.持久类型;

		public 游戏对象职业 需要职业 => this.物品模板.需要职业;

		public 游戏对象性别 需要性别 => this.物品模板.需要性别;

		public string 物品名字 => this.物品模板?.物品名字;

		public int 需要等级 => this.物品模板.需要等级;

		public int 物品编号 => this.对应模板.V.物品编号;

		public bool 触发lua => this.对应模板.V.触发lua;

		public int? 解包物品编号 => this.物品模板.解包物品编号;

		public string 召唤下属名字 => this.物品模板.召唤下属名字;

		public 物品使用类型 物品使用属性 => this.物品模板.物品使用;

		public int 物品重量
		{
			get
			{
				if (this.持久类型 != 物品持久分类.堆叠)
				{
					return this.物品模板.物品重量;
				}
				return this.当前持久.V * this.物品模板.物品重量;
			}
		}
		internal static uint method01(string str1)
		{
			uint num = default(uint);
			if (str1 != null)
			{
				num = 2166136261u;
				for (int i = 0; i < str1.Length; i++)
				{
					num = (str1[i] ^ num) * 16777619;
				}
			}
			return num;
		}

		public int 出售价格
		{
			get
			{
				switch (this.对应模板.V.持久类型)
				{
				default:
					return 0;
				case 物品持久分类.无:
					return 1;
				case 物品持久分类.装备:
				{
					装备数据 装备数据2;
					装备数据2 = this as 装备数据;
					游戏装备 obj;
					obj = this.对应模板.V as 游戏装备;
					int v3;
					v3 = 装备数据2.当前持久.V;
					int num2;
					num2 = obj.物品持久 * 1000;
					int num3;
					num3 = obj.出售价格;
					int num4;
					num4 = Math.Max((Byte)0, 装备数据2.幸运等级.V);
					int num5;
					num5 = 装备数据2.升级攻击.V * 100 + 装备数据2.升级魔法.V * 100 + 装备数据2.升级道术.V * 100 + 装备数据2.升级刺术.V * 100 + 装备数据2.升级弓术.V * 100;
					int num6;
					num6 = 0;
					foreach (铭文技能 value in 装备数据2.铭文技能.Values)
					{
						if (value != null)
						{
							num6++;
						}
					}
					int num7;
					num7 = 0;
					foreach (随机属性 item in 装备数据2.随机属性)
					{
						num7 += item.战力加成 * 100;
					}
					int num8;
					num8 = 0;
					foreach (游戏物品 value2 in 装备数据2.镶嵌灵石.Values)
					{
						string text;
						text = value2.物品名字;
						uint num9;
						num9 = method01(text);
						if (num9 <= 1965594569)
						{
							if (num9 <= 943749297)
							{
								if (num9 <= 573099060)
								{
									if (num9 <= 245049310)
									{
										if (num9 <= 208490910)
										{
											if (num9 <= 36171325)
											{
												if (num9 != 35240798)
												{
													if (num9 != 36171325 || !(text == "精绿灵石2级"))
													{
														continue;
													}
													goto IL_1847;
												}
												if (!(text == "精绿灵石5级"))
												{
													continue;
												}
												goto IL_17e3;
											}
											if (num9 == 36983370)
											{
												if (!(text == "精绿灵石9级"))
												{
													continue;
												}
												goto IL_18c0;
											}
											if (num9 != 74678801)
											{
												if (num9 != 208490910 || !(text == "驭朱灵石4级"))
												{
													continue;
												}
												goto IL_1901;
											}
											if (!(text == "透蓝灵石1级"))
											{
												continue;
											}
										}
										else
										{
											if (num9 > 209834109)
											{
												if (num9 != 210646154)
												{
													if (num9 != 234198814)
													{
														if (num9 != 245049310 || !(text == "蔚蓝灵石10级"))
														{
															continue;
														}
													}
													else if (!(text == "狂热幻彩灵石10级"))
													{
														continue;
													}
													goto IL_1975;
												}
												if (!(text == "驭朱灵石8级"))
												{
													continue;
												}
												goto IL_1726;
											}
											if (num9 != 209302955)
											{
												if (num9 != 209834109 || !(text == "驭朱灵石3级"))
												{
													continue;
												}
												goto IL_1990;
											}
											if (!(text == "驭朱灵石1级"))
											{
												continue;
											}
										}
										goto IL_195a;
									}
									if (num9 <= 406321612)
									{
										if (num9 <= 305587683)
										{
											if (num9 == 263991377)
											{
												if (!(text == "抵御幻彩灵石7级"))
												{
													continue;
												}
												goto IL_189f;
											}
											if (num9 != 305587683 || !(text == "精绿灵石8级"))
											{
												continue;
											}
										}
										else
										{
											if (num9 == 335211465)
											{
												if (!(text == "韧紫灵石3级"))
												{
													continue;
												}
												goto IL_1990;
											}
											if (num9 != 336023510)
											{
												if (num9 != 406321612 || !(text == "精绿灵石10级"))
												{
													continue;
												}
												goto IL_1975;
											}
											if (!(text == "韧紫灵石8级"))
											{
												continue;
											}
										}
									}
									else
									{
										if (num9 <= 479250467)
										{
											if (num9 != 470449305)
											{
												if (num9 != 479250467 || !(text == "驭朱灵石9级"))
												{
													continue;
												}
												goto IL_18c0;
											}
											if (!(text == "命朱灵石7级"))
											{
												continue;
											}
											goto IL_189f;
										}
										if (num9 != 531090082)
										{
											if (num9 != 549347465)
											{
												if (num9 != 573099060 || !(text == "精绿灵石3级"))
												{
													continue;
												}
												goto IL_1990;
											}
											if (!(text == "透蓝灵石10级"))
											{
												continue;
											}
											goto IL_1975;
										}
										if (!(text == "抵御幻彩灵石8级"))
										{
											continue;
										}
									}
									goto IL_1726;
								}
								if (num9 <= 738772727)
								{
									if (num9 <= 680541790)
									{
										if (num9 > 607107224)
										{
											if (num9 != 607638378)
											{
												if (num9 != 611931354)
												{
													if (num9 != 680541790 || !(text == "橙黄灵石10级"))
													{
														continue;
													}
													goto IL_1975;
												}
												if (!(text == "透蓝灵石6级"))
												{
													continue;
												}
												goto IL_1922;
											}
											if (!(text == "新阳灵石3级"))
											{
												continue;
											}
											goto IL_1990;
										}
										if (num9 == 603534887)
										{
											if (!(text == "韧紫灵石1级"))
											{
												continue;
											}
											goto IL_195a;
										}
										if (num9 != 607107224 || !(text == "新阳灵石5级"))
										{
											continue;
										}
									}
									else
									{
										if (num9 <= 692924671)
										{
											if (num9 != 691994144)
											{
												if (num9 != 692924671 || !(text == "蔚蓝灵石2级"))
												{
													continue;
												}
												goto IL_1847;
											}
											if (!(text == "蔚蓝灵石9级"))
											{
												continue;
											}
											goto IL_18c0;
										}
										if (num9 != 693736716)
										{
											if (num9 == 714727999)
											{
												if (!(text == "抵御幻彩灵石10级"))
												{
													continue;
												}
												goto IL_1975;
											}
											if (num9 != 738772727 || !(text == "命朱灵石5级"))
											{
												continue;
											}
										}
										else if (!(text == "蔚蓝灵石5级"))
										{
											continue;
										}
									}
									goto IL_17e3;
								}
								if (num9 <= 804167584)
								{
									if (num9 > 771022468)
									{
										if (num9 != 799900731)
										{
											if (num9 == 800712776)
											{
												if (!(text == "抵御幻彩灵石6级"))
												{
													continue;
												}
												goto IL_1922;
											}
											if (num9 != 804167584 || !(text == "橙黄灵石9级"))
											{
												continue;
											}
										}
										else if (!(text == "抵御幻彩灵石9级"))
										{
											continue;
										}
										goto IL_18c0;
									}
									if (num9 != 746349172)
									{
										if (num9 != 771022468 || !(text == "守阳灵石1级"))
										{
											continue;
										}
										goto IL_195a;
									}
									if (!(text == "驭朱灵石2级"))
									{
										continue;
									}
								}
								else
								{
									if (num9 > 805910156)
									{
										if (num9 != 840642163)
										{
											if (num9 != 896281263)
											{
												if (num9 != 943749297 || !(text == "深灰灵石4级"))
												{
													continue;
												}
												goto IL_1901;
											}
											if (!(text == "进击幻彩灵石10级"))
											{
												continue;
											}
											goto IL_1975;
										}
										if (!(text == "赤褐灵石7级"))
										{
											continue;
										}
										goto IL_189f;
									}
									if (num9 != 805098111)
									{
										if (num9 != 805910156 || !(text == "橙黄灵石5级"))
										{
											continue;
										}
										goto IL_17e3;
									}
									if (!(text == "橙黄灵石2级"))
									{
										continue;
									}
								}
								goto IL_1847;
							}
							if (num9 <= 1307537531)
							{
								if (num9 > 1144359777)
								{
									if (num9 <= 1230989269)
									{
										if (num9 <= 1211660047)
										{
											if (num9 == 1200044703)
											{
												if (!(text == "纯紫灵石1级"))
												{
													continue;
												}
												goto IL_195a;
											}
											if (num9 != 1211660047 || !(text == "深灰灵石6级"))
											{
												continue;
											}
										}
										else
										{
											if (num9 == 1229646070)
											{
												if (!(text == "蔚蓝灵石3级"))
												{
													continue;
												}
												goto IL_1990;
											}
											if (num9 != 1230458115)
											{
												if (num9 != 1230989269 || !(text == "蔚蓝灵石4级"))
												{
													continue;
												}
												goto IL_1901;
											}
											if (!(text == "蔚蓝灵石6级"))
											{
												continue;
											}
										}
									}
									else
									{
										if (num9 <= 1276630989)
										{
											if (num9 != 1275287790)
											{
												if (num9 != 1276630989 || !(text == "命朱灵石3级"))
												{
													continue;
												}
												goto IL_1990;
											}
											if (!(text == "命朱灵石4级"))
											{
												continue;
											}
											goto IL_1901;
										}
										if (num9 != 1284105784)
										{
											if (num9 != 1300070770)
											{
												if (num9 != 1307537531 || !(text == "守阳灵石2级"))
												{
													continue;
												}
												goto IL_1847;
											}
											if (!(text == "守阳灵石10级"))
											{
												continue;
											}
											goto IL_1975;
										}
										if (!(text == "进击幻彩灵石6级"))
										{
											continue;
										}
									}
									goto IL_1922;
								}
								if (num9 <= 1038933218)
								{
									if (num9 <= 1014483090)
									{
										if (num9 != 1007377040)
										{
											if (num9 != 1014483090 || !(text == "进击幻彩灵石8级"))
											{
												continue;
											}
											goto IL_1726;
										}
										if (!(text == "命朱灵石6级"))
										{
											continue;
										}
										goto IL_1922;
									}
									if (num9 == 1015295135)
									{
										if (!(text == "进击幻彩灵石5级"))
										{
											continue;
										}
										goto IL_17e3;
									}
									if (num9 == 1025365623)
									{
										if (!(text == "命朱灵石10级"))
										{
											continue;
										}
										goto IL_1975;
									}
									if (num9 != 1038933218 || !(text == "守阳灵石3级"))
									{
										continue;
									}
								}
								else
								{
									if (num9 > 1127514398)
									{
										if (num9 != 1128326443)
										{
											if (num9 != 1128444925)
											{
												if (num9 != 1144359777 || !(text == "新阳灵石4级"))
												{
													continue;
												}
											}
											else if (!(text == "盈绿灵石4级"))
											{
												continue;
											}
											goto IL_1901;
										}
										if (!(text == "盈绿灵石6级"))
										{
											continue;
										}
										goto IL_1922;
									}
									if (num9 == 1108552913)
									{
										if (!(text == "赤褐灵石1级"))
										{
											continue;
										}
										goto IL_195a;
									}
									if (num9 != 1127514398 || !(text == "盈绿灵石3级"))
									{
										continue;
									}
								}
							}
							else
							{
								if (num9 > 1665978369)
								{
									if (num9 <= 1914880594)
									{
										if (num9 <= 1697659818)
										{
											if (num9 == 1697128664)
											{
												if (!(text == "狂热幻彩灵石1级"))
												{
													continue;
												}
												goto IL_195a;
											}
											if (num9 != 1697659818 || !(text == "狂热幻彩灵石7级"))
											{
												continue;
											}
										}
										else if (num9 != 1738109301)
										{
											if (num9 != 1914349440)
											{
												if (num9 != 1914880594 || !(text == "精绿灵石1级"))
												{
													continue;
												}
												goto IL_195a;
											}
											if (!(text == "精绿灵石7级"))
											{
												continue;
											}
										}
										else if (!(text == "纯紫灵石7级"))
										{
											continue;
										}
										goto IL_189f;
									}
									if (num9 <= 1953388070)
									{
										if (num9 != 1952576025)
										{
											if (num9 != 1953388070 || !(text == "透蓝灵石2级"))
											{
												continue;
											}
											goto IL_1847;
										}
										if (!(text == "透蓝灵石9级"))
										{
											continue;
										}
										goto IL_18c0;
									}
									if (num9 != 1954318597)
									{
										if (num9 != 1964640041)
										{
											if (num9 != 1965594569 || !(text == "赤褐灵石10级"))
											{
												continue;
											}
											goto IL_1975;
										}
										if (!(text == "狂热幻彩灵石8级"))
										{
											continue;
										}
										goto IL_1726;
									}
									if (!(text == "透蓝灵石5级"))
									{
										continue;
									}
									goto IL_17e3;
								}
								if (num9 <= 1480470696)
								{
									if (num9 > 1342837891)
									{
										if (num9 != 1342956373)
										{
											if (num9 != 1468855352)
											{
												if (num9 != 1480470696 || !(text == "深灰灵石5级"))
												{
													continue;
												}
												goto IL_17e3;
											}
											if (!(text == "纯紫灵石2级"))
											{
												continue;
											}
											goto IL_1847;
										}
										if (!(text == "橙黄灵石4级"))
										{
											continue;
										}
										goto IL_1901;
									}
									if (num9 != 1342025846)
									{
										if (num9 != 1342837891 || !(text == "橙黄灵石6级"))
										{
											continue;
										}
										goto IL_1922;
									}
									if (!(text == "橙黄灵石3级"))
									{
										continue;
									}
								}
								else
								{
									if (num9 > 1553359733)
									{
										if (num9 != 1645599130)
										{
											if (num9 != 1665166324)
											{
												if (num9 != 1665978369 || !(text == "盈绿灵石8级"))
												{
													continue;
												}
												goto IL_1726;
											}
											if (!(text == "盈绿灵石5级"))
											{
												continue;
											}
											goto IL_17e3;
										}
										if (!(text == "赤褐灵石6级"))
										{
											continue;
										}
										goto IL_1922;
									}
									if (num9 == 1552016534)
									{
										if (!(text == "进击幻彩灵石4级"))
										{
											continue;
										}
										goto IL_1901;
									}
									if (num9 != 1553359733 || !(text == "进击幻彩灵石3级"))
									{
										continue;
									}
								}
							}
							goto IL_1990;
						}
						if (num9 <= 3186246800u)
						{
							if (num9 <= 2719719079u)
							{
								if (num9 <= 2489297424u)
								{
									if (num9 <= 2214629611u)
									{
										if (num9 > 2087909056)
										{
											if (num9 != 2142391142)
											{
												if (num9 != 2143734341)
												{
													if (num9 != 2214629611u || !(text == "韧紫灵石5级"))
													{
														continue;
													}
													goto IL_17e3;
												}
												if (!(text == "抵御幻彩灵石3级"))
												{
													continue;
												}
												goto IL_1990;
											}
											if (!(text == "抵御幻彩灵石4级"))
											{
												continue;
											}
											goto IL_1901;
										}
										if (num9 == 2004277479)
										{
											if (!(text == "纯紫灵石9级"))
											{
												continue;
											}
											goto IL_18c0;
										}
										if (num9 != 2087909056 || !(text == "驭朱灵石6级"))
										{
											continue;
										}
									}
									else if (num9 <= 2451395657u)
									{
										if (num9 == 2215160765u)
										{
											if (!(text == "韧紫灵石7级"))
											{
												continue;
											}
											goto IL_189f;
										}
										if (num9 != 2451395657u || !(text == "精绿灵石6级"))
										{
											continue;
										}
									}
									else
									{
										if (num9 != 2486038143u)
										{
											if (num9 != 2486850188u)
											{
												if (num9 != 2489297424u || !(text == "透蓝灵石8级"))
												{
													continue;
												}
												goto IL_1726;
											}
											if (!(text == "新阳灵石1级"))
											{
												continue;
											}
											goto IL_195a;
										}
										if (!(text == "新阳灵石6级"))
										{
											continue;
										}
									}
									goto IL_1922;
								}
								if (num9 <= 2649319065u)
								{
									if (num9 <= 2491039996u)
									{
										if (num9 == 2490227951u)
										{
											if (!(text == "透蓝灵石3级"))
											{
												continue;
											}
											goto IL_1990;
										}
										if (num9 != 2491039996u || !(text == "透蓝灵石4级"))
										{
											continue;
										}
									}
									else
									{
										if (num9 == 2619608627u)
										{
											if (!(text == "命朱灵石9级"))
											{
												continue;
											}
											goto IL_18c0;
										}
										if (num9 == 2625161609u)
										{
											if (!(text == "驭朱灵石7级"))
											{
												continue;
											}
											goto IL_189f;
										}
										if (num9 != 2649319065u || !(text == "守阳灵石4级"))
										{
											continue;
										}
									}
								}
								else
								{
									if (num9 <= 2679437359u)
									{
										if (num9 != 2651474309u)
										{
											if (num9 != 2679437359u || !(text == "抵御幻彩灵石5级"))
											{
												continue;
											}
											goto IL_17e3;
										}
										if (!(text == "守阳灵石8级"))
										{
											continue;
										}
										goto IL_1726;
									}
									if (num9 == 2680249404u)
									{
										if (!(text == "抵御幻彩灵石2级"))
										{
											continue;
										}
										goto IL_1847;
									}
									if (num9 == 2718739222u)
									{
										if (!(text == "盈绿灵石10级"))
										{
											continue;
										}
										goto IL_1975;
									}
									if (num9 != 2719719079u || !(text == "精绿灵石4级"))
									{
										continue;
									}
								}
								goto IL_1901;
							}
							if (num9 <= 2988502213u)
							{
								if (num9 > 2887120004u)
								{
									if (num9 <= 2917642487u)
									{
										if (num9 != 2893072359u)
										{
											if (num9 != 2917642487u || !(text == "守阳灵石6级"))
											{
												continue;
											}
											goto IL_1922;
										}
										if (!(text == "驭朱灵石5级"))
										{
											continue;
										}
									}
									else
									{
										if (num9 == 2986346969u)
										{
											if (!(text == "赤褐灵石9级"))
											{
												continue;
											}
											goto IL_18c0;
										}
										if (num9 == 2987159014u)
										{
											if (!(text == "赤褐灵石2级"))
											{
												continue;
											}
											goto IL_1847;
										}
										if (num9 != 2988502213u || !(text == "赤褐灵石5级"))
										{
											continue;
										}
									}
									goto IL_17e3;
								}
								if (num9 <= 2754361565u)
								{
									if (num9 != 2751882164u)
									{
										if (num9 != 2754361565u || !(text == "新阳灵石8级"))
										{
											continue;
										}
										goto IL_1726;
									}
									if (!(text == "韧紫灵石6级"))
									{
										continue;
									}
									goto IL_1922;
								}
								if (num9 != 2822149062u)
								{
									if (num9 != 2822961107u)
									{
										if (num9 != 2887120004u || !(text == "命朱灵石2级"))
										{
											continue;
										}
									}
									else if (!(text == "深灰灵石2级"))
									{
										continue;
									}
									goto IL_1847;
								}
								if (!(text == "深灰灵石7级"))
								{
									continue;
								}
							}
							else if (num9 <= 3090472484u)
							{
								if (num9 <= 3007257362u)
								{
									if (num9 == 3006726208u)
									{
										if (!(text == "盈绿灵石1级"))
										{
											continue;
										}
										goto IL_195a;
									}
									if (num9 != 3007257362u || !(text == "盈绿灵石7级"))
									{
										continue;
									}
								}
								else
								{
									if (num9 != 3022965878u)
									{
										if (num9 != 3023777923u)
										{
											if (num9 != 3090472484u || !(text == "深灰灵石9级"))
											{
												continue;
											}
											goto IL_18c0;
										}
										if (!(text == "新阳灵石2级"))
										{
											continue;
										}
										goto IL_1847;
									}
									if (!(text == "新阳灵石7级"))
									{
										continue;
									}
								}
							}
							else
							{
								if (num9 <= 3109167384u)
								{
									if (num9 != 3101887706u)
									{
										if (num9 != 3109167384u || !(text == "蔚蓝灵石1级"))
										{
											continue;
										}
										goto IL_195a;
									}
									if (!(text == "深灰灵石10级"))
									{
										continue;
									}
									goto IL_1975;
								}
								if (num9 != 3109285866u)
								{
									if (num9 != 3163745580u)
									{
										if (num9 != 3186246800u || !(text == "守阳灵石5级"))
										{
											continue;
										}
										goto IL_17e3;
									}
									if (!(text == "进击幻彩灵石2级"))
									{
										continue;
									}
									goto IL_1847;
								}
								if (!(text == "蔚蓝灵石7级"))
								{
									continue;
								}
							}
						}
						else
						{
							if (num9 <= 3581907291u)
							{
								if (num9 <= 3424978266u)
								{
									if (num9 > 3290876628u)
									{
										if (num9 <= 3360007324u)
										{
											if (num9 != 3348495148u)
											{
												if (num9 != 3360007324u || !(text == "深灰灵石1级"))
												{
													continue;
												}
												goto IL_195a;
											}
											if (!(text == "纯紫灵石6级"))
											{
												continue;
											}
											goto IL_1922;
										}
										if (num9 != 3376266089u)
										{
											if (num9 == 3423635067u)
											{
												if (!(text == "命朱灵石1级"))
												{
													continue;
												}
												goto IL_195a;
											}
											if (num9 != 3424978266u || !(text == "命朱灵石8级"))
											{
												continue;
											}
										}
										else if (!(text == "蔚蓝灵石8级"))
										{
											continue;
										}
										goto IL_1726;
									}
									if (num9 <= 3221134488u)
									{
										if (num9 != 3187989372u)
										{
											if (num9 != 3221134488u || !(text == "橙黄灵石1级"))
											{
												continue;
											}
											goto IL_195a;
										}
										if (!(text == "守阳灵石9级"))
										{
											continue;
										}
									}
									else
									{
										if (num9 == 3221665642u)
										{
											if (!(text == "橙黄灵石7级"))
											{
												continue;
											}
											goto IL_189f;
										}
										if (num9 != 3276673720u)
										{
											if (num9 != 3290876628u || !(text == "新阳灵石9级"))
											{
												continue;
											}
										}
										else if (!(text == "盈绿灵石9级"))
										{
											continue;
										}
									}
								}
								else if (num9 <= 3524411567u)
								{
									if (num9 > 3454157550u)
									{
										if (num9 != 3488645865u)
										{
											if (num9 != 3523068368u)
											{
												if (num9 != 3524411567u || !(text == "赤褐灵石3级"))
												{
													continue;
												}
												goto IL_1990;
											}
											if (!(text == "赤褐灵石8级"))
											{
												continue;
											}
										}
										else if (!(text == "橙黄灵石8级"))
										{
											continue;
										}
										goto IL_1726;
									}
									if (num9 != 3430725803u)
									{
										if (num9 != 3454157550u || !(text == "守阳灵石7级"))
										{
											continue;
										}
										goto IL_189f;
									}
									if (!(text == "进击幻彩灵石9级"))
									{
										continue;
									}
								}
								else
								{
									if (num9 > 3575025888u)
									{
										if (num9 != 3575956415u)
										{
											if (num9 != 3576768460u)
											{
												if (num9 != 3581907291u || !(text == "韧紫灵石10级"))
												{
													continue;
												}
												goto IL_1975;
											}
											if (!(text == "狂热幻彩灵石5级"))
											{
												continue;
											}
											goto IL_17e3;
										}
										if (!(text == "狂热幻彩灵石2级"))
										{
											continue;
										}
										goto IL_1847;
									}
									if (num9 == 3525223612u)
									{
										if (!(text == "赤褐灵石4级"))
										{
											continue;
										}
										goto IL_1901;
									}
									if (num9 != 3575025888u || !(text == "狂热幻彩灵石9级"))
									{
										continue;
									}
								}
								goto IL_18c0;
							}
							if (num9 > 3968584065u)
							{
								if (num9 <= 4112884150u)
								{
									if (num9 <= 4093457362u)
									{
										if (num9 != 4093338880u)
										{
											if (num9 != 4093457362u || !(text == "韧紫灵石4级"))
											{
												continue;
											}
											goto IL_1901;
										}
										if (!(text == "韧紫灵石2级"))
										{
											continue;
										}
										goto IL_1847;
									}
									if (num9 == 4094269407u)
									{
										if (!(text == "韧紫灵石9级"))
										{
											continue;
										}
										goto IL_18c0;
									}
									if (num9 == 4101735347u)
									{
										if (!(text == "透蓝灵石7级"))
										{
											continue;
										}
										goto IL_189f;
									}
									if (num9 != 4112884150u || !(text == "狂热幻彩灵石3级"))
									{
										continue;
									}
								}
								else
								{
									if (num9 <= 4113814677u)
									{
										if (num9 != 4113696195u)
										{
											if (num9 != 4113814677u || !(text == "狂热幻彩灵石4级"))
											{
												continue;
											}
											goto IL_1901;
										}
										if (!(text == "狂热幻彩灵石6级"))
										{
											continue;
										}
										goto IL_1922;
									}
									if (num9 != 4153333633u)
									{
										if (num9 != 4189275687u)
										{
											if (num9 != 4290635251u || !(text == "抵御幻彩灵石1级"))
											{
												continue;
											}
											goto IL_195a;
										}
										if (!(text == "驭朱灵石10级"))
										{
											continue;
										}
										goto IL_1975;
									}
									if (!(text == "纯紫灵石3级"))
									{
										continue;
									}
								}
								goto IL_1990;
							}
							if (num9 <= 3686647075u)
							{
								if (num9 <= 3616405898u)
								{
									if (num9 != 3614663326u)
									{
										if (num9 != 3616405898u || !(text == "纯紫灵石4级"))
										{
											continue;
										}
										goto IL_1901;
									}
									if (!(text == "纯紫灵石8级"))
									{
										continue;
									}
								}
								else
								{
									if (num9 != 3627518701u)
									{
										if (num9 != 3628330746u)
										{
											if (num9 != 3686647075u || !(text == "纯紫灵石10级"))
											{
												continue;
											}
											goto IL_1975;
										}
										if (!(text == "深灰灵石3级"))
										{
											continue;
										}
										goto IL_1990;
									}
									if (!(text == "深灰灵石8级"))
									{
										continue;
									}
								}
								goto IL_1726;
							}
							if (num9 <= 3812095847u)
							{
								if (num9 != 3700260643u)
								{
									if (num9 != 3812095847u || !(text == "盈绿灵石2级"))
									{
										continue;
									}
									goto IL_1847;
								}
								if (!(text == "进击幻彩灵石1级"))
								{
									continue;
								}
								goto IL_195a;
							}
							if (num9 == 3885010211u)
							{
								if (!(text == "纯紫灵石5级"))
								{
									continue;
								}
								goto IL_17e3;
							}
							if (num9 == 3922679306u)
							{
								if (!(text == "新阳灵石10级"))
								{
									continue;
								}
								goto IL_1975;
							}
							if (num9 != 3968584065u || !(text == "进击幻彩灵石7级"))
							{
								continue;
							}
						}
						goto IL_189f;
						IL_1847:
						num8 += 2000;
						continue;
						IL_1975:
						num8 += 10000;
						continue;
						IL_1726:
						num8 += 8000;
						continue;
						IL_18c0:
						num8 += 9000;
						continue;
						IL_1901:
						num8 += 4000;
						continue;
						IL_1922:
						num8 += 6000;
						continue;
						IL_1990:
						num8 += 3000;
						continue;
						IL_17e3:
						num8 += 5000;
						continue;
						IL_195a:
						num8 += 1000;
						continue;
						IL_189f:
						num8 += 7000;
					}
					int num10;
					num10 = num3 + num4 + num5 + num6 + num7 + num8;
					return (int)((decimal)v3 / (decimal)num2 * 0.9m * (decimal)num10 + (decimal)num10 * 0.1m);
				}
				case 物品持久分类.消耗:
				{
					int v2;
					v2 = this.当前持久.V;
					int 物品持久;
					物品持久 = this.对应模板.V.物品持久;
					int num;
					num = this.对应模板.V.出售价格;
					return (int)((decimal)v2 / (decimal)物品持久 * (decimal)num);
				}
				case 物品持久分类.堆叠:
				{
					int v;
					v = this.当前持久.V;
					return this.对应模板.V.出售价格 * v;
				}
				case 物品持久分类.回复:
					return 1;
				case 物品持久分类.容器:
					return this.对应模板.V.出售价格;
				case 物品持久分类.纯度:
					return this.对应模板.V.出售价格;
				}
			}
		}

		public int 堆叠上限 => this.对应模板.V.物品持久;

		public int 默认持久
		{
			get
			{
				if (this.持久类型 != 物品持久分类.装备)
				{
					return this.对应模板.V.物品持久;
				}
				return this.对应模板.V.物品持久 * 1000;
			}
		}

		public byte 当前位置
		{
			get
			{
				return this.物品位置.V;
			}
			set
			{
				this.物品位置.V = value;
			}
		}

		public bool 是否绑定
		{
			get
			{
				if (!this.绑定物品.V)
				{
					return this.物品模板.是否绑定;
				}
				return true;
			}
		}

		public bool 是否上锁 => this.上锁时间.V == uint.MaxValue;

		public bool 资源物品 => this.对应模板.V.资源物品;

		public bool 能否出售 => this.物品模板.能否出售;

		public bool 能否分解 => this.物品模板.能否分解;

		public bool 能否堆叠 => this.对应模板.V.持久类型 == 物品持久分类.堆叠;

		public bool 能否掉落 => this.物品模板.能否掉落;

		public ushort 技能编号 => this.物品模板.附加技能;

		public byte 分组编号 => this.物品模板.物品分组;

		public int 分组冷却 => this.物品模板.分组冷却;

		public int 冷却时间 => this.物品模板.冷却时间;

		public string 物品名一 => this.物品模板.物品名一;

		public string 物品名二 => this.物品模板.物品名二;

		public string 物品名三 => this.物品模板.物品名三;

		public int 物品数一 => this.物品模板.物品数一;

		public int 物品数二 => this.物品模板.物品数二;

		public int 物品数三 => this.物品模板.物品数三;

		public int 双倍经验 => this.物品模板.双倍经验;

		public int 经验数量 => this.物品模板.经验数量;

		public int 银币数量 => this.物品模板.银币数量;

		public int 金币数量 => this.物品模板.金币数量;

		public int 元宝数量 => this.物品模板.元宝数量;

		public bool 广播通知 => this.物品模板.广播通知;

		public bool 系统通知 => this.物品模板.系统通知;

		public bool 防具物品
		{
			get
			{
				if (this.物品类型 != 物品使用分类.衣服 && this.物品类型 != 物品使用分类.头盔 && this.物品类型 != 物品使用分类.护腕 && this.物品类型 != 物品使用分类.勋章 && this.物品类型 != 物品使用分类.鞋子 && this.物品类型 != 物品使用分类.腰带 && this.物品类型 != 物品使用分类.玉佩 && this.物品类型 != 物品使用分类.披风)
				{
					return this.物品类型 == 物品使用分类.护肩;
				}
				return true;
			}
		}

		public bool 饰品物品
		{
			get
			{
				if (this.物品类型 != 物品使用分类.项链 && this.物品类型 != 物品使用分类.戒指)
				{
					return this.物品类型 == 物品使用分类.手镯;
				}
				return true;
			}
		}

		public 物品数据()
		{
		}

		public 物品数据(游戏物品 模板, 角色数据 来源, byte 容器, byte 位置, int 持久, bool 绑定 = false, string 掉落怪物 = "")
		{
			this.绑定物品.V = 绑定;
			this.对应模板.V = 模板;
			this.生成来源.V = 来源;
			this.物品容器.V = 容器;
			this.物品位置.V = 位置;
			this.生成时间.V = 主程.当前时间;
			this.最大持久.V = this.物品模板.物品持久;
			数据监视器<string> obj;
			obj = this.掉落地图;
			object obj2;
			if (来源 == null)
			{
				obj2 = null;
			}
			else
			{
				客户网络 网络连接;
				网络连接 = 来源.网络连接;
				if (网络连接 == null)
				{
					obj2 = null;
				}
				else
				{
					玩家实例 绑定角色;
					绑定角色 = 网络连接.绑定角色;
					if (绑定角色 == null)
					{
						obj2 = null;
					}
					else
					{
						地图实例 当前地图;
						当前地图 = 绑定角色.当前地图;
						if (当前地图 == null)
						{
							obj2 = null;
						}
						else
						{
							游戏地图 地图模板;
							地图模板 = 当前地图.地图模板;
							if (地图模板 == null)
							{
								obj2 = null;
							}
							else
							{
								obj2 = 地图模板.地图名字;
								if (obj2 != null)
								{
									goto IL_00b6;
								}
							}
						}
					}
				}
			}
			obj2 = "";
			goto IL_00b6;
			IL_00b6:
			obj.V = (string)obj2;
			this.掉落怪物.V = 掉落怪物;
			this.当前持久.V = 持久;
			if (Settings.开启任务系统)
			{
				CharacterQuest[] inProgressQuests;
				inProgressQuests = 来源.GetInProgressQuests();
				foreach (CharacterQuest characterQuest in inProgressQuests)
				{
					CharacterQuestMission[] missionsOfType;
					missionsOfType = characterQuest.GetMissionsOfType(QuestMissionType.AdquireItem);
					bool flag;
					flag = false;
					CharacterQuestMission[] array;
					array = missionsOfType;
					foreach (CharacterQuestMission characterQuestMission in array)
					{
						if (!(characterQuestMission.CompletedDate.V != DateTime.MinValue) && characterQuestMission.Info.V.Id == 模板.物品编号)
						{
							characterQuestMission.Count.V = (byte)(characterQuestMission.Count.V + 持久);
							flag = true;
						}
					}
					if (flag)
					{
						来源.网络连接?.绑定角色.UpdateQuestProgress(characterQuest);
					}
				}
			}
			游戏数据网关.物品数据表.添加数据(this, 分配索引: true);
		}

		public override string ToString()
		{
			return this.物品名字;
		}

		public virtual byte[] 字节描述()
		{
			using MemoryStream memoryStream = new MemoryStream();
			using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(物品数据.数据版本);
			binaryWriter.Write(this.生成来源.V?.数据索引.V ?? 0);
			binaryWriter.Write(计算类.时间转换(this.生成时间.V));
			binaryWriter.Write(this.对应模板.V.物品编号);
			binaryWriter.Write(this.物品容器.V);
			binaryWriter.Write(this.物品位置.V);
			binaryWriter.Write(this.当前持久.V);
			binaryWriter.Write(this.最大持久.V);
			binaryWriter.Write((byte)(this.是否绑定 ? 10u : 0u));
			binaryWriter.Write((ushort)0);
			if (this.上锁时间.V == 0)
			{
				binaryWriter.Write(0);
			}
			else
			{
				binaryWriter.Write(524288);
			}
			binaryWriter.Write(this.上锁时间.V);
			if (this.特殊属性.Count > 0 && this.特殊属性.TryGetValue(0, out var v) && v > 0)
			{
				this.特殊属性描述(v, binaryWriter);
			}
			return memoryStream.ToArray();
		}

		public virtual byte[] 字节描述(int 数量)
		{
			using MemoryStream memoryStream = new MemoryStream();
			using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(物品数据.数据版本);
			binaryWriter.Write(this.生成来源.V?.数据索引.V ?? 0);
			binaryWriter.Write(计算类.时间转换(this.生成时间.V));
			binaryWriter.Write(this.对应模板.V.物品编号);
			binaryWriter.Write(this.物品容器.V);
			binaryWriter.Write(this.物品位置.V);
			binaryWriter.Write(数量);
			binaryWriter.Write(this.最大持久.V);
			binaryWriter.Write((byte)(this.是否绑定 ? 10u : 0u));
			binaryWriter.Write((ushort)0);
			if (this.上锁时间.V == 0)
			{
				binaryWriter.Write(0);
			}
			else
			{
				binaryWriter.Write(524288);
			}
			binaryWriter.Write(this.上锁时间.V);
			if (this.特殊属性.Count > 0 && this.特殊属性.TryGetValue(0, out var v) && v > 0)
			{
				this.特殊属性描述(v, binaryWriter);
			}
			return memoryStream.ToArray();
		}

		public void 特殊属性描述(int type, BinaryWriter binaryWriter)
		{
			if (type == 1)
			{
				binaryWriter.Write((ushort)this.特殊属性[1]);
				binaryWriter.Write((ushort)this.特殊属性[2]);
				binaryWriter.Write(this.特殊属性[3]);
				binaryWriter.Write((ushort)this.特殊属性[4]);
				binaryWriter.Write(this.特殊属性[5]);
				binaryWriter.Write(this.特殊属性[6]);
				binaryWriter.Write(this.特殊属性[7]);
				binaryWriter.Write(this.特殊属性[8]);
				binaryWriter.Write(this.特殊属性[9]);
				binaryWriter.Write(this.特殊属性[10]);
				binaryWriter.Write(this.特殊属性[11]);
				binaryWriter.Write(this.特殊属性[12]);
				binaryWriter.Write(this.特殊属性[13]);
				binaryWriter.Write(this.特殊属性[14]);
			}
		}

		static 物品数据()
		{
			物品数据.数据版本 = 15;
		}

		public int GetProp(物品使用属性 property, int defaultValue = 0)
		{
			foreach (使用属性数值 item in this.物品模板.属性字典)
			{
				if (item.类别 == property)
				{
					return item.值;
				}
			}
			return defaultValue;
		}

		public bool HasProp(物品使用属性 property)
		{
			foreach (使用属性数值 item in this.物品模板.属性字典)
			{
				if (item.类别 == property)
				{
					return true;
				}
			}
			return false;
		}
	}
}
