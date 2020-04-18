namespace Server.Items
{
    public class OrnateElvenTableSouthAddon : BaseAddon
    {
        [Constructable]
        public OrnateElvenTableSouthAddon()
        {
            AddComponent(new AddonComponent(0x308E), -1, 0, 0);
            AddComponent(new AddonComponent(0x308D), 0, 0, 0);
            AddComponent(new AddonComponent(0x308C), 1, 0, 0);
        }

        public OrnateElvenTableSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new OrnateElvenTableSouthDeed();
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

    public class OrnateElvenTableSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public OrnateElvenTableSouthDeed()
        {
        }

        public OrnateElvenTableSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new OrnateElvenTableSouthAddon();
        public override int LabelNumber => 1072869;// ornate table (south)
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