/***************************************************************************
 *                                GumpCheck.cs
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
	public class GumpCheck : GumpEntry
	{
		private int m_X, m_Y;
		private int m_ID1, m_ID2;
		private bool m_InitialState;
		private int m_SwitchID;

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

		public int InactiveID
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

		public int ActiveID
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

		public bool InitialState
		{
			get
			{
				return m_InitialState;
			}
			set
			{
				Delta( ref m_InitialState, value );
			}
		}

		public int SwitchID
		{
			get
			{
				return m_SwitchID;
			}
			set
			{
				Delta( ref m_SwitchID, value );
			}
		}

		public GumpCheck( int x, int y, int inactiveID, int activeID, bool initialState, int switchID )
		{
			m_X = x;
			m_Y = y;
			m_ID1 = inactiveID;
			m_ID2 = activeID;
			m_InitialState = initialState;
			m_SwitchID = switchID;
		}

		public override string Compile()
		{
			return String.Format( "{{ checkbox {0} {1} {2} {3} {4} {5} }}", m_X, m_Y, m_ID1, m_ID2, m_InitialState ? 1 : 0, m_SwitchID );
		}

		private static byte[] m_LayoutName = Gump.StringToBuffer( "checkbox" );

		public override void AppendTo( IGumpWriter disp )
		{
			disp.AppendLayout( m_LayoutName );
			disp.AppendLayout( m_X );
			disp.AppendLayout( m_Y );
			disp.AppendLayout( m_ID1 );
			disp.AppendLayout( m_ID2 );
			disp.AppendLayout( m_InitialState );
			disp.AppendLayout( m_SwitchID );

			disp.Switches++;
		}
	}
}