namespace Server.Items
{
    public class StoneAnvilEastAddon : BaseAddon
    {
        [Constructable]
        public StoneAnvilEastAddon()
        {
            AddComponent(new AnvilComponent(0x2DD6), 0, 0, 0);
        }

        public StoneAnvilEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new StoneAnvilEastDeed();
        public override bool RetainDeedHue => true;
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

    public class StoneAnvilEastDeed : BaseAddonDeed
    {
        [Constructable]
        public StoneAnvilEastDeed()
        {
        }

        public StoneAnvilEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new StoneAnvilEastAddon();
        public override int LabelNumber => 1073392;// stone anvil (east)
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