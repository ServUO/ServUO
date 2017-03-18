using System;

namespace Server.Engines.Reports
{
    public class Snapshot : PersistableObject
    {
        #region Type Identification
        public static readonly PersistableType ThisTypeID = new PersistableType("ss", new ConstructCallback(Construct));

        private static PersistableObject Construct()
        {
            return new Snapshot();
        }

        public override PersistableType TypeID
        {
            get
            {
                return ThisTypeID;
            }
        }
        #endregion

        private DateTime m_TimeStamp;
        private ObjectCollection m_Children;

        public DateTime TimeStamp
        {
            get
            {
                return this.m_TimeStamp;
            }
            set
            {
                this.m_TimeStamp = value;
            }
        }
        public ObjectCollection Children
        {
            get
            {
                return this.m_Children;
            }
            set
            {
                this.m_Children = value;
            }
        }

        public Snapshot()
        {
            this.m_Children = new ObjectCollection();
        }

        public override void SerializeAttributes(PersistenceWriter op)
        {
            op.SetDateTime("t", this.m_TimeStamp);
        }

        public override void DeserializeAttributes(PersistenceReader ip)
        {
            this.m_TimeStamp = ip.GetDateTime("t");
        }

        public override void SerializeChildren(PersistenceWriter op)
        {
            for (int i = 0; i < this.m_Children.Count; ++i)
                this.m_Children[i].Serialize(op);
        }

        public override void DeserializeChildren(PersistenceReader ip)
        {
            while (ip.HasChild)
                this.m_Children.Add(ip.GetChild());
        }
    }
}