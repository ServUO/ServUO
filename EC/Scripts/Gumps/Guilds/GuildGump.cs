using System;
using Server.Guilds;
using Server.Network;

namespace Server.Gumps
{
    public class GuildGump : Gump
    {
        private readonly Mobile m_Mobile;
        private readonly Guild m_Guild;
        public GuildGump(Mobile beholder, Guild guild)
            : base(20, 30)
        {
            this.m_Mobile = beholder;
            this.m_Guild = guild;

            this.Dragable = false;

            this.AddPage(0);
            this.AddBackground(0, 0, 550, 400, 5054);
            this.AddBackground(10, 10, 530, 380, 3000);

            this.AddHtml(20, 15, 200, 35, guild.Name, false, false);

            Mobile leader = guild.Leader;

            if (leader != null)
            {
                string leadTitle;

                if ((leadTitle = leader.GuildTitle) != null && (leadTitle = leadTitle.Trim()).Length > 0)
                    leadTitle += ": ";
                else
                    leadTitle = "";

                string leadName;

                if ((leadName = leader.Name) == null || (leadName = leadName.Trim()).Length <= 0)
                    leadName = "(empty)";

                this.AddHtml(220, 15, 250, 35, leadTitle + leadName, false, false);
            }

            this.AddButton(20, 50, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 50, 100, 20, 1013022, false, false); // Loyal to

            Mobile fealty = beholder.GuildFealty;

            if (fealty == null || !guild.IsMember(fealty))
                fealty = leader;

            if (fealty == null)
                fealty = beholder;

            string fealtyName;

            if (fealty == null || (fealtyName = fealty.Name) == null || (fealtyName = fealtyName.Trim()).Length <= 0)
                fealtyName = "(empty)";

            if (beholder == fealty)
                this.AddHtmlLocalized(55, 70, 470, 20, 1018002, false, false); // yourself
            else
                this.AddHtml(55, 70, 470, 20, fealtyName, false, false);

            this.AddButton(215, 50, 4005, 4007, 2, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(250, 50, 170, 20, 1013023, false, false); // Display guild abbreviation
            this.AddHtmlLocalized(250, 70, 50, 20, beholder.DisplayGuildTitle ? 1011262 : 1011263, false, false); // on/off

            this.AddButton(20, 100, 4005, 4007, 3, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 100, 470, 30, 1011086, false, false); // View the current roster.

            this.AddButton(20, 130, 4005, 4007, 4, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 130, 470, 30, 1011085, false, false); // Recruit someone into the guild.

            if (guild.Candidates.Count > 0)
            {
                this.AddButton(20, 160, 4005, 4007, 5, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(55, 160, 470, 30, 1011093, false, false); // View list of candidates who have been sponsored to the guild.
            }
            else
            {
                this.AddImage(20, 160, 4020);
                this.AddHtmlLocalized(55, 160, 470, 30, 1013031, false, false); // There are currently no candidates for membership.
            }

            this.AddButton(20, 220, 4005, 4007, 6, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 220, 470, 30, 1011087, false, false); // View the guild's charter.

            this.AddButton(20, 250, 4005, 4007, 7, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 250, 470, 30, 1011092, false, false); // Resign from the guild.

            this.AddButton(20, 280, 4005, 4007, 8, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 280, 470, 30, 1011095, false, false); // View list of guilds you are at war with.

            if (beholder.AccessLevel >= AccessLevel.GameMaster || beholder == leader)
            {
                this.AddButton(20, 310, 4005, 4007, 9, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(55, 310, 470, 30, 1011094, false, false); // Access guildmaster functions.
            }
            else
            {
                this.AddImage(20, 310, 4020);
                this.AddHtmlLocalized(55, 310, 470, 30, 1018013, false, false); // Reserved for guildmaster
            }

            this.AddButton(20, 360, 4005, 4007, 0, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 360, 470, 30, 1011441, false, false); // EXIT
        }

        public static void EnsureClosed(Mobile m)
        {
            m.CloseGump(typeof(DeclareFealtyGump));
            m.CloseGump(typeof(GrantGuildTitleGump));
            m.CloseGump(typeof(GuildAdminCandidatesGump));
            m.CloseGump(typeof(GuildCandidatesGump));
            m.CloseGump(typeof(GuildChangeTypeGump));
            m.CloseGump(typeof(GuildCharterGump));
            m.CloseGump(typeof(GuildDismissGump));
            m.CloseGump(typeof(GuildGump));
            m.CloseGump(typeof(GuildmasterGump));
            m.CloseGump(typeof(GuildRosterGump));
            m.CloseGump(typeof(GuildWarGump));
        }

        public static bool BadLeader(Mobile m, Guild g)
        {
            if (m.Deleted || g.Disbanded || (m.AccessLevel < AccessLevel.GameMaster && g.Leader != m))
                return true;

            Item stone = g.Guildstone;

            return (stone == null || stone.Deleted || !m.InRange(stone.GetWorldLocation(), 2));
        }

        public static bool BadMember(Mobile m, Guild g)
        {
            if (m.Deleted || g.Disbanded || (m.AccessLevel < AccessLevel.GameMaster && !g.IsMember(m)))
                return true;

            Item stone = g.Guildstone;

            return (stone == null || stone.Deleted || !m.InRange(stone.GetWorldLocation(), 2));
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (BadMember(this.m_Mobile, this.m_Guild))
                return;

            switch ( info.ButtonID )
            {
                case 1: // Loyalty
                    {
                        EnsureClosed(this.m_Mobile);
                        this.m_Mobile.SendGump(new DeclareFealtyGump(this.m_Mobile, this.m_Guild));

                        break;
                    }
                case 2: // Toggle display abbreviation
                    {
                        this.m_Mobile.DisplayGuildTitle = !this.m_Mobile.DisplayGuildTitle;

                        EnsureClosed(this.m_Mobile);
                        this.m_Mobile.SendGump(new GuildGump(this.m_Mobile, this.m_Guild));

                        break;
                    }
                case 3: // View the current roster
                    {
                        EnsureClosed(this.m_Mobile);
                        this.m_Mobile.SendGump(new GuildRosterGump(this.m_Mobile, this.m_Guild));

                        break;
                    }
                case 4: // Recruit
                    {
                        this.m_Mobile.Target = new GuildRecruitTarget(this.m_Mobile, this.m_Guild);

                        break;
                    }
                case 5: // Membership candidates
                    {
                        GuildGump.EnsureClosed(this.m_Mobile);
                        this.m_Mobile.SendGump(new GuildCandidatesGump(this.m_Mobile, this.m_Guild));

                        break;
                    }
                case 6: // View charter
                    {
                        EnsureClosed(this.m_Mobile);
                        this.m_Mobile.SendGump(new GuildCharterGump(this.m_Mobile, this.m_Guild));

                        break;
                    }
                case 7: // Resign
                    {
                        this.m_Guild.RemoveMember(this.m_Mobile);

                        break;
                    }
                case 8: // View wars
                    {
                        EnsureClosed(this.m_Mobile);
                        this.m_Mobile.SendGump(new GuildWarGump(this.m_Mobile, this.m_Guild));

                        break;
                    }
                case 9: // Guildmaster functions
                    {
                        if (this.m_Mobile.AccessLevel >= AccessLevel.GameMaster || this.m_Guild.Leader == this.m_Mobile)
                        {
                            EnsureClosed(this.m_Mobile);
                            this.m_Mobile.SendGump(new GuildmasterGump(this.m_Mobile, this.m_Guild));
                        }

                        break;
                    }
            }
        }
    }
}