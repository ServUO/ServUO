namespace Server.Items
{
    [Furniture]
    public class FancyCouchNorthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new FancyCouchNorthDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public FancyCouchNorthAddon()
        {
            AddComponent(new AddonComponent(0x9C62), -1, 0, 0);
            AddComponent(new AddonComponent(0x9C61), 0, 0, 0);
            AddComponent(new AddonComponent(0x9C60), 1, 0, 0);
        }

        public FancyCouchNorthAddon(Serial serial)
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

    public class FancyCouchNorthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new FancyCouchNorthAddon();
        public override int LabelNumber => 1156582;  // Fancy Couch (North)

        [Constructable]
        public FancyCouchNorthDeed()
        {
        }

        public FancyCouchNorthDeed(Serial serial)
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