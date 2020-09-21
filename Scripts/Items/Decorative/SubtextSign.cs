namespace Server.Items
{
    public class SubtextSign : Sign
    {
        private string m_Subtext;
        [Constructable]
        public SubtextSign(SignType type, SignFacing facing, string subtext)
            : base(type, facing)
        {
            m_Subtext = subtext;
        }

        [Constructable]
        public SubtextSign(int itemID, string subtext)
            : base(itemID)
        {
            m_Subtext = subtext;
        }

        public SubtextSign(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Subtext
        {
            get
            {
                return m_Subtext;
            }
            set
            {
                m_Subtext = value;
                InvalidateProperties();
            }
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            if (!string.IsNullOrEmpty(m_Subtext))
                list.Add(m_Subtext);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);

            writer.Write(m_Subtext);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Subtext = reader.ReadString();
        }
    }
}
