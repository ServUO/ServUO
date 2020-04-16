using Server.Multis;
using Server.Network;

namespace Server.Gumps
{
    public class ShipSecurityGump : BaseShipGump
    {
        private readonly Mobile m_From;
        private readonly BaseGalleon m_Galleon;
        private readonly SecurityEntry m_Entry;

        public ShipSecurityGump(Mobile from, BaseGalleon galleon)
            : base(galleon)
        {
            from.CloseGump(typeof(ShipSecurityGump));

            m_From = from;
            m_Galleon = galleon;
            m_Entry = galleon.SecurityEntry;

            if (m_Entry == null)
            {
                m_Entry = new SecurityEntry(m_Galleon);
                m_Galleon.SecurityEntry = m_Entry;
            }

            PartyAccess pa = m_Entry.PartyAccess;

            AddHtmlLocalized(10, 79, 300, 18, 1149743, LabelColor, false, false); // Party membership modifies access to this ship:

            if (pa == PartyAccess.Never)
            {
                AddImage(55, 97, 0xFA6);
            }
            else
            {
                AddButton(55, 97, 0xFA5, 0xFA7, 1001, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(90, 99, 210, 18, 1149778, LabelColor, false, false); // Never

            if (pa == PartyAccess.LeaderOnly)
            {
                AddImage(55, 115, 0xFA6);
            }
            else
            {
                AddButton(55, 115, 0xFA5, 0xFA7, 1002, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(90, 117, 210, 18, 1149744, LabelColor, false, false); // When I am Party Leader

            if (pa == PartyAccess.MemberOnly)
            {
                AddImage(55, 133, 0xFA6);
            }
            else
            {
                AddButton(55, 133, 0xFA5, 0xFA7, 1003, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(90, 135, 210, 18, 1149745, LabelColor, false, false); // When I am a Party Member

            AddHtmlLocalized(10, 158, 125, 18, 1149731, LabelColor, false, false); // Public Access:
            AddButton(140, 156, 0xFA5, 0xFA7, 0, GumpButtonType.Page, 2);
            AddHtmlLocalized(175, 158, 125, 18, GetLevel(m_Entry.DefaultPublicAccess), GetHue(m_Entry.DefaultPublicAccess), false, false);

            AddHtmlLocalized(10, 175, 150, 18, 1149732, LabelColor, false, false); // Party Access:
            AddButton(140, 173, 0xFA5, 0xFA7, 0, GumpButtonType.Page, 3);
            AddHtmlLocalized(175, 175, 125, 18, GetLevel(m_Entry.DefaultPartyAccess), GetHue(m_Entry.DefaultPartyAccess), false, false);

            AddHtmlLocalized(10, 193, 150, 18, 1149733, LabelColor, false, false); // Guild Access:
            AddButton(140, 191, 0xFA5, 0xFA7, 0, GumpButtonType.Page, 4);
            AddHtmlLocalized(175, 193, 125, 18, GetLevel(m_Entry.DefaultGuildAccess), GetHue(m_Entry.DefaultGuildAccess), false, false);

            AddHtmlLocalized(195, 357, 100, 18, 1149734, LabelColor, false, false); // ACCESS LIST
            AddButton(160, 355, 0xFA5, 0xFA7, 2000, GumpButtonType.Reply, 0);

            AddPage(2);

            AddBackground(30, 215, 190, 130, 0xA3C);
            AddHtmlLocalized(80, 220, 100, 18, 1149731, LabelColor, false, false); // Public Access:

            if (m_Entry.DefaultPublicAccess == SecurityLevel.NA)
            {
                AddImage(40, 243, 0xFA6);
            }
            else
            {
                AddButton(40, 243, 0xFA5, 0xFA7, 1100, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(75, 245, 140, 18, 1149725, m_Entry.DefaultPublicAccess == SecurityLevel.NA ? NAHue : LabelColor, false, false); // N/A

            if (m_Entry.DefaultPublicAccess == SecurityLevel.Passenger)
            {
                AddImage(40, 261, 0xFA6);
            }
            else
            {
                AddButton(40, 261, 0xFA5, 0xFA7, 1102, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(75, 263, 140, 18, 1149727, m_Entry.DefaultPublicAccess == SecurityLevel.Passenger ? PassengerHue : LabelColor, false, false); // PASSENGER

            if (m_Entry.DefaultPublicAccess == SecurityLevel.Crewman)
            {
                AddImage(40, 279, 0xFA6);
            }
            else
            {
                AddButton(40, 279, 0xFA5, 0xFA7, 1103, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(75, 281, 140, 18, 1149728, m_Entry.DefaultPublicAccess == SecurityLevel.Crewman ? CrewHue : LabelColor, false, false); // CREW

            if (m_Entry.DefaultPublicAccess == SecurityLevel.Officer)
            {
                AddImage(40, 297, 0xFA6);
            }
            else
            {
                AddButton(40, 297, 0xFA5, 0xFA7, 1104, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(75, 299, 140, 18, 1149729, m_Entry.DefaultPublicAccess == SecurityLevel.Officer ? OfficerHue : LabelColor, false, false); // OFFICER

            if (m_Entry.DefaultPublicAccess == SecurityLevel.Denied)
            {
                AddImage(40, 315, 0xFA6);
            }
            else
            {
                AddButton(40, 315, 0xFA5, 0xFA7, 1101, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(75, 317, 140, 18, 1149726, m_Entry.DefaultPublicAccess == SecurityLevel.Denied ? DenyHue : LabelColor, false, false); // DENY ACCESS

            AddPage(3);

            AddBackground(70, 215, 190, 130, 0xA3C);
            AddHtmlLocalized(120, 220, 100, 18, 1149732, LabelColor, false, false); // Party Access:

            if (m_Entry.DefaultPartyAccess == SecurityLevel.NA)
            {
                AddImage(80, 243, 0xFA6);
            }
            else
            {
                AddButton(80, 243, 0xFA5, 0xFA7, 1200, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(115, 245, 140, 18, 1149725, m_Entry.DefaultPartyAccess == SecurityLevel.NA ? NAHue : LabelColor, false, false); // N/A

            if (m_Entry.DefaultPartyAccess == SecurityLevel.Passenger)
            {
                AddImage(80, 261, 0xFA6);
            }
            else
            {
                AddButton(80, 261, 0xFA5, 0xFA7, 1202, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(115, 263, 140, 18, 1149727, m_Entry.DefaultPartyAccess == SecurityLevel.Passenger ? PassengerHue : LabelColor, false, false); // PASSENGER

            if (m_Entry.DefaultPartyAccess == SecurityLevel.Crewman)
            {
                AddImage(80, 279, 0xFA6);
            }
            else
            {
                AddButton(80, 279, 0xFA5, 0xFA7, 1203, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(115, 281, 140, 18, 1149728, m_Entry.DefaultPartyAccess == SecurityLevel.Crewman ? CrewHue : LabelColor, false, false); // CREW

            if (m_Entry.DefaultPartyAccess == SecurityLevel.Officer)
            {
                AddImage(80, 297, 0xFA6);
            }
            else
            {
                AddButton(80, 297, 0xFA5, 0xFA7, 1204, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(115, 299, 140, 18, 1149729, m_Entry.DefaultPartyAccess == SecurityLevel.Officer ? OfficerHue : LabelColor, false, false); // OFFICER

            if (m_Entry.DefaultPartyAccess == SecurityLevel.Denied)
            {
                AddImage(80, 315, 0xFA6);
            }
            else
            {
                AddButton(80, 315, 0xFA5, 0xFA7, 1201, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(115, 317, 140, 18, 1149726, m_Entry.DefaultPartyAccess == SecurityLevel.Denied ? DenyHue : LabelColor, false, false); // DENY ACCESS

            AddPage(4);

            AddBackground(110, 215, 190, 130, 0xA3C);
            AddHtmlLocalized(160, 220, 100, 18, 1149733, LabelColor, false, false); // Guild Access:

            if (m_Entry.DefaultGuildAccess == SecurityLevel.NA)
            {
                AddImage(120, 243, 0xFA6);
            }
            else
            {
                AddButton(120, 243, 0xFA5, 0xFA7, 1300, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(155, 245, 140, 18, 1149725, m_Entry.DefaultGuildAccess == SecurityLevel.NA ? NAHue : LabelColor, false, false); // N/A

            if (m_Entry.DefaultGuildAccess == SecurityLevel.Passenger)
            {
                AddImage(120, 261, 0xFA6);
            }
            else
            {
                AddButton(120, 261, 0xFA5, 0xFA7, 1302, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(155, 263, 140, 18, 1149727, m_Entry.DefaultGuildAccess == SecurityLevel.Passenger ? PassengerHue : LabelColor, false, false); // PASSENGER

            if (m_Entry.DefaultGuildAccess == SecurityLevel.Crewman)
            {
                AddImage(120, 279, 0xFA6);
            }
            else
            {
                AddButton(120, 279, 0xFA5, 0xFA7, 1303, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(155, 281, 140, 18, 1149728, m_Entry.DefaultGuildAccess == SecurityLevel.Crewman ? CrewHue : LabelColor, false, false); // CREW

            if (m_Entry.DefaultGuildAccess == SecurityLevel.Officer)
            {
                AddImage(120, 297, 0xFA6);
            }
            else
            {
                AddButton(120, 297, 0xFA5, 0xFA7, 1304, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(155, 299, 140, 18, 1149729, m_Entry.DefaultGuildAccess == SecurityLevel.Officer ? OfficerHue : LabelColor, false, false); // OFFICER

            if (m_Entry.DefaultGuildAccess == SecurityLevel.Denied)
            {
                AddImage(120, 315, 0xFA6);
            }
            else
            {
                AddButton(120, 315, 0xFA5, 0xFA7, 1301, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(155, 317, 140, 18, 1149726, m_Entry.DefaultGuildAccess == SecurityLevel.Denied ? DenyHue : LabelColor, false, false); // DENY ACCESS
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0: return;
                case 1001: //party access...Never
                    m_Entry.PartyAccess = PartyAccess.Never;
                    m_From.SendGump(new ShipSecurityGump(m_From, m_Galleon));
                    break;
                case 1002: //party access...leaderonly
                    m_Entry.PartyAccess = PartyAccess.LeaderOnly;
                    m_From.SendGump(new ShipSecurityGump(m_From, m_Galleon));
                    break;
                case 1003: //party access...member only
                    m_Entry.PartyAccess = PartyAccess.MemberOnly;
                    m_From.SendGump(new ShipSecurityGump(m_From, m_Galleon));
                    break;
                case 2000: //Access List
                    m_From.SendGump(new AccessListGump(m_From, m_Galleon));
                    return;
                default:
                    {
                        if (info.ButtonID >= 1100 && info.ButtonID <= 1104)
                        {
                            m_Entry.DefaultPublicAccess = (SecurityLevel)(info.ButtonID - 1100);
                        }
                        else if (info.ButtonID >= 1200 && info.ButtonID <= 1204)
                        {
                            m_Entry.DefaultPartyAccess = (SecurityLevel)(info.ButtonID - 1200);
                        }
                        else if (info.ButtonID >= 1300 && info.ButtonID <= 1304)
                        {
                            m_Entry.DefaultGuildAccess = (SecurityLevel)(info.ButtonID - 1300);
                        }

                        m_From.SendGump(new ShipSecurityGump(m_From, m_Galleon));

                        return;
                    }
            }
        }
    }
}
