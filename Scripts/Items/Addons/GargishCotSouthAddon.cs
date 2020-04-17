namespace Server.Items
{
    public class GargishCotSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new GargishCotSouthDeed();

        #region Mondain's Legacy
        public override bool RetainDeedHue => true;
        #endregion

        [Constructable]
        public GargishCotSouthAddon()
        {
            AddComponent(new AddonComponent(0x400D), 0, 0, 0);
            AddComponent(new AddonComponent(0x400C), 0, -1, 0);
        }

        public GargishCotSouthAddon(Serial serial)
            : base(serial)
        {
        }

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

    public class GargishCotSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new GargishCotSouthAddon();
        public override int LabelNumber => 1111920;// gargish cot (south)

        [Constructable]
        public GargishCotSouthDeed()
        {
        }

        public GargishCotSouthDeed(Serial serial)
            : base(serial)
        {
        }

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