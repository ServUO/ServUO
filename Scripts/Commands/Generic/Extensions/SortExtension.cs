using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Commands.Generic
{
    public sealed class SortExtension : BaseExtension
    {
        public static ExtensionInfo ExtInfo = new ExtensionInfo(40, "Order", -1, delegate () { return new SortExtension(); });
        private readonly List<OrderInfo> m_Orders;
        private IComparer m_Comparer;
        public SortExtension()
        {
            m_Orders = new List<OrderInfo>();
        }

        public override ExtensionInfo Info => ExtInfo;
        public static void Initialize()
        {
            ExtensionInfo.Register(ExtInfo);
        }

        public override void Optimize(Mobile from, Type baseType, ref AssemblyEmitter assembly)
        {
            if (baseType == null)
                throw new Exception("The ordering extension may only be used in combination with an object conditional.");

            foreach (OrderInfo order in m_Orders)
            {
                order.Property.BindTo(baseType, PropertyAccess.Read);
                order.Property.CheckAccess(from);
            }

            if (assembly == null)
                assembly = new AssemblyEmitter("__dynamic", false);

            m_Comparer = SortCompiler.Compile(assembly, baseType, m_Orders.ToArray());
        }

        public override void Parse(Mobile from, string[] arguments, int offset, int size)
        {
            if (size < 1)
                throw new Exception("Invalid ordering syntax.");

            if (Insensitive.Equals(arguments[offset], "by"))
            {
                ++offset;
                --size;

                if (size < 1)
                    throw new Exception("Invalid ordering syntax.");
            }

            int end = offset + size;

            while (offset < end)
            {
                string binding = arguments[offset++];

                bool isAscending = true;

                if (offset < end)
                {
                    string next = arguments[offset];

                    switch (next.ToLower())
                    {
                        case "+":
                        case "up":
                        case "asc":
                        case "ascending":
                            isAscending = true;
                            ++offset;
                            break;
                        case "-":
                        case "down":
                        case "desc":
                        case "descending":
                            isAscending = false;
                            ++offset;
                            break;
                    }
                }

                Property property = new Property(binding);

                m_Orders.Add(new OrderInfo(property, isAscending));
            }
        }

        public override void Filter(ArrayList list)
        {
            if (m_Comparer == null)
                throw new InvalidOperationException("The extension must first be optimized.");

            list.Sort(m_Comparer);
        }
    }
}