using System;

namespace Server.Engines.Reports
{
    public class ResponseInfo : PersistableObject
    {
        #region Type Identification
        public static readonly PersistableType ThisTypeID = new PersistableType("rs", new ConstructCallback(Construct));

        private static PersistableObject Construct()
        {
            return new ResponseInfo();
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

        private string m_SentBy;
        private string m_Message;

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

        public string SentBy
        {
            get
            {
                return this.m_SentBy;
            }
            set
            {
                this.m_SentBy = value;
            }
        }
        public string Message
        {
            get
            {
                return this.m_Message;
            }
            set
            {
                this.m_Message = value;
            }
        }

        public ResponseInfo()
        {
        }

        public ResponseInfo(string sentBy, string message)
        {
            this.m_TimeStamp = DateTime.UtcNow;
            this.m_SentBy = sentBy;
            this.m_Message = message;
        }

        public override void SerializeAttributes(PersistanceWriter op)
        {
            op.SetDateTime("t", this.m_TimeStamp);

            op.SetString("s", this.m_SentBy);
            op.SetString("m", this.m_Message);
        }

        public override void DeserializeAttributes(PersistanceReader ip)
        {
            this.m_TimeStamp = ip.GetDateTime("t");

            this.m_SentBy = ip.GetString("s");
            this.m_Message = ip.GetString("m");
        }
    }
}