namespace Server.Items
{
    [Furniture]
    public class FancyCouchSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new FancyCouchSouthDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public FancyCouchSouthAddon()
        {
            AddComponent(new AddonComponent(0x4C8D), 0, 0, 0);
            AddComponent(new AddonComponent(0x4C8E), -1, 0, 0);
            AddComponent(new AddonComponent(0x4C8F), 1, 0, 0);
        }

        public FancyCouchSouthAddon(Serial serial)
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

    public class FancyCouchSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new FancyCouchSouthAddon();
        public override int LabelNumber => 1154139;  // Fancy Couch (South)

        [Constructable]
        public FancyCouchSouthDeed()
        {
        }

        public FancyCouchSouthDeed(Serial serial)
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