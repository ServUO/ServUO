using System;
using System.Collections;

namespace Server.Commands.Generic
{
    public sealed class LimitExtension : BaseExtension
    {
        public static ExtensionInfo ExtInfo = new ExtensionInfo(80, "Limit", 1, delegate() { return new LimitExtension(); });
        private int m_Limit;
        public LimitExtension()
        {
        }

        public override ExtensionInfo Info
        {
            get
            {
                return ExtInfo;
            }
        }
        public int Limit
        {
            get
            {
                return this.m_Limit;
            }
        }
        public static void Initialize()
        {
            ExtensionInfo.Register(ExtInfo);
        }

        public override void Parse(Mobile from, string[] arguments, int offset, int size)
        {
            this.m_Limit = Utility.ToInt32(arguments[offset]);

            if (this.m_Limit < 0)
                throw new Exception("Limit cannot be less than zero.");
        }

        public override void Filter(ArrayList list)
        {
            if (list.Count > this.m_Limit)
                list.RemoveRange(this.m_Limit, list.Count - this.m_Limit);
        }
    }
}