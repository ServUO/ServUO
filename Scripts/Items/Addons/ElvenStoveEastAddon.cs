namespace Server.Items
{
    public class ElvenStoveEastAddon : BaseAddon
    {
        [Constructable]
        public ElvenStoveEastAddon()
        {
            AddComponent(new AddonComponent(0x2DDB), 0, 0, 0);
        }

        public ElvenStoveEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new ElvenStoveEastDeed();
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

    public class ElvenStoveEastDeed : BaseAddonDeed
    {
        [Constructable]
        public ElvenStoveEastDeed()
        {
        }

        public ElvenStoveEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new ElvenStoveEastAddon();
        public override int LabelNumber => 1073395;// elven oven (east)
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