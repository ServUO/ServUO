using System;
using Server;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Multis;

namespace Server.Items
{
    public enum RugType
    {
        EastLarge,
        SouthLarge,
        EastSmall,
        SouthSmall
    }

    [TypeAlias("Server.Items.RoseRugEastAddon", "Server.Items.RoseRugSouthAddon")]
    public class RoseRugAddon : BaseAddon, IRewardItem
    {
        public override bool ForceShowProperties { get { return true; } }

        private bool m_IsRewardItem;
        private int m_ResourceCount;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return m_IsRewardItem;
            }
            set
            {
                m_IsRewardItem = value;
                InvalidateAddonPropreties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ResourceCount
        {
            get
            {
                return m_ResourceCount;
            }
            set
            {
                m_ResourceCount = value;
                InvalidateAddonPropreties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextResourceCount { get; set; }

        private static int[,] _EastLarge =
        {
              {14551, 2, -3, 0}, {14550, 0, -3, 0}, {14549, 1, -3, 0}   // 1	2	3	
			, {14552, -1, -3, 0}, {14542, 0, -1, 0}, {14543, 2, -1, 0}  // 4	5	6	
			, {14536, -1, 1, 0}, {14527, 0, 3, 0}, {14530, 0, 2, 0}     // 7	8	9	
			, {14537, 1, 0, 0}, {14535, 2, 1, 0}, {14544, -1, -1, 0}    // 10	11	12	
			, {14532, -1, 2, 0}, {14529, 1, 2, 0}, {14541, 1, -1, 0}    // 13	14	15	
			, {14547, 2, -2, 0}, {14548, -1, -2, 0}, {14526, 1, 3, 0}   // 16	17	18	
			, {14531, 2, 2, 0}, {14546, 0, -2, 0}, {14540, -1, 0, 0}    // 19	20	21	
			, {14528, -1, 3, 0}, {14534, 0, 1, 0}, {14525, 2, 3, 0}     // 22	23	24	
			, {14533, 1, 1, 0}, {14545, 1, -2, 0}, {14539, 2, 0, 0}     // 25	26	27	
			, {14538, 0, 0, 0}// 28	
		};

        private static int[,] _SouthLarge =
        {
              {14512, 0, -1, 0}, {14511, 0, 2, 0}, {14497, -3, 2, 0}// 1	2	3	
			, {14507, -1, 2, 0}, {14498, -3, 1, 0}, {14501, -2, 1, 0}// 4	5	6	
			, {14509, 0, 1, 0}, {14500, -3, -1, 0}, {14516, 1, -1, 0}// 7	8	9	
			, {14517, 2, 1, 0}, {14518, 2, 0, 0}, {14515, 1, 2, 0}// 10	11	12	
			, {14503, -2, 2, 0}, {14506, -1, 0, 0}, {14510, 0, 0, 0}// 13	14	15	
			, {14508, -1, -1, 0}, {14504, -2, -1, 0}, {14502, -2, 0, 0}// 16	17	18	
			, {14505, -1, 1, 0}, {14519, 2, 2, 0}, {14514, 1, 0, 0}// 19	20	21	
			, {14520, 2, -1, 0}, {14513, 1, 1, 0}, {14499, -3, 0, 0}// 22	23	24	
			, {14522, 3, 0, 0}, {14521, 3, 1, 0}, {14523, 3, 2, 0}// 25	26	27	
			, {14524, 3, -1, 0}// 28	
		};

        private static int[,] _EastSmall =
        {
              {18249, 0, -1, 0},  {18248, -1, -1, 0}, {18250, 1, -1, 0} // 1	2	3	
			, {18251, -1, 0, 0},  {18252, 1, 2, 0},   {18253, 1, 0, 0}  // 4	5	6	
			, {18246, 0, -2, 0},  {18254, -1, 1, 0},  {18255, 0, 1, 0}  // 7	8	9	
			, {18256, 1, 1, 0},   {18257, -1, 2, 0},  {18258, 0, 2, 0}  // 10	11	12	
			, {18259, -1, -2, 0}, {18245, 0, 0, 0},   {18247, 1, -2, 0} // 13	14	15	
		};

        private static int[,] _SouthSmall =
        {
              {18269, 1, 1, 0},  {18270, 1, 0, 0},   {18271, 1, -1, 0}  // 1	2	3	
			, {18272, -2, 0, 0}, {18273, -2, 1, 0},  {18274, -2, -1, 0} // 4	5	6	
			, {18268, 0, -1, 0}, {18260, 0, 0, 0},   {18261, 2, 0, 0}   // 7	8	9	
			, {18264, -1, 0, 0}, {18265, -1, -1, 0}, {18266, 0, 1, 0}   // 10	11	12	
			, {18267, 2, 1, 0},  {18262, 2, -1, 0},  {18263, -1, 1, 0}  // 13	14	15
        };

        public override BaseAddonDeed Deed
        {
            get
            {
                RoseRugAddonDeed deed = new RoseRugAddonDeed(RugType, m_ResourceCount, NextResourceCount);
                deed.IsRewardItem = m_IsRewardItem;

                return deed;
            }
        }

        public RugType RugType { get; set; }

        [Constructable]
        public RoseRugAddon()
            : this(RugType.EastLarge)
        {
        }

        [Constructable]
        public RoseRugAddon(RugType type) : this(type, 0, DateTime.UtcNow)
        {
        }

        [Constructable]
        public RoseRugAddon(RugType type, int resCount, DateTime nextuse)
        {
            NextResourceCount = nextuse;
            RugType = type;
            ResourceCount = resCount;

            int[,] list;

            switch (type)
            {
                default:
                case RugType.EastLarge: list = _EastLarge; break;
                case RugType.SouthLarge: list = _SouthLarge; break;
                case RugType.EastSmall: list = _EastSmall; break;
                case RugType.SouthSmall: list = _SouthSmall; break;
            }

            for (int i = 0; i < list.Length / 4; i++)
                AddComponent(new InternalAddonComponent(list[i, 0]), list[i, 1], list[i, 2], list[i, 3]);
        }

        public override void OnComponentUsed(AddonComponent component, Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(from);

            if (house != null && (house.IsOwner(from) || (house.LockDowns.ContainsKey(this) && house.LockDowns[this] == from)))
            {
                if (m_ResourceCount > 0)
                {
                    Container cont = from.Backpack;

                    Engines.Plants.Seed seed = new Engines.Plants.Seed();

                    if (cont == null || !cont.TryDropItem(from, seed, false))
                    {
                        from.BankBox.DropItem(seed);
                        from.SendLocalizedMessage(1072224); // An item has been placed in your bank box.
                    }
                    else
                        from.SendLocalizedMessage(1072223); // An item has been placed in your backpack.

                    ResourceCount--;
                    NextResourceCount = DateTime.UtcNow + TimeSpan.FromDays(7);
                }
            }
            else
            {
                from.SendLocalizedMessage(502092); // You must be in your house to do 
            }
        }

        private class InternalAddonComponent : AddonComponent
        {
            public override int LabelNumber { get { return 1150121; } } // Rose Rug

            public InternalAddonComponent(int id)
                : base(id)
            {
            }

            public override void GetProperties(ObjectPropertyList list)
            {
                base.GetProperties(list);

                if (Addon is RoseRugAddon)
                {
                    list.Add(1150103, ((RoseRugAddon)Addon).ResourceCount.ToString()); // Messages in Bottles: ~1_val~
                }
            }

            public InternalAddonComponent(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.WriteEncodedInt(0); // version
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadEncodedInt();
            }
        }

        private void TryGiveResourceCount()
        {
            if (NextResourceCount < DateTime.UtcNow)
            {
                ResourceCount = Math.Min(10, m_ResourceCount + 1);
                NextResourceCount = DateTime.UtcNow + TimeSpan.FromDays(7);

                InvalidateAddonPropreties();
            }
        }

        public RoseRugAddon(Serial serial)
            : base(serial)
        {
        }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(3); // Version

            TryGiveResourceCount();

            writer.Write(m_ResourceCount);

            writer.Write((bool)m_IsRewardItem);
            writer.Write(NextResourceCount);
            writer.Write((int)RugType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 3:
                    m_ResourceCount = reader.ReadInt();
                    goto case 2;
                case 2:
                    m_IsRewardItem = reader.ReadBool();
                    NextResourceCount = reader.ReadDateTime();
                    RugType = (RugType)reader.ReadInt();
                    break;
                case 1:
                    NextResourceCount = reader.ReadDateTime();
                    RugType = (RugType)reader.ReadInt();
                    break;
                case 0:
                    NextResourceCount = reader.ReadDateTime();
                    break;
            }
        }
    }

    [TypeAlias("Server.Items.RoseRugSouthAddonDeed", "Server.Items.RoseRugEastAddonDeed")]
    public class RoseRugAddonDeed : BaseAddonDeed, IRewardOption, IRewardItem
    {
        public override BaseAddon Addon
        {
            get
            {
                RoseRugAddon addon = new RoseRugAddon(RugType, m_ResourceCount, NextResourceCount);
                addon.IsRewardItem = m_IsRewardItem;

                return addon;
            }
        }

        public override int LabelNumber
        {
            get
            {
                switch ((int)RugType)
                {
                    default: return 1150048;
                    case 0:
                    case 2: return 1150049;
                }
            }
        }

        private DateTime NextResourceCount;
        private int m_ResourceCount;
        private bool m_IsRewardItem;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return m_IsRewardItem;
            }
            set
            {
                m_IsRewardItem = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ResourceCount
        {
            get
            {
                return m_ResourceCount;
            }
            set
            {
                m_ResourceCount = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public RugType RugType { get; set; }

        [Constructable]
        public RoseRugAddonDeed()
            : this(RugType.EastLarge)
        {
        }

        [Constructable]
        public RoseRugAddonDeed(RugType type)
            : this(type, 0, DateTime.UtcNow)
        {
        }

        [Constructable]
        public RoseRugAddonDeed(RugType type, int resCount, DateTime nextuse)
        {
            RugType = type;
            NextResourceCount = nextuse;
            ResourceCount = resCount;

            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this, 1076583)); // Please select your rug size
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.       	
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add(1, "Rose Rug East 7x7");
            list.Add(2, "Rose Rug South 7x7");
            list.Add(3, "Rose Rug East 3x5");
            list.Add(4, "Rose Rug South 3x5");
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            RugType = (RugType)choice - 1;

            if (!Deleted && IsChildOf(from.Backpack))
                base.OnDoubleClick(from);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
                list.Add(1080457); // 10th Year Veteran Reward
        }

        public RoseRugAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(3); // Version

            writer.Write(m_ResourceCount);

            writer.Write((bool)m_IsRewardItem);
            writer.Write(NextResourceCount);
            writer.Write((int)RugType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 3:
                    m_ResourceCount = reader.ReadInt();
                    goto case 2;
                case 2:
                    m_IsRewardItem = reader.ReadBool();
                    NextResourceCount = reader.ReadDateTime();
                    RugType = (RugType)reader.ReadInt();
                    break;
                case 1:
                    NextResourceCount = reader.ReadDateTime();
                    RugType = (RugType)reader.ReadInt();
                    break;
                case 0:
                    NextResourceCount = reader.ReadDateTime();
                    break;
            }
        }
    }
}
