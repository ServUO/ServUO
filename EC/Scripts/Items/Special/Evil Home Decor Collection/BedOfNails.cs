using System;

namespace Server.Items
{
    public class BedOfNailsComponent : AddonComponent
    {
        public BedOfNailsComponent(int itemID)
            : base(itemID)
        {
        }

        public BedOfNailsComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074801;
            }
        }// Bed of Nails
        public override bool OnMoveOver(Mobile m)
        {
            bool allow = base.OnMoveOver(m);

            if (allow && this.Addon is BedOfNailsAddon)
                ((BedOfNailsAddon)this.Addon).OnMoveOver(m);

            return allow;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    [FlipableAddon(Direction.South, Direction.East)]
    public class BedOfNailsAddon : BaseAddon
    {
        private InternalTimer m_Timer;
        [Constructable]
        public BedOfNailsAddon()
            : base()
        {
            this.Direction = Direction.South;

            this.AddComponent(new BedOfNailsComponent(0x2A81), 0, 0, 0);
            this.AddComponent(new BedOfNailsComponent(0x2A82), 0, -1, 0);
        }

        public BedOfNailsAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new BedOfNailsDeed();
            }
        }
        public override bool OnMoveOver(Mobile m)
        {
            if (m.Alive && (m.IsPlayer() || !m.Hidden))
            {
                if (m.Player)
                {
                    if (m.Female)
                        Effects.PlaySound(this.Location, this.Map, Utility.RandomMinMax(0x53B, 0x53D));
                    else
                        Effects.PlaySound(this.Location, this.Map, Utility.RandomMinMax(0x53E, 0x540));
                }

                if (this.m_Timer == null || !this.m_Timer.Running)
                    (this.m_Timer = new InternalTimer(m)).Start();
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        public virtual void Flip(Mobile from, Direction direction)
        {
            switch( direction )
            {
                case Direction.East:
                    this.AddComponent(new BedOfNailsComponent(0x2A89), 0, 0, 0);
                    this.AddComponent(new BedOfNailsComponent(0x2A8A), -1, 0, 0);
                    break;
                case Direction.South:
                    this.AddComponent(new BedOfNailsComponent(0x2A81), 0, 0, 0);
                    this.AddComponent(new BedOfNailsComponent(0x2A82), 0, -1, 0);
                    break;
            }
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private Point3D m_Location;
            public InternalTimer(Mobile m)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(1), 5)
            {
                this.m_Mobile = m;
                this.m_Location = Point3D.Zero;
            }

            protected override void OnTick()
            {
                if (this.m_Mobile == null || this.m_Mobile.Map == null || this.m_Mobile.Deleted || !this.m_Mobile.Alive || this.m_Mobile.Map == Map.Internal)
                {
                    this.Stop();
                }
                else
                {
                    if (this.m_Location != this.m_Mobile.Location)
                    {
                        int amount = Utility.RandomMinMax(0, 7);

                        for (int i = 0; i < amount; i++)
                        {
                            int x = this.m_Mobile.X + Utility.RandomMinMax(-1, 1);
                            int y = this.m_Mobile.Y + Utility.RandomMinMax(-1, 1);
                            int z = this.m_Mobile.Z;

                            if (!this.m_Mobile.Map.CanFit(x, y, z, 1, false, false, true))
                            {
                                z = this.m_Mobile.Map.GetAverageZ(x, y);

                                if (!this.m_Mobile.Map.CanFit(x, y, z, 1, false, false, true))
                                {
                                    continue;
                                }
                            }

                            Blood blood = new Blood(Utility.RandomMinMax(0x122C, 0x122F));
                            blood.MoveToWorld(new Point3D(x, y, z), this.m_Mobile.Map);
                        }
                        this.m_Location = this.m_Mobile.Location;
                    }
                }
            }
        }
    }

    public class BedOfNailsDeed : BaseAddonDeed
    {
        [Constructable]
        public BedOfNailsDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public BedOfNailsDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new BedOfNailsAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1074801;
            }
        }// Bed of Nails
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}