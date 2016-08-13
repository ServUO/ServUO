using System;
using Server;
using Server.Items;
using Server.Multis;
using Server.Network;

namespace Server.Gumps
{
    public class ShipSecurityGump : BaseShipGump
    {
        private Mobile m_From;
        private BaseGalleon m_Galleon;
        private SecurityEntry m_Entry;

        public ShipSecurityGump(Mobile from, BaseGalleon galleon)
            : base(50, 50, galleon)
        {
            from.CloseGump(typeof(ShipSecurityGump));

            m_From = from;
            m_Galleon = galleon;
            m_Entry = galleon.SecurityEntry;

            if (m_Entry == null)
                m_Galleon.SecurityEntry = new SecurityEntry(m_Galleon);

            PartyAccess pa = m_Entry.PartyAccess;

            AddHtmlLocalized(10, 80, 330, 20, 1149743, LabelColor, false, false); //Party membership modifies access to this ship:

            //Party defaults
            AddButton(60, 100, pa == PartyAccess.Never ? 4006 : 4005, pa == PartyAccess.Never ? 4006 : 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(95, 100, 300, 20, 1149778, LabelColor, false, false); //Never

            AddButton(60, 120, pa == PartyAccess.LeaderOnly ? 4006 : 4005, pa == PartyAccess.LeaderOnly ? 4006 : 4007, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(95, 120, 300, 20, 1149744, LabelColor, false, false); //When I am Party Leader

            AddButton(60, 140, pa == PartyAccess.MemberOnly ? 4006 : 4005, pa == PartyAccess.MemberOnly ? 4006 : 4007, 3, GumpButtonType.Reply, 0);
            AddHtmlLocalized(95, 140, 300, 20, 1149745, LabelColor, false, false); //When I am a Party Member

            //Default security access
            AddHtmlLocalized(10, 165, 100, 20, 1149731, LabelColor, false, false); //Public Access:
            AddButton(140, 165, 4005, 4007, 4, GumpButtonType.Reply, 0);
            AddHtmlLocalized(175, 165, 100, 20, GetLevel(m_Entry.DefaultPublicAccess), GetHue(m_Entry.DefaultPublicAccess), false, false);

            AddHtmlLocalized(10, 185, 100, 20, 1149732, LabelColor, false, false); //Party Access:
            AddButton(140, 185, 4005, 4007, 5, GumpButtonType.Reply, 0);
            AddHtmlLocalized(175, 185, 100, 20, GetLevel(m_Entry.DefaultPartyAccess), GetHue(m_Entry.DefaultPartyAccess), false, false);

            AddHtmlLocalized(10, 205, 100, 20, 1149733, LabelColor, false, false); //Guild Access:
            AddButton(140, 205, 4005, 4007, 6, GumpButtonType.Reply, 0);
            AddHtmlLocalized(175, 205, 100, 20, GetLevel(m_Entry.DefaultGuildAccess), GetHue(m_Entry.DefaultGuildAccess), false, false);

            AddButton(195, 370, 4005, 4007, 7, GumpButtonType.Reply, 0);
            AddHtmlLocalized(230, 370, 100, 20, 1149734, LabelColor, false, false);  //ACCESS LIST
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            int num = 0;

            switch (info.ButtonID)
            {
                case 0: return;
                case 1: //party access...Never
                    m_Entry.PartyAccess = PartyAccess.Never;
                    break;
                case 2: //party access...leaderonly
                    m_Entry.PartyAccess = PartyAccess.LeaderOnly;
                    break;
                case 3: //party access...member only
                    m_Entry.PartyAccess = PartyAccess.MemberOnly;
                    break;
                case 4: //public default level
                    num = (int)m_Entry.DefaultPublicAccess;
                    num++;
                    if (num > (int)SecurityLevel.Captain)
                        num = (int)SecurityLevel.Denied;
                    m_Entry.DefaultPublicAccess = (SecurityLevel)num;
                    break;
                case 5: //party default level
                    num = (int)m_Entry.DefaultPartyAccess;
                    num++;
                    if (num > (int)SecurityLevel.Captain)
                        num = (int)SecurityLevel.Denied;
                    m_Entry.DefaultPartyAccess = (SecurityLevel)num;
                    break;
                case 6: //guild default level
                    num = (int)m_Entry.DefaultGuildAccess;
                    num++;
                    if (num > (int)SecurityLevel.Captain)
                        num = (int)SecurityLevel.Denied;
                    m_Entry.DefaultGuildAccess = (SecurityLevel)num;
                    break;
                case 7: //Access List
                    m_From.SendGump(new AccessListGump(m_From, m_Galleon));
                    return;
            }

            m_From.SendGump(new ShipSecurityGump(m_From, m_Galleon));
        }
    }
}