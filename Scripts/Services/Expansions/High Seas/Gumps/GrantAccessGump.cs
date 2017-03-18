using System;
using Server;
using Server.Items;
using Server.Multis;
using Server.Network;

namespace Server.Gumps
{
    public class GrantAccessGump : BaseShipGump
    {
        private Mobile m_Player;
        private BaseGalleon m_Galleon;
        private SecurityEntry m_Entry;

        public GrantAccessGump(Mobile player, BaseGalleon galleon) : base(50, 50, galleon)
        {
            player.CloseGump(typeof(GrantAccessGump));

            m_Player = player;
            m_Galleon = galleon;
            m_Entry = galleon.SecurityEntry;

            if (m_Entry == null)
            {
                m_Entry = new SecurityEntry(m_Galleon);
                m_Galleon.SecurityEntry = m_Entry;
            }

            bool isAccessed = m_Entry.Manifest.ContainsKey(player);
            bool inGuild = m_Entry.IsInGuild(player);
            bool inParty = m_Entry.IsInParty(player);
            bool isPublic = m_Entry.IsPublic;
            SecurityLevel level = m_Entry.GetEffectiveLevel(player);

            //Player Info
            AddHtmlLocalized(10, 85, 100, 20, 1149763, LabelColor, false, false); //Player: 
            AddHtml(90, 85, 100, 20, Color(player.Name, "DarkCyan"), false, false);

            AddHtmlLocalized(10, 105, 100, 20, 1149768, LabelColor, false, false); //Effective Level:
            AddHtmlLocalized(150, 105, 100, 20, GetLevel(level), GetHue(level), false, false);

            //Default Info
            int cliloc = isPublic ? 1149756 : 1149757;
            int hue = isPublic ? CrewHue : NoHue;

            AddHtmlLocalized(10, 130, 100, 20, 1149731, LabelColor, false, false);      //Public Access:
            AddHtmlLocalized(150, 130, 50, 20, cliloc, hue, false, false);  //Yes/No

            if (isPublic)
                AddHtmlLocalized(230, 130, 100, 20, GetLevel(m_Entry.DefaultPublicAccess), GetHue(m_Entry.DefaultPublicAccess), false, false);

            cliloc = inParty ? 1149756 : 1149757;
            hue = inParty ? CrewHue : NoHue;

            AddHtmlLocalized(10, 150, 200, 20, 1149769, LabelColor, false, false);  //Is Party Member:
            AddHtmlLocalized(150, 150, 50, 20, cliloc, hue, false, false);

            if (inParty)
                AddHtmlLocalized(230, 150, 100, 20, GetLevel(m_Entry.DefaultPartyAccess), GetHue(m_Entry.DefaultPartyAccess), false, false);

            cliloc = inGuild ? 1149756 : 1149757;
            hue = inGuild ? CrewHue : NoHue;

            AddHtmlLocalized(10, 170, 200, 20, 1149770, LabelColor, false, false);   //Is Guild Member
            AddHtmlLocalized(150, 170, 50, 20, cliloc, hue, false, false);

            if (inGuild)
                AddHtmlLocalized(230, 170, 100, 20, GetLevel(m_Entry.DefaultGuildAccess), GetHue(m_Entry.DefaultGuildAccess), false, false);

            AddHtmlLocalized(10, 195, 300, 20, 1149747, LabelColor, false, false);  //Access list status:

            AddButton(65, 215, isAccessed ? 4006 : 4005, isAccessed ? 4006 : 4007, 1, GumpButtonType.Reply, 0);
            if (!isAccessed)
                AddHtmlLocalized(100, 215, 200, 20, 1149775, NoHue, false, false); //Not in access list
            else
                AddHtmlLocalized(100, 215, 200, 20, 1149776, NoHue, false, false); //Remove from access list

            AddButton(65, 235, level == SecurityLevel.Denied ? 4006 : 4005, level == SecurityLevel.Denied ? 4006 : 4007, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(100, 235, 200, 20, 1149726, LabelColor, false, false);  //Deny access

            AddButton(65, 255, level == SecurityLevel.Passenger ? 4006 : 4005, level == SecurityLevel.Passenger ? 4006 : 4007, 3, GumpButtonType.Reply, 0);
            AddHtmlLocalized(100, 255, 200, 20, 1149727, level == SecurityLevel.Passenger ? GetHue(level) : LabelColor, false, false); //Passenger

            AddButton(65, 275, level == SecurityLevel.Crewman ? 4006 : 4005, level == SecurityLevel.Passenger ? 4006 : 4007, 4, GumpButtonType.Reply, 0);
            AddHtmlLocalized(100, 275, 200, 20, 1149728, level == SecurityLevel.Crewman ? GetHue(level) : LabelColor, false, false);  //Crew

            AddButton(65, 295, level == SecurityLevel.Officer ? 4006 : 4005, level == SecurityLevel.Passenger ? 4006 : 4007, 5, GumpButtonType.Reply, 0);
            AddHtmlLocalized(100, 295, 200, 20, 1149729, level == SecurityLevel.Officer ? GetHue(level) : LabelColor, false, false);

            AddButton(65, 315, level == SecurityLevel.Captain ? 4006 : 4005, level == SecurityLevel.Captain ? 4006 : 4007, 6, GumpButtonType.Reply, 0);
            AddHtmlLocalized(100, 315, 200, 20, 1149730, level == SecurityLevel.Captain ? GetHue(level) : LabelColor, false, false);

            //BOtton of screen
            AddButton(10, 370, 4005, 4007, 7, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 370, 100, 20, 1149777, LabelColor,false, false);  //MAIN MENU

            AddButton(195, 370, 4005, 4007, 8, GumpButtonType.Reply, 0);
            AddHtmlLocalized(230, 370, 100, 20, 1149734, LabelColor,false, false);  //ACCESS LIST

        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 0: return;
                case 1: //Not In Access List
                    if (m_Entry.Manifest.ContainsKey(m_Player))
                        m_Entry.RemoveFromAccessList(m_Player);
                    //else
                    //    m_Entry.AddToManifest(m_Player, SecurityLevel.Passenger);
                    break;
                case 2: //Deny Access
                    m_Entry.AddToManifest(m_Player, SecurityLevel.Denied);
                    break;
                case 3: //Passenger
                    m_Entry.AddToManifest(m_Player, SecurityLevel.Passenger);
                    break;
                case 4: //crewman
                    m_Entry.AddToManifest(m_Player, SecurityLevel.Crewman);
                    break;
                case 5: //Officer
                    m_Entry.AddToManifest(m_Player, SecurityLevel.Officer);
                    break;
                case 6: //Captain	
                    m_Entry.AddToManifest(m_Player, SecurityLevel.Captain);
                    break;
                case 7: //Main menu
                    from.SendGump(new ShipSecurityGump(from, m_Galleon));
                    return;
                case 8: //Access List
                    from.SendGump(new AccessListGump(m_Player, m_Galleon));
                    return;
            }

            from.SendGump(new GrantAccessGump(m_Player, m_Galleon));
        }
    }
}