using System;
using Server.Network;

namespace Server.Gumps
{
	public class GumpButtonTileArt : GumpEntry
	{
		private int m_X, m_Y;
		private int m_ID1, m_ID2;
		private GumpButtonType m_Type;
		private int m_Param;
		private int m_ButtonID;
		private int m_ItemID;
		private int m_Hue;
		private int m_Width;
		private int m_Height;

        private int m_LocalizedTooltip;

        public GumpButtonTileArt(int x, int y, int normalID, int pressedID, GumpButtonType type, int buttonID, int param, int itemID, int hue, int width, int height)
            : this(x, y, normalID, pressedID, type, buttonID, param, itemID, hue, width, height, -1)
        {
        }

		public GumpButtonTileArt( int x, int y, int normalID, int pressedID, GumpButtonType type, int param, int buttonID, int itemid, int hue, int width, int height, int localizedTooltip )
		{
			m_X = x;
			m_Y = y;
			m_ID1 = normalID;
			m_ID2 = pressedID;
			m_Type = type;
			m_Param = param;
			m_ButtonID = buttonID;
			m_ItemID = itemid;
			m_Hue = hue;
			m_Width = width;
			m_Height = height;

            m_LocalizedTooltip = localizedTooltip;
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

		public int NormalID
		{
			get
			{
				return m_ID1;
			}
			set
			{
				Delta( ref m_ID1, value );
			}
		}

		public int PressedID
		{
			get
			{
				return m_ID2;
			}
			set
			{
				Delta( ref m_ID2, value );
			}
		}

		public GumpButtonType Type
		{
			get
			{
				return m_Type;
			}
			set
			{
				if ( m_Type != value )
					m_Type = value;
			}
		}

		public int Param
		{
			get
			{
				return m_Param;
			}
			set
			{
				Delta( ref m_Param, value );
			}
		}

		public int ButtonID
		{
			get
			{
				return m_ButtonID;
			}
			set
			{
				Delta( ref m_ButtonID, value );
			}
		}

		public int ItemID
		{
			get
			{
				return m_ItemID;
			}
			set
			{
				Delta( ref m_ItemID, value );
			}
		}

		public int Hue
		{
			get
			{
				return m_Hue;
			}
			set
			{
				Delta( ref m_Hue, value );
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

        public int LocalizedTooltip
        {
            get
            {
                return m_LocalizedTooltip;
            }
            set
            {
                m_LocalizedTooltip = value;
            }
        }


        public override string Compile()
        {
            if (m_LocalizedTooltip > 0)
                return String.Format("{{ buttontileart {0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} }}{{ tooltip {11} }}", m_X, m_Y, m_ID1, m_ID2, (int)m_Type, m_Param, m_ButtonID, m_ItemID, m_Hue, m_Width, m_Height, m_LocalizedTooltip);
            else
                return String.Format("{{ buttontileart {0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10} }}", m_X, m_Y, m_ID1, m_ID2, (int)m_Type, m_Param, m_ButtonID, m_ItemID, m_Hue, m_Width, m_Height);
        }

		private static byte[] m_LayoutName = Gump.StringToBuffer( "buttontileart" );
        private static byte[] m_LayoutTooltip = Gump.StringToBuffer(" }{ tooltip");

        public override void AppendTo(IGumpWriter disp)
        {
            disp.AppendLayout(m_LayoutName);
            disp.AppendLayout(m_X);
            disp.AppendLayout(m_Y);
            disp.AppendLayout(m_ID1);
            disp.AppendLayout(m_ID2);
            disp.AppendLayout((int)m_Type);
            disp.AppendLayout(m_Param);
            disp.AppendLayout(m_ButtonID);
            disp.AppendLayout(m_ItemID);
            disp.AppendLayout(m_Hue);
            disp.AppendLayout(m_Width);
            disp.AppendLayout(m_Height);

            if (m_LocalizedTooltip > 0)
            {
                disp.AppendLayout(m_LayoutTooltip);
                disp.AppendLayout(m_LocalizedTooltip);
            }

        }
	}
}