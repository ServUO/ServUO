using Server.Items;
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
        private readonly int m_Hue;
        private readonly GemType m_GemType;

        private int m_Price;

        public BOBSmallEntry(SmallBOD bod)
        {
            m_ItemType = bod.Type;
            m_RequireExceptional = bod.RequireExceptional;

            m_DeedType = bod.BODType;

            m_Material = bod.Material;
            m_AmountCur = bod.AmountCur;
            m_AmountMax = bod.AmountMax;
            m_Number = bod.Number;
            m_Graphic = bod.Graphic;
            m_Hue = bod.GraphicHue;

            if (bod is SmallTinkerBOD)
            {
                m_GemType = ((SmallTinkerBOD)bod).GemType;
            }
        }

        public BOBSmallEntry(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch (version)
            {
                case 2:
                    {
                        m_GemType = (GemType)reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        m_Hue = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        string type = reader.ReadString();

                        if (type != null)
                            m_ItemType = ScriptCompiler.FindTypeByFullName(type);

                        m_RequireExceptional = reader.ReadBool();

                        m_DeedType = (BODType)reader.ReadEncodedInt();

                        m_Material = (BulkMaterialType)reader.ReadEncodedInt();
                        m_AmountCur = reader.ReadEncodedInt();
                        m_AmountMax = reader.ReadEncodedInt();
                        m_Number = reader.ReadEncodedInt();
                        m_Graphic = reader.ReadEncodedInt();
                        m_Price = reader.ReadEncodedInt();

                        break;
                    }
            }
        }

        public Type ItemType => m_ItemType;
        public bool RequireExceptional => m_RequireExceptional;
        public BODType DeedType => m_DeedType;
        public BulkMaterialType Material => m_Material;
        public int AmountCur => m_AmountCur;
        public int AmountMax => m_AmountMax;
        public int Number => m_Number;
        public int Graphic => m_Graphic;
        public int Hue => m_Hue;
        public GemType GemType => m_GemType;
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
        public Item Reconstruct()
        {
            SmallBOD bod = null;

            switch (m_DeedType)
            {
                case BODType.Smith: bod = new SmallSmithBOD(m_AmountCur, m_AmountMax, m_ItemType, m_Number, m_Graphic, m_RequireExceptional, m_Material, m_Hue); break;
                case BODType.Tailor: bod = new SmallTailorBOD(m_AmountCur, m_AmountMax, m_ItemType, m_Number, m_Graphic, m_RequireExceptional, m_Material, m_Hue); break;
                case BODType.Inscription: bod = new SmallInscriptionBOD(m_AmountCur, m_AmountMax, m_ItemType, m_Number, m_Graphic, m_RequireExceptional, m_Material, m_Hue); break;
                case BODType.Alchemy: bod = new SmallAlchemyBOD(m_AmountCur, m_AmountMax, m_ItemType, m_Number, m_Graphic, m_RequireExceptional, m_Material, m_Hue); break;
                case BODType.Carpentry: bod = new SmallCarpentryBOD(m_AmountCur, m_AmountMax, m_ItemType, m_Number, m_Graphic, m_RequireExceptional, m_Material, m_Hue); break;
                case BODType.Fletching: bod = new SmallFletchingBOD(m_AmountCur, m_AmountMax, m_ItemType, m_Number, m_Graphic, m_RequireExceptional, m_Material, m_Hue); break;
                case BODType.Tinkering: bod = new SmallTinkerBOD(m_AmountCur, m_AmountMax, m_ItemType, m_Number, m_Graphic, m_RequireExceptional, m_Material, m_Hue, m_GemType); break;
                case BODType.Cooking: bod = new SmallCookingBOD(m_AmountCur, m_AmountMax, m_ItemType, m_Number, m_Graphic, m_RequireExceptional, m_Material, m_Hue); break;
            }

            return bod;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(2); // version

            writer.Write((int)m_GemType);

            writer.Write(m_Hue);

            writer.Write(m_ItemType == null ? null : m_ItemType.FullName);

            writer.Write(m_RequireExceptional);

            writer.WriteEncodedInt((int)m_DeedType);
            writer.WriteEncodedInt((int)m_Material);
            writer.WriteEncodedInt(m_AmountCur);
            writer.WriteEncodedInt(m_AmountMax);
            writer.WriteEncodedInt(m_Number);
            writer.WriteEncodedInt(m_Graphic);
            writer.WriteEncodedInt(m_Price);
        }
    }
}