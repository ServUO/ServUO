using System;
using System.Collections;

using Server;
using Server.Items;

namespace Arya.Chess
{
	public class Pawn : BaseChessPiece
	{
		public static int GetGumpID( ChessColor color )
		{
			return color == ChessColor.Black ? 2343 : 2336;
		}

		public override int Power
		{
			get
			{
				return 1;
			}
		}

		public override bool AllowEnPassantCapture
		{
			get
			{
				return m_EnPassantRisk;
			}
			set { m_EnPassantRisk = value; }
		}

		private bool m_EnPassantRisk = false;

		public Pawn( BChessboard board, ChessColor color, Point2D position ) : base( board, color, position )
		{
		}

		public override void InitializePiece()
		{
			m_Piece = new ChessMobile( this );
			m_Piece.Name = string.Format( "Pawn [{0}]", m_Color.ToString() );

			switch ( m_BChessboard.ChessSet )
			{
				case ChessSet.Classic : CreateClassic();
					break;

				case ChessSet.Fantasy : CreateFantasy();
					break;

				case ChessSet.FantasyGiant : CreateFantasyGiant();
					break;

				case ChessSet.Animal : CreateAnimal();
					break;

				case ChessSet.Undead : CreateUndead();
					break;
			}
		}

		private void CreateUndead()
		{
			m_MoveSound = 451;
			m_CaptureSound = 1169;
			m_DeathSound = 455;

			m_Piece.BodyValue = 56; // Skeleton
			m_Piece.Hue = Hue;
		}

		private void CreateAnimal()
		{
			m_MoveSound = 1217;
			m_CaptureSound = 226;
			m_DeathSound = 228;

			m_Piece.BodyValue = 221; // Walrus
			m_Piece.Hue = Hue;
		}

		private void CreateFantasyGiant()
		{
			m_MoveSound = 709;
			m_CaptureSound = 712;
			m_DeathSound = 705;

			m_Piece.BodyValue = 303; // Devourer
			m_Piece.Hue = Hue;
		}

		private void CreateFantasy()
		{
			m_MoveSound = 408;
			m_CaptureSound = 927;
			m_DeathSound = 411;

			m_Piece.BodyValue = 776; // Horde Minion
			m_Piece.Hue = Hue;
		}

		private void CreateClassic()
		{
			m_MoveSound = 821;
			m_CaptureSound = 1094;
			m_DeathSound = 1059;

			m_Piece.Female = false;
			m_Piece.BodyValue = 0x190;

			if ( m_BChessboard.OverrideMinorHue )
				m_Piece.Hue = Hue;
			else
				m_Piece.Hue = m_BChessboard.SkinHue;
			m_Piece.AddItem( new ShortHair( m_BChessboard.OverrideMinorHue ? Hue : m_BChessboard.HairHue ) );

			Item item = null;

			item = new ChainChest();
			item.Hue = Hue;
			m_Piece.AddItem( item );

			item = new ChainLegs();
			item.Hue = MinorHue;
			m_Piece.AddItem( item );

			item = new Boots();
			item.Hue = Hue;
			m_Piece.AddItem( item );

			item = new Buckler();
			item.Hue = MinorHue;
			m_Piece.AddItem( item );
			
			item = new Scimitar();
			item.Hue = MinorHue;
			m_Piece.AddItem( item );
		}

		public override bool CanMoveTo(Point2D newLocation, ref string err)
		{
			if ( ! base.CanMoveTo (newLocation, ref err) )
				return false;

			if ( newLocation.X == m_Position.X )
			{
				// Regular move
				int dy = newLocation.Y - m_Position.Y;

				// Trying to move more than two pieces, or more than one piece after the first move
				if ( Math.Abs( dy ) > 2 || ( Math.Abs( dy ) ) > 1 && m_HasMoved )
				{
					err = "You can move only 1 tile at a time, or 2 if it's the pawn's first move";
					return false;
				}

				// Verify direction
				if ( m_Color == ChessColor.Black && dy < 0 )
				{
					err = "You can't move pawns backwards";
					return false;
				}

				if ( m_Color == ChessColor.White && dy > 0 )
				{
					err = "You can't move pawns backwards";
					return false;
				}

				// Verify if there are pieces (any) on the target squares
				for ( int i = 1; i <= Math.Abs( dy ); i++ )
				{
					int offset = m_Color == ChessColor.Black ? i : -i;

					if ( m_BChessboard[ m_Position.X, m_Position.Y + offset ] != null )
					{
						err = "Pawns can't move over other pieces, and capture in diagonal";
						return false;
					}
				}

				return true;
			}
			else
			{
				// Trying to capture?
				int dx = newLocation.X - m_Position.X;
				int dy = newLocation.Y - m_Position.Y;

				if ( Math.Abs( dx ) != 1 || Math.Abs( dy ) != 1 )
				{
					err = "Pawns move straight ahead, or capture in diagonal only";
					return false;
				}

				// Verify direction
				if ( m_Color == ChessColor.Black && dy < 0 )
				{
					err = "You can't move pawns backwards";
					return false;
				}
				if ( m_Color == ChessColor.White && dy > 0 )
				{
					err = "You can't move pawns backwards";
					return false;
				}

				// Verify if there's a piece to capture
				BaseChessPiece piece = m_BChessboard[ newLocation ];

				if ( piece != null && piece.Color != m_Color )
					return true;
				else
				{
					// Verify for an en passant capture
					Point2D passant = new Point2D( m_Position.X + dx, m_Position.Y );

					BaseChessPiece target = m_BChessboard[ passant ];

					if ( target != null && target.AllowEnPassantCapture && target.Color != m_Color )
						return true;
					else
					{
						err = "You must capture a piece when moving in diagonal";
						return false;
					}
				}
			}
		}

		public override ArrayList GetMoves(bool capture)
		{
			ArrayList moves = new ArrayList();

			int direction = m_Color == ChessColor.Black ? 1 : -1;

			Point2D step = new Point2D( m_Position.X, m_Position.Y + direction );

			if ( m_BChessboard.IsValid( step ) && m_BChessboard[ step ] == null )
			{
				moves.Add( step );

				// Verify if this pawn can make a second step
				step.Y += direction;

				if ( ! m_HasMoved && m_BChessboard[ step ] == null )
					moves.Add( step ); // Point2D is a value type
			}

			if ( capture )
			{
				// Verify captures too
				Point2D p1 = new Point2D( m_Position.X + 1, m_Position.Y + direction );
				Point2D p2 = new Point2D( m_Position.X - 1, m_Position.Y + direction );

				if ( m_BChessboard.IsValid( p1 ) )
				{
					BaseChessPiece piece1 = m_BChessboard[ p1 ];
					if ( piece1 != null && piece1.Color != m_Color )
						moves.Add( p1 );
					else
					{
						Point2D pass1 = new Point2D( m_Position.X - 1, m_Position.Y );

						if ( m_BChessboard.IsValid( pass1 ) )
						{
							BaseChessPiece passpiece1 = m_BChessboard[ pass1 ];
							if ( passpiece1 != null && passpiece1.Color != m_Color && passpiece1.AllowEnPassantCapture )
								moves.Add( p1 );
						}
					}
				}

				if ( m_BChessboard.IsValid( p2 ) )
				{
					BaseChessPiece piece2 = m_BChessboard[ p2 ];
					if ( piece2 != null && piece2.Color != m_Color )
						moves.Add( p2 );
					else
					{
						Point2D pass2 = new	 Point2D( m_Position.X + 1, m_Position.Y );

						if ( m_BChessboard.IsValid( p2 ) )
						{
							BaseChessPiece passpiece2 = m_BChessboard[ pass2 ];
							if ( passpiece2 != null && passpiece2.AllowEnPassantCapture && passpiece2.Color != m_Color )
								moves.Add( pass2 );
						}
					}
				}
			}

			return moves;
		}

		/// <summary>
		/// States whether this pawn has reached the other side of the board and should be promoted
		/// </summary>
		public bool ShouldBePromoted
		{
			get
			{
				if ( m_Color == ChessColor.White && m_Position.Y == 0 )
					return true;
				else if ( m_Color == ChessColor.Black && m_Position.Y == 7 )
					return true;
				else
					return false;
			}
		}

		public override void MoveTo(Move move)
		{
			// Set En Passant flags
			int dy = Math.Abs( move.To.Y - m_Position.Y );

			if ( ! m_HasMoved && dy == 2 )
			{
				m_EnPassantRisk = true;
			}

			base.MoveTo (move);
		}

		public override BaseChessPiece GetCaptured(Point2D at, ref bool enpassant)
		{
			BaseChessPiece basePiece = base.GetCaptured (at, ref enpassant);

			if ( basePiece != null && basePiece.Color != m_Color )
				return basePiece; // Normal capture

			if ( at.X == m_Position.X )
				return null; // Straight movement

			Point2D p = new Point2D( at.X, m_Position.Y );
			basePiece = m_BChessboard[ p ];

			if ( basePiece != null && basePiece.Color != m_Color )
			{
				enpassant = true;
				return basePiece;
			}
			else
				return null;
		}
	}
}