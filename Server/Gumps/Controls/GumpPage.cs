#region Header
// **********
// ServUO - GumpPage.cs
// **********
#endregion

#region References
using System;

using Server.Network;
#endregion

namespace Server.Gumps
{
	public class GumpPage : GumpEntry
	{
		private static readonly byte[] m_LayoutName = Gump.StringToBuffer("page");
		private int m_Page;

		public GumpPage(int page)
		{
			m_Page = page;
		}

		public int Page { get { return m_Page; } set { Delta(ref m_Page, value); } }

		public override string Compile()
		{
			return String.Format("{{ page {0} }}", m_Page);
		}

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(m_LayoutName);
			disp.AppendLayout(m_Page);
		}
	}
}