namespace Server.Items
{
    public class MiniSoulForge : BaseAddon
    {
        public override BaseAddonDeed Deed => new MiniSoulForgeDeed();

        [Constructable]
        public MiniSoulForge()
        {
            AddComponent(new AddonComponent(17607), 0, 0, 0);
        }

        public MiniSoulForge(Serial serial) : base(serial)
        {
        }

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
    public class MiniSoulForgeDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new MiniSoulForge();

        [Constructable]
        public MiniSoulForgeDeed()
        {
            Name = "SoulForge";
        }

        public MiniSoulForgeDeed(Serial serial) : base(serial)
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