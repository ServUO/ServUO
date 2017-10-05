using System;

namespace Server.Items
{
    public class WallMountedBellSouthDeed : BaseAddonDeed
    {
        public override int LabelNumber { get { return 1154162; } } // Wall Mounted Bell (South)

        [Constructable]
        public WallMountedBellSouthDeed()
        {
        }

        public WallMountedBellSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon { get { return new WallMountedBellSouthAddon(); } }

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

    public class WallMountedBellSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new WallMountedBellSouthDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public WallMountedBellSouthAddon()
        {
            AddComponent(new InstrumentedAddonComponent(0x4C5C, 0x66D), 0, 0, 10);
        }

        public WallMountedBellSouthAddon(Serial serial)
            : base(serial)
        {
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

    public class WallMountedBellEastDeed : BaseAddonDeed
    {
        public override int LabelNumber { get { return 1154163; } } // Wall Mounted Bell (East)

        [Constructable]
        public WallMountedBellEastDeed()
        {
        }

        public WallMountedBellEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon { get { return new WallMountedBellEastAddon(); } }

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

    public class WallMountedBellEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new WallMountedBellEastDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public WallMountedBellEastAddon()
        {
            AddComponent(new InstrumentedAddonComponent(0x4C5D, 0x66D), 0, 0, 10);
        }

        public WallMountedBellEastAddon(Serial serial)
            : base(serial)
        {
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
}