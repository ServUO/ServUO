namespace Server.Items
{
    class RareItem : Item
    {
        private int m_ItemId;
        private int m_ItemHue;
        private string m_ItemName;
        private string m_Context;

        public int ItemId
        {
            get { return m_ItemId; }
            set { m_ItemId = value; InvalidateProperties(); }
        }

        public int ItemHue
        {
            get { return m_ItemHue; }
            set { m_ItemHue = value; InvalidateProperties(); }
        }

        public string ItemName
        {
            get { return m_ItemName; }
            set { m_ItemName = value; InvalidateProperties(); }
        }

        public string Context
        {
            get { return m_Context; }
            set { m_Context = value; InvalidateProperties(); }
        }

        [Constructable]
        public RareItem(int itemid, int itemhue, string name, bool blessed = false, string context = "")
        {
            ItemID = itemid;
            Hue = itemhue;
            Name = name;
            Weight = 1;

            if (blessed)
                LootType = LootType.Blessed;

            if (context != "")
                m_Context = context;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if(m_Context != "")
                list.Add(m_Context);
        }

        public RareItem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);

            writer.Write(m_ItemId);
            writer.Write(m_ItemHue);
            writer.Write(m_ItemName);
            writer.Write(m_Context);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_ItemId = reader.ReadInt();
            m_ItemHue = reader.ReadInt();
            m_ItemName = reader.ReadString();
            m_Context = reader.ReadString();
        }
    }
}