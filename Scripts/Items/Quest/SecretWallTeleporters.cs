using System;

namespace Server.Items
{
    public class SecretWall : Item
    {
        private Point3D m_PointDest;
        private Map m_MapDest;
        private bool m_Locked;
        private bool m_Active;
        [Constructable]
        public SecretWall(int itemID)
            : base(itemID)
        {
            m_Active = true;
            m_Locked = true;
        }

        public SecretWall(Serial serial)
            : base(serial)
        {
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
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Locked
        {
            get
            {
                return m_Locked;
            }
            set
            {
                m_Locked = value;
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
        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 2))
            {
                if (!m_Locked && m_Active)
                {
                    from.MoveToWorld(m_PointDest, m_MapDest);
                    from.SendLocalizedMessage(1072790); // The wall becomes transparent, and you push your way through it.
                }
                else
                    from.Say(502684); // This door appears to be locked.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_PointDest);
            writer.Write(m_MapDest);
            writer.Write(m_Locked);
            writer.Write(m_Active);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_PointDest = reader.ReadPoint3D();
            m_MapDest = reader.ReadMap();
            m_Locked = reader.ReadBool();
            m_Active = reader.ReadBool();
        }
    }

    public class SecretSwitch : Item
    {
        private SecretWall m_Wall;
        private bool m_TurnedOn;
        [Constructable]
        public SecretSwitch()
            : this(0x108F, null)
        {
        }

        [Constructable]
        public SecretSwitch(int itemID, SecretWall wall)
            : base(itemID)
        {
            m_Wall = wall;
        }

        public SecretSwitch(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecretWall Wall
        {
            get
            {
                return m_Wall;
            }
            set
            {
                m_Wall = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool TurnedOn
        {
            get
            {
                return m_TurnedOn;
            }
            set
            {
                m_TurnedOn = value;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 2) && m_Wall != null)
            {
                if (m_TurnedOn)
                    ItemID -= 1;
                else
                {
                    ItemID += 1;

                    Timer.DelayCall(TimeSpan.FromSeconds(10), Lock);
                }

                m_TurnedOn = !m_TurnedOn;
                m_Wall.Locked = !m_Wall.Locked;

                if (Utility.RandomBool())
                {
                    Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, EffectItem.DefaultDuration), 0x36B0, 1, 14, 63, 7, 9915, 0);
                    Effects.PlaySound(from.Location, from.Map, 0x229);

                    AOS.Damage(from, Utility.Random(4, 5), 0, 0, 0, 100, 0);
                }

                from.SendLocalizedMessage(1072739); // You hear a click behind the wall.
                from.PlaySound(0x3E5);
            }
        }

        public virtual void Lock()
        {
            if (m_Wall != null)
            {
                if (m_TurnedOn)
                    ItemID -= 1;

                m_TurnedOn = false;
                m_Wall.Locked = true;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_Wall);
            writer.Write(m_TurnedOn);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Wall = reader.ReadItem() as SecretWall;
            m_TurnedOn = reader.ReadBool();
        }
    }
}
