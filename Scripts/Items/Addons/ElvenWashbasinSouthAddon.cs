namespace Server.Items
{
    public class ElvenWashBasinSouthAddon : BaseAddon
    {
        [Constructable]
        public ElvenWashBasinSouthAddon()
        {
            AddComponent(new AddonComponent(0x30E1), 0, 0, 0);
            AddComponent(new AddonComponent(0x30E2), 1, 0, 0);
        }

        public ElvenWashBasinSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new ElvenWashBasinSouthDeed();
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

    public class ElvenWashBasinSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public ElvenWashBasinSouthDeed()
        {
        }

        public ElvenWashBasinSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new ElvenWashBasinSouthAddon();
        public override int LabelNumber => 1072865;// elven wash basin (south)
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