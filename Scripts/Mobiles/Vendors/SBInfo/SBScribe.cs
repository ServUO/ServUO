using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBScribe : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBScribe()
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
                this.Add(new GenericBuyInfo(typeof(ScribesPen), 8, 20, 0xFBF, 0));
                this.Add(new GenericBuyInfo(typeof(BlankScroll), 5, 999, 0x0E34, 0));
                this.Add(new GenericBuyInfo(typeof(ScribesPen), 8, 20, 0xFC0, 0));
                this.Add(new GenericBuyInfo(typeof(BrownBook), 15, 10, 0xFEF, 0));
                this.Add(new GenericBuyInfo(typeof(TanBook), 15, 10, 0xFF0, 0));
                this.Add(new GenericBuyInfo(typeof(BlueBook), 15, 10, 0xFF2, 0));
                //Add( new GenericBuyInfo( "1041267", typeof( Runebook ), 3500, 10, 0xEFA, 0x461 ) );
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(ScribesPen), 4);
                this.Add(typeof(BrownBook), 7);
                this.Add(typeof(TanBook), 7);
                this.Add(typeof(BlueBook), 7);
                this.Add(typeof(BlankScroll), 3);
            }
        }
    }
}