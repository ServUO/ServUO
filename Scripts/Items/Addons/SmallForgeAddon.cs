namespace Server.Items
{
    public class SmallForgeAddon : BaseAddon
    {
        [Constructable]
        public SmallForgeAddon()
        {
            AddComponent(new ForgeComponent(0xFB1), 0, 0, 0);
        }

        public SmallForgeAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new SmallForgeDeed();
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

    public class SmallForgeDeed : BaseAddonDeed
    {
        [Constructable]
        public SmallForgeDeed()
        {
        }

        public SmallForgeDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new SmallForgeAddon();
        public override int LabelNumber => 1044330;// small forge
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