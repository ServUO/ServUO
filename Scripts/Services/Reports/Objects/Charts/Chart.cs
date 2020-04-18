namespace Server.Engines.Reports
{
    public abstract class Chart : PersistableObject
    {
        protected string m_Name;
        protected string m_FileName;
        protected ChartItemCollection m_Items;
        public Chart()
        {
            m_Items = new ChartItemCollection();
        }

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
        public string FileName
        {
            get
            {
                return m_FileName;
            }
            set
            {
                m_FileName = value;
            }
        }
        public ChartItemCollection Items => m_Items;
        public override void SerializeAttributes(PersistenceWriter op)
        {
            op.SetString("n", m_Name);
            op.SetString("f", m_FileName);
        }

        public override void DeserializeAttributes(PersistenceReader ip)
        {
            m_Name = Utility.Intern(ip.GetString("n"));
            m_FileName = Utility.Intern(ip.GetString("f"));
        }

        public override void SerializeChildren(PersistenceWriter op)
        {
            for (int i = 0; i < m_Items.Count; ++i)
                m_Items[i].Serialize(op);
        }

        public override void DeserializeChildren(PersistenceReader ip)
        {
            while (ip.HasChild)
                m_Items.Add(ip.GetChild() as ChartItem);
        }
    }
}