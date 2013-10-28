using System;

namespace Server.Engines.Reports
{
    public abstract class Chart : PersistableObject
    {
        protected string m_Name;
        protected string m_FileName;
        protected ChartItemCollection m_Items;
        public Chart()
        {
            this.m_Items = new ChartItemCollection();
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
        public string FileName
        {
            get
            {
                return this.m_FileName;
            }
            set
            {
                this.m_FileName = value;
            }
        }
        public ChartItemCollection Items
        {
            get
            {
                return this.m_Items;
            }
        }
        public override void SerializeAttributes(PersistanceWriter op)
        {
            op.SetString("n", this.m_Name);
            op.SetString("f", this.m_FileName);
        }

        public override void DeserializeAttributes(PersistanceReader ip)
        {
            this.m_Name = Utility.Intern(ip.GetString("n"));
            this.m_FileName = Utility.Intern(ip.GetString("f"));
        }

        public override void SerializeChildren(PersistanceWriter op)
        {
            for (int i = 0; i < this.m_Items.Count; ++i)
                this.m_Items[i].Serialize(op);
        }

        public override void DeserializeChildren(PersistanceReader ip)
        {
            while (ip.HasChild)
                this.m_Items.Add(ip.GetChild() as ChartItem);
        }
    }
}