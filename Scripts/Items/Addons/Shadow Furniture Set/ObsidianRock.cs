namespace Server.Items
{
    public class ObsidianRockAddon : BaseAddon
    {
        [Constructable]
        public ObsidianRockAddon()
        {
            AddComponent(new LocalizedAddonComponent(0x364E, 1076677), 0, 0, 0);
        }

        public ObsidianRockAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new ObsidianRockDeed();

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

    public class ObsidianRockDeed : BaseAddonDeed
    {
        public override int LabelNumber => 1076677;  // Obsidian Rock

        public override bool ExcludeDeedHue => true;

        [Constructable]
        public ObsidianRockDeed()
        {
            LootType = LootType.Blessed;
            Hue = 1908;
        }

        public ObsidianRockDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new ObsidianRockAddon();


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
