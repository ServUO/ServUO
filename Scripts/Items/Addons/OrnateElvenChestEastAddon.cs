namespace Server.Items
{
    public class OrnateElvenChestEastAddon : BaseAddonContainer
    {
        [Constructable]
        public OrnateElvenChestEastAddon()
            : base(0x309A)
        {
            AddComponent(new LocalizedContainerComponent(0x309B, 1073383), 0, -1, 0);
        }

        public OrnateElvenChestEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainerDeed Deed => new OrnateElvenChestEastDeed();
        public override int LabelNumber => 1073383;// ornate elven chest (east)
        public override bool RetainDeedHue => true;
        public override int DefaultGumpID => 0x10C;
        public override int DefaultDropSound => 0x42;
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

    public class OrnateElvenChestEastDeed : BaseAddonContainerDeed
    {
        [Constructable]
        public OrnateElvenChestEastDeed()
            : base()
        {
        }

        public OrnateElvenChestEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainer Addon => new OrnateElvenChestEastAddon();
        public override int LabelNumber => 1073383;// ornate elven chest (east)
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