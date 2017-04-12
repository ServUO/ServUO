using System;

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

        public BOBLargeEntry(LargeBOD bod)
        {
            this.m_RequireExceptional = bod.RequireExceptional;

            m_DeedType = bod.BODType;

            this.m_Material = bod.Material;
            this.m_AmountMax = bod.AmountMax;

            this.m_Entries = new BOBLargeSubEntry[bod.Entries.Length];

            for (int i = 0; i < this.m_Entries.Length; ++i)
                this.m_Entries[i] = new BOBLargeSubEntry(bod.Entries[i]);
        }

        public BOBLargeEntry(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_RequireExceptional = reader.ReadBool();

                        this.m_DeedType = (BODType)reader.ReadEncodedInt();

                        this.m_Material = (BulkMaterialType)reader.ReadEncodedInt();
                        this.m_AmountMax = reader.ReadEncodedInt();
                        this.m_Price = reader.ReadEncodedInt();

                        this.m_Entries = new BOBLargeSubEntry[reader.ReadEncodedInt()];

                        for (int i = 0; i < this.m_Entries.Length; ++i)
                            this.m_Entries[i] = new BOBLargeSubEntry(reader);

                        break;
                    }
            }
        }

        public bool RequireExceptional
        {
            get
            {
                return this.m_RequireExceptional;
            }
        }
        public BODType DeedType
        {
            get
            {
                return this.m_DeedType;
            }
        }
        public BulkMaterialType Material
        {
            get
            {
                return this.m_Material;
            }
        }
        public int AmountMax
        {
            get
            {
                return this.m_AmountMax;
            }
        }
        public int Price
        {
            get
            {
                return this.m_Price;
            }
            set
            {
                this.m_Price = value;
            }
        }
        public BOBLargeSubEntry[] Entries
        {
            get
            {
                return this.m_Entries;
            }
        }
        public Item Reconstruct()
        {
            LargeBOD bod = null;

            switch (m_DeedType)
            {
                case BODType.Smith: bod = new LargeSmithBOD(this.m_AmountMax, this.m_RequireExceptional, this.m_Material, this.ReconstructEntries()); break;
                case BODType.Tailor: bod = new LargeTailorBOD(this.m_AmountMax, this.m_RequireExceptional, this.m_Material, this.ReconstructEntries()); break;
                case BODType.Inscription: bod = new LargeInscriptionBOD(this.m_AmountMax, this.m_RequireExceptional, this.m_Material, this.ReconstructEntries()); break;
                case BODType.Alchemy: bod = new LargeAlchemyBOD(this.m_AmountMax, this.m_RequireExceptional, this.m_Material, this.ReconstructEntries()); break;
                case BODType.Carpentry: bod = new LargeCarpentryBOD(this.m_AmountMax, this.m_RequireExceptional, this.m_Material, this.ReconstructEntries()); break;
                case BODType.Fletching: bod = new LargeFletchingBOD(this.m_AmountMax, this.m_RequireExceptional, this.m_Material, this.ReconstructEntries()); break;
                case BODType.Tinkering: bod = new LargeTinkerBOD(this.m_AmountMax, this.m_RequireExceptional, this.m_Material, this.ReconstructEntries()); break;
                case BODType.Cooking: bod = new LargeCookingBOD(this.m_AmountMax, this.m_RequireExceptional, this.m_Material, this.ReconstructEntries()); break;
            }

            for (int i = 0; bod != null && i < bod.Entries.Length; ++i)
                bod.Entries[i].Owner = bod;

            return bod;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); // version

            writer.Write((bool)this.m_RequireExceptional);

            writer.WriteEncodedInt((int)this.m_DeedType);
            writer.WriteEncodedInt((int)this.m_Material);
            writer.WriteEncodedInt((int)this.m_AmountMax);
            writer.WriteEncodedInt((int)this.m_Price);

            writer.WriteEncodedInt((int)this.m_Entries.Length);

            for (int i = 0; i < this.m_Entries.Length; ++i)
                this.m_Entries[i].Serialize(writer);
        }

        private LargeBulkEntry[] ReconstructEntries()
        {
            LargeBulkEntry[] entries = new LargeBulkEntry[this.m_Entries.Length];

            for (int i = 0; i < this.m_Entries.Length; ++i)
            {
                entries[i] = new LargeBulkEntry(null, new SmallBulkEntry(this.m_Entries[i].ItemType, this.m_Entries[i].Number, this.m_Entries[i].Graphic, this.m_Entries[i].Hue));
                entries[i].Amount = this.m_Entries[i].AmountCur;
            }

            return entries;
        }
    }
}