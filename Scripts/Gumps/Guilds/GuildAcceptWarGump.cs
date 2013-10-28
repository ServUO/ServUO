using System;
using Server.Guilds;
using Server.Network;

namespace Server.Gumps
{
    public class GuildAcceptWarGump : GuildListGump
    {
        public GuildAcceptWarGump(Mobile from, Guild guild)
            : base(from, guild, true, guild.WarInvitations)
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
                        Guild g = (Guild)this.m_List[index];

                        if (g != null)
                        {
                            this.m_Guild.WarInvitations.Remove(g);
                            g.WarDeclarations.Remove(this.m_Guild);

                            this.m_Guild.AddEnemy(g);
                            this.m_Guild.GuildMessage(1018020, true, "{0} ({1})", g.Name, g.Abbreviation);

                            GuildGump.EnsureClosed(this.m_Mobile);

                            if (this.m_Guild.WarInvitations.Count > 0)
                                this.m_Mobile.SendGump(new GuildAcceptWarGump(this.m_Mobile, this.m_Guild));
                            else
                                this.m_Mobile.SendGump(new GuildmasterGump(this.m_Mobile, this.m_Guild));
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
            this.AddHtmlLocalized(20, 10, 400, 35, 1011147, false, false); // Select the guild to accept the invitations: 

            this.AddButton(20, 400, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 400, 245, 30, 1011100, false, false);  // Accept war invitations.

            this.AddButton(300, 400, 4005, 4007, 2, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(335, 400, 100, 35, 1011012, false, false); // CANCEL
        }
    }
}