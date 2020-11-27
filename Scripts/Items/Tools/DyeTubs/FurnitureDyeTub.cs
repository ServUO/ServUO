using System;

namespace Server.Items
{
    public class FurnitureDyeTub : DyeTub, Engines.VeteranRewards.IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public FurnitureDyeTub()
        {
            LootType = LootType.Blessed;
        }

        public FurnitureDyeTub(Serial serial)
            : base(serial)
        {
        }

        public override bool AllowDyables => false;
        public override bool AllowFurniture => true;
        public override int TargetMessage => 501019;// Select the furniture to dye.
        public override int FailMessage => 501021;// That is not a piece of furniture.
        public override int LabelNumber => 1041246;// Furniture Dye Tub

        private static Type[] _Dyables = new[]
        {
            typeof(PotionKeg), typeof(CustomizableSquaredDoorMatDeed), typeof(OrnateBedDeed),
            typeof(FourPostBedDeed), typeof(FormalDiningTableDeed)
        };

        public override Type[] ForcedDyables => _Dyables;

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
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (m_IsRewardItem && !Engines.VeteranRewards.RewardSystem.CheckIsUsableBy(from, this, null))
                return;

            base.OnDoubleClick(from);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
                list.Add(1076217); // 1st Year Veteran Reward
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            writer.Write(m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_IsRewardItem = reader.ReadBool();
                        break;
                    }
            }
        }
    }
}
