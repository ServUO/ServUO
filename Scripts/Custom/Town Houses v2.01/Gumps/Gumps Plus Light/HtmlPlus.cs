using System;
using Server;
using Server.Gumps;

namespace Knives.TownHouses
{
	public class HtmlPlus : GumpHtml
	{
		private bool c_Override;

		public bool Override{ get{ return c_Override; } set{ c_Override = value; } }

		public HtmlPlus( int x, int y, int width, int height, string text, bool back, bool scroll ) : base( x, y, width, height, text, back, scroll )
		{
			c_Override = true;
		}

		public HtmlPlus( int x, int y, int width, int height, string text, bool back, bool scroll, bool over ) : base( x, y, width, height, text, back, scroll )
		{
			c_Override = over;
		}
	}
}