namespace Server.Items
{
    public class ElvenLoveseatSouthAddon : BaseAddon
    {
        [Constructable]
        public ElvenLoveseatSouthAddon()
        {
            AddComponent(new AddonComponent(0x308A), 0, 0, 0);
            AddComponent(new AddonComponent(0x308B), 0, -1, 0);
        }

        public ElvenLoveseatSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new ElvenLoveseatSouthDeed();
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

    public class ElvenLoveseatSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public ElvenLoveseatSouthDeed()
        {
        }

        public ElvenLoveseatSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new ElvenLoveseatSouthAddon();
        public override int LabelNumber => 1072867;// elven loveseat (south)
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