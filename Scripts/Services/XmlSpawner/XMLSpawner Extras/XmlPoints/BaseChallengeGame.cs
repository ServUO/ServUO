using System;
using System.IO;
using System.Xml;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using System.Collections;
using System.Reflection;
using Server.Targeting;
using Server.Gumps;
using System.Text;
using Server.Commands;
using Server.Commands.Generic;

namespace Server.Engines.XmlSpawner2
{
    public enum ChallengeStatus
	{
        None,
        Active,
        Dead,
        OutOfBounds,
        Offline,
        Forfeit,
        Hidden,
        Disqualified
	}
	
	public class TeamInfo
    {
        public int     ID;
        public int     NActive;
        public int     Score;
        public bool    Winner;
        public ArrayList Members = new ArrayList();

        public TeamInfo(int teamid)
        {
            ID = teamid;
        }
    }


    public interface IChallengeGame
    {
        bool AreInGame(Mobile from);
        bool AreTeamMembers(Mobile from, Mobile target);
        bool AreChallengers(Mobile from, Mobile target);
        void OnPlayerKilled(Mobile killer, Mobile killed);
        void OnKillPlayer(Mobile killer, Mobile killed);
        bool InsuranceIsFree(Mobile from, Mobile awardto);
        bool IsOrganizer(Mobile from);
        void GameBroadcast(string msg);
        void GameBroadcast(int msgindex);
        void GameBroadcast(int msgindex, object msgarg);
        void GameBroadcast(int msgindex, object msgarg, object msgarg2);
        void EndGame();
        void StartGame();
        void OnTick();
        bool ChallengeBeingCancelled{ get; }
        ArrayList Organizers { get; }
        bool GameInProgress { get; set; }
        bool GameCompleted { get; }
        bool GameLocked { get; set; }
        string ChallengeName { get; }
        bool UseKillDelay { get; }
        bool AllowPoints { get; }
        void OnDoubleClick(Mobile from);
        void OnDelete();
        void Delete();
        bool Deleted { get; }
        void GetProperties( ObjectPropertyList list );
        ArrayList Participants { get; set; }
    }

    public interface IChallengeEntry
	{
        Mobile  Participant {get; set; }
        ChallengeStatus Status {get; set; }
        ChallengeStatus Caution {get; set; }
        bool    Accepted {get; set; }
        DateTime LastCaution {get; set; }
        int PageBeingViewed {get; set; }
        int Team {get; set; }
        int Score { get; set; }
        bool Winner { get; set; }
	}

	public abstract class BaseChallengeEntry : IChallengeEntry
	{
        private Mobile  m_Participant;
        private ChallengeStatus m_Status;
        private ChallengeStatus m_Caution;
        private bool    m_Accepted;
        private DateTime m_LastCaution;
        private int m_PageBeingViewed;
        private int m_Score;
        private int m_Team;
        private bool m_Winner;


        public virtual Mobile Participant { get { return m_Participant; } set { m_Participant = value; }}
        public virtual ChallengeStatus Status { get { return m_Status; } set { m_Status = value; }}
        public virtual ChallengeStatus Caution { get { return m_Caution; } set { m_Caution = value; }}
        public virtual bool Accepted { get { return m_Accepted; } set { m_Accepted = value; }}
        public virtual DateTime LastCaution { get { return m_LastCaution; } set { m_LastCaution = value; }}
        public virtual int PageBeingViewed { get { return m_PageBeingViewed; } set { m_PageBeingViewed = value; }}
        public virtual int Score { get { return m_Score; } set { m_Score = value; }}
        public virtual int Team { get { return m_Team; } set { m_Team = value; }}
        public virtual bool Winner { get { return m_Winner; } set { m_Winner = value; }}

        public BaseChallengeEntry(Mobile m)
        {
            Participant = m;
            Status = ChallengeStatus.Active;
            Accepted = false;

        }

        public BaseChallengeEntry()
        {
        }
	}


    public abstract class BaseChallengeGame : Item, IChallengeGame
    {
        private ChallengeTimer m_Timer;

        public ChallengeTimer GameTimer { get { return m_Timer; } }

        private bool m_IsInChallengeGameRegion;
        
        public bool IsInChallengeGameRegion { get { return m_IsInChallengeGameRegion; } set { m_IsInChallengeGameRegion = value; } }

        // how long before the gauntlet is removed after a game is completed
        public virtual TimeSpan PostGameDecayTime { get { return TimeSpan.FromMinutes(5.0); }}
         
         // how long before the gauntlet decays if a gauntlet is dropped but never started
        public override TimeSpan DecayTime { get{ return TimeSpan.FromMinutes( 15 ); } }  // this will apply to the setup

        public override bool Decays { get { return !GameLocked; }}

        public abstract int EntryFee { get; set; }
        
        public abstract int TotalPurse { get; set; }
        
        public abstract int ArenaSize { get; set; }
        
        public abstract Mobile Challenger { get; set; }

        public virtual bool AreInGame(Mobile from)
        {
            if(from == null) return false;

            // go through each teams members list and determine whether the players are on any team list
            if(Participants != null)
            {
                foreach(IChallengeEntry entry in Participants)
                {
                    if(!(entry.Status == ChallengeStatus.Active)) continue;

                    if(entry.Participant == from)
                    {
                        return true;
                    }
                }
            }

            return false;

        }
        
        public abstract void CheckForGameEnd();

        public abstract bool AreTeamMembers(Mobile from, Mobile target);


        public abstract bool AreChallengers(Mobile from, Mobile target);


        public abstract void OnPlayerKilled(Mobile killer, Mobile killed);

        public virtual void OnKillPlayer(Mobile killer, Mobile killed)
        {
        }
        
        public virtual void SetupChallenge(Mobile from)
        {
        }

        public abstract void OnTick();

        public void ResetAcceptance()
		{
		    // go through the participant list and clear all acceptance flags
		    if(Participants != null)
		    {
                foreach(IChallengeEntry entry in Participants)
                {
                    entry.Accepted = false;
                }
            }
		}
		
		public void ClearArena()
		{
		
            if(ArenaSize <= 0) return;

            ArrayList mlist = new ArrayList();

            // who is currently within the arena
            IPooledEnumerable eable = GetMobilesInRange(ArenaSize);
            foreach( Mobile p in eable)
            {
                if(p == null) continue;

                IChallengeEntry entry = GetParticipant(p);

                // if this is not a current participant then move them
                if(entry == null)
                {
                    // prepare to move them off
                    mlist.Add(p);
                }
            }
            eable.Free();

            // move non-participants
            foreach( Mobile p in mlist)
            {
                for(int i = 0; i<10;i++)
                {
                    int x = Location.X + (ArenaSize + i)*(Utility.RandomBool() ? 1 : -1);
                    int y = Location.Y + (ArenaSize + i)*(Utility.RandomBool() ? 1 : -1);
                    int z = Map.GetAverageZ( x, y );
                    Point3D newloc = new Point3D(x,y,z);

                    if(XmlSpawner.IsValidMapLocation(newloc, p.Map))
                    {
                        p.MoveToWorld(newloc, p.Map);
                    }
                }
            }
		}
		
		
		public virtual bool HasEntryFee(Mobile m)
        {
                Container bank = m.BankBox;
                int total = 0;

                if(bank != null)
                {
    				Item[] goldlist = bank.FindItemsByType( typeof(Gold), true );

    				if( goldlist != null )
    				{
    					foreach( Gold g in goldlist )
    						total += g.Amount;
    				}
                }
                return (total >= EntryFee);
        }

		
		public virtual bool CollectEntryFee(Mobile m, int amount)
        {
            if(m == null) return false;
            
            if(amount <= 0) return true;

        // take the money
            if(m.BankBox != null)
            {
                if(!m.BankBox.ConsumeTotal( typeof(Gold), amount, true))
                {
                    XmlPoints.SendText(m, 100541, amount);  // "Could not withdraw the Entry fee of {0} gold from your bank."
                    return false;
                }
            } else
            {
                XmlPoints.SendText(m, 100541, amount);  // "Could not withdraw the Entry fee of {0} gold from your bank."
                return false;
            }
            
            XmlPoints.SendText(m, 100542, amount);   // "The Entry fee of {0} gold has been withdrawn from your bank."
    
            // and add it to the purse
            TotalPurse += amount;
            
            return true;
        }
        
        public virtual bool CheckQualify(Mobile m)
        {
            if(m == null) return false;

            XmlPoints a = (XmlPoints)XmlAttach.FindAttachment(m, typeof(XmlPoints));

            if(a == null)
            {
                XmlPoints.SendText(Challenger, 100536, m.Name);   // "{0} does not qualify. No points support."
                return false;
            }

            // make sure they qualify
            if(a.HasChallenge)
            {
                XmlPoints.SendText(Challenger, 100537, m.Name);  // "{0} is already in a Challenge."
                return false;
            }
            
            // and they have the Entry fee in the bank
            if(!HasEntryFee(m))
            {
                XmlPoints.SendText(Challenger, 100538, m.Name);  // "{0} cannot afford the Entry fee."
                return false;
            }
            
            return true;
        }
        
        public virtual void Forfeit(Mobile m)
        {
            if(m == null) return;

            ClearNameHue(m);

            // inform him that he has been kicked
            XmlPoints.SendText(m, 100543, ChallengeName);   // "You dropped out of {0}"
            GameBroadcast(100544, m.Name);  // "{0} has dropped out."

            RefreshSymmetricNoto(m);

            // this could end the game so check
            CheckForGameEnd();

            // and clear his challenge game
            XmlPoints a = (XmlPoints)XmlAttach.FindAttachment(m, typeof(XmlPoints));
            if(a != null)
            {
                a.ChallengeGame = null;
            }
        }

		public virtual void AwardWinnings(Mobile m, int amount)
        {
            if(m == null) return;

            if(m.Backpack != null && amount > 0)
            {
                // give them a check for the winnings
                BankCheck check = new BankCheck( amount);
                check.Name = String.Format(XmlPoints.GetText(m, 100300),ChallengeName); // "Prize from {0}"
                m.AddToBackpack( check );
                XmlPoints.SendText(m, 100301, amount); // "You have received a bank check for {0}"
            }
        }
        
        public void AwardTeamWinnings(int team, int amount)
        {
            if(team == 0) return;

            int count = 0;
            // go through all of the team members
            foreach(IChallengeEntry entry in Participants)
            {
                if(entry.Team == team)
                {
                    count++;
                }
            }

            if(count == 0) return;

            int split = amount/count;

            // and split the purse
            foreach(IChallengeEntry entry in Participants)
            {
                if(entry.Team == team)
                {
                    Mobile m = entry.Participant;
                    if(m.Backpack != null && amount > 0)
                    {
                        // give them a check for the winnings
                        BankCheck check = new BankCheck( split);
                        check.Name = String.Format(XmlPoints.GetText(m, 100300),ChallengeName); // "Prize from {0}"
                        m.AddToBackpack( check );
                        XmlPoints.SendText(m, 100301, split); // "You have received a bank check for {0}"
                    }
                }
            }
        }
		
		public static void DoSetupChallenge(Mobile from, int nameindex, Type gametype)
		{
            if(from != null && gametype != null)
            {
                bool onlyinchallenge = false;

                FieldInfo finfo = null;
                finfo = gametype.GetField( "OnlyInChallengeGameRegion" );
                if(finfo != null && finfo.IsStatic && finfo.FieldType == typeof(bool))
                {
                    try{
                        onlyinchallenge = (bool)finfo.GetValue(null);
                    } catch{}
                }

                // is this in a challenge game region?
                Region r = Region.Find(from.Location, from.Map);
                if(r is ChallengeGameRegion)
                {
                    ChallengeGameRegion cgr = r as ChallengeGameRegion;

                    if(cgr.ChallengeGame != null && !cgr.ChallengeGame.Deleted && !cgr.ChallengeGame.GameCompleted && !cgr.ChallengeGame.IsOrganizer(from))
                    {
                        from.SendMessage(String.Format(XmlPoints.GetText(from, 100303), XmlPoints.GetText(from, nameindex)));  //"Unable to set up a {0} Challenge: Another Challenge Game is already in progress in this Challenge Game region.", "Last Man Standing"
                        return;
                    }
                } else
                if(onlyinchallenge)
                {
                    from.SendMessage(String.Format(XmlPoints.GetText(from, 100304), XmlPoints.GetText(from, nameindex))); // "Unable to set up a {0} Challenge: Must be in a Challenge Game region.", "Last Man Standing"
                    return;
                }

                // create the game gauntlet
                object newgame = null;
                object[] gameargs = new object[1];
                gameargs[0] = from;

                try{
                newgame = Activator.CreateInstance( gametype, gameargs );
                } catch{}

                BaseChallengeGame g = newgame as BaseChallengeGame;

                if(g == null || g.Deleted)
                {
                    from.SendMessage(String.Format(XmlPoints.GetText(from, 100305), XmlPoints.GetText(from, nameindex)));  // "Unable to set up a {0} Challenge.", "Last Man Standing"
                    return;
                }


                g.MoveToWorld(from.Location, from.Map);
                from.SendMessage(String.Format(XmlPoints.GetText(from, 100306), XmlPoints.GetText(from, nameindex))); // "Setting up a {0} Challenge.", "Last Man Standing"

                // call any game-specific setups
                g.SetupChallenge(from);

                if(r is ChallengeGameRegion)
                {
                    ChallengeGameRegion cgr = r as ChallengeGameRegion;

                    cgr.ChallengeGame = g;
                    
                    g.IsInChallengeGameRegion = true;

                    // announce challenge game region games
                    XmlPoints.BroadcastMessage( AccessLevel.Player, 0x482, String.Format(XmlPoints.SystemText(100307), XmlPoints.SystemText(nameindex), r.Name, from.Name) );  // "{0} Challenge being prepared in '{1}' by {2}", "Last Man Standing"

                }

                // if there was a previous challenge being setup then delete it unless it is still in progress
                XmlPoints afrom = (XmlPoints)XmlAttach.FindAttachment(from, typeof(XmlPoints));

                if(afrom != null)
                {
                    if(afrom.ChallengeSetup != null && !(afrom.ChallengeSetup.GameInProgress || afrom.ChallengeSetup.GameCompleted))
                    {
                        afrom.ChallengeSetup.Delete();
                    }
                    afrom.ChallengeSetup = g;
                }

            }
		}

		public virtual void StartGame()
        {
            GameLocked = true;

            GameInProgress = true;

            InvalidateProperties();

            // if there are any non-participants in the arena area then kick them
            //ClearArena();

            GameBroadcast(100002); // "Let the games begin!"

            // set up the noto on everyone
            if(Participants != null)
            {
                foreach(IChallengeEntry entry in Participants)
                {
                    if(entry.Participant != null)
                    {
                        RefreshNoto(entry.Participant);
                        XmlPoints afrom = (XmlPoints)XmlAttach.FindAttachment(entry.Participant, typeof(XmlPoints));

                        // update the points gumps on the players if they are open
                        if(afrom != null && entry.Participant.HasGump(typeof(XmlPoints.PointsGump)))
                        {
                            afrom.OnIdentify(entry.Participant);
                        }
                    }
                }
            }


            // start the challenge timer
            StartChallengeTimer();
        }

        public virtual void ClearChallenge(Mobile from)
		{
            // check for points attachments
            XmlPoints afrom = (XmlPoints)XmlAttach.FindAttachment(from, typeof(XmlPoints));
            if(afrom != null)
            {
                afrom.ChallengeGame = null;
            }
		}

        
        public virtual void EndGame()
        {

            // go through the participant list and clear the challenge team
		    if(Participants != null)
		    {
                foreach(IChallengeEntry entry in Participants)
                {
                    if(entry.Participant == null || entry.Status == ChallengeStatus.Forfeit) continue;

                    ClearChallenge(entry.Participant);
                    
                    // clear combatants
                    entry.Participant.Combatant = null;
                    entry.Participant.Warmode = false;
                }
            }

            RefreshAllNoto();

		    GameInProgress = false;

            // stop the challenge timer
            if(m_Timer != null)
                m_Timer.Stop();

            InvalidateProperties();

            // start the gauntlet deletion timer
            Timer.DelayCall( PostGameDecayTime, new TimerCallback( Delete ) );

        }
        
        public void StartChallengeTimer()
        {
            if(m_Timer != null)
                m_Timer.Stop();

            m_Timer = new ChallengeTimer(this, TimeSpan.FromSeconds(1));

            m_Timer.Start();
        }

        public class ChallengeTimer : Timer
		{
			private BaseChallengeGame m_gauntlet;

			public ChallengeTimer( BaseChallengeGame gauntlet, TimeSpan delay ) : base( delay, delay )
			{
				Priority = TimerPriority.OneSecond;
				m_gauntlet = gauntlet;
			}

			protected override void OnTick()
			{
				// check for disqualification

				if(m_gauntlet != null && !m_gauntlet.Deleted && m_gauntlet.GameInProgress)
                {
                    m_gauntlet.OnTick();
			    } else
			    {
			         Stop();
				}
			}
		}

        public virtual bool InsuranceIsFree(Mobile from, Mobile awardto)
        {
            return false;
        }

        public virtual bool IsOrganizer(Mobile from)
        {
            if(from == null || Organizers == null) return false;

            foreach(Mobile m in Organizers)
            {
                if(from == m) return true;
            }

            return false;
        }

        public virtual bool ChallengeBeingCancelled{ get { return false; } }

        public virtual string ChallengeName { get { return Name; } }

        public virtual ArrayList Organizers { get { return null; } }

        public virtual bool UseKillDelay { get { return true; } }

        public virtual bool AllowPoints { get { return false; } }

        public abstract bool GameInProgress { get; set; }

        public abstract bool GameLocked { get; set; }

        public abstract bool GameCompleted { get; }
        
        public abstract ArrayList Participants { get; set; }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if( GameInProgress )
            {
                list.Add( 1060742 ); // Active
            }
            else
            if( GameCompleted )
            {
                list.Add( 1046033 ); // Completed
            } else
            {
                list.Add( 3000097 ); // Setup
            }
        }
        
        public override void OnDelete()
        {
            // if the game is in progress, then return all Entry fees
            if(GameInProgress)
            {
                GameBroadcast(100003,ChallengeName);  // "{0} cancelled"

                // go through the participants and return their fees and clear noto
                if(Participants != null)
                {
                    foreach(IChallengeEntry entry in Participants)
                    {
                        if(entry.Status == ChallengeStatus.Forfeit) continue;

                        Mobile from = entry.Participant;

                        // return the entry fee
                        if(from != null && from.BankBox != null && EntryFee > 0)
                        {
                            Item gold = new Gold(EntryFee);

                            if ( !from.BankBox.TryDropItem( from, gold, false ) )
        					{
        						gold.Delete();
        						from.AddToBackpack( new BankCheck( EntryFee ) );
        						XmlPoints.SendText(from, 100000, EntryFee); // "Entry fee of {0} gold has been returned to you."
        					} else
        					{
                                XmlPoints.SendText(from, 100001, EntryFee);  // "Entry fee of {0} gold has been returned to your bank account."
                            }
                        }

                        entry.Status = ChallengeStatus.None;
                    }

                    // clear all noto
                    foreach(IChallengeEntry entry in Participants)
                    {
                        RefreshNoto(entry.Participant);
                    }
                }
                
                EndGame();
            } else
            if(!GameCompleted)
            {
                // this is when a game is cancelled during setup
                GameBroadcast(100003,ChallengeName); // "{0} cancelled"
            }

            base.OnDelete();
        }

        public BaseChallengeGame(int itemid) : base( itemid )
        {
        }

        public BaseChallengeGame() : base(0)
        {
        }


        public BaseChallengeGame( Serial serial ) : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 1 ); // version
            
            // version 1
            writer.Write(m_IsInChallengeGameRegion);
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
            
            switch(version)
            {
            case 1:
                m_IsInChallengeGameRegion = reader.ReadBool();
                break;
            }
            

        }
        
        public int ActivePlayers()
		{
            if(Participants == null || !GameInProgress) return 0;

            int leftstanding = 0;

            foreach(IChallengeEntry entry in Participants)
            {
                if(entry.Status == ChallengeStatus.Active)
                {
                    leftstanding++;
                }
            }
            return leftstanding;
		}

        public bool HasValidMembers(TeamInfo t)
        {
            // make sure the team has valid members
            foreach(IChallengeEntry entry in t.Members)
            {
                // just need to find one active member
                if(entry.Status == ChallengeStatus.Active)
                {
                    return true;
                }
            }

            return false;
        }
        
        public static int TeamColor(int team)
        {
            if(team < 6)
                return 20 + team * 40;
            else
                return 10 + (team - 6) * 20;
        }
        
        public virtual TeamInfo NewTeam(int team)
        {
            return new TeamInfo(team);
        }
        
        public TeamInfo GetTeam(int team)
        {
            ArrayList Teams = GetTeams();
            if(Teams != null)
            {
                foreach(TeamInfo t in Teams)
                {
                    if(t.ID == team) return t;
                }
            }
            
            return null;
        }
		
		public ArrayList GetTeams()
        {
            if(Participants == null) return null;

            ArrayList Teams = new ArrayList();

            foreach(IChallengeEntry entry in Participants)
            {
                if(entry == null) continue;

                int tid = entry.Team;
                TeamInfo team = null;

                // find the team info for the team the participant is on
                foreach(TeamInfo t in Teams)
                {
                    if(t.ID == tid)
                    {
                        team = t;
                    }
                }

                // keep track of the teams
                if(team == null)
                {
                    team = NewTeam(tid);
                    Teams.Add(team);
                }

                team.Members.Add(entry);

                // keep track of the number of total and active players
                if(entry.Status == ChallengeStatus.Active)
                {
                    team.NActive++;
                    team.Score += entry.Score;
                }

            }

            return Teams;
        }

        public void SetNameHue()
		{
            // set the namehue for each participant based on their team
            if(Participants != null)
            {
                foreach(IChallengeEntry entry in Participants)
                {
                    if(entry.Participant != null && entry.Status == ChallengeStatus.Active && entry.Team != 0)
                        entry.Participant.NameHue = TeamColor(entry.Team);
                }
            }
		}
		
		public void ClearNameHue()
		{
            if(Participants != null)
            {
                foreach(IChallengeEntry entry in Participants)
                {
                    if(entry.Participant != null)
                        entry.Participant.NameHue = -1;
                }
            }
		}
		
		public void ClearNameHue(Mobile m)
		{
            if(m == null) return;

            if(Participants != null)
            {
                foreach(IChallengeEntry entry in Participants)
                {
                    if(entry.Participant == m)
                        m.NameHue = -1;
                }
            }
		}
        
        public void RefreshAllNoto()
        {

            if(Participants != null)
            {
                foreach(IChallengeEntry entry in Participants)
                {
                    RefreshNoto(entry.Participant);
                }
            }

        }

        public void RefreshNoto(Mobile from)
        {
            if(from == null) return;

            if(Participants != null)
            {
                foreach(IChallengeEntry entry in Participants)
                {
                    if(entry.Participant != from)
                    {
                        from.Send( new MobileMoving( entry.Participant, Notoriety.Compute( from, entry.Participant ) ) );
                    }
                }
            }

        }
        
        public void RefreshSymmetricNoto(Mobile from)
        {
            if(from == null) return;

            if(Participants != null)
            {
                foreach(IChallengeEntry entry in Participants)
                {
                    if(entry.Participant != from)
                    {
                        from.Send( new MobileMoving( entry.Participant, Notoriety.Compute( from, entry.Participant ) ) );
                        entry.Participant.Send( new MobileMoving( from, Notoriety.Compute( entry.Participant, from ) ) );
                    }
                }
            }
        }

        public IChallengeEntry GetParticipant(Mobile m)
        {
            if(Participants != null)
		    {
                foreach(IChallengeEntry entry in Participants)
                {
                    if(entry.Participant == m && entry.Status == ChallengeStatus.Active) return entry;
                }
            }

            return null;
        }

        public virtual void GameBroadcastSound(int sound)
		{
            foreach(IChallengeEntry entry in Participants)
            {
                if(entry.Participant == null || entry.Status == ChallengeStatus.Forfeit) continue;

                // play the sound
                entry.Participant.PlaySound(sound);
            }
		}

		public virtual void GameBroadcast(int msgindex)
		{
            // go through the participant list and send all participants the message
		    if(Participants != null)
		    {
                foreach(IChallengeEntry entry in Participants)
                {
                    if(entry.Participant == null || entry.Status == ChallengeStatus.Forfeit) continue;
                    
                    XmlPoints.SendColorText(entry.Participant, 40, msgindex);
                }
            }
		}
		
		public virtual void GameBroadcast(int msgindex, object arg)
		{
            // go through the participant list and send all participants the message
		    if(Participants != null)
		    {
                foreach(IChallengeEntry entry in Participants)
                {
                    if(entry.Participant == null || entry.Status == ChallengeStatus.Forfeit) continue;
                    
                    XmlPoints.SendColorText(entry.Participant, 40, msgindex, arg);
                }
            }
		}
		
		public virtual void GameBroadcast(int msgindex, object arg, object arg2)
		{
            // go through the participant list and send all participants the message
		    if(Participants != null)
		    {
                foreach(IChallengeEntry entry in Participants)
                {
                    if(entry.Participant == null || entry.Status == ChallengeStatus.Forfeit) continue;
                    
                    XmlPoints.SendColorText(entry.Participant, 40, msgindex, arg, arg2);
                }
            }
		}
        
        public virtual void GameBroadcast(string msg)
		{
		    // go through the participant list and send all participants the message
		    if(Participants != null)
		    {
                foreach(IChallengeEntry entry in Participants)
                {
                    if(entry.Participant == null || entry.Status == ChallengeStatus.Forfeit) continue;
                    
                    entry.Participant.SendMessage(40,msg);
                }
            }
		}
    }
}
