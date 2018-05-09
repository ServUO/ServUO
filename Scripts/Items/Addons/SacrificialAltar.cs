using System;

namespace Server.Items
{
    [FlipableAddon(Direction.South, Direction.East)]
    public class SacrificialAltarAddon : BaseAddonContainer
    {
        private Timer m_Timer;
        [Constructable]
        public SacrificialAltarAddon()
            : base(0x2A9B)
        {
            this.Direction = Direction.South;

            this.AddComponent(new LocalizedContainerComponent(0x2A9A, 1074818), 1, 0, 0);
        }

        public SacrificialAltarAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainerDeed Deed
        {
            get
            {
                return new SacrificialAltarDeed();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1074818;
            }
        }// Sacrificial Altar
        public override int DefaultMaxWeight
        {
            get
            {
                return 0;
            }
        }
        public override int DefaultGumpID
        {
            get
            {
                return 0x107;
            }
        }
        public override int DefaultDropSound
        {
            get
            {
                return 0x42;
            }
        }
        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!base.OnDragDrop(from, dropped))
                return false;

            if (this.TotalItems >= 50)
            {
                this.SendLocalizedMessageTo(from, 501478); // The trash is full!  Emptying!
                this.Empty();
            }
            else
            {
                this.SendLocalizedMessageTo(from, 1010442); // The item will be deleted in three minutes

                if (this.m_Timer != null)
                    this.m_Timer.Stop();

                this.m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(3), new TimerCallback(Empty));
            }

            return true;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!base.OnDragDropInto(from, item, p))
                return false;

            if (this.TotalItems >= 50)
            {
                this.SendLocalizedMessageTo(from, 501478); // The trash is full!  Emptying!
                this.Empty();
            }
            else
            {
                this.SendLocalizedMessageTo(from, 1010442); // The item will be deleted in three minutes

                if (this.m_Timer != null)
                    this.m_Timer.Stop();

                this.m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(3), new TimerCallback(Empty));
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

            if (this.Items.Count > 0)
                this.m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(3), new TimerCallback(Empty));
        }

        public virtual void Flip(Mobile from, Direction direction)
        {
            switch ( direction )
            {
                case Direction.East:
                    this.ItemID = 0x2A9C;
                    this.AddComponent(new LocalizedContainerComponent(0x2A9D, 1074818), 0, -1, 0);
                    break;
                case Direction.South:
                    this.ItemID = 0x2A9B;
                    this.AddComponent(new LocalizedContainerComponent(0x2A9A, 1074818), 1, 0, 0);
                    break;
            }
        }

        public virtual void Empty()
        {
            if (this.Items.Count > 0)
            {
                Point3D location = this.Location;
                location.Z += 10;

                Effects.SendLocationEffect(location, this.Map, 0x3709, 10, 10, 0x356, 0);
                Effects.PlaySound(location, this.Map, 0x32E);

                for (int i = this.Items.Count - 1; i >= 0; --i)
                {
                    if (i >= this.Items.Count)
                        continue;

                    this.Items[i].Delete();
                }              
            }

            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;
        }
    }

    public class SacrificialAltarDeed : BaseAddonContainerDeed
    {
        [Constructable]
        public SacrificialAltarDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public SacrificialAltarDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainer Addon
        {
            get
            {
                return new SacrificialAltarAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1074818;
            }
        }// Sacrificial Altar
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