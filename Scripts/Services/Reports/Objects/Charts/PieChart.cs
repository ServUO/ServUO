using System;

namespace Server.Engines.Reports
{
    public class PieChart : Chart
    {
        #region Type Identification
        public static readonly PersistableType ThisTypeID = new PersistableType("pc", new ConstructCallback(Construct));

        private static PersistableObject Construct()
        {
            return new PieChart();
        }

        public override PersistableType TypeID
        {
            get
            {
                return ThisTypeID;
            }
        }
        #endregion

        private bool m_ShowPercents;

        public bool ShowPercents
        {
            get
            {
                return this.m_ShowPercents;
            }
            set
            {
                this.m_ShowPercents = value;
            }
        }

        public PieChart(string name, string fileName, bool showPercents)
        {
            this.m_Name = name;
            this.m_FileName = fileName;
            this.m_ShowPercents = showPercents;
        }

        private PieChart()
        {
        }

        public override void SerializeAttributes(PersistanceWriter op)
        {
            base.SerializeAttributes(op);

            op.SetBoolean("p", this.m_ShowPercents);
        }

        public override void DeserializeAttributes(PersistanceReader ip)
        {
            base.DeserializeAttributes(ip);

            this.m_ShowPercents = ip.GetBoolean("p");
        }
    }
}