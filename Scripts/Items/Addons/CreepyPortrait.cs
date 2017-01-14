using System;
using Server.Network;

namespace Server.Items
{
    [Flipable(0x2A69, 0x2A6D)]
    public class CreepyPortraitComponent : AddonComponent
    {
        public CreepyPortraitComponent()
            : base(0x2A69)
        {
        }

        public CreepyPortraitComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074481;
            }
        }// Creepy portrait
        public override bool HandlesOnMovement
        {
            get
            {
                return true;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (Utility.InRange(this.Location, from.Location, 2))
                Effects.PlaySound(this.Location, this.Map, Utility.RandomMinMax(0x565, 0x566));
            else
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
        }

        public override void OnMovement(Mobile m, Point3D old)
        {
            if (m.Alive && m.Player && (m.IsPlayer() || !m.Hidden))
            {
                if (!Utility.InRange(old, this.Location, 2) && Utility.InRange(m.Location, this.Location, 2))
                {
                    if (this.ItemID == 0x2A69 || this.ItemID == 0x2A6D)
                    {
                        this.Up();
                        Timer.DelayCall(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5), 2, new TimerCallback(Up));
                    }
                }
                else if (Utility.InRange(old, this.Location, 2) && !Utility.InRange(m.Location, this.Location, 2))
                {
                    if (this.ItemID == 0x2A6C || this.ItemID == 0x2A70)
                    {
                        this.Down();
                        Timer.DelayCall(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5), 2, new TimerCallback(Down));
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
			
            if (version == 0 && this.ItemID != 0x2A69 && this.ItemID != 0x2A6D)
                this.ItemID = 0x2A69;
        }

        private void Up()
        {
            this.ItemID += 1;
        }

        private void Down()
        {
            this.ItemID -= 1;
        }
    }

    public class CreepyPortraitAddon : BaseAddon
    {
        [Constructable]
        public CreepyPortraitAddon()
            : base()
        {
            this.AddComponent(new CreepyPortraitComponent(), 0, 0, 0);
        }

        public CreepyPortraitAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new CreepyPortraitDeed();
            }
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

    public class CreepyPortraitDeed : BaseAddonDeed
    {
        [Constructable]
        public CreepyPortraitDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public CreepyPortraitDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new CreepyPortraitAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1074481;
            }
        }// Creepy portrait
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