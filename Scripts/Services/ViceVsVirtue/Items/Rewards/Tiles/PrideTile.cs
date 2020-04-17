using Server.Gumps;
using Server.Items;

namespace Server.Engines.VvV
{
    public class PrideTileAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new PrideTileDeed();

        public TileType TileType { get; set; }

        private readonly int offset;

        [Constructable]
        public PrideTileAddon(TileType type)
        {
            TileType = type;

            offset = 0;

            if (type != TileType.North)
            {
                offset = 4;
            }

            AddComponent(new AddonComponent(39412 + offset), 0, 0, 0);
            AddComponent(new AddonComponent(39413 + offset), 1, 0, 0);
            AddComponent(new AddonComponent(39414 + offset), 0, 1, 0);
            AddComponent(new AddonComponent(39415 + offset), 1, 1, 0);
        }

        public PrideTileAddon(Serial serial)
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

    public class PrideTileDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon => new PrideTileAddon(TileType);
        public override int LabelNumber => 1155521;  // Pride Tile

        public TileType TileType { get; set; }

        [Constructable]
        public PrideTileDeed()
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

        public PrideTileDeed(Serial serial)
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
            list.Add((int)TileType.North, "Pride (North)");
            list.Add((int)TileType.West, "Pride (East)");
        }


        public void OnOptionSelected(Mobile from, int choice)
        {
            TileType = (TileType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }
    }
}