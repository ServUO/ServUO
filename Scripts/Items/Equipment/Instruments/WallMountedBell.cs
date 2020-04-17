namespace Server.Items
{
    public class WallMountedBellSouthDeed : BaseAddonDeed
    {
        public override int LabelNumber => 1154162;  // Wall Mounted Bell (South)

        [Constructable]
        public WallMountedBellSouthDeed()
        {
        }

        public WallMountedBellSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new WallMountedBellSouthAddon();

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
        public override BaseAddonDeed Deed => new WallMountedBellSouthDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public WallMountedBellSouthAddon()
        {
            AddComponent(new InstrumentedAddonComponent(0x4C5C, 0x66C), 0, 0, 10);
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
        public override int LabelNumber => 1154163;  // Wall Mounted Bell (East)

        [Constructable]
        public WallMountedBellEastDeed()
        {
        }

        public WallMountedBellEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new WallMountedBellEastAddon();

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
        public override BaseAddonDeed Deed => new WallMountedBellEastDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public WallMountedBellEastAddon()
        {
            AddComponent(new InstrumentedAddonComponent(0x4C5D, 0x66C), 0, 0, 10);
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