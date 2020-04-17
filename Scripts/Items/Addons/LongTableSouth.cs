namespace Server.Items
{
    public class LongTableSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new LongTableSouthDeed();

        [Constructable]
        public LongTableSouthAddon()
        {
            AddComponent(new AddonComponent(0x402F), 0, 0, 0);
            AddComponent(new AddonComponent(0x4030), 1, 0, 0);
        }

        public LongTableSouthAddon(Serial serial)
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

    public class LongTableSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new LongTableSouthAddon();
        public override int LabelNumber => 1111781;  // long table (south)

        [Constructable]
        public LongTableSouthDeed()
        {
        }

        public LongTableSouthDeed(Serial serial)
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