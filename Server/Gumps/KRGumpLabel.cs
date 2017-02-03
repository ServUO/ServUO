using System;
using Server.Network;

namespace Server.Gumps
{
	public class KRGumpLabel : GumpEntry
	{
		private int m_X, m_Y;
		private int m_Width, m_Height;
		private int m_Number;
		private bool m_Background, m_Scrollbar;

		public int X
		{
			get
			{
				return m_X;
			}
			set
			{
				Delta( ref m_X, value );
			}
		}

		public int Y
		{
			get
			{
				return m_Y;
			}
			set
			{
				Delta( ref m_Y, value );
			}
		}

		public int Width
		{
			get
			{
				return m_Width;
			}
			set
			{
				Delta( ref m_Width, value );
			}
		}

		public int Height
		{
			get
			{
				return m_Height;
			}
			set
			{
				Delta( ref m_Height, value );
			}
		}

		public int Number
		{
			get
			{
				return m_Number;
			}
			set
			{
				Delta( ref m_Number, value );
			}
		}

		public bool Background
		{
			get
			{
				return m_Background;
			}
			set
			{
				Delta( ref m_Background, value );
			}
		}

		public bool Scrollbar
		{
			get
			{
				return m_Scrollbar;
			}
			set
			{
				Delta( ref m_Scrollbar, value );
			}
		}

		public KRGumpLabel( int x, int y, int width, int height, int number, bool background, bool scrollbar )
		{
			m_X = x;
			m_Y = y;
			m_Width = width;
			m_Height = height;
			m_Number = number;
			m_Background = background;
			m_Scrollbar = scrollbar;
		}

		public override string Compile()
		{
			return String.Format( "{{ kr_text {0} {1} {2} {3} {4} {5} {6} }}", m_X, m_Y, m_Width, m_Height, m_Number, m_Background ? 1 : 0, m_Scrollbar ? 1 : 0 );
		}

		private static byte[] m_LayoutName = Gump.StringToBuffer( "kr_text" );

		public override void AppendTo( IGumpWriter disp )
		{
			disp.AppendLayout( m_LayoutName );
			disp.AppendLayout( m_X );
			disp.AppendLayout( m_Y );
			disp.AppendLayout( m_Width );
			disp.AppendLayout( m_Height );
			disp.AppendLayout( m_Number );
			disp.AppendLayout( m_Background );
			disp.AppendLayout( m_Scrollbar );
		}
	}
}