namespace Server.Items
{
    [Furniture]
    public class FancyLoveseatSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new FancyLoveseatSouthDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public FancyLoveseatSouthAddon()
        {
            AddComponent(new AddonComponent(0x4C87), 0, 0, 0);
            AddComponent(new AddonComponent(0x4C86), 1, 0, 0);
        }

        public FancyLoveseatSouthAddon(Serial serial)
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

    public class FancyLoveseatSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new FancyLoveseatSouthAddon();
        public override int LabelNumber => 1154137;  // Fancy Loveseat (South)

        [Constructable]
        public FancyLoveseatSouthDeed()
        {
        }

        public FancyLoveseatSouthDeed(Serial serial)
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