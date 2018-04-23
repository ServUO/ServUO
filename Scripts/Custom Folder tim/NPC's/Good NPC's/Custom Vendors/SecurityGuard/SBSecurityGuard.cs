/*
 * Created by SharpDevelop.
 * Date: 2/16/2008
 * Time: 11:21 PM
 * 
*/
using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBSecurityGuard : SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBSecurityGuard()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( "1047016", typeof( CommodityDeed ), 5, 20, 0x14F0, 0x47 ) );
				Add( new GenericBuyInfo( typeof( Backpack ), 15, 20, 0x9B2, 0 ) );
				Add( new GenericBuyInfo( typeof( Pouch ), 6, 20, 0xE79, 0 ) );
				Add( new GenericBuyInfo( typeof( Bag ), 6, 20, 0xE76, 0 ) );
				Add( new GenericBuyInfo( typeof( RecallRune ), 15, 10, 0x1f14, 0 ) );
				Add( new GenericBuyInfo( typeof( Bandage ), 5, 20, 0xE21, 0 ) );
				Add( new GenericBuyInfo( typeof( QuarterStaff ), 19, 20, 0xE89, 0 ) );
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
