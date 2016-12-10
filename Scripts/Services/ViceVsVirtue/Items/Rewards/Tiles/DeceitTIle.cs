using System;
using Server;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;

namespace Server.Engines.VvV
{
    public class DeceitTileAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new DeceitTileDeed(); } }

        public TileType TileType { get; set; }

        private int offset;

        [Constructable]
        public DeceitTileAddon(TileType type)
        {
            TileType = type;

            offset = 0;

            if (type != TileType.North)
            {
                offset = 4;
            }

            AddComponent(new AddonComponent(39380 + offset), 0, 0, 0);
            AddComponent(new AddonComponent(39381 + offset), 1, 0, 0);
            AddComponent(new AddonComponent(39382 + offset), 0, 1, 0);
            AddComponent(new AddonComponent(39383 + offset), 1, 1, 0);
        }

        public DeceitTileAddon(Serial serial)
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

    public class DeceitTileDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon { get { return new DeceitTileAddon(TileType); } }
        public override int LabelNumber { get { return 1155517; } } // Deceit Tile

        public TileType TileType { get; set; }

        [Constructable]
        public DeceitTileDeed()
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

        public DeceitTileDeed(Serial serial)
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
            list.Add((int)TileType.North, "Deceit (North)");
            list.Add((int)TileType.West, "Deceit (East)");
        }


        public void OnOptionSelected(Mobile from, int choice)
        {
            TileType = (TileType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }
    }
}