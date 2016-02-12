using System;
using Server.Guilds;
using Server.Prompts;

namespace Server.Gumps
{
    public class GuildTitlePrompt : Prompt
    {
        // Enter the new title for this guild member or 'none' to remove a title:
        public override int MessageCliloc { get { return 1011128; } }
        private readonly Mobile m_Leader;
        private readonly Mobile m_Target;
        private readonly Guild m_Guild;
        public GuildTitlePrompt(Mobile leader, Mobile target, Guild g)
        {
            this.m_Leader = leader;
            this.m_Target = target;
            this.m_Guild = g;
        }

        public override void OnCancel(Mobile from)
        {
            if (GuildGump.BadLeader(this.m_Leader, this.m_Guild))
                return;
            else if (this.m_Target.Deleted || !this.m_Guild.IsMember(this.m_Target))
                return;

            GuildGump.EnsureClosed(this.m_Leader);
            this.m_Leader.SendGump(new GuildmasterGump(this.m_Leader, this.m_Guild));
        }

        public override void OnResponse(Mobile from, string text)
        {
            if (GuildGump.BadLeader(this.m_Leader, this.m_Guild))
                return;
            else if (this.m_Target.Deleted || !this.m_Guild.IsMember(this.m_Target))
                return;

            text = text.Trim();

            if (text.Length > 20)
                text = text.Substring(0, 20);

            if (text.Length > 0)
                this.m_Target.GuildTitle = text;

            GuildGump.EnsureClosed(this.m_Leader);
            this.m_Leader.SendGump(new GuildmasterGump(this.m_Leader, this.m_Guild));
        }
    }
}