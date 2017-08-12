using System;

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

            switch ( version )
            {
                case 1:
                    {
                        this.m_Type = reader.ReadEncodedInt();
                        this.m_Quality = reader.ReadEncodedInt();
                        this.m_Material = reader.ReadEncodedInt();
                        this.m_Quantity = reader.ReadEncodedInt();

                        break;
                    }
            }
        }

        public bool IsDefault
        {
            get
            {
                return (this.m_Type == 0 && this.m_Quality == 0 && this.m_Material == 0 && this.m_Quantity == 0);
            }
        }
        public int Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
            }
        }
        public int Quality
        {
            get
            {
                return this.m_Quality;
            }
            set
            {
                this.m_Quality = value;
            }
        }
        public int Material
        {
            get
            {
                return this.m_Material;
            }
            set
            {
                this.m_Material = value;
            }
        }
        public int Quantity
        {
            get
            {
                return this.m_Quantity;
            }
            set
            {
                this.m_Quantity = value;
            }
        }
        public void Clear()
        {
            this.m_Type = 0;
            this.m_Quality = 0;
            this.m_Material = 0;
            this.m_Quantity = 0;
        }

        public void Serialize(GenericWriter writer)
        {
            if (this.IsDefault)
            {
                writer.WriteEncodedInt(0); // version
            }
            else
            {
                writer.WriteEncodedInt(1); // version

                writer.WriteEncodedInt(this.m_Type);
                writer.WriteEncodedInt(this.m_Quality);
                writer.WriteEncodedInt(this.m_Material);
                writer.WriteEncodedInt(this.m_Quantity);
            }
        }
    }
}