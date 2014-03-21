
using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBVendorMallManager : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBVendorMallManager()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
			                Add( new GenericBuyInfo( "1041243", typeof( TownContractOfEmployment ), 5000, 20, 0x14F0, 0 ) );
                            Add( new GenericBuyInfo( typeof( Backpack ), 15, 20, 0x9B2, 0 ) );
			                Add( new GenericBuyInfo( typeof( Pouch ), 3, 20, 0xE79, 0 ) );			               
                            Add( new GenericBuyInfo( typeof( Bag ), 3, 20, 0xE76, 0 ) );
                            Add( new GenericBuyInfo( typeof( WoodenBox ), 14, 20, 0xE7D, 0 ) );
                            Add( new GenericBuyInfo( typeof( BlueBook ), 15, 20, 0xFF2, 0 ) );
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