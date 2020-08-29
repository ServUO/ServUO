#region References
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
#endregion

namespace Server
{
	public static class ScriptCompiler
	{
		public static Assembly[] Assemblies { get; set; }

		public static bool Compile(bool debug, bool cache)
		{
			List<Assembly> assemblies = new List<Assembly>();

			assemblies.Add(Assembly.LoadFrom("Scripts.dll"));
			assemblies.Add(typeof(ScriptCompiler).Assembly);

			Assemblies = assemblies.ToArray();

			Utility.PushColor(ConsoleColor.Yellow);
			Console.WriteLine("Scripts: Verifying...");
			Utility.PopColor();

			Stopwatch watch = Stopwatch.StartNew();

			Core.VerifySerialization();

			watch.Stop();

			Utility.PushColor(ConsoleColor.Green);
			Console.WriteLine(
				"Finished ({0} items, {1} mobiles, {2} customs) ({3:F2} seconds)",
				Core.ScriptItems,
				Core.ScriptMobiles,
				Core.ScriptCustoms,
				watch.Elapsed.TotalSeconds);
			Utility.PopColor();

			return true;
		}

		public static void Invoke(string method)
		{
			List<MethodInfo> invoke = new List<MethodInfo>();

			foreach (Assembly a in Assemblies)
			{
				Type[] types = a.GetTypes();

				foreach (Type t in types)
				{
					MethodInfo m = t.GetMethod(method, BindingFlags.Static | BindingFlags.Public);

					if (m != null)
					{
						invoke.Add(m);
					}
				}
			}

			invoke.Sort(new CallPriorityComparer());

			foreach (MethodInfo m in invoke)
			{
				m.Invoke(null, null);
			}
		}

		private static readonly Dictionary<Assembly, TypeCache> m_TypeCaches = new Dictionary<Assembly, TypeCache>();
		private static TypeCache m_NullCache;

		public static TypeCache GetTypeCache(Assembly asm)
		{
			if (asm == null)
			{
				return m_NullCache ?? (m_NullCache = new TypeCache(null));
			}


			m_TypeCaches.TryGetValue(asm, out TypeCache c);

			if (c == null)
			{
				m_TypeCaches[asm] = c = new TypeCache(asm);
			}

			return c;
		}

		public static Type FindTypeByFullName(string fullName)
		{
			return FindTypeByFullName(fullName, true);
		}

		public static Type FindTypeByFullName(string fullName, bool ignoreCase)
		{
			if (string.IsNullOrWhiteSpace(fullName))
			{
				return null;
			}

			Type type = null;

			for (int i = 0; type == null && i < Assemblies.Length; ++i)
			{
				type = GetTypeCache(Assemblies[i]).GetTypeByFullName(fullName, ignoreCase);
			}

			return type ?? GetTypeCache(Core.Assembly).GetTypeByFullName(fullName, ignoreCase);
		}

		public static IEnumerable<Type> FindTypesByFullName(string name)
		{
			return FindTypesByFullName(name, true);
		}

		public static IEnumerable<Type> FindTypesByFullName(string name, bool ignoreCase)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				yield break;
			}

			for (int i = 0; i < Assemblies.Length; ++i)
			{
				foreach (Type t in GetTypeCache(Assemblies[i]).GetTypesByFullName(name, ignoreCase))
				{
					yield return t;
				}
			}

			foreach (Type t in GetTypeCache(Core.Assembly).GetTypesByFullName(name, ignoreCase))
			{
				yield return t;
			}
		}

		public static Type FindTypeByName(string name)
		{
			return FindTypeByName(name, true);
		}

		public static Type FindTypeByName(string name, bool ignoreCase)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				return null;
			}

			Type type = null;

			for (int i = 0; type == null && i < Assemblies.Length; ++i)
			{
				type = GetTypeCache(Assemblies[i]).GetTypeByName(name, ignoreCase);
			}

			return type ?? GetTypeCache(Core.Assembly).GetTypeByName(name, ignoreCase);
		}

		public static IEnumerable<Type> FindTypesByName(string name)
		{
			return FindTypesByName(name, true);
		}

		public static IEnumerable<Type> FindTypesByName(string name, bool ignoreCase)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				yield break;
			}

			for (int i = 0; i < Assemblies.Length; ++i)
			{
				foreach (Type t in GetTypeCache(Assemblies[i]).GetTypesByName(name, ignoreCase))
				{
					yield return t;
				}
			}

			foreach (Type t in GetTypeCache(Core.Assembly).GetTypesByName(name, ignoreCase))
			{
				yield return t;
			}
		}

	}

	public class TypeCache
	{
		private readonly Type[] m_Types;
		private readonly TypeTable m_Names;
		private readonly TypeTable m_FullNames;

		public Type[] Types => m_Types;
		public TypeTable Names => m_Names;
		public TypeTable FullNames => m_FullNames;

		public Type GetTypeByName(string name, bool ignoreCase)
		{
			return GetTypesByName(name, ignoreCase).FirstOrDefault(t => t != null);
		}

		public IEnumerable<Type> GetTypesByName(string name, bool ignoreCase)
		{
			return m_Names.Get(name, ignoreCase);
		}

		public Type GetTypeByFullName(string fullName, bool ignoreCase)
		{
			return GetTypesByFullName(fullName, ignoreCase).FirstOrDefault(t => t != null);
		}

		public IEnumerable<Type> GetTypesByFullName(string fullName, bool ignoreCase)
		{
			return m_FullNames.Get(fullName, ignoreCase);
		}

		public TypeCache(Assembly asm)
		{
			if (asm == null)
			{
				m_Types = Type.EmptyTypes;
			}
			else
			{
				m_Types = asm.GetTypes();
			}

			m_Names = new TypeTable(m_Types.Length);
			m_FullNames = new TypeTable(m_Types.Length);

			foreach (IGrouping<string, Type> g in m_Types.ToLookup(t => t.Name))
			{
				m_Names.Add(g.Key, g);

				foreach (Type type in g)
				{
					m_FullNames.Add(type.FullName, type);

					TypeAliasAttribute attr = type.GetCustomAttribute<TypeAliasAttribute>(false);

					if (attr != null)
					{
						foreach (string a in attr.Aliases)
						{
							m_FullNames.Add(a, type);
						}
					}
				}
			}

			m_Names.Prune();
			m_FullNames.Prune();

			m_Names.Sort();
			m_FullNames.Sort();
		}
	}

	public class TypeTable
	{
		private readonly Dictionary<string, List<Type>> m_Sensitive;
		private readonly Dictionary<string, List<Type>> m_Insensitive;

		public void Prune()
		{
			Prune(m_Sensitive);
			Prune(m_Insensitive);
		}

		private static void Prune(Dictionary<string, List<Type>> types)
		{
			List<Type> buffer = new List<Type>();

			foreach (List<Type> list in types.Values)
			{
				if (list.Count == 1)
				{
					continue;
				}

				buffer.AddRange(list.Distinct());

				list.Clear();
				list.AddRange(buffer);

				buffer.Clear();
			}

			buffer.TrimExcess();
		}

		public void Sort()
		{
			Sort(m_Sensitive);
			Sort(m_Insensitive);
		}

		private static void Sort(Dictionary<string, List<Type>> types)
		{
			foreach (List<Type> list in types.Values)
			{
				list.Sort(InternalSort);
			}
		}

		private static int InternalSort(Type l, Type r)
		{
			if (l == r)
			{
				return 0;
			}

			if (l != null && r == null)
			{
				return -1;
			}

			if (l == null && r != null)
			{
				return 1;
			}

			bool a = IsEntity(l);
			bool b = IsEntity(r);

			if (a && b)
			{
				a = IsConstructable(l, out AccessLevel x);
				b = IsConstructable(r, out AccessLevel y);

				if (a && !b)
				{
					return -1;
				}

				if (!a && b)
				{
					return 1;
				}

				return x > y ? -1 : x < y ? 1 : 0;
			}

			return a ? -1 : b ? 1 : 0;
		}

		private static bool IsEntity(Type type)
		{
			return type.GetInterface("IEntity") != null;
		}

		private static bool IsConstructable(Type type, out AccessLevel access)
		{
			foreach (ConstructorInfo ctor in type.GetConstructors().OrderBy(o => o.GetParameters().Length))
			{
				ConstructableAttribute attr = ctor.GetCustomAttribute<ConstructableAttribute>(false);

				if (attr != null)
				{
					access = attr.AccessLevel;
					return true;
				}
			}

			access = 0;
			return false;
		}

		public void Add(string key, IEnumerable<Type> types)
		{
			if (!string.IsNullOrWhiteSpace(key) && types != null)
			{
				Add(key, types.ToArray());
			}
		}

		public void Add(string key, params Type[] types)
		{
			if (string.IsNullOrWhiteSpace(key) || types == null || types.Length == 0)
			{
				return;
			}

			if (!m_Sensitive.TryGetValue(key, out List<Type> sensitive) || sensitive == null)
			{
				m_Sensitive[key] = new List<Type>(types);
			}
			else if (types.Length == 1)
			{
				sensitive.Add(types[0]);
			}
			else
			{
				sensitive.AddRange(types);
			}

			if (!m_Insensitive.TryGetValue(key, out List<Type> insensitive) || insensitive == null)
			{
				m_Insensitive[key] = new List<Type>(types);
			}
			else if (types.Length == 1)
			{
				insensitive.Add(types[0]);
			}
			else
			{
				insensitive.AddRange(types);
			}
		}

		public IEnumerable<Type> Get(string key, bool ignoreCase)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				return Type.EmptyTypes;
			}

			List<Type> t;

			if (ignoreCase)
			{
				m_Insensitive.TryGetValue(key, out t);
			}
			else
			{
				m_Sensitive.TryGetValue(key, out t);
			}

			if (t == null)
			{
				return Type.EmptyTypes;
			}

			return t.AsEnumerable();
		}

		public TypeTable(int capacity)
		{
			m_Sensitive = new Dictionary<string, List<Type>>(capacity);
			m_Insensitive = new Dictionary<string, List<Type>>(capacity, StringComparer.OrdinalIgnoreCase);
		}
	}
}
