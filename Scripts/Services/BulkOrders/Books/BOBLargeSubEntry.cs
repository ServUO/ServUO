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
            m_ItemType = lbe.Details.Type;
            m_AmountCur = lbe.Amount;
            m_Number = lbe.Details.Number;
            m_Graphic = lbe.Details.Graphic;
            m_Hue = lbe.Details.Hue;
        }

        public BOBLargeSubEntry(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch (version)
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
                            m_ItemType = ScriptCompiler.FindTypeByFullName(type);

                        m_AmountCur = reader.ReadEncodedInt();
                        m_Number = reader.ReadEncodedInt();
                        m_Graphic = reader.ReadEncodedInt();

                        break;
                    }
            }
        }

        public Type ItemType => m_ItemType;
        public int AmountCur => m_AmountCur;
        public int Number => m_Number;
        public int Graphic => m_Graphic;
        public int Hue => m_Hue;
        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(1); // version

            writer.WriteEncodedInt(m_Hue);

            writer.Write(m_ItemType == null ? null : m_ItemType.FullName);

            writer.WriteEncodedInt(m_AmountCur);
            writer.WriteEncodedInt(m_Number);
            writer.WriteEncodedInt(m_Graphic);
        }
    }
}