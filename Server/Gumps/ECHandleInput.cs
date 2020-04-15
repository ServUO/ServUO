using Server.Network;
using System;

namespace Server.Gumps
{
    public class ECHandleInput : GumpEntry
    {
        public ECHandleInput()
        {
        }

        public override string Compile()
        {
            return String.Format("{{ echandleinput }}");
        }

        private static readonly byte[] m_LayoutName = Gump.StringToBuffer("echandleinput");

        public override void AppendTo(IGumpWriter disp)
        {
            disp.AppendLayout(m_LayoutName);
        }
    }
}
