namespace Server.Items
{
    public class MetalTableEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new MetalTableEastDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public MetalTableEastAddon()
        {
            AddComponent(new AddonComponent(0x4CBA), 0, 0, 0);
            AddComponent(new AddonComponent(0x4CB9), 0, 1, 0);
            AddComponent(new AddonComponent(0x4CBB), 1, 0, 0);
            AddComponent(new AddonComponent(0x4CB8), 1, 1, 0);
        }

        public MetalTableEastAddon(Serial serial)
            : base(serial)
        {
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

    public class MetalTableEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new MetalTableEastAddon();
        public override int LabelNumber => 1154155;  // Metal Table (East)

        [Constructable]
        public MetalTableEastDeed()
        {
        }

        public MetalTableEastDeed(Serial serial)
            : base(serial)
        {
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