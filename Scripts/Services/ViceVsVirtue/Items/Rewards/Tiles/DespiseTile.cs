using Server.Gumps;
using Server.Items;

namespace Server.Engines.VvV
{
    public class DespiseTileAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new DespiseTileDeed();

        public TileType TileType { get; set; }

        private readonly int offset;

        [Constructable]
        public DespiseTileAddon(TileType type)
        {
            TileType = type;

            offset = 0;

            if (type != TileType.North)
            {
                offset = 4;
            }

            AddComponent(new AddonComponent(39388 + offset), 0, 0, 0);
            AddComponent(new AddonComponent(39389 + offset), 1, 0, 0);
            AddComponent(new AddonComponent(39390 + offset), 0, 1, 0);
            AddComponent(new AddonComponent(39391 + offset), 1, 1, 0);
        }

        public DespiseTileAddon(Serial serial)
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

    public class DespiseTileDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon => new DespiseTileAddon(TileType);
        public override int LabelNumber => 1155518;  // Despise Tile

        public TileType TileType { get; set; }

        [Constructable]
        public DespiseTileDeed()
        {
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this));
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.       	
        }

        public DespiseTileDeed(Serial serial)
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

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)TileType.North, "Despise (North)");
            list.Add((int)TileType.West, "Despise (East)");
        }


        public void OnOptionSelected(Mobile from, int choice)
        {
            TileType = (TileType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }
    }
}