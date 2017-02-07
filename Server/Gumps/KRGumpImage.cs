using System;
using Server.Network;

namespace Server.Gumps
{
	public class KRGumpImage : GumpEntry
	{
		private int m_X, m_Y;
		private int m_GumpID;

		public KRGumpImage( int x, int y, int gumpID )
		{
			m_X = x;
			m_Y = y;
			m_GumpID = gumpID;
		}

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

		public int GumpID
		{
			get
			{
				return m_GumpID;
			}
			set
			{
				Delta( ref m_GumpID, value );
			}
		}


		public override string Compile()
		{
			return String.Format( "{{ kr_gumppic {0} {1} {2} }}", m_X, m_Y, m_GumpID );
		}

		private static byte[] m_LayoutName = Gump.StringToBuffer( "kr_gumppic" );

		public override void AppendTo( IGumpWriter disp )
		{
			disp.AppendLayout( m_LayoutName );
			disp.AppendLayout( m_X );
			disp.AppendLayout( m_Y );
			disp.AppendLayout( m_GumpID );
		}
	}
}