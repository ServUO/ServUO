namespace Server.Items
{
    public class FirePitAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new FirePitDeed();

        [Constructable]
        public FirePitAddon()
        {
            AddComponent(new AddonComponent(10749), 0, 0, 0);
        }

        public FirePitAddon(Serial serial)
            : base(serial)
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

    public class FirePitDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new FirePitAddon();
        public override int LabelNumber => 1080206;  // Fire Pit

        [Constructable]
        public FirePitDeed()
        {
            LootType = LootType.Blessed;
        }

        public FirePitDeed(Serial serial)
            : base(serial)
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
