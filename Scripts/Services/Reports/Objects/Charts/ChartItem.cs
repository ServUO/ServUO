using System;

namespace Server.Engines.Reports
{
    public class ChartItem : PersistableObject
    {
        #region Type Identification
        public static readonly PersistableType ThisTypeID = new PersistableType("ci", new ConstructCallback(Construct));

        private static PersistableObject Construct()
        {
            return new ChartItem();
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
        private int m_Value;

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
        public int Value
        {
            get
            {
                return this.m_Value;
            }
            set
            {
                this.m_Value = value;
            }
        }

        private ChartItem()
        {
        }

        public ChartItem(string name, int value)
        {
            this.m_Name = name;
            this.m_Value = value;
        }

        public override void SerializeAttributes(PersistanceWriter op)
        {
            op.SetString("n", this.m_Name);
            op.SetInt32("v", this.m_Value);
        }

        public override void DeserializeAttributes(PersistanceReader ip)
        {
            this.m_Name = Utility.Intern(ip.GetString("n"));
            this.m_Value = ip.GetInt32("v");
        }
    }
}