// **********
// ServUO - TypeCache.cs
// **********

using System;
using System.Reflection;

namespace Server
{
    public class TypeCache
    {
        private readonly Type[] m_Types;
        private readonly TypeTable m_Names;
        private readonly TypeTable m_FullNames;

        public Type[] Types { get { return m_Types; } }
        public TypeTable Names { get { return m_Names; } }
        public TypeTable FullNames { get { return m_FullNames; } }

        public Type GetTypeByName(string name, bool ignoreCase)
        {
            return m_Names.Get(name, ignoreCase);
        }

        public Type GetTypeByFullName(string fullName, bool ignoreCase)
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

            Type typeofTypeAliasAttribute = typeof(TypeAliasAttribute);

            for (int i = 0; i < m_Types.Length; ++i)
            {
                Type type = m_Types[i];

                m_Names.Add(type.Name, type);
                m_FullNames.Add(type.FullName, type);

                if (type.IsDefined(typeofTypeAliasAttribute, false))
                {
                    var attrs = type.GetCustomAttributes(typeofTypeAliasAttribute, false);

                    if (attrs != null && attrs.Length > 0)
                    {
                        TypeAliasAttribute attr = attrs[0] as TypeAliasAttribute;

                        if (attr != null)
                        {
                            for (int j = 0; j < attr.Aliases.Length; ++j)
                            {
                                m_FullNames.Add(attr.Aliases[j], type);
                            }
                        }
                    }
                }
            }
        }
    }
}
