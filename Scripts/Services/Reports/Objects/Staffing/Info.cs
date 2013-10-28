using System;
using Server.Accounting;

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
            this.m_Account = account;
            this.m_Pages = new PageInfoCollection();
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
                return this.m_Account;
            }
            set
            {
                this.m_Account = value;
            }
        }
        public PageInfoCollection Pages
        {
            get
            {
                return this.m_Pages;
            }
            set
            {
                this.m_Pages = value;
            }
        }
        public string Display
        {
            get
            {
                if (this.m_Display != null)
                    return this.m_Display;

                if (this.m_Account != null)
                {
                    IAccount acct = Accounts.GetAccount(this.m_Account);

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
                            return (this.m_Display = mob.Name);
                    }
                }

                return (this.m_Display = this.m_Account);
            }
        }
        public int GetPageCount(PageResolution res, DateTime min, DateTime max)
        {
            return StaffHistory.GetPageCount(this.m_Pages, res, min, max);
        }

        public void Register(PageInfo page)
        {
            this.m_Pages.Add(page);
        }

        public void Unregister(PageInfo page)
        {
            this.m_Pages.Remove(page);
        }

        public int CompareTo(object obj)
        {
            BaseInfo cmp = obj as BaseInfo;

            int v = cmp.GetPageCount(cmp is StaffInfo ? PageResolution.Handled : PageResolution.None, DateTime.UtcNow - m_SortRange, DateTime.UtcNow) -
                    this.GetPageCount(this is StaffInfo ? PageResolution.Handled : PageResolution.None, DateTime.UtcNow - m_SortRange, DateTime.UtcNow);

            if (v == 0)
                v = String.Compare(this.Display, cmp.Display);

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