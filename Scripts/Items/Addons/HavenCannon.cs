using Server.Items;

namespace Server.Engines.Quests.Haven
{
    public enum CannonDirection
    {
        North,
        East,
        South,
        West
    }

    public class Cannon : BaseAddon
    {
        private CannonDirection m_CannonDirection;
        private MilitiaCanoneer m_Canoneer;

        [Constructable]
        public Cannon(CannonDirection direction)
        {
            m_CannonDirection = direction;

            switch (direction)
            {
                case CannonDirection.North:
                    {
                        AddComponent(new CannonComponent(0xE8D), 0, 0, 0);
                        AddComponent(new CannonComponent(0xE8C), 0, 1, 0);
                        AddComponent(new CannonComponent(0xE8B), 0, 2, 0);

                        break;
                    }
                case CannonDirection.East:
                    {
                        AddComponent(new CannonComponent(0xE96), 0, 0, 0);
                        AddComponent(new CannonComponent(0xE95), -1, 0, 0);
                        AddComponent(new CannonComponent(0xE94), -2, 0, 0);

                        break;
                    }
                case CannonDirection.South:
                    {
                        AddComponent(new CannonComponent(0xE91), 0, 0, 0);
                        AddComponent(new CannonComponent(0xE92), 0, -1, 0);
                        AddComponent(new CannonComponent(0xE93), 0, -2, 0);

                        break;
                    }
                default:
                    {
                        AddComponent(new CannonComponent(0xE8E), 0, 0, 0);
                        AddComponent(new CannonComponent(0xE8F), 1, 0, 0);
                        AddComponent(new CannonComponent(0xE90), 2, 0, 0);

                        break;
                    }
            }
        }

        public Cannon(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CannonDirection CannonDirection => m_CannonDirection;
        [CommandProperty(AccessLevel.GameMaster)]
        public MilitiaCanoneer Canoneer
        {
            get
            {
                return m_Canoneer;
            }
            set
            {
                m_Canoneer = value;
            }
        }
        public override bool HandlesOnMovement => m_Canoneer != null && !m_Canoneer.Deleted && m_Canoneer.Active;
        public void DoFireEffect(IPoint3D target)
        {
            Point3D from;
            switch (m_CannonDirection)
            {
                case CannonDirection.North:
                    from = new Point3D(X, Y - 1, Z);
                    break;
                case CannonDirection.East:
                    from = new Point3D(X + 1, Y, Z);
                    break;
                case CannonDirection.South:
                    from = new Point3D(X, Y + 1, Z);
                    break;
                default:
                    from = new Point3D(X - 1, Y, Z);
                    break;
            }

            Effects.SendLocationEffect(from, Map, 0x36B0, 16, 1);
            Effects.PlaySound(from, Map, 0x11D);

            Effects.SendLocationEffect(target, Map, 0x36B0, 16, 1);
            Effects.PlaySound(target, Map, 0x11D);
        }

        public void Fire(Mobile from, Mobile target)
        {
            DoFireEffect(target);

            target.Damage(9999, from);

            if (target.Corpse != null)
                target.Corpse.Delete();
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m_Canoneer == null || m_Canoneer.Deleted || !m_Canoneer.Active)
                return;

            bool canFire;
            switch (m_CannonDirection)
            {
                case CannonDirection.North:
                    canFire = m.X >= X - 7 && m.X <= X + 7 && m.Y == Y - 7 && oldLocation.Y < Y - 7;
                    break;
                case CannonDirection.East:
                    canFire = m.Y >= Y - 7 && m.Y <= Y + 7 && m.X == X + 7 && oldLocation.X > X + 7;
                    break;
                case CannonDirection.South:
                    canFire = m.X >= X - 7 && m.X <= X + 7 && m.Y == Y + 7 && oldLocation.Y > Y + 7;
                    break;
                default:
                    canFire = m.Y >= Y - 7 && m.Y <= Y + 7 && m.X == X - 7 && oldLocation.X < X - 7;
                    break;
            }

            if (canFire && m_Canoneer.WillFire(this, m))
                Fire(m_Canoneer, m);
        }

        public override void Serialize(GenericWriter writer)
        {
            if (m_Canoneer != null && m_Canoneer.Deleted)
                m_Canoneer = null;

            base.Serialize(writer);

            writer.Write(0); // version

            writer.WriteEncodedInt((int)m_CannonDirection);
            writer.Write(m_Canoneer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_CannonDirection = (CannonDirection)reader.ReadEncodedInt();
            m_Canoneer = (MilitiaCanoneer)reader.ReadMobile();
        }
    }

    public class CannonComponent : AddonComponent
    {
        public CannonComponent(int itemID)
            : base(itemID)
        {
        }

        public CannonComponent(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public MilitiaCanoneer Canoneer
        {
            get
            {
                return Addon is Cannon ? ((Cannon)Addon).Canoneer : null;
            }
            set
            {
                if (Addon is Cannon)
                    ((Cannon)Addon).Canoneer = value;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}