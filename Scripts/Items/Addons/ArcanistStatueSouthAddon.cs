namespace Server.Items
{
    public class ArcanistStatueSouthAddon : BaseAddon
    {
        [Constructable]
        public ArcanistStatueSouthAddon()
        {
            AddComponent(new AddonComponent(0x2D0F), 0, 0, 0);
        }

        public ArcanistStatueSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new ArcanistStatueSouthDeed();
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

    public class ArcanistStatueSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public ArcanistStatueSouthDeed()
        {
        }

        public ArcanistStatueSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new ArcanistStatueSouthAddon();
        public override int LabelNumber => 1072885;// arcanist statue (south)
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
}