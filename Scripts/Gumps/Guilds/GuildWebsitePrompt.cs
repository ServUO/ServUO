using System;
using Server.Guilds;
using Server.Prompts;

namespace Server.Gumps
{
    public class GuildWebsitePrompt : Prompt
    {
        public override int MessageCliloc { get { return 1013072; } }
        private readonly Mobile m_Mobile;
        private readonly Guild m_Guild;
        public GuildWebsitePrompt(Mobile m, Guild g)
        {
            this.m_Mobile = m;
            this.m_Guild = g;
        }

        public override void OnCancel(Mobile from)
        {
            if (GuildGump.BadLeader(this.m_Mobile, this.m_Guild))
                return;

            GuildGump.EnsureClosed(this.m_Mobile);
            this.m_Mobile.SendGump(new GuildmasterGump(this.m_Mobile, this.m_Guild));
        }

        public override void OnResponse(Mobile from, string text)
        {
            if (GuildGump.BadLeader(this.m_Mobile, this.m_Guild))
                return;

            text = text.Trim();

            if (text.Length > 50)
                text = text.Substring(0, 50);

            if (text.Length > 0)
                this.m_Guild.Website = text;

            GuildGump.EnsureClosed(this.m_Mobile);
            this.m_Mobile.SendGump(new GuildmasterGump(this.m_Mobile, this.m_Guild));
        }
    }
}