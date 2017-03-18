/***************************************************************************
 *                              GumpTextEntry.cs
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
	public class GumpTextEntry : GumpEntry
	{
		private int m_X, m_Y;
		private int m_Width, m_Height;
		private int m_Hue;
		private int m_EntryID;
		private string m_InitialText;
        private int m_InitialTextID = -1;

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

		public int EntryID
		{
			get
			{
				return m_EntryID;
			}
			set
			{
				Delta( ref m_EntryID, value );
			}
		}

		public string InitialText
		{
			get
			{
				return m_InitialText;
			}
			set
			{
				Delta( ref m_InitialText, value );
			}
		}

		public GumpTextEntry( int x, int y, int width, int height, int hue, int entryID, string initialText )
		{
			m_X = x;
			m_Y = y;
			m_Width = width;
			m_Height = height;
			m_Hue = hue;
			m_EntryID = entryID;
			m_InitialText = initialText;
		}

        public GumpTextEntry(int x, int y, int width, int height, int hue, int entryID, int initialTextID)
        {
            m_X = x;
            m_Y = y;
            m_Width = width;
            m_Height = height;
            m_Hue = hue;
            m_EntryID = entryID;
            m_InitialTextID = initialTextID;
        }

        public override string Compile()
		{
			return String.Format( "{{ textentry {0} {1} {2} {3} {4} {5} {6} }}", m_X, m_Y, m_Width, m_Height, m_Hue, m_EntryID, m_InitialTextID == -1 ?  Parent.Intern(m_InitialText) : m_InitialTextID);
		}

		private static byte[] m_LayoutName = Gump.StringToBuffer( "textentry" );

		public override void AppendTo( IGumpWriter disp )
		{
			disp.AppendLayout( m_LayoutName );
			disp.AppendLayout( m_X );
			disp.AppendLayout( m_Y );
			disp.AppendLayout( m_Width );
			disp.AppendLayout( m_Height );
			disp.AppendLayout( m_Hue );
			disp.AppendLayout( m_EntryID );
            disp.AppendLayout(m_InitialTextID == -1 ? Parent.Intern(m_InitialText) : m_InitialTextID);

            disp.TextEntries++;
		}
	}
}