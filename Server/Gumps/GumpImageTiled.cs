using Server.Network;

namespace Server.Gumps
{
	public class GumpImageTiled : GumpEntry
	{
		private int m_X, m_Y;
		private int m_Width, m_Height;
		private int m_GumpID;

		public GumpImageTiled(int x, int y, int width, int height, int gumpID)
		{
			m_X = x;
			m_Y = y;
			m_Width = width;
			m_Height = height;
			m_GumpID = gumpID;
		}

		public int X
		{
			get => m_X;
			set => Delta(ref m_X, value);
		}

		public int Y
		{
			get => m_Y;
			set => Delta(ref m_Y, value);
		}

		public int Width
		{
			get => m_Width;
			set => Delta(ref m_Width, value);
		}

		public int Height
		{
			get => m_Height;
			set => Delta(ref m_Height, value);
		}

		public int GumpID
		{
			get => m_GumpID;
			set => Delta(ref m_GumpID, value);
		}

		public override string Compile()
		{
			return $"{{ gumppictiled {m_X} {m_Y} {m_Width} {m_Height} {m_GumpID} }}";
		}

		private static readonly byte[] m_LayoutName = Gump.StringToBuffer("gumppictiled");

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(m_LayoutName);
			disp.AppendLayout(m_X);
			disp.AppendLayout(m_Y);
			disp.AppendLayout(m_Width);
			disp.AppendLayout(m_Height);
			disp.AppendLayout(m_GumpID);
		}
	}
}