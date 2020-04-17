namespace Server.Items
{
    public class GargishLongTableSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new GargishLongTableSouthDeed();

        #region Mondain's Legacy
        public override bool RetainDeedHue => true;
        #endregion

        [Constructable]
        public GargishLongTableSouthAddon()
        {
            AddComponent(new AddonComponent(0x402F), -1, 0, 0);
            AddComponent(new AddonComponent(0x4030), 0, 0, 0);
        }

        public GargishLongTableSouthAddon(Serial serial)
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

    public class GargishLongTableSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new GargishLongTableSouthAddon();
        public override int LabelNumber => 1111781;// long table

        [Constructable]
        public GargishLongTableSouthDeed()
        {
        }

        public GargishLongTableSouthDeed(Serial serial)
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