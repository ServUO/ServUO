namespace Server.Items
{
    [Furniture]
    public class FancyLoveseatEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new FancyLoveseatEastDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public FancyLoveseatEastAddon()
        {
            AddComponent(new AddonComponent(0x4C88), 0, 0, 0);
            AddComponent(new AddonComponent(0x4C89), 0, 1, 0);
        }

        public FancyLoveseatEastAddon(Serial serial)
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

    public class FancyLoveseatEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new FancyLoveseatEastAddon();
        public override int LabelNumber => 1154138;  // Fancy Loveseat (East)

        [Constructable]
        public FancyLoveseatEastDeed()
        {
        }

        public FancyLoveseatEastDeed(Serial serial)
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