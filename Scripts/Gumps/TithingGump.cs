using Server.Mobiles;
using Server.Network;
using System.Globalization;

namespace Server.Gumps
{
    public class TithingGump : Gump
    {
        private readonly int MaxTithing = 100000;

        public TithingGump(Mobile from, int offer)
            : base(100, 100)
        {
            int totalGold = Banker.GetBalance(from);

            string gold = totalGold >= MaxTithing ? "100,000+" : totalGold.ToString("N0", CultureInfo.GetCultureInfo("en-US"));

            AddPage(0);

            AddBackground(0, 0, 250, 355, 0x6DB);
            AddImage(8, 0, 0x66);

            AddHtmlLocalized(73, 65, 100, 150, 1114513, "#1060198", 0xC63, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>

            AddHtmlLocalized(85, 245, 50, 18, 3000311, 0xC63, false, false); // Gold:
            AddLabel(117, 245, 0x35, gold);

            AddHtmlLocalized(43, 275, 200, 18, 1159137, 0x7FFF, false, false); // How much whilst thou tithe?
            AddBackground(85, 295, 80, 22, 0x2486);
            AddTextEntry(87, 297, 76, 18, 0x0, 1, offer > 0 ? string.Format("{0}", offer) : "", 6);

            AddButton(205, 321, 0xFB7, 0xFB8, 2, GumpButtonType.Reply, 0); // OK

            AddButton(15, 326, 0x845, 0x846, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(35, 323, 200, 18, 1159139, 0x7FFF, false, false); // Tithe Maximum            
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            int m_Offer;

            switch (info.ButtonID)
            {
                case 0:
                    {
                        // You have decided to tithe no gold to the shrine.
                        from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1060193);
                        break;
                    }
                case 1:
                    {
                        if (from.TithingPoints >= MaxTithing)
                        {
                            from.SendLocalizedMessage(1060840); // You have reached the maximum amount of Tithing Points available.
                            break;
                        }

                        int totalGold = Banker.GetBalance(from);

                        if (totalGold <= 0)
                        {
                            // You have decided to tithe no gold to the shrine.
                            from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1060193);
                            break;
                        }

                        m_Offer = MaxTithing - from.TithingPoints;

                        if (m_Offer > totalGold)
                            m_Offer = totalGold;

                        from.SendGump(new TithingGump(from, m_Offer));

                        break;
                    }
                case 2:
                    {
                        if (from.TithingPoints >= MaxTithing)
                        {
                            from.SendLocalizedMessage(1060840); // You have reached the maximum amount of Tithing Points available.
                            break;
                        }

                        int totalGold = Banker.GetBalance(from);

                        if (totalGold <= 0)
                        {
                            // You have decided to tithe no gold to the shrine.
                            from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1060193);
                            break;
                        }

                        TextRelay tr = info.GetTextEntry(1);

                        if (tr != null)
                        {
                            string text = tr.Text;
                            int num = Utility.ToInt32(text);

                            if (num > 0)
                            {
                                if (num > totalGold)
                                    m_Offer = totalGold;
                                else
                                    m_Offer = num;

                                if ((from.TithingPoints + m_Offer) > MaxTithing)
                                    m_Offer = MaxTithing - from.TithingPoints;

                                if (Banker.Withdraw(from, m_Offer, true))
                                {
                                    // You tithe gold to the shrine as a sign of devotion.
                                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1060195);
                                    from.TithingPoints += m_Offer;

                                    from.PlaySound(0x243);
                                    from.PlaySound(0x2E6);
                                }
                                else
                                {
                                    // You do not have enough gold to tithe that amount!
                                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1060194);
                                }
                            }
                            else
                            {
                                from.SendLocalizedMessage(1159140); // You have entered an invalid entry. You must enter a number between 1 and 100,000.
                            }
                        }

                        break;
                    }
            }
        }
    }
}
