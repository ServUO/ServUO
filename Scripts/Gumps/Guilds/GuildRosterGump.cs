using System;
using Server.Guilds;
using Server.Network;

namespace Server.Gumps
{
    public class GuildRosterGump : GuildMobileListGump
    {
        public GuildRosterGump(Mobile from, Guild guild)
            : base(from, guild, false, guild.Members)
        {
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

        protected override void Design()
        {
            this.AddHtml(20, 10, 500, 35, String.Format("<center>{0}</center>", this.m_Guild.Name), false, false);

            this.AddButton(20, 400, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 400, 300, 35, 1011120, false, false); // Return to the main menu.
        }
    }
}