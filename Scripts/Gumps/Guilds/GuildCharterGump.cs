using System;
using Server.Guilds;
using Server.Network;

namespace Server.Gumps
{
    public class GuildCharterGump : Gump
    {
        private const string DefaultWebsite = "http://www.runuo.com/";
        private readonly Mobile m_Mobile;
        private readonly Guild m_Guild;
        public GuildCharterGump(Mobile from, Guild guild)
            : base(20, 30)
        {
            this.m_Mobile = from;
            this.m_Guild = guild;

            this.Dragable = false;

            this.AddPage(0);
            this.AddBackground(0, 0, 550, 400, 5054);
            this.AddBackground(10, 10, 530, 380, 3000);

            this.AddButton(20, 360, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 360, 300, 35, 1011120, false, false); // Return to the main menu.

            string charter;

            if ((charter = guild.Charter) == null || (charter = charter.Trim()).Length <= 0)
                this.AddHtmlLocalized(20, 20, 400, 35, 1013032, false, false); // No charter has been defined.
            else
                this.AddHtml(20, 20, 510, 75, charter, true, true);

            this.AddButton(20, 200, 4005, 4007, 2, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 200, 300, 20, 1011122, false, false); // Visit the guild website : 

            string website;

            if ((website = guild.Website) == null || (website = website.Trim()).Length <= 0)
                website = DefaultWebsite;

            this.AddHtml(55, 220, 300, 20, website, false, false);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (GuildGump.BadMember(this.m_Mobile, this.m_Guild))
                return;

            switch ( info.ButtonID )
            {
                case 0:
                    return; // Close
                case 1:
                    break; // Return to main menu
                case 2:
                    {
                        string website;

                        if ((website = this.m_Guild.Website) == null || (website = website.Trim()).Length <= 0)
                            website = DefaultWebsite;

                        this.m_Mobile.LaunchBrowser(website);
                        break;
                    }
            }

            GuildGump.EnsureClosed(this.m_Mobile);
            this.m_Mobile.SendGump(new GuildGump(this.m_Mobile, this.m_Guild));
        }
    }
}