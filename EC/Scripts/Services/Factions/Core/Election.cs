using System;
using System.Collections.Generic;
using System.Net;
using Server.Mobiles;

namespace Server.Factions
{
    public enum ElectionState
    {
        Pending,
        Campaign,
        Election
    }

    public class Election
    {
        public static readonly TimeSpan PendingPeriod = TimeSpan.FromDays(5.0);
        public static readonly TimeSpan CampaignPeriod = TimeSpan.FromDays(1.0);
        public static readonly TimeSpan VotingPeriod = TimeSpan.FromDays(3.0);
        public const int MaxCandidates = 10;
        public const int CandidateRank = 5;
        private readonly Faction m_Faction;
        private readonly List<Candidate> m_Candidates;
        private ElectionState m_State;
        private DateTime m_LastStateTime;
        private Timer m_Timer;
        public Election(Faction faction)
        {
            this.m_Faction = faction;
            this.m_Candidates = new List<Candidate>();

            this.StartTimer();
        }

        public Election(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Faction = Faction.ReadReference(reader);

                        this.m_LastStateTime = reader.ReadDateTime();
                        this.m_State = (ElectionState)reader.ReadEncodedInt();

                        this.m_Candidates = new List<Candidate>();

                        int count = reader.ReadEncodedInt();

                        for (int i = 0; i < count; ++i)
                        {
                            Candidate cd = new Candidate(reader);

                            if (cd.Mobile != null)
                                this.m_Candidates.Add(cd);
                        }

                        break;
                    }
            }

            this.StartTimer();
        }

        public Faction Faction
        {
            get
            {
                return this.m_Faction;
            }
        }
        public List<Candidate> Candidates
        {
            get
            {
                return this.m_Candidates;
            }
        }
        public ElectionState State
        {
            get
            {
                return this.m_State;
            }
            set
            {
                this.m_State = value;
                this.m_LastStateTime = DateTime.UtcNow;
            }
        }
        public DateTime LastStateTime
        {
            get
            {
                return this.m_LastStateTime;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ElectionState CurrentState
        {
            get
            {
                return this.m_State;
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public TimeSpan NextStateTime
        {
            get
            {
                TimeSpan period;

                switch ( this.m_State )
                {
                    default:
                    case ElectionState.Pending:
                        period = PendingPeriod;
                        break;
                    case ElectionState.Election:
                        period = VotingPeriod;
                        break;
                    case ElectionState.Campaign:
                        period = CampaignPeriod;
                        break;
                }

                TimeSpan until = (this.m_LastStateTime + period) - DateTime.UtcNow;

                if (until < TimeSpan.Zero)
                    until = TimeSpan.Zero;

                return until;
            }
            set
            {
                TimeSpan period;

                switch ( this.m_State )
                {
                    default:
                    case ElectionState.Pending:
                        period = PendingPeriod;
                        break;
                    case ElectionState.Election:
                        period = VotingPeriod;
                        break;
                    case ElectionState.Campaign:
                        period = CampaignPeriod;
                        break;
                }

                this.m_LastStateTime = DateTime.UtcNow - period + value;
            }
        }
        public void StartTimer()
        {
            this.m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.0), new TimerCallback(Slice));
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            Faction.WriteReference(writer, this.m_Faction);

            writer.Write((DateTime)this.m_LastStateTime);
            writer.WriteEncodedInt((int)this.m_State);

            writer.WriteEncodedInt(this.m_Candidates.Count);

            for (int i = 0; i < this.m_Candidates.Count; ++i)
                this.m_Candidates[i].Serialize(writer);
        }

        public void AddCandidate(Mobile mob)
        {
            if (this.IsCandidate(mob))
                return;

            this.m_Candidates.Add(new Candidate(mob));
            mob.SendLocalizedMessage(1010117); // You are now running for office.
        }

        public void RemoveVoter(Mobile mob)
        {
            if (this.m_State == ElectionState.Election)
            {
                for (int i = 0; i < this.m_Candidates.Count; ++i)
                {
                    List<Voter> voters = this.m_Candidates[i].Voters;

                    for (int j = 0; j < voters.Count; ++j)
                    {
                        Voter voter = voters[j];

                        if (voter.From == mob)
                            voters.RemoveAt(j--);
                    }
                }
            }
        }

        public void RemoveCandidate(Mobile mob)
        {
            Candidate cd = this.FindCandidate(mob);

            if (cd == null)
                return;

            this.m_Candidates.Remove(cd);
            mob.SendLocalizedMessage(1038031);

            if (this.m_State == ElectionState.Election)
            {
                if (this.m_Candidates.Count == 1)
                {
                    this.m_Faction.Broadcast(1038031); // There are no longer any valid candidates in the Faction Commander election.

                    Candidate winner = this.m_Candidates[0];

                    Mobile winMob = winner.Mobile;
                    PlayerState pl = PlayerState.Find(winMob);

                    if (pl == null || pl.Faction != this.m_Faction || winMob == this.m_Faction.Commander)
                    {
                        this.m_Faction.Broadcast(1038026); // Faction leadership has not changed.
                    }
                    else
                    {
                        this.m_Faction.Broadcast(1038028); // The faction has a new commander.
                        this.m_Faction.Commander = winMob;
                    }

                    this.m_Candidates.Clear();
                    this.State = ElectionState.Pending;
                }
                else if (this.m_Candidates.Count == 0) // well, I guess this'll never happen
                {
                    this.m_Faction.Broadcast(1038031); // There are no longer any valid candidates in the Faction Commander election.

                    this.m_Candidates.Clear();
                    this.State = ElectionState.Pending;
                }
            }
        }

        public bool IsCandidate(Mobile mob)
        {
            return (this.FindCandidate(mob) != null);
        }

        public bool CanVote(Mobile mob)
        {
            return (this.m_State == ElectionState.Election && !this.HasVoted(mob));
        }

        public bool HasVoted(Mobile mob)
        {
            return (this.FindVoter(mob) != null);
        }

        public Candidate FindCandidate(Mobile mob)
        {
            for (int i = 0; i < this.m_Candidates.Count; ++i)
            {
                if (this.m_Candidates[i].Mobile == mob)
                    return this.m_Candidates[i];
            }

            return null;
        }

        public Candidate FindVoter(Mobile mob)
        {
            for (int i = 0; i < this.m_Candidates.Count; ++i)
            {
                List<Voter> voters = this.m_Candidates[i].Voters;

                for (int j = 0; j < voters.Count; ++j)
                {
                    Voter voter = voters[j];

                    if (voter.From == mob)
                        return this.m_Candidates[i];
                }
            }

            return null;
        }

        public bool CanBeCandidate(Mobile mob)
        {
            if (this.IsCandidate(mob))
                return false;

            if (this.m_Candidates.Count >= MaxCandidates)
                return false;

            if (this.m_State != ElectionState.Campaign)
                return false; // sanity..

            PlayerState pl = PlayerState.Find(mob);

            return (pl != null && pl.Faction == this.m_Faction && pl.Rank.Rank >= CandidateRank);
        }

        public void Slice()
        {
            if (this.m_Faction.Election != this)
            {
                if (this.m_Timer != null)
                    this.m_Timer.Stop();

                this.m_Timer = null;

                return;
            }

            switch ( this.m_State )
            {
                case ElectionState.Pending:
                    {
                        if ((this.m_LastStateTime + PendingPeriod) > DateTime.UtcNow)
                            break;

                        this.m_Faction.Broadcast(1038023); // Campaigning for the Faction Commander election has begun.

                        this.m_Candidates.Clear();
                        this.State = ElectionState.Campaign;

                        break;
                    }
                case ElectionState.Campaign:
                    {
                        if ((this.m_LastStateTime + CampaignPeriod) > DateTime.UtcNow)
                            break;

                        if (this.m_Candidates.Count == 0)
                        {
                            this.m_Faction.Broadcast(1038025); // Nobody ran for office.
                            this.State = ElectionState.Pending;
                        }
                        else if (this.m_Candidates.Count == 1)
                        {
                            this.m_Faction.Broadcast(1038029); // Only one member ran for office.

                            Candidate winner = this.m_Candidates[0];

                            Mobile mob = winner.Mobile;
                            PlayerState pl = PlayerState.Find(mob);

                            if (pl == null || pl.Faction != this.m_Faction || mob == this.m_Faction.Commander)
                            {
                                this.m_Faction.Broadcast(1038026); // Faction leadership has not changed.
                            }
                            else
                            {
                                this.m_Faction.Broadcast(1038028); // The faction has a new commander.
                                this.m_Faction.Commander = mob;
                            }

                            this.m_Candidates.Clear();
                            this.State = ElectionState.Pending;
                        }
                        else
                        {
                            this.m_Faction.Broadcast(1038030);
                            this.State = ElectionState.Election;
                        }

                        break;
                    }
                case ElectionState.Election:
                    {
                        if ((this.m_LastStateTime + VotingPeriod) > DateTime.UtcNow)
                            break;

                        this.m_Faction.Broadcast(1038024); // The results for the Faction Commander election are in

                        Candidate winner = null;

                        for (int i = 0; i < this.m_Candidates.Count; ++i)
                        {
                            Candidate cd = this.m_Candidates[i];

                            PlayerState pl = PlayerState.Find(cd.Mobile);

                            if (pl == null || pl.Faction != this.m_Faction)
                                continue;

                            //cd.CleanMuleVotes();

                            if (winner == null || cd.Votes > winner.Votes)
                                winner = cd;
                        }

                        if (winner == null)
                        {
                            this.m_Faction.Broadcast(1038026); // Faction leadership has not changed.
                        }
                        else if (winner.Mobile == this.m_Faction.Commander)
                        {
                            this.m_Faction.Broadcast(1038027); // The incumbent won the election.
                        }
                        else
                        {
                            this.m_Faction.Broadcast(1038028); // The faction has a new commander.
                            this.m_Faction.Commander = winner.Mobile;
                        }

                        this.m_Candidates.Clear();
                        this.State = ElectionState.Pending;

                        break;
                    }
            }
        }
    }

    public class Voter
    {
        private readonly Mobile m_From;
        private readonly Mobile m_Candidate;
        private readonly IPAddress m_Address;
        private readonly DateTime m_Time;
        public Voter(Mobile from, Mobile candidate)
        {
            this.m_From = from;
            this.m_Candidate = candidate;

            if (this.m_From.NetState != null)
                this.m_Address = this.m_From.NetState.Address;
            else
                this.m_Address = IPAddress.None;

            this.m_Time = DateTime.UtcNow;
        }

        public Voter(GenericReader reader, Mobile candidate)
        {
            this.m_Candidate = candidate;

            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_From = reader.ReadMobile();
                        this.m_Address = Utility.Intern(reader.ReadIPAddress());
                        this.m_Time = reader.ReadDateTime();

                        break;
                    }
            }
        }

        public Mobile From
        {
            get
            {
                return this.m_From;
            }
        }
        public Mobile Candidate
        {
            get
            {
                return this.m_Candidate;
            }
        }
        public IPAddress Address
        {
            get
            {
                return this.m_Address;
            }
        }
        public DateTime Time
        {
            get
            {
                return this.m_Time;
            }
        }
        public object[] AcquireFields()
        {
            TimeSpan gameTime = TimeSpan.Zero;

            if (this.m_From is PlayerMobile)
                gameTime = ((PlayerMobile)this.m_From).GameTime;

            int kp = 0;

            PlayerState pl = PlayerState.Find(this.m_From);

            if (pl != null)
                kp = pl.KillPoints;

            int sk = this.m_From.Skills.Total;

            int factorSkills = 50 + ((sk * 100) / 10000);
            int factorKillPts = 100 + (kp * 2);
            int factorGameTime = 50 + (int)((gameTime.Ticks * 100) / TimeSpan.TicksPerDay);

            int totalFactor = (factorSkills * factorKillPts * Math.Max(factorGameTime, 100)) / 10000;

            if (totalFactor > 100)
                totalFactor = 100;
            else if (totalFactor < 0)
                totalFactor = 0;

            return new object[] { this.m_From, this.m_Address, this.m_Time, totalFactor };
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0);

            writer.Write((Mobile)this.m_From);
            writer.Write((IPAddress)this.m_Address);
            writer.Write((DateTime)this.m_Time);
        }
    }

    public class Candidate
    {
        private readonly Mobile m_Mobile;
        private readonly List<Voter> m_Voters;
        public Candidate(Mobile mob)
        {
            this.m_Mobile = mob;
            this.m_Voters = new List<Voter>();
        }

        public Candidate(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_Mobile = reader.ReadMobile();

                        int count = reader.ReadEncodedInt();
                        this.m_Voters = new List<Voter>(count);

                        for (int i = 0; i < count; ++i)
                        {
                            Voter voter = new Voter(reader, this.m_Mobile);

                            if (voter.From != null)
                                this.m_Voters.Add(voter);
                        }

                        break;
                    }
                case 0:
                    {
                        this.m_Mobile = reader.ReadMobile();

                        List<Mobile> mobs = reader.ReadStrongMobileList();
                        this.m_Voters = new List<Voter>(mobs.Count);

                        for (int i = 0; i < mobs.Count; ++i)
                            this.m_Voters.Add(new Voter(mobs[i], this.m_Mobile));

                        break;
                    }
            }
        }

        public Mobile Mobile
        {
            get
            {
                return this.m_Mobile;
            }
        }
        public List<Voter> Voters
        {
            get
            {
                return this.m_Voters;
            }
        }
        public int Votes
        {
            get
            {
                return this.m_Voters.Count;
            }
        }
        public void CleanMuleVotes()
        {
            for (int i = 0; i < this.m_Voters.Count; ++i)
            {
                Voter voter = (Voter)this.m_Voters[i];

                if ((int)voter.AcquireFields()[3] < 90)
                    this.m_Voters.RemoveAt(i--);
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)1); // version

            writer.Write((Mobile)this.m_Mobile);

            writer.WriteEncodedInt((int)this.m_Voters.Count);

            for (int i = 0; i < this.m_Voters.Count; ++i)
                ((Voter)this.m_Voters[i]).Serialize(writer);
        }
    }
}