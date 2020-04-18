namespace Server.Items
{
    public class ObsidianPillarAddon : BaseAddon
    {
        [Constructable]
        public ObsidianPillarAddon()
        {
            AddComponent(new LocalizedAddonComponent(0x364F, 1076678), 0, 0, 0);
        }

        public ObsidianPillarAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new ObsidianPillarDeed();

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

    public class ObsidianPillarDeed : BaseAddonDeed
    {
        public override int LabelNumber => 1076678;  // Obsidian Pillar

        public override bool ExcludeDeedHue => true;

        [Constructable]
        public ObsidianPillarDeed()
        {
            LootType = LootType.Blessed;
            Hue = 1908;
        }

        public ObsidianPillarDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new ObsidianPillarAddon();


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
