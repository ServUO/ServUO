namespace Server.Items
{
    public class DungeonFountainAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new DungeonFountainDeed();

        [Constructable]
        public DungeonFountainAddon()
            : this(true)
        {
        }

        [Constructable]
        public DungeonFountainAddon(bool east)
            : base()
        {
            AddComponent(new AddonComponent(42226), 0, 0, 0);
            AddComponent(new AddonComponent(42227), 1, 0, 0);
            AddComponent(new AddonComponent(42225), 0, 1, 0);
            AddComponent(new AddonComponent(42228), 1, 1, 0);
        }

        public DungeonFountainAddon(Serial serial)
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

    public class DungeonFountainDeed : BaseAddonDeed
    {
        public override int LabelNumber => 1159474;  // Dungeon Fountain
        public override BaseAddon Addon => new DungeonFountainAddon();

        [Constructable]
        public DungeonFountainDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public DungeonFountainDeed(Serial serial)
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
