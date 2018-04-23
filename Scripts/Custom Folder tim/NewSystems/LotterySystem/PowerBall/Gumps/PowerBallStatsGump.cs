using System; 
using Server; 
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Network;
using Server.Engines.LotterySystem;

namespace Server.Gumps
{ 
    public class PowerBallStatsGump : Gump
    {
        private const int labelColor = 2106;
        private const int GMColor = 33;

        private PowerBall m_PowerBall;
        private Mobile m_From;

        public PowerBallStatsGump(PowerBall item, Mobile from)
            : base(50, 50)
        {
            m_PowerBall = item;
            m_From = from;

            int gameNumber = 0;

            if (PowerBall.Game != null)
                gameNumber = PowerBall.Game.GameNumber;

            AddPage(0);

            AddBackground(50, 0, 350, 350, 9250);

            AddPage(1);

            if (PowerBall.Game != null && PowerBall.Game.IsActive)
                AddLabel(70, 15, labelColor, String.Format("Current Powerball Game: {0}", gameNumber));
            else
                AddLabel(70, 20, GMColor, "Offline");

            if (m_From.AccessLevel > AccessLevel.Player)
            {
                AddLabel(70, 40, GMColor, String.Format("Gold Sink: {0}", PowerBall.GoldSink));

                if (PowerBall.Instance != null)
                {
                    AddLabel(225, 40, GMColor, "Current Game's Profit:");
                    AddLabel(325, 60, GMColor, String.Format("{0}", PowerBall.Instance.Profit));
                }

                AddLabel(70, 185, GMColor, "*Warning* GM Options *Warning*");

                AddLabel(105, 215, GMColor, "Reset All Stats");
                AddButton(70, 215, 0xFBD, 0xFBF, 2, GumpButtonType.Reply, 0);

                AddLabel(105, 245, GMColor, "Current Game Guaratee Jackpot");
                AddButton(70, 245, 0xFBD, 0xFBF, 3, GumpButtonType.Reply, 0);

                AddLabel(105, 275, GMColor, m_PowerBall.IsActive ? "Set Game Inactive" : "Set Active");
                AddButton(70, 275, 0xFBD, 0xFBF, 4, GumpButtonType.Reply, 0);

                if (PowerBall.Game != null && PowerBall.Game.MaxWhiteBalls <= 20)
                {
                    long odds = GetOdds();
                    if (odds > 6)
                        AddLabel(70, 305, GMColor, String.Format("Jackpot Odds: 1 in {0}", odds));
                }
            }
            else
            {
                int hue = 2041;

                AddImage(70, 200, 0x15A9, hue);
                AddImage(115, 265, 0x15A9, hue);
                AddImage(160, 200, 0x15A9, hue);
                AddImage(205, 265, 0x15A9, hue);
                AddImage(250, 200, 0x15A9, hue);
                AddImage(295, 265, 0x15A9, 0x21);

                TimeSpan ts = m_PowerBall.NextGame - DateTime.Now;
                string text;

                if (ts.TotalHours >= 1)
                {
                    int minutes = (int)ts.TotalMinutes - (((int)ts.TotalHours) * 60);
                    text = String.Format("Next Picks: {0} {1} and {2} {3}", ((int)ts.TotalHours).ToString(), ts.TotalHours == 1 ? "hour" : "hours", minutes.ToString(), ts.TotalMinutes == 1 ? "minute" : "minutes");
                }
                else
                    text = String.Format("Next Picks: {0} {1}", ((int)ts.TotalMinutes).ToString(), ts.TotalMinutes == 1 ? "minute" : "minutes");

                AddLabel(70, 40, labelColor, text);
            }

            AddHtml(105, 65, 200, 16, "<Basefont Color=#FFFFFF>Purchase Ticket</Basefont>", false, false);
            if (PowerBall.Instance != null && PowerBall.Instance.CanBuyTickets)
                AddButton(70, 65, 0xFBD, 0xFBF, 1, GumpButtonType.Reply, 0);
            //else
            //    AddImage(70, 65, 0xFB4);

            AddButton(70, 95, 0xFBD, 0xFBF, 0, GumpButtonType.Page, 2);
            AddHtml(105, 95, 200, 16, "<Basefont Color=#FFFFFF>Archived Picks</Basefont>", false, false);

            AddButton(70, 125, 0xFBD, 0xFBF, 0, GumpButtonType.Page, 3);
            AddHtml(105, 125, 200, 16, "<Basefont Color=#FFFFFF>Jackpot Winners</Basefont>", false, false);

            AddHtml(105, 155, 200, 16, "<Basefont Color=#FFFFFF>Current Game Info</Basefont>", false, false);
            if (m_PowerBall.IsActive)
                AddButton(70, 155, 0xFBD, 0xFBF, 0, GumpButtonType.Page, 4);
            //else
            //    AddImage(70, 155, 0xFB4);

            AddPage(2); //Archived Picks

            AddLabel(70, 20, labelColor, "Game");
            AddLabel(150, 20, labelColor, "Last 10 Picks");
            AddLabel(275, 20, labelColor, "Total Payout");

            int count = 0;
            for (int i = PowerBallStats.PicksStats.Count - 1; i >= 0; --i)
            {
                PowerBallStats stats = PowerBallStats.PicksStats[i];
                string payout = String.Format("{0:##,###,###}", stats.Payout);

                AddHtml(70, 50 + (count * 25), 50, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", stats.GameNumber.ToString()), false, false);
                AddHtml(305, 50 + (count * 25), 50, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", payout), false, false);

                if (stats.Picks != null && stats.Picks.Count > 0)
                {
                    for (int j = 0; j < stats.Picks.Count; ++j)
                    {
                        if (j + 1 < stats.Picks.Count)
                            AddHtml(125 + (j * 25), 50 + (count * 25), 25, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", stats.Picks[j]), false, false);
                        else
                            AddHtml(250, 50 + (count * 25), 25, 16, String.Format("<Basefont Color=#FF0000>{0}</Basefont>", stats.Picks[j]), false, false);
                    }
                }

                if (count++ >= 9)
                    break;

            }

            AddButton(350, 300, 0xFAE, 0xFB0, 0, GumpButtonType.Page, 1);

            AddPage(3); //Jackpot Winners

            AddLabel(70, 20, labelColor, "Player");
            AddLabel(150, 20, labelColor, "Jackpot Amount");
            AddLabel(300, 20, labelColor, "Time");

            count = 0;
            for (int i = PowerBallStats.JackpotStats.Count - 1; i >= 0; --i)
            {
                PowerBallStats stats = PowerBallStats.JackpotStats[i];
                Mobile mob = stats.JackpotWinner;
                string jackpotamount = String.Format("{0:##,###,### Gold}", stats.JackpotAmount);

                AddHtml(70, 50 + (count * 25), 100, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", mob != null ? mob.Name : "Unanimous"), false, false);
                AddHtml(150, 50 + (count * 25), 100, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", jackpotamount), false, false);
                AddHtml(270, 50 + (count * 25), 100, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", stats.JackpotTime.ToString()), false, false);

                if (count++ >= 9)
                    break;
            }

            AddButton(350, 300, 0xFAE, 0xFB0, 0, GumpButtonType.Page, 1);

            AddPage(4); //Current Game info

            List<Mobile> ticketList = new List<Mobile>();
            if (PowerBall.Instance != null)
            {
                foreach (PowerBallTicket ticket in PowerBall.Instance.Tickets)
                {
                    if (!ticketList.Contains(ticket.Owner) && ticket.Owner != null)
                        ticketList.Add(ticket.Owner);
                }
            }

            AddLabel(70, 20, labelColor, "Current Game Information");

            if (PowerBall.Instance != null || PowerBall.Instance.PowerBall != null && PowerBall.Instance.PowerBall.IsActive)
            {
                AddLabel(70, 80, labelColor, "Game Number:");
                AddHtml(200, 80, 300, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", gameNumber), false, false);

                string jackpotamount = String.Format("{0:##,###,### Gold}", PowerBall.Instance.JackPot);
                AddLabel(70, 110, labelColor, "Jackpot Amount:");
                AddHtml(200, 110, 300, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", PowerBall.Instance.JackPot > 0 ? jackpotamount : "0 Gold"), false, false);

                AddLabel(70, 140, labelColor, "Players:");
                AddHtml(200, 140, 300, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", ticketList.Count), false, false);

                if (PowerBall.Instance != null)
                {
                    int entryCount = 0;
                    foreach (PowerBallTicket t in PowerBall.Instance.Tickets)
                        foreach (TicketEntry e in t.Entries)
                            entryCount++;

                    AddLabel(70, 170, labelColor, "Tickets:");
                    AddHtml(200, 170, 300, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", entryCount.ToString()), false, false);

                    AddLabel(70, 200, labelColor, "Next Picks:");
                    AddHtml(200, 200, 300, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", PowerBall.Instance.PowerBall.NextGame), false, false);
                }

                if (ticketList != null && ticketList.Count > 0)
                {
                    AddButton(70, 300, 0x483, 0x481, 0, GumpButtonType.Page, 5);
                    AddHtml(110, 300, 200, 16, "<Basefont Color=#FFFFFF>Players</Basefont>", false, false);
                }
            }
            else
                AddLabel(70, 80, labelColor, "Inactive");

            AddButton(350, 300, 0xFAE, 0xFB0, 0, GumpButtonType.Page, 1);

            AddPage(5);

            AddLabel(70, 20, labelColor, "Players in Current Game");

            for (int k = 0; k < ticketList.Count; ++k)
            {
                if (k < 12)
                    AddHtml(70, 45 + (k * 25), 100, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", ticketList[k] != null ? ticketList[k].Name : "Unanimous"), false, false);
                else if (k < 24)
                    AddHtml(180, 45 + ((k - 12) * 25), 100, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", ticketList[k] != null ? ticketList[k].Name : "Unanimous"), false, false);
                else if (k < 36)
                    AddHtml(290, 45 + ((k - 24) * 25), 100, 16, String.Format("<Basefont Color=#FFFFFF>{0}</Basefont>", ticketList[k] != null ? ticketList[k].Name : "Unanimous"), false, false);

            }

            AddButton(290, 20, 0xFB1, 0xFB3, 0, GumpButtonType.Page, 1);

        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                default:
                case 0: break;
                case 1:
                    {
                        Container pack = m_From.Backpack;

                        if (PowerBall.Instance != null && PowerBall.Game != null && PowerBall.Instance.CanBuyTickets)
                        {
                            int cost = PowerBall.Game.TicketCost;
                            if (pack != null && pack.GetAmount(typeof(Gold)) >= cost)
                            {
                                pack.ConsumeTotal(typeof(Gold), cost);
                                m_From.SendMessage("You purchase a Powerball ticket with {0} gold from your backpack.", cost);
                                pack.DropItem(new PowerBallTicket(m_From, m_PowerBall));

                                if (PowerBall.Instance != null)
                                    PowerBall.Instance.Profit += cost;
                            }
                            else if (Banker.Withdraw(m_From, cost, true))
                            {
                                var ticket = new PowerBallTicket(m_From, m_PowerBall);

                                if (!pack.TryDropItem(m_From, ticket, false))
                                {
                                    m_From.SendMessage("There is no room in your pack, so the ticket was placed in your bank box!");
                                    m_From.BankBox.DropItem(ticket);
                                }

                                if (PowerBall.Instance != null)
                                    PowerBall.Instance.Profit += cost;
                            }
                            else
                                m_From.SendLocalizedMessage(500191); //Begging thy pardon, but thy bank account lacks these funds.
                        }

                        m_From.SendGump(new PowerBallStatsGump(m_PowerBall, m_From));
                        break;
                    }
                case 2:  //Reset all stats
                    {
                        if (m_From.AccessLevel == AccessLevel.Player)
                            break;

                        for (int i = 0; i < PowerBallStats.PicksStats.Count; ++i)
                        {
                            PowerBallStats.PicksStats.Clear();
                        }

                        for (int i = 0; i < PowerBallStats.JackpotStats.Count; ++i)
                        {
                            PowerBallStats.JackpotStats.Clear();
                        }

                        if (PowerBall.Instance != null)
                        {
                            PowerBall.Game.InvalidateProperties();
                            PowerBall.Game.UpdateSatellites();
                        }

                        m_From.SendMessage("Stats erased!");
                        m_From.SendGump(new PowerBallStatsGump(m_PowerBall, m_From));

                        break;
                    }
                case 3:
                    {
                        if (m_From.AccessLevel == AccessLevel.Player)
                            break;

                        if (!m_PowerBall.DoJackpot)
                        {
                            m_PowerBall.DoJackpot = true;
                            m_From.SendGump(new PowerBallStatsGump(m_PowerBall, m_From));
                            m_From.SendMessage("Next picks will result in a jackpot!");
                        }

                        break;
                    }
                case 4:
                    {
                        if (m_From.AccessLevel == AccessLevel.Player)
                            break;

                        if (m_PowerBall.IsActive)
                        {
                            m_PowerBall.IsActive = false;
                            m_From.SendMessage("Powerball set to inactive.");
                        }
                        else
                        {
                            m_PowerBall.IsActive = true;
                            m_From.SendMessage("Powerball set to active.");
                        }
                        m_From.SendGump(new PowerBallStatsGump(m_PowerBall, m_From));
                        break;
                    }
            }
        }

        private long GetOdds()
        {
            if (PowerBall.Game == null)
                return 1;

            int white = PowerBall.Game.MaxWhiteBalls;
            int red = PowerBall.Game.MaxRedBalls;

            long amount1 = GetFact(white);
            long amount2 = GetFact(5);
            long amount3 = GetFact(white - 5);

            try
            {
                return (amount1 / (amount2 * amount3)) * red;
            }
            catch
            {
                return 1;
            }
        }

        private long GetFact(int balls)
        {
            long value = balls;
            
            for (int i = balls; i > 1; --i)
                value *= (i - 1);
            
            return value;
        }
    }  
}