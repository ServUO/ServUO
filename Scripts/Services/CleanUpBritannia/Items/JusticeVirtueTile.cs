using System;
using Server;
using Server.Gumps;

namespace Server.Items
{
    public enum JusticeTileType
    {
        North = 0,
        West = 1,
    }

    public class JusticeVirtueTileAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new JusticeVirtueTileDeed(); } }

        private JusticeTileType m_JusticeTileType;

        private int offset;

        [Constructable]
        public JusticeVirtueTileAddon(JusticeTileType type)
        {
            m_JusticeTileType = type;

            offset = 0;

            if (type == JusticeTileType.North)
            {
                offset = 4;
            }

            AddComponent(new AddonComponent(5295 + offset), 0, 0, 0);
            AddComponent(new AddonComponent(5296 + offset), 0, 1, 0);
            AddComponent(new AddonComponent(5297 + offset), 1, 1, 0);
            AddComponent(new AddonComponent(5298 + offset), 1, 0, 0);
        }

        public JusticeVirtueTileAddon(Serial serial)
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

    public class JusticeVirtueTileDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon { get { return new JusticeVirtueTileAddon(m_JusticeTileType); } }
        public override int LabelNumber { get { return 1080487; } } // Justice Virtue Tile Deed

        private JusticeTileType m_JusticeTileType;

        [Constructable]
        public JusticeVirtueTileDeed()
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

        public JusticeVirtueTileDeed(Serial serial)
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
            list.Add((int)JusticeTileType.North, 1080221);
            list.Add((int)JusticeTileType.West, 1080220);
        }


        public void OnOptionSelected(Mobile from, int choice)
        {
            m_JusticeTileType = (JusticeTileType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }
    }
}