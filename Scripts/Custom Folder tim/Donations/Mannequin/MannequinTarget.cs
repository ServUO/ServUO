using System.Text;
using Server.Targeting;
using Server.Multis;
using Server.Mobiles;
using Server.Gumps;
using Server.Items;

namespace Server.Misc
{
	public class MannequinTarget : Target
	{
		private Mobile m_From;
		private Mannequin m_Man;

		public MannequinTarget( Mobile from, Mannequin man ) : base( -1, true, TargetFlags.None )
		{
			m_From = from;
			m_Man = man;
			CheckLOS = false;
		}

		protected override void OnTarget( Mobile from, object targeted )
		{
			IPoint3D t = targeted as IPoint3D;
			if( t == null )
				return;

			Point3D loc = new Point3D( t );

			m_Man.AI = AIType.AI_Melee;
			m_Man.m_NextMove = loc;

			WayPoint GoHere = new WayPoint();
			GoHere.Map = from.Map;
			GoHere.Location = loc;
			m_Man.CurrentWayPoint = GoHere;
			m_Man.CantWalk = false;
			m_Man.Say( "I will gladly move here for you Master" );
			m_Man.m_WayPoints.Add( GoHere );
			from.SendGump( new MannequinControl( m_Man, from, 1 ) );
		}
	}
}