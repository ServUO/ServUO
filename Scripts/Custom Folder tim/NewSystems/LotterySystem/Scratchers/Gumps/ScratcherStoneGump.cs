using System; 
using Server; 
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Misc;
using Server.Network;
using Server.Engines.LotterySystem;

namespace Server.Gumps
{ 
    public class ScratcherStoneGump : Gump
    {
        private const int labelColor = 2106;
        private const int GMColor = 33;

        private ScratcherLotto m_Stone;
        private Mobile m_From;

        public ScratcherStoneGump(ScratcherLotto stone, Mobile from) : this( stone, from, true)
        {
        }

        public ScratcherStoneGump(ScratcherLotto stone, Mobile from, bool quickScratch)
            : base(50, 50)
        {
            m_Stone = stone;
            m_From = from;

            AddPage(0);

            AddBackground(50, 0, 350, 350, 9250);

            AddPage(1);

            if (m_From.AccessLevel > AccessLevel.Player)
            {
                AddLabel(70, 265, GMColor, String.Format("Gold Sink: {0}", m_Stone.GoldSink));

                if (ScratcherLotto.Stone != null)
                {
                    AddLabel(230, 265, GMColor, "Next Stats Reset:");
                    AddLabel(230, 295, GMColor, String.Format("{0}", ScratcherLotto.Stone.StatStart + ScratcherLotto.Stone.WipeStats));
                }

                AddLabel(105, 295, GMColor, m_Stone.IsActive ? "Set Game Inactive" : "Set Active");
                AddButton(70, 295, 0xFBD, 0xFBF, 6, GumpButtonType.Reply, 0);
            }

            AddHtml(70, 20, 300, 16, String.Format("<Center>{0} Lottery Scratchers</Center>", ServerList.ServerName), false, false);

            #region Quick/Normal Scratch Radio
            AddLabel(110, 40, labelColor, "Quick Scratch");
            AddLabel(110, 73, labelColor, "Normal Scratch");

            AddRadio(70, 40, 0x25F8, 0x25FB, quickScratch, 0);
            AddRadio(70, 70, 0x25F8, 0x25FB, !quickScratch, 1);
            #endregion

            #region Ticket Info
            AddLabel(60, 117, labelColor, "Buy Ticket");
            AddLabel(230, 117, labelColor, "Cost");
            AddLabel(350, 117, labelColor, "Stats");

            AddLabel(110, 140, 0, "Golden Ticket");
            if (ScratcherLotto.Stone != null && ScratcherLotto.Stone.IsActive)
            {
                AddButton(70, 140, 0xFBD, 0xFBF, (int)TicketType.GoldenTicket, GumpButtonType.Reply, 0);
                AddLabel(230, 140, 0, GoldenTicket.TicketCost.ToString());
                AddButton(350, 140, 0xFA5, 0xFA7, 0, GumpButtonType.Page, (int)TicketType.GoldenTicket + 1);
            }
            else
                AddLabel(230, 140, GMColor, "Offline");

            AddLabel(110, 170, 0, "Crazed Crafting");
            if (ScratcherLotto.Stone != null && ScratcherLotto.Stone.IsActive)
            {
                AddButton(70, 170, 0xFBD, 0xFBF, (int)TicketType.CrazedCrafting, GumpButtonType.Reply, 0);
                AddLabel(230, 170, 0, CrazedCrafting.TicketCost.ToString());
                AddButton(350, 170, 0xFA5, 0xFA7, 0, GumpButtonType.Page, (int)TicketType.CrazedCrafting + 1);
            }
            else
                AddLabel(230, 170, GMColor, "Offline");

            AddLabel(110, 200, 0, "Skies the Limit");
            if (ScratcherLotto.Stone != null && ScratcherLotto.Stone.IsActive)
            {
                AddButton(70, 200, 0xFBD, 0xFBF, (int)TicketType.SkiesTheLimit, GumpButtonType.Reply, 0);
                AddLabel(230, 200, 0, SkiesTheLimit.TicketCost.ToString());
                AddButton(350, 200, 0xFA5, 0xFA7, 0, GumpButtonType.Page, (int)TicketType.SkiesTheLimit + 1);
            }
            else
                AddLabel(230, 200, GMColor, "Offline");

            AddLabel(110, 230, 0, "Powerball");
            if (PowerBall.Instance != null && PowerBall.Game != null && !PowerBall.Game.Deleted && PowerBall.Instance.CanBuyTickets)
            {
                AddLabel(230, 230, 0, PowerBall.Game != null ? PowerBall.Game.TicketCost.ToString() : "");
                AddButton(70, 230, 0xFBD, 0xFBF, (int)TicketType.Powerball, GumpButtonType.Reply, 0);
            }
            else
                AddLabel(230, 230, GMColor, "Offline");

            #endregion

            AddPage(2); //Golden Ticket Stats

            AddLabel(70, 20, labelColor, "Golden Ticket Top 10 Winners");

            int index = 0;
            for(int i = ScratcherStats.Stats.Count - 1; i >= 0; --i)
            {
                if (ScratcherStats.Stats[i].Type == TicketType.GoldenTicket)
                {
                    string num = String.Format("{0:##,###,###}", ScratcherStats.Stats[i].Payout);
                    string name = "unknown player";

                    if (ScratcherStats.Stats[i].Winner != null)
                        name = ScratcherStats.Stats[i].Winner.Name;

                    AddHtml(70, 50 + (index * 25), 100, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", name), false, false);
                    AddHtml(150, 50 + (index * 25), 100, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", num), false, false);
                    AddHtml(270, 50 + (index * 25), 100, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", ScratcherStats.Stats[i].WinTime), false, false);
                    index++;
                }

                if (index >= 9)
                    break;
            }

            AddButton(350, 300, 0xFAE, 0xFB0, 0, GumpButtonType.Page, 1);

            AddPage(3); //Crazed Crafting Stats


            AddLabel(70, 20, labelColor, "Crazed Crafting Highest Winners");

            index = 0;
            for (int i = ScratcherStats.Stats.Count - 1; i >= 0; --i)
            {
                if (ScratcherStats.Stats[i].Type == TicketType.CrazedCrafting)
                {
                    string num = String.Format("{0:##,###,###}", ScratcherStats.Stats[i].Payout);
                    string name = "unknown player";

                    if (ScratcherStats.Stats[i].Winner != null)
                        name = ScratcherStats.Stats[i].Winner.Name;

                    AddHtml(70, 50 + (index * 25), 100, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", name), false, false);
                    AddHtml(150, 50 + (index * 25), 100, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", num), false, false);
                    AddHtml(270, 50 + (index * 25), 100, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", ScratcherStats.Stats[i].WinTime), false, false);
                    index++;
                }

                if (index >= 9)
                    break;
            }

            AddButton(350, 300, 0xFAE, 0xFB0, 0, GumpButtonType.Page, 1);

            AddPage(4); //Skies the Limit Stats

            AddLabel(70, 20, labelColor, "Skies the Limit");
            AddLabel(70, 300, labelColor, String.Format("Progressive Jackpot: {0}", m_Stone.SkiesProgressive));

            index = 0;
            for (int i = ScratcherStats.Stats.Count - 1; i >= 0; --i)
            {
                if (ScratcherStats.Stats[i].Type == TicketType.SkiesTheLimit)
                {
                    string num = String.Format("{0:##,###,###}", ScratcherStats.Stats[i].Payout);
                    string name = "unknown player";

                    if (ScratcherStats.Stats[i].Winner != null)
                        name = ScratcherStats.Stats[i].Winner.Name;

                    AddHtml(70, 50 + (index * 25), 100, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", name), false, false);
                    AddHtml(150, 50 + (index * 25), 100, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", num), false, false);
                    AddHtml(270, 50 + (index * 25), 100, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", ScratcherStats.Stats[i].WinTime), false, false);
                    index++;
                }

                if (index >= 9)
                    break;
            }

            AddButton(350, 300, 0xFAE, 0xFB0, 0, GumpButtonType.Page, 1);
        }

        private BaseLottoTicket FindFreeTicket(Container pack, Type type)
        {
            if (pack == null)
                return null;

            Item[] items = pack.FindItemsByType(typeof(BaseLottoTicket));

            foreach (Item item in items)
            {
                if (item is BaseLottoTicket && item.GetType() == type && ((BaseLottoTicket)item).FreeTicket)
                    return (BaseLottoTicket)item;
            }

            return null;
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_From == null)
                return;

            Container pack = m_From.Backpack;
            bool quickScratch = info.IsSwitched(0);

            switch (info.ButtonID)
            {
                default:
                case 0: break;
                case 1: //Golden Ticket
                    {
                        int cost = GoldenTicket.TicketCost;

                        if (ScratcherLotto.Stone != null && ScratcherLotto.Stone.IsActive)
                        {
                            BaseLottoTicket free = FindFreeTicket(pack, typeof(GoldenTicket));

                            if (free != null && free is GoldenTicket)
                            {
                                free.Delete();
                                m_From.SendMessage("You purchase a lottery ticket with your free ticket.", cost);

                                DropItem(new GoldenTicket(m_From, quickScratch));
                            }
                            else if (pack != null && pack.GetAmount(typeof(Gold)) >= cost)
                            {
                                pack.ConsumeTotal(typeof(Gold), cost);
                                m_From.SendMessage("You purchase a lottery ticket with {0} gold from your backpack.", cost);

                                DropItem(new GoldenTicket(m_From, quickScratch));

                                if (m_Stone != null)
                                    m_Stone.GoldSink += cost;
                            }
                            else if (Banker.Withdraw(m_From, cost, true))
                            {
                                m_From.SendMessage("You purchase a lottery ticket with {0} gold from your bankbox.", cost);

                                DropItem(new GoldenTicket(m_From, quickScratch));

                                if (m_Stone != null)
                                    m_Stone.GoldSink += cost;
                            }
                            else
                                m_From.SendLocalizedMessage(500191); //Begging thy pardon, but thy bank account lacks these funds.
                        }
                        m_From.SendGump(new ScratcherStoneGump(m_Stone, m_From, quickScratch));
                        break;
                    }
                case 2: //Crazed Crafting
                    {
                        int cost = CrazedCrafting.TicketCost;

                        if (ScratcherLotto.Stone != null && ScratcherLotto.Stone.IsActive)
                        {
                            BaseLottoTicket free = FindFreeTicket(pack, typeof(CrazedCrafting));
                            if (free != null && free is CrazedCrafting)
                            {
                                free.Delete();
                                m_From.SendMessage("You purchase a lottery ticket with your free ticket.", cost);

                                DropItem(new CrazedCrafting(m_From, quickScratch));
                            }
                            else if (pack != null && pack.GetAmount(typeof(Gold)) >= cost)
                            {
                                pack.ConsumeTotal(typeof(Gold), cost);
                                m_From.SendMessage("You purchase a lottery ticket with {0} gold from your backpack.", cost);

                                DropItem(new CrazedCrafting(m_From, quickScratch));

                                if (m_Stone != null)
                                    m_Stone.GoldSink += cost;
                            }
                            else if (Banker.Withdraw(m_From, cost, true))
                            {
                                m_From.SendMessage("You purchase a lottery ticket with {0} gold from your bankbox.", cost);

                                DropItem(new CrazedCrafting(m_From, quickScratch));

                                if (m_Stone != null)
                                    m_Stone.GoldSink += cost;
                            }
                            else
                                m_From.SendLocalizedMessage(500191); //Begging thy pardon, but thy bank account lacks these funds.
                        }
                        m_From.SendGump(new ScratcherStoneGump(m_Stone, m_From, quickScratch));
                        break;
                    }
                case 3: //Skies the Limit
                    {
                        int cost = SkiesTheLimit.TicketCost;

                        if (ScratcherLotto.Stone != null && ScratcherLotto.Stone.IsActive)
                        {
                            BaseLottoTicket free = FindFreeTicket(pack, typeof(SkiesTheLimit));
                            if (free != null && free is SkiesTheLimit)
                            {
                                free.Delete();
                                m_From.SendMessage("You purchase a lottery ticket with your free ticket.", cost);

                                DropItem(new SkiesTheLimit(m_From, quickScratch));
                            }
                            else if (pack != null && pack.GetAmount(typeof(Gold)) >= cost)
                            {
                                pack.ConsumeTotal(typeof(Gold), cost);
                                m_From.SendMessage("You purchase a lottery ticket with {0} gold from your backpack.", cost);

                                DropItem(new SkiesTheLimit(m_From, quickScratch));

                                if (m_Stone != null)
                                    m_Stone.GoldSink += cost;

                                m_Stone.SkiesProgressive += cost / 10;
                            }
                            else if (Banker.Withdraw(m_From, cost, true))
                            {
                                m_From.SendMessage("You purchase a lottery ticket with {0} gold from your bankbox.", cost);

                                DropItem(new SkiesTheLimit(m_From, quickScratch));

                                if (m_Stone != null)
                                    m_Stone.GoldSink += cost;

                                m_Stone.SkiesProgressive += cost / 10;
                            }
                            else
                                m_From.SendLocalizedMessage(500191); //Begging thy pardon, but thy bank account lacks these funds.
                        }
                        m_From.SendGump(new ScratcherStoneGump(m_Stone, m_From, quickScratch));
                        break;
                    }
                case 4: //PowerBall Ticket
                    {
                        if (PowerBall.Instance != null && PowerBall.Game != null && !PowerBall.Game.Deleted && PowerBall.Instance.CanBuyTickets)
                        {
                            int cost = PowerBall.Game.TicketCost;

                            if (pack != null && pack.GetAmount(typeof(Gold)) >= cost)
                            {
                                pack.ConsumeTotal(typeof(Gold), cost);
                                m_From.SendMessage("You purchase a Powerball ticket with {0} gold from your backpack.", cost);

                                DropItem(new PowerBallTicket(m_From, PowerBall.Game));

                                if (PowerBall.Instance != null)
                                    PowerBall.Instance.Profit += cost;
                            }
                            else if (Banker.Withdraw(m_From, cost, true))
                            {
                                m_From.SendMessage("You purchase a Powerball ticket with {0} gold from your bankbox.", cost);

                                DropItem(new PowerBallTicket(m_From, PowerBall.Game));

                                if (PowerBall.Instance != null)
                                    PowerBall.Instance.Profit += cost;
                            }
                            else
                                m_From.SendLocalizedMessage(500191); //Begging thy pardon, but thy bank account lacks these funds.
                        }

                        m_From.SendGump(new ScratcherStoneGump(m_Stone, m_From, quickScratch));
                        break;
                    }

                case 5:
                    {
                        if (PowerBall.Game != null)
                           m_From.SendGump(new PowerBallStatsGump(PowerBall.Game, m_From));

                        break;
                    }
                case 6:
                    {
                        if (m_From.AccessLevel == AccessLevel.Player)
                            break;

                        if (m_Stone != null)
                        {
                            if (m_Stone.IsActive)
                            {
                                m_From.SendMessage("set to inactive.");
                                m_Stone.IsActive = false;
                            }
                            else
                            {
                                m_From.SendMessage("set to active.");
                                m_Stone.IsActive = true;
                            }
                        }

                        m_From.SendGump(new ScratcherStoneGump(m_Stone, m_From, quickScratch));
                        break;
                    }
            }
        }

        private void DropItem(Item item)
        {
            Container pack = m_From.Backpack;

            if (pack == null || !pack.TryDropItem(m_From, item, false))
            {
                m_From.SendMessage("Your pack is full, so the ticket has been placed in your bank box!");
                m_From.BankBox.DropItem(item);
            }
        }
    }  
}