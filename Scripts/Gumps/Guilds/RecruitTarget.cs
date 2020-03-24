using System;

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
