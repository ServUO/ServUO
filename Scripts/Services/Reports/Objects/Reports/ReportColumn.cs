using System;

namespace Server.Engines.Reports
{
    public class ReportColumn : PersistableObject
    {
        #region Type Identification
        public static readonly PersistableType ThisTypeID = new PersistableType("rc", new ConstructCallback(Construct));

        private static PersistableObject Construct()
        {
            return new ReportColumn();
        }

        public override PersistableType TypeID
        {
            get
            {
                return ThisTypeID;
            }
        }
        #endregion

        private string m_Width;
        private string m_Align;
        private string m_Name;

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
        public string Align
        {
            get
            {
                return this.m_Align;
            }
            set
            {
                this.m_Align = value;
            }
        }
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

        private ReportColumn()
        {
        }

        public ReportColumn(string width, string align)
            : this(width, align, null)
        {
        }

        public ReportColumn(string width, string align, string name)
        {
            this.m_Width = width;
            this.m_Align = align;
            this.m_Name = name;
        }

        public override void SerializeAttributes(PersistenceWriter op)
        {
            op.SetString("w", this.m_Width);
            op.SetString("a", this.m_Align);
            op.SetString("n", this.m_Name);
        }

        public override void DeserializeAttributes(PersistenceReader ip)
        {
            this.m_Width = Utility.Intern(ip.GetString("w"));
            this.m_Align = Utility.Intern(ip.GetString("a"));
            this.m_Name = Utility.Intern(ip.GetString("n"));
        }
    }
}