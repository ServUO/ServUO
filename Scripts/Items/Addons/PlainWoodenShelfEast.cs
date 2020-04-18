namespace Server.Items
{
    public class PlainWoodenShelfEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new PlainWoodenShelfEastDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public PlainWoodenShelfEastAddon()
        {
            AddComponent(new AddonComponent(0x4C3B), 0, 0, 0);
        }

        public PlainWoodenShelfEastAddon(Serial serial)
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

    public class PlainWoodenShelfEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new PlainWoodenShelfEastAddon();
        public override int LabelNumber => 1154161;  // Plain Wooden Shelf (East)

        [Constructable]
        public PlainWoodenShelfEastDeed()
        {
        }

        public PlainWoodenShelfEastDeed(Serial serial)
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