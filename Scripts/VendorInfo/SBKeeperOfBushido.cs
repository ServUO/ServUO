using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBKeeperOfBushido : SBInfo
	{
		private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
		public SBKeeperOfBushido()
		{
		}

		public override IShopSellInfo SellInfo
		{
			get
			{
				return m_SellInfo;
			}
		}
		public override List<GenericBuyInfo> BuyInfo
		{
			get
			{
				return m_BuyInfo;
			}
		}
		
		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
				Add(new GenericBuyInfo(typeof(BookOfBushido), 500, 20, 9100, 0));
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