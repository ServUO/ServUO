namespace Server.Items
{
    public class StoneOvenSouthAddon : BaseAddon
    {
        [Constructable]
        public StoneOvenSouthAddon()
        {
            AddComponent(new AddonComponent(0x931), -1, 0, 0);
            AddComponent(new AddonComponent(0x930), 0, 0, 0);
        }

        public StoneOvenSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new StoneOvenSouthDeed();
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

    public class StoneOvenSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public StoneOvenSouthDeed()
        {
        }

        public StoneOvenSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new StoneOvenSouthAddon();
        public override int LabelNumber => 1044346;// stone oven (south)
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