using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using 游戏服务器.模板类;

namespace 游戏服务器.数据类
{
	public sealed class 数据字段
	{
		internal static readonly Dictionary<Type, Func<BinaryReader, 游戏数据, 数据字段, object>> 字段读取方法表;

		internal static readonly Dictionary<Type, Action<BinaryWriter, object>> 字段写入方法表;

		public string 字段类型Name;

		public string 字段名字 { get; }

		public Type 字段类型 { get; }

		public FieldInfo 字段详情 { get; }

		static 数据字段()
		{
			数据字段.字段读取方法表 = new Dictionary<Type, Func<BinaryReader, 游戏数据, 数据字段, object>>
			{
				[typeof(数据监视器<int>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<int> obj24;
					obj24 = new 数据监视器<int>(o);
					obj24.QuietlySetValue(r.ReadInt32());
					return obj24;
				},
				[typeof(数据监视器<uint>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<uint> obj23;
					obj23 = new 数据监视器<uint>(o);
					obj23.QuietlySetValue(r.ReadUInt32());
					return obj23;
				},
				[typeof(数据监视器<long>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<long> obj22;
					obj22 = new 数据监视器<long>(o);
					obj22.QuietlySetValue(r.ReadInt64());
					return obj22;
				},
				[typeof(数据监视器<bool>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<bool> obj21;
					obj21 = new 数据监视器<bool>(o);
					obj21.QuietlySetValue(r.ReadBoolean());
					return obj21;
				},
				[typeof(数据监视器<byte>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<byte> obj20;
					obj20 = new 数据监视器<byte>(o);
					obj20.QuietlySetValue(r.ReadByte());
					return obj20;
				},
				[typeof(数据监视器<sbyte>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<sbyte> obj19;
					obj19 = new 数据监视器<sbyte>(o);
					obj19.QuietlySetValue(r.ReadSByte());
					return obj19;
				},
				[typeof(数据监视器<string>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<string> obj18;
					obj18 = new 数据监视器<string>(o);
					obj18.QuietlySetValue(r.ReadString());
					return obj18;
				},
				[typeof(数据监视器<ushort>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<ushort> obj17;
					obj17 = new 数据监视器<ushort>(o);
					obj17.QuietlySetValue(r.ReadUInt16());
					return obj17;
				},
				[typeof(数据监视器<Point>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<Point> obj16;
					obj16 = new 数据监视器<Point>(o);
					obj16.QuietlySetValue(new Point(r.ReadInt32(), r.ReadInt32()));
					return obj16;
				},
				[typeof(数据监视器<TimeSpan>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<TimeSpan> obj15;
					obj15 = new 数据监视器<TimeSpan>(o);
					obj15.QuietlySetValue(TimeSpan.FromTicks(r.ReadInt64()));
					return obj15;
				},
				[typeof(数据监视器<DateTime>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<DateTime> obj14;
					obj14 = new 数据监视器<DateTime>(o);
					obj14.QuietlySetValue(DateTime.FromBinary(r.ReadInt64()));
					return obj14;
				},
				[typeof(数据监视器<随机属性>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<随机属性> obj13;
					obj13 = new 数据监视器<随机属性>(o);
					obj13.QuietlySetValue(随机属性.数据表.TryGetValue(r.ReadInt32(), out var value9) ? value9 : null);
					return obj13;
				},
				[typeof(数据监视器<铭文技能>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<铭文技能> obj12;
					obj12 = new 数据监视器<铭文技能>(o);
					obj12.QuietlySetValue(铭文技能.数据表.TryGetValue(r.ReadUInt16(), out var value8) ? value8 : null);
					return obj12;
				},
				[typeof(数据监视器<游戏物品>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<游戏物品> obj11;
					obj11 = new 数据监视器<游戏物品>(o);
					obj11.QuietlySetValue(游戏物品.数据表.TryGetValue(r.ReadInt32(), out var value7) ? value7 : null);
					return obj11;
				},
				[typeof(数据监视器<宠物模式>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<宠物模式> obj10;
					obj10 = new 数据监视器<宠物模式>(o);
					obj10.QuietlySetValue((宠物模式)r.ReadInt32());
					return obj10;
				},
				[typeof(数据监视器<攻击模式>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<攻击模式> obj9;
					obj9 = new 数据监视器<攻击模式>(o);
					obj9.QuietlySetValue((攻击模式)r.ReadInt32());
					return obj9;
				},
				[typeof(数据监视器<游戏方向>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<游戏方向> obj8;
					obj8 = new 数据监视器<游戏方向>(o);
					obj8.QuietlySetValue((游戏方向)r.ReadInt32());
					return obj8;
				},
				[typeof(数据监视器<对象发型分类>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<对象发型分类> obj7;
					obj7 = new 数据监视器<对象发型分类>(o);
					obj7.QuietlySetValue((对象发型分类)r.ReadInt32());
					return obj7;
				},
				[typeof(数据监视器<对象发色分类>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<对象发色分类> obj6;
					obj6 = new 数据监视器<对象发色分类>(o);
					obj6.QuietlySetValue((对象发色分类)r.ReadInt32());
					return obj6;
				},
				[typeof(数据监视器<对象脸型分类>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<对象脸型分类> obj5;
					obj5 = new 数据监视器<对象脸型分类>(o);
					obj5.QuietlySetValue((对象脸型分类)r.ReadInt32());
					return obj5;
				},
				[typeof(数据监视器<游戏对象性别>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<游戏对象性别> obj4;
					obj4 = new 数据监视器<游戏对象性别>(o);
					obj4.QuietlySetValue((游戏对象性别)r.ReadInt32());
					return obj4;
				},
				[typeof(数据监视器<游戏对象职业>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<游戏对象职业> obj3;
					obj3 = new 数据监视器<游戏对象职业>(o);
					obj3.QuietlySetValue((游戏对象职业)r.ReadInt32());
					return obj3;
				},
				[typeof(数据监视器<师门数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<师门数据> 数据监视器18;
					数据监视器18 = new 数据监视器<师门数据>(o);
					数据关联表.添加任务(o, f, 数据监视器18, typeof(师门数据), r.ReadInt32());
					return 数据监视器18;
				},
				[typeof(数据监视器<行会数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<行会数据> 数据监视器17;
					数据监视器17 = new 数据监视器<行会数据>(o);
					数据关联表.添加任务(o, f, 数据监视器17, typeof(行会数据), r.ReadInt32());
					return 数据监视器17;
				},
				[typeof(数据监视器<队伍数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<队伍数据> 数据监视器16;
					数据监视器16 = new 数据监视器<队伍数据>(o);
					数据关联表.添加任务(o, f, 数据监视器16, typeof(队伍数据), r.ReadInt32());
					return 数据监视器16;
				},
				[typeof(数据监视器<Buff数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<Buff数据> 数据监视器15;
					数据监视器15 = new 数据监视器<Buff数据>(o);
					数据关联表.添加任务(o, f, 数据监视器15, typeof(Buff数据), r.ReadInt32());
					return 数据监视器15;
				},
				[typeof(数据监视器<邮件数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<邮件数据> 数据监视器14;
					数据监视器14 = new 数据监视器<邮件数据>(o);
					数据关联表.添加任务(o, f, 数据监视器14, typeof(邮件数据), r.ReadInt32());
					return 数据监视器14;
				},
				[typeof(数据监视器<账号数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<账号数据> 数据监视器13;
					数据监视器13 = new 数据监视器<账号数据>(o);
					数据关联表.添加任务(o, f, 数据监视器13, typeof(账号数据), r.ReadInt32());
					return 数据监视器13;
				},
				[typeof(数据监视器<角色数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<角色数据> 数据监视器12;
					数据监视器12 = new 数据监视器<角色数据>(o);
					数据关联表.添加任务(o, f, 数据监视器12, typeof(角色数据), r.ReadInt32());
					return 数据监视器12;
				},
				[typeof(数据监视器<装备数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<装备数据> 数据监视器11;
					数据监视器11 = new 数据监视器<装备数据>(o);
					数据关联表.添加任务(o, f, 数据监视器11, typeof(装备数据), r.ReadInt32());
					return 数据监视器11;
				},
				[typeof(数据监视器<物品数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<物品数据> 数据监视器10;
					数据监视器10 = new 数据监视器<物品数据>(o);
					数据关联表.添加任务(o, f, 数据监视器10, r.ReadBoolean() ? typeof(装备数据) : typeof(物品数据), r.ReadInt32());
					return 数据监视器10;
				},
				[typeof(数据监视器<寄售数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<寄售数据> 数据监视器9;
					数据监视器9 = new 数据监视器<寄售数据>(o);
					数据关联表.添加任务(o, f, 数据监视器9, typeof(寄售数据), r.ReadInt32());
					return 数据监视器9;
				},
				[typeof(哈希监视器<CharacterQuest>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					哈希监视器<CharacterQuest> 哈希监视器14;
					哈希监视器14 = new 哈希监视器<CharacterQuest>(o);
					int num90;
					num90 = r.ReadInt32();
					for (int num91 = 0; num91 < num90; num91++)
					{
						数据关联表.添加任务(o, f, 哈希监视器14.ISet, r.ReadInt32());
					}
					return 哈希监视器14;
				},
				[typeof(数据监视器<CharacterQuest>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<CharacterQuest> 数据监视器8;
					数据监视器8 = new 数据监视器<CharacterQuest>(o);
					数据关联表.添加任务(o, f, 数据监视器8, typeof(CharacterQuest), r.ReadInt32());
					return 数据监视器8;
				},
				[typeof(数据监视器<CharacterQuestMission>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<CharacterQuestMission> 数据监视器7;
					数据监视器7 = new 数据监视器<CharacterQuestMission>(o);
					数据关联表.添加任务(o, f, 数据监视器7, typeof(CharacterQuestMission), r.ReadInt32());
					return 数据监视器7;
				},
				[typeof(哈希监视器<CharacterQuestMission>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					哈希监视器<CharacterQuestMission> 哈希监视器13;
					哈希监视器13 = new 哈希监视器<CharacterQuestMission>(o);
					int num88;
					num88 = r.ReadInt32();
					for (int num89 = 0; num89 < num88; num89++)
					{
						数据关联表.添加任务(o, f, 哈希监视器13.ISet, r.ReadInt32());
					}
					return 哈希监视器13;
				},
				[typeof(数据监视器<龙卫模板>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<龙卫模板> obj2;
					obj2 = new 数据监视器<龙卫模板>(o);
					obj2.QuietlySetValue(龙卫模板.数据表.TryGetValue(r.ReadInt32(), out var value6) ? value6 : null);
					return obj2;
				},
				[typeof(列表监视器<int>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					列表监视器<int> 列表监视器21;
					列表监视器21 = new 列表监视器<int>(o);
					int num86;
					num86 = r.ReadInt32();
					for (int num87 = 0; num87 < num86; num87++)
					{
						列表监视器21.QuietlyAdd(r.ReadInt32());
					}
					return 列表监视器21;
				},
				[typeof(列表监视器<uint>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					列表监视器<uint> 列表监视器20;
					列表监视器20 = new 列表监视器<uint>(o);
					int num84;
					num84 = r.ReadInt32();
					for (int num85 = 0; num85 < num84; num85++)
					{
						列表监视器20.QuietlyAdd(r.ReadUInt32());
					}
					return 列表监视器20;
				},
				[typeof(列表监视器<bool>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					列表监视器<bool> 列表监视器19;
					列表监视器19 = new 列表监视器<bool>(o);
					int num82;
					num82 = r.ReadInt32();
					for (int num83 = 0; num83 < num82; num83++)
					{
						列表监视器19.QuietlyAdd(r.ReadBoolean());
					}
					return 列表监视器19;
				},
				[typeof(列表监视器<byte>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					列表监视器<byte> 列表监视器18;
					列表监视器18 = new 列表监视器<byte>(o);
					int num80;
					num80 = r.ReadInt32();
					for (int num81 = 0; num81 < num80; num81++)
					{
						列表监视器18.QuietlyAdd(r.ReadByte());
					}
					return 列表监视器18;
				},
				[typeof(列表监视器<角色数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					列表监视器<角色数据> 列表监视器17;
					列表监视器17 = new 列表监视器<角色数据>(o);
					int num78;
					num78 = r.ReadInt32();
					for (int num79 = 0; num79 < num78; num79++)
					{
						数据关联表.添加任务(o, f, 列表监视器17.IList, typeof(角色数据), r.ReadInt32());
					}
					return 列表监视器17;
				},
				[typeof(列表监视器<宠物数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					列表监视器<宠物数据> 列表监视器16;
					列表监视器16 = new 列表监视器<宠物数据>(o);
					int num76;
					num76 = r.ReadInt32();
					for (int num77 = 0; num77 < num76; num77++)
					{
						数据关联表.添加任务(o, f, 列表监视器16.IList, typeof(宠物数据), r.ReadInt32());
					}
					return 列表监视器16;
				},
				[typeof(列表监视器<行会数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					列表监视器<行会数据> 列表监视器15;
					列表监视器15 = new 列表监视器<行会数据>(o);
					int num74;
					num74 = r.ReadInt32();
					for (int num75 = 0; num75 < num74; num75++)
					{
						数据关联表.添加任务(o, f, 列表监视器15.IList, typeof(行会数据), r.ReadInt32());
					}
					return 列表监视器15;
				},
				[typeof(列表监视器<行会事记>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					列表监视器<行会事记> 列表监视器14;
					列表监视器14 = new 列表监视器<行会事记>(o);
					int num72;
					num72 = r.ReadInt32();
					for (int num73 = 0; num73 < num72; num73++)
					{
						列表监视器14.QuietlyAdd(new 行会事记
						{
							事记类型 = (事记类型)r.ReadByte(),
							第一参数 = r.ReadInt32(),
							第二参数 = r.ReadInt32(),
							第三参数 = r.ReadInt32(),
							第四参数 = r.ReadInt32(),
							事记时间 = r.ReadInt32()
						});
					}
					return 列表监视器14;
				},
				[typeof(列表监视器<随机属性>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					列表监视器<随机属性> 列表监视器13;
					列表监视器13 = new 列表监视器<随机属性>(o);
					int num70;
					num70 = r.ReadInt32();
					for (int num71 = 0; num71 < num70; num71++)
					{
						if (随机属性.数据表.TryGetValue(r.ReadInt32(), out var value5))
						{
							列表监视器13.QuietlyAdd(value5);
						}
					}
					return 列表监视器13;
				},
				[typeof(列表监视器<装备孔洞颜色>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					列表监视器<装备孔洞颜色> 列表监视器12;
					列表监视器12 = new 列表监视器<装备孔洞颜色>(o);
					int num68;
					num68 = r.ReadInt32();
					for (int num69 = 0; num69 < num68; num69++)
					{
						列表监视器12.QuietlyAdd((装备孔洞颜色)r.ReadInt32());
					}
					return 列表监视器12;
				},
				[typeof(哈希监视器<宠物数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					哈希监视器<宠物数据> 哈希监视器12;
					哈希监视器12 = new 哈希监视器<宠物数据>(o);
					int num66;
					num66 = r.ReadInt32();
					for (int num67 = 0; num67 < num66; num67++)
					{
						数据关联表.添加任务(o, f, 哈希监视器12.ISet, r.ReadInt32());
					}
					return 哈希监视器12;
				},
				[typeof(哈希监视器<角色数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					哈希监视器<角色数据> 哈希监视器11;
					哈希监视器11 = new 哈希监视器<角色数据>(o);
					int num64;
					num64 = r.ReadInt32();
					for (int num65 = 0; num65 < num64; num65++)
					{
						数据关联表.添加任务(o, f, 哈希监视器11.ISet, r.ReadInt32());
					}
					return 哈希监视器11;
				},
				[typeof(哈希监视器<邮件数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					哈希监视器<邮件数据> 哈希监视器10;
					哈希监视器10 = new 哈希监视器<邮件数据>(o);
					int num62;
					num62 = r.ReadInt32();
					for (int num63 = 0; num63 < num62; num63++)
					{
						数据关联表.添加任务(o, f, 哈希监视器10.ISet, r.ReadInt32());
					}
					return 哈希监视器10;
				},
				[typeof(字典监视器<byte, int>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<byte, int> 字典监视器57;
					字典监视器57 = new 字典监视器<byte, int>(o);
					int num60;
					num60 = r.ReadInt32();
					for (int num61 = 0; num61 < num60; num61++)
					{
						字典监视器57.QuietlyAdd(r.ReadByte(), r.ReadInt32());
					}
					return 字典监视器57;
				},
				[typeof(字典监视器<int, int>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<int, int> 字典监视器56;
					字典监视器56 = new 字典监视器<int, int>(o);
					int num58;
					num58 = r.ReadInt32();
					for (int num59 = 0; num59 < num58; num59++)
					{
						字典监视器56.QuietlyAdd(r.ReadInt32(), r.ReadInt32());
					}
					return 字典监视器56;
				},
				[typeof(字典监视器<int, string>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<int, string> 字典监视器55;
					字典监视器55 = new 字典监视器<int, string>(o);
					int num56;
					num56 = r.ReadInt32();
					for (int num57 = 0; num57 < num56; num57++)
					{
						字典监视器55.QuietlyAdd(r.ReadInt32(), r.ReadString());
					}
					return 字典监视器55;
				},
				[typeof(字典监视器<int, bool>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<int, bool> 字典监视器54;
					字典监视器54 = new 字典监视器<int, bool>(o);
					int num54;
					num54 = r.ReadInt32();
					for (int num55 = 0; num55 < num54; num55++)
					{
						字典监视器54.QuietlyAdd(r.ReadInt32(), r.ReadBoolean());
					}
					return 字典监视器54;
				},
				[typeof(字典监视器<ushort, ushort>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<ushort, ushort> 字典监视器53;
					字典监视器53 = new 字典监视器<ushort, ushort>(o);
					int num52;
					num52 = r.ReadInt32();
					for (int num53 = 0; num53 < num52; num53++)
					{
						字典监视器53.QuietlyAdd(r.ReadUInt16(), r.ReadUInt16());
					}
					return 字典监视器53;
				},
				[typeof(字典监视器<ushort, byte>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<ushort, byte> 字典监视器52;
					字典监视器52 = new 字典监视器<ushort, byte>(o);
					int num50;
					num50 = r.ReadInt32();
					for (int num51 = 0; num51 < num50; num51++)
					{
						字典监视器52.QuietlyAdd(r.ReadUInt16(), r.ReadByte());
					}
					return 字典监视器52;
				},
				[typeof(字典监视器<int, DateTime>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<int, DateTime> 字典监视器51;
					字典监视器51 = new 字典监视器<int, DateTime>(o);
					int num48;
					num48 = r.ReadInt32();
					for (int num49 = 0; num49 < num48; num49++)
					{
						字典监视器51.QuietlyAdd(r.ReadInt32(), DateTime.FromBinary(r.ReadInt64()));
					}
					return 字典监视器51;
				},
				[typeof(字典监视器<byte, DateTime>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<byte, DateTime> 字典监视器50;
					字典监视器50 = new 字典监视器<byte, DateTime>(o);
					int num46;
					num46 = r.ReadInt32();
					for (int num47 = 0; num47 < num46; num47++)
					{
						字典监视器50.QuietlyAdd(r.ReadByte(), DateTime.FromBinary(r.ReadInt64()));
					}
					return 字典监视器50;
				},
				[typeof(字典监视器<string, DateTime>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<string, DateTime> 字典监视器49;
					字典监视器49 = new 字典监视器<string, DateTime>(o);
					int num44;
					num44 = r.ReadInt32();
					for (int num45 = 0; num45 < num44; num45++)
					{
						字典监视器49.QuietlyAdd(r.ReadString(), DateTime.FromBinary(r.ReadInt64()));
					}
					return 字典监视器49;
				},
				[typeof(字典监视器<byte, 游戏物品>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<byte, 游戏物品> 字典监视器48;
					字典监视器48 = new 字典监视器<byte, 游戏物品>(o);
					int num42;
					num42 = r.ReadInt32();
					for (int num43 = 0; num43 < num42; num43++)
					{
						byte key3;
						key3 = r.ReadByte();
						int key4;
						key4 = r.ReadInt32();
						if (游戏物品.数据表.TryGetValue(key4, out var value4))
						{
							字典监视器48.QuietlyAdd(key3, value4);
						}
					}
					return 字典监视器48;
				},
				[typeof(字典监视器<byte, 铭文技能>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<byte, 铭文技能> 字典监视器47;
					字典监视器47 = new 字典监视器<byte, 铭文技能>(o);
					int num40;
					num40 = r.ReadInt32();
					for (int num41 = 0; num41 < num40; num41++)
					{
						byte key;
						key = r.ReadByte();
						ushort key2;
						key2 = r.ReadUInt16();
						if (铭文技能.数据表.TryGetValue(key2, out var value3))
						{
							字典监视器47.QuietlyAdd(key, value3);
						}
					}
					return 字典监视器47;
				},
				[typeof(字典监视器<ushort, Buff数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<ushort, Buff数据> 字典监视器46;
					字典监视器46 = new 字典监视器<ushort, Buff数据>(o);
					int num37;
					num37 = r.ReadInt32();
					for (int num38 = 0; num38 < num37; num38++)
					{
						ushort num39;
						num39 = r.ReadUInt16();
						数据关联表.添加任务(值索引: r.ReadInt32(), 数据: o, 字段: f, 内部字典: 字典监视器46.IDictionary_0, 字典键: num39, 字典值: null, 键类型: typeof(ushort), 值类型: typeof(Buff数据), 键索引: 0);
					}
					return 字典监视器46;
				},
				[typeof(字典监视器<ushort, 技能数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<ushort, 技能数据> 字典监视器45;
					字典监视器45 = new 字典监视器<ushort, 技能数据>(o);
					int num34;
					num34 = r.ReadInt32();
					for (int num35 = 0; num35 < num34; num35++)
					{
						ushort num36;
						num36 = r.ReadUInt16();
						数据关联表.添加任务(值索引: r.ReadInt32(), 数据: o, 字段: f, 内部字典: 字典监视器45.IDictionary_0, 字典键: num36, 字典值: null, 键类型: typeof(ushort), 值类型: typeof(技能数据), 键索引: 0);
					}
					return 字典监视器45;
				},
				[typeof(字典监视器<byte, 技能数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<byte, 技能数据> 字典监视器44;
					字典监视器44 = new 字典监视器<byte, 技能数据>(o);
					int num32;
					num32 = r.ReadInt32();
					for (int num33 = 0; num33 < num32; num33++)
					{
						byte b4;
						b4 = r.ReadByte();
						数据关联表.添加任务(值索引: r.ReadInt32(), 数据: o, 字段: f, 内部字典: 字典监视器44.IDictionary_0, 字典键: b4, 字典值: null, 键类型: typeof(byte), 值类型: typeof(技能数据), 键索引: 0);
					}
					return 字典监视器44;
				},
				[typeof(字典监视器<byte, 装备数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<byte, 装备数据> 字典监视器43;
					字典监视器43 = new 字典监视器<byte, 装备数据>(o);
					int num30;
					num30 = r.ReadInt32();
					for (int num31 = 0; num31 < num30; num31++)
					{
						byte b3;
						b3 = r.ReadByte();
						数据关联表.添加任务(值索引: r.ReadInt32(), 数据: o, 字段: f, 内部字典: 字典监视器43.IDictionary_0, 字典键: b3, 字典值: null, 键类型: typeof(byte), 值类型: typeof(装备数据), 键索引: 0);
					}
					return 字典监视器43;
				},
				[typeof(字典监视器<byte, 物品数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<byte, 物品数据> 字典监视器42;
					字典监视器42 = new 字典监视器<byte, 物品数据>(o);
					int num28;
					num28 = r.ReadInt32();
					for (int num29 = 0; num29 < num28; num29++)
					{
						byte b2;
						b2 = r.ReadByte();
						bool flag2;
						flag2 = r.ReadBoolean();
						数据关联表.添加任务(值索引: r.ReadInt32(), 数据: o, 字段: f, 内部字典: 字典监视器42.IDictionary_0, 字典键: b2, 字典值: null, 键类型: typeof(byte), 值类型: flag2 ? typeof(装备数据) : typeof(物品数据), 键索引: 0);
					}
					return 字典监视器42;
				},
				[typeof(字典监视器<int, 角色数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<int, 角色数据> 字典监视器41;
					字典监视器41 = new 字典监视器<int, 角色数据>(o);
					int num25;
					num25 = r.ReadInt32();
					for (int num26 = 0; num26 < num25; num26++)
					{
						int num27;
						num27 = r.ReadInt32();
						数据关联表.添加任务(值索引: r.ReadInt32(), 数据: o, 字段: f, 内部字典: 字典监视器41.IDictionary_0, 字典键: num27, 字典值: null, 键类型: typeof(int), 值类型: typeof(角色数据), 键索引: 0);
					}
					return 字典监视器41;
				},
				[typeof(字典监视器<int, 邮件数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<int, 邮件数据> 字典监视器40;
					字典监视器40 = new 字典监视器<int, 邮件数据>(o);
					int num22;
					num22 = r.ReadInt32();
					for (int num23 = 0; num23 < num22; num23++)
					{
						int num24;
						num24 = r.ReadInt32();
						数据关联表.添加任务(值索引: r.ReadInt32(), 数据: o, 字段: f, 内部字典: 字典监视器40.IDictionary_0, 字典键: num24, 字典值: null, 键类型: typeof(int), 值类型: typeof(邮件数据), 键索引: 0);
					}
					return 字典监视器40;
				},
				[typeof(字典监视器<游戏货币, uint>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<游戏货币, uint> 字典监视器39;
					字典监视器39 = new 字典监视器<游戏货币, uint>(o);
					int num20;
					num20 = r.ReadInt32();
					for (int num21 = 0; num21 < num20; num21++)
					{
						字典监视器39.QuietlyAdd((游戏货币)r.ReadInt32(), r.ReadUInt32());
					}
					return 字典监视器39;
				},
				[typeof(字典监视器<游戏货币, int>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<游戏货币, int> 字典监视器38;
					字典监视器38 = new 字典监视器<游戏货币, int>(o);
					int num18;
					num18 = r.ReadInt32();
					for (int num19 = 0; num19 < num18; num19++)
					{
						字典监视器38.QuietlyAdd((游戏货币)r.ReadInt32(), r.ReadInt32());
					}
					return 字典监视器38;
				},
				[typeof(字典监视器<行会数据, DateTime>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<行会数据, DateTime> 字典监视器37;
					字典监视器37 = new 字典监视器<行会数据, DateTime>(o);
					int num16;
					num16 = r.ReadInt32();
					for (int num17 = 0; num17 < num16; num17++)
					{
						数据关联表.添加任务(键索引: r.ReadInt32(), 字典值: DateTime.FromBinary(r.ReadInt64()), 数据: o, 字段: f, 内部字典: 字典监视器37.IDictionary_0, 字典键: null, 键类型: typeof(行会数据), 值类型: typeof(DateTime), 值索引: 0);
					}
					return 字典监视器37;
				},
				[typeof(字典监视器<角色数据, DateTime>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<角色数据, DateTime> 字典监视器36;
					字典监视器36 = new 字典监视器<角色数据, DateTime>(o);
					int num14;
					num14 = r.ReadInt32();
					for (int num15 = 0; num15 < num14; num15++)
					{
						数据关联表.添加任务(键索引: r.ReadInt32(), 字典值: DateTime.FromBinary(r.ReadInt64()), 数据: o, 字段: f, 内部字典: 字典监视器36.IDictionary_0, 字典键: null, 键类型: typeof(角色数据), 值类型: typeof(DateTime), 值索引: 0);
					}
					return 字典监视器36;
				},
				[typeof(字典监视器<角色数据, 行会职位>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<角色数据, 行会职位> 字典监视器35;
					字典监视器35 = new 字典监视器<角色数据, 行会职位>(o);
					int num12;
					num12 = r.ReadInt32();
					for (int num13 = 0; num13 < num12; num13++)
					{
						数据关联表.添加任务(键索引: r.ReadInt32(), 字典值: (行会职位)r.ReadInt32(), 数据: o, 字段: f, 内部字典: 字典监视器35.IDictionary_0, 字典键: null, 键类型: typeof(角色数据), 值类型: typeof(行会职位), 值索引: 0);
					}
					return 字典监视器35;
				},
				[typeof(字典监视器<DateTime, 行会数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<DateTime, 行会数据> 字典监视器34;
					字典监视器34 = new 字典监视器<DateTime, 行会数据>(o);
					int num10;
					num10 = r.ReadInt32();
					for (int num11 = 0; num11 < num10; num11++)
					{
						long dateData;
						dateData = r.ReadInt64();
						数据关联表.添加任务(值索引: r.ReadInt32(), 数据: o, 字段: f, 内部字典: 字典监视器34.IDictionary_0, 字典键: DateTime.FromBinary(dateData), 字典值: null, 键类型: typeof(DateTime), 值类型: typeof(行会数据), 键索引: 0);
					}
					return 字典监视器34;
				},
				[typeof(字典监视器<角色数据, int>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<角色数据, int> 字典监视器33;
					字典监视器33 = new 字典监视器<角色数据, int>(o);
					int num8;
					num8 = r.ReadInt32();
					for (int n = 0; n < num8; n++)
					{
						int num9;
						num9 = r.ReadInt32();
						数据关联表.添加任务(键索引: r.ReadInt32(), 数据: o, 字段: f, 内部字典: 字典监视器33.IDictionary_0, 字典键: null, 字典值: num9, 键类型: typeof(角色数据), 值类型: typeof(int), 值索引: 0);
					}
					return 字典监视器33;
				},
				[typeof(哈希监视器<龙卫数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					哈希监视器<龙卫数据> 哈希监视器9;
					哈希监视器9 = new 哈希监视器<龙卫数据>(o);
					int num7;
					num7 = r.ReadInt32();
					for (int m = 0; m < num7; m++)
					{
						数据关联表.添加任务(o, f, 哈希监视器9.ISet, r.ReadInt32());
					}
					return 哈希监视器9;
				},
				[typeof(数据监视器<GameQuests>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<GameQuests> obj;
					obj = new 数据监视器<GameQuests>(o);
					obj.QuietlySetValue(GameQuests.数据表.TryGetValue(r.ReadInt32(), out var value2) ? value2 : null);
					return obj;
				},
				[typeof(数据监视器<GameQuestMission>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<GameQuestMission> 数据监视器6;
					数据监视器6 = new 数据监视器<GameQuestMission>(o);
					if (!GameQuests.数据表.TryGetValue(r.ReadInt32(), out var value))
					{
						return 数据监视器6;
					}
					数据监视器6.QuietlySetValue(value.Missions[r.ReadInt32()]);
					return 数据监视器6;
				},
				[typeof(数据监视器<CharacterQuest>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<CharacterQuest> 数据监视器5;
					数据监视器5 = new 数据监视器<CharacterQuest>(o);
					数据关联表.添加任务(o, f, 数据监视器5, typeof(CharacterQuest), r.ReadInt32());
					return 数据监视器5;
				},
				[typeof(数据监视器<CharacterQuestMission>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					数据监视器<CharacterQuestMission> 数据监视器4;
					数据监视器4 = new 数据监视器<CharacterQuestMission>(o);
					数据关联表.添加任务(o, f, 数据监视器4, typeof(CharacterQuestMission), r.ReadInt32());
					return 数据监视器4;
				},
				[typeof(哈希监视器<CharacterQuestMission>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					哈希监视器<CharacterQuestMission> 哈希监视器8;
					哈希监视器8 = new 哈希监视器<CharacterQuestMission>(o);
					int num6;
					num6 = r.ReadInt32();
					for (int l = 0; l < num6; l++)
					{
						数据关联表.添加任务(o, f, 哈希监视器8.ISet, r.ReadInt32());
					}
					return 哈希监视器8;
				},
				[typeof(字典监视器<行会职位, 行会权限>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<行会职位, 行会权限> 字典监视器32;
					字典监视器32 = new 字典监视器<行会职位, 行会权限>(o);
					int num5;
					num5 = r.ReadInt32();
					for (int k = 0; k < num5; k++)
					{
						字典监视器32.QuietlyAdd((行会职位)r.ReadInt32(), (行会权限)r.ReadInt32());
					}
					return 字典监视器32;
				},
				[typeof(字典监视器<int, 物品数据>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<int, 物品数据> 字典监视器31;
					字典监视器31 = new 字典监视器<int, 物品数据>(o);
					int num3;
					num3 = r.ReadInt32();
					for (int j = 0; j < num3; j++)
					{
						int num4;
						num4 = r.ReadInt32();
						bool flag;
						flag = r.ReadBoolean();
						数据关联表.添加任务(值索引: r.ReadInt32(), 数据: o, 字段: f, 内部字典: 字典监视器31.IDictionary_0, 字典键: num4, 字典值: null, 键类型: typeof(int), 值类型: flag ? typeof(装备数据) : typeof(物品数据), 键索引: 0);
					}
					return 字典监视器31;
				},
				[typeof(字典监视器<ushort, AchievementData>)] = delegate(BinaryReader r, 游戏数据 o, 数据字段 f)
				{
					字典监视器<ushort, AchievementData> 字典监视器30;
					字典监视器30 = new 字典监视器<ushort, AchievementData>(o);
					int num;
					num = r.ReadInt32();
					for (int i = 0; i < num; i++)
					{
						ushort num2;
						num2 = r.ReadUInt16();
						数据关联表.添加任务(值索引: r.ReadInt32(), 数据: o, 字段: f, 内部字典: 字典监视器30.IDictionary_0, 字典键: num2, 字典值: null, 键类型: typeof(ushort), 值类型: typeof(AchievementData), 键索引: 0);
					}
					return 字典监视器30;
				}
			};
			数据字段.字段写入方法表 = new Dictionary<Type, Action<BinaryWriter, object>>
			{
				[typeof(数据监视器<int>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<int>)o).V);
				},
				[typeof(数据监视器<uint>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<uint>)o).V);
				},
				[typeof(数据监视器<long>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<long>)o).V);
				},
				[typeof(数据监视器<bool>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<bool>)o).V);
				},
				[typeof(数据监视器<byte>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<byte>)o).V);
				},
				[typeof(数据监视器<sbyte>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<sbyte>)o).V);
				},
				[typeof(数据监视器<string>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<string>)o).V ?? "");
				},
				[typeof(数据监视器<ushort>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<ushort>)o).V);
				},
				[typeof(数据监视器<Point>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<Point>)o).V.X);
					b.Write(((数据监视器<Point>)o).V.Y);
				},
				[typeof(数据监视器<TimeSpan>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<TimeSpan>)o).V.Ticks);
				},
				[typeof(数据监视器<DateTime>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<DateTime>)o).V.ToBinary());
				},
				[typeof(数据监视器<随机属性>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<随机属性>)o).V?.属性编号 ?? 0);
				},
				[typeof(数据监视器<铭文技能>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<铭文技能>)o).V?.铭文索引 ?? 0);
				},
				[typeof(数据监视器<游戏物品>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<游戏物品>)o).V?.物品编号 ?? 0);
				},
				[typeof(数据监视器<宠物模式>)] = delegate(BinaryWriter b, object o)
				{
					b.Write((int)((数据监视器<宠物模式>)o).V);
				},
				[typeof(数据监视器<攻击模式>)] = delegate(BinaryWriter b, object o)
				{
					b.Write((int)((数据监视器<攻击模式>)o).V);
				},
				[typeof(数据监视器<游戏方向>)] = delegate(BinaryWriter b, object o)
				{
					b.Write((int)((数据监视器<游戏方向>)o).V);
				},
				[typeof(数据监视器<对象发型分类>)] = delegate(BinaryWriter b, object o)
				{
					b.Write((int)((数据监视器<对象发型分类>)o).V);
				},
				[typeof(数据监视器<对象发色分类>)] = delegate(BinaryWriter b, object o)
				{
					b.Write((int)((数据监视器<对象发色分类>)o).V);
				},
				[typeof(数据监视器<对象脸型分类>)] = delegate(BinaryWriter b, object o)
				{
					b.Write((int)((数据监视器<对象脸型分类>)o).V);
				},
				[typeof(数据监视器<游戏对象性别>)] = delegate(BinaryWriter b, object o)
				{
					b.Write((int)((数据监视器<游戏对象性别>)o).V);
				},
				[typeof(数据监视器<游戏对象职业>)] = delegate(BinaryWriter b, object o)
				{
					b.Write((int)((数据监视器<游戏对象职业>)o).V);
				},
				[typeof(数据监视器<师门数据>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<师门数据>)o).V?.数据索引.V ?? 0);
				},
				[typeof(数据监视器<行会数据>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<行会数据>)o).V?.数据索引.V ?? 0);
				},
				[typeof(数据监视器<队伍数据>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<队伍数据>)o).V?.数据索引.V ?? 0);
				},
				[typeof(数据监视器<Buff数据>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<Buff数据>)o).V?.数据索引.V ?? 0);
				},
				[typeof(数据监视器<邮件数据>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<邮件数据>)o).V?.数据索引.V ?? 0);
				},
				[typeof(数据监视器<账号数据>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<账号数据>)o).V?.数据索引.V ?? 0);
				},
				[typeof(数据监视器<角色数据>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<角色数据>)o).V?.数据索引.V ?? 0);
				},
				[typeof(数据监视器<装备数据>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<装备数据>)o).V?.数据索引.V ?? 0);
				},
				[typeof(数据监视器<物品数据>)] = delegate(BinaryWriter b, object o)
				{
					数据监视器<物品数据> 数据监视器3;
					数据监视器3 = (数据监视器<物品数据>)o;
					b.Write(数据监视器3.V is 装备数据);
					b.Write(数据监视器3.V?.数据索引.V ?? 0);
				},
				[typeof(数据监视器<龙卫模板>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<龙卫模板>)o).V?.龙卫编号 ?? 0);
				},
				[typeof(数据监视器<CharacterQuest>)] = delegate(BinaryWriter b, object o)
				{
					b.Write((((数据监视器<CharacterQuest>)o)?.V?.数据索引.V).GetValueOrDefault());
				},
				[typeof(数据监视器<寄售数据>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<寄售数据>)o).V?.数据索引.V ?? 0);
				},
				[typeof(哈希监视器<CharacterQuest>)] = delegate(BinaryWriter b, object o)
				{
					哈希监视器<CharacterQuest> 哈希监视器7;
					哈希监视器7 = (哈希监视器<CharacterQuest>)o;
					b.Write(哈希监视器7?.Count ?? 0);
					foreach (CharacterQuest item in 哈希监视器7)
					{
						b.Write(item.数据索引.V);
					}
				},
				[typeof(数据监视器<CharacterQuestMission>)] = delegate(BinaryWriter b, object o)
				{
					b.Write((((数据监视器<CharacterQuestMission>)o)?.V?.数据索引.V).GetValueOrDefault());
				},
				[typeof(哈希监视器<CharacterQuestMission>)] = delegate(BinaryWriter b, object o)
				{
					哈希监视器<CharacterQuestMission> 哈希监视器6;
					哈希监视器6 = (哈希监视器<CharacterQuestMission>)o;
					b.Write(哈希监视器6?.Count ?? 0);
					foreach (CharacterQuestMission item2 in 哈希监视器6)
					{
						b.Write(item2.数据索引.V);
					}
				},
				[typeof(列表监视器<int>)] = delegate(BinaryWriter b, object o)
				{
					列表监视器<int> 列表监视器11;
					列表监视器11 = (列表监视器<int>)o;
					b.Write(列表监视器11?.Count ?? 0);
					foreach (int item3 in 列表监视器11)
					{
						b.Write(item3);
					}
				},
				[typeof(列表监视器<uint>)] = delegate(BinaryWriter b, object o)
				{
					列表监视器<uint> 列表监视器10;
					列表监视器10 = (列表监视器<uint>)o;
					b.Write(列表监视器10?.Count ?? 0);
					foreach (uint item4 in 列表监视器10)
					{
						b.Write(item4);
					}
				},
				[typeof(列表监视器<bool>)] = delegate(BinaryWriter b, object o)
				{
					列表监视器<bool> 列表监视器9;
					列表监视器9 = (列表监视器<bool>)o;
					b.Write(列表监视器9?.Count ?? 0);
					foreach (bool item5 in 列表监视器9)
					{
						b.Write(item5);
					}
				},
				[typeof(列表监视器<byte>)] = delegate(BinaryWriter b, object o)
				{
					列表监视器<byte> 列表监视器8;
					列表监视器8 = (列表监视器<byte>)o;
					b.Write(列表监视器8?.Count ?? 0);
					foreach (byte item6 in 列表监视器8)
					{
						b.Write(item6);
					}
				},
				[typeof(列表监视器<角色数据>)] = delegate(BinaryWriter b, object o)
				{
					列表监视器<角色数据> 列表监视器7;
					列表监视器7 = (列表监视器<角色数据>)o;
					// 假人角色数据内存隔离、永不落盘(见 假人网关.cs): 序列化时跳过假人, 否则其索引重启后无法关联,
					// 触发战力/等级等榜单"数据关联失败"刷屏。假人仍保留在运行时内存榜单中正常显示, 仅不写入存档。
					int 真实数量 = 0;
					if (列表监视器7 != null)
					{
						foreach (角色数据 c7 in 列表监视器7)
						{
							if (!c7.是否假人)
							{
								真实数量++;
							}
						}
					}
					b.Write(真实数量);
					if (列表监视器7 != null)
					{
						foreach (角色数据 item7 in 列表监视器7)
						{
							if (!item7.是否假人)
							{
								b.Write(item7.数据索引.V);
							}
						}
					}
				},
				[typeof(列表监视器<宠物数据>)] = delegate(BinaryWriter b, object o)
				{
					列表监视器<宠物数据> 列表监视器6;
					列表监视器6 = (列表监视器<宠物数据>)o;
					b.Write(列表监视器6?.Count ?? 0);
					foreach (宠物数据 item8 in 列表监视器6)
					{
						b.Write(item8.数据索引.V);
					}
				},
				[typeof(列表监视器<行会数据>)] = delegate(BinaryWriter b, object o)
				{
					列表监视器<行会数据> 列表监视器5;
					列表监视器5 = (列表监视器<行会数据>)o;
					b.Write(列表监视器5?.Count ?? 0);
					foreach (行会数据 item9 in 列表监视器5)
					{
						b.Write(item9.数据索引.V);
					}
				},
				[typeof(列表监视器<行会事记>)] = delegate(BinaryWriter b, object o)
				{
					列表监视器<行会事记> 列表监视器4;
					列表监视器4 = (列表监视器<行会事记>)o;
					b.Write(列表监视器4?.Count ?? 0);
					foreach (行会事记 item10 in 列表监视器4)
					{
						b.Write((byte)item10.事记类型);
						b.Write(item10.第一参数);
						b.Write(item10.第二参数);
						b.Write(item10.第三参数);
						b.Write(item10.第四参数);
						b.Write(item10.事记时间);
					}
				},
				[typeof(列表监视器<随机属性>)] = delegate(BinaryWriter b, object o)
				{
					列表监视器<随机属性> 列表监视器3;
					列表监视器3 = (列表监视器<随机属性>)o;
					b.Write(列表监视器3?.Count ?? 0);
					foreach (随机属性 item11 in 列表监视器3)
					{
						b.Write(item11.属性编号);
					}
				},
				[typeof(列表监视器<装备孔洞颜色>)] = delegate(BinaryWriter b, object o)
				{
					列表监视器<装备孔洞颜色> 列表监视器2;
					列表监视器2 = (列表监视器<装备孔洞颜色>)o;
					b.Write(列表监视器2?.Count ?? 0);
					foreach (装备孔洞颜色 item12 in 列表监视器2)
					{
						b.Write((int)item12);
					}
				},
				[typeof(哈希监视器<宠物数据>)] = delegate(BinaryWriter b, object o)
				{
					哈希监视器<宠物数据> 哈希监视器5;
					哈希监视器5 = (哈希监视器<宠物数据>)o;
					b.Write(哈希监视器5?.Count ?? 0);
					foreach (宠物数据 item13 in 哈希监视器5)
					{
						b.Write(item13.数据索引.V);
					}
				},
				[typeof(哈希监视器<角色数据>)] = delegate(BinaryWriter b, object o)
				{
					哈希监视器<角色数据> 哈希监视器4;
					哈希监视器4 = (哈希监视器<角色数据>)o;
					b.Write(哈希监视器4?.Count ?? 0);
					foreach (角色数据 item14 in 哈希监视器4)
					{
						b.Write(item14.数据索引.V);
					}
				},
				[typeof(哈希监视器<邮件数据>)] = delegate(BinaryWriter b, object o)
				{
					哈希监视器<邮件数据> 哈希监视器3;
					哈希监视器3 = (哈希监视器<邮件数据>)o;
					b.Write(哈希监视器3?.Count ?? 0);
					foreach (邮件数据 item15 in 哈希监视器3)
					{
						b.Write(item15.数据索引.V);
					}
				},
				[typeof(字典监视器<byte, int>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<byte, int> 字典监视器29;
					字典监视器29 = (字典监视器<byte, int>)o;
					b.Write(字典监视器29?.Count ?? 0);
					foreach (KeyValuePair<byte, int> item16 in 字典监视器29)
					{
						b.Write(item16.Key);
						b.Write(item16.Value);
					}
				},
				[typeof(字典监视器<int, int>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<int, int> 字典监视器28;
					字典监视器28 = (字典监视器<int, int>)o;
					b.Write(字典监视器28?.Count ?? 0);
					foreach (KeyValuePair<int, int> item17 in 字典监视器28)
					{
						b.Write(item17.Key);
						b.Write(item17.Value);
					}
				},
				[typeof(字典监视器<int, string>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<int, string> 字典监视器27;
					字典监视器27 = (字典监视器<int, string>)o;
					b.Write(字典监视器27?.Count ?? 0);
					foreach (KeyValuePair<int, string> item18 in 字典监视器27)
					{
						b.Write(item18.Key);
						b.Write(item18.Value);
					}
				},
				[typeof(字典监视器<int, bool>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<int, bool> 字典监视器26;
					字典监视器26 = (字典监视器<int, bool>)o;
					b.Write(字典监视器26?.Count ?? 0);
					foreach (KeyValuePair<int, bool> item19 in 字典监视器26)
					{
						b.Write(item19.Key);
						b.Write(item19.Value);
					}
				},
				[typeof(字典监视器<ushort, ushort>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<ushort, ushort> 字典监视器25;
					字典监视器25 = (字典监视器<ushort, ushort>)o;
					b.Write(字典监视器25?.Count ?? 0);
					foreach (KeyValuePair<ushort, ushort> item20 in 字典监视器25)
					{
						b.Write(item20.Key);
						b.Write(item20.Value);
					}
				},
				[typeof(字典监视器<ushort, byte>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<ushort, byte> 字典监视器24;
					字典监视器24 = (字典监视器<ushort, byte>)o;
					b.Write(字典监视器24?.Count ?? 0);
					foreach (KeyValuePair<ushort, byte> item21 in 字典监视器24)
					{
						b.Write(item21.Key);
						b.Write(item21.Value);
					}
				},
				[typeof(字典监视器<int, DateTime>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<int, DateTime> 字典监视器23;
					字典监视器23 = (字典监视器<int, DateTime>)o;
					b.Write(字典监视器23?.Count ?? 0);
					foreach (KeyValuePair<int, DateTime> item22 in 字典监视器23)
					{
						b.Write(item22.Key);
						b.Write(item22.Value.ToBinary());
					}
				},
				[typeof(字典监视器<byte, DateTime>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<byte, DateTime> 字典监视器22;
					字典监视器22 = (字典监视器<byte, DateTime>)o;
					b.Write(字典监视器22?.Count ?? 0);
					foreach (KeyValuePair<byte, DateTime> item23 in 字典监视器22)
					{
						b.Write(item23.Key);
						b.Write(item23.Value.ToBinary());
					}
				},
				[typeof(字典监视器<string, DateTime>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<string, DateTime> 字典监视器21;
					字典监视器21 = (字典监视器<string, DateTime>)o;
					b.Write(字典监视器21?.Count ?? 0);
					foreach (KeyValuePair<string, DateTime> item24 in 字典监视器21)
					{
						b.Write(item24.Key);
						b.Write(item24.Value.ToBinary());
					}
				},
				[typeof(字典监视器<byte, 游戏物品>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<byte, 游戏物品> 字典监视器20;
					字典监视器20 = (字典监视器<byte, 游戏物品>)o;
					b.Write(字典监视器20?.Count ?? 0);
					foreach (KeyValuePair<byte, 游戏物品> item25 in 字典监视器20)
					{
						b.Write(item25.Key);
						b.Write(item25.Value.物品编号);
					}
				},
				[typeof(字典监视器<byte, 铭文技能>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<byte, 铭文技能> 字典监视器19;
					字典监视器19 = (字典监视器<byte, 铭文技能>)o;
					b.Write(字典监视器19?.Count ?? 0);
					foreach (KeyValuePair<byte, 铭文技能> item26 in 字典监视器19)
					{
						b.Write(item26.Key);
						b.Write(item26.Value.铭文索引);
					}
				},
				[typeof(字典监视器<ushort, Buff数据>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<ushort, Buff数据> 字典监视器18;
					字典监视器18 = (字典监视器<ushort, Buff数据>)o;
					b.Write(字典监视器18?.Count ?? 0);
					foreach (KeyValuePair<ushort, Buff数据> item27 in 字典监视器18)
					{
						b.Write(item27.Key);
						b.Write(item27.Value.数据索引.V);
					}
				},
				[typeof(字典监视器<ushort, 技能数据>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<ushort, 技能数据> 字典监视器17;
					字典监视器17 = (字典监视器<ushort, 技能数据>)o;
					b.Write(字典监视器17?.Count ?? 0);
					foreach (KeyValuePair<ushort, 技能数据> item28 in 字典监视器17)
					{
						b.Write(item28.Key);
						b.Write(item28.Value.数据索引.V);
					}
				},
				[typeof(字典监视器<byte, 技能数据>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<byte, 技能数据> 字典监视器16;
					字典监视器16 = (字典监视器<byte, 技能数据>)o;
					b.Write(字典监视器16?.Count ?? 0);
					foreach (KeyValuePair<byte, 技能数据> item29 in 字典监视器16)
					{
						b.Write(item29.Key);
						b.Write(item29.Value.数据索引.V);
					}
				},
				[typeof(字典监视器<byte, 装备数据>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<byte, 装备数据> 字典监视器15;
					字典监视器15 = (字典监视器<byte, 装备数据>)o;
					b.Write(字典监视器15?.Count ?? 0);
					foreach (KeyValuePair<byte, 装备数据> item30 in 字典监视器15)
					{
						b.Write(item30.Key);
						b.Write(item30.Value.数据索引.V);
					}
				},
				[typeof(字典监视器<byte, 物品数据>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<byte, 物品数据> 字典监视器14;
					字典监视器14 = (字典监视器<byte, 物品数据>)o;
					b.Write(字典监视器14?.Count ?? 0);
					foreach (KeyValuePair<byte, 物品数据> item31 in 字典监视器14)
					{
						b.Write(item31.Key);
						b.Write(item31.Value is 装备数据);
						b.Write(item31.Value.数据索引.V);
					}
				},
				[typeof(字典监视器<int, 角色数据>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<int, 角色数据> 字典监视器13;
					字典监视器13 = (字典监视器<int, 角色数据>)o;
					b.Write(字典监视器13?.Count ?? 0);
					foreach (KeyValuePair<int, 角色数据> item32 in 字典监视器13)
					{
						b.Write(item32.Key);
						b.Write(item32.Value.数据索引.V);
					}
				},
				[typeof(字典监视器<int, 邮件数据>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<int, 邮件数据> 字典监视器12;
					字典监视器12 = (字典监视器<int, 邮件数据>)o;
					b.Write(字典监视器12?.Count ?? 0);
					foreach (KeyValuePair<int, 邮件数据> item33 in 字典监视器12)
					{
						b.Write(item33.Key);
						b.Write(item33.Value.数据索引.V);
					}
				},
				[typeof(字典监视器<游戏货币, uint>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<游戏货币, uint> 字典监视器11;
					字典监视器11 = (字典监视器<游戏货币, uint>)o;
					b.Write(字典监视器11?.Count ?? 0);
					foreach (KeyValuePair<游戏货币, uint> item34 in 字典监视器11)
					{
						b.Write((int)item34.Key);
						b.Write(item34.Value);
					}
				},
				[typeof(字典监视器<游戏货币, int>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<游戏货币, int> 字典监视器10;
					字典监视器10 = (字典监视器<游戏货币, int>)o;
					b.Write(字典监视器10?.Count ?? 0);
					foreach (KeyValuePair<游戏货币, int> item35 in 字典监视器10)
					{
						b.Write((int)item35.Key);
						b.Write(item35.Value);
					}
				},
				[typeof(字典监视器<行会数据, DateTime>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<行会数据, DateTime> 字典监视器9;
					字典监视器9 = (字典监视器<行会数据, DateTime>)o;
					b.Write(字典监视器9?.Count ?? 0);
					foreach (KeyValuePair<行会数据, DateTime> item36 in 字典监视器9)
					{
						b.Write(item36.Key.数据索引.V);
						b.Write(item36.Value.ToBinary());
					}
				},
				[typeof(字典监视器<角色数据, DateTime>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<角色数据, DateTime> 字典监视器8;
					字典监视器8 = (字典监视器<角色数据, DateTime>)o;
					b.Write(字典监视器8?.Count ?? 0);
					foreach (KeyValuePair<角色数据, DateTime> item37 in 字典监视器8)
					{
						b.Write(item37.Key.数据索引.V);
						b.Write(item37.Value.ToBinary());
					}
				},
				[typeof(字典监视器<角色数据, 行会职位>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<角色数据, 行会职位> 字典监视器7;
					字典监视器7 = (字典监视器<角色数据, 行会职位>)o;
					b.Write(字典监视器7?.Count ?? 0);
					foreach (KeyValuePair<角色数据, 行会职位> item38 in 字典监视器7)
					{
						b.Write(item38.Key.数据索引.V);
						b.Write((int)item38.Value);
					}
				},
				[typeof(字典监视器<DateTime, 行会数据>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<DateTime, 行会数据> 字典监视器6;
					字典监视器6 = (字典监视器<DateTime, 行会数据>)o;
					b.Write(字典监视器6?.Count ?? 0);
					foreach (KeyValuePair<DateTime, 行会数据> item39 in 字典监视器6)
					{
						b.Write(item39.Key.ToBinary());
						b.Write(item39.Value.数据索引.V);
					}
				},
				[typeof(字典监视器<角色数据, int>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<角色数据, int> 字典监视器5;
					字典监视器5 = (字典监视器<角色数据, int>)o;
					b.Write(字典监视器5?.Count ?? 0);
					foreach (KeyValuePair<角色数据, int> item40 in 字典监视器5)
					{
						b.Write(item40.Value);
						b.Write(item40.Key.数据索引.V);
					}
				},
				[typeof(哈希监视器<龙卫数据>)] = delegate(BinaryWriter b, object o)
				{
					哈希监视器<龙卫数据> 哈希监视器2;
					哈希监视器2 = (哈希监视器<龙卫数据>)o;
					b.Write(哈希监视器2?.Count ?? 0);
					foreach (龙卫数据 item41 in 哈希监视器2)
					{
						b.Write(item41.数据索引.V);
					}
				},
				[typeof(数据监视器<GameQuests>)] = delegate(BinaryWriter b, object o)
				{
					b.Write(((数据监视器<GameQuests>)o).V?.Id ?? 0);
				},
				[typeof(数据监视器<GameQuestMission>)] = delegate(BinaryWriter b, object o)
				{
					数据监视器<GameQuestMission> 数据监视器2;
					数据监视器2 = (数据监视器<GameQuestMission>)o;
					b.Write(数据监视器2.V?.QuestId ?? 0);
					b.Write(数据监视器2.V?.MissionIndex ?? 0);
				},
				[typeof(字典监视器<行会职位, 行会权限>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<行会职位, 行会权限> 字典监视器4;
					字典监视器4 = (字典监视器<行会职位, 行会权限>)o;
					b.Write(字典监视器4?.Count ?? 0);
					foreach (KeyValuePair<行会职位, 行会权限> item42 in 字典监视器4)
					{
						b.Write((int)item42.Key);
						b.Write((int)item42.Value);
					}
				},
				[typeof(字典监视器<int, 物品数据>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<int, 物品数据> 字典监视器3;
					字典监视器3 = (字典监视器<int, 物品数据>)o;
					b.Write(字典监视器3?.Count ?? 0);
					foreach (KeyValuePair<int, 物品数据> item43 in 字典监视器3)
					{
						b.Write(item43.Key);
						b.Write(item43.Value is 装备数据);
						b.Write(item43.Value.数据索引.V);
					}
				},
				[typeof(字典监视器<ushort, AchievementData>)] = delegate(BinaryWriter b, object o)
				{
					字典监视器<ushort, AchievementData> 字典监视器2;
					字典监视器2 = (字典监视器<ushort, AchievementData>)o;
					b.Write(字典监视器2?.Count ?? 0);
					foreach (KeyValuePair<ushort, AchievementData> item44 in 字典监视器2)
					{
						b.Write(item44.Key);
						b.Write(item44.Value.数据索引.V);
					}
				}
			};
		}

		public override string ToString()
		{
			return this.字段名字;
		}

		public 数据字段(BinaryReader 读取流, Type 数据类型)
		{
			this.字段名字 = 读取流.ReadString();
			this.字段类型Name = 读取流.ReadString();
			this.字段类型 = Type.GetType(this.字段类型Name);
			this.字段详情 = 数据类型?.GetField(this.字段名字);
		}

		public 数据字段(FieldInfo 当前字段)
		{
			this.字段详情 = 当前字段;
			this.字段名字 = 当前字段.Name;
			this.字段类型 = 当前字段.FieldType;
		}

		public bool 检查字段版本(数据字段 对比字段)
		{
			if (string.Compare(this.字段名字, 对比字段.字段名字, StringComparison.Ordinal) == 0)
			{
				return this.字段类型 == 对比字段.字段类型;
			}
			return false;
		}

		public void 保存字段描述(BinaryWriter 写入流)
		{
			写入流.Write(this.字段名字);
			写入流.Write(this.字段类型.FullName);
		}

		public void 保存字段内容(BinaryWriter 写入流, object 数据)
		{
			数据字段.字段写入方法表[this.字段类型](写入流, 数据);
		}

		public object 读取字段内容(BinaryReader 读取流, object 数据, 数据字段 字段)
		{
			try
			{
				return 数据字段.字段读取方法表[this.字段类型](读取流, (游戏数据)数据, 字段);
			}
			catch (Exception ex)
			{
				主程.添加系统日志($"{字段.字段类型Name}读取失败 , Len:{读取流.BaseStream.Length} 线程ID:{Thread.CurrentThread.ManagedThreadId} e:{ex.Message} {((ex.InnerException != null) ? (".IE:" + ex.InnerException.Message) : "...")}");
			}
			return null;
		}
	}
}
