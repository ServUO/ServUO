using System;
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
            this.m_CannonDirection = direction;

            switch ( direction )
            {
                case CannonDirection.North:
                    {
                        this.AddComponent(new CannonComponent(0xE8D), 0, 0, 0);
                        this.AddComponent(new CannonComponent(0xE8C), 0, 1, 0);
                        this.AddComponent(new CannonComponent(0xE8B), 0, 2, 0);

                        break;
                    }
                case CannonDirection.East:
                    {
                        this.AddComponent(new CannonComponent(0xE96), 0, 0, 0);
                        this.AddComponent(new CannonComponent(0xE95), -1, 0, 0);
                        this.AddComponent(new CannonComponent(0xE94), -2, 0, 0);

                        break;
                    }
                case CannonDirection.South:
                    {
                        this.AddComponent(new CannonComponent(0xE91), 0, 0, 0);
                        this.AddComponent(new CannonComponent(0xE92), 0, -1, 0);
                        this.AddComponent(new CannonComponent(0xE93), 0, -2, 0);

                        break;
                    }
                default:
                    {
                        this.AddComponent(new CannonComponent(0xE8E), 0, 0, 0);
                        this.AddComponent(new CannonComponent(0xE8F), 1, 0, 0);
                        this.AddComponent(new CannonComponent(0xE90), 2, 0, 0);

                        break;
                    }
            }
        }

        public Cannon(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CannonDirection CannonDirection
        {
            get
            {
                return this.m_CannonDirection;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public MilitiaCanoneer Canoneer
        {
            get
            {
                return this.m_Canoneer;
            }
            set
            {
                this.m_Canoneer = value;
            }
        }
        public override bool HandlesOnMovement
        {
            get
            {
                return this.m_Canoneer != null && !this.m_Canoneer.Deleted && this.m_Canoneer.Active;
            }
        }
        public void DoFireEffect(IPoint3D target)
        {
            Point3D from;
            switch ( this.m_CannonDirection )
            {
                case CannonDirection.North:
                    from = new Point3D(this.X, this.Y - 1, this.Z);
                    break;
                case CannonDirection.East:
                    from = new Point3D(this.X + 1, this.Y, this.Z);
                    break;
                case CannonDirection.South:
                    from = new Point3D(this.X, this.Y + 1, this.Z);
                    break;
                default:
                    from = new Point3D(this.X - 1, this.Y, this.Z);
                    break;
            }

            Effects.SendLocationEffect(from, this.Map, 0x36B0, 16, 1);
            Effects.PlaySound(from, this.Map, 0x11D);

            Effects.SendLocationEffect(target, this.Map, 0x36B0, 16, 1);
            Effects.PlaySound(target, this.Map, 0x11D);
        }

        public void Fire(Mobile from, Mobile target)
        {
            this.DoFireEffect(target);

            target.Damage(9999, from);
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (this.m_Canoneer == null || this.m_Canoneer.Deleted || !this.m_Canoneer.Active)
                return;

            bool canFire;
            switch ( this.m_CannonDirection )
            {
                case CannonDirection.North:
                    canFire = m.X >= this.X - 7 && m.X <= this.X + 7 && m.Y == this.Y - 7 && oldLocation.Y < this.Y - 7;
                    break;
                case CannonDirection.East:
                    canFire = m.Y >= this.Y - 7 && m.Y <= this.Y + 7 && m.X == this.X + 7 && oldLocation.X > this.X + 7;
                    break;
                case CannonDirection.South:
                    canFire = m.X >= this.X - 7 && m.X <= this.X + 7 && m.Y == this.Y + 7 && oldLocation.Y > this.Y + 7;
                    break;
                default:
                    canFire = m.Y >= this.Y - 7 && m.Y <= this.Y + 7 && m.X == this.X - 7 && oldLocation.X < this.X - 7;
                    break;
            }

            if (canFire && this.m_Canoneer.WillFire(this, m))
                this.Fire(this.m_Canoneer, m);
        }

        public override void Serialize(GenericWriter writer)
        {
            if (this.m_Canoneer != null && this.m_Canoneer.Deleted)
                this.m_Canoneer = null;

            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.WriteEncodedInt((int)this.m_CannonDirection);
            writer.Write((Mobile)this.m_Canoneer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_CannonDirection = (CannonDirection)reader.ReadEncodedInt();
            this.m_Canoneer = (MilitiaCanoneer)reader.ReadMobile();
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
                return this.Addon is Cannon ? ((Cannon)this.Addon).Canoneer : null;
            }
            set
            {
                if (this.Addon is Cannon)
                    ((Cannon)this.Addon).Canoneer = value;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}