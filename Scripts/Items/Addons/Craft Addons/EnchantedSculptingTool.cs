using Server.Engines.Craft;
using Server.Engines.VeteranRewards;
using Server.Gumps;

namespace Server.Items
{
    public class EnchantedSculptingToolAddon : CraftAddon
    {
        public override CraftSystem CraftSystem => DefMasonry.CraftSystem;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem { get; set; }

        public override BaseAddonDeed Deed
        {
            get
            {
                EnchantedSculptingToolDeed deed = new EnchantedSculptingToolDeed(Tools.Count > 0 ? Tools[0].UsesRemaining : 0)
                {
                    IsRewardItem = IsRewardItem
                };

                return deed;
            }
        }

        [Constructable]
        public EnchantedSculptingToolAddon(DirectionType type, int uses)
        {
            switch (type)
            {
                case DirectionType.South:
                    AddCraftComponent(new AddonToolComponent(CraftSystem, 0xA538, 0xA541, 1157988, 1157987, 1029241, uses, this), 0, 0, 0);
                    AddComponent(new ToolDropComponent(0xA549, 1029241), 0, 1, 0);
                    break;
                case DirectionType.East:
                    AddCraftComponent(new AddonToolComponent(CraftSystem, 0xA540, 0xA539, 1157988, 1157987, 1029241, uses, this), 0, 0, 0);
                    AddComponent(new ToolDropComponent(0xA548, 1029241), 1, 0, 0);
                    break;
            }
        }

        public EnchantedSculptingToolAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            IsRewardItem = reader.ReadBool();
        }
    }

    public class EnchantedSculptingToolDeed : CraftAddonDeed, IRewardItem, IRewardOption
    {
        public override int LabelNumber => 1159421;  // Enchanted Sculpting Tool

        public override BaseAddon Addon
        {
            get
            {
                EnchantedSculptingToolAddon addon = new EnchantedSculptingToolAddon(_Direction, UsesRemaining)
                {
                    IsRewardItem = m_IsRewardItem
                };

                return addon;
            }
        }

        private DirectionType _Direction;

        private bool m_IsRewardItem;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set
            {
                m_IsRewardItem = value;
                InvalidateProperties();
            }
        }

        [Constructable]
        public EnchantedSculptingToolDeed()
            : this(0)
        {
        }

        [Constructable]
        public EnchantedSculptingToolDeed(int uses)
            : base(uses)
        {
            LootType = LootType.Blessed;
        }

        public EnchantedSculptingToolDeed(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
                list.Add(1076223); // 7th Year Veteran Reward
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)DirectionType.South, 1075386); // South
            list.Add((int)DirectionType.East, 1075387); // East
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            _Direction = (DirectionType)choice;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(AddonOptionGump));
                from.SendGump(new AddonOptionGump(this, 1154194)); // Choose a Facing:
            }
            else
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_IsRewardItem = reader.ReadBool();
        }
    }
}
