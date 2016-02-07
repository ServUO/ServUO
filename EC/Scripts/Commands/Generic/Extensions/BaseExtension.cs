using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Commands.Generic
{
    public delegate BaseExtension ExtensionConstructor();

    public sealed class ExtensionInfo
    {
        private static readonly Dictionary<string, ExtensionInfo> m_Table = new Dictionary<string, ExtensionInfo>(StringComparer.InvariantCultureIgnoreCase);
        private readonly int m_Order;
        private readonly string m_Name;
        private readonly int m_Size;
        private readonly ExtensionConstructor m_Constructor;
        public ExtensionInfo(int order, string name, int size, ExtensionConstructor constructor)
        {
            this.m_Name = name;
            this.m_Size = size;

            this.m_Order = order;

            this.m_Constructor = constructor;
        }

        public static Dictionary<string, ExtensionInfo> Table
        {
            get
            {
                return m_Table;
            }
        }
        public int Order
        {
            get
            {
                return this.m_Order;
            }
        }
        public string Name
        {
            get
            {
                return this.m_Name;
            }
        }
        public int Size
        {
            get
            {
                return this.m_Size;
            }
        }
        public bool IsFixedSize
        {
            get
            {
                return (this.m_Size >= 0);
            }
        }
        public ExtensionConstructor Constructor
        {
            get
            {
                return this.m_Constructor;
            }
        }
        public static void Register(ExtensionInfo ext)
        {
            m_Table[ext.m_Name] = ext;
        }
    }

    public sealed class Extensions : List<BaseExtension>
    {
        public Extensions()
        {
        }

        public static Extensions Parse(Mobile from, ref string[] args)
        {
            Extensions parsed = new Extensions();

            int size = args.Length;

            Type baseType = null;

            for (int i = args.Length - 1; i >= 0; --i)
            {
                ExtensionInfo extInfo = null;

                if (!ExtensionInfo.Table.TryGetValue(args[i], out extInfo))
                    continue;

                if (extInfo.IsFixedSize && i != (size - extInfo.Size - 1))
                    throw new Exception("Invalid extended argument count.");

                BaseExtension ext = extInfo.Constructor();

                ext.Parse(from, args, i + 1, size - i - 1);

                if (ext is WhereExtension)
                    baseType = (ext as WhereExtension).Conditional.Type;

                parsed.Add(ext);

                size = i;
            }

            parsed.Sort(delegate(BaseExtension a, BaseExtension b)
            {
                return (a.Order - b.Order);
            });

            AssemblyEmitter emitter = null;

            foreach (BaseExtension update in parsed)
                update.Optimize(from, baseType, ref emitter);

            if (size != args.Length)
            {
                string[] old = args;
                args = new string[size];

                for (int i = 0; i < args.Length; ++i)
                    args[i] = old[i];
            }

            return parsed;
        }

        public bool IsValid(object obj)
        {
            for (int i = 0; i < this.Count; ++i)
            {
                if (!this[i].IsValid(obj))
                    return false;
            }

            return true;
        }

        public void Filter(ArrayList list)
        {
            for (int i = 0; i < this.Count; ++i)
                this[i].Filter(list);
        }
    }

    public abstract class BaseExtension
    {
        public abstract ExtensionInfo Info { get; }
        public string Name
        {
            get
            {
                return this.Info.Name;
            }
        }
        public int Size
        {
            get
            {
                return this.Info.Size;
            }
        }
        public bool IsFixedSize
        {
            get
            {
                return this.Info.IsFixedSize;
            }
        }
        public int Order
        {
            get
            {
                return this.Info.Order;
            }
        }
        public virtual void Optimize(Mobile from, Type baseType, ref AssemblyEmitter assembly)
        {
        }

        public virtual void Parse(Mobile from, string[] arguments, int offset, int size)
        {
        }

        public virtual bool IsValid(object obj)
        {
            return true;
        }

        public virtual void Filter(ArrayList list)
        {
        }
    }
}