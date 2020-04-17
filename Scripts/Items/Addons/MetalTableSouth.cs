namespace Server.Items
{
    public class MetalTableSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new MetalTableSouthDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public MetalTableSouthAddon()
        {
            AddComponent(new AddonComponent(0x4CB4), 0, 0, 0);
            AddComponent(new AddonComponent(0x4CB7), 0, 1, 0);
            AddComponent(new AddonComponent(0x4CB5), 1, 0, 0);
            AddComponent(new AddonComponent(0x4CB6), 1, 1, 0);
        }

        public MetalTableSouthAddon(Serial serial)
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

    public class MetalTableSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new MetalTableSouthAddon();
        public override int LabelNumber => 1154154;  // Metal Table (South)

        [Constructable]
        public MetalTableSouthDeed()
        {
        }

        public MetalTableSouthDeed(Serial serial)
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