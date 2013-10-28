#region Header
// **********
// ServUO - GumpHtml.cs
// **********
#endregion

#region References
using System;

using Server.Network;
#endregion

namespace Server.Gumps
{
	public class GumpHtml : GumpEntry
	{
		private static readonly byte[] m_LayoutName = Gump.StringToBuffer("htmlgump");
		private int m_X, m_Y;
		private int m_Width, m_Height;
		private string m_Text;
		private bool m_Background, m_Scrollbar;

		public GumpHtml(int x, int y, int width, int height, string text, bool background, bool scrollbar)
		{
			m_X = x;
			m_Y = y;
			m_Width = width;
			m_Height = height;
			m_Text = text;
			m_Background = background;
			m_Scrollbar = scrollbar;
		}

		public override int X { get { return m_X; } set { Delta(ref m_X, value); } }
		public override int Y { get { return m_Y; } set { Delta(ref m_Y, value); } }
		public int Width { get { return m_Width; } set { Delta(ref m_Width, value); } }
		public int Height { get { return m_Height; } set { Delta(ref m_Height, value); } }
		public string Text { get { return m_Text; } set { Delta(ref m_Text, value); } }
		public bool Background { get { return m_Background; } set { Delta(ref m_Background, value); } }
		public bool Scrollbar { get { return m_Scrollbar; } set { Delta(ref m_Scrollbar, value); } }

		public override string Compile()
		{
			return String.Format(
				"{{ htmlgump {0} {1} {2} {3} {4} {5} {6} }}",
				m_X,
				m_Y,
				m_Width,
				m_Height,
				Container.RootParent.Intern(m_Text),
				m_Background ? 1 : 0,
				m_Scrollbar ? 1 : 0);
		}

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(m_LayoutName);
			disp.AppendLayout(m_X);
			disp.AppendLayout(m_Y);
			disp.AppendLayout(m_Width);
			disp.AppendLayout(m_Height);
			disp.AppendLayout(Container.RootParent.Intern(m_Text));
			disp.AppendLayout(m_Background);
			disp.AppendLayout(m_Scrollbar);
		}
	}
}