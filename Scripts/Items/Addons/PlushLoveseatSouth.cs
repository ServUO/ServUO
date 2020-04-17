namespace Server.Items
{
    [Furniture]
    public class PlushLoveseatSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new PlushLoveseatSouthDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public PlushLoveseatSouthAddon()
        {
            AddComponent(new AddonComponent(0x4C83), 0, 0, 0);
            AddComponent(new AddonComponent(0x4C82), 1, 0, 0);
        }

        public PlushLoveseatSouthAddon(Serial serial)
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

    public class PlushLoveseatSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new PlushLoveseatSouthAddon();
        public override int LabelNumber => 1154135;  // Plush Loveseat (South)

        [Constructable]
        public PlushLoveseatSouthDeed()
        {
        }

        public PlushLoveseatSouthDeed(Serial serial)
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