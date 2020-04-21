namespace Server.Engines.Reports
{
    public class ChartItem : PersistableObject
    {
        #region Type Identification
        public static readonly PersistableType ThisTypeID = new PersistableType("ci", Construct);

        private static PersistableObject Construct()
        {
            return new ChartItem();
        }

        public override PersistableType TypeID => ThisTypeID;
        #endregion

        private string m_Name;
        private int m_Value;

        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }
        public int Value
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

        private ChartItem()
        {
        }

        public ChartItem(string name, int value)
        {
            m_Name = name;
            m_Value = value;
        }

        public override void SerializeAttributes(PersistenceWriter op)
        {
            op.SetString("n", m_Name);
            op.SetInt32("v", m_Value);
        }

        public override void DeserializeAttributes(PersistenceReader ip)
        {
            m_Name = Utility.Intern(ip.GetString("n"));
            m_Value = ip.GetInt32("v");
        }
    }
}
