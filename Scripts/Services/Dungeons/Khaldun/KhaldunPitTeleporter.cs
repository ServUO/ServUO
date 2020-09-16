namespace Server.Items
{
    public class KhaldunPitTeleporter : Item
    {
        private bool m_Active;
        private Point3D m_PointDest;
        private Map m_MapDest;
        [Constructable]
        public KhaldunPitTeleporter()
            : this(new Point3D(5451, 1374, 0), Map.Felucca)
        {
        }

        [Constructable]
        public KhaldunPitTeleporter(Point3D pointDest, Map mapDest)
            : base(0x053B)
        {
            Movable = false;
            Hue = 1;

            m_Active = true;
            m_PointDest = pointDest;
            m_MapDest = mapDest;
        }

        public KhaldunPitTeleporter(Serial serial)
            : base(serial)
        {
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (Map != null && Map != Map.Internal && m_MapDest != Map)
            {
                m_MapDest = Map;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get
            {
                return m_Active;
            }
            set
            {
                m_Active = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D PointDest
        {
            get
            {
                return m_PointDest;
            }
            set
            {
                m_PointDest = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Map MapDest
        {
            get
            {
                return m_MapDest;
            }
            set
            {
                m_MapDest = value;
            }
        }
        public override int LabelNumber => 1016511;// the floor of the cavern seems to have collapsed here - a faint light is visible at the bottom of the pit
        public override void OnDoubleClick(Mobile m)
        {
            if (!m_Active)
                return;

            Map map = m_MapDest;

            if (map == null || map == Map.Internal)
                map = m.Map;

            Point3D p = m_PointDest;

            if (p == Point3D.Zero)
                p = m.Location;

            if (m.InRange(this, 3))
            {
                Mobiles.BaseCreature.TeleportPets(m, m_PointDest, m_MapDest);

                m.MoveToWorld(m_PointDest, m_MapDest);
            }
            else
            {
                m.SendLocalizedMessage(1019045); // I can't reach that.
            }
        }

        public override void OnDoubleClickDead(Mobile m)
        {
            OnDoubleClick(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            writer.Write(m_Active);
            writer.Write(m_PointDest);
            writer.Write(m_MapDest);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Active = reader.ReadBool();
            m_PointDest = reader.ReadPoint3D();
            m_MapDest = reader.ReadMap();

            if (version == 0 && m_MapDest != Map)
            {
                m_MapDest = Map;
            }
        }
    }
}
