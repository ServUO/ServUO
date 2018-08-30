using System;
using System.Reflection;

using Server;
using Server.Gumps;

namespace Arya.Chess
{
	/// <summary>
	/// Summary description for ChessSetGump.
	/// </summary>
	public class ChessSetGump : Gump
	{
		private const int LabelHue = 0x480;
		private const int GreenHue = 0x40;
		private static string[] m_Sets;

		/// <summary>
		/// Gets the list of chess sets available
		/// </summary>
		private static string[] Sets
		{
			get
			{
				if ( m_Sets == null )
					m_Sets = Enum.GetNames( typeof( Arya.Chess.ChessSet ) );

				return m_Sets;
			}
		}

		private ChessGame m_Game;
		private Mobile m_User;
		private bool m_IsOwner;
		private bool m_AllowSpectators;
		private int m_Page = 0;

		public ChessSetGump( Mobile m, ChessGame game, bool isOwner, bool allowSpectators, int page ) : base( 200, 200 )
		{
			m_Game = game;
			m_User = m;
			m_IsOwner = isOwner;
			m_AllowSpectators = allowSpectators;
			m_Page = page;

			m_User.CloseGump( typeof( ChessSetGump ) );

			MakeGump();
		}

		public ChessSetGump( Mobile m, ChessGame game, bool isOwner, bool allowSpectators ) : this( m, game, isOwner, allowSpectators, 0 )
		{
		}

		private void MakeGump()
		{
			this.Closable=false;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			this.AddPage(0);
			this.AddBackground(0, 0, 320, 170, 9250);
			this.AddAlphaRegion(0, 0, 320, 170);

			for ( int i = 0; i < 4 && 4 * m_Page + i < Sets.Length; i++ )
			{
				AddButton( 35, 45 + i * 20, 5601, 5605, 10 + 4 * m_Page + i, GumpButtonType.Reply, 0 );
				AddLabel( 60, 43 + i * 20, LabelHue, Sets[ 4 * m_Page + i ] );
			}

			if ( m_Page > 0 )
			{
				this.AddButton(15, 15, 5603, 5607, 1, GumpButtonType.Reply, 0);
			}

			int totalPages = ( Sets.Length - 1 ) / 4;

			// Prev page : 1
			// Next page : 2

			if ( totalPages > m_Page )
			{
				this.AddButton(35, 15, 5601, 5605, 2, GumpButtonType.Reply, 0);
			}

			this.AddLabel(60, 13, GreenHue, @"Chess set selection");

			// Cancel 3
			this.AddButton(15, 130, 4020, 4021,  3, GumpButtonType.Reply, 0);
			this.AddLabel(55, 130, GreenHue, @"Cancel Game");
		}

		public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
		{
			switch ( info.ButtonID )
			{
				case 0 : return;

				case 1:

					sender.Mobile.SendGump( new ChessSetGump( m_User, m_Game, m_IsOwner, m_AllowSpectators, --m_Page ) );
					break;

				case 2:

					sender.Mobile.SendGump( new ChessSetGump( m_User, m_Game, m_IsOwner, m_AllowSpectators, ++m_Page ) );
					break;

				case 3:

					m_Game.CancelGameStart( sender.Mobile );
					break;

				default:

					int index = info.ButtonID - 10;

					ChessSet s = (ChessSet) Enum.Parse( typeof( Arya.Chess.ChessSet ), Sets[ index ], false );
					m_Game.SetChessSet( s );

					sender.Mobile.SendGump( new StartGameGump( sender.Mobile, m_Game, m_IsOwner, m_AllowSpectators ) );
					sender.Mobile.Target = new ChessTarget( m_Game, sender.Mobile, "Please select your parnter...",
						new ChessTargetCallback( m_Game.ChooseOpponent ) );


					break;
			}
		}

	}
}
