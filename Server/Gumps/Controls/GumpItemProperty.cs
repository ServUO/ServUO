#region Header
// **********
// ServUO - GumpItemProperty.cs
// **********
#endregion

#region References
using System;

using Server.Network;
#endregion

namespace Server.Gumps
{
	public class GumpItemProperty : GumpEntry
	{
		private int m_Serial;

		public GumpItemProperty(int serial)
		{
			m_Serial = serial;
		}

		public int Serial { get { return m_Serial; } set { Delta(ref m_Serial, value); } }

		public override string Compile()
		{
			return String.Format("{{ itemproperty {0} }}", m_Serial);
		}

		private static readonly byte[] m_LayoutName = Gump.StringToBuffer("itemproperty");

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(m_LayoutName);
			disp.AppendLayout(m_Serial);
		}
	}
}