using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;

namespace Server.Engines.NewMagincia
{
    public class MaginciaLottoGump : Gump
    {
        private readonly MaginciaHousingPlot m_Plot;
        private readonly Mobile m_From;

        private readonly int BlueColor = 0x00BFFF;
        private readonly int LabelColor = 0xFFFFFF;
        private readonly int EntryColor = 0xE9967A;

        public MaginciaLottoGump(Mobile from, MaginciaHousingPlot plot) : base(75, 75)
        {
            m_Plot = plot;
            m_From = from;

            bool prime = plot.IsPrimeSpot;

            int ticketsBought = 0;
            if (plot.Participants.ContainsKey(from))
                ticketsBought = plot.Participants[from];

            int totalTicketsSold = 0;
            foreach (int i in plot.Participants.Values)
                totalTicketsSold += i;

            AddBackground(0, 0, 350, 380, 9500);

            AddHtmlLocalized(10, 10, 200, 20, 1150460, BlueColor, false, false); // New Magincia Housing Lottery

            AddHtmlLocalized(10, 50, 75, 20, 1150461, BlueColor, false, false); // This Facet:
            AddHtml(170, 50, 100, 16, string.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", LabelColor, plot.Map.ToString()), false, false);

            AddHtmlLocalized(10, 70, 75, 20, 1150462, BlueColor, false, false); // This Plot:
            AddHtml(170, 70, 100, 16, string.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", EntryColor, plot.Identifier), false, false);

            AddHtmlLocalized(10, 95, 130, 20, 1150463, BlueColor, false, false); // Total Tickets Sold:
            AddHtml(170, 95, 100, 16, string.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", EntryColor, totalTicketsSold.ToString()), false, false);

            AddHtmlLocalized(10, 110, 320, 40, prime ? 1150464 : 1150465, LabelColor, false, false);

            AddHtmlLocalized(10, 160, 90, 20, 1150466, BlueColor, false, false); // Your Tickets:
            AddHtml(170, 160, 100, 20, string.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", EntryColor, ticketsBought.ToString()), false, false);

            if (ticketsBought == 0)
                AddHtmlLocalized(10, 175, 320, 40, 1150467, LabelColor, false, false); // You have not bought a ticket, so you have no chance of winning this plot.
            else
            {
                int odds = totalTicketsSold / ticketsBought;

                AddHtmlLocalized(10, 175, 320, 40, 1150468, odds.ToString(), LabelColor, false, false); //  Your chances of winning this plot are currently about 1 in ~1_ODDS~
            }

            AddHtmlLocalized(10, 225, 115, 20, 1150472, BlueColor, false, false); // Price Per Ticket:
            AddHtml(170, 225, 100, 20, string.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", EntryColor, plot.LottoPrice.ToString("###,###,###")), false, false);

            if (plot.LottoOngoing)
            {
                if (!prime)
                {
                    AddButton(310, 245, 4005, 4007, 1, GumpButtonType.Reply, 0);
                    AddImageTiled(170, 245, 130, 20, 3004);
                    AddTextEntry(172, 245, 126, 16, 0, 0, "");
                    AddHtmlLocalized(10, 245, 100, 20, 1150477, BlueColor, false, false); // Buy Tickets
                }
                else
                {
                    if (plot.CanPurchaseLottoTicket(from))
                    {
                        AddButton(125, 245, 4014, 4007, 2, GumpButtonType.Reply, 0);
                        AddHtmlLocalized(165, 242, 100, 20, 1150469, BlueColor, false, false); // Buy Ticket
                    }
                    else
                        AddHtmlLocalized(10, 240, 320, 40, 1150475, LabelColor, false, false); // You may not purchase another ticket for this plot's lottery.
                }
            }
            else
                AddHtml(10, 240, 320, 40, "<BASEFONT COLOR=#{0:X6}>The lottery on this plot is currently disabled.</BASEFONT>", false, false);

            TimeSpan ts = plot.LottoEnds - DateTime.UtcNow;

            AddHtmlLocalized(10, 300, 320, 40, 1150476, LabelColor, false, false); // Ticket purchases are NONREFUNDABLE. Odds of winning may vary.

            if (ts.Days > 0)
                AddHtmlLocalized(10, 340, 320, 20, 1150504, ts.Days.ToString(), LabelColor, false, false); // There are ~1_DAYS~ days left before the drawing.
            else
                AddHtmlLocalized(10, 340, 320, 20, 1150503, LabelColor, false, false); // The lottery drawing will happen in less than 1 day.
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            bool prime = m_Plot.IsPrimeSpot;

            if (info.ButtonID == 0)
                return;

            if ((prime && !m_Plot.CanPurchaseLottoTicket(m_From)) || !m_Plot.LottoOngoing)
                return;

            int pricePer = m_Plot.LottoPrice;
            int total = pricePer;
            int toBuy = 1;

            if (!prime && info.ButtonID == 1)
            {
                toBuy = 0;
                TextRelay relay = info.GetTextEntry(0);
                string text = relay.Text;

                try
                {
                    toBuy = Convert.ToInt32(text);
                }
                catch (Exception e)
                {
                    Diagnostics.ExceptionLogging.LogException(e);
                }

                if (toBuy <= 0)
                    return;
            }

            if (toBuy > 1)
                total = toBuy * pricePer;

            if (Banker.Withdraw(m_From, total))
            {
                MaginciaLottoSystem.GoldSink += total;
                m_From.SendLocalizedMessage(1150480, string.Format("{0}\t{1}\t{2}", toBuy.ToString(), pricePer.ToString(), total.ToString())); // Purchase of ~1_COUNT~ ticket(s) at ~2_PRICE~gp each costs a total of ~3_TOTAL~. The funds have been withdrawn from your bank box and your ticket purchase has been recorded.
                m_Plot.PurchaseLottoTicket(m_From, toBuy);
            }
            else
                m_From.SendLocalizedMessage(1150479, string.Format("{0}\t{1}\t{2}", toBuy.ToString(), pricePer.ToString(), total.ToString())); // Purchase of ~1_COUNT~ ticket(s) at ~2_PRICE~gp each costs a total of ~3_TOTAL~. You do not have the required funds in your bank box to make the purchase.
        }
    }
}
