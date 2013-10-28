using System;
using Server.Items;
using Server.Network;

namespace Server.Gumps
{
    public class TithingGump : Gump
    {
        private readonly Mobile m_From;
        private int m_Offer;
        public TithingGump(Mobile from, int offer)
            : base(160, 40)
        {
            int totalGold = from.TotalGold;

            if (offer > totalGold)
                offer = totalGold;
            else if (offer < 0)
                offer = 0;

            this.m_From = from;
            this.m_Offer = offer;

            this.AddPage(0);

            this.AddImage(30, 30, 102);

            this.AddHtmlLocalized(95, 100, 120, 100, 1060198, 0, false, false); // May your wealth bring blessings to those in need, if tithed upon this most sacred site.

            this.AddLabel(57, 274, 0, "Gold:");
            this.AddLabel(87, 274, 53, (totalGold - offer).ToString());

            this.AddLabel(137, 274, 0, "Tithe:");
            this.AddLabel(172, 274, 53, offer.ToString());

            this.AddButton(105, 230, 5220, 5220, 2, GumpButtonType.Reply, 0);
            this.AddButton(113, 230, 5222, 5222, 2, GumpButtonType.Reply, 0);
            this.AddLabel(108, 228, 0, "<");
            this.AddLabel(112, 228, 0, "<");

            this.AddButton(127, 230, 5223, 5223, 1, GumpButtonType.Reply, 0);
            this.AddLabel(131, 228, 0, "<");

            this.AddButton(147, 230, 5224, 5224, 3, GumpButtonType.Reply, 0);
            this.AddLabel(153, 228, 0, ">");

            this.AddButton(168, 230, 5220, 5220, 4, GumpButtonType.Reply, 0);
            this.AddButton(176, 230, 5222, 5222, 4, GumpButtonType.Reply, 0);
            this.AddLabel(172, 228, 0, ">");
            this.AddLabel(176, 228, 0, ">");

            this.AddButton(217, 272, 4023, 4024, 5, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch ( info.ButtonID )
            {
                case 0:
                    {
                        // You have decided to tithe no gold to the shrine.
                        this.m_From.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1060193);
                        break;
                    }
                case 1:
                case 2:
                case 3:
                case 4:
                    {
                        int offer = 0;

                        switch ( info.ButtonID )
                        {
                            case 1:
                                offer = this.m_Offer - 100;
                                break;
                            case 2:
                                offer = 0;
                                break;
                            case 3:
                                offer = this.m_Offer + 100;
                                break;
                            case 4:
                                offer = this.m_From.TotalGold;
                                break;
                        }

                        this.m_From.SendGump(new TithingGump(this.m_From, offer));
                        break;
                    }
                case 5:
                    {
                        int totalGold = this.m_From.TotalGold;

                        if (this.m_Offer > totalGold)
                            this.m_Offer = totalGold;
                        else if (this.m_Offer < 0)
                            this.m_Offer = 0;

                        if ((this.m_From.TithingPoints + this.m_Offer) > 100000) // TODO: What's the maximum?
                            this.m_Offer = (100000 - this.m_From.TithingPoints);

                        if (this.m_Offer <= 0)
                        {
                            // You have decided to tithe no gold to the shrine.
                            this.m_From.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1060193);
                            break;
                        }

                        Container pack = this.m_From.Backpack;

                        if (pack != null && pack.ConsumeTotal(typeof(Gold), this.m_Offer))
                        {
                            // You tithe gold to the shrine as a sign of devotion.
                            this.m_From.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1060195);
                            this.m_From.TithingPoints += this.m_Offer;

                            this.m_From.PlaySound(0x243);
                            this.m_From.PlaySound(0x2E6);
                        }
                        else
                        {
                            // You do not have enough gold to tithe that amount!
                            this.m_From.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1060194);
                        }

                        break;
                    }
            }
        }
    }
}