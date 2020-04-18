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

        public override int LabelNumber => 1074801;// Bed of Nails
        public override bool OnMoveOver(Mobile m)
        {
            bool allow = base.OnMoveOver(m);

            if (allow && Addon is BedOfNailsAddon)
                ((BedOfNailsAddon)Addon).OnMoveOver(m);

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
            Direction = Direction.South;

            AddComponent(new BedOfNailsComponent(0x2A81), 0, 0, 0);
            AddComponent(new BedOfNailsComponent(0x2A82), 0, -1, 0);
        }

        public BedOfNailsAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new BedOfNailsDeed();
        public override bool OnMoveOver(Mobile m)
        {
            if (m.Alive && (m.IsPlayer() || !m.Hidden))
            {
                if (m.Player)
                {
                    if (m.Female)
                        Effects.PlaySound(Location, Map, Utility.RandomMinMax(0x53B, 0x53D));
                    else
                        Effects.PlaySound(Location, Map, Utility.RandomMinMax(0x53E, 0x540));
                }

                if (m_Timer == null || !m_Timer.Running)
                    (m_Timer = new InternalTimer(m)).Start();
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
            switch (direction)
            {
                case Direction.East:
                    AddComponent(new BedOfNailsComponent(0x2A89), 0, 0, 0);
                    AddComponent(new BedOfNailsComponent(0x2A8A), -1, 0, 0);
                    break;
                case Direction.South:
                    AddComponent(new BedOfNailsComponent(0x2A81), 0, 0, 0);
                    AddComponent(new BedOfNailsComponent(0x2A82), 0, -1, 0);
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
                m_Mobile = m;
                m_Location = Point3D.Zero;
            }

            protected override void OnTick()
            {
                if (m_Mobile == null || m_Mobile.Map == null || m_Mobile.Deleted || !m_Mobile.Alive || m_Mobile.Map == Map.Internal)
                {
                    Stop();
                }
                else
                {
                    if (m_Location != m_Mobile.Location)
                    {
                        int amount = Utility.RandomMinMax(0, 7);

                        for (int i = 0; i < amount; i++)
                        {
                            int x = m_Mobile.X + Utility.RandomMinMax(-1, 1);
                            int y = m_Mobile.Y + Utility.RandomMinMax(-1, 1);
                            int z = m_Mobile.Z;

                            if (!m_Mobile.Map.CanFit(x, y, z, 1, false, false, true))
                            {
                                z = m_Mobile.Map.GetAverageZ(x, y);

                                if (!m_Mobile.Map.CanFit(x, y, z, 1, false, false, true))
                                {
                                    continue;
                                }
                            }

                            Blood blood = new Blood(Utility.RandomMinMax(0x122C, 0x122F));
                            blood.MoveToWorld(new Point3D(x, y, z), m_Mobile.Map);
                        }
                        m_Location = m_Mobile.Location;
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
            LootType = LootType.Blessed;
        }

        public BedOfNailsDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new BedOfNailsAddon();
        public override int LabelNumber => 1074801;// Bed of Nails
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