using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using System.Collections;
using Server.Targeting;
using Server.Engines.XmlSpawner2;

/*
** LastManStandingGauntlet
** ArteGordon
** updated 12/05/04
**
** used to set up a last man standing pvp challenge game through the XmlPoints system.
*/

namespace Server.Items
{
    public class LastManStandingGauntlet : BaseChallengeGame
    {


		public class ChallengeEntry : BaseChallengeEntry
		{

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
        
        private int m_ArenaSize = 0;        // maximum distance from the challenge gauntlet allowed before disqualification.  Zero is unlimited range

        private Mobile m_Winner;

        // how long before the gauntlet decays if a gauntlet is dropped but never started
        public override TimeSpan DecayTime { get{ return TimeSpan.FromMinutes( 15 ); } }  // this will apply to the setup

        public override ArrayList Organizers { get { return m_Organizers; } }

        public override bool AllowPoints { get{ return false; } }   // determines whether kills during the game will award points.  If this is false, UseKillDelay is ignored

        public override bool UseKillDelay { get{ return true; } }   // determines whether the normal delay between kills of the same player for points is enforced

        public bool AutoRes { get { return true; } }            // determines whether players auto res after being killed

        [CommandProperty( AccessLevel.GameMaster )]
        public override Mobile Challenger { get{ return m_Challenger; } set { m_Challenger = value; } }

        public override bool GameInProgress { get{ return m_GameInProgress; } set {m_GameInProgress = value; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public override bool GameCompleted { get{ return !m_GameInProgress && m_GameLocked; } }

        public override bool GameLocked { get{ return m_GameLocked; } set {m_GameLocked = value; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Winner { get{ return m_Winner; } set { m_Winner = value; } }

        public override ArrayList Participants { get{ return m_Participants; } set { m_Participants = value; } }

        public override int TotalPurse { get { return m_TotalPurse; } set { m_TotalPurse = value; } }

        public override int EntryFee { get { return m_EntryFee; } set { m_EntryFee = value; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public override int ArenaSize { get { return m_ArenaSize; } set { m_ArenaSize = value; } }

        public override bool InsuranceIsFree(Mobile from, Mobile awardto)
        {
            return true;
        }
                
        public override void OnTick()
		{
            CheckForDisqualification();
		}

		public void CheckForDisqualification()
		{
		
            if(Participants == null || !GameInProgress) return;

            bool statuschange = false;

            foreach(ChallengeEntry entry in Participants)
            {
                if(entry.Participant == null || entry.Status == ChallengeStatus.Forfeit || entry.Status == ChallengeStatus.Disqualified) continue;

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
                                entry.Status = ChallengeStatus.Disqualified;
                                GameBroadcast(100308, entry.Participant.Name);  // "{0} has been disqualified"
                                RefreshSymmetricNoto(entry.Participant);
                                statuschange = true;
                            }
                        } else
                        {
                            entry.LastCaution  = DateTime.UtcNow;
                            statuschange = true;
                        }
    
                        entry.Caution = ChallengeStatus.Offline;
                    } else
                    {
                        // changing to any other map is instant disqualification
                        entry.Status = ChallengeStatus.Disqualified;
                        GameBroadcast(100308, entry.Participant.Name);  // "{0} has been disqualified"
                        RefreshSymmetricNoto(entry.Participant);
                        statuschange = true;
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
                            entry.Status = ChallengeStatus.Disqualified;
                            GameBroadcast(100308, entry.Participant.Name);  // "{0} has been disqualified"
                            RefreshSymmetricNoto(entry.Participant);
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
                            entry.Status = ChallengeStatus.Disqualified;
                            GameBroadcast(100308, entry.Participant.Name);  // "{0} has been disqualified"
                            RefreshSymmetricNoto(entry.Participant);
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
                LastManStandingGump.RefreshAllGumps(this, false);
            }

            // it is possible that the game could end like this so check
            CheckForGameEnd();
		}

		public override void CheckForGameEnd()
		{

            if(Participants == null || !GameInProgress) return;

            int leftstanding = 0;
            Mobile winner = null;

            foreach(ChallengeEntry entry in Participants)
            {
                if(entry.Status == ChallengeStatus.Active)
                {
                    leftstanding++;
                    winner = entry.Participant;
                }
            }

            // and then check to see if this is the last man standing
            if(leftstanding == 1 && winner != null)
            {
                // declare the winner and end the game
                XmlPoints.SendText(winner, 100311, ChallengeName);  // "You have won {0}"
                Winner = winner;
                RefreshSymmetricNoto(winner);
                GameBroadcast( 100312, winner.Name); // "The winner is {0}"
                AwardWinnings(winner, TotalPurse);

                EndGame();
                LastManStandingGump.RefreshAllGumps(this, true);
            }
            if(leftstanding < 1)
            {
                // declare a tie and keep the fees
                GameBroadcast(100313);  // "The match is a draw"

                EndGame();
                LastManStandingGump.RefreshAllGumps(this, true);
            }
		}

		public override void OnPlayerKilled(Mobile killer, Mobile killed)
		{
			if (killed == null) return;

			/*
			// move the killed player and their corpse to a location
			// you have to replace x,y,z with some valid coordinates
			int x = this.Location.X + 30;
			int y = this.Location.Y + 30;
			int z = this.Location.Z;
			Point3D killedloc = new Point3D(x, y, z);

			ArrayList petlist = new ArrayList();

			foreach (Mobile m in killed.GetMobilesInRange(16))
			{
				if (m is BaseCreature && ((BaseCreature)m).ControlMaster == killed)
				{
					petlist.Add(m);
				}
			}

			// port the pets
			foreach (Mobile m in petlist)
			{
				m.MoveToWorld(killedloc, killed.Map);
			}

			// do the actual moving
			killed.MoveToWorld(killedloc, killed.Map);
			if (killed.Corpse != null)
				killed.Corpse.MoveToWorld(killedloc, killed.Map);
			*/

			if (AutoRes)
            {
                // prepare the autores callback
                    Timer.DelayCall( RespawnTime, new TimerStateCallback( XmlPoints.AutoRes_Callback ),
                    new object[]{ killed, false } );
            }

            // find the player in the participants list and set their status to Dead
            if(m_Participants != null)
            {
                foreach(ChallengeEntry entry in m_Participants)
                {
                    if(entry.Participant == killed && entry.Status != ChallengeStatus.Forfeit)
                    {
                        entry.Status = ChallengeStatus.Dead;
                        // clear up their noto
                        RefreshSymmetricNoto(killed);

                        GameBroadcast(100314, killed.Name); // "{0} has been killed"
                    }
                }
            }
            
            LastManStandingGump.RefreshAllGumps(this, true);

            // see if the game is over
            CheckForGameEnd();
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

        public LastManStandingGauntlet(Mobile challenger) : base( 0x1414 )
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
                Name = XmlPoints.SystemText(100302) + " " + String.Format(XmlPoints.SystemText(100315), challenger.Name); // "Challenge by {0}"

            }
        }


        public LastManStandingGauntlet( Serial serial ) : base( serial )
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
            writer.Write(m_Winner);

            if(Participants != null)
            {
                writer.Write(Participants.Count);

                foreach(ChallengeEntry entry in Participants)
                {
                    writer.Write(entry.Participant);
                    writer.Write(entry.Status.ToString());
                    writer.Write(entry.Accepted);
                    writer.Write(entry.PageBeingViewed);
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
                m_Winner = reader.ReadMobile();
                
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
                    
                    Participants.Add(entry);
                }
                break;
            }
            
             if(GameCompleted)
                Timer.DelayCall( PostGameDecayTime, new TimerCallback( Delete ) );
            
            StartChallengeTimer();
        }

        public override void OnDoubleClick( Mobile from )
        {

            from.SendGump( new LastManStandingGump( this, from ) );

        }
    }
}
