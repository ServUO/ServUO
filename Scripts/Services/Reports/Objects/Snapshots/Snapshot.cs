using System;

namespace Server.Engines.Reports
{
    public class Snapshot : PersistableObject
    {
        #region Type Identification
        public static readonly PersistableType ThisTypeID = new PersistableType("ss", Construct);

        private static PersistableObject Construct()
        {
            return new Snapshot();
        }

        public override PersistableType TypeID => ThisTypeID;
        #endregion

        private DateTime m_TimeStamp;
        private ObjectCollection m_Children;

        public DateTime TimeStamp
        {
            get
            {
                return m_TimeStamp;
            }
            set
            {
                m_TimeStamp = value;
            }
        }
        public ObjectCollection Children
        {
            get
            {
                return m_Children;
            }
            set
            {
                m_Children = value;
            }
        }

        public Snapshot()
        {
            m_Children = new ObjectCollection();
        }

        public override void SerializeAttributes(PersistenceWriter op)
        {
            op.SetDateTime("t", m_TimeStamp);
        }

        public override void DeserializeAttributes(PersistenceReader ip)
        {
            m_TimeStamp = ip.GetDateTime("t");
        }

        public override void SerializeChildren(PersistenceWriter op)
        {
            for (int i = 0; i < m_Children.Count; ++i)
                m_Children[i].Serialize(op);
        }

        public override void DeserializeChildren(PersistenceReader ip)
        {
            while (ip.HasChild)
                m_Children.Add(ip.GetChild());
        }
    }
}
