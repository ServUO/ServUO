using System;
using System.Collections;

using Server;
using Server.Items;

namespace Arya.Chess
{
	public class Knight : BaseChessPiece
	{
		public static int GetGumpID( ChessColor color )
		{
			return color == ChessColor.Black ? 2342 : 2335;
		}

		public override int Power
		{
			get
			{
				return 3;
			}
		}


		public Knight( BChessboard board, ChessColor color, Point2D position ) : base( board, color, position )
		{
		}

		public override void InitializePiece()
		{
			m_Piece = new ChessMobile( this );
			m_Piece.Name = string.Format( "Knight [{0}]", m_Color.ToString() );

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
			m_MoveSound = 588;
			m_CaptureSound = 1164;
			m_DeathSound = 416;

			m_Piece.Female = false;
			m_Piece.BodyValue = 0x190;
			m_Piece.AddItem( new HoodedShroudOfShadows( Hue ) );

			Server.Mobiles.SkeletalMount mount = new Server.Mobiles.SkeletalMount();
			mount.Hue = MinorHue;
			mount.Rider = m_Piece;

			m_Piece.Direction = Facing;
		}

		private void CreateAnimal()
		{
			m_MoveSound = 183;
			m_CaptureSound = 1011;
			m_DeathSound = 185;

			m_Piece.BodyValue = 292; // Pack llama
			m_Piece.Hue = Hue;
		}

		private void CreateFantasyGiant()
		{
			m_MoveSound = 875;
			m_CaptureSound = 378;
			m_DeathSound = 879;

			m_Piece.BodyValue = 315; // Flesh renderer
			m_Piece.Hue = Hue;
		}

		private void CreateFantasy()
		{
			m_MoveSound = 762;
			m_CaptureSound = 758;
			m_DeathSound = 759;

			m_Piece.BodyValue = 101; // Centaur
			m_Piece.Hue = Hue;
		}

		private void CreateClassic()
		{
			m_MoveSound = 588;
			m_CaptureSound = 168;
			m_DeathSound = 170;

			m_Piece.Female = false;
			m_Piece.BodyValue = 0x190;

			if ( m_BChessboard.OverrideMinorHue )
				m_Piece.Hue = Hue;
			else
				m_Piece.Hue = m_BChessboard.SkinHue;
			m_Piece.AddItem( new PonyTail( m_BChessboard.OverrideMinorHue ? Hue : m_BChessboard.HairHue ) );

			Item item = null;

			item = new PlateLegs();
			item.Hue = Hue;
			m_Piece.AddItem( item );

			item = new PlateChest();
			item.Hue = Hue;
			m_Piece.AddItem( item );

			item = new PlateArms();
			item.Hue = Hue;
			m_Piece.AddItem( item );

			item = new PlateGorget();
			item.Hue = Hue;
			m_Piece.AddItem( item );

			item = new PlateGloves();
			item.Hue = Hue;
			m_Piece.AddItem( item );

			item = new Doublet( MinorHue );
			m_Piece.AddItem( item );

			item = new Lance();
			item.Hue = MinorHue;
			m_Piece.AddItem( item );

			Server.Mobiles.Horse horse = new Server.Mobiles.Horse();
			horse.BodyValue = 200;
			horse.Hue = MinorHue;

			horse.Rider = m_Piece;

			m_Piece.Direction = Facing;
		}

		public override bool CanMoveTo(Point2D newLocation, ref string err)
		{
			if ( ! base.CanMoveTo (newLocation, ref err) )
				return false;

			// Care only about absolutes for knights
			int dx = Math.Abs( newLocation.X - m_Position.X );
			int dy = Math.Abs( newLocation.Y - m_Position.Y );

			if ( ! ( ( dx == 1 && dy == 2 ) || ( dx == 2 && dy == 1 ) ) )
			{
				err = "Knights can only make L shaped moves (2-3 tiles length)";
				return false; // Wrong move
			}
            
			// Verify target piece
			BaseChessPiece piece = m_BChessboard[ newLocation ];

			if ( piece == null || piece.Color != m_Color )
				return true;
			else
			{
				err = "You can't capture pieces of your same color";
				return false;
			}
		}

		public override ArrayList GetMoves(bool capture)
		{
			ArrayList moves = new ArrayList();

			for ( int dx = -2; dx <= 2; dx++ )
			{
				for ( int dy = -2; dy <= 2; dy++ )
				{
					if ( ! ( ( Math.Abs( dx ) == 1 && Math.Abs( dy ) == 2 ) || ( Math.Abs( dx ) == 2 && Math.Abs( dy ) == 1 ) ) )
						continue;

					Point2D p = new Point2D( m_Position.X + dx, m_Position.Y + dy );

					if ( ! m_BChessboard.IsValid( p ) )
						continue;

					BaseChessPiece piece = m_BChessboard[ p ];

					if ( piece == null )
						moves.Add( p );
					else if ( capture && piece.Color != m_Color )
						moves.Add( p );
				}
			}

			return moves;
		}

	}
}