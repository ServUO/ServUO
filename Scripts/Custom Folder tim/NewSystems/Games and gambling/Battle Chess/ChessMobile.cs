using System;
using System.Collections;

using Server;
using Server.Items;
using Server.Mobiles;

namespace Arya.Chess
{
	/// <summary>
	/// The basic mobile that will be used as the actual chess piece
	/// </summary>
	public class ChessMobile : BaseCreature
	{
		/// <summary>
		/// The chess piece that owns this NPC
		/// </summary>
		private BaseChessPiece m_Piece;
		/// <summary>
		/// Specifies the location of the next position of this piece
		/// </summary>
		private Point3D m_NextMove = Point3D.Zero;
		/// <summary>
		/// The list of waypoints used by this NPC
		/// </summary>
		private ArrayList m_WayPoints;

		public ChessMobile( BaseChessPiece piece ) : base( AIType.AI_Use_Default, FightMode.None, 1, 1, 0.2, 0.2 )
		{
			m_WayPoints = new ArrayList();

			InitStats( 25, 100, 100 );
			m_Piece = piece;

			Blessed = true;
			Paralyzed = true;
			Direction = m_Piece.Facing;
		}

		#region Serialization

		public ChessMobile( Serial serial ) : base( serial )
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize( writer );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			Delete();
		}

		#endregion

		#region Movement on the BChessboard

		/// <summary>
		/// Places the piece on the board for the first time
		/// </summary>
		/// <param name="location">The location where the piece should be placed</param>
		/// <param name="map">The map where the game takes place</param>
		public void Place( Point3D location, Map map )
		{
			MoveToWorld( location, map );
			FixedParticles( 0x373A, 1, 15, 5012, Hue, 2, EffectLayer.Waist );
		}

		/// <summary>
		/// Moves the NPC to the specified location
		/// </summary>
		/// <param name="to">The location the NPC should move to</param>
		public void GoTo( Point2D to )
		{
			AI = AIType.AI_Melee;

			m_NextMove = new Point3D( to, Z );

			if ( m_Piece is Knight )
			{
				WayPoint end = new WayPoint();
				WayPoint start = new WayPoint();

				end.MoveToWorld( m_NextMove, Map );

				// This is a knight, so do L shaped move
				int dx = to.X - X;
				int dy = to.Y - Y;

				Point3D p = Location; // Point3D is a value type

				if ( Math.Abs( dx ) == 1 )
					p.X += dx;
				else
					p.Y += dy;

				start.MoveToWorld( p, Map );
				start.NextPoint = end;

				CurrentWayPoint = start;

				m_WayPoints.Add( start );
				m_WayPoints.Add( end );
			}
			else
			{
				WayPoint wp = new WayPoint();
				wp.MoveToWorld( m_NextMove, Map );
				CurrentWayPoint = wp;

				m_WayPoints.Add( wp );
			}

			Paralyzed = false;
		}

		protected override void OnLocationChange(Point3D oldLocation)
		{
			if ( m_NextMove == Point3D.Zero || m_NextMove != Location )
				return;

			// The NPC is at the waypoint
			AI = AIType.AI_Use_Default;

			CurrentWayPoint = null;
			Paralyzed = true;

			foreach( WayPoint wp in m_WayPoints )
				wp.Delete();

			m_WayPoints.Clear();

			m_NextMove = Point3D.Zero;

			Direction = m_Piece.Facing;

			m_Piece.OnMoveOver();

			Server.Timer.DelayCall( TimeSpan.FromMilliseconds( 500 ), TimeSpan.FromMilliseconds( 500 ), 1, new TimerStateCallback ( OnFacingTimer ), null );
		}

		private void OnFacingTimer( object state )
		{
			if ( ! Deleted && m_Piece != null )
			{
				Direction = m_Piece.Facing;
			}
		}

		#endregion

		public override bool HandlesOnSpeech(Mobile from)
		{
			return false;
		}

		public override void OnDelete()
		{
			if ( m_Piece != null )
				m_Piece.OnPieceDeleted();

			CurrentWayPoint = null;

			if ( m_WayPoints != null && m_WayPoints.Count > 0 )
			{
				foreach( WayPoint wp in m_WayPoints )
					wp.Delete();

				m_WayPoints.Clear();
			}

			base.OnDelete ();
		}

		public override bool OnMoveOver(Mobile m)
		{
			return true;
		}

		public override bool CanPaperdollBeOpenedBy(Mobile from)
		{
			return false;
		}
	}
}