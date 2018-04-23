// Created By Lucid Nagual - Admin of The Conjuring

using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Misc;


namespace Server.Mobiles
{
	public class DrugDealer : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }
		
		private static bool m_Talked;

		string[] kfcsay = new string[]
		{
		"Pst. Hey come here!!!",
		"I withdraw right out of the bank!",
		"What would you be looking for?!!!!",
		"I withdraw right out of the bank!",
		"Have I seen you here before?!!!",
		"Hey I remember you.",
		"I withdraw right out of the bank!",
		"You want the usual?!!!"
		};

		public override void VendorBuy( Mobile from )
		{
			from.SendGump( new Gump_DrugDealer( from, this ) );
		}

		
		[Constructable]
		public DrugDealer() : base( "The Drug Dealer" )
		{
			CantWalk = true;
			Name = "Bud Green";
		}

		public override VendorShoeType ShoeType
		{
			get{ return VendorShoeType.Sandals; }
		}

		public override int GetShoeHue()
		{
			return 0;
		}

		public override  void InitBody() 
		{
			base.InitBody();
		}

		public override void InitOutfit()
		{
			AddItem( new Server.Items.Shirt( Utility.RandomRedHue() ) );
			AddItem( new Server.Items.SkullCap( Utility.RandomBlueHue() ) );
			AddItem( new Server.Items.ShortPants( Utility.RandomBlueHue() ) );
			int hairHue=0x3E;
			switch ( Utility.Random( 9 ) )
			{
				case 0: AddItem( new Afro( hairHue ) ); break;
				case 1: AddItem( new KrisnaHair( hairHue ) ); break;
				case 2: AddItem( new PageboyHair( hairHue ) ); break;
				case 3: AddItem( new PonyTail( hairHue ) ); break;
				case 4: AddItem( new ReceedingHair( hairHue ) ); break;
				case 5: AddItem( new TwoPigTails( hairHue ) ); break;
				case 6: AddItem( new ShortHair( hairHue ) ); break;
				case 7: AddItem( new LongHair( hairHue ) ); break;
				case 8: AddItem( new BunsHair( hairHue ) ); break;
			}
		}

		

		public override void InitSBInfo()
		{
		}

		public DrugDealer( Serial serial ) : base( serial )
		{
		}

		private class SpamTimer : Timer 
		{ 
			public SpamTimer() : base( TimeSpan.FromSeconds( 8 ) ) 
			{ 
				Priority = TimerPriority.OneSecond; 
			} 

			protected override void OnTick() 
			{ 
				m_Talked = false; 
			} 
		} 

		private static void SayRandom( string[] say, Mobile m ) 
		{ 
			m.Say( say[Utility.Random( say.Length )] ); 
		} 

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

//--Traveling Agent Gump Start----------------------------------------------------
	
	public class Gump_DrugDealer : Gump
	{
		private Mobile m_From;
		private Mobile m_Vendor;

		public Gump_DrugDealer( Mobile from, Mobile vendor ) : base( 50, 50 )
		{
			m_From = from;
			m_Vendor = vendor;

			int balance = Banker.GetBalance( from );

			Closable=true;
			Disposable=true;
			Dragable=true;
			Resizable=false;
			AddPage(0);
			AddBackground(375, 77, 254, 379, 9200);
			AddLabel(405, 90, 139, @"Hey I think I got what you want!!!");
			AddLabel(405, 140, 0, @"WaterBong 250 gp");
			AddLabel(405, 170, 0, @"Joint 50 gp");
			AddLabel(405, 200, 0, @"Rolling Paper 10 gp");
			AddLabel(405, 230, 0, @"Marijuana 200 gp");
			AddLabel(405, 260, 0, @"Marijuana Seeds 500 gp");
			AddLabel(405, 290, 0, @"Karma Bong 250 gp");
			AddLabel(405, 421, 52, @"Thank you for your purchase!");
			AddImage(523, 385, 10430);
			AddButton(390, 143, 1210, 248, 1, GumpButtonType.Reply, 0);
			AddButton(390, 173, 1210, 248, 2, GumpButtonType.Reply, 0);
			AddButton(390, 203, 1210, 248, 3, GumpButtonType.Reply, 0);
			AddButton(390, 233, 1210, 248, 4, GumpButtonType.Reply, 0);
			AddButton(390, 263, 1210, 248, 5, GumpButtonType.Reply, 0);
			AddButton(390, 293, 1210, 248, 6, GumpButtonType.Reply, 0);
			AddButton(390, 393, 4012, 248, 7, GumpButtonType.Reply, 0);
			AddImage(325, 40, 10400);
			AddImage(325, 221, 10401);
			AddImage(325, 399, 10402);
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{

			if ( info.ButtonID == 1) // Water Bong
			{
				if ( Banker.Withdraw( m_From, 250 ) )
				{
					m_From.AddToBackpack(new WaterBong());
					m_From.PrivateOverheadMessage( MessageType.Label, 090, true,"A water bong is placed in your backpack.", m_From.NetState );
				}
				else
				m_Vendor.PrivateOverheadMessage( MessageType.Label, 090, true,"You don't have enough money in your bank.", m_From.NetState );
			}

			if ( info.ButtonID == 2) // Joint
			{
				if ( Banker.Withdraw( m_From, 50 ) )
				{
					m_From.AddToBackpack(new Joint());
					m_From.PrivateOverheadMessage( MessageType.Label, 090, true,"A joint is placed in your backpack.", m_From.NetState );
				}
				else
				m_Vendor.PrivateOverheadMessage( MessageType.Label, 090, true,"You don't have enough money in your bank.", m_From.NetState );
			}

			if ( info.ButtonID == 3) // Rolling Paper
			{
				if ( Banker.Withdraw( m_From, 10 ) )
				{
					m_From.AddToBackpack(new RollingPaper());
					m_From.PrivateOverheadMessage( MessageType.Label, 090, true,"A rolling paper is placed in your backpack.", m_From.NetState );
				}
				else
				m_Vendor.PrivateOverheadMessage( MessageType.Label, 090, true,"You don't have enough money in your bank.", m_From.NetState );
			}

			if ( info.ButtonID == 4) // Marijuana
			{
				if ( Banker.Withdraw( m_From, 200 ) )
				{
					m_From.AddToBackpack(new Marijuana());
					m_From.PrivateOverheadMessage( MessageType.Label, 090, true,"Some marijuana's placed in your backpack.", m_From.NetState );
				}
				else
				m_Vendor.PrivateOverheadMessage( MessageType.Label, 090, true,"You don't have enough money in your bank.", m_From.NetState );
			}
			if ( info.ButtonID == 5) // MarijuanaSeeds
			   {
				if ( Banker.Withdraw( m_From, 500 ) )
				{
					m_From.AddToBackpack(new MarijuanaSeeds());
					m_From.PrivateOverheadMessage( MessageType.Label, 090, true,"Some MarijuanaSeed is placed in your backpack.", m_From.NetState );
				}
				else
				m_Vendor.PrivateOverheadMessage( MessageType.Label, 090, true,"You don't have enough money in your bank.", m_From.NetState );
			}
			
			if ( info.ButtonID == 6 ) // Cigarette
               {
				if ( Banker.Withdraw( m_From, 250 ) )
				{
					m_From.AddToBackpack(new KarmaBong());
					m_From.PrivateOverheadMessage( MessageType.Label, 090, true,"A Karma Bong is placed in your backpack.", m_From.NetState );
				}
				else
				m_Vendor.PrivateOverheadMessage( MessageType.Label, 090, true,"You don't have enough money in your bank.", m_From.NetState );
			}

		}
	}
}
