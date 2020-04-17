namespace Server.Items
{
    public class RusticBenchEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new RusticBenchEastDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public RusticBenchEastAddon()
        {
            AddComponent(new AddonComponent(0x0E53), 0, 0, 0);
            AddComponent(new AddonComponent(0x0E52), 0, 1, 0);
        }

        public RusticBenchEastAddon(Serial serial)
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

    public class RusticBenchEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new RusticBenchEastAddon();
        public override int LabelNumber => 1150594;  // rustic bench (east)

        [Constructable]
        public RusticBenchEastDeed()
        {
        }

        public RusticBenchEastDeed(Serial serial)
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