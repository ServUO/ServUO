using Server.Gumps;
using Server.Network;
using System.Collections.Generic;

namespace Server.Engines.NewMagincia
{
    public class LottoTrackingGump : Gump
    {
        private readonly int LabelColor = 0xFFFFFF;
        private readonly List<MaginciaHousingPlot> m_List;

        public LottoTrackingGump() : base(50, 50)
        {
            AddBackground(0, 0, 410, 564, 9500);

            AddHtml(205, 10, 205, 20, "<DIV ALIGN=RIGHT><Basefont Color=#FFFFFF>New Magincia Lotto Tracking</DIV>", false, false);
            AddHtml(10, 10, 205, 20, Color(string.Format("Gold Sink: {0}", MaginciaLottoSystem.GoldSink.ToString("###,###,###")), 0xFFFFFF), false, false);

            AddHtml(45, 40, 40, 20, Color("ID", LabelColor), false, false);
            AddHtml(85, 40, 60, 20, Color("Facet", LabelColor), false, false);
            AddHtml(145, 40, 40, 20, Color("#bids", LabelColor), false, false);

            m_List = new List<MaginciaHousingPlot>(MaginciaLottoSystem.Plots);

            int y = 60;
            int x = 0;
            for (int i = 0; i < m_List.Count; i++)
            {
                MaginciaHousingPlot plot = m_List[i];

                if (plot == null)
                    continue;

                int bids = 0;
                foreach (int bid in plot.Participants.Values)
                    bids += bid;

                AddButton(10 + x, y, 4005, 4007, i + 5, GumpButtonType.Reply, 0);
                AddHtml(45 + x, y, 40, 22, Color(plot.Identifier, LabelColor), false, false);
                AddHtml(85 + x, y, 60, 22, Color(plot.Map.ToString(), LabelColor), false, false);

                if (plot.LottoOngoing)
                    AddHtml(145 + x, y, 40, 22, Color(bids.ToString(), LabelColor), false, false);
                else if (plot.Complete)
                    AddHtml(145 + x, y, 40, 22, Color("Owned", "red"), false, false);
                else
                    AddHtml(145 + x, y, 40, 22, Color("Expired", "red"), false, false);

                if (i == 21)
                {
                    y = 60;
                    x = 200;

                    AddHtml(45 + x, 40, 40, 20, Color("ID", LabelColor), false, false);
                    AddHtml(85 + x, 40, 60, 20, Color("Facet", LabelColor), false, false);
                    AddHtml(145 + x, 40, 40, 20, Color("#bids", LabelColor), false, false);
                }
                else
                    y += 22;
            }
        }

        private string Color(string str, int color)
        {
            return string.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, str);
        }

        private string Color(string str, string color)
        {
            return string.Format("<BASEFONT COLOR={0}>{1}</BASEFONT>", color, str);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (info.ButtonID >= 5 && from.AccessLevel > AccessLevel.Player)
            {
                int index = info.ButtonID - 5;

                if (index >= 0 && index < m_List.Count)
                {
                    MaginciaHousingPlot plot = m_List[index];

                    if (plot != null)
                    {
                        from.SendGump(new PlotTrackingGump(plot));
                    }
                }
            }
        }
    }

    public class PlotTrackingGump : Gump
    {
        public PlotTrackingGump(MaginciaHousingPlot plot) : base(50, 50)
        {
            int partCount = plot.Participants.Count;
            int y = 544;
            int x = 600;

            AddBackground(0, 0, x, y, 9500);

            AddHtml(10, 10, 580, 20, string.Format("<Center><Basefont Color=#FFFFFF>Plot {0}</Center>", plot.Identifier), false, false);

            AddHtml(10, 40, 80, 20, Color("Player", 0xFFFFFF), false, false);
            AddHtml(92, 40, 60, 20, Color("Tickets", 0xFFFFFF), false, false);
            AddHtml(154, 40, 60, 20, Color("Total Gold", 0xFFFFFF), false, false);

            x = 0;
            y = 60;
            int goldSink = 0;

            List<Mobile> mobiles = new List<Mobile>(plot.Participants.Keys);
            List<int> amounts = new List<int>(plot.Participants.Values);

            for (int i = 0; i < mobiles.Count; i++)
            {
                Mobile m = mobiles[i];
                int amt = amounts[i];
                int total = amt * plot.LottoPrice;
                goldSink += total;

                AddHtml(10 + x, y, 80, 22, Color(m.Name, 0xFFFFFF), false, false);
                AddHtml(92 + x, y, 60, 22, Color(amt.ToString(), 0xFFFFFF), false, false);
                AddHtml(154 + x, y, 60, 22, Color(total.ToString(), 0xFFFFFF), false, false);

                if (i == 21)
                {
                    x = 200;
                    y = 60;

                    AddHtml(10 + x, 40, 80, 20, Color("Player", 0xFFFFFF), false, false);
                    AddHtml(92 + x, 40, 60, 20, Color("Tickets", 0xFFFFFF), false, false);
                    AddHtml(154 + x, 40, 60, 20, Color("Total Gold", 0xFFFFFF), false, false);
                }
                else if (i == 43)
                {
                    x = 400;
                    y = 60;

                    AddHtml(10 + x, 40, 80, 20, Color("Player", 0xFFFFFF), false, false);
                    AddHtml(92 + x, 40, 60, 20, Color("Tickets", 0xFFFFFF), false, false);
                    AddHtml(154 + x, 40, 60, 20, Color("Total Gold", 0xFFFFFF), false, false);
                }
                else
                    y += 22;
            }

            AddHtml(10, 10, 150, 20, Color(string.Format("Gold Sink: {0}", goldSink.ToString()), 0xFFFFFF), false, false);

            AddButton(10, 544 - 32, 4014, 4016, 1, GumpButtonType.Reply, 0);
            AddHtml(45, 544 - 32, 150, 20, Color("Back", 0xFFFFFF), false, false);
        }

        private string Color(string str, int color)
        {
            return string.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, str);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (info.ButtonID == 1)
                from.SendGump(new LottoTrackingGump());
        }
    }
}