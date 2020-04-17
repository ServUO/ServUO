namespace Server.Items
{
    public class GargishCouchEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new GargishCouchEastDeed();

        #region Mondain's Legacy
        public override bool RetainDeedHue => true;
        #endregion

        [Constructable]
        public GargishCouchEastAddon()
        {
            AddComponent(new AddonComponent(0x402A), 0, 0, 0);
            AddComponent(new AddonComponent(0x4029), 0, -1, 0);
        }

        public GargishCouchEastAddon(Serial serial)
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

    public class GargishCouchEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new GargishCouchEastAddon();
        public override int LabelNumber => 1111776;// gargish couch (East)

        [Constructable]
        public GargishCouchEastDeed()
        {
        }

        public GargishCouchEastDeed(Serial serial)
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