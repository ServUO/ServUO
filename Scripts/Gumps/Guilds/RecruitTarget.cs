using System;
using Server.Factions;
using Server.Guilds;
using Server.Targeting;

namespace Server.Gumps
{
    public class GuildRecruitTarget : Target
    {
        private readonly Mobile m_Mobile;
        private readonly Guild m_Guild;

        public GuildRecruitTarget(Mobile m, Guild guild)
            : base(10, false, TargetFlags.None)
        {
            this.m_Mobile = m;
            this.m_Guild = guild;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (GuildGump.BadMember(this.m_Mobile, this.m_Guild))
                return;

            if (targeted is Mobile)
            {
                Mobile m = (Mobile)targeted;

                PlayerState guildState = PlayerState.Find(this.m_Guild.Leader);
                PlayerState targetState = PlayerState.Find(m);

                Faction guildFaction = (guildState == null ? null : guildState.Faction);
                Faction targetFaction = (targetState == null ? null : targetState.Faction);

                if (!m.Player)
                {
                    this.m_Mobile.SendLocalizedMessage(501161); // You may only recruit players into the guild.
                }
                else if (!m.Alive)
                {
                    this.m_Mobile.SendLocalizedMessage(501162); // Only the living may be recruited.
                }
                else if (this.m_Guild.IsMember(m))
                {
                    this.m_Mobile.SendLocalizedMessage(501163); // They are already a guildmember!
                }
                else if (this.m_Guild.Candidates.Contains(m))
                {
                    this.m_Mobile.SendLocalizedMessage(501164); // They are already a candidate.
                }
                else if (this.m_Guild.Accepted.Contains(m))
                {
                    this.m_Mobile.SendLocalizedMessage(501165); // They have already been accepted for membership, and merely need to use the Guildstone to gain full membership.
                }
                else if (m.Guild != null)
                {
                    this.m_Mobile.SendLocalizedMessage(501166); // You can only recruit candidates who are not already in a guild.
                }
                #region Factions
                else if (guildFaction != targetFaction)
                {
                    if (guildFaction == null)
                        this.m_Mobile.SendLocalizedMessage(1013027); // That player cannot join a non-faction guild.
                    else if (targetFaction == null)
                        this.m_Mobile.SendLocalizedMessage(1013026); // That player must be in a faction before joining this guild.
                    else
                        this.m_Mobile.SendLocalizedMessage(1013028); // That person has a different faction affiliation.
                }
                else if (targetState != null && targetState.IsLeaving)
                {
                    // OSI does this quite strangely, so we'll just do it this way
                    this.m_Mobile.SendMessage("That person is quitting their faction and so you may not recruit them.");
                }
                #endregion
                else if (this.m_Mobile.AccessLevel >= AccessLevel.GameMaster || this.m_Guild.Leader == this.m_Mobile)
                {
                    this.m_Guild.Accepted.Add(m);
                }
                else
                {
                    this.m_Guild.Candidates.Add(m);
                }
            }
        }

        protected override void OnTargetFinish(Mobile from)
        {
            if (GuildGump.BadMember(this.m_Mobile, this.m_Guild))
                return;

            GuildGump.EnsureClosed(this.m_Mobile);
            this.m_Mobile.SendGump(new GuildGump(this.m_Mobile, this.m_Guild));
        }
    }
}