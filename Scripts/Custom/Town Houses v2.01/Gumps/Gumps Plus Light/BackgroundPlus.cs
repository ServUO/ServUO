using System;
using Server;
using Server.Gumps;

namespace Knives.TownHouses
{
	public class BackgroundPlus : GumpBackground
	{
		private bool c_Override;

		public bool Override{ get{ return c_Override; } set{ c_Override = value; } }

		public BackgroundPlus( int x, int y, int width, int height, int back ) : base( x, y, width, height, back )
		{
			c_Override = true;
		}

		public BackgroundPlus( int x, int y, int width, int height, int back, bool over ) : base( x, y, width, height, back )
		{
			c_Override = over;
		}
	}
}