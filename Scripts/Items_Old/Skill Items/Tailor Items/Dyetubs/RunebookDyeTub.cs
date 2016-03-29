using System;

namespace Server.Items
{
    public class RunebookDyeTub : DyeTub, Engines.VeteranRewards.IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public RunebookDyeTub()
        {
            this.LootType = LootType.Blessed;
        }

        public RunebookDyeTub(Serial serial)
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
        public override bool AllowRunebooks
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
                return 1049774;
            }
        }// Target the runebook or runestone to dye
        public override int FailMessage
        {
            get
            {
                return 1049775;
            }
        }// You can only dye runestones or runebooks with this tub.
        public override int LabelNumber
        {
            get
            {
                return 1049740;
            }
        }// Runebook Dye Tub
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
                list.Add(1076220); // 4th Year Veteran Reward
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