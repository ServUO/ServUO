using Server.Gumps;

namespace Server.Items
{
    public enum HonorTileType
    {
        North = 0,
        West = 1,
    }

    public class HonorVirtueTileAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new HonorVirtueTileDeed();

        private readonly HonorTileType m_HonorTileType;

        private readonly int offset;

        [Constructable]
        public HonorVirtueTileAddon(HonorTileType type)
        {
            m_HonorTileType = type;

            offset = 0;

            if (type == HonorTileType.North)
            {
                offset = 4;
            }

            AddComponent(new AddonComponent(5319 + offset), 0, 0, 0);
            AddComponent(new AddonComponent(5320 + offset), 0, 1, 0);
            AddComponent(new AddonComponent(5321 + offset), 1, 1, 0);
            AddComponent(new AddonComponent(5322 + offset), 1, 0, 0);
        }

        public HonorVirtueTileAddon(Serial serial)
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

    public class HonorVirtueTileDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon => new HonorVirtueTileAddon(m_HonorTileType);
        public override int LabelNumber => 1080485;  // Honor Virtue Tile Deed

        private HonorTileType m_HonorTileType;

        [Constructable]
        public HonorVirtueTileDeed()
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

        public HonorVirtueTileDeed(Serial serial)
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
            list.Add((int)HonorTileType.North, 1080230);
            list.Add((int)HonorTileType.West, 1080229);
        }


        public void OnOptionSelected(Mobile from, int choice)
        {
            m_HonorTileType = (HonorTileType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }
    }
}
