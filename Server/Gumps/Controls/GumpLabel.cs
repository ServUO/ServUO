#region Header
// **********
// ServUO - GumpLabel.cs
// **********
#endregion

#region References
using System;

using Server.Network;
#endregion

namespace Server.Gumps
{
	public class GumpLabel : GumpEntry
	{
		private static readonly byte[] m_LayoutName = Gump.StringToBuffer("text");
		private int m_X, m_Y;
		private int m_Hue;
		private string m_Text;

		public GumpLabel(int x, int y, int hue, string text)
		{
			m_X = x;
			m_Y = y;
			m_Hue = hue;
			m_Text = text;
		}

		public override int X { get { return m_X; } set { Delta(ref m_X, value); } }
		public override int Y { get { return m_Y; } set { Delta(ref m_Y, value); } }
		public int Hue { get { return m_Hue; } set { Delta(ref m_Hue, value); } }
		public string Text { get { return m_Text; } set { Delta(ref m_Text, value); } }

		public override string Compile()
		{
			return String.Format("{{ text {0} {1} {2} {3} }}", m_X, m_Y, m_Hue, Container.RootParent.Intern(m_Text));
		}

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(m_LayoutName);
			disp.AppendLayout(m_X);
			disp.AppendLayout(m_Y);
			disp.AppendLayout(m_Hue);
			disp.AppendLayout(Container.RootParent.Intern(m_Text));
		}
	}
}