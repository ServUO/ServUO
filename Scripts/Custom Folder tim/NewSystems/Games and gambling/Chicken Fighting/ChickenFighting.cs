using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;
using Server.Mobiles;
using Server.Items;
using Server.Accounting;

namespace Server.Items
{
    public class CockFightingControlStone : Item
    {
		private int m_MaxPlayers = 2;
		private int m_NumPlayers;
		private int m_DeadCount;
		private int m_Bet = 1000;  //The default amount to play per player.  This will make the winner get 10k per player as well.
		private int m_ArenaSize = 10;  //10 tiles by default.
		
		private bool m_GameRunning = false;
		private bool m_Tournament = false;
		
		private Mobile m_LastWinner;

        public List<Mobile> ChickenList = new List<Mobile>();
		
		private Point3D m_Corner1;
		private Point3D m_Corner2;
		private Point3D m_Corner3;
		private Point3D m_Corner4;
        private static int m_MaxInLine = 5;
        private List<Mobile> m_UsersInLine;
		
		private FightingChicken m_LastChicken;
		
		
		[CommandProperty( AccessLevel.GameMaster )]
        public int MaxPlayers
        {
            get
            {
                return m_MaxPlayers;
            }
            set
            {
				if(value < 2)
					value = 2;
					
				m_MaxPlayers = value;
				
				InvalidateProperties();
            }
        }
		
		[CommandProperty( AccessLevel.GameMaster )]
        public int NumberOfPlayers
        {
            get
            {
                return m_NumPlayers;
            }
            set
            {
				if(value < 0)
					value = 0;
					
                m_NumPlayers = value; 
				InvalidateProperties();
            }
        }
        
        [CommandProperty( AccessLevel.GameMaster )]
        public int DeadChickens
        {
            get
            {
                return m_DeadCount;
            }
            set
            {
				if(value < 0)
					value = 0;
					
                m_DeadCount = value; 
				InvalidateProperties();
            }
        }
		
		[CommandProperty( AccessLevel.GameMaster )]
        public int Bet
        {
            get
            {
                return m_Bet;
            }
            set
            {
				if(value < 0)
					value = 0;
					
                m_Bet = value; 
				InvalidateProperties();
            }
        }
		
		[CommandProperty( AccessLevel.GameMaster )]
        public int ArenaSize
        {
            get
            {
                return m_ArenaSize;
            }
            set
            {
				if(value < 10)
					value = 10;
					
                m_ArenaSize = value; 
				InvalidateProperties();
            }
        }
        
        [CommandProperty( AccessLevel.GameMaster )]
        public bool GameRunning
        {
            get
            {
                return m_GameRunning;
            }
            set
            {
                m_GameRunning = value; 
				InvalidateProperties();
            }
        }
        
        [CommandProperty( AccessLevel.GameMaster )]
        public bool Tournament
        {
            get
            {
                return m_Tournament;
            }
            set
            {
                m_Tournament = value; 
				InvalidateProperties();
            }
        }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D Corner1
		{
			get { return m_Corner1; }
			set { m_Corner1 = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D Corner2
		{
			get { return m_Corner2; }
			set { m_Corner2 = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D Corner3
		{
			get { return m_Corner3; }
			set { m_Corner3 = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D Corner4
		{
			get { return m_Corner4; }
			set { m_Corner4 = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.Owner )]
		public Mobile LastWinner
        { 
            get
            { 
                return m_LastWinner; 
            } 
            set 
            {
                m_LastWinner = value; InvalidateProperties(); 
            }
        }
		
		[CommandProperty( AccessLevel.GameMaster )]
		public FightingChicken LastChicken
		{
			get { return m_LastChicken; }
			set { m_LastChicken = value; InvalidateProperties(); }
		}

        public List<Mobile> UsersInLine
        {
            get 
            { 
                return m_UsersInLine; 
            }
            set 
            { 
                m_UsersInLine = value; 
            }
        }
		

		 [Constructable]
        public CockFightingControlStone() :base( 0xED4 )
        {
			Movable = false;
			Hue = 1157;
			Name = "Cock Fights";
            m_UsersInLine = new List<Mobile>();
        }


        public override void OnDoubleClick(Mobile from)
        {
            Account acct = (Account)from.Account;
            bool CockFightBetted = Convert.ToBoolean(acct.GetTag("CockFightBetted"));

            if (CockFightBetted) //added account tag check
            {
                from.SendMessage("You have already placed a bet on a chicken" );
                return;
            }
            else //what to do if account not been tagged
            {
                if (!from.Alive)
                {
                    from.SendMessage("Bring yourself back to life first is a better idea.");
                    return;
                }
                if (!from.InRange(this.GetWorldLocation(), 2))
                {
                    from.SendMessage("You are too far away to use the Cock Pit control.");
                    return;
                }
                if (m_GameRunning)
                {
                    from.SendMessage("A fight is currently in progress, please wait until it is over.");
                    return;
                }
                if (m_UsersInLine.Count >= m_MaxInLine)
                {
                    from.SendMessage("The arena has been fully booked at the moment. Try back later.");
                    return;
                }
                else
                {
                    Item item = from.Backpack.FindItemByType(typeof(Gold));
                    Gold g = item as Gold;

                    if (g != null && g.Amount >= m_Bet)
                    {
                        g.Amount -= m_Bet; //g.Consume(m_Bet); <--?
                        JoinGame(from, this);
                        from.SendMessage("{0} has been withdrawn from your backpack.  The chicken you are betting on has been placed in the arena to warm up for his fight.", m_Bet);
                        acct.SetTag("CockFightBetted", "true");
                    }
                    else
                    {
                        from.SendMessage("You don't have enough gold to place a bet on a chicken.");
                    }
                }
            }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );
   
			   list.Add( 1042971, String.Format("Cock Fights - {0} / {1}  Players Ready To Play", m_NumPlayers, m_MaxPlayers) );
			   
			   if(m_Bet > 0)
			   {
					list.Add( 1060659, "Cost To Play\t{0}", m_Bet );
					list.Add( 1070722, String.Format( "Winner Takes {0} to the Bank!", m_Bet * m_MaxPlayers ) );
			   }
			   
				if(m_Tournament)
					list.Add( 1060660, "Mode\t{0}", "Tournament");
				
			   if(m_GameRunning)
					list.Add( "FIGHT IN PROGRESS");
        }
		

        public CockFightingControlStone( Serial serial ) : base( serial )
        {
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
			m_MaxPlayers = reader.ReadInt();
			m_NumPlayers = reader.ReadInt();
			m_DeadCount = reader.ReadInt();
			m_Bet = reader.ReadInt();
			m_ArenaSize = reader.ReadInt();
			m_GameRunning = reader.ReadBool();
			m_Corner1 = reader.ReadPoint3D();
			m_Corner2 = reader.ReadPoint3D();
			m_Corner3 = reader.ReadPoint3D();
			m_Corner4 = reader.ReadPoint3D();
            m_UsersInLine = new List<Mobile>();

            if (m_UsersInLine == null)
                m_UsersInLine = new List<Mobile>();
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
			writer.Write( (int)m_MaxPlayers);
			writer.Write( (int)m_NumPlayers);
			writer.Write( (int)m_DeadCount);
			writer.Write( (int)m_Bet);
			writer.Write( (int)m_ArenaSize);
			writer.Write( (bool)m_GameRunning);
			writer.Write( m_Corner1 );
			writer.Write( m_Corner2);
			writer.Write( m_Corner3 );
			writer.Write( m_Corner4 );

        }
		
		public void ReleaseTheChickens()
		{
			foreach (Mobile m in GetMobilesInRange( m_ArenaSize ))
			{
                if (m is FightingChicken)
				{
					FightingChicken fc = m as FightingChicken;
					fc.Blessed = false;
					fc.Frozen = false;
                    ChickenList.Add(fc);
					
					if(fc.Owner != null)
						fc.Owner.SendMessage("Fight!");
				}
			}
		}
		
		public void JoinGame(Mobile from, CockFightingControlStone controller)
		{
			if( !controller.GameRunning )
			{
				controller.NumberOfPlayers++;
					
				FightingChicken fc = new FightingChicken(from, controller);
				
				fc.Blessed = true;
				fc.Frozen = true;
				fc.Map = from.Map;
				
				switch(Utility.Random(4) )
				{
					case 0: { fc.Location = controller.Corner1; } break;
					case 1: { fc.Location = controller.Corner2; } break;
					case 2: { fc.Location = controller.Corner3; } break;
					case 3: { fc.Location = controller.Corner4; } break;
				}
				
				if(controller.NumberOfPlayers == controller.MaxPlayers)
				{
					controller.GameRunning = true;
					controller.ReleaseTheChickens();
				}
				
				fc.Combatant = controller.LastChicken;
				controller.LastChicken = fc;
			}
			else
			{
				from.SendMessage("A fight is currently in progress, please wait until it is over.");
			}
		}

        public void EndGame( CockFightingControlStone controller)
        {
			if( ChickenList.Count > 0 )
			{
				for( int i = 0; i < ChickenList.Count; i++ )
				{
					if( controller != null && !controller.Deleted)
					{
						FightingChicken fc = (FightingChicken)ChickenList[i];
												
						if( fc.Owner != null && !(fc.Owner.Deleted) )
						{
							controller.LastWinner = fc.Owner;
                            Account acct = (Account)fc.Owner.Account;
                            bool CockFightBetted = Convert.ToBoolean(acct.GetTag("CockFightBetted"));
							if(controller.Bet > 0)
							{
								BankCheck bc = new BankCheck(controller.MaxPlayers * controller.Bet);
								fc.Owner.AddToBackpack(bc);
								fc.Owner.SendMessage("You won the cock fight!");
                                acct.SetTag("CockFightBetted", "false");
							}
							fc.Delete();	
						}							
					}
				}
			}
            controller.ChickenList.Clear();
			controller.DeadChickens = 0;
			controller.NumberOfPlayers = 0;
			controller.GameRunning = false;
			controller.LastChicken = null;
		}
    }
}
