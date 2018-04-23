using System; 
using System.Collections.Generic; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBMinorArtifactBuyer : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBMinorArtifactBuyer()
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
			                 Add( typeof( GoldBricks ), 2000 );
                                                                      Add( typeof( PhillipsWoodenSteed ), 2000 );
			                 Add( typeof( AlchemistsBauble ), 2000 );
                                                                      Add( typeof( ArcticDeathDealer ), 2000 );
			                 Add( typeof( BlazeOfDeath ), 2000 );
                                                                      Add( typeof( BowOfTheJukaKing ), 2000 );
			                 Add( typeof( BurglarsBandana ), 2000 );
                                                                      Add( typeof( CavortingClub ), 2000 );
			                 Add( typeof( EnchantedTitanLegBone ), 2000 );
                                                                      Add( typeof( GwennosHarp ), 2000 );
			                 Add( typeof( IolosLute ),  2000 );
                                                                      Add( typeof( LunaLance ), 2000 );
			                 Add( typeof( NightsKiss ), 2000 ); 
                                                                      Add( typeof( NoxRangersHeavyCrossbow ), 2000 );
			                 Add( typeof( OrcishVisage ), 2000 );
                                                                      Add( typeof( PolarBearMask ), 2000 );
			                 Add( typeof( ShieldOfInvulnerability ), 2000 );
                                                                      Add( typeof( StaffOfPower ), 2000 );
			                 Add( typeof( VioletCourage ), 2000 ); 
                                                                      Add( typeof( HeartOfTheLion ), 2000 ); 
			                 Add( typeof( WrathOfTheDryad ), 2000 );
                                                                      Add( typeof( PixieSwatter ), 2000 ); 
			                 Add( typeof( GlovesOfThePugilist ), 2000 ); 
                                                                      Add( typeof( AdmiralsHeartyRum ), 3 );
                                                                      Add( typeof( CandelabraOfSouls ), 2000 );
                                                                      Add( typeof( GhostShipAnchor ), 2000 );
                                                                      Add( typeof( ShipModelOfTheHMSCape ), 2000 );
                                                                      Add( typeof( CaptainQuacklebushsCutlass ), 2000 );
                                                                      Add( typeof( DreadPirateHat ), 2000 );
                                                                      Add( typeof( SeahorseStatuette ), 2000 );
                                                                                            	
			}
		}
	}
}