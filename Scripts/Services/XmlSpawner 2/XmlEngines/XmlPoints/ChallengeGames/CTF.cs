using System;
using System.Collections;
using Server.Engines.XmlSpawner2;
using Server.Gumps;
using Server.Network;

/*
** CTFGauntlet
** ArteGordon
** updated 12/05/04
**
** used to set up a capture the flag pvp challenge game through the XmlPoints system.
*/
namespace Server.Items
{
    public class CTFBase : Item
    {
        private int m_Team;
        private int m_ProximityRange = 1;
        private CTFFlag m_Flag;
        private bool m_HasFlag;
        private CTFGauntlet m_gauntlet;
        public CTFBase(CTFGauntlet gauntlet, int team)
            : base(0x1183)
        {
            this.Movable = false;
            this.Hue = BaseChallengeGame.TeamColor(team);
            this.Team = team;
            this.Name = String.Format("Team {0} Base", team);
            this.m_gauntlet = gauntlet;

            // add the flag

            this.Flag = new CTFFlag(this, team);
            this.Flag.HomeBase = this;
            this.HasFlag = true;
        }

        public CTFBase(Serial serial)
            : base(serial)
        {
        }

        public int Team
        {
            get
            {
                return this.m_Team;
            }
            set
            {
                this.m_Team = value;
            }
        }
        public CTFFlag Flag
        {
            get
            {
                return this.m_Flag;
            }
            set
            {
                this.m_Flag = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ProximityRange
        {
            get
            {
                return this.m_ProximityRange;
            }
            set
            {
                this.m_ProximityRange = value;
            }
        }
        public bool HasFlag
        {
            get
            {
                return this.m_HasFlag;
            }
            set
            {
                this.m_HasFlag = value;
            }
        }
        public override bool HandlesOnMovement
        {
            get
            {
                return this.m_gauntlet != null;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.Team);
            writer.Write(this.m_ProximityRange);
            writer.Write(this.m_Flag);
            writer.Write(this.m_gauntlet);
            writer.Write(this.m_HasFlag);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.Team = reader.ReadInt();
            this.ProximityRange = reader.ReadInt();
            this.Flag = reader.ReadItem() as CTFFlag;
            this.m_gauntlet = reader.ReadItem() as CTFGauntlet;
            this.m_HasFlag = reader.ReadBool();
        }

        public override void OnDelete()
        {
            // delete any flag associated with the base
            if (this.m_Flag != null)
                this.m_Flag.Delete();

            base.OnDelete();
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            // set the flag location
            this.PlaceFlagAtBase();
        }

        public void PlaceFlagAtBase()
        {
            if (this.m_Flag != null)
            {
                this.m_Flag.MoveToWorld(new Point3D(this.Location.X + 1, this.Location.Y, this.Location.Z + 4), this.Map);
            }
        }

        public override void OnMapChange()
        {
            // set the flag location
            this.PlaceFlagAtBase();
        }

        public void ReturnFlag()
        {
            this.ReturnFlag(true);
        }

        public void ReturnFlag(bool verbose)
        {
            if (this.Flag == null)
                return;

            this.PlaceFlagAtBase();
            this.HasFlag = true;
            if (this.m_gauntlet != null && verbose)
            {
                this.m_gauntlet.GameBroadcast(100419, this.Team); // "Team {0} flag has been returned to base"
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m == null || this.m_gauntlet == null)
                return;
            
            if (m == null || m.AccessLevel > AccessLevel.Player)
                return;

            // look for players within range of the base
            // check to see if player is within range of the spawner
            if ((this.Parent == null) && Utility.InRange(m.Location, this.Location, this.m_ProximityRange))
            {
                CTFGauntlet.ChallengeEntry entry = this.m_gauntlet.GetParticipant(m) as CTFGauntlet.ChallengeEntry;

                if (entry == null)
                    return;
                
                bool carryingflag = false;
                // is the player carrying a flag?
                foreach (CTFBase b in this.m_gauntlet.HomeBases)
                {
                    if (b != null && !b.Deleted && b.Flag != null && b.Flag.RootParent == m)
                    {
                        carryingflag = true;
                        break;
                    }
                }

                // if the player is on an opposing team and the flag is at the base and the player doesnt already
                // have a flag then give them the flag
                if (entry.Team != this.Team && this.HasFlag && !carryingflag)
                {
                    m.AddToBackpack(this.m_Flag);
                    this.HasFlag = false;
                    this.m_gauntlet.GameBroadcast(100420, entry.Team, this.Team); // "Team {0} has the Team {1} flag"
                    this.m_gauntlet.GameBroadcastSound(513);
                }
                else if (entry.Team == this.Team)
                {
                    // if the player has an opposing teams flag then give them a point and return the flag
                    foreach (CTFBase b in this.m_gauntlet.HomeBases)
                    {
                        if (b != null && !b.Deleted && b.Flag != null && b.Flag.RootParent == m && b.Team != entry.Team)
                        {
                            this.m_gauntlet.GameBroadcast(100421, entry.Team);  // "Team {0} has scored"
                            this.m_gauntlet.AddScore(entry);

                            Effects.SendTargetParticles(entry.Participant, 0x375A, 35, 20, BaseChallengeGame.TeamColor(entry.Team), 0x00, 9502,
                                (EffectLayer)255, 0x100);
                            // play the score sound
                            this.m_gauntlet.ScoreSound(entry.Team);

                            b.ReturnFlag(false);
                            break;
                        }
                    }
                }
            }
        }
    }

    public class CTFFlag : Item
    {
        public CTFBase HomeBase;
        public CTFFlag(CTFBase homebase, int team)
            : base(0x161D)
        {
            this.Hue = BaseChallengeGame.TeamColor(team);
            this.Name = String.Format("Team {0} Flag", team);
            this.HomeBase = homebase;
        }

        public CTFFlag(Serial serial)
            : base(serial)
        {
        }

        public override bool OnDroppedInto(Mobile from, Container target, Point3D p)
        {
            // allow movement within a players backpack
            if (from != null && from.Backpack == target)
            {
                return base.OnDroppedInto(from, target, p);
            }

            return false;
        }

        public override bool OnDroppedOnto(Mobile from, Item target)
        {
            return false;
        }

        public override bool OnDroppedToMobile(Mobile from, Mobile target)
        {
            return false;
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            // only allow staff to pick it up when at a base
            if ((from != null && from.AccessLevel > AccessLevel.Player) || this.RootParent != null)
            {
                return base.CheckLift(from, item, ref reject);
            }
            return false;
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D point)
        {
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.HomeBase);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.HomeBase = reader.ReadItem() as CTFBase;
        }
    }

    public class CTFGauntlet : BaseChallengeGame
    {
        public static bool OnlyInChallengeGameRegion = false;// if this is true, then the game can only be set up in a challenge game region
        private static readonly TimeSpan MaximumOutOfBoundsDuration = TimeSpan.FromSeconds(15);// maximum time allowed out of bounds before disqualification
        private static readonly TimeSpan MaximumOfflineDuration = TimeSpan.FromSeconds(60);// maximum time allowed offline before disqualification
        private static readonly TimeSpan MaximumHiddenDuration = TimeSpan.FromSeconds(10);// maximum time allowed hidden before disqualification
        private static readonly TimeSpan RespawnTime = TimeSpan.FromSeconds(6);// delay until autores if autores is enabled
        private readonly ArrayList m_Organizers = new ArrayList();
        private Mobile m_Challenger;
        private ArrayList m_Participants = new ArrayList();
        private bool m_GameLocked;
        private bool m_GameInProgress;
        private int m_TotalPurse;
        private int m_EntryFee;
        private int m_TargetScore = 10;// default target score to end match is 10
        private DateTime m_MatchStart;
        private DateTime m_MatchEnd;
        private TimeSpan m_MatchLength = TimeSpan.FromMinutes(10);// default match length is 10 mins
        private int m_ArenaSize = 0;// maximum distance from the challenge gauntlet allowed before disqualification.  Zero is unlimited range
        private int m_Winner = 0;
        private ArrayList m_HomeBases = new ArrayList();
        public CTFGauntlet(Mobile challenger)
            : base(0x1414)
        {
            this.m_Challenger = challenger;

            this.m_Organizers.Add(challenger);

            // check for points attachments
            XmlPoints afrom = (XmlPoints)XmlAttach.FindAttachment(challenger, typeof(XmlPoints));

            this.Movable = false;

            this.Hue = 33;

            if (challenger == null || afrom == null || afrom.Deleted)
            {
                this.Delete();
            }
            else
            {
                this.Name = XmlPoints.SystemText(100418) + " " + String.Format(XmlPoints.SystemText(100315), challenger.Name); // "Challenge by {0}"
            }
        }

        public CTFGauntlet(Serial serial)
            : base(serial)
        {
        }

        public ArrayList HomeBases
        {
            get
            {
                return this.m_HomeBases;
            }
            set
            {
                this.m_HomeBases = value;
            }
        }
        // how long before the gauntlet decays if a gauntlet is dropped but never started
        public override TimeSpan DecayTime
        {
            get
            {
                return TimeSpan.FromMinutes(15);
            }
        }// this will apply to the setup
        public override ArrayList Organizers
        {
            get
            {
                return this.m_Organizers;
            }
        }
        public override bool AllowPoints
        {
            get
            {
                return false;
            }
        }// determines whether kills during the game will award points.  If this is false, UseKillDelay is ignored
        public override bool UseKillDelay
        {
            get
            {
                return true;
            }
        }// determines whether the normal delay between kills of the same player for points is enforced
        public bool AutoRes
        {
            get
            {
                return true;
            }
        }// determines whether players auto res after being killed
        public bool AllowOnlyInChallengeRegions
        {
            get
            {
                return false;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan MatchLength
        {
            get
            {
                return this.m_MatchLength;
            }
            set
            {
                this.m_MatchLength = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime MatchStart
        {
            get
            {
                return this.m_MatchStart;
            }
            set
            {
                this.m_MatchStart = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime MatchEnd
        {
            get
            {
                return this.m_MatchEnd;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public override Mobile Challenger
        {
            get
            {
                return this.m_Challenger;
            }
            set
            {
                this.m_Challenger = value;
            }
        }
        public override bool GameLocked
        {
            get
            {
                return this.m_GameLocked;
            }
            set
            {
                this.m_GameLocked = value;
            }
        }
        public override bool GameInProgress
        {
            get
            {
                return this.m_GameInProgress;
            }
            set
            {
                this.m_GameInProgress = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public override bool GameCompleted
        {
            get
            {
                return !this.m_GameInProgress && this.m_GameLocked;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public override int ArenaSize
        {
            get
            {
                return this.m_ArenaSize;
            }
            set
            {
                this.m_ArenaSize = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int TargetScore
        {
            get
            {
                return this.m_TargetScore;
            }
            set
            {
                this.m_TargetScore = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Winner
        {
            get
            {
                return this.m_Winner;
            }
            set
            {
                this.m_Winner = value;
            }
        }
        public override ArrayList Participants
        {
            get
            {
                return this.m_Participants;
            }
            set
            {
                this.m_Participants = value;
            }
        }
        public override int TotalPurse
        {
            get
            {
                return this.m_TotalPurse;
            }
            set
            {
                this.m_TotalPurse = value;
            }
        }
        public override int EntryFee
        {
            get
            {
                return this.m_EntryFee;
            }
            set
            {
                this.m_EntryFee = value;
            }
        }
        public override bool InsuranceIsFree(Mobile from, Mobile awardto)
        {
            return true;
        }

        public void ScoreSound(int team)
        {
            foreach (ChallengeEntry entry in this.Participants)
            {
                if (entry.Participant == null || entry.Status != ChallengeStatus.Active)
                    continue;

                if (entry.Team == team)
                {
                    // play the team scored sound
                    entry.Participant.PlaySound(503);
                }
                else
                {
                    // play the opponent scored sound
                    entry.Participant.PlaySound(855 /*700*/);
                }
            }
        }

        public override void OnTick()
        {
            this.CheckForDisqualification();

            // check for anyone carrying flags
            if (this.HomeBases != null)
            {
                ArrayList dlist = null;

                foreach (CTFBase b in this.HomeBases)
                {
                    if (b == null || b.Deleted)
                    {
                        if (dlist == null)
                            dlist = new ArrayList();
                        dlist.Add(b);
                        continue;
                    }

                    if (!b.Deleted && b.Flag != null && !b.Flag.Deleted)
                    {
                        if (b.Flag.RootParent is Mobile)
                        {
                            Mobile m = b.Flag.RootParent as Mobile;

                            // make sure a participant has it
                            IChallengeEntry entry = this.GetParticipant(m);

                            if (entry != null)
                            {
                                // display the flag
                                //m.PublicOverheadMessage( MessageType.Regular, BaseChallengeGame.TeamColor(b.Team), false, b.Team.ToString());
                                Effects.SendTargetParticles(m, 0x375A, 35, 10, BaseChallengeGame.TeamColor(b.Team), 0x00, 9502,
                                    (EffectLayer)255, 0x100);
                            }
                            else
                            {
                                b.ReturnFlag();
                            }
                        }
                        else if (!b.HasFlag)
                        {
                            b.ReturnFlag();
                        }
                    }
                }

                if (dlist != null)
                {
                    foreach (CTFBase b in dlist)
                        this.HomeBases.Remove(b);
                }
            }
        }

        public void ReturnAnyFlags(Mobile m)
        {
            // check for anyone carrying flags
            if (this.HomeBases != null)
            {
                foreach (CTFBase b in this.HomeBases)
                {
                    if (!b.Deleted && b.Flag != null && !b.Flag.Deleted)
                    {
                        if (b.Flag.RootParent is Mobile)
                        {
                            if (m == b.Flag.RootParent as Mobile)
                            {
                                b.ReturnFlag();
                            }
                        }
                        else if (b.Flag.RootParent is Corpse)
                        {
                            if (m == ((Corpse)(b.Flag.RootParent)).Owner)
                            {
                                b.ReturnFlag();
                            }
                        }
                    }
                }
            }
        }

        public void CheckForDisqualification()
        {
            if (this.Participants == null || !this.GameInProgress)
                return;

            bool statuschange = false;

            foreach (ChallengeEntry entry in this.Participants)
            {
                if (entry.Participant == null || entry.Status != ChallengeStatus.Active)
                    continue;

                bool hadcaution = (entry.Caution != ChallengeStatus.None);

                // and a map check
                if (entry.Participant.Map != this.Map)
                {
                    // check to see if they are offline
                    if (entry.Participant.Map == Map.Internal)
                    {
                        // then give them a little time to return before disqualification
                        if (entry.Caution == ChallengeStatus.Offline)
                        {
                            // were previously out of bounds so check for disqualification
                            // check to see how long they have been out of bounds
                            if (DateTime.UtcNow - entry.LastCaution > MaximumOfflineDuration)
                            {
                                // return any flag they might be carrying
                                this.ReturnAnyFlags(entry.Participant);
                                entry.LastCaution = DateTime.UtcNow;
                            }
                        }
                        else
                        {
                            entry.LastCaution = DateTime.UtcNow;
                            statuschange = true;
                        }

                        entry.Caution = ChallengeStatus.Offline;
                    }
                    else
                    {
                        // changing to any other map results in
                        // return of any flag they might be carrying
                        this.ReturnAnyFlags(entry.Participant);
                        entry.Caution = ChallengeStatus.None;
                    }
                }
                else if (this.m_ArenaSize > 0 && !Utility.InRange(entry.Participant.Location, this.Location, this.m_ArenaSize) ||
                         (this.IsInChallengeGameRegion && !(Region.Find(entry.Participant.Location, entry.Participant.Map) is ChallengeGameRegion)))
                {
                    if (entry.Caution == ChallengeStatus.OutOfBounds)
                    {
                        // were previously out of bounds so check for disqualification
                        // check to see how long they have been out of bounds
                        if (DateTime.UtcNow - entry.LastCaution > MaximumOutOfBoundsDuration)
                        {
                            // return any flag they might be carrying
                            this.ReturnAnyFlags(entry.Participant);
                            entry.Caution = ChallengeStatus.None;
                            statuschange = true;
                        }
                    }
                    else
                    {
                        entry.LastCaution = DateTime.UtcNow;
                        // inform the player
                        XmlPoints.SendText(entry.Participant, 100309, MaximumOutOfBoundsDuration.TotalSeconds);  // "You are out of bounds!  You have {0} seconds to return"
                        statuschange = true;
                    }

                    entry.Caution = ChallengeStatus.OutOfBounds;
                }
                else if (entry.Participant.Hidden)
                {
                    if (entry.Caution == ChallengeStatus.Hidden)
                    {
                        // were previously hidden so check for disqualification
                        // check to see how long they have hidden
                        if (DateTime.UtcNow - entry.LastCaution > MaximumHiddenDuration)
                        {
                            // return any flag they might be carrying
                            this.ReturnAnyFlags(entry.Participant);
                            entry.Participant.Hidden = false;
                            entry.Caution = ChallengeStatus.None;
                            statuschange = true;
                        }
                    }
                    else
                    {
                        entry.LastCaution = DateTime.UtcNow;
                        // inform the player
                        XmlPoints.SendText(entry.Participant, 100310, MaximumHiddenDuration.TotalSeconds); // "You have {0} seconds become unhidden"
                        statuschange = true;
                    }

                    entry.Caution = ChallengeStatus.Hidden;
                }
                else
                {
                    entry.Caution = ChallengeStatus.None;
                }

                if (hadcaution && entry.Caution == ChallengeStatus.None)
                    statuschange = true;
            }

            if (statuschange)
            {
                // update gumps with the new status
                CTFGump.RefreshAllGumps(this, false);
            }

            // it is possible that the game could end like this so check
            this.CheckForGameEnd();
        }

        public CTFBase FindBase(int team)
        {
            // go through the current bases and see if there is one for this team
            if (this.HomeBases != null)
            {
                foreach (CTFBase b in this.HomeBases)
                {
                    if (b.Team == team)
                    {
                        // found one
                        return b;
                    }
                }
            }
            
            return null;
        }

        public void DeleteBases()
        {
            if (this.HomeBases != null)
            {
                foreach (CTFBase b in this.HomeBases)
                {
                    b.Delete();
                }
                
                this.HomeBases = new ArrayList();
            }
        }

        public override void OnDelete()
        {
            this.ClearNameHue();

            // remove all bases
            this.DeleteBases();

            base.OnDelete();
        }

        public override void EndGame()
        {
            this.ClearNameHue();
            
            this.DeleteBases();
            
            this.m_MatchEnd = DateTime.UtcNow;

            base.EndGame();
        }

        public override void StartGame()
        {
            base.StartGame();

            this.MatchStart = DateTime.UtcNow;

            this.SetNameHue();
			
            // teleport to base
            this.TeleportPlayersToBase();
        }

        public void TeleportPlayersToBase()
        {
            // teleport players to the base
            if (this.Participants != null)
            {
                foreach (ChallengeEntry entry in this.Participants)
                {
                    CTFBase teambase = this.FindBase(entry.Team);

                    if (entry.Participant != null && teambase != null)
                    {
                        entry.Participant.MoveToWorld(teambase.Location, teambase.Map);
                    }
                }
            }
        }

        public override void CheckForGameEnd()
        {
            if (this.Participants == null || !this.GameInProgress)
                return;

            ArrayList winner = new ArrayList();

            ArrayList teams = this.GetTeams();

            int leftstanding = 0;

            int maxscore = -99999;

            // has any team reached the target score
            TeamInfo lastt = null;

            foreach (TeamInfo t in teams)
            {
                if (!this.HasValidMembers(t))
                    continue;

                if (this.TargetScore > 0 && t.Score >= this.TargetScore)
                {
                    winner.Add(t);
                    t.Winner = true;
                }

                if (t.Score >= maxscore)
                {
                    maxscore = t.Score;
                }
                leftstanding++;
                lastt = t;
            }

            // check to make sure the team hasnt been disqualified

            // if only one is left then they are the winner
            if (leftstanding == 1 && winner.Count == 0)
            {
                winner.Add(lastt);
                lastt.Winner = true;
            }

            if (winner.Count == 0 && this.MatchLength > TimeSpan.Zero && (DateTime.UtcNow >= this.MatchStart + this.MatchLength))
            {
                // find the highest score
                // has anyone reached the target score
                foreach (TeamInfo t in teams)
                {
                    if (!this.HasValidMembers(t))
                        continue;

                    if (t.Score >= maxscore)
                    {
                        winner.Add(t);
                        t.Winner = true;
                    }
                }
            }

            // and then check to see if this is the CTF
            if (winner.Count > 0)
            {
                // declare the winner(s) and end the game
                foreach (TeamInfo t in winner)
                {
                    // flag all members as winners
                    foreach (IChallengeEntry entry in t.Members)
                        entry.Winner = true;

                    this.GameBroadcast(100414, t.ID);  // "Team {0} is the winner!"

                    this.GameBroadcastSound(744);
                    this.AwardTeamWinnings(t.ID, this.TotalPurse / winner.Count);

                    if (winner.Count == 1)
                        this.Winner = t.ID;
                }

                this.RefreshAllNoto();

                this.EndGame();
                CTFGump.RefreshAllGumps(this, true);
            }
        }

        public void SubtractScore(ChallengeEntry entry)
        {
            if (entry == null)
                return;

            entry.Score--;

            // refresh the gumps
            CTFGump.RefreshAllGumps(this, false);
        }

        public void AddScore(ChallengeEntry entry)
        {
            if (entry == null)
                return;

            entry.Score++;
            
            // refresh the gumps
            CTFGump.RefreshAllGumps(this, false);
        }

        public override void OnPlayerKilled(Mobile killer, Mobile killed)
        {
            if (killed == null)
                return;

            if (this.AutoRes)
            {
                // prepare the autores callback
                Timer.DelayCall(RespawnTime, new TimerStateCallback(XmlPoints.AutoRes_Callback),
                    new object[] { killed, true });
            }

            // return any flag they were carrying
            this.ReturnAnyFlags(killed);
        }

        public override bool AreTeamMembers(Mobile from, Mobile target)
        {
            if (from == null || target == null)
                return false;

            int frommember = 0;
            int targetmember = 0;

            // go through each teams members list and determine whether the players are on any team list
            if (this.m_Participants != null)
            {
                foreach (ChallengeEntry entry in this.m_Participants)
                {
                    if (!(entry.Status == ChallengeStatus.Active))
                        continue;

                    Mobile m = entry.Participant;

                    if (m == from)
                    {
                        frommember = entry.Team;
                    }
                    if (m == target)
                    {
                        targetmember = entry.Team;
                    }
                }
            }

            return (frommember == targetmember && frommember != 0 && targetmember != 0);
        }

        public override bool AreChallengers(Mobile from, Mobile target)
        {
            if (from == null || target == null)
                return false;

            int frommember = 0;
            int targetmember = 0;

            // go through each teams members list and determine whether the players are on any team list
            if (this.m_Participants != null)
            {
                foreach (ChallengeEntry entry in this.m_Participants)
                {
                    if (!(entry.Status == ChallengeStatus.Active))
                        continue;

                    Mobile m = entry.Participant;

                    if (m == from)
                    {
                        frommember = entry.Team;
                    }
                    if (m == target)
                    {
                        targetmember = entry.Team;
                    }
                }
            }

            return (frommember != targetmember && frommember != 0 && targetmember != 0);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            
            // save the home base list
            if (this.HomeBases != null)
            {
                writer.Write(this.HomeBases.Count);
                foreach (CTFBase b in this.HomeBases)
                {
                    writer.Write(b);
                }
            }
            else
            {
                writer.Write((int)0);
            }

            writer.Write(this.m_Challenger);
            writer.Write(this.m_GameLocked);
            writer.Write(this.m_GameInProgress);
            writer.Write(this.m_TotalPurse);
            writer.Write(this.m_EntryFee);
            writer.Write(this.m_ArenaSize);
            writer.Write(this.m_TargetScore);
            writer.Write(this.m_MatchLength);

            if (this.GameTimer != null && this.GameTimer.Running)
            {
                writer.Write(DateTime.UtcNow - this.m_MatchStart);
            }
            else
            {
                writer.Write(TimeSpan.Zero);
            }
            
            writer.Write(this.m_MatchEnd);

            if (this.Participants != null)
            {
                writer.Write(this.Participants.Count);

                foreach (ChallengeEntry entry in this.Participants)
                {
                    writer.Write(entry.Participant);
                    writer.Write(entry.Status.ToString());
                    writer.Write(entry.Accepted);
                    writer.Write(entry.PageBeingViewed);
                    writer.Write(entry.Score);
                    writer.Write(entry.Winner);
                    writer.Write(entry.Team);
                }
            }
            else
            {
                writer.Write((int)0);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch(version)
            {
                case 0:
                    int count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        CTFBase b = reader.ReadItem() as CTFBase;
                        this.HomeBases.Add(b);
                    }
                    this.m_Challenger = reader.ReadMobile();

                    this.m_Organizers.Add(this.m_Challenger);

                    this.m_GameLocked = reader.ReadBool();
                    this.m_GameInProgress = reader.ReadBool();
                    this.m_TotalPurse = reader.ReadInt();
                    this.m_EntryFee = reader.ReadInt();
                    this.m_ArenaSize = reader.ReadInt();
                    this.m_TargetScore = reader.ReadInt();
                    this.m_MatchLength = reader.ReadTimeSpan();

                    TimeSpan elapsed = reader.ReadTimeSpan();

                    if (elapsed > TimeSpan.Zero)
                    {
                        this.m_MatchStart = DateTime.UtcNow - elapsed;
                    }
                
                    this.m_MatchEnd = reader.ReadDateTime();

                    count = reader.ReadInt();
                    for (int i = 0; i < count; i++)
                    {
                        ChallengeEntry entry = new ChallengeEntry();
                        entry.Participant = reader.ReadMobile();
                        string sname = reader.ReadString();
                        // look up the enum by name
                        ChallengeStatus status = ChallengeStatus.None;
                        try
                        {
                            status = (ChallengeStatus)Enum.Parse(typeof(ChallengeStatus), sname);
                        }
                        catch
                        {
                        }
                        entry.Status = status;
                        entry.Accepted = reader.ReadBool();
                        entry.PageBeingViewed = reader.ReadInt();
                        entry.Score = reader.ReadInt();
                        entry.Winner = reader.ReadBool();
                        entry.Team = reader.ReadInt();
                    
                        this.Participants.Add(entry);
                    }
                    break;
            }
            
            if (this.GameCompleted)
                Timer.DelayCall(this.PostGameDecayTime, new TimerCallback(Delete));

            // start the challenge timer
            this.StartChallengeTimer();
            
            this.SetNameHue();
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendGump(new CTFGump(this, from));
        }

        public class ChallengeEntry : BaseChallengeEntry
        {
            public ChallengeEntry(Mobile m, int team)
                : base(m)
            {
                this.Team = team;
            }

            public ChallengeEntry(Mobile m)
                : base(m)
            {
            }

            public ChallengeEntry()
                : base()
            {
            }
        }
    }
}