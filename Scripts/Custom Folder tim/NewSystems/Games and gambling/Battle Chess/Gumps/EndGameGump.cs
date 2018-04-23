using System;

using Server;
using Server.Gumps;

namespace Arya.Chess
{
	public class EndGameGump : Gump
	{
		private const int LabelHue = 0x480;
		private const int GreenHue = 0x40;

		private ChessGame m_Game;
		private bool m_GameOver;

		public EndGameGump( Mobile m, ChessGame game, bool over, string details, int timeout ) : base( 200, 200 )
		{
			m.CloseGump( typeof( EndGameGump ) );
			m_Game = game;
			m_GameOver = over;

			this.Closable=false;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			this.AddPage(0);
			this.AddBackground(0, 0, 265, 160, 9250);
			this.AddImageTiled(0, 0, 265, 160, 9304);
			this.AddImageTiled(1, 1, 263, 158, 9274);
			this.AddAlphaRegion(1, 1, 263, 158);

			// Button 1 : End Game
			this.AddButton(10, 127, 5601, 5605, 1, GumpButtonType.Reply, 0);

			this.AddLabel(10, 10, LabelHue, string.Format( "This game is : {0}", over ? "Over" : "Pending" ) );
			this.AddLabel(10, 30, LabelHue, @"Details");
			this.AddLabel(30, 55, GreenHue, details);
			this.AddLabel(30, 125, LabelHue, @"End Game");

			if ( timeout > -1 )
			{
				this.AddLabel(10, 80, LabelHue, string.Format( "Wait {0} minutes before the game ends", timeout ) );
				this.AddLabel(10, 95, LabelHue, @"automatically. Do not close this gump.");
			}
		}

		public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
		{
			if ( info.ButtonID == 1 )
			{
				if ( m_GameOver )
				{
					// Reset the game
					m_Game.NotifyGameOver( sender.Mobile );
				}
				else
				{
					// Force end the game
					m_Game.Cleanup();
				}
			}
		}
	}
}