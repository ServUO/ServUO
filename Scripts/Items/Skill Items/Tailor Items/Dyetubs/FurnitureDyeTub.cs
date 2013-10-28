using System;

namespace Server.Items
{
    public class FurnitureDyeTub : DyeTub, Engines.VeteranRewards.IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public FurnitureDyeTub()
        {
            this.LootType = LootType.Blessed;
        }

        public FurnitureDyeTub(Serial serial)
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
        public override bool AllowFurniture
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
                return 501019;
            }
        }// Select the furniture to dye.
        public override int FailMessage
        {
            get
            {
                return 501021;
            }
        }// That is not a piece of furniture.
        public override int LabelNumber
        {
            get
            {
                return 1041246;
            }
        }// Furniture Dye Tub
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

            if (this.LootType == LootType.Regular)
                this.LootType = LootType.Blessed;
        }
    }
}