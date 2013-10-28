using System;
using System.Collections.Generic;
using Server.Factions;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Guilds
{
    public class GuildRosterGump : BaseGuildListGump<PlayerMobile>
    {
        #region Comparers
        private class NameComparer : IComparer<PlayerMobile>
        {
            public static readonly IComparer<PlayerMobile> Instance = new NameComparer();

            public NameComparer()
            {
            }

            public int Compare(PlayerMobile x, PlayerMobile y)
            {
                if (x == null && y == null)
                    return 0;
                else if (x == null)
                    return -1;
                else if (y == null)
                    return 1;

                return Insensitive.Compare(x.Name, y.Name);
            }
        }

        private class LastOnComparer : IComparer<PlayerMobile>
        {
            public static readonly IComparer<PlayerMobile> Instance = new LastOnComparer();

            public LastOnComparer()
            {
            }

            public int Compare(PlayerMobile x, PlayerMobile y)
            {
                if (x == null && y == null)
                    return 0;
                else if (x == null)
                    return -1;
                else if (y == null)
                    return 1;

                NetState aState = x.NetState;
                NetState bState = y.NetState;

                if (aState == null && bState == null)
                    return x.LastOnline.CompareTo(y.LastOnline);
                else if (aState == null)
                    return 1;
                else if (bState == null)
                    return -1;
                else
                    return 0;
            }
        }

        private class TitleComparer : IComparer<PlayerMobile>
        {
            public static readonly IComparer<PlayerMobile> Instance = new TitleComparer();

            public TitleComparer()
            {
            }

            public int Compare(PlayerMobile x, PlayerMobile y)
            {
                if (x == null && y == null)
                    return 0;
                else if (x == null)
                    return -1;
                else if (y == null)
                    return 1;

                return Insensitive.Compare(x.GuildTitle, y.GuildTitle);
            }
        }

        private class RankComparer : IComparer<PlayerMobile>
        {
            public static readonly IComparer<PlayerMobile> Instance = new RankComparer();

            public RankComparer()
            {
            }

            public int Compare(PlayerMobile x, PlayerMobile y)
            {
                if (x == null && y == null)
                    return 0;
                else if (x == null)
                    return -1;
                else if (y == null)
                    return 1;

                return x.GuildRank.Rank.CompareTo(y.GuildRank.Rank);
            }
        }

        #endregion

        private static readonly InfoField<PlayerMobile>[] m_Fields = new InfoField<PlayerMobile>[]
        {
            new InfoField<PlayerMobile>(1062955, 130, GuildRosterGump.NameComparer.Instance), //Name
            new InfoField<PlayerMobile>(1062956, 80, GuildRosterGump.RankComparer.Instance), //Rank
            new InfoField<PlayerMobile>(1062952, 80, GuildRosterGump.LastOnComparer.Instance), //Last On
            new InfoField<PlayerMobile>(1062953, 150, GuildRosterGump.TitleComparer.Instance)//Guild Title
        };

        public GuildRosterGump(PlayerMobile pm, Guild g)
            : this(pm, g, GuildRosterGump.LastOnComparer.Instance, true, "", 0)
        {
        }

        public GuildRosterGump(PlayerMobile pm, Guild g, IComparer<PlayerMobile> currentComparer, bool ascending, string filter, int startNumber)
            : base(pm, g, Utility.SafeConvertList<Mobile, PlayerMobile>(g.Members), currentComparer, ascending, filter, startNumber, m_Fields)
        {
            this.PopulateGump();
        }

        public override void PopulateGump()
        {
            base.PopulateGump();

            this.AddHtmlLocalized(266, 43, 110, 26, 1062974, 0xF, false, false); // Guild Roster
        }

        public override void DrawEndingEntry(int itemNumber)
        {
            this.AddBackground(225, 148 + itemNumber * 28, 150, 26, 0x2486);
            this.AddButton(230, 153 + itemNumber * 28, 0x845, 0x846, 8, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(255, 151 + itemNumber * 28, 110, 26, 1062992, 0x0, false, false); // Invite Player
        }

        protected override TextDefinition[] GetValuesFor(PlayerMobile pm, int aryLength)
        {
            TextDefinition[] defs = new TextDefinition[aryLength];

            string name = String.Format("{0}{1}", pm.Name, (this.player.GuildFealty == pm && this.player.GuildFealty != this.guild.Leader) ? " *" : "");

            if (pm == this.player)
                name = Color(name, 0x006600);
            else if (pm.NetState != null)
                name = Color(name, 0x000066);

            defs[0] = name;
            defs[1] = pm.GuildRank.Name;
            defs[2] = (pm.NetState != null) ? new TextDefinition(1063015) : new TextDefinition(pm.LastOnline.ToString("yyyy-MM-dd")); 
            defs[3] = (pm.GuildTitle == null) ? "" : pm.GuildTitle;

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

            if (pm == null || !IsMember(pm, this.guild))
                return;

            if (info.ButtonID == 8)
            {
                if (pm.GuildRank.GetFlag(RankFlags.CanInvitePlayer))
                {
                    pm.SendLocalizedMessage(1063048); // Whom do you wish to invite into your guild?
                    pm.BeginTarget(-1, false, Targeting.TargetFlags.None, new TargetStateCallback(InvitePlayer_Callback), this.guild);
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

            PlayerState guildState = PlayerState.Find(g.Leader);
            PlayerState targetState = PlayerState.Find(targ);

            Faction guildFaction = (guildState == null ? null : guildState.Faction);
            Faction targetFaction = (targetState == null ? null : targetState.Faction);

            if (pm == null || !IsMember(pm, this.guild) || !pm.GuildRank.GetFlag(RankFlags.CanInvitePlayer))
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
            #region Factions
            else if (targ.Young && guildFaction != null)
            {
                pm.SendLocalizedMessage(1070766); // You cannot invite a young player to your faction-aligned guild.
            }
            else if (guildFaction != targetFaction)
            {
                if (guildFaction == null)
                    pm.SendLocalizedMessage(1013027); // That player cannot join a non-faction guild.
                else if (targetFaction == null)
                    pm.SendLocalizedMessage(1013026); // That player must be in a faction before joining this guild.
                else
                    pm.SendLocalizedMessage(1013028); // That person has a different faction affiliation.
            }
            else if (targetState != null && targetState.IsLeaving)
            {
                // OSI does this quite strangely, so we'll just do it this way
                pm.SendMessage("That person is quitting their faction and so you may not recruit them.");
            }
            #endregion
            else
            {
                pm.SendLocalizedMessage(1063053, targ.Name); // You invite ~1_val~ to join your guild.
                targ.SendGump(new GuildInvitationRequest(targ, this.guild, pm));
            }
        }
    }
}