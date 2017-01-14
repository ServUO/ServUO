using System;
using Server.Engines.VeteranRewards;

namespace Server.Items
{ 
    [Furniture]
    [Flipable(0x9AA, 0xE7D)]
    public class CommodityDeedBox : BaseContainer, IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public CommodityDeedBox()
            : base(0x9AA)
        {
            this.Hue = 0x47;
            this.Weight = 4.0;
        }

        public CommodityDeedBox(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1080523;
            }
        }// Commodity Deed Box
        public override int DefaultGumpID
        {
            get
            {
                return 0x43;
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
                this.InvalidateProperties();
            }
        }
        public static CommodityDeedBox Find(Item deed)
        {
            Item parent = deed;

            while (parent != null && !(parent is CommodityDeedBox))
                parent = parent.Parent as Item;

            return parent as CommodityDeedBox;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			
            if (this.m_IsRewardItem)
                list.Add(1076217); // 1st Year Veteran Reward		
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write((bool)this.m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_IsRewardItem = reader.ReadBool();
        }
    }
}