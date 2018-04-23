using System; 
using Server; 
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Network;
using Server.Misc;
using Server.Engines.LotterySystem;

namespace Server.Gumps
{ 
    public class PowerballTicketGump : Gump
    {
        private const int labelColor = 88;
        private const int maxEntries = 10;

        private PowerBallTicket m_Ticket;
        private Mobile m_From;

        public PowerballTicketGump(PowerBallTicket ticket, Mobile from) : this(ticket, from, true)
        {
        }

        public PowerballTicketGump(PowerBallTicket ticket, Mobile from, bool powerBall)
            : base(50, 50)
        {
            m_Ticket = ticket;
            m_From = from;

            if (ticket.Owner == null)
                ticket.Owner = from;

            int gameNumber = 0;

            if (PowerBall.Game != null)
                gameNumber = PowerBall.Game.GameNumber;

            int entries = ticket.Entries.Count;
            int yoffSet = 25;
            string title = String.Format("<BASEFONT SIZE=8 COLOR=#FF0000><Center>Powerball</Center></BASEFONT>");

            AddBackground(50, 0, 300, 200 + (yoffSet * entries), 9270);
            AddHtml(65, 170 + (yoffSet * entries), 350, 20, String.Format("<Basefont Size=2>Lotto Association of {0}, All Rights Reserved</Basefont>", ServerList.ServerName), false, false);

            AddHtml(50, 20, 300, 30, title, false, false);
            AddLabel(65, 50, labelColor, String.Format("Picks: {0}", entries));
            AddLabel(65, 70, labelColor, String.Format("Game:  {0}", m_Ticket.GameNumber));

            if (ticket.GameNumber == gameNumber && entries < maxEntries)
            {
                AddHtml(180, 50, 80, 16, "<BASEFONT COLOR=#FFFFFF>Powerball</BASEFONT>", false, false);
                AddRadio(260, 50, 0x25F8, 0x25FB, powerBall, 0);
                AddHtml(180, 80, 80, 16, "<BASEFONT COLOR=#FFFFFF>No Powerball</BASEFONT>", false, false);
                AddRadio(260, 80, 0x25F8, 0x25FB, !powerBall, 1);

                AddLabel(245, 120, labelColor, "Quick Picks");
                AddButton(321, 123, 0x837, 0x838, 2, GumpButtonType.Reply, 0); //Quick Pick
            }

            if (entries > 0)
            {
                AddLabel(65, 120, labelColor, "Pick");

                for (int i = 0; i < ticket.Entries.Count; ++i)
                {
                    TicketEntry entry = ticket.Entries[i];
                    int index = i + 1;

                    List<int> entryList = new List<int>();
                    entryList.Add(entry.One);
                    entryList.Add(entry.Two);
                    entryList.Add(entry.Three);
                    entryList.Add(entry.Four);
                    entryList.Add(entry.Five);

                    AddLabel(75, 150 + (i * yoffSet), labelColor, String.Format("{0}", index.ToString()));
                    AddHtml(270, 150 + (i * yoffSet), 50, 16, String.Format("<BASEFONT COLOR=#FF0000>{0}</BASEFONT>", entry.PowerBall > 0 ? entry.PowerBall.ToString() : "<B>-</B>"), false, false);
                    AddHtml(310, 150 + (i * yoffSet), 50, 16, entry.Winner ? "<BASEFONT COLOR=#FFFFFF><b>*</b></BASEFONT>" : "", false, false);

                    for (int j = entryList.Count - 1; j >= 0; --j)
                    {
                        AddHtml(105 + (j * 30), 150 + (i * yoffSet), 50, 16, String.Format("<BASEFONT COLOR=#FFFFFF>{0}</BASEFONT>", entryList[j]), false, false);
                    }

                    ColUtility.Free(entryList);
                }
            }

            if (entries < maxEntries && ticket.GameNumber == gameNumber)
            {
                int yStart = 130 + (yoffSet * (entries));

                AddButton(75, yStart + (yoffSet + 2), 0x837, 0x838, 1, GumpButtonType.Reply, 0);

                AddTextEntry(100, yStart + yoffSet, 20, 16, labelColor, 1, "*");
                AddTextEntry(130, yStart + yoffSet, 20, 16, labelColor, 2, "*");
                AddTextEntry(160, yStart + yoffSet, 20, 16, labelColor, 3, "*");
                AddTextEntry(190, yStart + yoffSet, 20, 16, labelColor, 4, "*");
                AddTextEntry(220, yStart + yoffSet, 20, 16, labelColor, 5, "*");
                AddTextEntry(250, yStart + yoffSet, 20, 16, labelColor, 6, "*");

            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            bool powerball = info.IsSwitched(0);

            if (PowerBall.Game == null || m_Ticket.GameNumber != PowerBall.Game.GameNumber)
                return;

            int white = PowerBall.Game.MaxWhiteBalls;
            int red = PowerBall.Game.MaxRedBalls;

            switch (info.ButtonID)
            {
                default:
                case 0: m_From.CloseGump(typeof(PowerballTicketGump)); break;
                case 1:
                    {
                        int pick1 = 0; int pick2 = 0; int pick3 = 0;
                        int pick4 = 0; int pick5 = 0; int pick6 = 0;

                        TextRelay s1 = info.GetTextEntry(1);
                        TextRelay s2 = info.GetTextEntry(2);
                        TextRelay s3 = info.GetTextEntry(3);
                        TextRelay s4 = info.GetTextEntry(4);
                        TextRelay s5 = info.GetTextEntry(5);
                        TextRelay s6 = info.GetTextEntry(6);

                        try
                        {
                            pick1 = Convert.ToInt32(s1.Text);
                            pick2 = Convert.ToInt32(s2.Text);
                            pick3 = Convert.ToInt32(s3.Text);
                            pick4 = Convert.ToInt32(s4.Text);
                            pick5 = Convert.ToInt32(s5.Text);
                            pick6 = Convert.ToInt32(s6.Text);
                        }
                        catch
                        {
                        }

                        if (PowerBall.Instance != null && PowerBall.Game != null)
                        {

                            if (pick1 < 1 || pick2 < 1 || pick3 < 1 || pick4 < 1 || pick5 < 1)
                                m_From.SendMessage(String.Format("You must choose a number from 1 to {0} on a non-powerball pick.", white.ToString()));
                            else if (pick1 == pick2 || pick1 == pick3 || pick1 == pick4 || pick1 == pick5 || pick2 == pick3 || pick2 == pick4 || pick2 == pick5 || pick3 == pick4 || pick3 == pick5 || pick4 == pick5)
                                m_From.SendMessage("You should think twice before picking the same number twice.");
                            else if (pick1 > white || pick2 > white || pick3 > white || pick4 > white || pick5 > white)
                                m_From.SendMessage(String.Format("White numbers cannot be any higher than {0}.", white.ToString()));
                            else if (pick6 > red)
                                m_From.SendMessage(String.Format("Red numbers cannot be any higher than {0}.", red.ToString()));
                            else if (powerball)
                            {
                                if (pick6 < 1)
                                    m_From.SendMessage(String.Format("You must choose a number from 1 to {0} on your powerball pick.", red.ToString()));
                                else
                                {
                                    Container pack = m_From.Backpack;
                                    int price = PowerBall.Game.TicketEntryPrice + PowerBall.Game.PowerBallPrice;

                                    if (pack != null && pack.GetAmount(typeof(Gold)) >= price)
                                    {
                                        pack.ConsumeTotal(typeof(Gold), price);
                                        m_From.SendMessage("You purchase a powerball ticket with {0} gold from your backpack.", price);
                                        new TicketEntry(m_Ticket, pick1, pick2, pick3, pick4, pick5, pick6, false);
                                        PowerBall.Instance.Profit += price;
                                    }
                                    else if (Banker.Withdraw(m_From, price, true))
                                    {
                                        new TicketEntry(m_Ticket, pick1, pick2, pick3, pick4, pick5, pick6, false);

                                        PowerBall.Instance.Profit += price;
                                    }
                                    else
                                        m_From.SendLocalizedMessage(500191); //Begging thy pardon, but thy bank account lacks these funds.
                                }
                            }
                            else
                            {
                                Container pack = m_From.Backpack;
                                int price = PowerBall.Game.TicketEntryPrice;

                                if (pack != null && pack.GetAmount(typeof(Gold)) >= price)
                                {
                                    pack.ConsumeTotal(typeof(Gold), price);
                                    m_From.SendMessage("You purchase a powerball ticket with {0} gold from your backpack.", price);
                                    new TicketEntry(m_Ticket, pick1, pick2, pick3, pick4, pick5, false);
                                    PowerBall.Instance.Profit += price;
                                }
                                else if (Banker.Withdraw(m_From, price, true))
                                {
                                    new TicketEntry(m_Ticket, pick1, pick2, pick3, pick4, pick5, false);

                                    PowerBall.Instance.Profit += price;
                                }
                                else
                                    m_From.SendLocalizedMessage(500191); //Begging thy pardon, but thy bank account lacks these funds.

                            }
                        }

                        m_From.SendGump(new PowerballTicketGump(m_Ticket, m_From, powerball));
                        break;
                    }
                case 2: //Quickpick
                    {
                        Container pack = m_From.Backpack;
                        int price;

                        if (powerball)
                            price = PowerBall.Game.TicketEntryPrice + PowerBall.Game.PowerBallPrice;
                        else
                            price = PowerBall.Game.TicketEntryPrice;

                        if (pack != null && pack.GetAmount(typeof(Gold)) >= price)
                        {
                            m_From.SendMessage("You purchase a powerball ticket with {0} gold from your backpack.", price);
                            pack.ConsumeTotal(typeof(Gold), price);
                        }
                        else if (!Banker.Withdraw(m_From, price, true))
                        {
                            m_From.SendLocalizedMessage(1060398, price.ToString()); //~1_AMOUNT~ gold has been withdrawn from your bank box.

                            m_From.SendLocalizedMessage(500191); //Begging thy pardon, but thy bank account lacks these funds.
                            m_From.SendGump(new PowerballTicketGump(m_Ticket, m_From, powerball));
                            break;
                        }

                        int pick1 = 0; int pick2 = 0; int pick3 = 0;
                        int pick4 = 0; int pick5 = 0; int pick6 = 0;

                        List<int> whiteList = new List<int>();

                        for (int i = 1; i < white + 1; ++i)
                            whiteList.Add(i);

                        int count = 0;
                        while (++count < 6)
                        {
                            int ran = Utility.Random(whiteList.Count);

                            if (count == 1)
                                pick1 = whiteList[ran];
                            else if (count == 2)
                                pick2 = whiteList[ran];
                            else if (count == 3)
                                pick3 = whiteList[ran];
                            else if (count == 4)
                                pick4 = whiteList[ran];
                            else
                                pick5 = whiteList[ran];

                            whiteList.Remove(whiteList[ran]);

                            pick6 = Utility.RandomMinMax(1, red);
                        }

                        if (powerball)
                        {
                            new TicketEntry(m_Ticket, pick1, pick2, pick3, pick4, pick5, pick6, false);
                            PowerBall.Instance.Profit += price;
                        }
                        else
                        {
                            new TicketEntry(m_Ticket, pick1, pick2, pick3, pick4, pick5, false);
                            PowerBall.Instance.Profit += price;
                        }
                        m_From.SendGump(new PowerballTicketGump(m_Ticket, m_From, powerball));
                    }
                    break;
            }
            
        }
    }  
}