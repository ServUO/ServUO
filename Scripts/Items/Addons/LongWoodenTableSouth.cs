namespace Server.Items
{
    public class LongWoodenTableSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new LongWoodenTableSouthDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public LongWoodenTableSouthAddon()
        {
            AddComponent(new AddonComponent(0x4CCC), -1, 0, 0);
            AddComponent(new AddonComponent(0x4CD1), -1, 1, 0);
            AddComponent(new AddonComponent(0x4CCE), 0, 0, 0);
            AddComponent(new AddonComponent(0x4CCD), 1, 0, 0);
            AddComponent(new AddonComponent(0x4CD0), 0, 1, 0);
            AddComponent(new AddonComponent(0x4CCF), 1, 1, 0);
        }

        public LongWoodenTableSouthAddon(Serial serial)
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

    public class LongWoodenTableSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new LongWoodenTableSouthAddon();
        public override int LabelNumber => 1154166;  // Long Wooden Table (South)

        [Constructable]
        public LongWoodenTableSouthDeed()
        {
        }

        public LongWoodenTableSouthDeed(Serial serial)
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