using System;

using Server;
using Server.Gumps;

namespace Arya.Chess
{
	/// <summary>
	/// Summary description for ScoreGump.
	/// </summary>
	public class ScoreGump : Gump
	{
		private ChessGame m_Game;
		private const int LabelHue = 0x480;
		private const int GreenHue = 0x40;

		public ScoreGump( Mobile m, ChessGame game, int[] white, int[] black, int totwhite, int totblack ) : base( 0, 25 )
		{
			m.CloseGump( typeof( ScoreGump ) );
			m_Game = game;

			MakeGump( white, black, totwhite, totblack );
		}

		private void MakeGump( int[] white, int[] black, int whitescore, int blackscore )
		{
			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);
			this.AddBackground(0, 0, 60, 345, 9350);
			this.AddAlphaRegion(0, 0, 60, 345);
			this.AddImage(5, 5, 2336);
			this.AddImage(30, 6, 2343);
			this.AddImage(5, 50, 2335);
			this.AddImage(30, 50, 2342);
			this.AddImage(5, 105, 2332);
			this.AddImage(30, 105, 2339);
			this.AddImage(5, 160, 2333);
			this.AddImage(30, 160, 2340);
			this.AddImage(5, 220, 2337);
			this.AddImage(30, 220, 2344);
			this.AddImageTiled(0, 285, 60, 1, 5124);
			this.AddImage(10, 305, 2331);
			this.AddImage(10, 325, 2338);

			this.AddLabel(11, 33, LabelHue, white[0].ToString() );
			this.AddLabel(5, 285, GreenHue, @"Score");
			this.AddLabel(36, 33, LabelHue, black[0].ToString() );
			this.AddLabel(11, 87, LabelHue, white[1].ToString() );
			this.AddLabel(36, 87, LabelHue, black[1].ToString() );
			this.AddLabel(11, 142, LabelHue, white[2].ToString() );
			this.AddLabel(36, 142, LabelHue, black[2].ToString() );
			this.AddLabel(11, 200, LabelHue, white[3].ToString() );
			this.AddLabel(36, 200, LabelHue, black[3].ToString() );
			this.AddLabel(11, 260, LabelHue, white[4].ToString() );
			this.AddLabel(36, 260, LabelHue, black[4].ToString() );
			this.AddLabel(30, 302, LabelHue, whitescore.ToString() );
			this.AddLabel(30, 322, LabelHue, blackscore.ToString() );
		}
	}
}
