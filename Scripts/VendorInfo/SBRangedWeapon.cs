using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBRangedWeapon : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBRangedWeapon()
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
                this.Add(new GenericBuyInfo(typeof(Crossbow), 55, 20, 0xF50, 0));
                this.Add(new GenericBuyInfo(typeof(HeavyCrossbow), 55, 20, 0x13FD, 0));
                if (Core.AOS)
                {
                    this.Add(new GenericBuyInfo(typeof(RepeatingCrossbow), 46, 20, 0x26C3, 0));
                    this.Add(new GenericBuyInfo(typeof(CompositeBow), 45, 20, 0x26C2, 0));
                }
                this.Add(new GenericBuyInfo(typeof(Bolt), 2, Utility.Random(30, 60), 0x1BFB, 0));
                this.Add(new GenericBuyInfo(typeof(Bow), 40, 20, 0x13B2, 0));
                this.Add(new GenericBuyInfo(typeof(Arrow), 2, Utility.Random(30, 60), 0xF3F, 0));
                this.Add(new GenericBuyInfo(typeof(Feather), 2, Utility.Random(30, 60), 0x1BD1, 0));
                this.Add(new GenericBuyInfo(typeof(Shaft), 3, Utility.Random(30, 60), 0x1BD4, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(Bolt), 1);
                this.Add(typeof(Arrow), 1);
                this.Add(typeof(Shaft), 1);
                this.Add(typeof(Feather), 1);			

                this.Add(typeof(HeavyCrossbow), 27);
                this.Add(typeof(Bow), 17);
                this.Add(typeof(Crossbow), 25); 

                if (Core.AOS)
                {
                    this.Add(typeof(CompositeBow), 23);
                    this.Add(typeof(RepeatingCrossbow), 22);
                }
            }
        }
    }
}