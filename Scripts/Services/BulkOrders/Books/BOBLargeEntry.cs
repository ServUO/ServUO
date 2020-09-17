using Server.Items;

namespace Server.Engines.BulkOrders
{
    public class BOBLargeEntry
    {
        private readonly bool m_RequireExceptional;
        private readonly BODType m_DeedType;
        private readonly BulkMaterialType m_Material;
        private readonly int m_AmountMax;
        private readonly BOBLargeSubEntry[] m_Entries;
        private int m_Price;

        private GemType m_GemType;

        public BOBLargeEntry(LargeBOD bod)
        {
            m_RequireExceptional = bod.RequireExceptional;

            m_DeedType = bod.BODType;

            m_Material = bod.Material;
            m_AmountMax = bod.AmountMax;

            m_Entries = new BOBLargeSubEntry[bod.Entries.Length];

            for (int i = 0; i < m_Entries.Length; ++i)
                m_Entries[i] = new BOBLargeSubEntry(bod.Entries[i]);

            if (bod is LargeTinkerBOD)
            {
                m_GemType = ((LargeTinkerBOD)bod).GemType;
            }
        }

        public BOBLargeEntry(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 1:
                    {
                        m_GemType = (GemType)reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        m_RequireExceptional = reader.ReadBool();

                        m_DeedType = (BODType)reader.ReadEncodedInt();

                        m_Material = (BulkMaterialType)reader.ReadEncodedInt();
                        m_AmountMax = reader.ReadEncodedInt();
                        m_Price = reader.ReadEncodedInt();

                        m_Entries = new BOBLargeSubEntry[reader.ReadEncodedInt()];

                        for (int i = 0; i < m_Entries.Length; ++i)
                            m_Entries[i] = new BOBLargeSubEntry(reader);

                        break;
                    }
            }
        }

        public bool RequireExceptional => m_RequireExceptional;
        public BODType DeedType => m_DeedType;
        public BulkMaterialType Material => m_Material;
        public int AmountMax => m_AmountMax;
        public int Price
        {
            get
            {
                return m_Price;
            }
            set
            {
                m_Price = value;
            }
        }
        public GemType GemType
        {
            get
            {
                return m_GemType;
            }
            set
            {
                m_GemType = value;
            }
        }
        public BOBLargeSubEntry[] Entries => m_Entries;
        public Item Reconstruct()
        {
            LargeBOD bod = null;

            switch (m_DeedType)
            {
                case BODType.Smith: bod = new LargeSmithBOD(m_AmountMax, m_RequireExceptional, m_Material, ReconstructEntries()); break;
                case BODType.Tailor: bod = new LargeTailorBOD(m_AmountMax, m_RequireExceptional, m_Material, ReconstructEntries()); break;
                case BODType.Inscription: bod = new LargeInscriptionBOD(m_AmountMax, m_RequireExceptional, m_Material, ReconstructEntries()); break;
                case BODType.Alchemy: bod = new LargeAlchemyBOD(m_AmountMax, m_RequireExceptional, m_Material, ReconstructEntries()); break;
                case BODType.Carpentry: bod = new LargeCarpentryBOD(m_AmountMax, m_RequireExceptional, m_Material, ReconstructEntries()); break;
                case BODType.Fletching: bod = new LargeFletchingBOD(m_AmountMax, m_RequireExceptional, m_Material, ReconstructEntries()); break;
                case BODType.Tinkering: bod = new LargeTinkerBOD(m_AmountMax, m_RequireExceptional, m_Material, ReconstructEntries(), m_GemType); break;
                case BODType.Cooking: bod = new LargeCookingBOD(m_AmountMax, m_RequireExceptional, m_Material, ReconstructEntries()); break;
            }

            for (int i = 0; bod != null && i < bod.Entries.Length; ++i)
                bod.Entries[i].Owner = bod;

            return bod;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(1); // version

            writer.Write((int)m_GemType);

            writer.Write(m_RequireExceptional);

            writer.WriteEncodedInt((int)m_DeedType);
            writer.WriteEncodedInt((int)m_Material);
            writer.WriteEncodedInt(m_AmountMax);
            writer.WriteEncodedInt(m_Price);

            writer.WriteEncodedInt(m_Entries.Length);

            for (int i = 0; i < m_Entries.Length; ++i)
                m_Entries[i].Serialize(writer);
        }

        private LargeBulkEntry[] ReconstructEntries()
        {
            LargeBulkEntry[] entries = new LargeBulkEntry[m_Entries.Length];

            for (int i = 0; i < m_Entries.Length; ++i)
            {
                entries[i] = new LargeBulkEntry(null, new SmallBulkEntry(m_Entries[i].ItemType, m_Entries[i].Number, m_Entries[i].Graphic, m_Entries[i].Hue))
                {
                    Amount = m_Entries[i].AmountCur
                };
            }

            return entries;
        }
    }
}