using System;
using Server;
using Server.Gumps;

namespace Knives.TownHouses
{
	public class InfoGump : GumpPlusLight
	{
		private int c_Width, c_Height;
		private string c_Text;
		private bool c_Scroll;

		public InfoGump( Mobile m, int width, int height, string text, bool scroll ) : base( m, 100, 100 )
		{
			c_Width = width;
			c_Height = height;
			c_Text= text;
			c_Scroll = scroll;

			NewGump();
		}

		protected override void BuildGump()
		{
			AddBackground( 0, 0, c_Width, c_Height, 0x13BE );

			AddHtml( 20, 20, c_Width-40, c_Height-40, HTML.White + c_Text, false, c_Scroll );
		}
	}
}