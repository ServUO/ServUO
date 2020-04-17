using Server.Accounting;
using System;

namespace Server.Engines.Reports
{
    public abstract class BaseInfo : IComparable
    {
        private static TimeSpan m_SortRange;
        private string m_Account;
        private string m_Display;
        private PageInfoCollection m_Pages;
        public BaseInfo(string account)
        {
            m_Account = account;
            m_Pages = new PageInfoCollection();
        }

        public static TimeSpan SortRange
        {
            get
            {
                return m_SortRange;
            }
            set
            {
                m_SortRange = value;
            }
        }
        public string Account
        {
            get
            {
                return m_Account;
            }
            set
            {
                m_Account = value;
            }
        }
        public PageInfoCollection Pages
        {
            get
            {
                return m_Pages;
            }
            set
            {
                m_Pages = value;
            }
        }
        public string Display
        {
            get
            {
                if (m_Display != null)
                    return m_Display;

                if (m_Account != null)
                {
                    IAccount acct = Accounts.GetAccount(m_Account);

                    if (acct != null)
                    {
                        Mobile mob = null;

                        for (int i = 0; i < acct.Length; ++i)
                        {
                            Mobile check = acct[i];

                            if (check != null && (mob == null || check.AccessLevel > mob.AccessLevel))
                                mob = check;
                        }

                        if (mob != null && mob.Name != null && mob.Name.Length > 0)
                            return (m_Display = mob.Name);
                    }
                }

                return (m_Display = m_Account);
            }
        }
        public int GetPageCount(PageResolution res, DateTime min, DateTime max)
        {
            return StaffHistory.GetPageCount(m_Pages, res, min, max);
        }

        public void Register(PageInfo page)
        {
            m_Pages.Add(page);
        }

        public void Unregister(PageInfo page)
        {
            m_Pages.Remove(page);
        }

        public int CompareTo(object obj)
        {
            BaseInfo cmp = obj as BaseInfo;

            int v = cmp.GetPageCount(cmp is StaffInfo ? PageResolution.Handled : PageResolution.None, DateTime.UtcNow - m_SortRange, DateTime.UtcNow) -
                    GetPageCount(this is StaffInfo ? PageResolution.Handled : PageResolution.None, DateTime.UtcNow - m_SortRange, DateTime.UtcNow);

            if (v == 0)
                v = String.Compare(Display, cmp.Display);

            return v;
        }
    }

    public class StaffInfo : BaseInfo
    {
        public StaffInfo(string account)
            : base(account)
        {
        }
    }

    public class UserInfo : BaseInfo
    {
        public UserInfo(string account)
            : base(account)
        {
        }
    }
}