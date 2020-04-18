namespace Server.Items
{
    public class StoneAnvilSouthAddon : BaseAddon
    {
        [Constructable]
        public StoneAnvilSouthAddon()
        {
            AddComponent(new AnvilComponent(0x2DD5), 0, 0, 0);
        }

        public StoneAnvilSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new StoneAnvilSouthDeed();
        public override bool RetainDeedHue => true;
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

    public class StoneAnvilSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public StoneAnvilSouthDeed()
        {
        }

        public StoneAnvilSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new StoneAnvilSouthAddon();
        public override int LabelNumber => 1072876;// stone anvil (south)
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