namespace Server.Items
{
    public class ElvenBedSouthAddon : BaseAddon
    {
        [Constructable]
        public ElvenBedSouthAddon()
        {
            AddComponent(new AddonComponent(0x3050), 0, 0, 0);
            AddComponent(new AddonComponent(0x3051), 0, -1, 0);
        }

        public ElvenBedSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new ElvenBedSouthDeed();
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

    public class ElvenBedSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public ElvenBedSouthDeed()
        {
        }

        public ElvenBedSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new ElvenBedSouthAddon();
        public override int LabelNumber => 1072860;// elven bed (south)
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