using System;
using System.Collections.Generic;
using Server.Guilds;
using Server.Network;

namespace Server.Gumps
{
    public class GuildWarGump : Gump
    {
        private readonly Mobile m_Mobile;
        private readonly Guild m_Guild;
        public GuildWarGump(Mobile from, Guild guild)
            : base(20, 30)
        {
            this.m_Mobile = from;
            this.m_Guild = guild;

            this.Dragable = false;

            this.AddPage(0);
            this.AddBackground(0, 0, 550, 440, 5054);
            this.AddBackground(10, 10, 530, 420, 3000);

            this.AddHtmlLocalized(20, 10, 500, 35, 1011133, false, false); // <center>WARFARE STATUS</center>

            this.AddButton(20, 400, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 400, 300, 35, 1011120, false, false); // Return to the main menu.

            this.AddPage(1);

            this.AddButton(375, 375, 5224, 5224, 0, GumpButtonType.Page, 2);
            this.AddHtmlLocalized(410, 373, 100, 25, 1011066, false, false); // Next page

            this.AddHtmlLocalized(20, 45, 400, 20, 1011134, false, false); // We are at war with:

            List<Guild> enemies = guild.Enemies;

            if (enemies.Count == 0)
            {
                this.AddHtmlLocalized(20, 65, 400, 20, 1013033, false, false); // No current wars
            }
            else
            {
                for (int i = 0; i < enemies.Count; ++i)
                {
                    Guild g = enemies[i];

                    this.AddHtml(20, 65 + (i * 20), 300, 20, g.Name, false, false);
                }
            }

            this.AddPage(2);

            this.AddButton(375, 375, 5224, 5224, 0, GumpButtonType.Page, 3);
            this.AddHtmlLocalized(410, 373, 100, 25, 1011066, false, false); // Next page

            this.AddButton(30, 375, 5223, 5223, 0, GumpButtonType.Page, 1);
            this.AddHtmlLocalized(65, 373, 150, 25, 1011067, false, false); // Previous page

            this.AddHtmlLocalized(20, 45, 400, 20, 1011136, false, false); // Guilds that we have declared war on: 

            List<Guild> declared = guild.WarDeclarations;

            if (declared.Count == 0)
            {
                this.AddHtmlLocalized(20, 65, 400, 20, 1018012, false, false); // No current invitations received for war.
            }
            else
            {
                for (int i = 0; i < declared.Count; ++i)
                {
                    Guild g = (Guild)declared[i];

                    this.AddHtml(20, 65 + (i * 20), 300, 20, g.Name, false, false);
                }
            }

            this.AddPage(3);

            this.AddButton(30, 375, 5223, 5223, 0, GumpButtonType.Page, 2);
            this.AddHtmlLocalized(65, 373, 150, 25, 1011067, false, false); // Previous page

            this.AddHtmlLocalized(20, 45, 400, 20, 1011135, false, false); // Guilds that have declared war on us: 

            List<Guild> invites = guild.WarInvitations;

            if (invites.Count == 0)
            {
                this.AddHtmlLocalized(20, 65, 400, 20, 1013055, false, false); // No current war declarations
            }
            else
            {
                for (int i = 0; i < invites.Count; ++i)
                {
                    Guild g = invites[i];

                    this.AddHtml(20, 65 + (i * 20), 300, 20, g.Name, false, false);
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (GuildGump.BadMember(this.m_Mobile, this.m_Guild))
                return;

            if (info.ButtonID == 1)
            {
                GuildGump.EnsureClosed(this.m_Mobile);
                this.m_Mobile.SendGump(new GuildGump(this.m_Mobile, this.m_Guild));
            }
        }
    }
}