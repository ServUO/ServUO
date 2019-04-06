using System;

namespace Server.Items
{
    public class MetallicDyeTub : DyeTub, Engines.VeteranRewards.IRewardItem
    {
        private bool m_IsRewardItem;

        [Constructable]
        public MetallicDyeTub()
        {
            LootType = LootType.Blessed;
        }

        public MetallicDyeTub(Serial serial)
            : base(serial)
        {
        }

        public override bool AllowDyables { get { return false; } }
        public override bool AllowMetal { get { return true; } }
        public override int TargetMessage { get { return 1080393; } } // Select the metal item to dye.
        public override int FailMessage { get { return 1080394; } } // You can only dye metal with this tub.
        public override int LabelNumber { get { return 1150067; } } // Metallic Dye Tub
        public override CustomHuePicker CustomHuePicker { get { return CustomHuePicker.MetallicDyeTub; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get { return m_IsRewardItem; }
            set { m_IsRewardItem = value; }
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

            if (Core.ML && m_IsRewardItem)
                list.Add(1076221); // 5th Year Veteran Reward
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((bool)m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_IsRewardItem = reader.ReadBool();
        }
    }
}