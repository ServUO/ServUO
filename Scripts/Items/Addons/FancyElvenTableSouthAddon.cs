namespace Server.Items
{
    public class FancyElvenTableSouthAddon : BaseAddon
    {
        [Constructable]
        public FancyElvenTableSouthAddon()
        {
            AddComponent(new AddonComponent(0x3095), 0, 1, 0);
            AddComponent(new AddonComponent(0x3096), 0, 0, 0);
            AddComponent(new AddonComponent(0x3097), 0, -1, 0);
        }

        public FancyElvenTableSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new FancyElvenTableSouthDeed();
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

    public class FancyElvenTableSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public FancyElvenTableSouthDeed()
        {
        }

        public FancyElvenTableSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new FancyElvenTableSouthAddon();
        public override int LabelNumber => 1073385;// hardwood table (south)
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