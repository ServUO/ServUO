namespace Server.Items
{
    public class TableWithBlueClothAddon : BaseAddon
    {
        [Constructable]
        public TableWithBlueClothAddon()
            : base()
        {
            AddComponent(new LocalizedAddonComponent(0x118C, 1076276), 0, 0, 0);
        }

        public TableWithBlueClothAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new TableWithBlueClothDeed();
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class TableWithBlueClothDeed : BaseAddonDeed
    {
        [Constructable]
        public TableWithBlueClothDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public TableWithBlueClothDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new TableWithBlueClothAddon();
        public override int LabelNumber => 1076276;// Table With A Blue Tablecloth
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}