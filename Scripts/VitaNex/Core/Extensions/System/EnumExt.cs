#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System.Collections.Generic;
using System.Linq;

using Server;
#endregion

namespace System
{
	public static class EnumExtUtility
	{
		public static string ToString(this Enum e, bool friendly)
		{
			return friendly ? GetFriendlyName(e) : GetName(e);
		}

		public static string GetName(this Enum e)
		{
			return EnumCache.GetName(e);
		}

		public static string GetFriendlyName(this Enum e)
		{
			return EnumCache.GetFriendlyName(e);
		}

		public static string GetDescription(this Enum e)
		{
			return EnumCache.GetDescription(e);
		}

		public static T GetAttribute<T>(this Enum e) where T : Attribute
		{
			return GetAttributes<T>(e).FirstOrDefault();
		}

		public static IEnumerable<T> GetAttributes<T>(this Enum e) where T : Attribute
		{
			var type = e.GetType();
			var info = type.GetMember(e.ToString());

			if (info.IsNullOrEmpty() || info[0] == null)
			{
				return Enumerable.Empty<T>();
			}

			var attributes = info[0].GetCustomAttributes(typeof(T), false);

			if (attributes.IsNullOrEmpty())
			{
				return Enumerable.Empty<T>();
			}

			return attributes.OfType<T>();
		}

		public static TEnum Normalize<TEnum>(this Enum e)
#if MONO
			where TEnum : struct, IComparable, IFormattable, IConvertible
#else
			where TEnum : struct, Enum
#endif
		{
			var type = typeof(TEnum);
			var flag = default(TEnum);

			if (!type.IsEnum)
			{
				return flag;
			}

			if (!Enum.TryParse(e.ToString(), out flag) || (!type.HasCustomAttribute<FlagsAttribute>(true) && !Enum.IsDefined(type, flag)))
			{
				flag = default(TEnum);
			}

			return flag;
		}

		public static bool IsValid(this Enum e)
		{
			return Enum.IsDefined(e.GetType(), e);
		}

		public static TCast[] Split<TCast>(this Enum e) where TCast : IConvertible
		{
			return GetValues<TCast>(e, true);
		}

		public static TCast[] GetValues<TCast>(this Enum e) where TCast : IConvertible
		{
			return GetValues<TCast>(e, false);
		}

		public static TCast[] GetValues<TCast>(this Enum e, bool local) where TCast : IConvertible
		{
			return EnumerateValues<TCast>(e, local).ToArray();
		}

		public static TCast[] GetAbsoluteValues<TCast>(this Enum e) where TCast : IConvertible
		{
			return GetAbsoluteValues<TCast>(e, false);
		}

		public static TCast[] GetAbsoluteValues<TCast>(this Enum e, bool local) where TCast : IConvertible
		{
			return EnumerateAbsoluteValues<TCast>(e, local).ToArray();
		}

		public static IEnumerable<TCast> EnumerateAbsoluteValues<TCast>(this Enum e) where TCast : IConvertible
		{
			return EnumerateAbsoluteValues<TCast>(e, false);
		}

		public static IEnumerable<TCast> EnumerateAbsoluteValues<TCast>(this Enum e, bool local) where TCast : IConvertible
		{
			return EnumerateValues<TCast>(e, local).Where(o => !Equals(o, 0) && !Equals(o, String.Empty)).ToArray();
		}

		public static IEnumerable<TCast> EnumerateValues<TCast>(this Enum e) where TCast : IConvertible
		{
			return EnumerateValues<TCast>(e, false);
		}

		public static IEnumerable<TCast> EnumerateValues<TCast>(this Enum e, bool local) where TCast : IConvertible
		{
			var eType = e.GetType();
			var vType = typeof(TCast);
			var vals = EnumCache.EnumerateValues(eType);

			if (local)
			{
				vals = vals.Where(e.HasFlag);
			}

			if (vType.IsEqual(typeof(char)))
			{
				return vals.Select(Convert.ToChar).Cast<TCast>();
			}

			if (vType.IsEqual(typeof(sbyte)))
			{
				return vals.Select(Convert.ToSByte).Cast<TCast>();
			}

			if (vType.IsEqual(typeof(byte)))
			{
				return vals.Select(Convert.ToByte).Cast<TCast>();
			}

			if (vType.IsEqual(typeof(short)))
			{
				return vals.Select(Convert.ToInt16).Cast<TCast>();
			}

			if (vType.IsEqual(typeof(ushort)))
			{
				return vals.Select(Convert.ToUInt16).Cast<TCast>();
			}

			if (vType.IsEqual(typeof(int)))
			{
				return vals.Select(Convert.ToInt32).Cast<TCast>();
			}

			if (vType.IsEqual(typeof(uint)))
			{
				return vals.Select(Convert.ToUInt32).Cast<TCast>();
			}

			if (vType.IsEqual(typeof(long)))
			{
				return vals.Select(Convert.ToInt64).Cast<TCast>();
			}

			if (vType.IsEqual(typeof(ulong)))
			{
				return vals.Select(Convert.ToUInt64).Cast<TCast>();
			}

			if (vType.IsEqual(typeof(string)))
			{
				return vals.Select(Convert.ToString).Cast<TCast>();
			}

			return vals.Cast<TCast>();
		}

		public static bool AnyFlags<TEnum>(this Enum e, IEnumerable<TEnum> flags)
#if MONO
			where TEnum : struct, IComparable, IFormattable, IConvertible
#else
			where TEnum : struct, Enum
#endif
		{
			if (!e.GetType().HasCustomAttribute<FlagsAttribute>(true))
			{
				return flags != null && flags.Any(o => Equals(e, o));
			}

			return flags != null && flags.Cast<Enum>().Any(e.HasFlag);
		}

		public static bool AnyFlags<TEnum>(this Enum e, params TEnum[] flags)
#if MONO
			where TEnum : struct, IComparable, IFormattable, IConvertible
#else
			where TEnum : struct, Enum
#endif
		{
			if (!e.GetType().HasCustomAttribute<FlagsAttribute>(true))
			{
				return flags != null && flags.Any(o => Equals(e, o));
			}

			return flags != null && flags.Cast<Enum>().Any(e.HasFlag);
		}

		public static bool AllFlags<TEnum>(this Enum e, IEnumerable<TEnum> flags)
#if MONO
			where TEnum : struct, IComparable, IFormattable, IConvertible
#else
			where TEnum : struct, Enum
#endif
		{
			if (!e.GetType().HasCustomAttribute<FlagsAttribute>(true))
			{
				return flags != null && flags.All(o => Equals(e, o));
			}

			return flags != null && flags.Cast<Enum>().All(e.HasFlag);
		}

		public static bool AllFlags<TEnum>(this Enum e, params TEnum[] flags)
#if MONO
			where TEnum : struct, IComparable, IFormattable, IConvertible
#else
			where TEnum : struct, Enum
#endif
		{
			if (!e.GetType().HasCustomAttribute<FlagsAttribute>(true))
			{
				return flags != null && flags.All(o => Equals(e, o));
			}

			return flags != null && flags.Cast<Enum>().All(e.HasFlag);
		}

		private static class EnumCache
		{
			public class Entry
			{
				public readonly Enum[] Values;
				public readonly string[] Names, FriendlyNames, Descriptions;

				public Entry(Type type)
				{
					if (type == null)
					{
						Values = new Enum[0];
						Names = new string[0];
						FriendlyNames = new string[0];
						Descriptions = new string[0];
					}
					else
					{
						Values = Enum.GetValues(type).CastToArray<Enum>();
						Names = Enum.GetNames(type);

						FriendlyNames = new string[Names.Length];
						Descriptions = new string[Names.Length];

						for (var i = 0; i < Values.Length; i++)
						{
							FriendlyNames[i] = Names[i].SpaceWords();
							Descriptions[i] = String.Join("\n", GetAttributes<DescriptionAttribute>(Values[i]).Where(o => !String.IsNullOrWhiteSpace(o.Description)));
						}
					}
				}
			}

			private static readonly Entry _Empty = new Entry(null);

			private static readonly Dictionary<Type, Entry> _Cache = new Dictionary<Type, Entry>();

			public static Entry Lookup(Type type)
			{
				if (type == null || !type.IsEnum)
				{
					return _Empty;
				}

				if (!_Cache.TryGetValue(type, out var result))
				{
					_Cache[type] = result = new Entry(type);
				}

				return result;
			}

			public static string[] GetNames(Type type)
			{
				return Lookup(type).Names;
			}

			public static Enum[] GetValues(Type type)
			{
				return Lookup(type).Values;
			}

			public static string[] GetDescriptions(Type type)
			{
				return Lookup(type).Descriptions;
			}

			public static IEnumerable<string> EnumerateNames(Type type)
			{
				return GetNames(type);
			}

			public static IEnumerable<Enum> EnumerateValues(Type type)
			{
				return GetValues(type);
			}

			public static IEnumerable<string> EnumerateDescriptions(Type type)
			{
				return GetDescriptions(type);
			}

			public static string GetName(Enum e)
			{
				var type = e.GetType();
				var entry = Lookup(type);

				var index = Array.IndexOf(entry.Values, e);

				if (index >= 0)
				{
					return entry.Names[index];
				}

				return String.Empty;
			}

			public static string GetFriendlyName(Enum e)
			{
				var type = e.GetType();
				var entry = Lookup(type);

				var index = Array.IndexOf(entry.Values, e);

				if (index >= 0)
				{
					return entry.FriendlyNames[index];
				}

				return String.Empty;
			}

			public static string GetDescription(Enum e)
			{
				var type = e.GetType();
				var entry = Lookup(type);

				var index = Array.IndexOf(entry.Values, e);

				if (index >= 0)
				{
					return entry.Descriptions[index];
				}

				return String.Empty;
			}

			public static int GetIndex(Enum e)
			{
				var type = e.GetType();
				var entry = Lookup(type);

				return Array.IndexOf(entry.Values, e);
			}
		}
	}
}
