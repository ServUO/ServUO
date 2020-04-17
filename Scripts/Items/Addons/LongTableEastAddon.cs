namespace Server.Items
{
    public class LongTableEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new LongTableEastDeed();

        [Constructable]
        public LongTableEastAddon()
        {
            AddComponent(new AddonComponent(0x4031), 0, 0, 0);
            AddComponent(new AddonComponent(0x4032), 0, 1, 0);
        }

        public LongTableEastAddon(Serial serial)
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

    public class LongTableEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new LongTableEastAddon();
        public override int LabelNumber => 1111782;  // long table (east)

        [Constructable]
        public LongTableEastDeed()
        {
        }

        public LongTableEastDeed(Serial serial)
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