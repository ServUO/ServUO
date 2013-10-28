using System;

namespace Server.Items
{
    public class StatuetteDyeTub : DyeTub, Engines.VeteranRewards.IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public StatuetteDyeTub()
        {
            this.LootType = LootType.Blessed;
        }

        public StatuetteDyeTub(Serial serial)
            : base(serial)
        {
        }

        public override bool AllowDyables
        {
            get
            {
                return false;
            }
        }
        public override bool AllowStatuettes
        {
            get
            {
                return true;
            }
        }
        public override int TargetMessage
        {
            get
            {
                return 1049777;
            }
        }// Target the statuette to dye
        public override int FailMessage
        {
            get
            {
                return 1049778;
            }
        }// You can only dye veteran reward statuettes with this tub.
        public override int LabelNumber
        {
            get
            {
                return 1049741;
            }
        }// Reward Statuette Dye Tub
        public override CustomHuePicker CustomHuePicker
        {
            get
            {
                return CustomHuePicker.LeatherDyeTub;
            }
        }
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
                list.Add(1076221); // 5th Year Veteran Reward
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