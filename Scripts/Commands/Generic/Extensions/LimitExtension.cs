using System;
using System.Collections;

namespace 
    Server.Commands.Generic
{
    public sealed class LimitExtension : BaseExtension
    {
        public static ExtensionInfo ExtInfo = new ExtensionInfo(80, "Limit", 1, delegate { return new LimitExtension(); });
        private int m_Limit;

        public override ExtensionInfo Info => ExtInfo;
        public int Limit => m_Limit;
        public static void Initialize()
        {
            ExtensionInfo.Register(ExtInfo);
        }

        public override void Parse(Mobile from, string[] arguments, int offset, int size)
        {
            m_Limit = Utility.ToInt32(arguments[offset]);

            if (m_Limit < 0)
                throw new Exception("Limit cannot be less than zero.");
        }

        public override void Filter(ArrayList list)
        {
            if (list.Count > m_Limit)
                list.RemoveRange(m_Limit, list.Count - m_Limit);
        }
    }
}
