namespace Server.Items
{
    public class GlobeOfSosariaAddon : BaseAddon
    {
        [Constructable]
        public GlobeOfSosariaAddon()
        {
            AddComponent(new LocalizedAddonComponent(0x3657, 1076681), 1, 1, 0);
            AddComponent(new LocalizedAddonComponent(0x3658, 1076681), 0, 1, 0);
            AddComponent(new LocalizedAddonComponent(0x3659, 1076681), 1, 0, 0);
            AddComponent(new LocalizedAddonComponent(0x3661, 1076681), 1, 1, 0);
        }

        public GlobeOfSosariaAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new GlobeOfSosariaDeed();

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

    public class GlobeOfSosariaDeed : BaseAddonDeed
    {
        public override int LabelNumber => 1076681;  // Globe of Sosaria

        public override bool ExcludeDeedHue => true;

        [Constructable]
        public GlobeOfSosariaDeed()
        {
            LootType = LootType.Blessed;
            Hue = 1908;
        }

        public GlobeOfSosariaDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new GlobeOfSosariaAddon();

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
