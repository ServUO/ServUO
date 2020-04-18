namespace Server.Items
{
    public class RusticBenchSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new RusticBenchSouthDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public RusticBenchSouthAddon()
        {
            AddComponent(new AddonComponent(0x0E50), 0, 0, 0);
            AddComponent(new AddonComponent(0x0E51), 1, 0, 0);
        }

        public RusticBenchSouthAddon(Serial serial)
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

    public class RusticBenchSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new RusticBenchSouthAddon();
        public override int LabelNumber => 1150593;  // rustic bench (south)

        [Constructable]
        public RusticBenchSouthDeed()
        {
        }

        public RusticBenchSouthDeed(Serial serial)
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