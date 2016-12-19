using System;
using Server;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;

namespace Server.Engines.VvV
{
    public class HythlothTileAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new HythlothTileDeed(); } }

        public TileType TileType { get; set; }

        private int offset;

        [Constructable]
        public HythlothTileAddon(TileType type)
        {
            TileType = type;

            offset = 0;

            if (type != TileType.North)
            {
                offset = 4;
            }

            AddComponent(new AddonComponent(39404 + offset), 0, 0, 0);
            AddComponent(new AddonComponent(39405 + offset), 1, 0, 0);
            AddComponent(new AddonComponent(39406 + offset), 0, 1, 0);
            AddComponent(new AddonComponent(39407 + offset), 1, 1, 0);
        }

        public HythlothTileAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class HythlothTileDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon { get { return new HythlothTileAddon(TileType); } }
        public override int LabelNumber { get { return 1155520; } } // Hythloth Tile

        public TileType TileType { get; set; }

        [Constructable]
        public HythlothTileDeed()
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

        public HythlothTileDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)TileType.North, "Hythloth (North)");
            list.Add((int)TileType.West, "Hythloth (East)");
        }


        public void OnOptionSelected(Mobile from, int choice)
        {
            TileType = (TileType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }
    }
}