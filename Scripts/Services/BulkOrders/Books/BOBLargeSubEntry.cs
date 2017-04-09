using System;

namespace Server.Engines.BulkOrders
{
    public class BOBLargeSubEntry
    {
        private readonly Type m_ItemType;
        private readonly int m_AmountCur;
        private readonly int m_Number;
        private readonly int m_Graphic;
        private readonly int m_Hue;

        public BOBLargeSubEntry(LargeBulkEntry lbe)
        {
            this.m_ItemType = lbe.Details.Type;
            this.m_AmountCur = lbe.Amount;
            this.m_Number = lbe.Details.Number;
            this.m_Graphic = lbe.Details.Graphic;
            this.m_Hue = lbe.Details.Hue;
        }

        public BOBLargeSubEntry(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 1:
                    {
                        m_Hue = reader.ReadEncodedInt();
                        goto case 0;
                    }
                case 0:
                    {
                        string type = reader.ReadString();

                        if (type != null)
                            this.m_ItemType = ScriptCompiler.FindTypeByFullName(type);

                        this.m_AmountCur = reader.ReadEncodedInt();
                        this.m_Number = reader.ReadEncodedInt();
                        this.m_Graphic = reader.ReadEncodedInt();

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
        public int AmountCur
        {
            get
            {
                return this.m_AmountCur;
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
        public int Hue
        {
            get
            {
                return this.m_Hue;
            }
        }
        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(1); // version

            writer.WriteEncodedInt((int)this.m_Hue);

            writer.Write(this.m_ItemType == null ? null : this.m_ItemType.FullName);

            writer.WriteEncodedInt((int)this.m_AmountCur);
            writer.WriteEncodedInt((int)this.m_Number);
            writer.WriteEncodedInt((int)this.m_Graphic);
        }
    }
}