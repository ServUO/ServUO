namespace Server.Engines.Reports
{
    public class ItemValue : PersistableObject
    {
        #region Type Identification
        public static readonly PersistableType ThisTypeID = new PersistableType("iv", Construct);

        private static PersistableObject Construct()
        {
            return new ItemValue();
        }

        public override PersistableType TypeID => ThisTypeID;
        #endregion

        private string m_Value;
        private string m_Format;

        public string Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
            }
        }
        public string Format
        {
            get
            {
                return m_Format;
            }
            set
            {
                m_Format = value;
            }
        }

        private ItemValue()
        {
        }

        public ItemValue(string value)
            : this(value, null)
        {
        }

        public ItemValue(string value, string format)
        {
            m_Value = value;
            m_Format = format;
        }

        public override void SerializeAttributes(PersistenceWriter op)
        {
            op.SetString("v", m_Value);
            op.SetString("f", m_Format);
        }

        public override void DeserializeAttributes(PersistenceReader ip)
        {
            m_Value = ip.GetString("v");
            m_Format = Utility.Intern(ip.GetString("f"));

            if (m_Format == null)
                Utility.Intern(ref m_Value);
        }
    }
}
