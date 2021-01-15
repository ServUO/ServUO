using Server.Network;

namespace Server.Gumps
{
	public class GumpPage : GumpEntry
	{
		private int m_Page;

		public GumpPage(int page)
		{
			m_Page = page;
		}

		public int Page
		{
			get => m_Page;
			set => Delta(ref m_Page, value);
		}

		public override string Compile()
		{
			return $"{{ page {m_Page} }}";
		}

		private static readonly byte[] m_LayoutName = Gump.StringToBuffer("page");

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(m_LayoutName);
			disp.AppendLayout(m_Page);
		}
	}
}