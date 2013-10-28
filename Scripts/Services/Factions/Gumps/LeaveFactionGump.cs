using System;
using Server.Guilds;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Factions
{
    public class LeaveFactionGump : FactionGump
    {
        private readonly PlayerMobile m_From;
        private readonly Faction m_Faction;
        public LeaveFactionGump(PlayerMobile from, Faction faction)
            : base(20, 30)
        {
            this.m_From = from;
            this.m_Faction = faction;

            this.AddBackground(0, 0, 270, 120, 5054);
            this.AddBackground(10, 10, 250, 100, 3000);

            if (from.Guild is Guild && ((Guild)from.Guild).Leader == from)
                this.AddHtmlLocalized(20, 15, 230, 60, 1018057, true, true); // Are you sure you want your entire guild to leave this faction?
            else
                this.AddHtmlLocalized(20, 15, 230, 60, 1018063, true, true); // Are you sure you want to leave this faction?

            this.AddHtmlLocalized(55, 80, 75, 20, 1011011, false, false); // CONTINUE
            this.AddButton(20, 80, 4005, 4007, 1, GumpButtonType.Reply, 0);

            this.AddHtmlLocalized(170, 80, 75, 20, 1011012, false, false); // CANCEL
            this.AddButton(135, 80, 4005, 4007, 2, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch ( info.ButtonID )
            {
                case 1: // continue
                    {
                        Guild guild = this.m_From.Guild as Guild;

                        if (guild == null)
                        {
                            PlayerState pl = PlayerState.Find(this.m_From);

                            if (pl != null)
                            {
                                pl.Leaving = DateTime.UtcNow;

                                if (Faction.LeavePeriod == TimeSpan.FromDays(3.0))
                                    this.m_From.SendLocalizedMessage(1005065); // You will be removed from the faction in 3 days
                                else
                                    this.m_From.SendMessage("You will be removed from the faction in {0} days.", Faction.LeavePeriod.TotalDays);
                            }
                        }
                        else if (guild.Leader != this.m_From)
                        {
                            this.m_From.SendLocalizedMessage(1005061); // You cannot quit the faction because you are not the guild master
                        }
                        else
                        {
                            this.m_From.SendLocalizedMessage(1042285); // Your guild is now quitting the faction.

                            for (int i = 0; i < guild.Members.Count; ++i)
                            {
                                Mobile mob = (Mobile)guild.Members[i];
                                PlayerState pl = PlayerState.Find(mob);

                                if (pl != null)
                                {
                                    pl.Leaving = DateTime.UtcNow;

                                    if (Faction.LeavePeriod == TimeSpan.FromDays(3.0))
                                        mob.SendLocalizedMessage(1005060); // Your guild will quit the faction in 3 days
                                    else
                                        mob.SendMessage("Your guild will quit the faction in {0} days.", Faction.LeavePeriod.TotalDays);
                                }
                            }
                        }

                        break;
                    }
                case 2: // cancel
                    {
                        this.m_From.SendLocalizedMessage(500737); // Canceled resignation.
                        break;
                    }
            }
        }
    }
}