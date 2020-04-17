namespace Server.Items
{
    public class CrystalBrazierAddon : BaseAddon
    {
        [Constructable]
        public CrystalBrazierAddon()
        {
            AddComponent(new LocalizedAddonComponent(0x35EF, 1076667), 0, 0, 0);
        }

        public CrystalBrazierAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new CrystalBrazierDeed();

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

    public class CrystalBrazierDeed : BaseAddonDeed
    {
        public override int LabelNumber => 1076667;  // Crystal Brazier

        public override bool ExcludeDeedHue => true;

        public override BaseAddon Addon => new CrystalBrazierAddon();

        [Constructable]
        public CrystalBrazierDeed()
            : base()
        {
            LootType = LootType.Blessed;
            Hue = 1173;
        }

        public CrystalBrazierDeed(Serial serial)
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
