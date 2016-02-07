using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Commands.Generic
{
    public sealed class DistinctExtension : BaseExtension
    {
        public static ExtensionInfo ExtInfo = new ExtensionInfo(30, "Distinct", -1, delegate() { return new DistinctExtension(); });
        private readonly List<Property> m_Properties;
        private IComparer m_Comparer;
        public DistinctExtension()
        {
            this.m_Properties = new List<Property>();
        }

        public override ExtensionInfo Info
        {
            get
            {
                return ExtInfo;
            }
        }
        public static void Initialize()
        {
            ExtensionInfo.Register(ExtInfo);
        }

        public override void Optimize(Mobile from, Type baseType, ref AssemblyEmitter assembly)
        {
            if (baseType == null)
                throw new Exception("Distinct extension may only be used in combination with an object conditional.");

            foreach (Property prop in this.m_Properties)
            {
                prop.BindTo(baseType, PropertyAccess.Read);
                prop.CheckAccess(from);
            }

            if (assembly == null)
                assembly = new AssemblyEmitter("__dynamic", false);

            this.m_Comparer = DistinctCompiler.Compile(assembly, baseType, this.m_Properties.ToArray());
        }

        public override void Parse(Mobile from, string[] arguments, int offset, int size)
        {
            if (size < 1)
                throw new Exception("Invalid distinction syntax.");

            int end = offset + size;

            while (offset < end)
            {
                string binding = arguments[offset++];

                this.m_Properties.Add(new Property(binding));
            }
        }

        public override void Filter(ArrayList list)
        {
            if (this.m_Comparer == null)
                throw new InvalidOperationException("The extension must first be optimized.");

            ArrayList copy = new ArrayList(list);

            copy.Sort(this.m_Comparer);

            list.Clear();

            object last = null;

            for (int i = 0; i < copy.Count; ++i)
            {
                object obj = copy[i];

                if (last == null || this.m_Comparer.Compare(obj, last) != 0)
                {
                    list.Add(obj);
                    last = obj;
                }
            }
        }
    }
}