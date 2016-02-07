using System;
using Server.Guilds;
using Server.Prompts;

namespace Server.Gumps
{
    public class GuildNamePrompt : Prompt
    {
        // Enter new guild name (40 characters max):
        public override int MessageCliloc { get { return 1013060; } }
        private readonly Mobile m_Mobile;
        private readonly Guild m_Guild;
        public GuildNamePrompt(Mobile m, Guild g)
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

            if (text.Length > 40)
                text = text.Substring(0, 40);

            if (text.Length > 0)
            {
                if (Guild.FindByName(text) != null)
                {
                    this.m_Mobile.SendMessage("{0} conflicts with the name of an existing guild.", text);
                }
                else
                {
                    this.m_Guild.Name = text;
                    this.m_Guild.GuildMessage(1018024, true, text); // The name of your guild has changed:
                }
            }

            GuildGump.EnsureClosed(this.m_Mobile);
            this.m_Mobile.SendGump(new GuildmasterGump(this.m_Mobile, this.m_Guild));
        }
    }
}