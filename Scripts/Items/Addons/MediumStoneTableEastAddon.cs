namespace Server.Items
{
    public class MediumStoneTableEastAddon : BaseAddon
    {
        [Constructable]
        public MediumStoneTableEastAddon()
            : this(0)
        {
        }

        [Constructable]
        public MediumStoneTableEastAddon(int hue)
        {
            AddComponent(new AddonComponent(0x1202), 0, 0, 0);
            AddComponent(new AddonComponent(0x1201), 0, 1, 0);
            Hue = hue;
        }

        public MediumStoneTableEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new MediumStoneTableEastDeed();
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

    public class MediumStoneTableEastDeed : BaseAddonDeed
    {
        [Constructable]
        public MediumStoneTableEastDeed()
        {
        }

        public MediumStoneTableEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new MediumStoneTableEastAddon(Hue);
        public override int LabelNumber => 1044508;// stone table (east)
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