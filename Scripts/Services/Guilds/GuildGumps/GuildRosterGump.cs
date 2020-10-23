using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System.Collections.Generic;

namespace Server.Guilds
{
    public class GuildRosterGump : BaseGuildListGump<PlayerMobile>
    {
        #region Comparers
        private class NameComparer : IComparer<PlayerMobile>
        {
            public static readonly IComparer<PlayerMobile> Instance = new NameComparer();

            public int Compare(PlayerMobile x, PlayerMobile y)
            {
                if (x == null && y == null)
                    return 0;
                if (x == null)
                    return -1;
                if (y == null)
                    return 1;

                return Insensitive.Compare(x.Name, y.Name);
            }
        }

        private class LastOnComparer : IComparer<PlayerMobile>
        {
            public static readonly IComparer<PlayerMobile> Instance = new LastOnComparer();

            public int Compare(PlayerMobile x, PlayerMobile y)
            {
                if (x == null && y == null)
                    return 0;
                if (x == null)
                    return -1;
                if (y == null)
                    return 1;

                NetState aState = x.NetState;
                NetState bState = y.NetState;

                if (aState == null && bState == null)
                    return x.LastOnline.CompareTo(y.LastOnline);
                if (aState == null)
                    return 1;
                if (bState == null)
                    return -1;
                else
                    return 0;
            }
        }

        private class TitleComparer : IComparer<PlayerMobile>
        {
            public static readonly IComparer<PlayerMobile> Instance = new TitleComparer();

            public int Compare(PlayerMobile x, PlayerMobile y)
            {
                if (x == null && y == null)
                    return 0;
                if (x == null)
                    return -1;
                if (y == null)
                    return 1;

                return Insensitive.Compare(x.GuildTitle, y.GuildTitle);
            }
        }

        private class RankComparer : IComparer<PlayerMobile>
        {
            public static readonly IComparer<PlayerMobile> Instance = new RankComparer();

            public int Compare(PlayerMobile x, PlayerMobile y)
            {
                if (x == null && y == null)
                    return 0;
                if (x == null)
                    return -1;
                if (y == null)
                    return 1;

                return x.GuildRank.Rank.CompareTo(y.GuildRank.Rank);
            }
        }

        #endregion

        private static readonly InfoField<PlayerMobile>[] m_Fields =
        {
            new InfoField<PlayerMobile>(1062955, 130, NameComparer.Instance), //Name
            new InfoField<PlayerMobile>(1062956, 80, RankComparer.Instance), //Rank
            new InfoField<PlayerMobile>(1062952, 80, LastOnComparer.Instance), //Last On
            new InfoField<PlayerMobile>(1062953, 150, TitleComparer.Instance)//Guild Title
        };

        public GuildRosterGump(PlayerMobile pm, Guild g)
            : this(pm, g, LastOnComparer.Instance, true, "", 0)
        {
        }

        public GuildRosterGump(PlayerMobile pm, Guild g, IComparer<PlayerMobile> currentComparer, bool ascending, string filter, int startNumber)
            : base(pm, g, Utility.SafeConvertList<Mobile, PlayerMobile>(g.Members), currentComparer, ascending, filter, startNumber, m_Fields)
        {
            PopulateGump();
        }

        public override void PopulateGump()
        {
            base.PopulateGump();

            AddHtmlLocalized(266, 43, 110, 26, 1062974, 0xF, false, false); // Guild Roster
        }

        public override void DrawEndingEntry(int itemNumber)
        {
            AddBackground(225, 148 + itemNumber * 28, 150, 26, 0x2486);
            AddButton(230, 153 + itemNumber * 28, 0x845, 0x846, 8, GumpButtonType.Reply, 0);
            AddHtmlLocalized(255, 151 + itemNumber * 28, 110, 26, 1062992, 0x0, false, false); // Invite Player
        }

        protected override TextDefinition[] GetValuesFor(PlayerMobile pm, int aryLength)
        {
            TextDefinition[] defs = new TextDefinition[aryLength];

            string name = string.Format("{0} {1}{2}",
                pm.Name,
                Engines.VvV.ViceVsVirtueSystem.IsVvV(pm) ? "VvV" : "",
                (player.GuildFealty == pm && player.GuildFealty != guild.Leader) ? " *" : "");

            if (pm == player)
                name = Color(name, 0x006600);
            else if (pm.NetState != null)
                name = Color(name, 0x000066);

            defs[0] = name;
            defs[1] = pm.GuildRank.Name;
            defs[2] = (pm.NetState != null) ? new TextDefinition(1063015) : new TextDefinition(pm.LastOnline.ToString("yyyy-MM-dd"));
            defs[3] = pm.GuildTitle ?? "";

            return defs;
        }

        protected override bool IsFiltered(PlayerMobile pm, string filter)
        {
            if (pm == null)
                return true;

            return !Insensitive.Contains(pm.Name, filter);
        }

        public override Gump GetResentGump(PlayerMobile pm, Guild g, IComparer<PlayerMobile> comparer, bool ascending, string filter, int startNumber)
        {
            return new GuildRosterGump(pm, g, comparer, ascending, filter, startNumber);
        }

        public override Gump GetObjectInfoGump(PlayerMobile pm, Guild g, PlayerMobile o)
        {
            return new GuildMemberInfoGump(pm, g, o, false, false);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            base.OnResponse(sender, info);

            PlayerMobile pm = sender.Mobile as PlayerMobile;

            if (pm == null || !IsMember(pm, guild))
                return;

            if (info.ButtonID == 8)
            {
                if (pm.GuildRank.GetFlag(RankFlags.CanInvitePlayer))
                {
                    pm.SendLocalizedMessage(1063048); // Whom do you wish to invite into your guild?
                    pm.BeginTarget(-1, false, Targeting.TargetFlags.None, new TargetStateCallback(InvitePlayer_Callback), guild);
                }
                else
                    pm.SendLocalizedMessage(503301); // You don't have permission to do that.
            }
        }

        public void InvitePlayer_Callback(Mobile from, object targeted, object state)
        {
            PlayerMobile pm = from as PlayerMobile;
            PlayerMobile targ = targeted as PlayerMobile;

            Guild g = state as Guild;

            if (pm == null || !IsMember(pm, guild) || !pm.GuildRank.GetFlag(RankFlags.CanInvitePlayer))
            {
                pm.SendLocalizedMessage(503301); // You don't have permission to do that.
            }
            else if (targ == null)
            {
                pm.SendLocalizedMessage(1063334); // That isn't a valid player.
            }
            else if (!targ.AcceptGuildInvites)
            {
                pm.SendLocalizedMessage(1063049, targ.Name); // ~1_val~ is not accepting guild invitations.
            }
            else if (g.IsMember(targ))
            {
                pm.SendLocalizedMessage(1063050, targ.Name); // ~1_val~ is already a member of your guild!
            }
            else if (targ.Guild != null)
            {
                pm.SendLocalizedMessage(1063051, targ.Name); // ~1_val~ is already a member of a guild.
            }
            else if (targ.HasGump(typeof(BaseGuildGump)) || targ.HasGump(typeof(CreateGuildGump)))	//TODO: Check message if CreateGuildGump Open
            {
                pm.SendLocalizedMessage(1063052, targ.Name); // ~1_val~ is currently considering another guild invitation.
            }
            else
            {
                pm.SendLocalizedMessage(1063053, targ.Name); // You invite ~1_val~ to join your guild.
                targ.SendGump(new GuildInvitationRequest(targ, guild, pm));
            }
        }
    }
}
