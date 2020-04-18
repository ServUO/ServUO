namespace Server.Items
{
    public class ElvenWashBasinSouthAddonWithDrawer : BaseAddonContainer
    {
        [Constructable]
        public ElvenWashBasinSouthAddonWithDrawer()
            : base(0x30E2)
        {
            AddComponent(new AddonContainerComponent(0x30E1), -1, 0, 0);
        }

        public ElvenWashBasinSouthAddonWithDrawer(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainerDeed Deed => new ElvenWashBasinSouthWithDrawerDeed();
        public override bool RetainDeedHue => true;
        public override int DefaultGumpID => 0x0104;
        public override int DefaultDropSound => 0x0042;
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

    public class ElvenWashBasinSouthWithDrawerDeed : BaseAddonContainerDeed
    {
        [Constructable]
        public ElvenWashBasinSouthWithDrawerDeed()
        {
        }

        public ElvenWashBasinSouthWithDrawerDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonContainer Addon => new ElvenWashBasinSouthAddonWithDrawer();
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