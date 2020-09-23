using Server.Engines.VeteranRewards;

namespace Server.Items
{
    [Flipable(0x9AA, 0xE7D)]
    public class CommodityDeedBox : BaseContainer, IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public CommodityDeedBox()
            : base(0x9AA)
        {
            Hue = 0x47;
            Weight = 4.0;
        }

        public CommodityDeedBox(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1080523;// Commodity Deed Box
        public override int DefaultGumpID => 0x43;
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

            if (m_IsRewardItem)
                list.Add(1076217); // 1st Year Veteran Reward		
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
}
