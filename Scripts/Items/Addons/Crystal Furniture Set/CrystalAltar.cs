namespace Server.Items
{
    public class CrystalAltarAddon : BaseAddon
    {
        [Constructable]
        public CrystalAltarAddon()
        {
            AddComponent(new LocalizedAddonComponent(13826, 1076672), 1, 1, 0);
            AddComponent(new LocalizedAddonComponent(13828, 1076672), 1, 0, 0);
            AddComponent(new LocalizedAddonComponent(15778, 1076672), 0, 0, 0);
            AddComponent(new LocalizedAddonComponent(13827, 1076672), 0, 1, 0);
        }

        public CrystalAltarAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new CrystalAltarDeed();

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class CrystalAltarDeed : BaseAddonDeed
    {
        public override int LabelNumber => 1076672;  // Crystal Altar

        public override bool ExcludeDeedHue => true;

        public override BaseAddon Addon => new CrystalAltarAddon();

        [Constructable]
        public CrystalAltarDeed()
            : base()
        {
            LootType = LootType.Blessed;
            Hue = 1173;
        }

        public CrystalAltarDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
