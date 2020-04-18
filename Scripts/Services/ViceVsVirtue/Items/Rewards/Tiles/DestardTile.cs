using Server.Gumps;
using Server.Items;

namespace Server.Engines.VvV
{
    public class DestardTileAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new DestardTileDeed();

        public TileType TileType { get; set; }

        private readonly int offset;

        [Constructable]
        public DestardTileAddon(TileType type)
        {
            TileType = type;

            offset = 0;

            if (type != TileType.North)
            {
                offset = 4;
            }

            AddComponent(new AddonComponent(39396 + offset), 0, 0, 0);
            AddComponent(new AddonComponent(39397 + offset), 1, 0, 0);
            AddComponent(new AddonComponent(39398 + offset), 0, 1, 0);
            AddComponent(new AddonComponent(39399 + offset), 1, 1, 0);
        }

        public DestardTileAddon(Serial serial)
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

    public class DestardTileDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon => new DestardTileAddon(TileType);
        public override int LabelNumber => 1155519;  // Destard Tile

        public TileType TileType { get; set; }

        [Constructable]
        public DestardTileDeed()
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

        public DestardTileDeed(Serial serial)
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
            list.Add((int)TileType.North, "Destard (North)");
            list.Add((int)TileType.West, "Destard (East)");
        }


        public void OnOptionSelected(Mobile from, int choice)
        {
            TileType = (TileType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }
    }
}