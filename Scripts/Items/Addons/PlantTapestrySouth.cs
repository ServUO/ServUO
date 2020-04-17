namespace Server.Items
{
    public class PlantTapestrySouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new PlantTapestrySouthDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public PlantTapestrySouthAddon()
        {
            AddComponent(new AddonComponent(0x4C9C), 0, 0, 0);
            AddComponent(new AddonComponent(0x4C9D), 1, 0, 0);
        }

        public PlantTapestrySouthAddon(Serial serial)
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

    public class PlantTapestrySouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new PlantTapestrySouthAddon();
        public override int LabelNumber => 1154146;  // Plant Tapestry (South)

        [Constructable]
        public PlantTapestrySouthDeed()
        {
        }

        public PlantTapestrySouthDeed(Serial serial)
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