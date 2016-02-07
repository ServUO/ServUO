using System;
using Server.Guilds;
using Server.Network;

namespace Server.Gumps
{
    public class GrantGuildTitleGump : GuildMobileListGump
    {
        public GrantGuildTitleGump(Mobile from, Guild guild)
            : base(from, guild, true, guild.Members)
        {
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (GuildGump.BadLeader(this.m_Mobile, this.m_Guild))
                return;

            if (info.ButtonID == 1)
            {
                int[] switches = info.Switches;

                if (switches.Length > 0)
                {
                    int index = switches[0];

                    if (index >= 0 && index < this.m_List.Count)
                    {
                        Mobile m = (Mobile)this.m_List[index];

                        if (m != null && !m.Deleted)
                        {
                            this.m_Mobile.SendLocalizedMessage(1013074); // New title (20 characters max):
                            this.m_Mobile.Prompt = new GuildTitlePrompt(this.m_Mobile, m, this.m_Guild);
                        }
                    }
                }
            }
            else if (info.ButtonID == 2)
            {
                GuildGump.EnsureClosed(this.m_Mobile);
                this.m_Mobile.SendGump(new GuildmasterGump(this.m_Mobile, this.m_Guild));
            }
        }

        protected override void Design()
        {
            this.AddHtmlLocalized(20, 10, 400, 35, 1011118, false, false); // Grant a title to another member.

            this.AddButton(20, 400, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 400, 245, 30, 1011127, false, false); // I dub thee...

            this.AddButton(300, 400, 4005, 4007, 2, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(335, 400, 100, 35, 1011012, false, false); // CANCEL
        }
    }
}