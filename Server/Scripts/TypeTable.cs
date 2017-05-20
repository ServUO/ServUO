// **********
// ServUO - TypeTable.cs
// **********

using System;
using System.Collections.Generic;

namespace Server
{
    public class TypeTable
    {
        private readonly Dictionary<string, Type> m_Sensitive;
        private readonly Dictionary<string, Type> m_Insensitive;

        public void Add(string key, Type type)
        {
            m_Sensitive[key] = type;
            m_Insensitive[key] = type;
        }

        public Type Get(string key, bool ignoreCase)
        {
            Type t = null;

            if (ignoreCase)
            {
                m_Insensitive.TryGetValue(key, out t);
            }
            else
            {
                m_Sensitive.TryGetValue(key, out t);
            }

            return t;
        }

        public TypeTable(int capacity)
        {
            m_Sensitive = new Dictionary<string, Type>(capacity);
            m_Insensitive = new Dictionary<string, Type>(capacity, StringComparer.OrdinalIgnoreCase);
        }
    }
}
