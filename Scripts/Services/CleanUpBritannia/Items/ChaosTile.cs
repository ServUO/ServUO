namespace Server.Items
{
    public class ChaosTileAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new ChaosTileDeed();

        [Constructable]
        public ChaosTileAddon()
        {
            AddComponent(new AddonComponent(5347), 0, 0, 0);
            AddComponent(new AddonComponent(5348), 0, 1, 0);
            AddComponent(new AddonComponent(5349), 1, 1, 0);
            AddComponent(new AddonComponent(5350), 1, 0, 0);
        }

        public ChaosTileAddon(Serial serial)
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

    public class ChaosTileDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new ChaosTileAddon();
        public override int LabelNumber => 1080490;  // Chaos Tile Deed

        [Constructable]
        public ChaosTileDeed()
        {
            LootType = LootType.Blessed;
        }

        public ChaosTileDeed(Serial serial)
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
