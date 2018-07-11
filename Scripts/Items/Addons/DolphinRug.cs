using System;
using Server;
using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Multis;

namespace Server.Items
{
    [TypeAlias("Server.Items.DolphinRugEastAddon", "Server.Items.DolphinRugSouthAddon")]
    public class DolphinRugAddon : BaseAddon, IRewardItem
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

        private static int[,] _EastLarge = new int[,]
        {
              {14590, 1, -1, 0}, {14586, 1, -2, 0}, {14589, 0, -1, 0}// 1	2	3	
			, {14592, -1, -1, 0}, {14593, 0, 0, 0}, {14597, 0, 1, 0}// 4	5	6	
			, {14596, -1, 0, 0}, {14600, -1, 1, 0}, {14604, -1, 2, 0}// 7	8	9	
			, {14585, 0, -2, 0}, {14587, 2, -2, 0}, {14591, 2, -1, 0}// 10	11	12	
			, {14601, 0, 2, 0}, {14588, -1, -2, 0}, {14581, -1, -3, 0}// 13	14	15	
			, {14582, 0, -3, 0}, {14583, 1, -3, 0}, {14584, 2, -3, 0}// 16	17	18	
			, {14605, 0, 3, 0}, {14606, 1, 3, 0}, {14595, 2, 0, 0}// 19	20	21	
			, {14599, 2, 1, 0}, {14603, 2, 2, 0}, {14608, -1, 3, 0}// 22	23	24	
			, {14607, 2, 3, 0}, {14594, 1, 0, 0}, {14602, 1, 1, 0}// 25	26	27	
			, {14598, 1, 2, 0}// 28	
		};

        private static int[,] _SouthLarge = new int[,]
        {
              {14553, -3, 2, 0}, {14554, -3, 1, 0}, {14555, -3, 0, 0}// 1	2	3	
			, {14556, -3, -1, 0}, {14557, -2, 1, 0}, {14558, -2, 0, 0}// 4	5	6	
			, {14559, -2, -1, 0}, {14560, -2, 2, 0}, {14561, -1, 1, 0}// 7	8	9	
			, {14562, -1, 0, 0}, {14564, -1, 2, 0}, {14565, 0, 1, 0}// 10	11	12	
			, {14568, 0, 2, 0}, {14563, -1, -1, 0}, {14567, 0, -1, 0}// 13	14	15	
			, {14566, 0, 0, 0}, {14571, 1, -1, 0}, {14578, 3, 0, 0}// 16	17	18	
			, {14572, 1, 2, 0}, {14569, 1, 1, 0}, {14576, 2, 2, 0}// 19	20	21	
			, {14580, 3, 2, 0}, {14573, 2, 1, 0}, {14577, 3, 1, 0}// 22	23	24	
			, {14579, 3, -1, 0}, {14575, 2, -1, 0}, {14570, 1, 0, 0}// 25	26	27	
			, {14574, 2, 0, 0}// 28	
		};

        private static int[,] _EastSmall =
        {
              {18283, 1, 0, 0}, {18276, 1, 1, 0},   {18289, 1, 2, 0}    // 1	2	3	
			, {18288, 0, 2, 0}, {18287, -1, -2, 0}, {18286, 1, -2, 0}   // 4	5	6	
			, {18285, 0, -2, 0}, {18284, -1, 2, 0}, {18282, -1, 1, 0}   // 7	8	9	
			, {18281, -1, 0, 0}, {18280, 1, -1, 0}, {18279, 0, -1, 0}   // 10	11	12	
			, {18278, -1, -1, 0}, {18277, 0, 1, 0}, {18275, 0, 0, 0}    // 13	14	15	
		};

        private static int[,] _SouthSmall =
        {
              {18393, 2, -1, 0},  {18392, -2, -1, 0}, {18391, -2, 1, 0} // 1	2	3	
			, {18390, 1, -1, 0},  {18300, 1, 0, 0},   {18299, 1, 1, 0}  // 4	5	6	
			, {18298, 0, -1, 0},  {18297, 2, 0, 0},   {18296, 0, 1, 0}  // 7	8	9	
			, {18295, -1, -1, 0}, {18294, -1, 0, 0},  {18293, -1, 1, 0} // 10	11	12	
			, {18292, 2, 1, 0},   {18291, -2, 0, 0},  {18290, 0, 0, 0}  // 13	14	15	
        };

        public override BaseAddonDeed Deed
        {
            get
            {
                DolphinRugAddonDeed deed = new DolphinRugAddonDeed(RugType, m_ResourceCount, NextResourceCount);
                deed.IsRewardItem = m_IsRewardItem;

                return deed;
            }
        }

        public RugType RugType { get; set; }

        [Constructable]
        public DolphinRugAddon()
            : this(RugType.EastLarge)
        {
        }

        [Constructable]
        public DolphinRugAddon(RugType type)
            : this(type, 0, DateTime.UtcNow)
        {
        }

        [Constructable]
        public DolphinRugAddon(RugType type, int resCount, DateTime nextuse)
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

                    MessageInABottle mib = new MessageInABottle();

                    if (cont == null || !cont.TryDropItem(from, mib, false))
                    {
                        from.BankBox.DropItem(mib);
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
                from.SendLocalizedMessage(502092); // You must be in your house to do this.
            }
        }

        private class InternalAddonComponent : AddonComponent
        {
            public override int LabelNumber { get { return 1150122; } } // Dolphin Rug

            public InternalAddonComponent(int id)
                : base(id)
            {
            }

            public override void GetProperties(ObjectPropertyList list)
            {
                base.GetProperties(list);

                if (Addon is DolphinRugAddon)
                {
                    list.Add(1150103, ((DolphinRugAddon)Addon).ResourceCount.ToString()); // Messages in Bottles: ~1_val~
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

        public DolphinRugAddon(Serial serial)
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

    [TypeAlias("Server.Items.DolphinRugSouthAddonDeed", "Server.Items.DolphinRugEastAddonDeed")]
    public class DolphinRugAddonDeed : BaseAddonDeed, IRewardOption, IRewardItem
    {
        public override BaseAddon Addon
        {
            get
            {
                DolphinRugAddon addon = new DolphinRugAddon(RugType, m_ResourceCount, NextResourceCount);
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
                    default: return 1150050;
                    case 0:
                    case 2: return 1150051;
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
        public DolphinRugAddonDeed()
            : this(RugType.EastLarge)
        {
        }

        [Constructable]
        public DolphinRugAddonDeed(RugType type)
            : this(type, 0, DateTime.UtcNow)
        {
        }

        [Constructable]
        public DolphinRugAddonDeed(RugType type, int resCount, DateTime nextuse)
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
            list.Add(1, "Dolphin Rug East 7x7");
            list.Add(2, "Dolphin Rug South 7x7");
            list.Add(3, "Dolphin Rug East 3x5");
            list.Add(4, "Dolphin Rug South 3x5");
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

        public DolphinRugAddonDeed(Serial serial)
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
