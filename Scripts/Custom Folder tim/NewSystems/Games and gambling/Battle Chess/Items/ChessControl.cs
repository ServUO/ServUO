using System;

using Server;

namespace Arya.Chess
{
	/// <summary>
	/// This is the control stone that allows player to play chess
	/// </summary>
	public class ChessControl : Item
	{
		#region Variables

		private ChessGame m_Game;
		private Rectangle2D m_Bounds;
		private int m_SquareWidth = 2;
		private int m_BoardHeight = 0;
		private ChessSet m_ChessSet = ChessSet.Classic;
		private int m_WhiteHue = 91;
		private int m_BlackHue = 437;
		private int m_AttackEffect = 14068; // Fire snake
		private int m_CaptureEffect = 14186; // Blue/Gold Sparkle 2
		private bool m_BoltOnDeath = true;
		private bool m_AllowSpectators = false;
		private BoardOrientation m_Orientation = BoardOrientation.NorthSouth;
		private int m_BlackMinorHue = 447;
		private int m_WhiteMinorHue = 96;
		private bool m_OverrideMinorHue = false;

		#endregion

		#region Properties

		[ CommandProperty( AccessLevel.GameMaster ) ]
		public Rectangle2D Bounds
		{
			get { return m_Bounds; }
		}

		[ CommandProperty( AccessLevel.GameMaster ) ]
		public Point2D BoardNorthWestCorner
		{
			get { return m_Bounds.Start; }
			set
			{
				m_Bounds.Start = value;
				m_Bounds.Width = 8 * m_SquareWidth;
				m_Bounds.Height = 8 * m_SquareWidth;

				InvalidateProperties();
			}
		}

		[ CommandProperty( AccessLevel.GameMaster ) ]
		public int SquareWidth
		{
			get { return m_SquareWidth; }
			set
			{
				if ( value < 1 )
					return;

				m_SquareWidth = value;

				if ( m_Bounds.Start != Point2D.Zero )
				{
					m_Bounds.Width = 8 * m_SquareWidth;
					m_Bounds.Height = 8 * m_SquareWidth;

					InvalidateProperties();
				}
			}
		}

		[ CommandProperty( AccessLevel.GameMaster ) ]
		public int WhiteHue
		{
			get { return m_WhiteHue; }
			set
			{
				if ( value < 0 || value > 3000 )
					return;

				if ( value == m_WhiteHue )
					return;

				m_WhiteHue = value;

				if ( m_Game != null )
					m_Game.SetHues( m_WhiteHue, m_BlackHue, m_WhiteMinorHue, m_BlackMinorHue );				
			}
		}

		[ CommandProperty( AccessLevel.GameMaster ) ]
		public int BlackHue
		{
			get { return m_BlackHue; }
			set
			{
				if ( value < 0 || value > 3000 )
					return;

				if ( value == m_BlackHue )
					return;

				m_BlackHue = value;

				if ( m_Game != null )
					m_Game.SetHues( m_WhiteHue, m_BlackHue, m_WhiteMinorHue, m_BlackMinorHue );
			}
		}

		[ CommandProperty( AccessLevel.GameMaster ) ]
		public int WhiteMinorHue
		{
			get { return m_WhiteMinorHue; }
			set
			{
				if ( value < 0 || value > 3000 )
					return;

				if ( value == m_WhiteMinorHue )
					return;

				m_WhiteMinorHue = value;

				if ( m_Game != null )
					m_Game.SetHues( m_WhiteHue, m_BlackHue, m_WhiteMinorHue, m_BlackMinorHue );
			}
		}

		[ CommandProperty( AccessLevel.GameMaster ) ]
		public int BlackMinorHue
		{
			get { return m_BlackMinorHue; }
			set
			{
				if ( value < 0 || value > 3000 )
					return;

				if ( value == m_BlackMinorHue )
					return;

				m_BlackMinorHue = value;

				if ( m_Game != null )
					m_Game.SetHues( m_WhiteHue, m_BlackHue, m_WhiteMinorHue, m_BlackMinorHue );
			}
		}

		[ CommandProperty( AccessLevel.GameMaster ) ]
		public bool OverrideMinorHue
		{
			get { return m_OverrideMinorHue; }
			set
			{
				if ( value != m_OverrideMinorHue )
				{
					m_OverrideMinorHue = value;

					if ( m_Game != null )
						m_Game.SetMinorHueOverride( m_OverrideMinorHue );
				}
			}
		}

		[ CommandProperty( AccessLevel.GameMaster ) ]
		public int AttackEffect
		{
			get { return m_AttackEffect; }
			set { m_AttackEffect = value; }
		}

		[ CommandProperty( AccessLevel.GameMaster ) ]
		public int CaptureEffect
		{
			get { return m_CaptureEffect; }
			set { m_CaptureEffect = value; }
		}

		[ CommandProperty( AccessLevel.GameMaster ) ]
		public bool BoltOnDeath
		{
			get { return m_BoltOnDeath; }
			set { m_BoltOnDeath = value; }
		}

		[ CommandProperty( AccessLevel.GameMaster ) ]
		public int BoardHeight
		{
			get { return m_BoardHeight; }
			set { m_BoardHeight = value; }
		}

		[ CommandProperty( AccessLevel.GameMaster ) ]
		public ChessSet ChessSet
		{
			get { return m_ChessSet; }
			set
			{
				m_ChessSet = value;

				if ( m_Game != null )
					m_Game.SetChessSet( m_ChessSet );
			}
		}

		[ CommandProperty( AccessLevel.GameMaster ) ]
		public bool AllowSpectators
		{
			get
			{
				if ( m_Game != null && m_Game.Region != null )
					return m_Game.Region.AllowSpectators;
				else
					return m_AllowSpectators;
			}
			set
			{
				m_AllowSpectators = value;

				if ( m_Game != null && m_Game.Region != null )
				{
					m_Game.Region.AllowSpectators = m_AllowSpectators;
					InvalidateProperties();
				}
			}
		}

		[ CommandProperty( AccessLevel.GameMaster ) ]
		public BoardOrientation Orientation
		{
			get { return m_Orientation; }
			set
			{
				m_Orientation = value;

				if ( m_Game != null )
					m_Game.SetOrientation( m_Orientation );
			}
		}

		#endregion

		[ Constructable ]
		public ChessControl() : base( 3796 )
		{
			Movable = false;
			Hue = 1285;
			Name = "Battle Chess";
			m_Bounds = new Rectangle2D( 0, 0, 0, 0 );
		}

		public ChessControl( Serial serial ) : base( serial )
		{
		}

		#region Serialization

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize (writer);

			writer.Write( 3 ); // Version;

			// Version 3
			writer.Write( m_OverrideMinorHue );
			writer.Write( m_AllowSpectators );
			// Version 2
			writer.Write( (byte) m_Orientation );
			writer.Write( m_BlackMinorHue );
			writer.Write( m_WhiteMinorHue );
			// Version 1
			writer.Write( (int) m_ChessSet );
			writer.Write( m_WhiteHue );
			writer.Write( m_BlackHue );
			writer.Write( m_AttackEffect );
			writer.Write( m_CaptureEffect );
			writer.Write( m_BoltOnDeath );
			// Version 0
			writer.Write( m_Bounds );
			writer.Write( m_SquareWidth );
			writer.Write( m_BoardHeight );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize (reader);

			int version = reader.ReadInt();

			switch ( version )
			{
				case 3:

					m_OverrideMinorHue = reader.ReadBool();
					m_AllowSpectators = reader.ReadBool();
					goto case 2;

				case 2 :

					m_Orientation = (BoardOrientation) reader.ReadByte();
					m_BlackMinorHue = reader.ReadInt();
					m_WhiteMinorHue = reader.ReadInt();
					goto case 1;

				case 1:

					m_ChessSet = ( ChessSet ) reader.ReadInt();
					m_WhiteHue = reader.ReadInt();
					m_BlackHue = reader.ReadInt();
					m_AttackEffect = reader.ReadInt();
					m_CaptureEffect = reader.ReadInt();
					m_BoltOnDeath = reader.ReadBool();
					goto case 0;

				case 0:
					
					m_Bounds = reader.ReadRect2D();
					m_SquareWidth = reader.ReadInt();
					m_BoardHeight = reader.ReadInt();
					break;
			}
		}

		#endregion

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties (list);

			string status = "Not Configured";

			if ( m_Bounds.Width > 0 )
			{
				if ( m_Game == null )
					status = "Ready";
				else
					status = "Game in progress";
			}

			list.Add( 1060658, "Game Status\t{0}", status );

			if ( m_Game != null )
			{
				bool spect = m_AllowSpectators;
				if ( m_Game.Region != null )
					spect = m_Game.Region.AllowSpectators;

				list.Add( 1060659, "Spectators on the Chessboard\t{0}",
					spect ? "Allowed" : "Not Allowed" );
				list.Add( 1060660, "If you loose your target say\t{0}", ChessConfig.ResetKeyword );
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			if ( m_Bounds.Width == 0 )
			{
				// Not configured yet
				if ( from.AccessLevel >= AccessLevel.GameMaster )
				{
					from.Target = new ChessTarget( from, "Target the north west corner of the chessboard you wish to create",
						new ChessTargetCallback( OnBoardTarget ) );
				}
				else
				{
					from.SendMessage( 0x40, "This chess board isn't ready for a game yet. Please contact a game master for assistance with its configuration." );
				}
			}
			else if ( m_Game != null )
			{
				if ( m_Game.IsPlayer( from ) && m_Game.AllowGame )
					m_Game.SendAllGumps( null, null );
				else
					from.SendMessage( 0x40, "A chess game is currently in progress. Please try again later." );
			}
			else
			{
				m_Game = new ChessGame( this, from, m_Bounds, m_BoardHeight );

				InvalidateProperties();
			}
		}

		public void OnGameOver()
		{
			m_Game = null;
			InvalidateProperties();
		}

		private void OnBoardTarget( Mobile from, object targeted )
		{
			IPoint3D p = targeted as IPoint3D;

			if ( p == null )
			{
				from.SendMessage( 0x40, "Invalid location" );
				return;
			}

			BuildBoard( this, new Point3D( p ), Map );
			InvalidateProperties();
		}

		public override void OnDelete()
		{
			if ( m_Game != null )
				m_Game.Cleanup();

			base.OnDelete();
		}

		public void SetChessSet( ChessSet s )
		{
			m_ChessSet = s;
		}

		#region Board Building

		private static void BuildBoard( ChessControl chess, Point3D p, Map map )
		{
			chess.m_BoardHeight = p.Z + 5; // Placing stairs on the specified point
			chess.BoardNorthWestCorner = new Point2D( p.X, p.Y );

			#region Board Tiles

			int stairNW = 1909;
			int stairSE = 1910;
			int stairSW = 1911;
			int stairNE = 1912;
			int stairS = 1901;
			int stairE = 1902;
			int stairN = 1903;
			int stairW = 1904;
			int black = 1295;
			int white = 1298;

			#endregion

			for ( int x = 0; x < 8; x++ )
			{
				for ( int y = 0; y < 8; y++ )
				{
					int tile = 0;

					if ( x % 2 == 0 )
					{
						if ( y % 2 == 0 )
							tile = black;
						else
							tile = white;
					}
					else
					{
						if ( y % 2 == 0 )
							tile = white;
						else
							tile = black;
					}

					if ( chess.Orientation == BoardOrientation.EastWest ) // Invert tiles if the orientation is EW
					{
						if ( tile == white )
							tile = black;
						else
							tile = white;
					}

					for ( int kx = 0; kx < chess.m_SquareWidth; kx++ )
					{
						for ( int ky = 0; ky < chess.m_SquareWidth; ky++ )
						{
							Server.Items.Static s = new Server.Items.Static( tile );
							Point3D target = new Point3D( p.X + x * chess.m_SquareWidth + kx, p.Y + y * chess.m_SquareWidth + ky, chess.m_BoardHeight );
							s.MoveToWorld( target, map );
						}
					}
				}
			}

			Point3D nw = new Point3D( p.X - 1, p.Y - 1, p.Z );
			Point3D ne = new Point3D( p.X + 8 * chess.m_SquareWidth, p.Y - 1, p.Z );
			Point3D se = new Point3D( p.X + 8 * chess.m_SquareWidth, p.Y + 8 * chess.m_SquareWidth, p.Z );
			Point3D sw = new Point3D( p.X - 1, p.Y + 8 * chess.m_SquareWidth, p.Z );

			new Server.Items.Static( stairNW ).MoveToWorld( nw, map );
			new Server.Items.Static( stairNE ).MoveToWorld( ne, map );
			new Server.Items.Static( stairSE ).MoveToWorld( se, map );
			new Server.Items.Static( stairSW ).MoveToWorld( sw, map );

			for ( int x = 0; x < 8 * chess.m_SquareWidth; x++ )
			{
				Point3D top = new Point3D( p.X + x, p.Y - 1, p.Z );
				Point3D bottom = new Point3D( p.X + x, p.Y + 8 * chess.m_SquareWidth, p.Z );
				Point3D left = new Point3D( p.X - 1, p.Y + x, p.Z );
				Point3D right = new Point3D( p.X + chess.m_SquareWidth * 8, p.Y + x, p.Z );

				new Server.Items.Static( stairN ).MoveToWorld( top, map );
				new Server.Items.Static( stairS ).MoveToWorld( bottom, map );
				new Server.Items.Static( stairW ).MoveToWorld( left, map );
				new Server.Items.Static( stairE ).MoveToWorld( right, map );
			}
		}

		#endregion
	}
}
