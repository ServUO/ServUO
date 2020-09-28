#region References
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

        public static int FindHashByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return 0;
            }

            int hash = 0;

            for (int i = 0; hash == 0 && i < Assemblies.Length; ++i)
            {
                hash = GetTypeCache(Assemblies[i]).GetTypeHashByName(name);
            }

            return hash != 0 ? hash : GetTypeCache(Core.Assembly).GetTypeHashByName(name);
        }

        public static int FindHashByFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return 0;
            }

            int hash = 0;

            for (int i = 0; hash == 0 && i < Assemblies.Length; ++i)
            {
                hash = GetTypeCache(Assemblies[i]).GetTypeHashByFullName(fullName);
            }

            return hash != 0 ? hash : GetTypeCache(Core.Assembly).GetTypeHashByFullName(fullName);
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

        public static Type FindTypeByNameHash(int hash)
        {
            Type type = null;

            for (int i = 0; type == null && i < Assemblies.Length; ++i)
            {
                type = GetTypeCache(Assemblies[i]).GetTypeByNameHash(hash);
            }

            return type ?? GetTypeCache(Core.Assembly).GetTypeByNameHash(hash);
        }

        public static IEnumerable<Type> FindTypesByNameHash(int hash)
        {
            for (int i = 0; i < Assemblies.Length; ++i)
            {
                foreach (Type t in GetTypeCache(Assemblies[i]).GetTypesByNameHash(hash))
                {
                    yield return t;
                }
            }

            foreach (Type t in GetTypeCache(Core.Assembly).GetTypesByNameHash(hash))
            {
                yield return t;
            }
        }

        public static Type FindTypeByFullNameHash(int hash)
        {
            Type type = null;

            for (int i = 0; type == null && i < Assemblies.Length; ++i)
            {
                type = GetTypeCache(Assemblies[i]).GetTypeByFullNameHash(hash);
            }

            return type ?? GetTypeCache(Core.Assembly).GetTypeByFullNameHash(hash);
        }

        public static IEnumerable<Type> FindTypesByFullNameHash(int hash)
        {
            for (int i = 0; i < Assemblies.Length; ++i)
            {
                foreach (Type t in GetTypeCache(Assemblies[i]).GetTypesByFullNameHash(hash))
                {
                    yield return t;
                }
            }

            foreach (Type t in GetTypeCache(Core.Assembly).GetTypesByFullNameHash(hash))
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

        public Type GetTypeByNameHash(int hash)
        {
            return GetTypesByNameHash(hash).FirstOrDefault(t => t != null);
        }

        public IEnumerable<Type> GetTypesByNameHash(int hash)
        {
            return m_Names.Get(hash);
        }

        public Type GetTypeByFullNameHash(int hash)
        {
            return GetTypesByFullNameHash(hash).FirstOrDefault(t => t != null);
        }

        public IEnumerable<Type> GetTypesByFullNameHash(int hash)
        {
            return m_FullNames.Get(hash);
        }

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

        public int GetTypeHashByName(string name)
        {
            return m_Names.GetHash(name);
        }

        public int GetTypeHashByFullName(string fullName)
        {
            return m_FullNames.GetHash(fullName);
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

			m_Names.Sort();
			m_FullNames.Sort();
		}
	}

	public class TypeTable
	{
        private readonly Dictionary<string, int> m_Hashes;
        private readonly Dictionary<int, HashSet<Type>> m_Hashed;
		private readonly Dictionary<string, HashSet<Type>> m_Sensitive;
		private readonly Dictionary<string, HashSet<Type>> m_Insensitive;

		public void Sort()
		{
            Sort(m_Hashed);
			Sort(m_Sensitive);
			Sort(m_Insensitive);
		}

		private static void Sort<T>(Dictionary<T, HashSet<Type>> types)
		{
            List<Type> sorter = new List<Type>();

			foreach (HashSet<Type> list in types.Values)
			{
                sorter.AddRange(list);
                sorter.Sort(InternalSort);

                list.Clear();
                list.UnionWith(sorter);

                sorter.Clear();
            }

            sorter.TrimExcess();
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

			if (!m_Sensitive.TryGetValue(key, out HashSet<Type> sensitive) || sensitive == null)
			{
				m_Sensitive[key] = new HashSet<Type>(types);
			}
			else if (types.Length == 1)
			{
				sensitive.Add(types[0]);
			}
			else
			{
				sensitive.UnionWith(types);
			}

			if (!m_Insensitive.TryGetValue(key, out HashSet<Type> insensitive) || insensitive == null)
			{
				m_Insensitive[key] = new HashSet<Type>(types);
			}
			else if (types.Length == 1)
			{
				insensitive.Add(types[0]);
			}
			else
			{
				insensitive.UnionWith(types);
            }

            int hash = GenerateHash(key);

            if (!m_Hashed.TryGetValue(hash, out HashSet<Type> hashed) || hashed == null)
            {
                m_Hashed[hash] = new HashSet<Type>(types);
            }
            else if (types.Length == 1)
            {
                hashed.Add(types[0]);
            }
            else
            {
                hashed.UnionWith(types);
            }
        }

		public IEnumerable<Type> Get(string key, bool ignoreCase)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				return Type.EmptyTypes;
			}

            HashSet<Type> t;

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

        public IEnumerable<Type> Get(int hash)
        {
            m_Hashed.TryGetValue(hash, out HashSet<Type> t);

            if (t == null)
            {
                return Type.EmptyTypes;
            }

            return t.AsEnumerable();
        }

        public int GetHash(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return 0;
            }

            m_Hashes.TryGetValue(key, out int hash);

            return hash;
        }

        private int GenerateHash(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return 0;
            }

            int hash = GetHash(key);

            if (hash != 0)
            {
                return hash;
            }

            hash = key.Length;

            unchecked
            {
                for (int i = 0; i < key.Length; i++)
                {
                    hash = (hash * 397) ^ Convert.ToInt32(key[i]);
                }
            }

            m_Hashes[key] = hash;

            return hash;
        }

        public TypeTable(int capacity)
		{
            m_Hashes = new Dictionary<string, int>();
            m_Hashed = new Dictionary<int, HashSet<Type>>(capacity);
			m_Sensitive = new Dictionary<string, HashSet<Type>>(capacity);
			m_Insensitive = new Dictionary<string, HashSet<Type>>(capacity, StringComparer.OrdinalIgnoreCase);
		}
	}
}
