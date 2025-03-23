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
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using Server;

using VitaNex.Crypto;
#endregion

namespace System
{
	public static class TypeExtUtility
	{
		private static readonly Dictionary<Type, List<Type>> _ChildrenCache;
		private static readonly Dictionary<Type, List<Type>> _ConstructableChildrenCache;

		private static readonly Dictionary<Type, int> _ValueHashCache;

		static TypeExtUtility()
		{
			_ChildrenCache = new Dictionary<Type, List<Type>>(0x100);
			_ConstructableChildrenCache = new Dictionary<Type, List<Type>>(0x100);

			_ValueHashCache = new Dictionary<Type, int>(0x400);
		}

		private static string FormatName(string value)
		{
			var i = value.IndexOf('`');

			return (i > 0 ? value.Substring(0, i) : value).SpaceWords();
		}

		public static string ResolveName(this Type t)
		{
			return FormatName(t.Name);
		}

		public static int GetValueHashCode(this Type t)
		{
			if (t == null)
			{
				return 0;
			}

			if (!_ValueHashCache.TryGetValue(t, out var hash) || hash == 0)
			{
				using (var c = new CryptoHashCode(CryptoHashType.MD5, t.FullName))
				{
					_ValueHashCache[t] = hash = c.ValueHash;
				}
			}

			return hash;
		}

		public static Type[] GetTypeCache(this Assembly asm)
		{
			return ScriptCompiler.GetTypeCache(asm).Types;
		}

		public static Type[] GetHierarchy(this Type t)
		{
			return GetHierarchy(t, false);
		}

		public static Type[] GetHierarchy(this Type t, bool self)
		{
			return EnumerateHierarchy(t, self).ToArray();
		}

		public static IEnumerable<Type> EnumerateHierarchy(this Type t)
		{
			return EnumerateHierarchy(t, false);
		}

		public static IEnumerable<Type> EnumerateHierarchy(this Type t, bool self)
		{
			if (t == null)
			{
				yield break;
			}

			if (self)
			{
				yield return t;
			}

			while (t.BaseType != null)
			{
				yield return t = t.BaseType;
			}
		}

		public static Type FindParent<T>(this Type type)
		{
			var ot = typeof(T);

			return EnumerateHierarchy(type, false).FirstOrDefault(pt => pt == ot);
		}

		public static bool TryFindParent<T>(this Type type, out Type parent)
		{
			return (parent = FindParent<T>(type)) != null;
		}

		public static Type FindParent<T>(this Type type, Func<Type, bool> predicate)
		{
			var ot = typeof(T);

			return EnumerateHierarchy(type, false).Where(t => t == ot).FirstOrDefault(predicate);
		}

		public static bool TryFindParent<T>(this Type type, Func<Type, bool> predicate, out Type parent)
		{
			return (parent = FindParent<T>(type, predicate)) != null;
		}

		public static Type FindParent(this Type type, Func<Type, bool> predicate)
		{
			return EnumerateHierarchy(type, false).FirstOrDefault(predicate);
		}

		public static bool TryFindParent(this Type type, Func<Type, bool> predicate, out Type parent)
		{
			return (parent = FindParent(type, predicate)) != null;
		}

		public static bool GetCustomAttributes<TAttribute>(this Type t, bool inherit, out TAttribute[] attrs)
			where TAttribute : Attribute
		{
			attrs = GetCustomAttributes<TAttribute>(t, inherit);
			return attrs != null && attrs.Length > 0;
		}

		public static TAttribute[] GetCustomAttributes<TAttribute>(this Type t, bool inherit)
			where TAttribute : Attribute
		{
			return t != null
				? t.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().ToArray()
				: new TAttribute[0];
		}

		public static bool HasCustomAttribute<TAttribute>(this Type t, bool inherit)
			where TAttribute : Attribute
		{
			var attrs = GetCustomAttributes<TAttribute>(t, inherit);

			return attrs != null && attrs.Length > 0;
		}

		public static int CompareTo(this Type t, Type other)
		{
			var result = 0;

			if (t.CompareNull(other, ref result))
			{
				return result;
			}

			var lp = t.BaseType;

			while (lp != null)
			{
				if (lp == other)
				{
					return -1;
				}

				lp = lp.BaseType;
			}

			return 1;
		}

		public static bool IsEqual(this Type a, string bName)
		{
			return IsEqual(a, bName, true);
		}

		public static bool IsEqual(this Type a, string bName, bool ignoreCase)
		{
			return IsEqual(a, bName, ignoreCase, bName.ContainsAny('.', '+'));
		}

		public static bool IsEqual(this Type a, string bName, bool ignoreCase, bool fullName)
		{
			var b = Type.GetType(bName) ??
					(fullName ? ScriptCompiler.FindTypeByFullName(bName) : ScriptCompiler.FindTypeByName(bName));

			return IsEqual(a, b);
		}

		public static bool IsEqual(this Type a, Type b)
		{
			return a == b;
		}

		public static bool IsEqual<TObj>(this Type t)
		{
			return IsEqual(t, typeof(TObj));
		}

		public static bool IsEqualOrChildOf(this Type a, string bName)
		{
			return IsEqualOrChildOf(a, bName, true);
		}

		public static bool IsEqualOrChildOf(this Type a, string bName, bool ignoreCase)
		{
			return IsEqualOrChildOf(a, bName, ignoreCase, bName.ContainsAny('.', '+'));
		}

		public static bool IsEqualOrChildOf(this Type a, string bName, bool ignoreCase, bool fullName)
		{
			var b = Type.GetType(bName) ??
					(fullName ? ScriptCompiler.FindTypeByFullName(bName) : ScriptCompiler.FindTypeByName(bName));

			return IsEqualOrChildOf(a, b);
		}

		public static bool IsEqualOrChildOf(this Type a, Type b)
		{
			return IsEqual(a, b) || IsChildOf(a, b);
		}

		public static bool IsEqualOrChildOf<TObj>(this Type t)
		{
			return IsEqualOrChildOf(t, typeof(TObj));
		}

		public static bool IsChildOf(this Type a, Type b)
		{
			return a != null && b != null && a != b && !a.IsInterface && !a.IsEnum &&
				   (b.IsInterface ? HasInterface(a, b) : b.IsAssignableFrom(a));
		}

		public static bool IsChildOf<TObj>(this Type t)
		{
			return IsChildOf(t, typeof(TObj));
		}

		public static bool HasInterface(this Type t, string i)
		{
			var iType = Type.GetType(i, false) ??
						(i.IndexOf('.') < 0 ? ScriptCompiler.FindTypeByName(i) : ScriptCompiler.FindTypeByFullName(i));

			return iType != null && iType.IsInterface && HasInterface(t, iType);
		}

		public static bool HasInterface<TObj>(this Type t)
		{
			return HasInterface(t, typeof(TObj));
		}

		public static bool HasInterface(this Type t, Type i)
		{
			return t != null && i != null && i.IsInterface && t.GetInterface(i.FullName) != null;
		}

		public static bool IsConstructable(this Type a)
		{
			return IsConstructable(a, Type.EmptyTypes);
		}

		public static bool IsConstructable(this Type a, Type[] argTypes)
		{
			if (a == null || a.IsAbstract || a.IsInterface || a.IsEnum)
			{
				return false;
			}

			return a.GetConstructor(argTypes) != null;
		}

		public static bool IsConstructableFrom(this Type a, Type b)
		{
			if (a == null || b == null || a.IsAbstract || !IsChildOf(a, b))
			{
				return false;
			}

			return a.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Length > 0;
		}

		public static bool IsConstructableFrom<TObj>(this Type t)
		{
			return IsConstructableFrom(t, typeof(TObj));
		}

		public static bool IsConstructableFrom(this Type a, Type b, Type[] argTypes)
		{
			if (a == null || b == null || a.IsAbstract || !IsChildOf(a, b))
			{
				return false;
			}

			return a.GetConstructor(argTypes) != null;
		}

		public static bool IsConstructableFrom<TObj>(this Type t, Type[] argTypes)
		{
			return IsConstructableFrom(t, typeof(TObj), argTypes);
		}

		public static Type[] GetChildren(this Type type, Func<Type, bool> predicate = null)
		{
			return FindChildren(type, predicate).ToArray();
		}

		public static IEnumerable<Type> FindChildren(this Type type, Func<Type, bool> predicate = null)
		{
			if (type == null)
			{
				return Type.EmptyTypes;
			}

			var types = _ChildrenCache.GetValue(type);

			if (types == null)
			{
				var asm = ScriptCompiler.Assemblies.With(Core.Assembly, Assembly.GetCallingAssembly()).ToList();

				asm.Prune();

				types = asm.Select(GetTypeCache).SelectMany(o => o.Where(t => !IsEqual(t, type) && IsChildOf(t, type))).ToList();

				asm.Free(true);

				if (types.Count > 0 && types.Count <= 0x100)
				{
					_ChildrenCache[type] = types;
				}
			}

			if (_ChildrenCache.Count >= 0x100)
			{
				_ChildrenCache.Pop().Value.Free(true);
			}

			return predicate != null ? types.Where(predicate) : types.AsEnumerable();
		}

		public static Type[] GetConstructableChildren(this Type type, Func<Type, bool> predicate = null)
		{
			return FindConstructableChildren(type, predicate).ToArray();
		}

		public static IEnumerable<Type> FindConstructableChildren(this Type type, Func<Type, bool> predicate = null)
		{
			if (type == null)
			{
				return Type.EmptyTypes;
			}

			var types = _ConstructableChildrenCache.GetValue(type);

			if (types == null)
			{
				types = FindChildren(type).Where(t => IsConstructableFrom(t, type)).ToList();

				if (types.Count > 0 && types.Count <= 0x100)
				{
					_ConstructableChildrenCache[type] = types;
				}
			}

			if (_ConstructableChildrenCache.Count >= 0x100)
			{
				_ConstructableChildrenCache.Pop().Value.Free(true);
			}

			return predicate != null ? types.Where(predicate) : types.AsEnumerable();
		}

		public static TObj CreateInstance<TObj>(this Type t, params object[] args)
		{
			if (t == null || t.IsAbstract || t.IsInterface || t.IsEnum)
			{
				return default(TObj);
			}

			if (args == null || args.Length == 0)
			{
				return (TObj)Activator.CreateInstance(t, true);
			}

			return (TObj)Activator.CreateInstance(t, args);
		}

		public static TObj CreateInstanceSafe<TObj>(this Type t, params object[] args)
		{
			try
			{
				return CreateInstance<TObj>(t, args);
			}
			catch (Exception e)
			{
				e = new TypeConstructException(t, new StackTrace(1, true), e);

				e.ToConsole(true, true);

				return default(TObj);
			}
		}

		public static object CreateInstanceUnsafe(this Type t, params object[] args)
		{
			return CreateInstance<object>(t, args);
		}

		public static object CreateInstance(this Type t, params object[] args)
		{
			return CreateInstanceSafe<object>(t, args);
		}
	}

	public class TypeConstructException : Exception
	{
		private readonly string _Trace;

		public override string StackTrace => _Trace;

		public TypeConstructException(Type type, StackTrace trace, Exception inner)
			: base("Type Construction Failed: " + type.FullName, inner)
		{
			_Trace = trace.ToString();
		}

		public override string ToString()
		{
			return base.ToString() + "\n\n" + _Trace;
		}
	}
}