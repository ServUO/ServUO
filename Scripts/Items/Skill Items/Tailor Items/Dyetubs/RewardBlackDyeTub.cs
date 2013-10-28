using System;

namespace Server.Items
{
    public class RewardBlackDyeTub : DyeTub, Engines.VeteranRewards.IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public RewardBlackDyeTub()
        {
            this.Hue = this.DyedHue = 0x0001;
            this.Redyable = false;
            this.LootType = LootType.Blessed;
        }

        public RewardBlackDyeTub(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1006008;
            }
        }// Black Dye Tub
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return this.m_IsRewardItem;
            }
            set
            {
                this.m_IsRewardItem = value;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_IsRewardItem && !Engines.VeteranRewards.RewardSystem.CheckIsUsableBy(from, this, null))
                return;

            base.OnDoubleClick(from);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Core.ML && this.m_IsRewardItem)
                list.Add(1076217); // 1st Year Veteran Reward
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((bool)this.m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_IsRewardItem = reader.ReadBool();
                        break;
                    }
            }
        }
    }
}