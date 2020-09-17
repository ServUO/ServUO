using Server.Engines.VeteranRewards;
using Server.Gumps;

namespace Server.Items
{
    public enum MinotaurStatueType
    {
        AttackSouth = 100,
        AttackEast = 101,
        DefendSouth = 102,
        DefendEast = 103
    }

    public class MinotaurStatue : BaseAddon, IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public MinotaurStatue(MinotaurStatueType type)
            : base()
        {
            switch (type)
            {
                case MinotaurStatueType.AttackSouth:
                    AddComponent(new AddonComponent(0x306C), 0, 0, 0);
                    AddComponent(new AddonComponent(0x306D), -1, 0, 0);
                    AddComponent(new AddonComponent(0x306E), 0, -1, 0);
                    break;
                case MinotaurStatueType.AttackEast:
                    AddComponent(new AddonComponent(0x3074), 0, 0, 0);
                    AddComponent(new AddonComponent(0x3075), -1, 0, 0);
                    AddComponent(new AddonComponent(0x3076), 0, -1, 0);
                    break;
                case MinotaurStatueType.DefendSouth:
                    AddComponent(new AddonComponent(0x3072), 0, 0, 0);
                    AddComponent(new AddonComponent(0x3073), 0, -1, 0);
                    break;
                case MinotaurStatueType.DefendEast:
                    AddComponent(new AddonComponent(0x306F), 0, 0, 0);
                    AddComponent(new AddonComponent(0x3070), -1, 0, 0);
                    AddComponent(new AddonComponent(0x3071), 0, -1, 0);
                    break;
            }
        }

        public MinotaurStatue(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                MinotaurStatueDeed deed = new MinotaurStatueDeed
                {
                    IsRewardItem = m_IsRewardItem
                };

                return deed;
            }
        }
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
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_IsRewardItem = reader.ReadBool();
        }
    }

    public class MinotaurStatueDeed : BaseAddonDeed, IRewardItem, IRewardOption
    {
        private MinotaurStatueType m_StatueType;
        private bool m_IsRewardItem;
        [Constructable]
        public MinotaurStatueDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public MinotaurStatueDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1080409;// Minotaur Statue Deed
        public override BaseAddon Addon
        {
            get
            {
                MinotaurStatue addon = new MinotaurStatue(m_StatueType)
                {
                    IsRewardItem = m_IsRewardItem
                };

                return addon;
            }
        }
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
        public override void OnDoubleClick(Mobile from)
        {
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
                return;

            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this));
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.    
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
                list.Add(1076218); // 2nd Year Veteran Reward
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_IsRewardItem = reader.ReadBool();
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)MinotaurStatueType.AttackSouth, 1080410); // Minotaur Attack South
            list.Add((int)MinotaurStatueType.AttackEast, 1080411); // Minotaur Attack East
            list.Add((int)MinotaurStatueType.DefendSouth, 1080412); // Minotaur Defend South
            list.Add((int)MinotaurStatueType.DefendEast, 1080413); // Minotaur Defend East
        }

        public void OnOptionSelected(Mobile from, int option)
        {
            m_StatueType = (MinotaurStatueType)option;

            if (!Deleted)
                base.OnDoubleClick(from);
        }
    }
}