using System; 
using System.Collections.Generic; 
using Server.Items;


namespace Server.Mobiles 
{ 
	public class SBNarcotics : SBInfo
	{ 
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo(); 
		private IShopSellInfo m_SellInfo = new InternalSellInfo(); 

		public SBNarcotics()
		{ 
		} 

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } } 
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } } 

		public class InternalBuyInfo : List<GenericBuyInfo> 
		{ 
			public InternalBuyInfo() 
			{ 
        	Add( new GenericBuyInfo( "Water Bong",typeof( WaterBong), 250, 20, 3624, 1289 ) ); 
	    	Add( new GenericBuyInfo( "Joint",typeof( Joint ), 50, 20, 0x1420, 1153 ) ); 
		    Add( new GenericBuyInfo( "Rolling Paper",typeof( RollingPaper ), 10, 20, 0xFEF, 1153 ) ); 
		    Add( new GenericBuyInfo( "Marijuana",typeof(  Marijuana ), 200, 20, 0x18E5, 0 ) );
            Add( new GenericBuyInfo( "MarijuanaSeeds",typeof(  MarijuanaSeeds ), 500, 20, 0x1AA2, 0 ) );
            Add( new GenericBuyInfo( "KarmaBong",typeof(  KarmaBong ), 100, 20, 0x183A, 0 ) );
			}
		} 

		public class InternalSellInfo : GenericSellInfo 
		{ 
			public InternalSellInfo() 
			{ 
				
            }
		} 
	} 
}
