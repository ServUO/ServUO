using System;
using Server.Guilds;
using Server.Network;

namespace Server.Gumps
{
    public class GuildDismissGump : GuildMobileListGump
    {
        public GuildDismissGump(Mobile from, Guild guild)
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
                            this.m_Guild.RemoveMember(m);

                            if (this.m_Mobile.AccessLevel >= AccessLevel.GameMaster || this.m_Mobile == this.m_Guild.Leader)
                            {
                                GuildGump.EnsureClosed(this.m_Mobile);
                                this.m_Mobile.SendGump(new GuildmasterGump(this.m_Mobile, this.m_Guild));
                            }
                        }
                    }
                }
            }
            else if (info.ButtonID == 2 && (this.m_Mobile.AccessLevel >= AccessLevel.GameMaster || this.m_Mobile == this.m_Guild.Leader))
            {
                GuildGump.EnsureClosed(this.m_Mobile);
                this.m_Mobile.SendGump(new GuildmasterGump(this.m_Mobile, this.m_Guild));
            }
        }

        protected override void Design()
        {
            this.AddHtmlLocalized(20, 10, 400, 35, 1011124, false, false); // Whom do you wish to dismiss?

            this.AddButton(20, 400, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 400, 245, 30, 1011125, false, false); // Kick them out!

            this.AddButton(300, 400, 4005, 4007, 2, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(335, 400, 100, 35, 1011012, false, false); // CANCEL
        }
    }
}