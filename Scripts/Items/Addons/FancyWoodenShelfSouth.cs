namespace Server.Items
{
    public class FancyWoodenShelfSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new FancyWoodenShelfSouthDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public FancyWoodenShelfSouthAddon()
        {
            AddComponent(new AddonComponent(0x4C38), 0, 0, 0);
        }

        public FancyWoodenShelfSouthAddon(Serial serial)
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

    public class FancyWoodenShelfSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new FancyWoodenShelfSouthAddon();
        public override int LabelNumber => 1154158;  // Fancy Wooden Shelf (South)

        [Constructable]
        public FancyWoodenShelfSouthDeed()
        {
        }

        public FancyWoodenShelfSouthDeed(Serial serial)
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