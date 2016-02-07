using System;

namespace Server.Engines.Reports
{
    public class Report : PersistableObject
    {
        #region Type Identification
        public static readonly PersistableType ThisTypeID = new PersistableType("rp", new ConstructCallback(Construct));

        private static PersistableObject Construct()
        {
            return new Report();
        }

        public override PersistableType TypeID
        {
            get
            {
                return ThisTypeID;
            }
        }
        #endregion

        private string m_Name;
        private string m_Width;
        private readonly ReportColumnCollection m_Columns;
        private readonly ReportItemCollection m_Items;

        public string Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }
        public string Width
        {
            get
            {
                return this.m_Width;
            }
            set
            {
                this.m_Width = value;
            }
        }
        public ReportColumnCollection Columns
        {
            get
            {
                return this.m_Columns;
            }
        }
        public ReportItemCollection Items
        {
            get
            {
                return this.m_Items;
            }
        }

        private Report()
            : this(null, null)
        {
        }

        public Report(string name, string width)
        {
            this.m_Name = name;
            this.m_Width = width;
            this.m_Columns = new ReportColumnCollection();
            this.m_Items = new ReportItemCollection();
        }

        public override void SerializeAttributes(PersistanceWriter op)
        {
            op.SetString("n", this.m_Name);
            op.SetString("w", this.m_Width);
        }

        public override void DeserializeAttributes(PersistanceReader ip)
        {
            this.m_Name = Utility.Intern(ip.GetString("n"));
            this.m_Width = Utility.Intern(ip.GetString("w"));
        }

        public override void SerializeChildren(PersistanceWriter op)
        {
            for (int i = 0; i < this.m_Columns.Count; ++i)
                this.m_Columns[i].Serialize(op);

            for (int i = 0; i < this.m_Items.Count; ++i)
                this.m_Items[i].Serialize(op);
        }

        public override void DeserializeChildren(PersistanceReader ip)
        {
            while (ip.HasChild)
            {
                PersistableObject child = ip.GetChild();

                if (child is ReportColumn)
                    this.m_Columns.Add((ReportColumn)child);
                else if (child is ReportItem)
                    this.m_Items.Add((ReportItem)child);
            }
        }
    }
}