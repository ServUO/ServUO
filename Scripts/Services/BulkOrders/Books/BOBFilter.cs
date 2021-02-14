namespace Server.Engines.BulkOrders
{
    public class BOBFilter
    {
        private int m_Type;
        private int m_Quality;
        private int m_Material;
        private int m_Quantity;

        public BOBFilter()
        {
        }

        public BOBFilter(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 1:
                    {
                        m_Type = reader.ReadEncodedInt();
                        m_Quality = reader.ReadEncodedInt();
                        m_Material = reader.ReadEncodedInt();
                        m_Quantity = reader.ReadEncodedInt();

                        break;
                    }
            }
        }

        public bool IsDefault => (m_Type == 0 && m_Quality == 0 && m_Material == 0 && m_Quantity == 0);
        public int Type
        {
            get => m_Type;
            set => m_Type = value;
        }
        public int Quality
        {
            get => m_Quality;
            set => m_Quality = value;
        }
        public int Material
        {
            get => m_Material;
            set => m_Material = value;
        }
        public int Quantity
        {
            get => m_Quantity;
            set => m_Quantity = value;
        }
        public void Clear()
        {
            m_Type = 0;
            m_Quality = 0;
            m_Material = 0;
            m_Quantity = 0;
        }

        public void Serialize(GenericWriter writer)
        {
            if (IsDefault)
            {
                writer.WriteEncodedInt(0); // version
            }
            else
            {
                writer.WriteEncodedInt(1); // version

                writer.WriteEncodedInt(m_Type);
                writer.WriteEncodedInt(m_Quality);
                writer.WriteEncodedInt(m_Material);
                writer.WriteEncodedInt(m_Quantity);
            }
        }
    }
}
