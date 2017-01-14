using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using System.Collections;
using Server.Targeting;
using Server.Regions;
using Server.Engines.XmlSpawner2;

/*
** DeathmatchGauntlet
** ArteGordon
** updated 12/05/04
**
** used to set up a Deathmatch pvp challenge game through the XmlPoints system.
*/

namespace Server.Items
{
    public class DeathmatchGauntlet : BaseChallengeGame
    {

		public class ChallengeEntry : BaseChallengeEntry
		{
            public ChallengeEntry(Mobile m, int team) : base( m)
            {
                Team = team;
            }
            
            public ChallengeEntry(Mobile m) : base (m)
            {
            }
            
            public ChallengeEntry() : base ()
            {
            }
		}

		private static TimeSpan MaximumOutOfBoundsDuration = TimeSpan.FromSeconds(15);    // maximum time allowed out of bounds before disqualification

        private static TimeSpan MaximumOfflineDuration = TimeSpan.FromSeconds(60);    // maximum time allowed offline before disqualification

        private static TimeSpan MaximumHiddenDuration = TimeSpan.FromSeconds(10);    // maximum time allowed hidden before disqualification
               
        private static TimeSpan RespawnTime = TimeSpan.FromSeconds(6);    // delay until autores if autores is enabled

        public static bool OnlyInChallengeGameRegion = false;

        private Mobile m_Challenger;

        private ArrayList m_Participants = new ArrayList();
        
        private ArrayList m_Organizers = new ArrayList();
        
        private bool m_GameLocked;

        private bool m_GameInProgress;

        private int m_TotalPurse;
        
        private int m_EntryFee;
        
        private int m_TargetScore = 10;                                 // default target score to end match is 10
        
        private DateTime m_MatchStart;

        private TimeSpan m_MatchLength = TimeSpan.FromMinutes(10);      // default match length is 10 mins

        private int m_ArenaSize = 0;        // maximum distance from the challenge gauntlet allowed before disqualification.  Zero is unlimited range

        // how long before the gauntlet decays if a gauntlet is dropped but never started
        public override TimeSpan DecayTime { get{ return TimeSpan.FromMinutes( 15 ); } }  // this will apply to the setup

        public override ArrayList Organizers { get { return m_Organizers; } }

        public override bool AllowPoints { get{ return false; } }   // determines whether kills during the game will award points.  If this is false, UseKillDelay is ignored

        public override bool UseKillDelay { get{ return true; } }   // determines whether the normal delay between kills of the same player for points is enforced

        public bool AutoRes { get { return true; } }            // determines whether players auto res after being killed

        public bool AllowOnlyInChallengeRegions { get { return false; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public TimeSpan MatchLength { get{ return m_MatchLength; } set { m_MatchLength = value; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime MatchStart { get{ return m_MatchStart; } set { m_MatchStart = value; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public override Mobile Challenger { get{ return m_Challenger; } set { m_Challenger = value; } }

        public override bool GameInProgress { get{ return m_GameInProgress; } set { m_GameInProgress = value; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public override bool GameCompleted { get{ return !m_GameInProgress && m_GameLocked; } }

        public override bool GameLocked { get{ return m_GameLocked; } set {m_GameLocked = value; }}
        
        [CommandProperty( AccessLevel.GameMaster )]
        public override int ArenaSize { get{ return m_ArenaSize; } set { m_ArenaSize = value; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public int TargetScore { get{ return m_TargetScore; } set { m_TargetScore = value; } }

        public override ArrayList Participants { get{ return m_Participants; } set { m_Participants = value; } }

        public override int TotalPurse { get { return m_TotalPurse; } set { m_TotalPurse = value; } }

        public override int EntryFee { get { return m_EntryFee; } set { m_EntryFee = value; } }

        public override bool InsuranceIsFree(Mobile from, Mobile awardto)
        {
            return true;
        }

		public override void OnTick()
		{
		
            CheckForDisqualification();

            if(MatchLength > TimeSpan.Zero && DateTime.UtcNow >= MatchStart + MatchLength)
            {
                CheckForGameEnd();
            } else
            // count down the last 10 seconds
            if(MatchLength > TimeSpan.Zero && DateTime.UtcNow >= MatchStart + MatchLength - TimeSpan.FromSeconds(10))
            {
                GameBroadcast((MatchStart + MatchLength - DateTime.UtcNow).ToString());
            }
		}

        public override void StartGame()
        {
            base.StartGame();
            
            MatchStart = DateTime.UtcNow;
        }

		public void CheckForDisqualification()
		{
		
            if(Participants == null || !GameInProgress) return;
            
             bool statuschange = false;

            foreach(ChallengeEntry entry in Participants)
            {
                if(entry.Participant == null || entry.Status != ChallengeStatus.Active) continue;

                bool hadcaution = (entry.Caution != ChallengeStatus.None);

                // and a map check
                if(entry.Participant.Map != Map)
                {
                    // check to see if they are offline
                    if(entry.Participant.Map == Map.Internal)
                    {
                        // then give them a little time to return before disqualification
                        if(entry.Caution == ChallengeStatus.Offline)
                        {
                            // were previously out of bounds so check for disqualification
                            // check to see how long they have been out of bounds
                            if(DateTime.UtcNow - entry.LastCaution > MaximumOfflineDuration)
                            {
                                // penalize them
                                SubtractScore(entry);
                                entry.LastCaution  = DateTime.UtcNow;
                            }
                        } else
                        {
                            entry.LastCaution  = DateTime.UtcNow;
                            statuschange = true;
                        }
    
                        entry.Caution = ChallengeStatus.Offline;

                    } else
                    {
                        // changing to any other map results in instant
                        // teleport back to the gauntlet
                        // and point loss
                        RespawnWithPenalty(entry);
                        entry.Caution = ChallengeStatus.None;
                    }
                    

                } else
                // make a range check
                if(m_ArenaSize > 0 && !Utility.InRange(entry.Participant.Location, Location, m_ArenaSize)
                || (IsInChallengeGameRegion && !(Region.Find(entry.Participant.Location, entry.Participant.Map) is ChallengeGameRegion)))
                {
                    if(entry.Caution == ChallengeStatus.OutOfBounds)
                    {
                        // were previously out of bounds so check for disqualification
                        // check to see how long they have been out of bounds
                        if(DateTime.UtcNow - entry.LastCaution > MaximumOutOfBoundsDuration)
                        {
                            // teleport them back to the gauntlet
                            RespawnWithPenalty(entry);
                            GameBroadcast(100401, entry.Participant.Name);  // "{0} was penalized."
                            entry.Caution = ChallengeStatus.None;
                            statuschange = true;
                        }
                    } else
                    {
                        entry.LastCaution  = DateTime.UtcNow;
                        // inform the player
                        XmlPoints.SendText(entry.Participant, 100309, MaximumOutOfBoundsDuration.TotalSeconds);  // "You are out of bounds!  You have {0} seconds to return"
                        statuschange = true;
                    }

                    entry.Caution = ChallengeStatus.OutOfBounds;
                    

                } else
                // make a hiding check
                if(entry.Participant.Hidden)
                {
                    if(entry.Caution == ChallengeStatus.Hidden)
                    {
                        // were previously hidden so check for disqualification
                        // check to see how long they have hidden
                        if(DateTime.UtcNow - entry.LastCaution > MaximumHiddenDuration)
                        {
                            // penalize them
                            SubtractScore(entry);
                            entry.Participant.Hidden = false;
                            GameBroadcast(100401, entry.Participant.Name);  // "{0} was penalized."
                            entry.Caution = ChallengeStatus.None;
                            statuschange = true;
                        }
                    } else
                    {
                        entry.LastCaution  = DateTime.UtcNow;
                        // inform the player
                        XmlPoints.SendText(entry.Participant, 100310, MaximumHiddenDuration.TotalSeconds); // "You have {0} seconds become unhidden"
                        statuschange = true;
                    }

                    entry.Caution = ChallengeStatus.Hidden;
                    

                } else
                {
                    entry.Caution = ChallengeStatus.None;
                }

                if(hadcaution && entry.Caution == ChallengeStatus.None)
                    statuschange = true;

            }
            
            if(statuschange)
            {
                // update gumps with the new status
                DeathmatchGump.RefreshAllGumps(this, false);
            }

            // it is possible that the game could end like this so check
            CheckForGameEnd();



		}


		public override void CheckForGameEnd()
		{
		
            if(Participants == null || !GameInProgress) return;

            ArrayList winner = new ArrayList();

            int leftstanding = 0;
            IChallengeEntry lastentry = null;
            int maxscore = -99999;

            // has anyone reached the target score
            foreach(IChallengeEntry entry in Participants)
            {
                if(entry.Status == ChallengeStatus.Active)
                {
                    if(TargetScore > 0 && entry.Score >= TargetScore)
                    {
                        winner.Add(entry);
                        entry.Winner = true;
                    }
                    
                    if(entry.Score >= maxscore)
                    {
                        maxscore = entry.Score;
                    }
                    leftstanding++;
                    lastentry = entry;
                }
            }
            
            // if only one is left then they are the winner
            if(leftstanding == 1 && winner.Count == 0)
            {
                winner.Add(lastentry);
                lastentry.Winner = true;
            }
                
            if(winner.Count == 0 && MatchLength > TimeSpan.Zero && (DateTime.UtcNow >= MatchStart + MatchLength))
            {
                // find the highest score
                // has anyone reached the target score

                foreach(IChallengeEntry entry in Participants)
                {
                    if(entry.Status == ChallengeStatus.Active)
                    {
                        if(entry.Score >= maxscore)
                        {
                            winner.Add(entry);
                            entry.Winner = true;
                        }
                    }
                }
            }

            // and then check to see if this is the Deathmatch
            if(winner.Count > 0)
            {
                // declare the winner(s) and end the game
                foreach(IChallengeEntry entry in winner)
                {
                    if(entry.Participant != null)
                    {
                        XmlPoints.SendText(entry.Participant, 100311, ChallengeName);  // "You have won {0}"
                        GameBroadcast( 100312, entry.Participant.Name); // "The winner is {0}"
                        AwardWinnings(entry.Participant, TotalPurse/winner.Count);
                    }
                }

                EndGame();
                DeathmatchGump.RefreshAllGumps(this, true);
            }

		}
		
		public void SubtractScore(ChallengeEntry entry)
		{
            if(entry == null) return;
            
            entry.Score--;

            // refresh the gumps
            DeathmatchGump.RefreshAllGumps(this, false);
		}
		
		public void AddScore(ChallengeEntry entry)
		{
            if(entry == null) return;
            
            entry.Score++;
            
            // refresh the gumps
            DeathmatchGump.RefreshAllGumps(this, false);
		}
		
		public void RespawnWithPenalty(ChallengeEntry entry)
		{
            if(entry == null) return;
            
            SubtractScore(entry);
            
            // move the participant to the gauntlet
            if(entry.Participant != null)
            {
                entry.Participant.MoveToWorld(this.Location, this.Map);
                entry.Participant.PlaySound( 0x214 );
				entry.Participant.FixedEffect( 0x376A, 10, 16 );
				GameBroadcast(100401, entry.Participant.Name);  // "{0} was penalized."
            }
		}

        public override void OnPlayerKilled(Mobile killer, Mobile killed)
        {
            if(killed == null) return;

            if(AutoRes)
            {
                // prepare the autores callback
                    Timer.DelayCall( RespawnTime, new TimerStateCallback( XmlPoints.AutoRes_Callback ),
                    new object[]{ killed, true } );
            }

			// add 15 seconds of res kill protection
			//XmlAttach.AttachTo(killed, new XmlBless(15.0));


            // find the player in the participants list and announce it
            if(m_Participants != null)
            {

                foreach(ChallengeEntry entry in m_Participants)
                {
                    if(entry.Status == ChallengeStatus.Active && entry.Participant == killed)
                    {
                        GameBroadcast(100314, killed.Name); // "{0} has been killed"
                        SubtractScore(entry);
                    }
                }
            }

            // see if the game is over
            CheckForGameEnd();
        }
        
        public override void OnKillPlayer(Mobile killer, Mobile killed)
        {
            if(killer == null) return;

            // find the player in the participants list and announce it
            if(m_Participants != null)
            {

                foreach(ChallengeEntry entry in m_Participants)
                {
                    if(entry.Status == ChallengeStatus.Active && entry.Participant == killer)
                    {
                        AddScore(entry);
                    }
                }
            }
        }

        public override bool AreTeamMembers(Mobile from, Mobile target)
        {
            // there are no teams, its every man for himself
            if(from == target) return true;
            return false;
        }

        public override bool AreChallengers(Mobile from, Mobile target)
        {
            // everyone participant is a challenger to everyone other participant, so just being a participant
            // makes you a challenger
            return(AreInGame(from) && AreInGame(target));
        }

        public DeathmatchGauntlet(Mobile challenger) : base( 0x1414 )
        {
            m_Challenger = challenger;
            
            m_Organizers.Add(challenger);

            // check for points attachments
            XmlPoints afrom = (XmlPoints)XmlAttach.FindAttachment(challenger, typeof(XmlPoints));

            Movable = false;

            Hue = 33;

            if(challenger == null || afrom == null || afrom.Deleted)
            {
                Delete();
            } else
            {
                Name = XmlPoints.SystemText(100400) + " " + String.Format(XmlPoints.SystemText(100315), challenger.Name); // "Challenge by {0}"

            }
        }


        public DeathmatchGauntlet( Serial serial ) : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version

            writer.Write(m_Challenger);
            writer.Write(m_GameLocked);
            writer.Write(m_GameInProgress);
            writer.Write(m_TotalPurse);
            writer.Write(m_EntryFee);
            writer.Write(m_ArenaSize);
            writer.Write(m_TargetScore);
            writer.Write(m_MatchLength);
            
            if(GameTimer != null && GameTimer.Running)
            {
                writer.Write(DateTime.UtcNow - m_MatchStart);
            } else
            {
                writer.Write(TimeSpan.Zero);
            }

            if(Participants != null)
            {
                writer.Write(Participants.Count);

                foreach(ChallengeEntry entry in Participants)
                {
                    writer.Write(entry.Participant);
                    writer.Write(entry.Status.ToString());
                    writer.Write(entry.Accepted);
                    writer.Write(entry.PageBeingViewed);
                    writer.Write(entry.Score);
                    writer.Write(entry.Winner);
                }
            } else
            {
                writer.Write((int)0);
            }

        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch(version)
            {
            case 0:
                m_Challenger = reader.ReadMobile();
                
                m_Organizers.Add(m_Challenger);

                m_GameLocked = reader.ReadBool();
                m_GameInProgress = reader.ReadBool();
                m_TotalPurse = reader.ReadInt();
                m_EntryFee = reader.ReadInt();
                m_ArenaSize = reader.ReadInt();
                m_TargetScore = reader.ReadInt();
                m_MatchLength = reader.ReadTimeSpan();
                
                TimeSpan elapsed = reader.ReadTimeSpan();
                
                if(elapsed > TimeSpan.Zero)
                {
                    m_MatchStart = DateTime.UtcNow - elapsed;
                }
                
                int count = reader.ReadInt();
                for(int i = 0;i<count;i++)
                {
                    ChallengeEntry entry = new ChallengeEntry();
                    entry.Participant = reader.ReadMobile();
                    string sname = reader.ReadString();
                    // look up the enum by name
                    ChallengeStatus status = ChallengeStatus.None;
                    try{
                    status = (ChallengeStatus)Enum.Parse(typeof(ChallengeStatus), sname);
                    } catch{}
                    entry.Status = status;
                    entry.Accepted = reader.ReadBool();
                    entry.PageBeingViewed = reader.ReadInt();
                    entry.Score = reader.ReadInt();
                    entry.Winner = reader.ReadBool();
                    
                    Participants.Add(entry);
                }
                break;
            }
            
             if(GameCompleted)
                Timer.DelayCall( PostGameDecayTime, new TimerCallback( Delete ) );
            
            // start the challenge timer
            StartChallengeTimer();
        }

        public override void OnDoubleClick( Mobile from )
        {

            from.SendGump( new DeathmatchGump( this, from ) );

        }
    }
}
