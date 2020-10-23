using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Guilds
{
    public class GuildInfoGump : BaseGuildGump
    {
        private readonly bool m_IsResigning;
        private readonly bool m_IsResigningVvV;

        public GuildInfoGump(PlayerMobile pm, Guild g)
            : this(pm, g, false, false)
        {
        }

        public GuildInfoGump(PlayerMobile pm, Guild g, bool isResigning, bool isResigningVvV)
            : base(pm, g)
        {
            m_IsResigning = isResigning;
            m_IsResigningVvV = isResigningVvV;
            PopulateGump();
        }

        public override void PopulateGump()
        {
            bool isLeader = IsLeader(player, guild);
            base.PopulateGump();

            AddHtmlLocalized(96, 43, 110, 26, 1063014, 0xF, false, false); // My Guild

            AddImageTiled(65, 80, 160, 26, 0xA40);
            AddImageTiled(67, 82, 156, 22, 0xBBC);
            AddHtmlLocalized(70, 83, 150, 20, 1062954, 0x0, false, false); // <i>Guild Name</i>
            AddHtml(233, 84, 320, 26, guild.Name, false, false);

            AddImageTiled(65, 114, 160, 26, 0xA40);
            AddImageTiled(67, 116, 156, 22, 0xBBC);
            AddHtmlLocalized(70, 117, 150, 20, 1063025, 0x0, false, false); // <i>Alliance</i>

            if (guild.Alliance != null && guild.Alliance.IsMember(guild))
            {
                AddHtml(233, 118, 320, 26, guild.Alliance.Name, false, false);
                AddButton(40, 120, 0x4B9, 0x4BA, 6, GumpButtonType.Reply, 0);	//Alliance Roster
            }

            AddImageTiled(65, 166, 480, 4, 0x238D);

            string s = guild.Charter;
            if (string.IsNullOrEmpty(s))
                s = "The guild leader has not yet set the guild charter.";

            AddHtml(65, 186, 480, 80, s, true, true);
            if (isLeader)
                AddButton(40, 221, 0x4B9, 0x4BA, 4, GumpButtonType.Reply, 0);	//Charter Edit button

            s = guild.Website;

            if (string.IsNullOrEmpty(s))
                s = "Guild website not yet set.";

            AddHtml(65, 276, 480, 30, s, true, false);

            if (isLeader)
                AddButton(40, 283, 0x4B9, 0x4BA, 5, GumpButtonType.Reply, 0);	//Website Edit button

            AddBackground(65, 370, 170, 26, 0x2486);

            if (Engines.VvV.ViceVsVirtueSystem.Enabled)
            {
                if (Engines.VvV.ViceVsVirtueSystem.IsVvV(player))
                {
                    AddButton(67, 375, 0x4B9, 0x4BA, 9, GumpButtonType.Reply, 0); // Resign Vice vs Virtue
                    AddHtmlLocalized(92, 373, 170, 26, 1155557, m_IsResigningVvV ? 0x5000 : 0, false, false);

                    AddBackground(255, 370, 170, 26, 0x2486);
                    AddButton(257, 375, 0x4B9, 0x4BA, 10, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(282, 373, 150, 26, 1114982, false, false); // Leaderboards
                }
                else
                {
                    AddButton(67, 375, 0x4B9, 0x4BA, 8, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(92, 373, 170, 26, 1155556, false, false); // Join Vice vs Virtue
                }
            }

            AddBackground(445, 370, 100, 26, 0x2486);
            AddButton(447, 375, 0x845, 0x846, 7, GumpButtonType.Reply, 0);
            AddHtmlLocalized(472, 373, 60, 26, 3006115, (m_IsResigning) ? 0x5000 : 0, false, false); // Resign
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            base.OnResponse(sender, info);

            PlayerMobile pm = sender.Mobile as PlayerMobile;

            if (!IsMember(pm, guild))
                return;

            switch (info.ButtonID)
            {
                //1-3 handled by base.OnResponse
                case 4:
                    {
                        if (IsLeader(pm, guild))
                        {
                            pm.SendLocalizedMessage(1013071); // Enter the new guild charter (50 characters max):

                            pm.BeginPrompt(SetCharter_Callback, true);	//Have the same callback handle both canceling and deletion cause the 2nd callback would just get a text of ""
                        }
                        break;
                    }
                case 5:
                    {
                        if (IsLeader(pm, guild))
                        {
                            pm.SendLocalizedMessage(1013072); // Enter the new website for the guild (50 characters max):
                            pm.BeginPrompt(SetWebsite_Callback, true);	//Have the same callback handle both canceling and deletion cause the 2nd callback would just get a text of ""
                        }
                        break;
                    }
                case 6:
                    {
                        //Alliance Roster
                        if (guild.Alliance != null && guild.Alliance.IsMember(guild))
                            pm.SendGump(new AllianceInfo.AllianceRosterGump(pm, guild, guild.Alliance));

                        break;
                    }
                case 7:
                    {
                        //Resign
                        if (!m_IsResigning)
                        {
                            pm.SendLocalizedMessage(1063332); // Are you sure you wish to resign from your guild?
                            pm.SendGump(new GuildInfoGump(pm, guild, true, false));
                        }
                        else
                        {
                            guild.RemoveMember(pm, 1063411); // You resign from your guild.
                        }
                        break;
                    }
                case 8:
                    if (pm.Young)
                    {
                        pm.SendLocalizedMessage(1155562); // Young players may not join Vice vs Virtue. Renounce your young player status by saying, "I renounce my young player status" and try again.
                    }
                    else
                    {
                        pm.SendGump(new Engines.VvV.ConfirmSignupGump(pm));
                    }
                    break;
                case 9:
                    if (Engines.Points.PointsSystem.ViceVsVirtue.IsResigning(pm, guild))
                    {
                        pm.SendLocalizedMessage(1155560); // You are currently in the process of quitting Vice vs Virtue.
                    }
                    else if (m_IsResigningVvV)
                    {
                        pm.SendLocalizedMessage(1155559); // You have begun the Vice vs Virtue resignation process.  You will be removed from VvV in 3 days.
                        Engines.Points.PointsSystem.ViceVsVirtue.OnResign(pm);
                    }
                    else
                    {
                        pm.SendLocalizedMessage(1155558); // Are you sure you wish to resign from Vice vs Virtue? You will not be allowed to rejoin for 3 days.
                        pm.SendGump(new GuildInfoGump(pm, guild, false, true));
                    }
                    break;
                case 10:
                    pm.SendGump(new Engines.VvV.ViceVsVirtueLeaderboardGump(pm));
                    break;
            }
        }

        public void SetCharter_Callback(Mobile from, string text)
        {
            if (!IsLeader(from, guild))
                return;

            string charter = Utility.FixHtml(text.Trim());

            if (charter.Length > 50)
            {
                from.SendLocalizedMessage(1070774, "50"); // Your guild charter cannot exceed ~1_val~ characters.
            }
            else
            {
                guild.Charter = charter;
                from.SendLocalizedMessage(1070775); // You submit a new guild charter.
            }
        }

        public void SetWebsite_Callback(Mobile from, string text)
        {
            if (!IsLeader(from, guild))
                return;

            string site = Utility.FixHtml(text.Trim());

            if (site.Length > 50)
                from.SendLocalizedMessage(1070777, "50"); // Your guild website cannot exceed ~1_val~ characters.
            else
            {
                guild.Website = site;
                from.SendLocalizedMessage(1070778); // You submit a new guild website.
            }
        }
    }
}
