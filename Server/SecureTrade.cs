#region Header
// **********
// ServUO - SecureTrade.cs
// **********
#endregion

#region References
using System;
using Server.Accounting;
using Server.Items;
using Server.Network;
#endregion

namespace Server
{
    public class SecureTrade
    {
        private readonly SecureTradeInfo m_From;
        private readonly SecureTradeInfo m_To;
        private bool m_Valid;

        public SecureTradeInfo From { get { return m_From; } }

        public SecureTradeInfo To { get { return m_To; } }

        public bool Valid { get { return m_Valid; } }

        public void Cancel()
        {
            if (!m_Valid)
            {
                return;
            }

            var list = m_From.Container.Items;

            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (i < list.Count)
                {
                    Item item = list[i];

                    item.OnSecureTrade(m_From.Mobile, m_To.Mobile, m_From.Mobile, false);

                    if (!item.Deleted)
                    {
                        m_From.Mobile.AddToBackpack(item);
                    }
                }
            }

            list = m_To.Container.Items;

            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (i < list.Count)
                {
                    Item item = list[i];

                    item.OnSecureTrade(m_To.Mobile, m_From.Mobile, m_To.Mobile, false);

                    if (!item.Deleted)
                    {
                        m_To.Mobile.AddToBackpack(item);
                    }
                }
            }

            Close();
        }

        public void Close()
        {
            if (!m_Valid)
            {
                return;
            }

            m_From.Mobile.Send(new CloseSecureTrade(m_From.Container));
            m_To.Mobile.Send(new CloseSecureTrade(m_To.Container));

            m_Valid = false;

            NetState ns = m_From.Mobile.NetState;

            if (ns != null)
            {
                ns.RemoveTrade(this);
            }

            ns = m_To.Mobile.NetState;

            if (ns != null)
            {
                ns.RemoveTrade(this);
            }

            Timer.DelayCall(TimeSpan.Zero, delegate { m_From.Container.Delete(); });
            Timer.DelayCall(TimeSpan.Zero, delegate { m_To.Container.Delete(); });
        }

        public void Update(int updateGold)
        {
            if (!m_Valid)
            {
                return;
            }
            switch (updateGold)
            {
                case 1:

                    int totalGold;
                    double goldStub;
                    int totalPlat;
                    double platStub;
                    m_From.Mobile.Account.GetBalance(out totalGold,out goldStub,out totalPlat, out platStub);

                    if (m_From.Plat > totalPlat)
                    {
                        m_From.Plat = totalPlat;
                        m_From.Mobile.SendMessage("You do not have enough currency. Your balance is " + totalPlat + " Plat and " + totalGold + " gold.");
                    }
                    else if (m_From.Gold > totalGold && totalPlat < 1)
                    {
                        m_From.Gold = totalGold;
                        m_From.Mobile.SendMessage("You do not have enough currency. Your balance is " + totalPlat + " Plat and " + totalGold + " gold. ");
                    }

                    m_To.Mobile.Send(new UpdateSecureTradeGold(m_To.Container, m_From.Gold, m_From.Plat));
                    break;
                case 2:
                    m_To.Mobile.Account.GetBalance(out totalGold, out goldStub, out totalPlat, out platStub);
                    if (m_To.Plat > totalPlat)
                    {
                        m_To.Plat = totalPlat;
                        m_To.Mobile.SendMessage("You do not have enough currency. Your balance is " + totalPlat + " Plat and " + totalGold + " gold.");
                    }

                    else if (m_To.Gold > totalGold && totalPlat < 1)
                    {
                        m_To.Gold = totalGold;
                        m_To.Mobile.SendMessage("You do not have enough currency. Your balance is " + totalPlat + " Plat and " + totalGold + " gold.");
                    }

                    m_From.Mobile.Send(new UpdateSecureTradeGold(m_From.Container, m_To.Gold, m_To.Plat));
                    break;
                case 0:
                    if (m_From.Accepted && m_To.Accepted)
                    {
                        var list = m_From.Container.Items;

                        bool allowed = true;

                        for (int i = list.Count - 1; allowed && i >= 0; --i)
                        {
                            if (i < list.Count)
                            {
                                Item item = list[i];

                                if (!item.AllowSecureTrade(m_From.Mobile, m_To.Mobile, m_To.Mobile, true))
                                {
                                    allowed = false;
                                }
                            }
                        }

                        list = m_To.Container.Items;

                        for (int i = list.Count - 1; allowed && i >= 0; --i)
                        {
                            if (i < list.Count)
                            {
                                Item item = list[i];

                                if (!item.AllowSecureTrade(m_To.Mobile, m_From.Mobile, m_From.Mobile, true))
                                {
                                    allowed = false;
                                }
                            }
                        }

                        if (!allowed)
                        {
                            m_From.Accepted = false;
                            m_To.Accepted = false;

                            m_From.Mobile.Send(new UpdateSecureTrade(m_From.Container, m_From.Accepted, m_To.Accepted));
                            m_To.Mobile.Send(new UpdateSecureTrade(m_To.Container, m_To.Accepted, m_From.Accepted));


                            return;
                        }

                        list = m_From.Container.Items;

                        for (int i = list.Count - 1; i >= 0; --i)
                        {
                            if (i < list.Count)
                            {
                                Item item = list[i];

                                item.OnSecureTrade(m_From.Mobile, m_To.Mobile, m_To.Mobile, true);

                                if (!item.Deleted)
                                {
                                    m_To.Mobile.AddToBackpack(item);
                                }
                            }
                        }

                        list = m_To.Container.Items;

                        for (int i = list.Count - 1; i >= 0; --i)
                        {
                            if (i < list.Count)
                            {
                                Item item = list[i];

                                item.OnSecureTrade(m_To.Mobile, m_From.Mobile, m_From.Mobile, true);

                                if (!item.Deleted)
                                {
                                    m_From.Mobile.AddToBackpack(item);
                                }
                            }
                        }

                        m_From.Mobile.Account.DepositGold(m_To.Gold);
                        m_From.Mobile.Account.WithdrawGold(m_From.Gold);
                        m_From.Mobile.Account.DepositPlat(m_To.Plat);
                        m_From.Mobile.Account.WithdrawPlat(m_From.Plat);

                        m_To.Mobile.Account.DepositGold(m_From.Gold);
                        m_To.Mobile.Account.WithdrawGold(m_To.Gold);
                        m_To.Mobile.Account.DepositPlat(m_From.Plat);
                        m_To.Mobile.Account.WithdrawPlat(m_To.Plat);

                        Close();
                    }
                    else
                    {
                        m_From.Mobile.Send(new UpdateSecureTrade(m_From.Container, m_From.Accepted, m_To.Accepted));
                        m_To.Mobile.Send(new UpdateSecureTrade(m_To.Container, m_To.Accepted, m_From.Accepted));
                    }
                    break;
            }
        }

        public SecureTrade(Mobile from, Mobile to)
        {
            m_Valid = true;

            m_From = new SecureTradeInfo(this, from, new SecureTradeContainer(this));
            m_To = new SecureTradeInfo(this, to, new SecureTradeContainer(this));

            bool from6017 = (from.NetState == null ? false : from.NetState.ContainerGridLines);
            bool to6017 = (to.NetState == null ? false : to.NetState.ContainerGridLines);

            from.Send(new MobileStatus(from, to));
            from.Send(new UpdateSecureTrade(m_From.Container, false, false));
            if (from6017)
            {
                from.Send(new SecureTradeEquip6017(m_To.Container, to));
            }
            else
            {
                from.Send(new SecureTradeEquip(m_To.Container, to));
            }
            from.Send(new UpdateSecureTrade(m_From.Container, false, false));
            if (from6017)
            {
                from.Send(new SecureTradeEquip6017(m_From.Container, from));
            }
            else
            {
                from.Send(new SecureTradeEquip(m_From.Container, from));
            }
            from.Send(new DisplaySecureTrade(to, m_From.Container, m_To.Container, to.Name));
            from.Send(new UpdateSecureTrade(m_From.Container, false, false));
            if (Core.TOL)
            {

                int goldBalance;
                double goldStub;
                int platBalance;
                double platStub;
                m_From.Mobile.Account.GetBalance(out goldBalance, out goldStub, out platBalance, out platStub);

                from.Send(new UpdateSecureTradeGoldLedger(m_From.Container, goldBalance, platBalance));
            }


            to.Send(new MobileStatus(to, from));
            to.Send(new UpdateSecureTrade(m_To.Container, false, false));
            if (to6017)
            {
                to.Send(new SecureTradeEquip6017(m_From.Container, from));
            }
            else
            {
                to.Send(new SecureTradeEquip(m_From.Container, from));
            }
            to.Send(new UpdateSecureTrade(m_To.Container, false, false));
            if (to6017)
            {
                to.Send(new SecureTradeEquip6017(m_To.Container, to));
            }
            else
            {
                to.Send(new SecureTradeEquip(m_To.Container, to));
            }
            to.Send(new DisplaySecureTrade(from, m_To.Container, m_From.Container, from.Name));
            to.Send(new UpdateSecureTrade(m_To.Container, false, false));
            if (Core.TOL)
            {
                int goldBalance;
                double goldStub;
                int platBalance;
                double platStub;
                to.Account.GetBalance(out goldBalance, out goldStub, out platBalance, out platStub);
                to.Send(new UpdateSecureTradeGoldLedger(m_To.Container, goldBalance, platBalance));
            }


        }
    }

    public class SecureTradeInfo
    {
        private readonly SecureTrade m_Owner;
        private readonly Mobile m_Mobile;
        private readonly SecureTradeContainer m_Container;

        public SecureTradeInfo(SecureTrade owner, Mobile m, SecureTradeContainer c)
        {
            m_Owner = owner;
            m_Mobile = m;
            m_Container = c;

            m_Mobile.AddItem(m_Container);
        }

        public SecureTrade Owner { get { return m_Owner; } }

        public Mobile Mobile { get { return m_Mobile; } }

        public SecureTradeContainer Container { get { return m_Container; } }

        public int Gold;

        public int Plat;

        public bool Accepted { get; set; }
    }
}