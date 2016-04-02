using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBNecromancer : SBInfo
	{
		private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
		public SBNecromancer()
		{
		}

		public override IShopSellInfo SellInfo { get { return this.m_SellInfo; } }
		public override List<GenericBuyInfo> BuyInfo { get { return this.m_BuyInfo; } }

		public class InternalBuyInfo : List<GenericBuyInfo>
		{
			public InternalBuyInfo()
			{
				this.Add( new GenericBuyInfo( typeof( BlackPearl ), 5, 20, 0xF7A, 0 ) );
				this.Add( new GenericBuyInfo( typeof( Bloodmoss ), 7, 20, 0xF7B, 0 ) );
				this.Add( new GenericBuyInfo( typeof( MandrakeRoot ), 3, 20, 0xF86, 0 ) );
				this.Add( new GenericBuyInfo( typeof( Garlic ), 3, 20, 0xF84, 0 ) );
				this.Add( new GenericBuyInfo( typeof( Ginseng ), 3, 20, 0xF85, 0 ) );
				this.Add( new GenericBuyInfo( typeof( Nightshade ), 4, 20, 0xF88, 0 ) );
				this.Add( new GenericBuyInfo( typeof( SpidersSilk ), 3, 20, 0xF8D, 0 ) );
				this.Add( new GenericBuyInfo( typeof( SulfurousAsh ), 4, 20, 0xF8C, 0 ) );

				if ( Core.AOS )
				{
					this.Add( new GenericBuyInfo( typeof( BatWing ), 4, 20, 0xF78, 0 ) );
					this.Add( new GenericBuyInfo( typeof( GraveDust ), 4, 20, 0xF8F, 0 ) );
					this.Add( new GenericBuyInfo( typeof( DaemonBlood ), 4, 20, 0xF7D, 0 ) );
					this.Add( new GenericBuyInfo( typeof( NoxCrystal ), 4, 20, 0xF8E, 0 ) );
					this.Add( new GenericBuyInfo( typeof( PigIron ), 4, 20, 0xF8A, 0 ) );

					this.Add( new GenericBuyInfo( typeof( NecromancerSpellbook ), 150, 10, 0x2253, 0 ) );
				}
				
				this.Add(new GenericBuyInfo("1041072", typeof(MagicWizardsHat), 11, 10, 0x1718, Utility.RandomDyedHue()));
                this.Add(new GenericBuyInfo(typeof(ScribesPen), 8, 10, 0xFBF, 0));
                this.Add(new GenericBuyInfo(typeof(BlankScroll), 5, 20, 0x0E34, 0));
				this.Add( new GenericBuyInfo( typeof( RecallRune ), 25, 10, 0x1f14, 0 ) );
				this.Add( new GenericBuyInfo( typeof( Spellbook ), 50, 10, 0xEFA, 0 ) );
			
				Type[] types = Loot.RegularScrollTypes;
			
				for (int i = 0; i < types.Length && i < 8; ++i)
                {
                    int itemID = 0x1F2E + i;

                    if (i == 6)
                        itemID = 0x1F2D;
                    else if (i > 6)
                        --itemID;

                    this.Add(new GenericBuyInfo(types[i], 12 + ((i / 8) * 10), 20, itemID, 0));
                }
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				this.Add( typeof( WizardsHat ), 15 );
				this.Add( typeof( Runebook ), 1250 );
				this.Add( typeof( BlackPearl ), 3 ); 
				this.Add( typeof( Bloodmoss ),4 ); 
				this.Add( typeof( MandrakeRoot ), 2 ); 
				this.Add( typeof( Garlic ), 2 ); 
				this.Add( typeof( Ginseng ), 2 ); 
				this.Add( typeof( Nightshade ), 2 ); 
				this.Add( typeof( SpidersSilk ), 2 ); 
				this.Add( typeof( SulfurousAsh ), 2 );
				this.Add( typeof( RecallRune ), 13 );
				this.Add( typeof( Spellbook ), 25 );
				
				if ( Core.AOS )
				{
				this.Add( typeof( PigIron ), 2 );
				this.Add( typeof( DaemonBlood ), 3 );
				this.Add( typeof( NoxCrystal ), 3 );
				this.Add( typeof( BatWing ), 1 );
				this.Add( typeof( GraveDust ), 1 );
				}

				Type[] types = Loot.RegularScrollTypes;

				for (int i = 0; i < types.Length; ++i)
                    this.Add(types[i], ((i / 8) + 2) * 2);
			}
		}
	}
}