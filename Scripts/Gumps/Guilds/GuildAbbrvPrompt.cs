using System;
using Server.Guilds;
using Server.Prompts;

namespace Server.Gumps
{
    public class GuildAbbrvPrompt : Prompt
    {
        // Enter new guild abbreviation (3 characters max):
        public override int MessageCliloc { get { return 1013061; } }
        private readonly Mobile m_Mobile;
        private readonly Guild m_Guild;
        public GuildAbbrvPrompt(Mobile m, Guild g)
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

            if (text.Length > 3)
                text = text.Substring(0, 3);

            if (text.Length > 0)
            {
                if (Guild.FindByAbbrev(text) != null)
                {
                    this.m_Mobile.SendMessage("{0} conflicts with the abbreviation of an existing guild.", text);
                }
                else
                {
                    this.m_Guild.Abbreviation = text;
                    this.m_Guild.GuildMessage(1018025, true, text); // Your guild abbreviation has changed:
                }
            }

            GuildGump.EnsureClosed(this.m_Mobile);
            this.m_Mobile.SendGump(new GuildmasterGump(this.m_Mobile, this.m_Guild));
        }
    }
}