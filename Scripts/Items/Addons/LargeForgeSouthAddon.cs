namespace Server.Items
{
    public class LargeForgeSouthAddon : BaseAddon
    {
        [Constructable]
        public LargeForgeSouthAddon()
        {
            AddComponent(new ForgeComponent(0x197A), 0, 0, 0);
            AddComponent(new ForgeComponent(0x197E), 1, 0, 0);
            AddComponent(new ForgeComponent(0x19A2), 2, 0, 0);
            AddComponent(new ForgeComponent(0x199E), 3, 0, 0);
        }

        public LargeForgeSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new LargeForgeSouthDeed();
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

    public class LargeForgeSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public LargeForgeSouthDeed()
        {
        }

        public LargeForgeSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new LargeForgeSouthAddon();
        public override int LabelNumber => 1044332;// large forge (south)
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