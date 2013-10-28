using System;

namespace Server.Items
{
    public class LargeStoneTableSouthAddon : BaseAddon
    {
        [Constructable]
        public LargeStoneTableSouthAddon()
            : this(0)
        {
        }

        [Constructable]
        public LargeStoneTableSouthAddon(int hue)
        {
            this.AddComponent(new AddonComponent(0x1205), 0, 0, 0);
            this.AddComponent(new AddonComponent(0x1206), 1, 0, 0);
            this.AddComponent(new AddonComponent(0x1204), 2, 0, 0);
            this.Hue = hue;
        }

        public LargeStoneTableSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new LargeStoneTableSouthDeed();
            }
        }
        public override bool RetainDeedHue
        {
            get
            {
                return true;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class LargeStoneTableSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public LargeStoneTableSouthDeed()
        {
        }

        public LargeStoneTableSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new LargeStoneTableSouthAddon(this.Hue);
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1044512;
            }
        }// large stone table (South)
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