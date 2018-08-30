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
    public class ScratcherGump : Gump
    {
        private const int labelColor = 88;

        private BaseLottoTicket m_Ticket;
        private Mobile m_From;

        public ScratcherGump(BaseLottoTicket ticket, Mobile from)
            : base(50, 50)
        {
            m_Ticket = ticket;
            m_From = from;
            int bg = GetBackGround();

            AddBackground(50, 0, 400, 200, bg);

            AddImage(125, 12, 0xFC4);
            AddImage(325, 12, 0xFC4);

            AddHtml(50, 20, 400, 30, String.Format("<BASEFONT SIZE=9><Center>{0}</Center></BASEFONT>", ScratcherLotto.GetGameType(m_Ticket.Type)), false, false);

            if (m_Ticket.Checked && m_Ticket.Payout > 0)
                AddHtml(75, 150, 200, 20, String.Format("<Basefont Color=#FFFF00>Winnings: {0}</Basefont>", m_Ticket.Payout), false, false);

            switch (m_Ticket.Type)
            {
                case TicketType.GoldenTicket: GoldTicket(); break;
                case TicketType.CrazedCrafting: CrazedCrafting(); break;
                case TicketType.SkiesTheLimit: SkiesTheLimit(); break;
            }

            AddHtml(75, 170, 350, 20, String.Format("<Basefont Size=2>Lotto Association of {0}, All Rights Reserved</Basefont>", ServerList.ServerName), false, false);
        }

        private void GoldTicket()
        {
            int yStart = 90;

            if (m_Ticket.Scratch1 == 0)
                AddButton(80, yStart, 0x98B, 0x98B, 1, GumpButtonType.Reply, 0);
            else if (m_Ticket.Scratch1 == 2)
                AddHtml(60, yStart, 100, 60, String.Format("<Basefont Size=8 COLOR=#FFFF00><Center>Free</Center></Basefont>"), false, false);
            else
            {
                string num = String.Format("{0:##,###,###}", m_Ticket.Scratch1);
                AddHtml(60, yStart, 100, 60, String.Format("<Basefont Size=8 COLOR=#FFFF00><Center>{0}</Center></Basefont>", num), false, false);
            }

            if (m_Ticket.Scratch2 == 0)
                AddButton(220, yStart, 0x98B, 0x98B, 2, GumpButtonType.Reply, 0);
            else if (m_Ticket.Scratch2 == 2)
                AddHtml(200, yStart, 100, 60, String.Format("<Basefont Size=8 COLOR=#FFFF00><Center>Free</Center></Basefont>"), false, false);
            else
            {
                string num = String.Format("{0:##,###,###}", m_Ticket.Scratch2);
                AddHtml(200, yStart, 100, 60, String.Format("<Basefont Size=8 COLOR=#FFFF00><Center>{0}</Center></Basefont>", num), false, false);
            }

            if (m_Ticket.Scratch3 == 0)
                AddButton(360, yStart, 0x98B, 0x98B, 3, GumpButtonType.Reply, 0);
            else if (m_Ticket.Scratch3 == 2)
                AddHtml(340, yStart, 100, 60, String.Format("<Basefont Size=8 COLOR=#FFFF00><Center>Free</Center></Basefont>"), false, false);
            else
            {
                string num = String.Format("{0:##,###,###}", m_Ticket.Scratch3);
                AddHtml(340, yStart, 100, 60, String.Format("<Basefont Size=8 COLOR=#FFFF00><Center>{0}</Center></Basefont>", num), false, false);
            }
        }

        private void CrazedCrafting()
        {
            int yStart = 75;

            AddImage(70, yStart - 10, 0x589);
            AddImage(210, yStart - 10, 0x589);
            AddImage(350, yStart - 10, 0x589);

            CrazedCrafting ticket = null;

            if (m_Ticket is CrazedCrafting)
                ticket = (CrazedCrafting)m_Ticket;

            if (ticket != null)
            {
                if (m_Ticket.Scratch1 == 0)
                    AddButton(79, yStart, 0x15C3, 0x15C4, 1, GumpButtonType.Reply, 0);
                else if (m_Ticket.Scratch1 == 2)
                    AddHtml(71, yStart + 20, 80, 20, String.Format("<Basefont Size=6 Color=#FFFF00><Center>Free</Center></Basefont>"), false, false);
                else
                {
                    bool wildCard = false;
                    foreach (int num in ticket.WildCards)
                    {
                        if (m_Ticket.Scratch1 == num)
                        {
                            AddImage(80, yStart, num, 2);
                            wildCard = true;
                        }
                    }

                    if (!wildCard)
                    {
                        string value = String.Format("{0:##,###,###}", ticket.Scratch1);
                        AddHtml(71, yStart + 20, 80, 20, String.Format("<Basefont Size=6 Color=#FFFF00><Center>{0}</Center></Basefont>", value), false, false);
                        AddImage(80, yStart, 0x15AA, 1);
                    }
                }

                if (m_Ticket.Scratch2 == 0)
                    AddButton(219, yStart, 0x15C3, 0x15C4, 2, GumpButtonType.Reply, 0);
                else if (m_Ticket.Scratch2 == 2)
                    AddHtml(211, yStart + 20, 80, 20, String.Format("<Basefont Size=6 Color=#FFFF00><Center>Free</Center></Basefont>"), false, false);
                else
                {
                    bool wildCard = false;
                    foreach (int num in ticket.WildCards)
                    {
                        if (m_Ticket.Scratch2 == num)
                        {
                            AddImage(220, yStart, num, 2);
                            wildCard = true;
                        }
                    }

                    if (!wildCard)
                    {
                        string value = String.Format("{0:##,###,###}", ticket.Scratch2);
                        AddHtml(211, yStart + 20, 80, 20, String.Format("<Basefont Size=6 Color=#FFFF00><Center>{0}</Center></Basefont>", value), false, false);
                        AddImage(220, yStart, 0x15AA, 1);
                    }
                }

                if (m_Ticket.Scratch3 == 0)
                    AddButton(359, yStart, 0x15C3, 0x15C4, 3, GumpButtonType.Reply, 0);
                else if (m_Ticket.Scratch3 == 2)
                    AddHtml(351, yStart + 20, 80, 20, String.Format("<Basefont Size=6 Color=#FFFF00><Center>Free</Center></Basefont>"), false, false);
                else
                {
                    bool wildCard = false;
                    foreach (int num in ticket.WildCards)
                    {
                        if (m_Ticket.Scratch3 == num)
                        {
                            AddImage(360, yStart, num, 2);
                            wildCard = true;
                        }
                    }

                    if (!wildCard)
                    {
                        string value = String.Format("{0:##,###,###}", ticket.Scratch3);
                        AddHtml(351, yStart + 20, 80, 20, String.Format("<Basefont Size=6 Color=#FFFF00><Center>{0}</Center></Basefont>", value), false, false);
                        AddImage(360, yStart, 0x15AA, 1);
                    }
                }
            }
            else
            {
                AddHtml(110, yStart, 80, 20, "<Basefont Size=6 Color=#FFFF00><Center>Void</Center></Basefont>", false, false);
                AddHtml(250, yStart, 80, 20, "<Basefont Size=6 Color=#FFFF00><Center>Void</Center></Basefont>", false, false);
                AddHtml(250, yStart, 80, 20, "<Basefont Size=6 Color=#FFFF00><Center>Void</Center></Basefont>", false, false);
            }
        }

        private void SkiesTheLimit()
        {
            int yStart = 90;

            if (m_Ticket.Scratch1 == 0)
                AddButton(80, yStart, 0x98B, 0x98B, 1, GumpButtonType.Reply, 0);
            else if (m_Ticket.Scratch1 == 2)
                AddHtml(60, yStart, 100, 60, String.Format("<Basefont Size=8 COLOR=#0000FF><Center>Free</Center></Basefont>"), false, false);
            else if (m_Ticket.Scratch1 == 1)
            {
                AddImage(100, yStart - 5, 0x265A);
                AddItem(98, yStart, 0xEEF);
                AddItem(108, yStart, 0xEEF);
            }
            else
            {
                string value = String.Format("{0:##,###,###}", m_Ticket.Scratch1);
                AddHtml(60, yStart, 100, 60, String.Format("<Basefont Size=8 COLOR=#0000FF><Center>{0}</Center></Basefont>", value), false, false);
            }

            if (m_Ticket.Scratch2 == 0)
                AddButton(220, yStart, 0x98B, 0x98B, 2, GumpButtonType.Reply, 0);
            else if (m_Ticket.Scratch2 == 2)
                AddHtml(200, yStart, 100, 60, String.Format("<Basefont Size=8 COLOR=#0000FF><Center>Free</Center></Basefont>"), false, false);
            else if (m_Ticket.Scratch2 == 1)
            {
                AddImage(240, yStart - 5, 0x265A);
                AddItem(238, yStart, 0xEEF);
                AddItem(248, yStart, 0xEEF);
            }
            else
            {
                string value = String.Format("{0:##,###,###}", m_Ticket.Scratch2);
                AddHtml(200, yStart, 100, 60, String.Format("<Basefont Size=8 COLOR=#0000FF><Center>{0}</Center></Basefont>", value), false, false);
            }

            if (m_Ticket.Scratch3 == 0)
                AddButton(360, yStart, 0x98B, 0x98B, 3, GumpButtonType.Reply, 0);
            else if (m_Ticket.Scratch3 == 2)
                AddHtml(340, yStart, 100, 60, String.Format("<Basefont Size=8 COLOR=#0000FF><Center>Free</Center></Basefont>"), false, false);
            else if (m_Ticket.Scratch3 == 1)
            {
                AddImage(380, yStart - 5, 0x265A);
                AddItem(378, yStart, 0xEEF);
                AddItem(388, yStart, 0xEEF);
            }
            else
            {
                string value = String.Format("{0:##,###,###}", m_Ticket.Scratch3);
                AddHtml(340, yStart, 100, 60, String.Format("<Basefont Size=8 COLOR=#0000FF><Center>{0}</Center></Basefont>", value), false, false);
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            int res = info.ButtonID;

            if (res < 4 && res > 0)
            {
                if (!m_Ticket.DoScratch(res, m_From))
                    m_From.SendMessage("Put some elbow greese in it next time!");

                m_From.PlaySound(0x249);
                m_From.SendGump(new ScratcherGump(m_Ticket, m_From));
            }
        }

        private int GetBackGround()
        {
            if (m_Ticket != null)
            {
                switch (m_Ticket.Type)
                {
                    case TicketType.CrazedCrafting: return 0x2454;
                    case TicketType.SkiesTheLimit: return 0x2486;
                    case TicketType.GoldenTicket: return 0xDAC;
                }
            }

            return 9270;
        }
    }  
}