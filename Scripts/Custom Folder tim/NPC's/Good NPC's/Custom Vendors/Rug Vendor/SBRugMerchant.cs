using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBRugMercant: SBInfo
	{
		private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBRugMercant()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo("a persian rug deed", typeof( FancyCarpetDeed ), 50000, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo("an asian rug deed", typeof( AsianCarpetDeed ), 50000, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo("a fancy blue rug deed", typeof( FancyCarpetDeed ), 25000, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo("a blue rug deed", typeof( BlueCarpetDeed ), 20000, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo("a plain blue rug deed", typeof( PlainBlueCarpetDeed ), 10000, 20, 0x14F0, 0 ) );
				Add( new GenericBuyInfo("a plain red rug deed", typeof( RedCarpetDeed ), 10000, 20, 0x14F0, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( Cloth ), 7 );
			}
		}
	}
}
