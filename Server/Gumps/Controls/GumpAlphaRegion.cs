#region Header
// **********
// ServUO - GumpAlphaRegion.cs
// **********
#endregion

#region References
using System;

using Server.Network;
#endregion

namespace Server.Gumps
{
	public class GumpAlphaRegion : GumpEntry
	{
		private static readonly byte[] m_LayoutName = Gump.StringToBuffer("checkertrans");
		private int m_X, m_Y;
		private int m_Width, m_Height;

		public GumpAlphaRegion(int x, int y, int width, int height)
		{
			m_X = x;
			m_Y = y;
			m_Width = width;
			m_Height = height;
		}

		public override int X { get { return m_X; } set { Delta(ref m_X, value); } }
		public override int Y { get { return m_Y; } set { Delta(ref m_Y, value); } }
		public int Width { get { return m_Width; } set { Delta(ref m_Width, value); } }
		public int Height { get { return m_Height; } set { Delta(ref m_Height, value); } }

		public override string Compile()
		{
			return String.Format("{{ checkertrans {0} {1} {2} {3} }}", m_X, m_Y, m_Width, m_Height);
		}

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(m_LayoutName);
			disp.AppendLayout(m_X);
			disp.AppendLayout(m_Y);
			disp.AppendLayout(m_Width);
			disp.AppendLayout(m_Height);
		}
	}
}