using System;

using Server;
using Server.Gumps;

namespace Arya.Chess
{
	/// <summary>
	/// Main game gump
	/// </summary>
	public class GameGump : Gump
	{
		private const int LabelHue = 0x480;
		private const int GreenHue = 0x40;

		private ChessGame m_Game;
		private bool m_Move;
		private bool m_Moving;
		private string m_Message;
		private ChessColor m_Color;

		public GameGump( Mobile m, ChessGame game, ChessColor color, string message, bool move, bool moving ): base( 60, 25 )
		{
			m.CloseGump( typeof( GameGump ) );

			m_Game = game;
			m_Message = message;
			m_Color = color;
			m_Move = move;
			m_Moving = moving;

			if ( move && ! moving )
				m_Game.SendMoveTarget( m );

			MakeGump();
		}

		private void MakeGump()
		{
			this.Closable=false;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			this.AddPage(0);
			this.AddBackground(0, 0, 555, 50, 9250);
			this.AddImageTiled(0, 0, 555, 50, 9304);
			this.AddImageTiled(1, 1, 553, 48, 9274);
			this.AddAlphaRegion(1, 1, 553, 48);

			if ( m_Color == ChessColor.White )
			{
				this.AddImage(5, 5, 2331);
				this.AddLabel(30, 5, LabelHue, @"You are WHITE");
			}
			else
			{
				this.AddImage(5, 5, 2338);
				this.AddLabel(30, 5, LabelHue, @"You are BLACK");
			}

			string msg = null;
			
			if ( m_Moving )
				msg = "Making your move";
			else if ( m_Move )
				msg = "Make your move";
			else
				msg = "Waiting for opponent to move";

			this.AddLabel(165, 5, LabelHue, msg);

			// B1 : Make move
			if ( m_Move )
				this.AddButton(145, 7, 5601, 5605, 1, GumpButtonType.Reply, 0);

			// B2 : Chess Help
			this.AddButton(365, 7, 5601, 5605, 2, GumpButtonType.Reply, 0);
			this.AddLabel(385, 5, LabelHue, @"Chess Help");

			// B3 : End Game
			this.AddButton(460, 7, 5601, 5605, 3, GumpButtonType.Reply, 0);
			this.AddLabel(480, 5, LabelHue, @"End Game");

			if ( m_Message != null )
				this.AddLabel(5, 25, GreenHue, m_Message );
		}

		public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
		{
			switch ( info.ButtonID )
			{
				case 1 : // Make move

					sender.Mobile.SendGump( new GameGump( sender.Mobile, m_Game, m_Color, m_Message, m_Move, m_Moving ) );
					break;

				case 2: // Chess Help

					sender.Mobile.SendGump( new ChessHelpGump( sender.Mobile ) );

					sender.Mobile.SendGump( this );
					break;

				case 3: // End game

					m_Game.Cleanup();
					break;
			}
		}
	}
}