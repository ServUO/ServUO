﻿namespace Server.Items
{
    public class ShipLadder : Item
    {
        private Point3D m_PointDest;
        private Map m_Map;

        [Constructable]
        public ShipLadder(Point3D point, Map map, int id)
            : base(0x08A6)
        {
            ItemID = id;
            m_PointDest = point;
            m_Map = map;
            Movable = false;
        }

        public ShipLadder(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClickDead(Mobile from)
        {
            if (from.InRange(Location, 2))
            {
                Mobiles.BaseCreature.TeleportPets(from, m_PointDest, from.Map);
                from.MoveToWorld(m_PointDest, m_Map);
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.Alive)
                return;

            if (from.InRange(Location, 2))
            {
                Mobiles.BaseCreature.TeleportPets(from, m_PointDest, from.Map);
                from.MoveToWorld(m_PointDest, m_Map);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_PointDest);
            writer.Write(m_Map);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_PointDest = reader.ReadPoint3D();
            m_Map = reader.ReadMap();
        }
    }
}