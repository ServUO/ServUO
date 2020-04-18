using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Commands.Generic
{
    public sealed class DistinctExtension : BaseExtension
    {
        public static ExtensionInfo ExtInfo = new ExtensionInfo(30, "Distinct", -1, delegate () { return new DistinctExtension(); });
        private readonly List<Property> m_Properties;
        private IComparer m_Comparer;
        public DistinctExtension()
        {
            m_Properties = new List<Property>();
        }

        public override ExtensionInfo Info => ExtInfo;
        public static void Initialize()
        {
            ExtensionInfo.Register(ExtInfo);
        }

        public override void Optimize(Mobile from, Type baseType, ref AssemblyEmitter assembly)
        {
            if (baseType == null)
                throw new Exception("Distinct extension may only be used in combination with an object conditional.");

            foreach (Property prop in m_Properties)
            {
                prop.BindTo(baseType, PropertyAccess.Read);
                prop.CheckAccess(from);
            }

            if (assembly == null)
                assembly = new AssemblyEmitter("__dynamic", false);

            m_Comparer = DistinctCompiler.Compile(assembly, baseType, m_Properties.ToArray());
        }

        public override void Parse(Mobile from, string[] arguments, int offset, int size)
        {
            if (size < 1)
                throw new Exception("Invalid distinction syntax.");

            int end = offset + size;

            while (offset < end)
            {
                string binding = arguments[offset++];

                m_Properties.Add(new Property(binding));
            }
        }

        public override void Filter(ArrayList list)
        {
            if (m_Comparer == null)
                throw new InvalidOperationException("The extension must first be optimized.");

            ArrayList copy = new ArrayList(list);

            copy.Sort(m_Comparer);

            list.Clear();

            object last = null;

            for (int i = 0; i < copy.Count; ++i)
            {
                object obj = copy[i];

                if (last == null || m_Comparer.Compare(obj, last) != 0)
                {
                    list.Add(obj);
                    last = obj;
                }
            }
        }
    }
}