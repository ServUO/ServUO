using System;
using System.Collections;

using Server;

namespace Arya.Chess
{
	/// <summary>
	/// Specifies the status of a player
	/// </summary>
	public enum Status
	{
		Normal,
		Check,
		CheckMate,
		Stall
	}

	/// <summary>
	/// Defines what kind of piece a promoted pawn should be changed to
	/// </summary>
	public enum PawnPromotion
	{
		Queen,
		Rook,
		Knight,
		Bishop,
		None
	}

	/// <summary>
	/// Defines the style of the NPCs on the BChessboard
	/// </summary>
	public enum ChessSet : int
	{
		Classic,
		Fantasy,
		FantasyGiant,
		Animal,
		Undead
	}

	/// <summary>
	/// The orientaion of the game on the BChessboard
	/// </summary>
	public enum BoardOrientation : byte
	{
		NorthSouth = 0,
		EastWest = 1
	}

	/// <summary>
	/// This class holds the BChessboard logic that manages the chess game
	/// </summary>
	public class BChessboard
	{
		#region Variables

		/// <summary>
		/// The Map the physical BChessboard lies on
		/// </summary>
		private Map m_Map;
		/// <summary>
		/// The Z coordinate of the BChessboard plane
		/// </summary>
		private int m_Z;
		/// <summary>
		/// The player playing white
		/// </summary>
		private Mobile m_White;
		/// <summary>
		/// The player playing black
		/// </summary>
		private Mobile m_Black;
		/// <summary>
		/// This table holds all the pieces, their location on the board is the key
		/// </summary>
		private Hashtable m_Table;
		/// <summary>
		/// Lists the pieces that have been captured
		/// </summary>
		private ArrayList m_CapturedPieces;
		/// <summary>
		/// The physical bounds of this board
		/// </summary>
		private Rectangle2D m_Bounds;
		/// <summary>
		/// The step between each square
		/// </summary>
		private int m_Step;
		/// <summary>
		/// The offset for the piece position
		/// </summary>
		private int m_Offset;
		/// <summary>
		/// The game object
		/// </summary>
		private ChessGame m_Game;
		/// <summary>
		/// The chess set for this board
		/// </summary>
		private ChessSet m_ChessSet;
		/// <summary>
		/// The hue used for white pieces
		/// </summary>
		private int m_WhiteHue = 1151;
		/// <summary>
		/// The hue used for black pieces
		/// </summary>
		private int m_BlackHue = 1175;
		/// <summary>
		/// The orientation of the BChessboard
		/// </summary>
		private BoardOrientation m_Orientation;
		/// <summary>
		/// The minor hue for white pieces
		/// </summary>
		private int m_WhiteMinorHue;
		/// <summary>
		/// The minor hue for black pieces
		/// </summary>
		private int m_BlackMinorHue;
		/// <summary>
		/// States if a piece is performing a move
		/// </summary>
		private bool m_IsMoving = false;
		/// <summary>
		/// States if a rebuild action should be performed after a move is over
		/// </summary>
		private bool m_DoRebuild = false;
		/// <summary>
		/// Specifies if the Minor Hue should be ignored
		/// </summary>
		private bool m_OverrideMinorHue;
		/// <summary>
		/// The hue for the NPCs skin
		/// </summary>
		private int m_SkinHue = -1;
		/// <summary>
		/// The hue for the NPCs hair
		/// </summary>
		private int m_HairHue = -1;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the map for this game
		/// </summary>
		public Map Map
		{
			get { return m_Map; }
		}

		/// <summary>
		/// Gets the Z coordinate of the board plane
		/// </summary>
		public int Z
		{
			get { return m_Z; }
		}

		/// <summary>
		/// Gets or sets the chess set for this board
		/// </summary>
		public ChessSet ChessSet
		{
			get { return m_ChessSet; }
			set
			{
				if ( m_ChessSet != value )
				{
					m_ChessSet = value;
					Rebuild();
				}
			}
		}

		/// <summary>
		/// Gets or sets the hue used for white pieces
		/// </summary>
		public int WhiteHue
		{
			get { return m_WhiteHue; }
			set
			{
				if ( m_WhiteHue != value )
				{
					m_WhiteHue = value;
					Rebuild();
				}
			}
		}

		/// <summary>
		/// Gets or sets the hue for black pieces
		/// </summary>
		public int BlackHue
		{
			get { return m_BlackHue; }
			set
			{
				if ( m_BlackHue != value )
				{
					m_BlackHue = value;
					Rebuild();
				}
			}
		}

		/// <summary>
		/// Gets or sets the minor hue for white pieces
		/// </summary>
		public int WhiteMinorHue
		{
			get
			{
				return m_WhiteMinorHue;
			}
			set
			{
				if ( m_WhiteMinorHue != value )
				{
					m_WhiteMinorHue = value;

					if ( ! m_OverrideMinorHue )
						Rebuild();
				}
			}
		}

		/// <summary>
		/// Gets or sets the minor hue for black pieces
		/// </summary>
		public int BlackMinorHue
		{
			get
			{
				return m_BlackMinorHue;
			}
			set
			{
				if ( m_BlackMinorHue != value )
				{
					m_BlackMinorHue = value;

					if ( ! m_OverrideMinorHue )
						Rebuild();
				}
			}
		}

		/// <summary>
		/// Gets or sets the effect performed whan a NPC attacks
		/// </summary>
		public int AttackEffect
		{
			get
			{
				if ( m_Game != null )
					return m_Game.AttackEffect;
				else
					return 0;
			}
		}

		/// <summary>
		/// Gets or sets the effect performed when capturing a NPC
		/// </summary>
		public int CaptureEffect
		{
			get
			{
				if ( m_Game != null )
					return m_Game.CaptureEffect;
				else
					return 0;
			}
		}

		/// <summary>
		/// States whether the dying NPC should display a bolt animation
		/// </summary>
		public bool BoltOnDeath
		{
			get
			{
				if ( m_Game != null )
					return m_Game.BoltOnDeath;
				else
					return false;
			}
		}

		/// <summary>
		/// Gets the orientation of the board
		/// </summary>
		public BoardOrientation Orientation
		{
			get { return m_Orientation; }
			set
			{
				if ( value != m_Orientation )
				{
					m_Orientation = value;
					Rebuild();
				}
			}
		}

		/// <summary>
		/// Specifies whether the main hue should be used as the minor hue as well
		/// </summary>
		public bool OverrideMinorHue
		{
			get { return m_OverrideMinorHue; }
			set
			{
				if ( m_OverrideMinorHue != value )
				{
					m_OverrideMinorHue = value;
					Rebuild();
				}
			}
		}

		/// <summary>
		/// Gets the hue for this game
		/// </summary>
		public int SkinHue
		{
			get
			{
				if ( m_SkinHue == -1 )
					m_SkinHue = Utility.RandomSkinHue();

				return m_SkinHue;
			}
		}

		/// <summary>
		/// Gets the hue for hair for this game
		/// </summary>
		public int HairHue
		{
			get
			{
				if ( m_HairHue == -1 )
					m_HairHue = Utility.RandomHairHue();

				return m_HairHue;
			}
		}

		#endregion

		public BChessboard( Mobile black, Mobile white, int z, Rectangle2D bounds, ChessGame game, ChessSet chessSet, int whiteHue, int blackHue, int whiteMinorHue, int blackMinorHue, bool overrideMinorHue )
		{
			m_Game = game;
			m_Black = black;
			m_White = white;

			m_ChessSet = chessSet;
			m_WhiteHue = whiteHue;
			m_BlackHue = blackHue;
			m_WhiteMinorHue = whiteMinorHue;
			m_BlackMinorHue = blackMinorHue;
			m_Orientation = m_Game.Orientation;
			m_OverrideMinorHue = overrideMinorHue;

			m_Map = m_Black.Map;
			m_Z = z;

			m_Table = new Hashtable();
			m_CapturedPieces = new ArrayList();

			m_Bounds = bounds;
			m_Step = bounds.Width / 8;
			m_Offset = m_Step / 2;

			PlacePieces();
		}

		/// <summary>
		/// Gets the chess piece currently on the board
		/// </summary>
		public BaseChessPiece this[int x, int y]
		{
			get
			{
				return m_Table[ new Point2D( x,y ) ] as BaseChessPiece;
			}
		}

		/// <summary>
		/// Gets the chess piece currently on the board
		/// </summary>
		public BaseChessPiece this[ Point2D p ]
		{
			get
			{
				return m_Table[ p ] as BaseChessPiece;
			}
		}

		#region Board Creation

		/// <summary>
		/// Adds a chess piece to the board table
		/// </summary>
		/// <param name="piece">The piece being added</param>
		private void AddPiece( BaseChessPiece piece )
		{
			m_Table.Add( piece.Position, piece );
		}

		/// <summary>
		/// Creates the pieces and places them in the world
		/// </summary>
		private void PlacePieces()
		{
			// Rooks
			AddPiece( new Rook( this, ChessColor.Black, new Point2D( 0,0 ) ) );
			AddPiece( new Rook( this, ChessColor.Black, new Point2D( 7,0 ) ) );

			AddPiece( new Rook( this, ChessColor.White, new Point2D( 0,7 ) ) );
			AddPiece( new Rook( this, ChessColor.White, new Point2D( 7,7 ) ) );

			// Knights
			AddPiece( new Knight( this, ChessColor.Black, new Point2D( 1,0 ) ) );
			AddPiece( new Knight( this, ChessColor.Black, new Point2D( 6,0 ) ) );

			AddPiece( new Knight( this, ChessColor.White, new Point2D( 1,7 ) ) );
			AddPiece( new Knight( this, ChessColor.White, new Point2D( 6,7 ) ) );
			
			// Bishops
			AddPiece( new Bishop( this, ChessColor.Black, new Point2D( 2,0 ) ) );
			AddPiece( new Bishop( this, ChessColor.Black, new Point2D( 5,0 ) ) );
			
			AddPiece( new Bishop( this, ChessColor.White, new Point2D( 2,7 ) ) );
			AddPiece( new Bishop( this, ChessColor.White, new Point2D( 5,7 ) ) );

			// Queens
			AddPiece( new Queen( this, ChessColor.Black, new Point2D( 3,0 ) ) );

			AddPiece( new Queen( this, ChessColor.White, new Point2D( 3,7 ) ) );

			// Kings
			AddPiece( new King( this, ChessColor.Black, new Point2D( 4,0 ) ) );
			
			AddPiece( new King( this, ChessColor.White, new Point2D( 4,7 ) ) );

			// Pawns
			for ( int i = 0; i < 8; i++ )
			{
				AddPiece( new Pawn( this, ChessColor.Black, new Point2D( i, 1 ) ) );
				AddPiece( new Pawn( this, ChessColor.White, new Point2D( i, 6 ) ) );
			}
		}

		/// <summary>
		/// Rebuilds the BChessboard applying any changes made to the NPCs
		/// </summary>
		public void Rebuild()
		{
			// If a piece is moving, set the rebuild flag to true. When the move is over, the OnMoveOver
			// function will call Rebuild()
			if ( m_IsMoving )
			{
				m_DoRebuild = true;
				return;
			}

			foreach( BaseChessPiece piece in m_Table.Values )
			{
				piece.Rebuild();
			}

			m_DoRebuild = false;
		}

		#endregion

		#region Physic Board Managment

		/// <summary>
		/// Verifies is a given Point2D is a valid position on the BChessboard
		/// </summary>
		/// <param name="pos">The Point2D considered</param>
		/// <returns>True if the position provided is part of the BChessboard</returns>
		public bool IsValid( Point2D pos )
		{
			return pos.X >= 0 && pos.Y >= 0 && pos.X < 8 && pos.Y < 8;
		}

		/// <summary>
		/// Converts a position on the board, to a real world location
		/// </summary>
		/// <param name="boardPosition">The point on the board</param>
		/// <returns>The corresponding real world coordinate</returns>
		public Point2D BoardToWorld( Point2D boardPosition )
		{
			if ( m_Orientation == BoardOrientation.NorthSouth )
			{
				int xoffset = boardPosition.X * m_Step + m_Offset;
				int yoffset = boardPosition.Y * m_Step + m_Offset;

				return new Point2D( m_Bounds.X + xoffset, m_Bounds.Y + yoffset );
			}
			else
			{
				int xoffset = boardPosition.Y * m_Step + m_Offset;
				int yoffset = boardPosition.X * m_Step + m_Offset;

				return new Point2D(
					m_Bounds.X + xoffset,
					m_Bounds.End.Y - yoffset );
			}
		}

		/// <summary>
		/// Converts a position in the game world to a position on the board
		/// </summary>
		/// <param name="worldPosition">The location being converted</param>
		/// <returns>Board coordinates</returns>
		public Point2D WorldToBoard( Point2D worldPosition )
		{
			if ( m_Orientation == BoardOrientation.NorthSouth )
			{
				int dx = worldPosition.X - m_Bounds.X;
				int dy = worldPosition.Y - m_Bounds.Y;

				return new Point2D( dx / m_Step, dy / m_Step );
			}
			else
			{
				int dx = m_Bounds.End.Y - worldPosition.Y - 1;
				int dy = worldPosition.X - m_Bounds.X;

				return new Point2D( dx / m_Step, dy / m_Step );
			}
		}

		#endregion

		#region Move Managment

		/// <summary>
		/// Tries to perfrom a move. Will send any diagnostic messages to user if failed.
		/// Will perform the move if it's valid.
		/// </summary>
		/// <param name="err">This string will hold any error messages if the move fails</param>
		/// <param name="piece">The piece being moved</param>
		/// <param name="to">The world location of the move target</param>
		/// <returns>True if the move is legal</returns>
		public bool TryMove( ref string err, BaseChessPiece piece, Point2D to )
		{
			Point2D p2 = WorldToBoard( to );

			if ( piece == null )
			{
				err = "You must select a piece for your move";
				return false;
			}

			if ( ! piece.CanMoveTo( p2, ref err ) )
			{
				return false; // Invalid move
			}

			if ( piece.IsCastle( p2 ) )
			{
				// This move is making a castle. All needed verification is made by the AllowCastle() function.
				Rook rook = piece as Rook;

				if ( rook == null )
					rook = this[ p2 ] as Rook;

				m_IsMoving = true;
				rook.Castle();
				return true;
			}

			Move move = new Move( piece, p2 );
			ApplyMove( move );

			// bool ok = !IsCheck( piece.Color ) || IsCheckMate( piece.EnemyColor );
			bool ok = !IsCheck( piece.Color );

			UndoMove( move );

			if ( ok )
			{
				m_IsMoving = true;
				piece.MoveTo( move );

				foreach( BaseChessPiece pass in m_Table.Values )
				{
					if ( pass != piece )
						pass.AllowEnPassantCapture = false; // Reset en passant allowance
				}
			}
			else
			{
				err = "That move would put your king under check";
			}

			return ok;
		}

		/// <summary>
		/// A chess piece will call this function to notify the board that its movement is complete
		/// </summary>
		/// <param name="move">The move perfromed</param>
		public void OnMoveOver( Move move )
		{
			ApplyMove( move );
			FinalizeMove( move );

			m_IsMoving = false;

			if ( m_DoRebuild )
				Rebuild();

			PushGame( move.EnemyColor, move );
		}

		/// <summary>
		/// Pushes forward the game, by allowing the next move to be executed and supplying information about the game status
		/// </summary>
		/// <param name="nextMoveColor">The color of the player making the next move</param>
		/// <param name="move">The last move made</param>
		private void PushGame( ChessColor nextMoveColor, Move move )
		{
			// Move is over. Verify the following:
			//
			// 1. Opponent is checked
			// 2. Opponent is checkmated
			// 3. Opponent is stalled

			Status status = GetStatus( nextMoveColor );

			switch ( status )
			{
				case Status.Check:

					King k = GetKing( nextMoveColor ) as King;
					k.PlayCheck();

					if ( nextMoveColor == ChessColor.White )
						m_Game.OnMoveOver( move, "Your king is under check!", "You have the opponent's king under check!" );
					else
						m_Game.OnMoveOver( move, "You have the opponent's kind under check!", "Your king is under check" );

					break;

				case Status.CheckMate:

					King king = GetKing( nextMoveColor ) as King;
					king.PlayCheckMate();

					m_Game.EndGame( nextMoveColor == ChessColor.White ? m_Black : m_White );

					break;

				case Status.Stall:

					King wKing = GetKing( ChessColor.White ) as King;
					King bKing = GetKing( ChessColor.Black ) as King;

					wKing.PlayStaleMate();
					bKing.PlayStaleMate();

					m_Game.EndGame( null );

					break;

				case Status.Normal:

					m_Game.OnMoveOver( move, null, null );

					break;
			}
		}

		/// <summary>
		/// A pawn has been promoted and should be changed on the board
		/// </summary>
		/// <param name="pawn">The pawn that has been promoted</param>
		/// <param name="to">The type of piece it should be promoted to</param>
		public void OnPawnPromoted( BaseChessPiece pawn, PawnPromotion to )
		{
			BaseChessPiece promoted = null;

			switch ( to )
			{
				case PawnPromotion.Queen:

					promoted = new Queen( this, pawn.Color, pawn.Position );
					break;

				case PawnPromotion.Rook:

					promoted = new Rook( this, pawn.Color, pawn.Position );
					break;

				case PawnPromotion.Knight:

					promoted = new Knight( this, pawn.Color, pawn	.Position );
					break;

				case PawnPromotion.Bishop:

					promoted = new Bishop( this, pawn.Color, pawn.Position );
					break;
			}

			if ( promoted != null )
			{
				m_Table[ pawn.Position ] = promoted;

				pawn.Die( false );
			}

			PushGame( pawn.EnemyColor, null );
		}

		/// <summary>
		/// Applies a move to the BChessboard logic
		/// </summary>
		/// <param name="move">The move object</param>
		public void ApplyMove( Move move )
		{
			if ( move.Capture )
			{
				m_Table.Remove( move.CapturedPiece.Position );
			}

			m_Table.Remove( move.From );
			m_Table[ move.To ] = move.Piece; // This will automatically remove any piece stored before

			move.Piece.Position = move.To;
		}

		/// <summary>
		/// Finalizes the move adding the captured piece to the captured pieces list
		/// </summary>
		/// <param name="move">The move performed</param>
		private void FinalizeMove( Move move )
		{
			if ( ! move.Capture )
				return;

			m_CapturedPieces.Add( move.CapturedPiece );
		}

		/// <summary>
		/// Undos a move
		/// </summary>
		/// <param name="move">The move being reverted</param>
		private void UndoMove( Move move )
		{
			m_Table.Remove( move.To );
			m_Table.Add( move.From, move.Piece	);
			move.Piece.Position = move.From;

			if ( move.Capture )
			{
				m_Table.Add( move.To, move.CapturedPiece );
				move.CapturedPiece.Position = move.To;
			}
		}

		/// <summary>
		/// Plays a sound to all mobiles within 20 tiles of the NPC emitting it
		/// </summary>
		/// <param name="m">The NPC producing the sound</param>
		/// <param name="sound">The sound to play</param>
		public void PlaySound( ChessMobile m, int sound )
		{
			if ( m == null )
				return;

			Server.Network.Packet p = new Server.Network.PlaySound( sound, m.Location );

			foreach( Server.Network.NetState state in m.GetClientsInRange( 20 ) )
			{
				if ( state.Mobile.CanSee( m ) )
					state.Send( p );
			}
		}

		#endregion

		#region Game Checks

		/// <summary>
		/// Verifies if the specified mobile is the owner or a given chess piece
		/// </summary>
		/// <param name="m">The mobile being checked</param>
		/// <param name="piece">The piece being examined for ownership</param>
		/// <returns>True if the mobile is the owner of the specified piece</returns>
		private bool IsOwner( Mobile m, BaseChessPiece piece )
		{
			if ( m == m_Black && piece.Color == ChessColor.Black )
				return true;
			else if ( m == m_White && piece.Color == ChessColor.White)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Verifies if a given player is in check
		/// </summary>
		/// <param name="color">The color being examined</param>
		/// <returns>True if the specified king is checked</returns>
		public bool IsCheck( ChessColor color )
		{
			BaseChessPiece king = GetKing( color );

			if ( king == null )
			{
				// This occurs when the game is performing a simulation
				// In this case a potential move kills the king,
				// therefore the king is indeed attacked.
				return true;
			}

			return IsAttacked( king.Position, king.EnemyColor );
		}

		/// <summary>
		/// Verifies is a player is check mate (game lost). This function does not consider the current location
		/// of the king, but only if a move can lead out of check.
		/// </summary>
		/// <param name="color">The color being examined</param>
		/// <returns>True if the player is check mated and the game is lost</returns>
		public bool IsCheckMate( ChessColor color )
		{
			ArrayList moves = new ArrayList();

			// Generate list of possible moves including captures
			foreach ( BaseChessPiece piece in m_Table.Values )
			{
				if ( piece.Color != color )
					continue;

				ArrayList m = piece.GetMoves( true );

				foreach( Point2D p in m )
				{
					moves.Add( new Move( piece, p ) );
				}
			}

			// Applying each move, and if at least one of them doesn't lead to a check, there isn't a check mate
			foreach( Move move in moves )
			{
				ApplyMove( move );

				bool check = IsCheck( color );

				UndoMove( move );

				if ( ! check )
					return false;
			}

			return true;
		}

		/// <summary>
		/// Gets the current status of a player
		/// </summary>
		/// <param name="color">The player being examined</param>
		/// <returns>The Status enumeration value corresponding to the board situation</returns>
		private Status GetStatus( ChessColor color )
		{
			bool check = IsCheck( color );
			bool mate = IsCheckMate( color );

			if ( check && mate )
				return Status.CheckMate;

			if ( check && ! mate )
				return Status.Check;

			if ( ! check && mate )
				return Status.Stall;

			return Status.Normal;
		}

		/// <summary>
		/// Verifies is a given square can be attacked
		/// </summary>
		/// <param name="pos">The square examined. The square should be either empty or occupied by a possible capture for the attacker</param>
		/// <param name="color">The color of the player attacking</param>
		/// <returns>True if the specified player can attack the square</returns>
		public bool IsAttacked( Point2D pos, ChessColor by )
		{
			string err = null;

			foreach( BaseChessPiece piece in m_Table.Values )
			{
				if ( piece.Color == by && piece.CanMoveTo( pos, ref err ) )
					return true;
			}

			return false;
		}

		#endregion

		#region Castle

		/// <summary>
		/// Verifies if the requested castle moved is allowed
		/// </summary>
		/// <param name="king">The King performing the castle</param>
		/// <param name="rook">The Rook</param>
		/// <param name="err">Will hold any error messages</param>
		/// <returns>True if the castle move is allowed</returns>
		public bool AllowCastle( BaseChessPiece king, BaseChessPiece rook, ref string err )
		{
			#region Castle Rules
			// 1 Your king has been moved earlier in the game.
			// 2 The rook that castles has been moved earlier in the game.
			// 3 There are pieces standing between your king and rook.
			// 4 The king is in check.
			// 5 The king moves through a square that is attacked by a piece of the opponent.
			// 6 The king would be in check after castling.
			#endregion

			if ( king.HasMoved || rook.HasMoved )
			{
				err = "You can't castle if the rook or king have already moved";
				return false; // Rules 1 and 2
			}

			if ( IsCheck( king.Color ) )
			{
				err = "You can't castle if your king is in check";
				return false; // Rule 4
			}

			bool queenside = rook.Position.X == 0;

			if ( queenside )
			{
				for ( int i = 1; i < 4; i++ )
				{
					if ( this[ i, king.Position.Y ] != null )
					{
						err = "You can't castle if there are pieces between the king and the rook";
						return false; // Rule 3 queenside
					}
				}
			}
			else
			{
				if ( this[ 5, king.Position.Y ] != null || this[ 6, king.Position.Y ] != null )
				{
					err = "You can't castle if there are pieces between the king and the rook";
					return false; // Rule 3 kingside
				}
			}

			// King always moves 2
			int kingX = king.Position.X + ( queenside ? -2 : 2 );
			int kingTransit = king.Position.X + ( queenside ? -1 : 1 );

			if ( IsAttacked( new Point2D( kingTransit, king.Position.Y ), king.EnemyColor ) )
			{
				err = "The king cannot move through a square that is under attack by the opponent";
				return false; // Rule 5
			}

			if ( IsAttacked( new Point2D( kingX, king.Position.Y ), king.EnemyColor ) )
			{
				err = "The king would be in check after the castle";
				return false; // Rule 6
			}

			return true;
		}

		#endregion

		/// <summary>
		/// Gets the King for a specified player
		/// </summary>
		/// <param name="color">The color of the king searched</param>
		/// <returns>The King piece</returns>
		public BaseChessPiece GetKing( ChessColor color )
		{
			foreach( BaseChessPiece piece in m_Table.Values )
			{
				if ( piece is King && piece.Color == color )
					return piece;
			}

			return null;
		}

		#region Score

		/// <summary>
		/// Gets the score for a given player
		/// </summary>
		/// <param name="color">The color of the player</param>
		/// <returns>The score value</returns>
		public int GetScore( ChessColor color )
		{
			int score = 0;

			foreach( BaseChessPiece piece in m_CapturedPieces )
			{
				if ( piece.Color != color )
					score += piece.Power;
			}

			return score;
		}

		/// <summary>
		/// Gets the amounts of pieces captured for a given. The pieces order in the array is:
		/// Pawns, Knights, Bishops, Rooks, Queen
		/// </summary>
		/// <param name="color">The player being examined</param>
		/// <returns>An array of int values specifies the amount of captures of the specified color</returns>
		public int[] GetCaptured( ChessColor color )
		{
			int[] captured = new int[] { 0, 0, 0, 0, 0 };

			foreach( BaseChessPiece piece in m_CapturedPieces )
			{
				if ( piece.Color != color )
					continue;

				if ( piece is Pawn )
					captured[ 0 ]++;
				else if ( piece is Knight )
					captured[ 1 ]++;
				else if ( piece is Bishop )
					captured[ 2 ]++;
				else if ( piece is Rook )
					captured[ 3 ]++;
				else if ( piece is Queen )
					captured[ 4 ]++;
			}

			return captured;
		}

		#endregion

		#region Deletion

		/// <summary>
		/// Deletes all the mobiles associated with the board
		/// </summary>
		public void Delete()
		{
			foreach( BaseChessPiece piece in m_Table.Values )
				piece.Die( false );
		}

		/// <summary>
		/// The staff deleted a piece from the BChessboard, so clean up the game
		/// </summary>
		public void OnStaffDelete()
		{
			if ( m_Black != null )
				m_Black.SendMessage( 0x40, "Your game has been terminated by the staff" );

			if ( m_White != null )
				m_White.SendMessage( 0x40, "Your game has been terminated by the staff" );
															  
			m_Game.Cleanup();
		}

		#endregion
	}
}