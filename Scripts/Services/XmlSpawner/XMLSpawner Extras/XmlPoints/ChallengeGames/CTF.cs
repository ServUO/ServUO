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

        public int Team { get{ return m_Team; } set { m_Team = value; } }

        public CTFFlag Flag { get{ return m_Flag; } set { m_Flag = value; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public int ProximityRange { get{ return m_ProximityRange; } set { m_ProximityRange = value; } }

        public bool HasFlag { get { return m_HasFlag; } set { m_HasFlag = value;} }

		public CTFBase(CTFGauntlet gauntlet, int team) : base( 0x1183 )
		{
            Movable = false;
            Hue = BaseChallengeGame.TeamColor(team);
            Team = team;
            Name = String.Format("Team {0} Base", team);
            m_gauntlet = gauntlet;

            // add the flag

            Flag = new CTFFlag(this, team);
            Flag.HomeBase = this;
            HasFlag = true;
		}

		public CTFBase( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write(Team);
			writer.Write(m_ProximityRange);
			writer.Write(m_Flag);
			writer.Write(m_gauntlet);
			writer.Write(m_HasFlag);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			Team = reader.ReadInt();
			ProximityRange = reader.ReadInt();
			Flag = reader.ReadItem() as CTFFlag;
			m_gauntlet = reader.ReadItem() as CTFGauntlet;
			m_HasFlag = reader.ReadBool();
		}

		public override void OnDelete()
		{
            // delete any flag associated with the base
            if(m_Flag != null)
                m_Flag.Delete();

            base.OnDelete();
		}

		public override void OnLocationChange( Point3D oldLocation )
        {
            // set the flag location
            PlaceFlagAtBase();
        }
        
        public void PlaceFlagAtBase()
        {
            if(m_Flag != null)
            {
                m_Flag.MoveToWorld(new Point3D(Location.X + 1, Location.Y, Location.Z + 4), Map);
            }
        }
        
        public override void OnMapChange( )
        {
            // set the flag location
            PlaceFlagAtBase();
        }
        
        public void ReturnFlag()
        {
            ReturnFlag(true);
        }

        public void ReturnFlag(bool verbose)
        {
            if(Flag == null) return;

            PlaceFlagAtBase();
            HasFlag = true;
            if(m_gauntlet != null && verbose)
            {
                m_gauntlet.GameBroadcast(100419,Team); // "Team {0} flag has been returned to base"
            }

        }


		public override bool HandlesOnMovement { get{ return m_gauntlet != null; } }

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
            if(m == null || m_gauntlet == null) return;
            
            if(m == null || m.AccessLevel > AccessLevel.Player) return;

            // look for players within range of the base
            // check to see if player is within range of the spawner
  			if ((this.Parent == null) && Utility.InRange( m.Location, this.Location, m_ProximityRange ) )
            {

                CTFGauntlet.ChallengeEntry entry = m_gauntlet.GetParticipant(m) as CTFGauntlet.ChallengeEntry;

                if(entry == null) return;
                
                bool carryingflag = false;
                // is the player carrying a flag?
                foreach(CTFBase b in m_gauntlet.HomeBases)
                {
                    if(b != null && !b.Deleted && b.Flag != null && b.Flag.RootParent == m)
                    {
                        carryingflag = true;
                        break;
                    }
                }

                // if the player is on an opposing team and the flag is at the base and the player doesnt already
                // have a flag then give them the flag
                if(entry.Team != Team && HasFlag && !carryingflag)
                {
                    m.AddToBackpack(m_Flag);
                    HasFlag = false;
                    m_gauntlet.GameBroadcast(100420, entry.Team, Team); // "Team {0} has the Team {1} flag"
                    m_gauntlet.GameBroadcastSound(513);
                } else
                if(entry.Team == Team)
                {

                    // if the player has an opposing teams flag then give them a point and return the flag
                    foreach(CTFBase b in m_gauntlet.HomeBases)
                    {
                        if(b != null && !b.Deleted && b.Flag != null && b.Flag.RootParent == m && b.Team != entry.Team)
                        {
                            m_gauntlet.GameBroadcast(100421,entry.Team);  // "Team {0} has scored"
                            m_gauntlet.AddScore(entry);

                            Effects.SendTargetParticles( entry.Participant, 0x375A, 35, 20, BaseChallengeGame.TeamColor(entry.Team), 0x00, 9502,
                                (EffectLayer)255, 0x100 );
                            // play the score sound
                            m_gauntlet.ScoreSound(entry.Team);

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

        public CTFBase  HomeBase;

		public CTFFlag(CTFBase homebase, int team) : base( 0x161D )
		{
            Hue = BaseChallengeGame.TeamColor(team);;
            Name = String.Format("Team {0} Flag", team);
            HomeBase = homebase;
		}

		public CTFFlag( Serial serial ) : base( serial )
		{
		}

		public override bool OnDroppedInto( Mobile from, Container target, Point3D p)
        {
            // allow movement within a players backpack
            if(from != null && from.Backpack == target)
            {
                return base.OnDroppedInto(from, target, p);
            }

            return false;
        }
        
        public override bool OnDroppedOnto( Mobile from, Item target )
        {
            return false;
        }
        
        public override bool OnDroppedToMobile( Mobile from, Mobile target )
        {
            return false;
        }

		public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            // only allow staff to pick it up when at a base
            if((from != null && from.AccessLevel > AccessLevel.Player) || RootParent != null)
            {
				return base.CheckLift(from, item, ref reject);
            }
            return false;
        }

        public override bool OnDroppedToWorld(Mobile from,Point3D point)
        {
            return false;
        }


		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( HomeBase );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			HomeBase = reader.ReadItem() as CTFBase;
		}
    }


    public class CTFGauntlet : BaseChallengeGame
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

        public static bool OnlyInChallengeGameRegion = false;           // if this is true, then the game can only be set up in a challenge game region

        private Mobile m_Challenger;

        private ArrayList m_Organizers = new ArrayList();

        private ArrayList m_Participants = new ArrayList();
        
        private bool m_GameLocked;

        private bool m_GameInProgress;

        private int m_TotalPurse;
        
        private int m_EntryFee;

        private int m_TargetScore = 10;                                 // default target score to end match is 10

        private DateTime m_MatchStart;
        
        private DateTime m_MatchEnd;

        private TimeSpan m_MatchLength = TimeSpan.FromMinutes(10);      // default match length is 10 mins

        private int m_ArenaSize = 0;        // maximum distance from the challenge gauntlet allowed before disqualification.  Zero is unlimited range

        private int m_Winner = 0;
        
        private ArrayList m_HomeBases = new ArrayList();
        
        public ArrayList HomeBases { get{ return m_HomeBases; } set { m_HomeBases = value; } }

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
        public DateTime MatchEnd { get{ return m_MatchEnd; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public override Mobile Challenger { get{ return m_Challenger; } set { m_Challenger = value; } }

        public override bool GameLocked { get{ return m_GameLocked; } set { m_GameLocked = value; }}

        public override bool GameInProgress { get{ return m_GameInProgress; } set { m_GameInProgress = value; }}

        [CommandProperty( AccessLevel.GameMaster )]
        public override bool GameCompleted { get{ return !m_GameInProgress && m_GameLocked; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public override int ArenaSize { get{ return m_ArenaSize; } set { m_ArenaSize = value; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public int TargetScore { get{ return m_TargetScore; } set { m_TargetScore = value; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Winner { get{ return m_Winner; } set { m_Winner = value; } }

        public override ArrayList Participants { get{ return m_Participants; } set { m_Participants = value; } }

        public override int TotalPurse { get { return m_TotalPurse; } set { m_TotalPurse = value; } }

        public override int EntryFee { get { return m_EntryFee; } set { m_EntryFee = value; } }

        public override bool InsuranceIsFree(Mobile from, Mobile awardto)
        {
            return true;
        }

        public void ScoreSound(int team)
		{
            foreach(ChallengeEntry entry in Participants)
            {
                if(entry.Participant == null || entry.Status != ChallengeStatus.Active) continue;

                if(entry.Team == team)
                {
                    // play the team scored sound
                    entry.Participant.PlaySound(503);
                } else
                {
                    // play the opponent scored sound
                    entry.Participant.PlaySound(855 /*700*/);
                }
            }
		}


        public override void OnTick()
		{

            CheckForDisqualification();

            // check for anyone carrying flags
            if(HomeBases != null)
            {
                ArrayList dlist = null;

                foreach(CTFBase b in HomeBases)
                {
                    if(b == null || b.Deleted)
                    {
                        if(dlist == null)
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
							IChallengeEntry entry = GetParticipant(m);

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
						} else
							// if the flag somehow ends up on the ground, send it back to the base
							if (!b.HasFlag)
							{
								b.ReturnFlag();
							}
					}
	
                }

                if(dlist != null)
                {
                    foreach(CTFBase b in dlist)
                        HomeBases.Remove(b);
                }
            }
		}

		public void ReturnAnyFlags(Mobile m)
		{
            // check for anyone carrying flags
            if(HomeBases != null)
            {
                foreach(CTFBase b in HomeBases)
                {
                    if(!b.Deleted && b.Flag != null && !b.Flag.Deleted)
                    {
                        if(b.Flag.RootParent is Mobile)
                        {
                            if(m == b.Flag.RootParent as Mobile)
                            {
                                b.ReturnFlag();
                            }
                        } else
                        if(b.Flag.RootParent is Corpse)
                        {
                            if(m == ((Corpse)(b.Flag.RootParent)).Owner)
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
                                // return any flag they might be carrying
                                ReturnAnyFlags(entry.Participant);
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
                        // changing to any other map results in
                        // return of any flag they might be carrying
                        ReturnAnyFlags(entry.Participant);
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
                            // return any flag they might be carrying
                            ReturnAnyFlags(entry.Participant);
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
                            // return any flag they might be carrying
                            ReturnAnyFlags(entry.Participant);
                            entry.Participant.Hidden = false;
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
                CTFGump.RefreshAllGumps(this, false);
            }

            // it is possible that the game could end like this so check
            CheckForGameEnd();
		}
		
		public CTFBase FindBase(int team)
        {
            // go through the current bases and see if there is one for this team
            if(HomeBases != null)
            {
                foreach(CTFBase b in HomeBases)
                {
                    if(b.Team == team)
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
            if(HomeBases != null)
            {
                foreach(CTFBase b in HomeBases)
                {
                    b.Delete();
                }
                
                HomeBases = new ArrayList();
            }
        }

        public override void OnDelete()
        {
            ClearNameHue();

            // remove all bases
            DeleteBases();

            base.OnDelete();

        }

		public override void EndGame()
		{
            ClearNameHue();
            
            DeleteBases();
            
            m_MatchEnd = DateTime.UtcNow;

            base.EndGame();

		}

        public override void StartGame()
        {
            base.StartGame();

            MatchStart = DateTime.UtcNow;

            SetNameHue();
			
			// teleport to base
			TeleportPlayersToBase();
        }

		public void TeleportPlayersToBase()
		{
			// teleport players to the base
			if (Participants != null)
			{
				foreach (ChallengeEntry entry in Participants)
				{
					CTFBase teambase = FindBase(entry.Team);

					if (entry.Participant != null && teambase != null)
					{
						entry.Participant.MoveToWorld(teambase.Location, teambase.Map);
					}
				}
			}
		}

		public override void CheckForGameEnd()
		{

            if(Participants == null || !GameInProgress) return;

            ArrayList winner = new ArrayList();

            ArrayList teams = GetTeams();

            int leftstanding = 0;

            int maxscore = -99999;

            // has any team reached the target score
            TeamInfo lastt = null;

            foreach(TeamInfo t in teams)
            {

                if(!HasValidMembers(t)) continue;

                if(TargetScore > 0 && t.Score >= TargetScore)
                {
                        winner.Add(t);
                        t.Winner = true;
                }

                if(t.Score >= maxscore)
                {
                    maxscore = t.Score;
                }
                leftstanding++;
                lastt = t;
            }

            // check to make sure the team hasnt been disqualified

            // if only one is left then they are the winner
            if(leftstanding == 1 && winner.Count == 0)
            {
                winner.Add(lastt);
                lastt.Winner = true;
            }

            if(winner.Count == 0 && MatchLength > TimeSpan.Zero && (DateTime.UtcNow >= MatchStart + MatchLength))
            {
                // find the highest score
                // has anyone reached the target score

                foreach(TeamInfo t in teams)
                {

                    if(!HasValidMembers(t)) continue;

                    if(t.Score >= maxscore)
                    {
                        winner.Add(t);
                        t.Winner = true;
                    }
                }
            }

            // and then check to see if this is the CTF
            if(winner.Count > 0)
            {

                // declare the winner(s) and end the game
                foreach(TeamInfo t in winner)
                {
                    // flag all members as winners
                    foreach(IChallengeEntry entry in t.Members)
                        entry.Winner = true;

                    GameBroadcast( 100414, t.ID);  // "Team {0} is the winner!"

                    GameBroadcastSound(744);
                    AwardTeamWinnings(t.ID, TotalPurse/winner.Count);

                    if(winner.Count == 1) Winner = t.ID;
                }

                RefreshAllNoto();

                EndGame();
                CTFGump.RefreshAllGumps(this, true);
            }

		}
		
		public void SubtractScore(ChallengeEntry entry)
		{
            if(entry == null) return;

            entry.Score--;

            // refresh the gumps
            CTFGump.RefreshAllGumps(this, false);
		}

		public void AddScore(ChallengeEntry entry)
		{
            if(entry == null) return;

            entry.Score++;
            
            // refresh the gumps
            CTFGump.RefreshAllGumps(this, false);
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

            // return any flag they were carrying
            ReturnAnyFlags(killed);
        }

        public override bool AreTeamMembers(Mobile from, Mobile target)
        {
            if(from == null || target == null) return false;

            int frommember = 0;
            int targetmember = 0;

            // go through each teams members list and determine whether the players are on any team list
            if(m_Participants != null)
            {
                foreach(ChallengeEntry entry in m_Participants)
                {
                    if(!(entry.Status == ChallengeStatus.Active)) continue;

                    Mobile m = entry.Participant;

                    if(m == from)
                    {
                        frommember = entry.Team;
                    }
                    if(m == target)
                    {
                        targetmember = entry.Team;
                    }
                }
            }

            return (frommember == targetmember && frommember != 0 && targetmember != 0);

        }

        public override bool AreChallengers(Mobile from, Mobile target)
        {
            if(from == null || target == null) return false;

            int frommember = 0;
            int targetmember = 0;

            // go through each teams members list and determine whether the players are on any team list
            if(m_Participants != null)
            {
                foreach(ChallengeEntry entry in m_Participants)
                {
                    if(!(entry.Status == ChallengeStatus.Active)) continue;

                    Mobile m = entry.Participant;

                    if(m == from)
                    {
                        frommember = entry.Team;
                    }
                    if(m == target)
                    {
                        targetmember = entry.Team;
                    }
                }
            }

            return (frommember != targetmember && frommember != 0 && targetmember != 0);

        }

        public CTFGauntlet(Mobile challenger) : base( 0x1414 )
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
                Name = XmlPoints.SystemText(100418) + " " + String.Format(XmlPoints.SystemText(100315), challenger.Name); // "Challenge by {0}"
            }
        }


        public CTFGauntlet( Serial serial ) : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 0 ); // version
            
            // save the home base list
            if(HomeBases != null)
            {
                writer.Write(HomeBases.Count);
                foreach(CTFBase b in HomeBases)
                {
                    writer.Write(b);
                }
            } else
            {
                writer.Write((int)0);
            }

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
            
            writer.Write(m_MatchEnd);

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
                    writer.Write(entry.Team);
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
                int count = reader.ReadInt();
                for(int i = 0; i< count; i++)
                {
                    CTFBase b = reader.ReadItem() as CTFBase;
                    HomeBases.Add(b);
                }
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
                
                m_MatchEnd = reader.ReadDateTime();

                count = reader.ReadInt();
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
                    entry.Team = reader.ReadInt();
                    
                    Participants.Add(entry);
                }
                break;
            }
            
             if(GameCompleted)
                Timer.DelayCall( PostGameDecayTime, new TimerCallback( Delete ) );

            // start the challenge timer
            StartChallengeTimer();
            
            SetNameHue();
        }

        public override void OnDoubleClick( Mobile from )
        {

            from.SendGump( new CTFGump( this, from ) );

        }
    }
}
