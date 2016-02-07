using System;

namespace Server.Engines.BulkOrders
{
    public class BOBSmallEntry
    {
        private readonly Type m_ItemType;
        private readonly bool m_RequireExceptional;
        private readonly BODType m_DeedType;
        private readonly BulkMaterialType m_Material;
        private readonly int m_AmountCur;
        private readonly int m_AmountMax;
        private readonly int m_Number;
        private readonly int m_Graphic;
        private int m_Price;
        public BOBSmallEntry(SmallBOD bod)
        {
            this.m_ItemType = bod.Type;
            this.m_RequireExceptional = bod.RequireExceptional;

            if (bod is SmallTailorBOD)
                this.m_DeedType = BODType.Tailor;
            else if (bod is SmallSmithBOD)
                this.m_DeedType = BODType.Smith;

            this.m_Material = bod.Material;
            this.m_AmountCur = bod.AmountCur;
            this.m_AmountMax = bod.AmountMax;
            this.m_Number = bod.Number;
            this.m_Graphic = bod.Graphic;
        }

        public BOBSmallEntry(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 0:
                    {
                        string type = reader.ReadString();

                        if (type != null)
                            this.m_ItemType = ScriptCompiler.FindTypeByFullName(type);

                        this.m_RequireExceptional = reader.ReadBool();

                        this.m_DeedType = (BODType)reader.ReadEncodedInt();

                        this.m_Material = (BulkMaterialType)reader.ReadEncodedInt();
                        this.m_AmountCur = reader.ReadEncodedInt();
                        this.m_AmountMax = reader.ReadEncodedInt();
                        this.m_Number = reader.ReadEncodedInt();
                        this.m_Graphic = reader.ReadEncodedInt();
                        this.m_Price = reader.ReadEncodedInt();

                        break;
                    }
            }
        }

        public Type ItemType
        {
            get
            {
                return this.m_ItemType;
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
        public int AmountCur
        {
            get
            {
                return this.m_AmountCur;
            }
        }
        public int AmountMax
        {
            get
            {
                return this.m_AmountMax;
            }
        }
        public int Number
        {
            get
            {
                return this.m_Number;
            }
        }
        public int Graphic
        {
            get
            {
                return this.m_Graphic;
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
        public Item Reconstruct()
        {
            SmallBOD bod = null;

            if (this.m_DeedType == BODType.Smith)
                bod = new SmallSmithBOD(this.m_AmountCur, this.m_AmountMax, this.m_ItemType, this.m_Number, this.m_Graphic, this.m_RequireExceptional, this.m_Material);
            else if (this.m_DeedType == BODType.Tailor)
                bod = new SmallTailorBOD(this.m_AmountCur, this.m_AmountMax, this.m_ItemType, this.m_Number, this.m_Graphic, this.m_RequireExceptional, this.m_Material);

            return bod;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); // version

            writer.Write(this.m_ItemType == null ? null : this.m_ItemType.FullName);

            writer.Write((bool)this.m_RequireExceptional);

            writer.WriteEncodedInt((int)this.m_DeedType);
            writer.WriteEncodedInt((int)this.m_Material);
            writer.WriteEncodedInt((int)this.m_AmountCur);
            writer.WriteEncodedInt((int)this.m_AmountMax);
            writer.WriteEncodedInt((int)this.m_Number);
            writer.WriteEncodedInt((int)this.m_Graphic);
            writer.WriteEncodedInt((int)this.m_Price);
        }
    }
}