namespace Server.Items
{
    public class OrnateElvenChestSouthAddon : BaseAddonContainer
    {
        [Constructable]
        public OrnateElvenChestSouthAddon()
            : base(0x3098)
        {
            AddComponent(new LocalizedContainerComponent(0x3099, 1072862), -1, 0, 0);
        }

        public OrnateElvenChestSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainerDeed Deed => new OrnateElvenChestSouthDeed();
        public override int LabelNumber => 1072862;// ornate elven chest (south)
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

    public class OrnateElvenChestSouthDeed : BaseAddonContainerDeed
    {
        [Constructable]
        public OrnateElvenChestSouthDeed()
            : base()
        {
        }

        public OrnateElvenChestSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainer Addon => new OrnateElvenChestSouthAddon();
        public override int LabelNumber => 1072862;// ornate elven chest (south)
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