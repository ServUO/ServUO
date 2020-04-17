using Server.Gumps;

namespace Server.Items
{
    public enum ValorTileType
    {
        North = 0,
        West = 1,
    }

    public class ValorVirtueTileAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new ValorVirtueTileDeed();

        private readonly ValorTileType m_ValorTileType;

        private readonly int offset;

        [Constructable]
        public ValorVirtueTileAddon(ValorTileType type)
        {
            m_ValorTileType = type;

            offset = 0;

            if (type == ValorTileType.North)
            {
                offset = 4;
            }

            AddComponent(new AddonComponent(5303 + offset), 0, 0, 0);
            AddComponent(new AddonComponent(5304 + offset), 0, 1, 0);
            AddComponent(new AddonComponent(5305 + offset), 1, 1, 0);
            AddComponent(new AddonComponent(5306 + offset), 1, 0, 0);
        }

        public ValorVirtueTileAddon(Serial serial)
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

    public class ValorVirtueTileDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon => new ValorVirtueTileAddon(m_ValorTileType);
        public override int LabelNumber => 1080486;  // Valor Virtue Tile Deed

        private ValorTileType m_ValorTileType;

        [Constructable]
        public ValorVirtueTileDeed()
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

        public ValorVirtueTileDeed(Serial serial)
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
            list.Add((int)ValorTileType.North, 1080224);
            list.Add((int)ValorTileType.West, 1080223);
        }


        public void OnOptionSelected(Mobile from, int choice)
        {
            m_ValorTileType = (ValorTileType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }
    }
}