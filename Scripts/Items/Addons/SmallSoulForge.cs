namespace Server.Items
{
    public class SmallSoulForge : BaseAddon
    {
        public override BaseAddonDeed Deed => new SmallSoulForgeDeed();
        public override bool RetainDeedHue => true;

        [Constructable]
        public SmallSoulForge()
        {
            AddComponent(new ForgeComponent(17607), 0, 0, 0);
        }

        public SmallSoulForge(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SmallSoulForgeDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new SmallSoulForge();
        public override int LabelNumber => 1149695;

        [Constructable]
        public SmallSoulForgeDeed()
        {
        }

        public SmallSoulForgeDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}