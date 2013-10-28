using System;
using Server.Guilds;
using Server.Network;

namespace Server.Gumps
{
    public class DeclareFealtyGump : GuildMobileListGump
    {
        public DeclareFealtyGump(Mobile from, Guild guild)
            : base(from, guild, true, guild.Members)
        {
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (GuildGump.BadMember(this.m_Mobile, this.m_Guild))
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
                            state.Mobile.GuildFealty = m;
                        }
                    }
                }
            }

            GuildGump.EnsureClosed(this.m_Mobile);
            this.m_Mobile.SendGump(new GuildGump(this.m_Mobile, this.m_Guild));
        }

        protected override void Design()
        {
            this.AddHtmlLocalized(20, 10, 400, 35, 1011097, false, false); // Declare your fealty

            this.AddButton(20, 400, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 400, 250, 35, 1011098, false, false); // I have selected my new lord.

            this.AddButton(300, 400, 4005, 4007, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(335, 400, 100, 35, 1011012, false, false); // CANCEL
        }
    }
}