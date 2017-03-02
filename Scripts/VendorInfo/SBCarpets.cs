using System;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
	public class SBCarpets : SBInfo
	{
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBCarpets()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
			private void AddCarpet( int itemId, int price )
			{
				Add( new GenericBuyInfo( typeof( DecorativeCarpet ), price, 500, itemId, 0, new object[] { itemId } ) );
			}

			private void AddCarpetGroup( int baseId, int count, int price )
			{
				for ( int i = 0; i < count; i++ )
					AddCarpet( baseId + i, price );
			}

			public InternalBuyInfo()
			{
				// Blue carpets
				AddCarpetGroup( 0x56B8, 13, 100 );

				// Red carpets
				AddCarpetGroup( 0x56C5, 11, 100 );

				// Blue/gold carpets
				AddCarpetGroup( 0x56D0, 9, 200 );

				// Brown/gold carpets
				AddCarpetGroup( 0x56D9, 9, 200 );

				// Brown/red carpets
				AddCarpetGroup( 0x56E2, 9, 200 );

				// Fancy carpets
				AddCarpetGroup( 0x56EB, 10, 500 );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( Scissors ), 6 );
				Add( typeof( Dyes ), 4 );
				Add( typeof( DyeTub ), 4 );
				Add( typeof( BoltOfCloth ), 60 );
				Add( typeof( LightYarnUnraveled ), 9 );
				Add( typeof( LightYarn ), 9 );
				Add( typeof( DarkYarn ), 9 );
			}
		}
	}
}