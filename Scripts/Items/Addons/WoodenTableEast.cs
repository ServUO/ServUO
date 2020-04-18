namespace Server.Items
{
    public class WoodenTableEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new WoodenTableEastDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public WoodenTableEastAddon()
        {
            AddComponent(new AddonComponent(0x4CCB), 0, 0, 0);
            AddComponent(new AddonComponent(0x4CCA), 0, 1, 0);
        }

        public WoodenTableEastAddon(Serial serial)
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

    public class WoodenTableEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new WoodenTableEastAddon();
        public override int LabelNumber => 1154157;  // Wooden Table (East)

        [Constructable]
        public WoodenTableEastDeed()
        {
        }

        public WoodenTableEastDeed(Serial serial)
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