/***************************************************************************
 *                             GumpAlphaRegion.cs
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

using Server.Network;

namespace Server.Gumps
{
	public class GumpAlphaRegion : GumpEntry
	{
		private int m_X, m_Y;
		private int m_Width, m_Height;

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

		public GumpAlphaRegion(int x, int y, int width, int height)
		{
			m_X = x;
			m_Y = y;
			m_Width = width;
			m_Height = height;
		}

		public override string Compile()
		{
			return string.Format("{{ checkertrans {0} {1} {2} {3} }}", m_X, m_Y, m_Width, m_Height);
		}

		private static readonly byte[] m_LayoutName = Gump.StringToBuffer("checkertrans");

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