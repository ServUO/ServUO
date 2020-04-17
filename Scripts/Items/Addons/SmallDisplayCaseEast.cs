namespace Server.Items
{
    public class SmallDisplayCaseEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new SmallDisplayCaseEastDeed();

        [Constructable]
        public SmallDisplayCaseEastAddon()
        {
            AddComponent(new AddonComponent(0x0B09), 0, 0, 0);
            AddComponent(new AddonComponent(0x0B0B), 0, 0, 3);
        }

        public SmallDisplayCaseEastAddon(Serial serial)
            : base(serial)
        {
        }

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

    public class SmallDisplayCaseEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new SmallDisplayCaseEastAddon();
        public override int LabelNumber => 1155843;  // Small Display Case (East)

        [Constructable]
        public SmallDisplayCaseEastDeed()
        {
        }

        public SmallDisplayCaseEastDeed(Serial serial)
            : base(serial)
        {
        }

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