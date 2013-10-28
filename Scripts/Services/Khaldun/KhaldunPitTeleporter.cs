using System;

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
            this.Movable = false;
            this.Hue = 1;

            this.m_Active = true;
            this.m_PointDest = pointDest;
            this.m_MapDest = mapDest;
        }

        public KhaldunPitTeleporter(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get
            {
                return this.m_Active;
            }
            set
            {
                this.m_Active = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D PointDest
        {
            get
            {
                return this.m_PointDest;
            }
            set
            {
                this.m_PointDest = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Map MapDest
        {
            get
            {
                return this.m_MapDest;
            }
            set
            {
                this.m_MapDest = value;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1016511;
            }
        }// the floor of the cavern seems to have collapsed here - a faint light is visible at the bottom of the pit
        public override void OnDoubleClick(Mobile m)
        {
            if (!this.m_Active)
                return;

            Map map = this.m_MapDest;

            if (map == null || map == Map.Internal)
                map = m.Map;

            Point3D p = this.m_PointDest;

            if (p == Point3D.Zero)
                p = m.Location;

            if (m.InRange(this, 3))
            {
                Server.Mobiles.BaseCreature.TeleportPets(m, this.m_PointDest, this.m_MapDest);

                m.MoveToWorld(this.m_PointDest, this.m_MapDest);
            }
            else
            {
                m.SendLocalizedMessage(1019045); // I can't reach that.
            }
        }

        public override void OnDoubleClickDead(Mobile m)
        {
            this.OnDoubleClick(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Active);
            writer.Write(this.m_PointDest);
            writer.Write(this.m_MapDest);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Active = reader.ReadBool();
            this.m_PointDest = reader.ReadPoint3D();
            this.m_MapDest = reader.ReadMap();
        }
    }
}