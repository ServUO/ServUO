using System;
using System.Collections.Generic;
using Server.Guilds;
using Server.Prompts;

namespace Server.Gumps
{
    public class GuildDeclareWarPrompt : Prompt
    {
        public override int MessageCliloc { get { return 1018001; } }
        private readonly Mobile m_Mobile;
        private readonly Guild m_Guild;
        public GuildDeclareWarPrompt(Mobile m, Guild g)
        {
            this.m_Mobile = m;
            this.m_Guild = g;
        }

        public override void OnCancel(Mobile from)
        {
            if (GuildGump.BadLeader(this.m_Mobile, this.m_Guild))
                return;

            GuildGump.EnsureClosed(this.m_Mobile);
            this.m_Mobile.SendGump(new GuildWarAdminGump(this.m_Mobile, this.m_Guild));
        }

        public override void OnResponse(Mobile from, string text)
        {
            if (GuildGump.BadLeader(this.m_Mobile, this.m_Guild))
                return;

            text = text.Trim();

            if (text.Length >= 3)
            {
                List<Guild> guilds = Utility.CastConvertList<BaseGuild, Guild>(Guild.Search(text));

                GuildGump.EnsureClosed(this.m_Mobile);

                if (guilds.Count > 0)
                {
                    this.m_Mobile.SendGump(new GuildDeclareWarGump(this.m_Mobile, this.m_Guild, guilds));
                }
                else
                {
                    this.m_Mobile.SendGump(new GuildWarAdminGump(this.m_Mobile, this.m_Guild));
                    this.m_Mobile.SendLocalizedMessage(1018003); // No guilds found matching - try another name in the search
                }
            }
            else
            {
                this.m_Mobile.SendMessage("Search string must be at least three letters in length.");
            }
        }
    }
}