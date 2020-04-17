namespace Server.Items
{
    [Furniture]
    public class FancyCouchEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new FancyCouchEastDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public FancyCouchEastAddon()
        {
            AddComponent(new AddonComponent(0x4C8C), 0, -1, 0);
            AddComponent(new AddonComponent(0x4C8A), 0, 0, 0);
            AddComponent(new AddonComponent(0x4C8B), 0, 1, 0);
        }

        public FancyCouchEastAddon(Serial serial)
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

    public class FancyCouchEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new FancyCouchEastAddon();
        public override int LabelNumber => 1154140;  // Fancy Couch (East)

        [Constructable]
        public FancyCouchEastDeed()
        {
        }

        public FancyCouchEastDeed(Serial serial)
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