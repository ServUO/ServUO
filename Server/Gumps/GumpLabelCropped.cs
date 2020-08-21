/***************************************************************************
 *                            GumpLabelCropped.cs
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
	public class GumpLabelCropped : GumpEntry
	{
		private int m_X, m_Y;
		private int m_Width, m_Height;
		private int m_Hue;
		private string m_Text;
		private readonly int m_TextID;

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

		public int Hue
		{
			get => m_Hue;
			set => Delta(ref m_Hue, value);
		}

		public string Text
		{
			get => m_Text;
			set => Delta(ref m_Text, value);
		}

		public GumpLabelCropped(int x, int y, int width, int height, int hue, string text)
		{
			m_X = x;
			m_Y = y;
			m_Width = width;
			m_Height = height;
			m_Hue = hue;
			m_Text = text;
		}

		public GumpLabelCropped(int x, int y, int width, int height, int hue, int textid)
		{
			m_X = x;
			m_Y = y;
			m_Width = width;
			m_Height = height;
			m_Hue = hue;
			m_TextID = textid;
		}

		public override string Compile()
		{
			return string.Format("{{ croppedtext {0} {1} {2} {3} {4} {5} }}", m_X, m_Y, m_Width, m_Height, m_Hue, m_Text == null ? m_TextID : Parent.Intern(m_Text));
		}

		private static readonly byte[] m_LayoutName = Gump.StringToBuffer("croppedtext");

		public override void AppendTo(IGumpWriter disp)
		{
			disp.AppendLayout(m_LayoutName);
			disp.AppendLayout(m_X);
			disp.AppendLayout(m_Y);
			disp.AppendLayout(m_Width);
			disp.AppendLayout(m_Height);
			disp.AppendLayout(m_Hue);
			disp.AppendLayout(m_Text == null ? m_TextID : Parent.Intern(m_Text));
		}
	}
}
