using Server.Engines.VeteranRewards;
using Server.Gumps;

namespace Server.Items
{
    public class WaterWheelAddon : BaseAddon, IWaterSource
    {
        [Constructable]
        public WaterWheelAddon(DirectionType type)
        {
            switch (type)
            {
                case DirectionType.South:
                    AddComponent(new LocalizedAddonComponent(0xA0F8, 1125222), 0, 0, 0);
                    break;
                case DirectionType.East:
                    AddComponent(new LocalizedAddonComponent(0xA0EE, 1125222), 0, 0, 0);
                    break;
            }
        }

        public int Quantity
        {
            get { return 500; }
            set { }
        }

        public WaterWheelAddon(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem { get; set; }

        public override BaseAddonDeed Deed
        {
            get
            {
                WaterWheelDeed deed = new WaterWheelDeed
                {
                    IsRewardItem = IsRewardItem
                };

                return deed;
            }
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

    public class WaterWheelDeed : BaseAddonDeed, IRewardItem, IRewardOption
    {
        public override int LabelNumber => 1158881;  // Water Wheel

        public override BaseAddon Addon
        {
            get
            {
                WaterWheelAddon addon = new WaterWheelAddon(_Direction)
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

        [Constructable]
        public WaterWheelDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public WaterWheelDeed(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
                list.Add(1080457); // 10th Year Veteran Reward
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
