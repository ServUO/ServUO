using System;
using Server;
using Server.Gumps;

namespace Server.Items
{
    public enum SpiritualityTileType
    {
        North = 0,
        West = 1,
    }

    public class SpiritualityVirtueTileAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new SpiritualityVirtueTileDeed(); } }

        private SpiritualityTileType m_SpiritualityTileType;

        private int offset;

        [Constructable]
        public SpiritualityVirtueTileAddon(SpiritualityTileType type)
        {
            m_SpiritualityTileType = type;

            offset = 0;

            if (type == SpiritualityTileType.North)
            {
                offset = 4;
            }

            AddComponent(new AddonComponent(5311 + offset), 0, 0, 0);
            AddComponent(new AddonComponent(5312 + offset), 0, 1, 0);
            AddComponent(new AddonComponent(5313 + offset), 1, 1, 0);
            AddComponent(new AddonComponent(5314 + offset), 1, 0, 0);
        }

        public SpiritualityVirtueTileAddon(Serial serial)
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

    public class SpiritualityVirtueTileDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon { get { return new SpiritualityVirtueTileAddon(m_SpiritualityTileType); } }
        public override int LabelNumber { get { return 1080484; } } // Spirituality Virtue Tile Deed

        private SpiritualityTileType m_SpiritualityTileType;

        [Constructable]
        public SpiritualityVirtueTileDeed()
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

        public SpiritualityVirtueTileDeed(Serial serial)
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
            list.Add((int)SpiritualityTileType.North, 1080227);
            list.Add((int)SpiritualityTileType.West, 1080226);
        }


        public void OnOptionSelected(Mobile from, int choice)
        {
            m_SpiritualityTileType = (SpiritualityTileType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }
    }
}