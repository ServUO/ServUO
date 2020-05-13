using Server.Mobiles;

namespace Server.Items
{
    public class MysteriousTunnel : Item
    {
        public override int LabelNumber => 1152265;  // mysterious tunnel       
        private Point3D m_PointDest;

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D PointDest
        {
            get { return m_PointDest; }
            set { m_PointDest = value; }
        }

        [Constructable]
        public MysteriousTunnel()
            : base(0x1B71)
        {
            Movable = false;
        }

        public MysteriousTunnel(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m is PlayerMobile)
            {
                Point3D loc = PointDest;
                m.MoveToWorld(loc, Map);
                BaseCreature.TeleportPets(m, loc, Map);

                return false;
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_PointDest);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_PointDest = reader.ReadPoint3D();
        }
    }
}