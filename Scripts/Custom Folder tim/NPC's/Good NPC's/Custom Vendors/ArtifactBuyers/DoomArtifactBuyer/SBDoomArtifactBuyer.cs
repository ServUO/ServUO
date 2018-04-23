using System; 
using System.Collections.Generic; 
using Server.Items;


namespace Server.Mobiles 
{ 
	public class SBDoomArtifactBuyer : SBInfo
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBDoomArtifactBuyer()
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{ 
		    
                                                     }
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{ 
			
	                                          Add( typeof( SpiritOfTheTotem ), 20000 );
	                                          Add( typeof( HuntersHeaddress ), 20000 );
	                                          Add( typeof( DivineCountenance ), 20000 );
                                                            Add( typeof( Aegis ), 20000 );
                                                            Add( typeof( ArcaneShield ), 20000 );
	                                          Add( typeof( HatOfTheMagi ), 20000 );
	                                          Add( typeof( AxeOfTheHeavens ), 20000 );
	                                          Add( typeof( BoneCrusher ), 20000 );
	                                          Add( typeof( LegacyOfTheDreadLord ), 20000 );
	                                          Add( typeof( TheBeserkersMaul ), 20000 );
	                                          Add( typeof( TitansHammer ), 20000 );
	                                          Add( typeof( BladeOfInsanity ), 20000 );
	                                          Add( typeof( BreathOfTheDead ), 20000 );
	                                          Add( typeof( SerpentsFang ), 20000 );
	                                          Add( typeof( TheDragonSlayer ), 20000 );
	                                          Add( typeof( ZyronicClaw ), 20000 );
	                                          Add( typeof( BladeOfTheRighteous ), 20000 );
	                                          Add( typeof( Frostbringer ), 20000 );
	                                          Add( typeof( StaffOfTheMagi ), 20000 );
	                                          Add( typeof( TheTaskmaster ), 20000 );
	                                          Add( typeof( ArmorOfFortune ), 20000 );
	                                          Add( typeof( HolyKnightsBreastplate ), 20000 );
	                                          Add( typeof( LeggingsOfBane ), 20000 );
	                                          Add( typeof( ShadowDancerLeggings ), 20000 );
	                                          Add( typeof( GauntletsOfNobility ), 20000 );
	                                          Add( typeof( InquisitorsResolution ), 20000 );
	                                          Add( typeof( MidnightBracers ), 20000 );
	                                          Add( typeof( TunicOfFire ), 20000 );
	                                          Add( typeof( HelmOfInsight ), 20000 );
	                                          Add( typeof( JackalsCollar ), 20000 );
	                                          Add( typeof( OrnateCrownOfTheHarrower ), 20000 );
	                                          Add( typeof( VoiceOfTheFallenKing ), 20000 );
	                                          Add( typeof( BraceletOfHealth ), 20000 );
	                                          Add( typeof( RingOfTheVile ), 20000 );
	                                          Add( typeof( OrnamentOfTheMagician ), 20000 );
	                                          Add( typeof( RingOfTheElements ), 20000 );


	
				
            		                 }
		} 
	} 
}