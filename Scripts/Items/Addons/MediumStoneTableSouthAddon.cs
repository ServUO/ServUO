namespace Server.Items
{
    public class MediumStoneTableSouthAddon : BaseAddon
    {
        [Constructable]
        public MediumStoneTableSouthAddon()
            : this(0)
        {
        }

        [Constructable]
        public MediumStoneTableSouthAddon(int hue)
        {
            AddComponent(new AddonComponent(0x1205), 0, 0, 0);
            AddComponent(new AddonComponent(0x1204), 1, 0, 0);
            Hue = hue;
        }

        public MediumStoneTableSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new MediumStoneTableSouthDeed();
        public override bool RetainDeedHue => true;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class MediumStoneTableSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public MediumStoneTableSouthDeed()
        {
        }

        public MediumStoneTableSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new MediumStoneTableSouthAddon(Hue);
        public override int LabelNumber => 1044509;// stone table (South)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}