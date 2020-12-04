using Server.Accounting;
using Server.Gumps;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.CityLoyalty
{
    [PropertyObject]
    public class CityElection
    {
        public static readonly int VotePeriod = 7;
        public static readonly int NominationDeadline = 24;

        [CommandProperty(AccessLevel.GameMaster)]
        public CityLoyaltySystem City { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ElectionEnded { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Ongoing => CanNominate() || CanVote();

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime AutoPickGovernor { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string Time1 => StartTimes.Length > 0 ? StartTimes[0].ToShortDateString() : "Empty";

        [CommandProperty(AccessLevel.GameMaster)]
        public string Time2 => StartTimes.Length > 1 ? StartTimes[1].ToShortDateString() : "Empty";

        [CommandProperty(AccessLevel.GameMaster)]
        public string Time3 => StartTimes.Length > 2 ? StartTimes[2].ToShortDateString() : "Empty";

        [CommandProperty(AccessLevel.GameMaster)]
        public string Time4 => StartTimes.Length > 3 ? StartTimes[3].ToShortDateString() : "Empty";

        public List<BallotEntry> Candidates { get; set; }

        public DateTime[] StartTimes { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ForceStart
        {
            get { return false; }
            set
            {
                if (value)
                {
                    if (Ongoing)
                        EndElection();

                    StartTimes = new DateTime[1];
                    StartTimes[0] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                }
            }
        }

        public override string ToString()
        {
            return "...";
        }

        public CityElection(CityLoyaltySystem city)
        {
            City = city;

            ElectionEnded = true;
            Candidates = new List<BallotEntry>();

            GetDefaultStartTimes();
        }

        public bool TryNominate(PlayerMobile pm)
        {
            CityLoyaltyEntry pentry = City.GetPlayerEntry<CityLoyaltyEntry>(pm);

            if (pm.Young)
                pm.SendMessage("Young players cannot be nominated for the ballot!");
            else if (!City.IsCitizen(pm) || pentry == null)
                pm.SendLocalizedMessage(1153890); // You must be a citizen of this City to nominate yourself for the ballot! 
            else if (City.GetLoyaltyRating(pm) < LoyaltyRating.Adored)
                pm.SendLocalizedMessage(1153891); // You must at least be adored within the City to nominate yourself for the ballot. 
            else
            {
                Account a = pm.Account as Account;
                for (int i = 0; i < a.Length; i++)
                {
                    Mobile m = a[i];

                    if (!(m is PlayerMobile))
                        continue;

                    BallotEntry ballot = Candidates.FirstOrDefault(entry => entry.Player == m);

                    if (ballot != null)
                    {
                        pm.SendLocalizedMessage(ballot.Endorsements.Count > 0 ? 1153917 : 1153889);  // A character from this account is currently endorsed for Candidacy and cannot be nominated.                                                                      // A character from this account has already been nominated to run for office.
                        return false;                                                                // A character from this account has already been nominated to run for office. 
                    }

                    ballot = Candidates.FirstOrDefault(entry => entry.Endorsements.Contains(m));

                    if (ballot != null)
                    {
                        pm.SendLocalizedMessage(1153892); // A character from this account has already endorsed a nominee! 
                        return false;
                    }
                }

                Candidates.Add(new BallotEntry(pm, pentry.Love, pentry.Hate));
                pm.PrivateOverheadMessage(Network.MessageType.Regular, 0x3B2, 1153905, pm.NetState); // *You etch your name into the stone* 
                pm.SendLocalizedMessage(1154087); // You have 24 hours to get your nomination endorsed. If you do not get an endorsement within that period you will need to re-nominate yourself.

                return true;
            }

            return false;
        }

        public bool TryEndorse(PlayerMobile pm, PlayerMobile nominee)
        {
            if (pm.Young)
                pm.SendMessage("Young players cannot endorose an nominee!");
            else if (!City.IsCitizen(pm))
                pm.SendLocalizedMessage(1153893); // You must be a citizen of this City to endorse a nominee for the ballot! 
            else
            {
                Account a = pm.Account as Account;

                for (int i = 0; i < a.Length; i++)
                {
                    Mobile m = a[i];

                    if (!(m is PlayerMobile))
                        continue;

                    BallotEntry ballot = Candidates.FirstOrDefault(entry => entry.Endorsements.Contains(m as PlayerMobile));

                    if (ballot != null)
                    {
                        pm.SendLocalizedMessage(1153892); // A character from this account has already endorsed a nominee! 
                        return false;
                    }

                    BallotEntry ballot2 = Candidates.FirstOrDefault(entry => entry.Player == m);

                    if (ballot2 != null)
                    {
                        pm.SendLocalizedMessage(1153912); // A character from this account is currently nominated for candidacy and cannot offer an endorsement.  
                        return false;
                    }
                }

                BallotEntry pentry = Candidates.FirstOrDefault(e => e.Player == nominee);

                if (pentry != null)
                {
                    //<CENTER>Are you sure you wish to endorse this nominee? All endorsements are final and cannot be changed!</CENTER>
                    pm.SendGump(new ConfirmCallbackGump(pm, null, 1154091, pentry, null, (m, o) =>
                    {
                        BallotEntry e = o as BallotEntry;
                        e.Endorsements.Add(m as PlayerMobile);
                        m.PrivateOverheadMessage(Network.MessageType.Regular, 0x3B2, 1153913, m.NetState); // *You etch your endorsement for the nominee into the stone*

                    }));

                    return true;
                }
            }

            return false;
        }

        public bool TryVote(PlayerMobile voter, PlayerMobile candidate)
        {
            if (!CanVote())
                voter.SendLocalizedMessage(1153919); // The stone is not currently accepting votes. 
            else if (voter.Young)
                voter.SendMessage("Young players cannot vote in a city election!");
            else if (!City.IsCitizen(voter))
                voter.SendLocalizedMessage(1153894); // You must be a citizen of this City to vote! 
            else if (City.GetLoyaltyRating(voter) < LoyaltyRating.Respected)
                voter.SendLocalizedMessage(1154579); // You must be at least respected within the city to vote. 
            else
            {
                Account a = voter.Account as Account;

                for (int i = 0; i < a.Length; i++)
                {
                    Mobile m = a[i];

                    if (m is PlayerMobile && Candidates.FirstOrDefault(e => e.Votes.Contains(m)) != null)
                    {
                        voter.SendLocalizedMessage(1153922); // This account has already cast a vote in this election. You may only vote once.
                        return false;
                    }
                }

                BallotEntry pentry = Candidates.FirstOrDefault(e => e.Player == candidate);

                if (pentry != null)
                {
                    //<CENTER>Are you sure you wish to cast your vote for this candidate? All votes are final and cannot be changed!</CENTER>
                    voter.SendGump(new ConfirmCallbackGump(voter, null, 1153921, pentry, null, (m, o) =>
                    {
                        BallotEntry e = o as BallotEntry;
                        e.Votes.Add(voter);
                        m.PrivateOverheadMessage(Network.MessageType.Regular, 0x3B2, 1153923, voter.NetState); // *You etch your vote into the stone* 
                    }));
                }
            }

            return false;
        }

        public void TryWithdraw(PlayerMobile pm)
        {
            BallotEntry entry = Candidates.FirstOrDefault(e => e.Player == pm);

            if (entry != null)
            {
                //Are you sure you wish to withdrawal? 
                pm.SendGump(new ConfirmCallbackGump(pm, null, 1153918, entry, null, (m, o) =>
                {
                    BallotEntry e = (BallotEntry)o;

                    if (Candidates.Contains(e))
                    {
                        Candidates.Remove(e);
                        m.PrivateOverheadMessage(Network.MessageType.Regular, 0x3B2, 1153911, m.NetState); // *You smudge your name off the stone* 
                    }

                }));
            }
            else
                pm.SendLocalizedMessage(1153924); // You are not currently on any ballot to withdraw from. 
        }

        public double GetStanding(BallotEntry entry)
        {
            if (Candidates.Contains(entry))
            {
                if (entry.Votes.Count <= 0)
                    return 0.0;

                return (entry.Votes.Count / GetTotalVotes()) * 100;
            }

            return 0.0;
        }

        public int GetTotalVotes()
        {
            int votes = 0;

            Candidates.ForEach(c =>
                {
                    votes += c.Votes.Count;
                });

            return votes;
        }

        public void OnTick()
        {
            foreach (DateTime dt in StartTimes)
            {
                if (dt.Year == DateTime.Now.Year && DateTime.Now.Month == dt.Month && DateTime.Now.Day > 14 && !ElectionEnded)
                {
                    EndElection();
                }
                else if (dt.Year == DateTime.Now.Year && DateTime.Now.Month == dt.Month && DateTime.Now.Day == dt.Day && ElectionEnded)
                {
                    StartNewElection();
                }
            }

            if (City.Governor == null && AutoPickGovernor != DateTime.MinValue && DateTime.Now > AutoPickGovernor)
            {
                if (City.GovernorElect != null)
                {
                    if (Candidates.Count > 0)
                    {
                        BallotEntry entry = Candidates.FirstOrDefault(c => c.Player == City.GovernorElect);

                        if (entry != null)
                            Candidates.Remove(entry);
                    }

                    City.GovernorElect = null;
                }

                if (Candidates.Count > 0)
                {
                    Candidates.Sort();
                    City.GovernorElect = Candidates[0].Player;
                }

                AutoPickGovernor = DateTime.MinValue;
            }

            if (CanNominate())
            {
                foreach (var entry in Candidates.ToList())
                {
                    if (entry.TimeOfNomination + TimeSpan.FromHours(NominationDeadline) < DateTime.Now && entry.Endorsements.Count == 0)
                        Candidates.Remove(entry);
                }
            }
        }

        public void StartNewElection()
        {
            ElectionEnded = false;

            Candidates.Clear();
            Candidates.TrimExcess();

            AutoPickGovernor = DateTime.MinValue;
        }

        private static readonly int[] _Periods = { 3, 6, 9, 12 };

        public void GetDefaultStartTimes()
        {
            StartTimes = new DateTime[4];

            for (int i = 0; i < _Periods.Length; i++)
            {
                DateTime dt = new DateTime(DateTime.Now.Year, _Periods[i], 1);

                if (dt < DateTime.Now)
                    dt = new DateTime(dt.Year + 1, dt.Month, dt.Day);

                StartTimes[i] = dt;
            }
        }

        public static DateTime[] ValidateStartTimes(Mobile m, int[] times)
        {
            if (times != null && times.Length > 0)
            {
                DateTime[] starttimes = new DateTime[times.Length];

                for (int i = 0; i < times.Length; i++)
                {
                    int month = times[i];

                    if (month <= 0 || month > 12)
                    {
                        m.SendMessage("Invalid month for #{0}.  It must be between 0 and 12.", i + 1);
                        return null;
                    }

                    DateTime dt = new DateTime(DateTime.Now.Year, month, 1);

                    if (dt < DateTime.Now)
                        dt = new DateTime(dt.Year + 1, dt.Month, dt.Day);

                    starttimes[i] = dt;
                }

                if (CheckConflict(starttimes))
                {
                    m.SendMessage("times are too close to another start time.  Please try again.");
                    return null;
                }

                return starttimes;
            }

            return null;
        }

        public static bool CheckConflict(DateTime[] times)
        {
            if (times == null || times.Length == 0)
                return false;

            for (int i = 0; i < times.Length; i++)
            {
                DateTime t = times[i];

                for (int j = 0; j < times.Length; j++)
                {
                    if (i == j)
                        continue;

                    if ((times[j] > t && times[j] - t < TimeSpan.FromDays(30)) || times[j] < t && t - times[j] < TimeSpan.FromDays(30))
                        return true;
                }
            }

            return false;
        }

        public DateTime NextElection()
        {
            if (CanNominate() || CanVote())
                return DateTime.MinValue;

            DateTime closest = DateTime.MinValue;

            foreach (DateTime dt in StartTimes)
            {
                if (DateTime.Now < dt)
                {
                    if (closest == DateTime.MinValue || dt - DateTime.Now < closest - DateTime.Now)
                    {
                        closest = dt;
                    }
                }
            }

            return closest;
        }

        public bool CanNominate()
        {
            DateTime until = DateTime.MinValue;
            return CanNominate(out until);
        }

        public bool CanNominate(out DateTime until)
        {
            until = DateTime.Now;
            foreach (DateTime dt in StartTimes)
            {
                if (dt.Year == DateTime.Now.Year && DateTime.Now.Month == dt.Month && DateTime.Now <= dt.AddDays(VotePeriod))
                {
                    until = dt.AddDays(VotePeriod);
                    return true;
                }
            }

            return false;
        }

        public bool CanVote()
        {
            DateTime until = DateTime.MinValue;
            return CanVote(out until);
        }

        public bool CanVote(out DateTime until)
        {
            until = DateTime.Now;
            foreach (DateTime dt in StartTimes)
            {
                if (dt.Year == DateTime.Now.Year && DateTime.Now.Month == dt.Month && DateTime.Now > dt.AddDays(VotePeriod) && DateTime.Now <= dt.AddDays(VotePeriod * 2))
                {
                    until = dt.AddDays(VotePeriod * 2);

                    return true;
                }
            }

            return false;
        }

        public void EndElection()
        {
            ElectionEnded = true;

            if (Candidates.Count > 0)
            {
                Candidates.Sort();
                City.GovernorElect = Candidates[0].Player;

                AutoPickGovernor = DateTime.Now + TimeSpan.FromDays(7);
            }

            if (City.GovernorElect == null)
            {
                City.PendingGovernor = true;
                AutoPickGovernor = DateTime.Now + TimeSpan.FromDays(Utility.RandomMinMax(2, 4));

                if (City.Stone != null)
                    City.Stone.InvalidateProperties();
            }

            for (int i = 0; i < StartTimes.Length; i++)
            {
                DateTime dt = StartTimes[i];

                if (dt == DateTime.MinValue)
                    continue;

                if (dt < DateTime.Now)
                    dt = new DateTime(dt.Year + 1, dt.Month, dt.Day);
            }
        }

        public void OnRejectOffice(PlayerMobile pm)
        {
            BallotEntry entry = Candidates.FirstOrDefault(c => c.Player == pm);

            if (entry != null)
                Candidates.Remove(entry);

            AutoPickGovernor = DateTime.Now + TimeSpan.FromDays(Utility.RandomMinMax(2, 4));
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(ElectionEnded);
            writer.Write(AutoPickGovernor);

            writer.Write(StartTimes.Length);
            foreach (DateTime dt in StartTimes)
                writer.Write(dt);

            writer.Write(Candidates.Count);
            Candidates.ForEach(entry => entry.Serialize(writer));
        }

        public CityElection(CityLoyaltySystem city, GenericReader reader)
        {
            City = city;
            Candidates = new List<BallotEntry>();

            int version = reader.ReadInt();

            ElectionEnded = reader.ReadBool();
            AutoPickGovernor = reader.ReadDateTime();

            int c = reader.ReadInt();
            StartTimes = new DateTime[c];
            for (int i = 0; i < c; i++)
            {
                DateTime time = reader.ReadDateTime();

                if (time < DateTime.Now && ElectionEnded)
                    time = new DateTime(time.Year + 1, time.Month, time.Day);

                StartTimes[i] = time;
            }

            c = reader.ReadInt();
            for (int i = 0; i < c; i++)
            {
                BallotEntry entry = new BallotEntry(reader);
                if (entry.Player != null)
                    Candidates.Add(entry);
            }
        }
    }

    public class BallotEntry : IComparable<BallotEntry>
    {
        public PlayerMobile Player { get; set; }
        public DateTime TimeOfNomination { get; set; }

        public int Love { get; set; }
        public int Hate { get; set; }

        public List<PlayerMobile> Endorsements { get; set; }
        public List<PlayerMobile> Votes { get; set; }

        public BallotEntry(PlayerMobile pm, int love, int hate)
        {
            Player = pm;
            TimeOfNomination = DateTime.Now;

            Love = love;
            Hate = hate;

            Endorsements = new List<PlayerMobile>();
            Votes = new List<PlayerMobile>();
        }

        public int CompareTo(BallotEntry entry)
        {
            if (Player == null)
                return -1;

            if (Votes.Count > entry.Votes.Count)
                return 1;
            else if (Votes.Count < entry.Votes.Count)
                return -1;
            else
            {
                if (Love > entry.Love || Hate < entry.Hate)
                    return 1;
                else if (Love == entry.Love && Hate == entry.Hate && Utility.RandomBool())
                    return 1;
                else
                    return -1;
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(0);

            writer.Write(Player);
            writer.Write(TimeOfNomination);

            writer.Write(Love);
            writer.Write(Hate);

            writer.Write(Endorsements.Count);
            Endorsements.ForEach(p => writer.Write(p));

            writer.Write(Votes.Count);
            Votes.ForEach(p => writer.Write(p));
        }

        public BallotEntry(GenericReader reader)
        {
            Endorsements = new List<PlayerMobile>();
            Votes = new List<PlayerMobile>();

            int version = reader.ReadInt();

            Player = reader.ReadMobile() as PlayerMobile;
            TimeOfNomination = reader.ReadDateTime();

            Love = reader.ReadInt();
            Hate = reader.ReadInt();

            int c = reader.ReadInt();
            for (int i = 0; i < c; i++)
            {
                PlayerMobile p = reader.ReadMobile() as PlayerMobile;
                if (p != null)
                    Endorsements.Add(p);
            }

            c = reader.ReadInt();
            for (int i = 0; i < c; i++)
            {
                PlayerMobile p = reader.ReadMobile() as PlayerMobile;
                if (p != null)
                    Votes.Add(p);
            }
        }
    }
}
