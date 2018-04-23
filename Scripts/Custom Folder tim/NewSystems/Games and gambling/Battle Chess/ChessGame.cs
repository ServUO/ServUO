using System;

using Server;

namespace Arya.Chess
{
	/// <summary>
	/// Describes the status of the current game
	/// </summary>
	public enum GameStatus
	{
		/// <summary>
		/// The game is being setup
		/// </summary>
		Setup,
		/// <summary>
		/// White should make the next move
		/// </summary>
		WhiteToMove,
		/// <summary>
		/// Black should make the next move
		/// </summary>
		BlackToMove,
		/// <summary>
		/// A white piece is moving
		/// </summary>
		WhiteMoving,
		/// <summary>
		/// A black piece is moving
		/// </summary>
		BlackMoving,
		/// <summary>
		/// A white pawn has been promoted and the system is waiting for the user to make the decision
		/// </summary>
		WhitePromotion,
		/// <summary>
		/// A black pawn has been promoted and the system is waiting for the user to make the decision
		/// </summary>
		BlackPromotion,
		/// <summary>
		/// Game over
		/// </summary>
		Over
	}

	/// <summary>
	/// Describes the logic for dealing with players, from creation to game end
	/// </summary>
	public class ChessGame
	{
		#region Variables

		/// <summary>
		/// The mobile playing black
		/// </summary>
		private Mobile m_Black;
		/// <summary>
		/// The mobile playing white
		/// </summary>
		private Mobile m_White;
		/// <summary>
		/// Flag stating that the black player is the game owner
		/// </summary>
		private bool m_BlackOwner;
		/// <summary>
		/// Moment when the game started
		/// </summary>
		private DateTime m_GameStart;
		/// <summary>
		/// The time used by black to make its moves
		/// </summary>
		private TimeSpan m_WhiteTime = TimeSpan.Zero;
		/// <summary>
		/// Time used by white to make its moves
		/// </summary>
		private TimeSpan m_BlackTime = TimeSpan.Zero;
		/// <summary>
		/// The BChessboard object providing game logic
		/// </summary>
		private BChessboard m_Board;
		/// <summary>
		/// The piece that is performing a move
		/// </summary>
		private BaseChessPiece m_MovingPiece;
		/// <summary>
		/// The status of the game
		/// </summary>
		private GameStatus m_Status = GameStatus.Setup;
		/// <summary>
		/// The moment when the last move was made
		/// </summary>
		private DateTime m_MoveTime;
		/// <summary>
		/// The bounds of the BChessboard
		/// </summary>
		private Rectangle2D m_Bounds;
		/// <summary>
		/// The height of the BChessboard
		/// </summary>
		private int m_Z;
		/// <summary>
		/// The pawn that is being promoted
		/// </summary>
		private Pawn m_PromotedPawn;
		/// <summary>
		/// The timer object providing time out support
		/// </summary>
		private ChessTimer m_Timer;
		/// <summary>
		/// States whether the game is idle because a player has been disconnected
		/// </summary>
		private bool m_Pending;
		/// <summary>
		/// The ChessControl object owner of this game
		/// </summary>
		private ChessControl m_Parent;
		/// <summary>
		/// The region for the BChessboard
		/// </summary>
		private ChessRegion m_Region;
		/// <summary>
		/// Specifies if other players can get on the board or not
		/// </summary>
		private bool m_AllowSpectators;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the mobile who started this game
		/// </summary>
		public Mobile Owner
		{
			get
			{
				return m_BlackOwner ? m_Black : m_White;
			}
		}

		/// <summary>
		/// Gets or sets the player who has been invited to join the game
		/// </summary>
		public Mobile Guest
		{
			get
			{
				return m_BlackOwner ? m_White : m_Black;
			}
			set
			{
				if ( m_BlackOwner )
					m_White = value;
				else
					m_Black = value;
			}
		}

		/// <summary>
		/// States whether the game is able to accept targets
		/// </summary>
		public bool AllowTarget
		{
			get
			{
				if ( m_Pending ) // Pending status
					return false;

				if ( m_Status == GameStatus.Setup && Owner == null ) // Ownerless setup - is this even needed?
					return false;

				if ( m_Status != GameStatus.Setup )
				{
					if ( m_White == null || m_Black == null || m_White.NetState == null || m_Black.NetState == null )
						return false;
				}

				if ( m_Status == GameStatus.Over )
					return false;

				return true;
			}
		}

		/// <summary>
		/// Verifies if the game is in a consistent state and can send the game gumps to players
		/// </summary>
		public bool AllowGame
		{
			get
			{
				if ( m_Pending )
					return false;

				if ( m_Status == GameStatus.Setup || m_Status == GameStatus.Over )
					return false;

				if ( m_White == null || m_Black == null || m_White.NetState == null || m_Black.NetState == null )
					return false;

				return false;
			}
		}

		/// <summary>
		/// Sets the attack effect on the BChessboard
		/// </summary>
		public int AttackEffect
		{
			get
			{
				if ( m_Parent != null )
					return m_Parent.AttackEffect;
				else
					return 0;
			}
		}

		/// <summary>
		/// Sets the capture effect on the BChessboard
		/// </summary>
		public int CaptureEffect
		{
			get
			{
				if ( m_Parent != null )
					return m_Parent.CaptureEffect;
				else
					return 0;
			}
		}

		/// <summary>
		/// Sets the BoltOnDeath property on the BChessboard
		/// </summary>
		public bool BoltOnDeath
		{
			get
			{
				if ( m_Parent != null )
					return m_Parent.BoltOnDeath;
				else
					return false;
			}
		}

		/// <summary>
		/// Gets the orientation of the BChessboard
		/// </summary>
		public BoardOrientation Orientation
		{
			get
			{
				if ( m_Parent != null )
					return m_Parent.Orientation;
				else
					return BoardOrientation.NorthSouth;
			}
		}
		
		/// <summary>
		/// States whether other players can get on the board
		/// </summary>
		public bool AllowSpectators
		{
			get { return m_AllowSpectators; }
			set { m_AllowSpectators = value; }
		}

		/// <summary>
		/// Gets the BChessboard region
		/// </summary>
		public ChessRegion Region
		{
			get { return m_Region; }
		}

		#endregion

		public ChessGame( ChessControl parent, Mobile owner, Rectangle2D bounds, int z )
		{
			m_Parent = parent;
			m_Bounds = bounds;
			m_Z = z;

			m_BlackOwner = Utility.RandomBool();

			if ( m_BlackOwner )
				m_Black = owner;
			else
				m_White = owner;

			m_AllowSpectators = m_Parent.AllowSpectators;

			// Owner.SendGump( new StartGameGump( Owner, this, true, m_AllowSpectators ) );
			Owner.SendGump( new ChessSetGump( Owner, this, true, m_AllowSpectators ) );

			// Owner.Target = new ChessTarget( this, Owner, "Please select your partner...",
			//	new ChessTargetCallback( ChooseOpponent ) );

			EventSink.Login += new LoginEventHandler(OnPlayerLogin);
			EventSink.Disconnected += new DisconnectedEventHandler(OnPlayerDisconnected);

			m_Timer = new ChessTimer( this );
		}

		#region Game Startup

		/// <summary>
		/// This function is called when one of the two players initializing the game decides to cancel
		/// </summary>
		/// <param name="from">The player refusing</param>
		public void CancelGameStart( Mobile from )
		{
			if ( from == Owner )
			{
				// End this game
				if ( Guest != null )
				{
					Guest.SendMessage( 0x40, "The owner of this game decided to cancel." );
					Guest.CloseGump( typeof( StartGameGump ) );
				}

				if ( from.Target != null && from.Target is ChessTarget )
					(from.Target as ChessTarget).Remove( from );

				Cleanup();
			}
			else if ( from == Guest )
			{
				Guest = null;

				Owner.SendGump( new StartGameGump( Owner, this, true, m_AllowSpectators ) );
				Owner.Target = new ChessTarget( this, Owner, "The selected partner refused the game. Please select another partner...",
					new ChessTargetCallback( ChooseOpponent ) );
			}
		}

		/// <summary>
		/// The guest accepted the game
		/// </summary>
		/// <param name="guest">The player accepting the game</param>
		public void AcceptGame( Mobile guest )
		{
			if ( Owner == null )
			{
				guest.SendMessage( 0x40, "Your partner canceled the game" );
				return;
			}

			m_GameStart = DateTime.Now;
			m_Timer.OnGameStart();

			m_Status = GameStatus.WhiteToMove;

			Guest = guest;

			Owner.CloseGump( typeof( Arya.Chess.StartGameGump ) );

			m_Board = new BChessboard( m_Black, m_White, m_Z, m_Bounds, this, m_Parent.ChessSet, m_Parent.WhiteHue, m_Parent.BlackHue, m_Parent.WhiteMinorHue, m_Parent.BlackMinorHue, m_Parent.OverrideMinorHue );

			m_MoveTime = DateTime.Now;

			// Create the region
			m_Region = new ChessRegion( m_Parent.Map, this, m_AllowSpectators, m_Bounds, m_Z );
			m_Region.Register();

			SendAllGumps( null, null );
		}

		/// <summary>
		/// Callback for choosing a partner for the game
		/// </summary>
		public void ChooseOpponent( Mobile from, object targeted )
		{
			Mobile m = targeted as Mobile;

			if ( m == null || ! m.Player || m.NetState == null )
			{
				Owner.SendGump( new StartGameGump( Owner, this, true, m_AllowSpectators ) );
				Owner.Target = new ChessTarget( this, Owner, "You must select a player. Please select another partner...",
					new ChessTargetCallback( ChooseOpponent ) );
			}
			else if ( m == from )
			{
				from.SendMessage( 0x40, "You can't play against yourself" );

				Owner.SendGump( new StartGameGump( Owner, this, true, m_AllowSpectators ) );
				Owner.Target = new ChessTarget( this, Owner, "You must select a player. Please select another partner...",
					new ChessTargetCallback( ChooseOpponent ) );
			}
			else
			{
				Guest = m;

				Owner.SendGump( new StartGameGump( Owner, this, true, m_AllowSpectators ) );

				Guest.SendGump( new StartGameGump( Guest, this, false, m_AllowSpectators ) );
			}
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Verify if a player is logging back in after disconnecting
		/// </summary>
		private void OnPlayerLogin(LoginEventArgs e)
		{
			if ( ! m_Pending )
				return;

			if ( m_White == null || m_Black == null )
				return;

			if ( e.Mobile == m_White || e.Mobile == m_Black )
			{
				if ( m_White.NetState != null && m_Black.NetState != null )
				{
					m_Pending = false;

					// Both players are back and playing
					m_Timer.OnPlayerConnected();

					m_White.CloseGump( typeof( EndGameGump ) );
					m_Black.CloseGump( typeof( EndGameGump ) );

					if ( m_Status == GameStatus.BlackPromotion )
					{
						// Black
						SendAllGumps( "Waiting for black to promote their pawn", "Please promote your pawn" );

						m_Black.SendGump( new PawnPromotionGump( m_Black, this ) );
					}
					else if ( m_Status == GameStatus.WhitePromotion )
					{
						// White
						SendAllGumps( "Please promote your pawn", "Waiting for white to promote their pawn" );

						m_White.SendGump( new PawnPromotionGump( m_White, this ) );
					}
					else
						SendAllGumps( null, null );
				}
			}
		}

		/// <summary>
		/// Verify if one of the players disconnects
		/// </summary>
		private void OnPlayerDisconnected(DisconnectedEventArgs e)
		{
			if ( e.Mobile != m_Black && e.Mobile != m_White )
				return;

			if ( m_Status == GameStatus.Setup )
			{
				Cleanup(); // No game to loose, just end.
				return;
			}

			if ( m_Status == GameStatus.Over )
			{
				// If game is over, logging out = confirming game over through the gump
				NotifyGameOver( e.Mobile );
				return;
			}

			// Game in progress

			m_Pending = true;

			if ( m_Black != null && m_Black.NetState != null )
			{
				m_Black.CloseGump( typeof( GameGump ) );
				m_Black.SendGump( new EndGameGump( m_Black, this, false,
					"Your partner has been disconnected", ChessConfig.DisconnectTimeOut.Minutes ) );
			}

			if ( m_White != null && m_White.NetState != null )
			{
					m_White.CloseGump( typeof( GameGump ) );
					m_White.SendGump( new EndGameGump( m_White, this, false,
						"Your partner has been disconnected", ChessConfig.DisconnectTimeOut.Minutes ) );
			}

			if ( m_Timer != null )
				m_Timer.OnPlayerDisconnected();
		}

		#endregion

		/// <summary>
		/// Gets the color of a mobile
		/// </summary>
		/// <param name="m">The mobile examined</param>
		/// <returns>The color of the player</returns>
		public ChessColor GetColor( Mobile m )
		{
			if ( m == m_Black )
				return ChessColor.Black;
			else
				return ChessColor.White;
		}

		#region Moving Pieces

		/// <summary>
		/// Sends the move request target
		/// </summary>
		/// <param name="m">The player that must make the move</param>
		public void SendMoveTarget( Mobile m )
		{
			if ( GetColor( m ) == ChessColor.White )
				m_Status = GameStatus.WhiteToMove;
			else
				m_Status = GameStatus.BlackToMove;

			m.Target = new ChessTarget( this, m, "Select the piece you wish to move...",
				new ChessTargetCallback( OnPickPieceTarget ) );
		}

		/// <summary>
		/// Callback for selecting the piece to move
		/// </summary>
		public void OnPickPieceTarget( Mobile from, object targeted )
		{
			m_Black.CloseGump( typeof( EndGameGump ) );
			m_White.CloseGump( typeof( EndGameGump ) );

			if ( ! ( targeted is IPoint2D ) )
			{
				from.SendMessage( 0x40, "Invalid selection" );
				SendMoveTarget( from );
				return;
			}

			BaseChessPiece piece = m_Board[ m_Board.WorldToBoard( new Point2D( targeted as IPoint2D ) ) ];
			
			if ( piece == null || piece.Color != GetColor( from ) )
			{
				from.SendMessage( 0x40, "Invalid selection" );
				SendMoveTarget( from );
				return;
			}

			m_MovingPiece = piece;
			from.Target = new ChessTarget( this, from, "Where do you wish to move?",
				new ChessTargetCallback( OnPieceMove ) );
		}

		/// <summary>
		/// Callback for the move finalization
		/// </summary>
		public void OnPieceMove( Mobile from, object targeted )
		{
			string err = null;

			if ( ! ( targeted is IPoint2D ) )
			{
				err = "Invalid Move";
				m_MovingPiece = null;
				
				if ( GetColor( from ) == ChessColor.Black )
					SendAllGumps( null, err );
				else
					SendAllGumps( err, null );

				return;
			}

			if ( ! m_Board.TryMove( ref err, m_MovingPiece, new Point2D( targeted as IPoint2D ) ) )
			{
				m_MovingPiece = null;

				if ( GetColor( from ) == ChessColor.Black )
					SendAllGumps( null, err );
				else
					SendAllGumps( err, null );

				return;
			}

			// Move has been made. Wait until it's over
			if ( m_Status == GameStatus.WhiteToMove )
			{
				m_Status = GameStatus.WhiteMoving;
				SendAllGumps( "Making your move", null );
			}
			else
			{
				m_Status = GameStatus.BlackMoving;
				SendAllGumps( null, "Making your move" );
			}
		}

		/// <summary>
		/// This function is called when a move is completed, and the next step in game should be performed
		/// </summary>
		public void OnMoveOver( Move move, string whiteMsg, string blackMsg )
		{
			m_Timer.OnMoveMade();

			if ( move != null && move.Piece is Pawn && ( move.Piece as Pawn ).ShouldBePromoted )
			{
				// A pawn should be promoted
				m_PromotedPawn = move.Piece as Pawn;

				if ( m_Status == GameStatus.BlackMoving )
				{
					// Black
					m_Status = GameStatus.BlackPromotion;
					SendAllGumps( "Waiting for black to promote their pawn", "Please promote your pawn" );

					m_Black.SendGump( new PawnPromotionGump( m_Black, this ) );
				}
				else
				{
					// White
					m_Status = GameStatus.WhitePromotion;
					SendAllGumps( "Please promote your pawn", "Waiting for white to promote their pawn" );

					m_White.SendGump( new PawnPromotionGump( m_White, this ) );
				}

				return;
			}

			if ( m_Status == GameStatus.BlackMoving || m_Status == GameStatus.BlackPromotion )
			{
				m_BlackTime += ( DateTime.Now.Subtract( m_MoveTime ) );
				m_Status = GameStatus.WhiteToMove;
			}
			else if ( m_Status == GameStatus.WhiteMoving || m_Status == GameStatus.WhitePromotion )
			{
				m_WhiteTime += ( DateTime.Now.Subtract( m_MoveTime ) );
				m_Status = GameStatus.BlackToMove;
			}

			m_MoveTime = DateTime.Now;

			SendAllGumps( null, null );
		}

		/// <summary>
		/// The user decided to promote a pawn
		/// </summary>
		/// <param name="type">The piece the pawn should promote to</param>
		public void OnPawnPromoted( PawnPromotion type )
		{
			m_Board.OnPawnPromoted( m_PromotedPawn, type );

			m_PromotedPawn = null;
		}

		/// <summary>
		/// This function sends pending gumps to players notifying them to hurry to make a move
		/// </summary>
		public void OnMoveTimeout()
		{
			if ( m_Black.NetState != null )
			{
				m_Black.SendGump( new EndGameGump( m_Black, this, false, string.Format( "No move made in {0} minutes", ChessConfig.MoveTimeOut.Minutes ), ChessConfig.EndGameTimerOut.Minutes ) );
			}

			if ( m_White.NetState != null )
			{
				m_White.SendGump( new EndGameGump( m_White, this, false, string.Format( "No move made in {0} minutes", ChessConfig.MoveTimeOut.Minutes ), ChessConfig.EndGameTimerOut.Minutes ) );
			}
		}

		#endregion

		/// <summary>
		/// Sends the game and score gumps to both player
		/// </summary>
		/// <param name="whiteMsg">The message displayed to white</param>
		/// <param name="blackMsg">The message displayed to black</param>
		public void SendAllGumps( string whiteMsg, string blackMsg )
		{
			if ( m_White == null || m_Black == null )
				return;

			if ( m_Pending )
			{
				whiteMsg = "This game is temporarily stopped";
				blackMsg = whiteMsg;
			}

			m_Black.SendGump( new GameGump(
				m_Black, this, ChessColor.Black, blackMsg,
				! m_Pending && m_Status == GameStatus.BlackToMove,
				m_Status == GameStatus.BlackMoving ) );

			m_White.SendGump( new GameGump(
				m_White, this, ChessColor.White, whiteMsg,
				! m_Pending && m_Status == GameStatus.WhiteToMove,
				m_Status == GameStatus.WhiteMoving ) );

			int[] white = m_Board.GetCaptured( ChessColor.White );
			int[] black = m_Board.GetCaptured( ChessColor.Black );
			int ws = m_Board.GetScore( ChessColor.White );
			int bs = m_Board.GetScore( ChessColor.Black );

			m_White.SendGump( new ScoreGump( m_White, this, white, black, ws, bs ) );
			m_Black.SendGump( new ScoreGump( m_Black, this, white, black, ws, bs ) );
		}

		#region End Game

		/// <summary>
		/// Notifies the parent object to clean up this game
		/// </summary>
		private void ParentCleanup()
		{
			m_Parent.OnGameOver();
		}

		/// <summary>
		/// Cleans up the resources used by this game
		/// </summary>
		public void Cleanup()
		{
			EventSink.Disconnected -= new DisconnectedEventHandler( OnPlayerDisconnected );
			EventSink.Login -= new LoginEventHandler( OnPlayerLogin );

			if ( m_Board != null )
			{
				m_Board.Delete();
			}

			if ( m_Timer != null )
			{
				m_Timer.Stop();
				m_Timer = null;
			}

			if ( m_Black != null && m_Black.Target != null && m_Black.Target is ChessTarget )
				( m_Black.Target as ChessTarget ).Remove( m_Black );

			if ( m_White != null && m_White.Target != null && m_White.Target is ChessTarget )
				( m_White.Target as ChessTarget ).Remove( m_White );

			if ( m_Black != null && m_Black.NetState != null )
			{
				m_Black.CloseGump( typeof( StartGameGump ) );
				m_Black.CloseGump( typeof( GameGump ) );
				m_Black.CloseGump( typeof( EndGameGump ) );
				m_Black.CloseGump( typeof( PawnPromotionGump ) );
				m_Black.CloseGump( typeof( ScoreGump ) );
			}

			if ( m_White != null && m_White.NetState != null )
			{
				m_White.CloseGump( typeof( StartGameGump ) );
				m_White.CloseGump( typeof( GameGump ) );
				m_White.CloseGump( typeof( EndGameGump ) );
				m_White.CloseGump( typeof( PawnPromotionGump ) );
				m_White.CloseGump( typeof( ScoreGump ) );
			}

			if ( m_Region != null )
			{
				m_Region.Unregister();
				m_Region = null;
			}

			ParentCleanup();
		}

		/// <summary>
		/// Notifies that the game has ended
		/// </summary>
		/// <param name="winner">The winner of the game, null for a stall</param>
		public void EndGame( Mobile winner )
		{
			m_Status = GameStatus.Over;

			m_Timer.OnGameOver();

			if ( winner != null )
			{
				GiveWinnerBook( winner );
			}

			string msg = null;

			if ( winner != null )
				msg = string.Format( "Winner: {0}", winner.Name );
			else
				msg = "Game Stalled";

			m_Black.SendGump( new EndGameGump( m_Black, this, true, msg, -1 ) );
			m_White.SendGump( new EndGameGump( m_White, this, true, msg, -1 ) );
		}

		/// <summary>
		/// This function is called by the gumps when players acknowledge the end of the game
		/// </summary>
		/// <param name="m">The mobile acknowledging the end of the game</param>
		public void NotifyGameOver( Mobile m )
		{
			if ( m_Black == m )
			{
				m_Black.CloseGump( typeof( StartGameGump ) );
				m_Black.CloseGump( typeof( GameGump ) );
				m_Black.CloseGump( typeof( EndGameGump ) );
				m_Black.CloseGump( typeof( PawnPromotionGump ) );
				m_Black.CloseGump( typeof( ScoreGump ) );

				m_Black = null;
			}

			if ( m_White == m )
			{
				m_White.CloseGump( typeof( StartGameGump ) );
				m_White.CloseGump( typeof( GameGump ) );
				m_White.CloseGump( typeof( EndGameGump ) );
				m_White.CloseGump( typeof( PawnPromotionGump ) );
				m_White.CloseGump( typeof( ScoreGump ) );

				m_White = null;
			}

			if ( m_Black == null && m_White == null )
			{
				Cleanup();
			}
		}

		/// <summary>
		/// Gives the winner the book with the sum up of the game
		/// </summary>
		/// <param name="to">The winner of the game</param>
		public void GiveWinnerBook( Mobile to )
		{
			Mobile winner = to;
			Mobile looser = null;
			TimeSpan winTime = TimeSpan.Zero;
			TimeSpan looseTime = TimeSpan.Zero;
			int winnerScore = 0;
			int looserScore = 0;

			if ( winner == m_Black )
			{
				looser = m_White;
				looseTime = m_WhiteTime;
				winTime = m_BlackTime;
				winnerScore = m_Board.GetScore( ChessColor.Black );
				looserScore = m_Board.GetScore( ChessColor.White );
			}
			else
			{
				looser = m_Black;
				looseTime = m_BlackTime;
				winTime = m_WhiteTime;
				winnerScore = m_Board.GetScore( ChessColor.White );
				looserScore = m_Board.GetScore( ChessColor.Black );
			}

			if ( winner == null || looser == null )
				return;

			WinnerPaper paper = new WinnerPaper( winner, looser, DateTime.Now - m_GameStart, winTime, looseTime, winnerScore, looserScore );

			if ( to.Backpack != null )
				to.Backpack.AddItem( paper );
		}

		#endregion

		#region Appearance

		/// <summary>
		/// Changes the board's chess set
		/// </summary>
		public void SetChessSet( ChessSet chessset )
		{
			if ( m_Board != null )
				m_Board.ChessSet = chessset;
			else
				m_Parent.SetChessSet( chessset );  // This allows players to choose their own set
		}

		/// <summary>
		/// Resets the pieces hues
		/// </summary>
		/// <param name="white"></param>
		/// <param name="black"></param>
		public void SetHues( int white, int black, int whiteMinor, int blackMinor )
		{
			if ( m_Board != null )
			{
				m_Board.WhiteHue = white;
				m_Board.BlackHue = black;
				m_Board.WhiteMinorHue = whiteMinor;
				m_Board.BlackMinorHue = blackMinor;
			}
		}

		/// <summary>
		/// Sets the orientation of the board
		/// </summary>
		/// <param name="orientation"></param>
		public void SetOrientation( BoardOrientation orientation )
		{
			if ( m_Board != null )
				m_Board.Orientation = orientation;
		}

		/// <summary>
		/// Sets the flag that makes pieces ignore their minor hue
		/// </summary>
		/// <param name="doOverride"></param>
		public void SetMinorHueOverride( bool doOverride )
		{
			if ( m_Board != null )
				m_Board.OverrideMinorHue = doOverride;
		}

		#endregion

		#region Misc

		/// <summary>
		/// Verifies if a specified mobile is a player in the game
		/// </summary>
		/// <param name="m"></param>
		public bool IsPlayer( Mobile m )
		{
			return m == m_Black || m == m_White;
		}

		#endregion
	}
}