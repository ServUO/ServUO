using Server.Engines.Help;
using System;

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
        public static readonly PersistableType ThisTypeID = new PersistableType("pi", Construct);

        private static PersistableObject Construct()
        {
            return new PageInfo();
        }

        public override PersistableType TypeID => ThisTypeID;
        #endregion

        private StaffHistory m_History;
        private StaffInfo m_Resolver;
        private UserInfo m_Sender;

        public StaffInfo Resolver
        {
            get
            {
                return m_Resolver;
            }
            set
            {
                if (m_Resolver == value)
                    return;

                lock (StaffHistory.RenderLock)
                {
                    if (m_Resolver != null)
                        m_Resolver.Unregister(this);

                    m_Resolver = value;

                    if (m_Resolver != null)
                        m_Resolver.Register(this);
                }
            }
        }

        public UserInfo Sender
        {
            get
            {
                return m_Sender;
            }
            set
            {
                if (m_Sender == value)
                    return;

                lock (StaffHistory.RenderLock)
                {
                    if (m_Sender != null)
                        m_Sender.Unregister(this);

                    m_Sender = value;

                    if (m_Sender != null)
                        m_Sender.Register(this);
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
                return m_History;
            }
            set
            {
                if (m_History == value)
                    return;

                if (m_History != null)
                {
                    Sender = null;
                    Resolver = null;
                }

                m_History = value;

                if (m_History != null)
                {
                    Sender = m_History.GetUserInfo(m_SentBy);
                    UpdateResolver();
                }
            }
        }

        public PageType PageType
        {
            get
            {
                return m_PageType;
            }
            set
            {
                m_PageType = value;
            }
        }
        public PageResolution Resolution => m_Resolution;

        public DateTime TimeSent
        {
            get
            {
                return m_TimeSent;
            }
            set
            {
                m_TimeSent = value;
            }
        }
        public DateTime TimeResolved => m_TimeResolved;

        public string SentBy
        {
            get
            {
                return m_SentBy;
            }
            set
            {
                m_SentBy = value;

                if (m_History != null)
                    Sender = m_History.GetUserInfo(m_SentBy);
            }
        }

        public string ResolvedBy => m_ResolvedBy;

        public string Message
        {
            get
            {
                return m_Message;
            }
            set
            {
                m_Message = value;
            }
        }
        public ResponseInfoCollection Responses
        {
            get
            {
                return m_Responses;
            }
            set
            {
                m_Responses = value;
            }
        }

        public void UpdateResolver()
        {
            string resolvedBy;
            DateTime timeResolved;
            PageResolution res = GetResolution(out resolvedBy, out timeResolved);

            if (m_History != null && IsStaffResolution(res))
                Resolver = m_History.GetStaffInfo(resolvedBy);
            else
                Resolver = null;

            m_ResolvedBy = resolvedBy;
            m_TimeResolved = timeResolved;
            m_Resolution = res;
        }

        public bool IsStaffResolution(PageResolution res)
        {
            return (res == PageResolution.Handled);
        }

        public static PageResolution ResFromResp(string resp)
        {
            switch (resp)
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
            for (int i = m_Responses.Count - 1; i >= 0; --i)
            {
                ResponseInfo resp = m_Responses[i];
                PageResolution res = ResFromResp(resp.Message);

                if (res != PageResolution.None)
                {
                    resolvedBy = resp.SentBy;
                    timeResolved = resp.TimeStamp;
                    return res;
                }
            }

            resolvedBy = m_SentBy;
            timeResolved = m_TimeSent;
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
            m_Responses = new ResponseInfoCollection();
        }

        public PageInfo(PageEntry entry)
        {
            m_PageType = entry.Type;

            m_TimeSent = entry.Sent;
            m_SentBy = GetAccount(entry.Sender);

            m_Message = entry.Message;
            m_Responses = new ResponseInfoCollection();
        }

        public override void SerializeAttributes(PersistenceWriter op)
        {
            op.SetInt32("p", (int)m_PageType);

            op.SetDateTime("ts", m_TimeSent);
            op.SetString("s", m_SentBy);

            op.SetString("m", m_Message);
        }

        public override void DeserializeAttributes(PersistenceReader ip)
        {
            m_PageType = (PageType)ip.GetInt32("p");

            m_TimeSent = ip.GetDateTime("ts");
            m_SentBy = ip.GetString("s");

            m_Message = ip.GetString("m");
        }

        public override void SerializeChildren(PersistenceWriter op)
        {
            lock (this)
            {
                for (int i = 0; i < m_Responses.Count; ++i)
                    m_Responses[i].Serialize(op);
            }
        }

        public override void DeserializeChildren(PersistenceReader ip)
        {
            while (ip.HasChild)
                m_Responses.Add(ip.GetChild() as ResponseInfo);
        }
    }
}
