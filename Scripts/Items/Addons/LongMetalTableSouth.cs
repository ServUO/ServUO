namespace Server.Items
{
    public class LongMetalTableSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new LongMetalTableSouthDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public LongMetalTableSouthAddon()
        {
            AddComponent(new AddonComponent(0x4CC1), -1, 1, 0);
            AddComponent(new AddonComponent(0x4CBD), -1, 0, 0);
            AddComponent(new AddonComponent(0x4CBE), 0, 0, 0);
            AddComponent(new AddonComponent(0x4CC0), 0, 1, 0);
            AddComponent(new AddonComponent(0x4CBC), 1, 0, 0);
            AddComponent(new AddonComponent(0x4CBF), 1, 1, 0);
        }

        public LongMetalTableSouthAddon(Serial serial)
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

    public class LongMetalTableSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new LongMetalTableSouthAddon();
        public override int LabelNumber => 1154164;  // Long Metal Table (South)

        [Constructable]
        public LongMetalTableSouthDeed()
        {
        }

        public LongMetalTableSouthDeed(Serial serial)
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