using System; 
using System.Collections.Generic; 
using Server.Items; 

namespace Server.Mobiles 
{ 
	public class SBBarber : SBInfo 
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBBarber() 
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{ 
				Add( new GenericBuyInfo( "special beard dye", typeof( SpecialBeardDye ), 20000, 20, 0xE26, 0 ) ); 
				Add( new GenericBuyInfo( "special hair dye", typeof( SpecialHairDye ), 20000, 20, 0xE26, 0 ) ); 
				Add( new GenericBuyInfo( "barber scissors", typeof( BarberScissors ), 5000, 20, 0xDFC, 0 ) );
				Add( new GenericBuyInfo( "brush", typeof( HairBrush ), 5000, 20, 0x1372, 0 ) );
				Add( new GenericBuyInfo( "razor", typeof( Razor ), 5000, 20, 0xEC4, 0 ) );
				Add( new GenericBuyInfo( "hair growth elixir", typeof( HairGrowthElixir ), 60, 20, 0xE26, 0 ) );
				Add( new GenericBuyInfo( "1041060", typeof( HairDye ), 60, 20, 0xEFF, 0 ) ); 
			} 
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{ 
				Add( typeof( HairDye ), 30 ); 
				Add( typeof( SpecialBeardDye ), 10000 ); 
				Add( typeof( SpecialHairDye ), 10000 );
				Add( typeof( BarberScissors ), 2500 );
				Add( typeof( HairBrush ), 2500 );
				Add( typeof( Razor ), 2500 );
				Add( typeof( HairGrowthElixir ), 30 ); 
			} 
		} 
	} 
}