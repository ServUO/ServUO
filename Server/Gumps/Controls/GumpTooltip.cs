#region Header
// **********
// ServUO - GumpTooltip.cs
// **********
#endregion

#region References
using System;

using Server.Network;
#endregion

namespace Server.Gumps
{
	public class GumpTooltip : GumpEntry
	{
		private static readonly byte[] m_LayoutName = Gump.StringToBuffer("tooltip");
		private int m_Number;

		public GumpTooltip(int number)
		{
			m_Number = number;
		}

		public int Number { get { return m_Number; } set { Delta(ref m_Number, value); } }

		public override string Compile()
		{
			return String.Format("{{ tooltip {0} }}", m_Number);
		}

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(m_LayoutName);
			disp.AppendLayout(m_Number);
		}
	}
}