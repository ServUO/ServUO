namespace Server.Items
{
    public class LongWoodenTableEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new LongWoodenTableEastDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public LongWoodenTableEastAddon()
        {
            AddComponent(new AddonComponent(0x4CD3), 1, -1, 0);
            AddComponent(new AddonComponent(0x4CD6), 0, -1, 0);
            AddComponent(new AddonComponent(0x4CD7), 0, 0, 0);
            AddComponent(new AddonComponent(0x4CD2), 1, 0, 0);
            AddComponent(new AddonComponent(0x4CD5), 0, 1, 0);
            AddComponent(new AddonComponent(0x4CD4), 1, 1, 0);
        }

        public LongWoodenTableEastAddon(Serial serial)
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

    public class LongWoodenTableEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new LongWoodenTableEastAddon();
        public override int LabelNumber => 1154167;  // Long Wooden Table (East)

        [Constructable]
        public LongWoodenTableEastDeed()
        {
        }

        public LongWoodenTableEastDeed(Serial serial)
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