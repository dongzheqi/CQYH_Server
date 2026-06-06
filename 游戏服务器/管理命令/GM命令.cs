using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace 游戏服务器.管理命令
{
	public abstract class GM命令
	{
		private static readonly Dictionary<string, Type> 命令字典;

		private static readonly Dictionary<string, FieldInfo[]> 字段列表;

		private static readonly Dictionary<Type, Func<string, object>> 字段写入方法表;

		public static readonly Dictionary<string, string> 命令格式;

		public abstract 执行方式 执行方式 { get; }

		static GM命令()
		{
			GM命令.命令字典 = new Dictionary<string, Type>();
			GM命令.命令格式 = new Dictionary<string, string>();
			GM命令.字段列表 = new Dictionary<string, FieldInfo[]>();
			Type[] types;
			types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (Type type in types)
			{
				if (!type.IsSubclassOf(typeof(GM命令)))
				{
					continue;
				}
				Dictionary<FieldInfo, int> 字段集合;
				字段集合 = new Dictionary<FieldInfo, int>();
				FieldInfo[] fields;
				fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
				foreach (FieldInfo fieldInfo in fields)
				{
					字段描述 customAttribute;
					customAttribute = CustomAttributeExtensions.GetCustomAttribute<字段描述>(fieldInfo);
					if (customAttribute != null)
					{
						字段集合.Add(fieldInfo, customAttribute.排序);
					}
				}
				GM命令.命令字典[type.Name] = type;
				GM命令.字段列表[type.Name] = 字段集合.Keys.OrderBy((FieldInfo x) => 字段集合[x]).ToArray();
				GM命令.命令格式[type.Name] = "@" + type.Name;
				fields = GM命令.字段列表[type.Name];
				foreach (FieldInfo fieldInfo2 in fields)
				{
					字段描述 customAttribute2;
					customAttribute2 = CustomAttributeExtensions.GetCustomAttribute<字段描述>(fieldInfo2);
					Dictionary<string, string> dictionary;
					dictionary = GM命令.命令格式;
					string name;
					name = type.Name;
					dictionary[name] = dictionary[name] + " " + ((customAttribute2 == null || !customAttribute2.可选) ? fieldInfo2.Name : ("[" + fieldInfo2.Name + "]"));
				}
			}
			GM命令.字段写入方法表 = new Dictionary<Type, Func<string, object>>
			{
				[typeof(string)] = (string s) => s,
				[typeof(int)] = (string s) => Convert.ToInt32(s),
				[typeof(uint)] = (string s) => Convert.ToUInt32(s),
				[typeof(byte)] = (string s) => Convert.ToByte(s),
				[typeof(bool)] = (string s) => Convert.ToBoolean(s),
				[typeof(float)] = (string s) => Convert.ToSingle(s),
				[typeof(decimal)] = (string s) => Convert.ToDecimal(s),
				[typeof(short)] = (string s) => Convert.ToInt16(s),
				[typeof(ushort)] = (string s) => Convert.ToUInt16(s),
				[typeof(long)] = (string s) => Convert.ToInt64(s),
				[typeof(ulong)] = (string s) => Convert.ToUInt64(s),
				[typeof(double)] = (string s) => Convert.ToDouble(s)
			};
		}

		public static bool 解析命令(string 文本, out GM命令 命令)
		{
			string[] array;
			array = 文本.Trim('@').Split(new char[2] { ' ', '|' }, StringSplitOptions.RemoveEmptyEntries);
			// 玩家发纯 "@" 或 "@   "(全空白)时, RemoveEmptyEntries 会得到空数组, 下面访问 array[0] 即抛 IndexOutOfRangeException.
			// 这条路径来自玩家聊天输入(远程可达), 必须先守卫长度.
			if (array.Length == 0)
			{
				命令 = null;
				return false;
			}
			if (GM命令.命令字典.TryGetValue(array[0], out var value) && GM命令.字段列表.TryGetValue(array[0], out var value2))
			{
				int num;
				num = value2.Length;
				for (int num2 = value2.Length - 1; num2 >= 0; num2--)
				{
					字段描述 customAttribute;
					customAttribute = CustomAttributeExtensions.GetCustomAttribute<字段描述>(value2[num2]);
					if (customAttribute == null || !customAttribute.可选)
					{
						break;
					}
					num--;
				}
				if (array.Length <= num)
				{
					主程.添加命令日志("<= @参数长度错误, 请参照格式: " + GM命令.命令格式[array[0]]);
					命令 = null;
					return false;
				}
				GM命令 gM命令;
				gM命令 = Activator.CreateInstance(value) as GM命令;
				for (int i = 0; i < value2.Length; i++)
				{
					if (array.Length <= i + 1)
					{
						continue;
					}
					try
					{
						Type type;
						type = value2[i].FieldType;
						if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
						{
							type = type.GetGenericArguments()[0];
						}
						value2[i].SetValue(gM命令, GM命令.字段写入方法表[type](array[i + 1]));
					}
					catch
					{
						主程.添加命令日志("<= @参数转换错误. 不能将字符串 '" + array[i + 1] + "' 转换为参数 '" + value2[i].Name + "' 所需要的数据类型");
						命令 = null;
						return false;
					}
				}
				命令 = gM命令;
				return true;
			}
			主程.添加命令日志("<= @命令解析错误, '" + array[0] + "' 不是支持的GM命令");
			命令 = null;
			return false;
		}

		public abstract void 执行命令();
	}
}
