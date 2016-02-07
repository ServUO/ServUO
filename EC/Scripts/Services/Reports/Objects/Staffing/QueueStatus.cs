using System;

namespace Server.Engines.Reports
{
    public class QueueStatus : PersistableObject
    {
        #region Type Identification
        public static readonly PersistableType ThisTypeID = new PersistableType("qs", new ConstructCallback(Construct));

        private static PersistableObject Construct()
        {
            return new QueueStatus();
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
        private int m_Count;

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
        public int Count
        {
            get
            {
                return this.m_Count;
            }
            set
            {
                this.m_Count = value;
            }
        }

        public QueueStatus()
        {
        }

        public QueueStatus(int count)
        {
            this.m_TimeStamp = DateTime.UtcNow;
            this.m_Count = count;
        }

        public override void SerializeAttributes(PersistanceWriter op)
        {
            op.SetDateTime("t", this.m_TimeStamp);
            op.SetInt32("c", this.m_Count);
        }

        public override void DeserializeAttributes(PersistanceReader ip)
        {
            this.m_TimeStamp = ip.GetDateTime("t");
            this.m_Count = ip.GetInt32("c");
        }
    }
}