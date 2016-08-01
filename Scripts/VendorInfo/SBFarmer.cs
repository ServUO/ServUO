using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles 
{ 
    public class SBFarmer : SBInfo 
    { 
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBFarmer() 
        { 
        }

        public override IShopSellInfo SellInfo
        {
            get
            {
                return this.m_SellInfo;
            }
        }
        public override List<GenericBuyInfo> BuyInfo
        {
            get
            {
                return this.m_BuyInfo;
            }
        }

        public class InternalBuyInfo : List<GenericBuyInfo> 
        { 
            public InternalBuyInfo() 
            { 
                this.Add(new GenericBuyInfo(typeof(Cabbage), 5, 20, 0xC7B, 0));
                this.Add(new GenericBuyInfo(typeof(Cantaloupe), 6, 20, 0xC79, 0));
                this.Add(new GenericBuyInfo(typeof(Carrot), 3, 20, 0xC78, 0));
                this.Add(new GenericBuyInfo(typeof(HoneydewMelon), 7, 20, 0xC74, 0));
                this.Add(new GenericBuyInfo(typeof(Squash), 3, 20, 0xC72, 0));
                this.Add(new GenericBuyInfo(typeof(Lettuce), 5, 20, 0xC70, 0));
                this.Add(new GenericBuyInfo(typeof(Onion), 3, 20, 0xC6D, 0));
                this.Add(new GenericBuyInfo(typeof(Pumpkin), 11, 20, 0xC6A, 0));
                this.Add(new GenericBuyInfo(typeof(GreenGourd), 3, 20, 0xC66, 0));
                this.Add(new GenericBuyInfo(typeof(YellowGourd), 3, 20, 0xC64, 0));
                //Add( new GenericBuyInfo( typeof( Turnip ), 6, 20, XXXXXX, 0 ) );
                this.Add(new GenericBuyInfo(typeof(Watermelon), 7, 20, 0xC5C, 0));
                //Add( new GenericBuyInfo( typeof( EarOfCorn ), 3, 20, XXXXXX, 0 ) );
                this.Add(new GenericBuyInfo(typeof(Eggs), 3, 20, 0x9B5, 0));
                this.Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Milk, 7, 20, 0x9AD, 0));
                this.Add(new GenericBuyInfo(typeof(Peach), 3, 20, 0x9D2, 0));
                this.Add(new GenericBuyInfo(typeof(Pear), 3, 20, 0x994, 0));
                this.Add(new GenericBuyInfo(typeof(Lemon), 3, 20, 0x1728, 0));
                this.Add(new GenericBuyInfo(typeof(Lime), 3, 20, 0x172A, 0));
                this.Add(new GenericBuyInfo(typeof(Grapes), 3, 20, 0x9D1, 0));
                this.Add(new GenericBuyInfo(typeof(Apple), 3, 20, 0x9D0, 0));
                this.Add(new GenericBuyInfo(typeof(SheafOfHay), 2, 20, 0xF36, 0));
                this.Add(new GenericBuyInfo(typeof(Hoe), 5, 20, 3897, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo 
        { 
            public InternalSellInfo() 
            { 
                this.Add(typeof(Pitcher), 5);
                this.Add(typeof(Eggs), 1);
                this.Add(typeof(Apple), 1);
                this.Add(typeof(Grapes), 1);
                this.Add(typeof(Watermelon), 3);
                this.Add(typeof(YellowGourd), 1);
                this.Add(typeof(GreenGourd), 1);
                this.Add(typeof(Pumpkin), 5);
                this.Add(typeof(Onion), 1);
                this.Add(typeof(Lettuce), 2);
                this.Add(typeof(Squash), 1);
                this.Add(typeof(Carrot), 1);
                this.Add(typeof(HoneydewMelon), 3);
                this.Add(typeof(Cantaloupe), 3);
                this.Add(typeof(Cabbage), 2);
                this.Add(typeof(Lemon), 1);
                this.Add(typeof(Lime), 1);
                this.Add(typeof(Peach), 1);
                this.Add(typeof(Pear), 1);
                this.Add(typeof(SheafOfHay), 1);
            }
        }
    }
}