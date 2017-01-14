using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles 
{ 
    public class SBCook : SBInfo 
    { 
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBCook() 
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
                this.Add(new GenericBuyInfo(typeof(BreadLoaf), 5, 20, 0x103B, 0));
                this.Add(new GenericBuyInfo(typeof(BreadLoaf), 5, 20, 0x103C, 0));
                this.Add(new GenericBuyInfo(typeof(ApplePie), 7, 20, 0x1041, 0)); //OSI just has Pie, not Apple/Fruit/Meat
                this.Add(new GenericBuyInfo(typeof(Cake), 13, 20, 0x9E9, 0));
                this.Add(new GenericBuyInfo(typeof(Muffins), 3, 20, 0x9EA, 0));

                this.Add(new GenericBuyInfo(typeof(CheeseWheel), 21, 10, 0x97E, 0));
                this.Add(new GenericBuyInfo(typeof(CookedBird), 17, 20, 0x9B7, 0));
                this.Add(new GenericBuyInfo(typeof(LambLeg), 8, 20, 0x160A, 0));
                this.Add(new GenericBuyInfo(typeof(ChickenLeg), 5, 20, 0x1608, 0));

                this.Add(new GenericBuyInfo(typeof(WoodenBowlOfCarrots), 3, 20, 0x15F9, 0));
                this.Add(new GenericBuyInfo(typeof(WoodenBowlOfCorn), 3, 20, 0x15FA, 0));
                this.Add(new GenericBuyInfo(typeof(WoodenBowlOfLettuce), 3, 20, 0x15FB, 0));
                this.Add(new GenericBuyInfo(typeof(WoodenBowlOfPeas), 3, 20, 0x15FC, 0));
                this.Add(new GenericBuyInfo(typeof(EmptyPewterBowl), 2, 20, 0x15FD, 0));
                this.Add(new GenericBuyInfo(typeof(PewterBowlOfCorn), 3, 20, 0x15FE, 0));
                this.Add(new GenericBuyInfo(typeof(PewterBowlOfLettuce), 3, 20, 0x15FF, 0));
                this.Add(new GenericBuyInfo(typeof(PewterBowlOfPeas), 3, 20, 0x1601, 0));
                this.Add(new GenericBuyInfo(typeof(PewterBowlOfPotatos), 3, 20, 0x1601, 0));
                this.Add(new GenericBuyInfo(typeof(WoodenBowlOfStew), 3, 20, 0x1604, 0));
                this.Add(new GenericBuyInfo(typeof(WoodenBowlOfTomatoSoup), 3, 20, 0x1606, 0));

                this.Add(new GenericBuyInfo(typeof(RoastPig), 106, 20, 0x9BB, 0));
                this.Add(new GenericBuyInfo(typeof(SackFlour), 3, 20, 0x1039, 0));
                this.Add(new GenericBuyInfo(typeof(JarHoney), 3, 20, 0x9EC, 0));
                this.Add(new GenericBuyInfo(typeof(RollingPin), 2, 20, 0x1043, 0));
                this.Add(new GenericBuyInfo(typeof(FlourSifter), 2, 20, 0x103E, 0));
                this.Add(new GenericBuyInfo("1044567", typeof(Skillet), 3, 20, 0x97F, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo 
        { 
            public InternalSellInfo() 
            { 
                this.Add(typeof(CheeseWheel), 12);
                this.Add(typeof(CookedBird), 8);
                this.Add(typeof(RoastPig), 53);
                this.Add(typeof(Cake), 5);
                this.Add(typeof(JarHoney), 1);
                this.Add(typeof(SackFlour), 1);
                this.Add(typeof(BreadLoaf), 2);
                this.Add(typeof(ChickenLeg), 3);
                this.Add(typeof(LambLeg), 4);
                this.Add(typeof(Skillet), 1);
                this.Add(typeof(FlourSifter), 1);
                this.Add(typeof(RollingPin), 1);
                this.Add(typeof(Muffins), 1);
                this.Add(typeof(ApplePie), 3);

                this.Add(typeof(WoodenBowlOfCarrots), 1);
                this.Add(typeof(WoodenBowlOfCorn), 1);
                this.Add(typeof(WoodenBowlOfLettuce), 1);
                this.Add(typeof(WoodenBowlOfPeas), 1);
                this.Add(typeof(EmptyPewterBowl), 1);
                this.Add(typeof(PewterBowlOfCorn), 1);
                this.Add(typeof(PewterBowlOfLettuce), 1);
                this.Add(typeof(PewterBowlOfPeas), 1);
                this.Add(typeof(PewterBowlOfPotatos), 1);
                this.Add(typeof(WoodenBowlOfStew), 1);
                this.Add(typeof(WoodenBowlOfTomatoSoup), 1);
            }
        }
    }
}