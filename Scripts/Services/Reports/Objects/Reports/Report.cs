namespace Server.Engines.Reports
{
    public class Report : PersistableObject
    {
        #region Type Identification
        public static readonly PersistableType ThisTypeID = new PersistableType("rp", Construct);

        private static PersistableObject Construct()
        {
            return new Report();
        }

        public override PersistableType TypeID => ThisTypeID;
        #endregion

        private string m_Name;
        private string m_Width;
        private readonly ReportColumnCollection m_Columns;
        private readonly ReportItemCollection m_Items;

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
        public string Width
        {
            get
            {
                return m_Width;
            }
            set
            {
                m_Width = value;
            }
        }
        public ReportColumnCollection Columns => m_Columns;
        public ReportItemCollection Items => m_Items;

        private Report()
            : this(null, null)
        {
        }

        public Report(string name, string width)
        {
            m_Name = name;
            m_Width = width;
            m_Columns = new ReportColumnCollection();
            m_Items = new ReportItemCollection();
        }

        public override void SerializeAttributes(PersistenceWriter op)
        {
            op.SetString("n", m_Name);
            op.SetString("w", m_Width);
        }

        public override void DeserializeAttributes(PersistenceReader ip)
        {
            m_Name = Utility.Intern(ip.GetString("n"));
            m_Width = Utility.Intern(ip.GetString("w"));
        }

        public override void SerializeChildren(PersistenceWriter op)
        {
            for (int i = 0; i < m_Columns.Count; ++i)
                m_Columns[i].Serialize(op);

            for (int i = 0; i < m_Items.Count; ++i)
                m_Items[i].Serialize(op);
        }

        public override void DeserializeChildren(PersistenceReader ip)
        {
            while (ip.HasChild)
            {
                PersistableObject child = ip.GetChild();

                if (child is ReportColumn)
                    m_Columns.Add((ReportColumn)child);
                else if (child is ReportItem)
                    m_Items.Add((ReportItem)child);
            }
        }
    }
}
