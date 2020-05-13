using Server.Multis;
using Server.Network;

namespace Server.Gumps
{
    public class GrantAccessGump : BaseShipGump
    {
        private readonly Mobile m_Player;
        private readonly BaseGalleon m_Galleon;
        private readonly SecurityEntry m_Entry;

        public GrantAccessGump(Mobile player, BaseGalleon galleon)
            : base(galleon)
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
            AddHtmlLocalized(10, 79, 125, 18, 1149763, LabelColor, false, false); // Player: 
            AddLabel(140, 79, 0x30, player.Name);

            AddHtmlLocalized(10, 97, 125, 18, 1149768, LabelColor, false, false); // Effective Level:
            AddHtmlLocalized(140, 97, 160, 18, GetLevel(level), GetHue(level), false, false);

            //Default Info
            int cliloc = isPublic ? 1149756 : 1149757;
            int hue = isPublic ? CrewHue : NoHue;

            AddHtmlLocalized(10, 120, 125, 18, 1149731, LabelColor, false, false); // Public Access:
            AddHtmlLocalized(140, 120, 50, 18, cliloc, hue, false, false); // Yes/No

            if (isPublic)
                AddHtmlLocalized(200, 120, 100, 18, GetLevel(m_Entry.DefaultPublicAccess), GetHue(m_Entry.DefaultPublicAccess), false, false);

            cliloc = inParty ? 1149756 : 1149757;
            hue = inParty ? CrewHue : NoHue;

            AddHtmlLocalized(10, 138, 125, 18, 1149769, LabelColor, false, false); // Is Party Member:
            AddHtmlLocalized(140, 138, 50, 18, cliloc, hue, false, false);

            if (inParty)
                AddHtmlLocalized(200, 138, 50, 18, GetLevel(m_Entry.DefaultPartyAccess), GetHue(m_Entry.DefaultPartyAccess), false, false);

            cliloc = inGuild ? 1149756 : 1149757;
            hue = inGuild ? CrewHue : NoHue;

            AddHtmlLocalized(10, 156, 125, 18, 1149770, LabelColor, false, false); // Is Guild Member
            AddHtmlLocalized(140, 156, 50, 18, cliloc, hue, false, false);

            if (inGuild)
                AddHtmlLocalized(200, 156, 50, 18, GetLevel(m_Entry.DefaultGuildAccess), GetHue(m_Entry.DefaultGuildAccess), false, false);

            AddHtmlLocalized(10, 179, 300, 18, 1149747, LabelColor, false, false); // Access List Status:

            if (level == SecurityLevel.NA)
            {
                AddImage(65, 197, 0xFA6);
                AddHtmlLocalized(100, 199, 200, 18, 1149775, NoHue, false, false); // NOT IN ACCESS LIST
            }
            else
            {
                AddButton(65, 197, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(100, 199, 200, 18, 1149776, NoHue, false, false); // REMOVE FROM LIST
            }

            if (level == SecurityLevel.Denied)
            {
                AddImage(65, 215, 0xFA6);
            }
            else
            {
                AddButton(65, 215, 0xFA5, 0xFA7, 2, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(100, 217, 100, 18, 1149726, level == SecurityLevel.Denied ? GetHue(level) : LabelColor, false, false); // DENY ACCESS

            if (level == SecurityLevel.Passenger)
            {
                AddImage(65, 233, 0xFA6);
            }
            else
            {
                AddButton(65, 233, 0xFA5, 0xFA7, 3, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(100, 235, 100, 18, 1149727, level == SecurityLevel.Passenger ? GetHue(level) : LabelColor, false, false); // PASSENGER

            if (level == SecurityLevel.Crewman)
            {
                AddImage(65, 251, 0xFA6);
            }
            else
            {
                AddButton(65, 251, 0xFA5, 0xFA7, 4, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(100, 253, 100, 18, 1149728, level == SecurityLevel.Crewman ? GetHue(level) : LabelColor, false, false); // CREW

            if (level == SecurityLevel.Officer)
            {
                AddImage(65, 269, 0xFA6);
            }
            else
            {
                AddButton(65, 269, 0xFA5, 0xFA7, 5, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(100, 271, 100, 18, 1149729, level == SecurityLevel.Officer ? GetHue(level) : LabelColor, false, false); // OFFICER

            if (level == SecurityLevel.Captain)
            {
                AddImage(65, 287, 0xFA6);
            }
            else
            {
                AddButton(65, 287, 0xFA5, 0xFA7, 6, GumpButtonType.Reply, 0);
            }

            AddHtmlLocalized(100, 289, 100, 18, 1149730, level == SecurityLevel.Captain ? GetHue(level) : LabelColor, false, false); // CAPTAIN

            AddButton(10, 355, 0xFA5, 0xFA7, 7, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 357, 100, 18, 1149777, LabelColor, false, false); // MAIN MENU

            AddButton(160, 355, 0xFA5, 0xFA, 8, GumpButtonType.Reply, 0);
            AddHtmlLocalized(195, 357, 100, 18, 1149734, LabelColor, false, false); // ACCESS LIST
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null)
                return;

            switch (info.ButtonID)
            {
                case 0: return;
                case 1: // REMOVE FROM LIST
                    m_Entry.RemoveFromAccessList(m_Player);
                    from.SendGump(new AccessListGump(m_Player, m_Galleon));
                    return;
                case 2: // DENY ACCESS
                    m_Entry.AddToManifest(m_Player, SecurityLevel.Denied);
                    break;
                case 3: // PASSENGER
                    m_Entry.AddToManifest(m_Player, SecurityLevel.Passenger);
                    break;
                case 4: // CREW
                    m_Entry.AddToManifest(m_Player, SecurityLevel.Crewman);
                    break;
                case 5: // OFFICER
                    m_Entry.AddToManifest(m_Player, SecurityLevel.Officer);
                    break;
                case 6: // CAPTAIN	
                    m_Entry.AddToManifest(m_Player, SecurityLevel.Captain);
                    break;
                case 7: // MAIN MENU
                    from.SendGump(new ShipSecurityGump(from, m_Galleon));
                    return;
                case 8: // ACCESS LIST
                    from.SendGump(new AccessListGump(m_Player, m_Galleon));
                    return;
            }

            from.SendGump(new GrantAccessGump(m_Player, m_Galleon));
        }
    }
}
