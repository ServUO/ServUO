using System;
using Server.Engines.Help;

namespace Server.Engines.Reports
{
    public enum PageResolution
    {
        None,
        Handled,
        Deleted,
        Logged,
        Canceled
    }

    public class PageInfo : PersistableObject
    {
        #region Type Identification
        public static readonly PersistableType ThisTypeID = new PersistableType("pi", new ConstructCallback(Construct));

        private static PersistableObject Construct()
        {
            return new PageInfo();
        }

        public override PersistableType TypeID
        {
            get
            {
                return ThisTypeID;
            }
        }
        #endregion

        private StaffHistory m_History;
        private StaffInfo m_Resolver;
        private UserInfo m_Sender;

        public StaffInfo Resolver
        {
            get
            {
                return this.m_Resolver;
            }
            set
            {
                if (this.m_Resolver == value)
                    return;

                lock (StaffHistory.RenderLock)
                {
                    if (this.m_Resolver != null)
                        this.m_Resolver.Unregister(this);

                    this.m_Resolver = value;

                    if (this.m_Resolver != null)
                        this.m_Resolver.Register(this);
                }
            }
        }

        public UserInfo Sender
        {
            get
            {
                return this.m_Sender;
            }
            set
            {
                if (this.m_Sender == value)
                    return;

                lock (StaffHistory.RenderLock)
                {
                    if (this.m_Sender != null)
                        this.m_Sender.Unregister(this);

                    this.m_Sender = value;

                    if (this.m_Sender != null)
                        this.m_Sender.Register(this);
                }
            }
        }

        private PageType m_PageType;
        private PageResolution m_Resolution;

        private DateTime m_TimeSent;
        private DateTime m_TimeResolved;

        private string m_SentBy;
        private string m_ResolvedBy;

        private string m_Message;
        private ResponseInfoCollection m_Responses;

        public StaffHistory History
        {
            get
            {
                return this.m_History;
            }
            set
            {
                if (this.m_History == value)
                    return;

                if (this.m_History != null)
                {
                    this.Sender = null;
                    this.Resolver = null;
                }

                this.m_History = value;

                if (this.m_History != null)
                {
                    this.Sender = this.m_History.GetUserInfo(this.m_SentBy);
                    this.UpdateResolver();
                }
            }
        }

        public PageType PageType
        {
            get
            {
                return this.m_PageType;
            }
            set
            {
                this.m_PageType = value;
            }
        }
        public PageResolution Resolution
        {
            get
            {
                return this.m_Resolution;
            }
        }

        public DateTime TimeSent
        {
            get
            {
                return this.m_TimeSent;
            }
            set
            {
                this.m_TimeSent = value;
            }
        }
        public DateTime TimeResolved
        {
            get
            {
                return this.m_TimeResolved;
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

                if (this.m_History != null)
                    this.Sender = this.m_History.GetUserInfo(this.m_SentBy);
            }
        }

        public string ResolvedBy
        {
            get
            {
                return this.m_ResolvedBy;
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
        public ResponseInfoCollection Responses
        {
            get
            {
                return this.m_Responses;
            }
            set
            {
                this.m_Responses = value;
            }
        }

        public void UpdateResolver()
        {
            string resolvedBy;
            DateTime timeResolved;
            PageResolution res = this.GetResolution(out resolvedBy, out timeResolved); 

            if (this.m_History != null && this.IsStaffResolution(res))
                this.Resolver = this.m_History.GetStaffInfo(resolvedBy);
            else
                this.Resolver = null;

            this.m_ResolvedBy = resolvedBy;
            this.m_TimeResolved = timeResolved;
            this.m_Resolution = res;
        }

        public bool IsStaffResolution(PageResolution res)
        {
            return (res == PageResolution.Handled);
        }

        public static PageResolution ResFromResp(string resp)
        {
            switch ( resp )
            {
                case "[Handled]":
                    return PageResolution.Handled;
                case "[Deleting]":
                    return PageResolution.Deleted;
                case "[Logout]":
                    return PageResolution.Logged;
                case "[Canceled]":
                    return PageResolution.Canceled;
            }

            return PageResolution.None;
        }

        public PageResolution GetResolution(out string resolvedBy, out DateTime timeResolved)
        {
            for (int i = this.m_Responses.Count - 1; i >= 0; --i)
            {
                ResponseInfo resp = this.m_Responses[i];
                PageResolution res = ResFromResp(resp.Message);

                if (res != PageResolution.None)
                {
                    resolvedBy = resp.SentBy;
                    timeResolved = resp.TimeStamp;
                    return res;
                }
            }

            resolvedBy = this.m_SentBy;
            timeResolved = this.m_TimeSent;
            return PageResolution.None;
        }

        public static string GetAccount(Mobile mob)
        {
            if (mob == null)
                return null;

            Accounting.Account acct = mob.Account as Accounting.Account;

            if (acct == null)
                return null;

            return acct.Username;
        }

        public PageInfo()
        {
            this.m_Responses = new ResponseInfoCollection();
        }

        public PageInfo(PageEntry entry)
        {
            this.m_PageType = entry.Type;

            this.m_TimeSent = entry.Sent;
            this.m_SentBy = GetAccount(entry.Sender);

            this.m_Message = entry.Message;
            this.m_Responses = new ResponseInfoCollection();
        }

        public override void SerializeAttributes(PersistenceWriter op)
        {
            op.SetInt32("p", (int)this.m_PageType);

            op.SetDateTime("ts", this.m_TimeSent);
            op.SetString("s", this.m_SentBy);

            op.SetString("m", this.m_Message);
        }

        public override void DeserializeAttributes(PersistenceReader ip)
        {
            this.m_PageType = (PageType)ip.GetInt32("p");

            this.m_TimeSent = ip.GetDateTime("ts");
            this.m_SentBy = ip.GetString("s");

            this.m_Message = ip.GetString("m");
        }

        public override void SerializeChildren(PersistenceWriter op)
        {
            lock (this)
            {
                for (int i = 0; i < this.m_Responses.Count; ++i)
                    this.m_Responses[i].Serialize(op);
            }
        }

        public override void DeserializeChildren(PersistenceReader ip)
        {
            while (ip.HasChild)
                this.m_Responses.Add(ip.GetChild() as ResponseInfo);
        }
    }
}