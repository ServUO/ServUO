using System;
using Server.Guilds;
using Server.Network;

namespace Server.Gumps
{
    public class GuildDeclarePeaceGump : GuildListGump
    {
        public GuildDeclarePeaceGump(Mobile from, Guild guild)
            : base(from, guild, true, guild.Enemies)
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
                            this.m_Guild.RemoveEnemy(g);
                            this.m_Guild.GuildMessage(1018018, true, "{0} ({1})", g.Name, g.Abbreviation); // Guild Message: You are now at peace with this guild:

                            GuildGump.EnsureClosed(this.m_Mobile);

                            if (this.m_Guild.Enemies.Count > 0)
                                this.m_Mobile.SendGump(new GuildDeclarePeaceGump(this.m_Mobile, this.m_Guild));
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
            this.AddHtmlLocalized(20, 10, 400, 35, 1011137, false, false); // Select the guild you wish to declare peace with.

            this.AddButton(20, 400, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 400, 245, 30, 1011138, false, false); // Send the olive branch.

            this.AddButton(300, 400, 4005, 4007, 2, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(335, 400, 100, 35, 1011012, false, false); // CANCEL
        }
    }
}