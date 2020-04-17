namespace Server.Items
{
    public class LocalizedSign : Sign
    {
        private int m_LabelNumber;
        [Constructable]
        public LocalizedSign(SignType type, SignFacing facing, int labelNumber)
            : base((0xB95 + (2 * (int)type)) + (int)facing)
        {
            m_LabelNumber = labelNumber;
        }

        [Constructable]
        public LocalizedSign(int itemID, int labelNumber)
            : base(itemID)
        {
            m_LabelNumber = labelNumber;
        }

        public LocalizedSign(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => m_LabelNumber;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Number
        {
            get
            {
                return m_LabelNumber;
            }
            set
            {
                m_LabelNumber = value;
                InvalidateProperties();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);

            writer.Write(m_LabelNumber);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_LabelNumber = reader.ReadInt();
                        break;
                    }
            }
        }
    }
}