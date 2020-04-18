namespace Server.Items
{
    public class TerMurDresserEastAddon : BaseAddon
    {
        [Constructable]
        public TerMurDresserEastAddon()
        {
            AddComponent(new AddonComponent(0x402E), 0, 0, 0);
            AddComponent(new AddonComponent(0x402D), 0, -1, 0);
        }

        public TerMurDresserEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new TerMurDresserEastDeed();
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

    public class TerMurDresserEastDeed : BaseAddonDeed
    {
        [Constructable]
        public TerMurDresserEastDeed()
        {
        }

        public TerMurDresserEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new TerMurDresserEastAddon();
        public override int LabelNumber => 1111784;// Ter-Mur style dresser (east)
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