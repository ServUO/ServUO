using System;
using Server;
using Server.Gumps;

namespace Server.Items
{
    public enum SacrificeTileType
    {
        North = 0,
        West = 1,
    }

    public class SacrificeVirtueTileAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new SacrificeVirtueTileDeed(); } }

        private SacrificeTileType m_SacrificeTileType;

        private int offset;

        [Constructable]
        public SacrificeVirtueTileAddon(SacrificeTileType type)
        {
            m_SacrificeTileType = type;

            offset = 0;

            if (type == SacrificeTileType.North)
            {
                offset = 4;
            }

            AddComponent(new AddonComponent(5386 + offset), 0, 0, 0);
            AddComponent(new AddonComponent(5387 + offset), 0, 1, 0);
            AddComponent(new AddonComponent(5388 + offset), 1, 1, 0);
            AddComponent(new AddonComponent(5389 + offset), 1, 0, 0);
        }

        public SacrificeVirtueTileAddon(Serial serial)
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

    public class SacrificeVirtueTileDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon { get { return new SacrificeVirtueTileAddon(m_SacrificeTileType); } }
        public override int LabelNumber { get { return 1080482; } } // Sacrifice Virtue Tile Deed

        private SacrificeTileType m_SacrificeTileType;

        [Constructable]
        public SacrificeVirtueTileDeed()
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

        public SacrificeVirtueTileDeed(Serial serial)
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
            list.Add((int)SacrificeTileType.North, 1080236);
            list.Add((int)SacrificeTileType.West, 1080235);
        }


        public void OnOptionSelected(Mobile from, int choice)
        {
            m_SacrificeTileType = (SacrificeTileType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }
    }
}