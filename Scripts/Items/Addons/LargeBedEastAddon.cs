namespace Server.Items
{
    public class LargeBedEastAddon : BaseAddon
    {
        [Constructable]
        public LargeBedEastAddon()
        {
            AddComponent(new AddonComponent(0xA7D), 0, 0, 0);
            AddComponent(new AddonComponent(0xA7C), 0, 1, 0);
            AddComponent(new AddonComponent(0xA79), 1, 0, 0);
            AddComponent(new AddonComponent(0xA78), 1, 1, 0);
        }

        public LargeBedEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new LargeBedEastDeed();
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

    public class LargeBedEastDeed : BaseAddonDeed
    {
        [Constructable]
        public LargeBedEastDeed()
        {
        }

        public LargeBedEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new LargeBedEastAddon();
        public override int LabelNumber => 1044324;// large bed (east)
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