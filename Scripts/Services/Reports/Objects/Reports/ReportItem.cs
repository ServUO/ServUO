using System;

namespace Server.Engines.Reports
{
    public class ReportItem : PersistableObject
    {
        #region Type Identification
        public static readonly PersistableType ThisTypeID = new PersistableType("ri", new ConstructCallback(Construct));

        private static PersistableObject Construct()
        {
            return new ReportItem();
        }

        public override PersistableType TypeID
        {
            get
            {
                return ThisTypeID;
            }
        }
        #endregion

        private readonly ItemValueCollection m_Values;

        public ItemValueCollection Values
        {
            get
            {
                return this.m_Values;
            }
        }

        public ReportItem()
        {
            this.m_Values = new ItemValueCollection();
        }

        public override void SerializeChildren(PersistanceWriter op)
        {
            for (int i = 0; i < this.m_Values.Count; ++i)
                this.m_Values[i].Serialize(op);
        }

        public override void DeserializeChildren(PersistanceReader ip)
        {
            while (ip.HasChild)
                this.m_Values.Add(ip.GetChild() as ItemValue);
        }
    }
}