using System; 
using Server;
using Server.Gumps;

namespace Server.Items
{ 
	public class MinerSlotStone : Item 
	{
		private int m_ReelOne = 0;
		private int m_ReelTwo = 0;
		private int m_ReelThree = 0;

		private int m_Cost = 100;
		private int m_Won;
		private int m_TotalCollected;

		private bool m_IsInUse;

		private int m_LastPay;

		public static int[] m_Bars = new int[]
		{
			7147,
			7159,
			7141,
			7153
		};

		public int ReelOne
		{
			get{ return m_ReelOne; }
			set{ m_ReelOne = value; }
		}

		public int ReelTwo
		{
			get{ return m_ReelTwo; }
			set{ m_ReelTwo = value; }
		}

		public int ReelThree
		{
			get{ return m_ReelThree; }
			set{ m_ReelThree = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Cost
		{
			get{ return m_Cost; }

			set
			{
				if ( value == 4 || value <= 4 )
					m_Cost = 5; 
				else
					m_Cost = value; 
			}
		}

		public int Won
		{
			get{ return m_Won; }
			set{ m_Won = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int TotalCollected
		{
			get{ return m_TotalCollected; }
			set{ m_TotalCollected = value; }
		}

		public int LastPay
		{
			get{ return m_LastPay; }
			set{ m_LastPay = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool IsInUse
		{
			get{ return m_IsInUse; }
			set{ m_IsInUse = value; }
		}

		[Constructable] 
		public MinerSlotStone() : base( 3804 ) 
		{ 
			Movable = false; 
			Name = "Miner Madness";
		} 

		public override void OnDoubleClick( Mobile from ) 
		{
			if ( m_IsInUse == false )
			{
				from.CloseGump( typeof( MinerMadnessSlotGump ) );
				from.SendGump( new MinerMadnessSlotGump( this ) );
			}
			else
			{
				from.SendMessage( "Someone else is playing this machine." );
			}
		} 

		public void DoSpin( Mobile from )
		{
			SpinReelOne();
			SpinReelTwo();
			SpinReelThree();

			from.PlaySound ( 739 );

			if ( m_ReelOne == m_ReelTwo && m_ReelOne == m_ReelThree ) // Winner
			{
				if ( m_ReelOne == 7147 )
				{
					int p = m_Cost * 10000;

					m_Won += p;
					from.SendMessage( 38, "You win {0} Gold!", p );
					from.PlaySound ( 61 );
					m_LastPay = p;
					m_TotalCollected -= p;
					from.SendGump( new MinerMadnessSlotGump( this ) );
				}

				if ( m_ReelOne == 7159 )
				{
					int p = m_Cost * 5000;

					m_Won += p;
					from.SendMessage( 38, "You win {0} Gold!", p );
					from.PlaySound ( 61 );
					m_LastPay = p;
					m_TotalCollected -= p;
					from.SendGump( new MinerMadnessSlotGump( this ) );
				}

				if ( m_ReelOne == 7141 )
				{
					int p = m_Cost * 1000;

					m_Won += p;
					from.SendMessage( 38, "You win {0} Gold!", p );
					from.PlaySound ( 61 );
					m_LastPay = p;
					m_TotalCollected -= p;
					from.SendGump( new MinerMadnessSlotGump( this ) );
				}

				if ( m_ReelOne == 7153 )
				{
					int p = m_Cost * 500;

					m_Won += p;
					from.SendMessage( 38, "You win {0} Gold!", p );
					from.PlaySound ( 61 );
					m_LastPay = p;
					m_TotalCollected -= p;
					from.SendGump( new MinerMadnessSlotGump( this ) );
				}

				if ( m_ReelOne == 3717 )
				{
					from.SendMessage( 38, "You have hit the bonus round." );
					from.SendGump( new MinerBonusGump( this, false, false, false, false, false, false, false, false, false, 0, 0, 0, 0, 0, 0, 0, 0, 0 ) );
				}

				if ( m_ReelOne == 4020 )
				{
					int p = m_Cost * 5;

					m_Won += p;
					from.SendMessage( 38, "You win {0} Gold!", p );
					from.PlaySound ( 61 );
					m_LastPay = p;
					m_TotalCollected -= p;
					from.SendGump( new MinerMadnessSlotGump( this ) );
				}

				if ( m_ReelOne == 5091 )
				{
					int p = m_Cost * 3;

					m_Won += p;
					from.SendMessage( 38, "You win {0} Gold!", p );
					from.PlaySound ( 61 );
					m_LastPay = p;
					m_TotalCollected -= p;
					from.SendGump( new MinerMadnessSlotGump( this ) );
				}

				if ( m_ReelOne == 6262 )
				{
					int p = m_Cost * 2;

					m_Won += p;
					from.SendMessage( 38, "You win {0} Gold!", p );
					from.PlaySound ( 61 );
					m_LastPay = p;
					m_TotalCollected -= p;
					from.SendGump( new MinerMadnessSlotGump( this ) );
				}
			}
			else
			{
				bool r1isbar = false;
				bool r2isbar = false;
				bool r3isbar = false;

				foreach ( int reel1 in m_Bars )
				{
	  				if ( m_ReelOne == reel1  )
						r1isbar = true;
				}

				foreach ( int reel2 in m_Bars )
				{
	  				if ( m_ReelTwo == reel2 )
						r2isbar = true;
				}

				foreach ( int reel3 in m_Bars )
				{
	  				if ( m_ReelThree == reel3 )
						r3isbar = true;
				}

				if ( r1isbar == true && r2isbar == true && r3isbar == true )
				{
					int p = m_Cost * 10;

					m_Won += p;
					from.SendMessage( 38, "You win {0} Gold!", p );
					from.PlaySound ( 61 );
					m_LastPay = p;
					m_TotalCollected -= p;
					from.SendGump( new MinerMadnessSlotGump( this ) );
				}
				else if ( m_ReelOne == 3717 && m_ReelTwo != 3717 && m_ReelThree != 3717 )
				{
					int p = m_Cost / 3;

					m_Won += p;
					from.SendMessage( 38, "You win {0} Gold!", p );
					m_LastPay = p;
					m_TotalCollected -= p;
					from.SendGump( new MinerMadnessSlotGump( this ) );
				}
				else if ( m_ReelOne != 3717 && m_ReelTwo == 3717 && m_ReelThree != 3717 )
				{
					int p = m_Cost / 3;

					m_Won += p;
					from.SendMessage( 38, "You win {0} Gold!", p );
					m_LastPay = p;
					m_TotalCollected -= p;
					from.SendGump( new MinerMadnessSlotGump( this ) );
				}
				else if ( m_ReelOne != 3717 && m_ReelTwo != 3717 && m_ReelThree == 3717 )
				{
					int p = m_Cost / 3;

					m_Won += p;
					from.SendMessage( 38, "You win {0} Gold!", p );
					m_LastPay = p;
					m_TotalCollected -= p;
					from.SendGump( new MinerMadnessSlotGump( this ) );
				}
				else if ( m_ReelOne == 3717 && m_ReelTwo == 3717 )
				{
					int p = m_Cost / 2;

					m_Won += p;
					from.SendMessage( 38, "You win {0} Gold!", p );
					m_LastPay = p;
					m_TotalCollected -= p;
					from.SendGump( new MinerMadnessSlotGump( this ) );
				}
				else if ( m_ReelTwo == 3717 && m_ReelThree == 3717 )
				{
					int p = m_Cost / 2;

					m_Won += p;
					from.SendMessage( 38, "You win {0} Gold!", p );
					m_LastPay = p;
					m_TotalCollected -= p;
					from.SendGump( new MinerMadnessSlotGump( this ) );
				}
				else if ( m_ReelOne == 3717 && m_ReelThree == 3717 )
				{
					int p = m_Cost / 2;

					m_Won += p;
					from.SendMessage( 38, "You win {0} Gold!", p );
					m_LastPay = p;
					m_TotalCollected -= p;
					from.SendGump( new MinerMadnessSlotGump( this ) );
				}
				else
				{
					from.SendMessage( "Sorry you didnt win, Try Again!" );
					m_LastPay = 0;
					from.SendGump( new MinerMadnessSlotGump( this ) );
				}
			}
		}

		public void DoCashOut( Mobile from )
		{
			from.AddToBackpack( new BankCheck( m_Won ) );
			from.SendMessage( "You collect all your winnings." );
			from.PlaySound( 52 );
			from.PlaySound( 53 );
			from.PlaySound( 54 );
			from.PlaySound( 55 );
			m_Won = 0;
		}

		public void SpinReelOne()
		{
			int [] icon1 = { 1, 9 }; 								  // Gold Bars
			int [] icon2 = { 2, 10, 17 }; 								  // Silver Bars
			int [] icon3 = { 3, 11, 18, 24 }; 							  // Bronze Bars
			int [] icon4 = { 4, 12, 19, 25, 30 }; 							  // Iron Bars
			int [] icon8 = { 5, 13, 20, 26, 31, 35, 39, 43 }; 					  // Pickaxe
			int [] icon5 = { 6, 14, 21, 27, 32, 36, 40, 44, 47, 50, 53 }; 				  // Sledge Hammer
			int [] icon6 = { 7, 15, 22, 28, 33, 37, 41, 45, 48, 51, 54, 56, 58, 60, 62 }; 		  // Smith Hammer
			int [] icon7 = { 8, 16, 23, 29, 34, 38, 42, 46, 49, 52, 55, 57, 59, 61, 63, 64, 65, 66 }; // Iron Wire

			int spin = Utility.Random( 66 );
			
			foreach ( int reel in icon1 )
			{
	  			if ( reel == spin )
					m_ReelOne = 7147;
			}

			foreach ( int reel in icon2 )
			{
	  			if ( reel == spin )
					m_ReelOne = 7159;
			}

			foreach ( int reel in icon3 )
			{
	  			if ( reel == spin )
					m_ReelOne = 7141;
			}

			foreach ( int reel in icon4 )
			{
	  			if ( reel == spin )
					m_ReelOne = 7153;
			}

			foreach ( int reel in icon5 )
			{
	  			if ( reel == spin )
					m_ReelOne = 4020;
			}

			foreach ( int reel in icon6 )
			{
	  			if ( reel == spin )
					m_ReelOne = 5091;
			}

			foreach ( int reel in icon7 )
			{
	  			if ( reel == spin )
					m_ReelOne = 6262;
			}

			foreach ( int reel in icon8 )
			{
	  			if ( reel == spin )
					m_ReelOne = 3717;
			}
		}

		public void SpinReelTwo()
		{
			int [] icon1 = { 1, 9 }; 								  // Gold Bars
			int [] icon2 = { 2, 10, 17 }; 								  // Silver Bars
			int [] icon3 = { 3, 11, 18, 24 }; 							  // Bronze Bars
			int [] icon4 = { 4, 12, 19, 25, 30 }; 							  // Iron Bars
			int [] icon8 = { 5, 13, 20, 26, 31, 35, 39, 43 }; 					  // Pickaxe
			int [] icon5 = { 6, 14, 21, 27, 32, 36, 40, 44, 47, 50, 53 }; 				  // Sledge Hammer
			int [] icon6 = { 7, 15, 22, 28, 33, 37, 41, 45, 48, 51, 54, 56, 58, 60, 62 }; 		  // Smith Hammer
			int [] icon7 = { 8, 16, 23, 29, 34, 38, 42, 46, 49, 52, 55, 57, 59, 61, 63, 64, 65, 66 }; // Iron Wire

			int spin = Utility.Random( 66 );
			
			foreach ( int reel in icon1 )
			{
	  			if ( reel == spin )
					m_ReelTwo = 7147;
			}

			foreach ( int reel in icon2 )
			{
	  			if ( reel == spin )
					m_ReelTwo = 7159;
			}

			foreach ( int reel in icon3 )
			{
	  			if ( reel == spin )
					m_ReelTwo = 7141;
			}

			foreach ( int reel in icon4 )
			{
	  			if ( reel == spin )
					m_ReelTwo = 7153;
			}

			foreach ( int reel in icon5 )
			{
	  			if ( reel == spin )
					m_ReelTwo = 4020;
			}

			foreach ( int reel in icon6 )
			{
	  			if ( reel == spin )
					m_ReelTwo = 5091;
			}

			foreach ( int reel in icon7 )
			{
	  			if ( reel == spin )
					m_ReelTwo = 6262;
			}

			foreach ( int reel in icon8 )
			{
	  			if ( reel == spin )
					m_ReelTwo = 3717;
			}
		}

		public void SpinReelThree()
		{
			int [] icon1 = { 1, 9 }; 								  // Gold Bars
			int [] icon2 = { 2, 10, 17 }; 								  // Silver Bars
			int [] icon3 = { 3, 11, 18, 24 }; 							  // Bronze Bars
			int [] icon4 = { 4, 12, 19, 25, 30 }; 							  // Iron Bars
			int [] icon8 = { 5, 13, 20, 26, 31, 35, 39, 43 }; 					  // Pickaxe
			int [] icon5 = { 6, 14, 21, 27, 32, 36, 40, 44, 47, 50, 53 }; 				  // Sledge Hammer
			int [] icon6 = { 7, 15, 22, 28, 33, 37, 41, 45, 48, 51, 54, 56, 58, 60, 62 }; 		  // Smith Hammer
			int [] icon7 = { 8, 16, 23, 29, 34, 38, 42, 46, 49, 52, 55, 57, 59, 61, 63, 64, 65, 66 }; // Iron Wire

			int spin = Utility.Random( 66 );
			
			foreach ( int reel in icon1 )
			{
	  			if ( reel == spin )
					m_ReelThree = 7147;
			}

			foreach ( int reel in icon2 )
			{
	  			if ( reel == spin )
					m_ReelThree = 7159;
			}

			foreach ( int reel in icon3 )
			{
	  			if ( reel == spin )
					m_ReelThree = 7141;
			}

			foreach ( int reel in icon4 )
			{
	  			if ( reel == spin )
					m_ReelThree = 7153;
			}

			foreach ( int reel in icon5 )
			{
	  			if ( reel == spin )
					m_ReelThree = 4020;
			}

			foreach ( int reel in icon6 )
			{
	  			if ( reel == spin )
					m_ReelThree = 5091;
			}

			foreach ( int reel in icon7 )
			{
	  			if ( reel == spin )
					m_ReelThree = 6262;
			}

			foreach ( int reel in icon8 )
			{
	  			if ( reel == spin )
					m_ReelThree = 3717;
			}
		}

		public MinerSlotStone( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 

			writer.Write( (int) 0 ); // version

			// Release 0
			writer.Write( m_ReelOne );
			writer.Write( m_ReelTwo );
			writer.Write( m_ReelThree );
			writer.Write( m_Cost );
			writer.Write( m_Won );
			writer.Write( TotalCollected );
			writer.Write( m_LastPay );
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0: // Release 0
				{
					m_ReelOne = reader.ReadInt();
					m_ReelTwo = reader.ReadInt();
					m_ReelThree = reader.ReadInt();
					m_Cost = reader.ReadInt();
					m_Won = reader.ReadInt();
					m_TotalCollected = reader.ReadInt();
					m_LastPay = reader.ReadInt();
					break;
				}
			}
		} 
	} 
} 