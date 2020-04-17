namespace Server.Items
{
    public class AnvilSouthAddon : BaseAddon
    {
        [Constructable]
        public AnvilSouthAddon()
        {
            AddComponent(new AnvilComponent(0xFB0), 0, 0, 0);
        }

        public AnvilSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new AnvilSouthDeed();

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

    public class AnvilSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public AnvilSouthDeed()
        {
        }

        public AnvilSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new AnvilSouthAddon();
        public override int LabelNumber => 1044334;// anvil (south)
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