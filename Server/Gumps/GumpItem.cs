/***************************************************************************
 *                                GumpItem.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id$
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using Server.Network;

namespace Server.Gumps
{
	public class GumpItem : GumpEntry
	{
		private int m_X, m_Y;
		private int m_ItemID;
		private int m_Hue;

		public GumpItem( int x, int y, int itemID ) : this( x, y, itemID, 0 )
		{
		}

		public GumpItem( int x, int y, int itemID, int hue )
		{
			m_X = x;
			m_Y = y;
			m_ItemID = itemID;
			m_Hue = hue;
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

		public override string Compile()
		{
			if ( m_Hue == 0 )
				return String.Format( "{{ tilepic {0} {1} {2} }}", m_X, m_Y, m_ItemID );
			else
				return String.Format( "{{ tilepichue {0} {1} {2} {3} }}", m_X, m_Y, m_ItemID, m_Hue );
		}

		private static byte[] m_LayoutName = Gump.StringToBuffer( "tilepic" );
		private static byte[] m_LayoutNameHue = Gump.StringToBuffer( "tilepichue" );

		public override void AppendTo( IGumpWriter disp )
		{
			disp.AppendLayout( m_Hue == 0 ? m_LayoutName : m_LayoutNameHue );
			disp.AppendLayout( m_X );
			disp.AppendLayout( m_Y );
			disp.AppendLayout( m_ItemID );

			if ( m_Hue != 0 )
				disp.AppendLayout( m_Hue );
		}
	}
}