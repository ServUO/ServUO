using System;
using System.Collections.Generic;
using Server.Factions;
using Server.Guilds;
using Server.Network;

namespace Server.Gumps
{
    public class GuildDeclareWarGump : GuildListGump
    {
        public GuildDeclareWarGump(Mobile from, Guild guild, List<Guild> list)
            : base(from, guild, true, list)
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
                        Guild g = this.m_List[index];

                        if (g != null)
                        {
                            if (g == this.m_Guild)
                            {
                                this.m_Mobile.SendLocalizedMessage(501184); // You cannot declare war against yourself!
                            }
                            else if ((g.WarInvitations.Contains(this.m_Guild) && this.m_Guild.WarDeclarations.Contains(g)) || this.m_Guild.IsWar(g))
                            {
                                this.m_Mobile.SendLocalizedMessage(501183); // You are already at war with that guild.
                            }
                            else if (Faction.Find(this.m_Guild.Leader) != null)
                            {
                                this.m_Mobile.SendLocalizedMessage(1005288); // You cannot declare war while you are in a faction
                            }
                            else
                            {
                                if (!this.m_Guild.WarDeclarations.Contains(g))
                                {
                                    this.m_Guild.WarDeclarations.Add(g);
                                    this.m_Guild.GuildMessage(1018019, true, "{0} ({1})", g.Name, g.Abbreviation); // Guild Message: Your guild has sent an invitation for war:
                                }

                                if (!g.WarInvitations.Contains(this.m_Guild))
                                {
                                    g.WarInvitations.Add(this.m_Guild);
                                    g.GuildMessage(1018021, true, "{0} ({1})", this.m_Guild.Name, this.m_Guild.Abbreviation); // Guild Message: Your guild has received an invitation to war:
                                }
                            }

                            GuildGump.EnsureClosed(this.m_Mobile);
                            this.m_Mobile.SendGump(new GuildWarAdminGump(this.m_Mobile, this.m_Guild));
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
            this.AddHtmlLocalized(20, 10, 400, 35, 1011065, false, false); // Select the guild you wish to declare war on.

            this.AddButton(20, 400, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 400, 245, 30, 1011068, false, false); // Send the challenge!

            this.AddButton(300, 400, 4005, 4007, 2, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(335, 400, 100, 35, 1011012, false, false); // CANCEL
        }
    }
}