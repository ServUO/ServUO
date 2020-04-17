namespace Server.Items
{
    public class WarriorStatueEastAddon : BaseAddon
    {
        [Constructable]
        public WarriorStatueEastAddon()
        {
            AddComponent(new AddonComponent(0x2D12), 0, 0, 0);
        }

        public WarriorStatueEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new WarriorStatueEastDeed();
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

    public class WarriorStatueEastDeed : BaseAddonDeed
    {
        [Constructable]
        public WarriorStatueEastDeed()
        {
        }

        public WarriorStatueEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new WarriorStatueEastAddon();
        public override int LabelNumber => 1072888;// warrior statue (east)
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