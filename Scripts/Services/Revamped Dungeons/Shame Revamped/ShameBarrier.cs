using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Regions;

namespace Server.Items
{
    public class ShameBarrier : Item
    {
		private Point3D m_PointDest;
		private Map m_MapDest;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D PointDest
		{
			get { return m_PointDest; }
			set { m_PointDest = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Map MapDest
		{
			get { return m_MapDest; }
			set { m_MapDest = value; InvalidateProperties(); }
		}
      
        [Constructable]
        public ShameBarrier() : base(0x49E)
        {
            Movable = false;
            Visible = false;
        }

        public override bool OnMoveOver(Mobile m)
        {			
			if (!m.Alive && m is PlayerMobile)
			{
				DoTeleport(m);
				return false;
			}
			
			return true;        
        }
		
		public virtual void DoTeleport( Mobile m )
		{
			Map map = MapDest;

			if ( map == null || map == Map.Internal )
				map = m.Map;

			Point3D p = PointDest;

			if ( p == Point3D.Zero )
				p = m.Location;
			
			BaseCreature.TeleportPets(m, p, map);
			m.MoveToWorld( p, map );
		}	

        public ShameBarrier(Serial serial): base(serial)
        {
        }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
			
			writer.Write( m_PointDest );
			writer.Write( m_MapDest );
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
			switch ( version )
			{				
				case 0:
				{
					m_PointDest = reader.ReadPoint3D();
					m_MapDest = reader.ReadMap();

					break;
				}
			}

		}
	}
}

	
