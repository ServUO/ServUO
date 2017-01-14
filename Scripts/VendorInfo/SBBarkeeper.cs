using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBBarkeeper : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBBarkeeper()
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
                this.Add(new BeverageBuyInfo(typeof(BeverageBottle), BeverageType.Ale, 7, 20, 0x99F, 0));
                this.Add(new BeverageBuyInfo(typeof(BeverageBottle), BeverageType.Wine, 7, 20, 0x9C7, 0));
                this.Add(new BeverageBuyInfo(typeof(BeverageBottle), BeverageType.Liquor, 7, 20, 0x99B, 0));
                this.Add(new BeverageBuyInfo(typeof(Jug), BeverageType.Cider, 13, 20, 0x9C8, 0));
                this.Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Milk, 7, 20, 0x9F0, 0));
                this.Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Ale, 11, 20, 0x1F95, 0));
                this.Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Cider, 11, 20, 0x1F97, 0));
                this.Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Liquor, 11, 20, 0x1F99, 0));
                this.Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Wine, 11, 20, 0x1F9B, 0));
                this.Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Water, 11, 20, 0x1F9D, 0));

                this.Add(new GenericBuyInfo(typeof(BreadLoaf), 6, 10, 0x103B, 0));
                this.Add(new GenericBuyInfo(typeof(CheeseWheel), 21, 10, 0x97E, 0));
                this.Add(new GenericBuyInfo(typeof(CookedBird), 17, 20, 0x9B7, 0));
                this.Add(new GenericBuyInfo(typeof(LambLeg), 8, 20, 0x160A, 0));

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

                this.Add(new GenericBuyInfo(typeof(ApplePie), 7, 20, 0x1041, 0)); //OSI just has Pie, not Apple/Fruit/Meat

                this.Add(new GenericBuyInfo("1016450", typeof(Chessboard), 2, 20, 0xFA6, 0));
                this.Add(new GenericBuyInfo("1016449", typeof(CheckerBoard), 2, 20, 0xFA6, 0));
                this.Add(new GenericBuyInfo(typeof(Backgammon), 2, 20, 0xE1C, 0));
                this.Add(new GenericBuyInfo(typeof(Dices), 2, 20, 0xFA7, 0));
                this.Add(new GenericBuyInfo("1041243", typeof(ContractOfEmployment), 1252, 20, 0x14F0, 0));
                this.Add(new GenericBuyInfo("a barkeep contract", typeof(BarkeepContract), 1252, 20, 0x14F0, 0));
                if (Multis.BaseHouse.NewVendorSystem)
                    this.Add(new GenericBuyInfo("1062332", typeof(VendorRentalContract), 1252, 20, 0x14F0, 0x672));
                /*if ( Map == Tokuno )
                {
                Add( new GenericBuyInfo( typeof( Wasabi ), 2, 20, 0x24E8, 0 ) );
                Add( new GenericBuyInfo( typeof( Wasabi ), 2, 20, 0x24E9, 0 ) );
                Add( new GenericBuyInfo( typeof( BentoBox ), 6, 20, 0x2836, 0 ) );
                Add( new GenericBuyInfo( typeof( BentoBox ), 6, 20, 0x2837, 0 ) );
                Add( new GenericBuyInfo( typeof( GreenTeaBasket ), 2, 20, 0x284B, 0 ) );
                }*/
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
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
                this.Add(typeof(BeverageBottle), 3);
                this.Add(typeof(Jug), 6);
                this.Add(typeof(Pitcher), 5);
                this.Add(typeof(GlassMug), 1);
                this.Add(typeof(BreadLoaf), 3);
                this.Add(typeof(CheeseWheel), 12);
                this.Add(typeof(Ribs), 6);
                this.Add(typeof(Peach), 1);
                this.Add(typeof(Pear), 1);
                this.Add(typeof(Grapes), 1);
                this.Add(typeof(Apple), 1);
                this.Add(typeof(Banana), 1);
                this.Add(typeof(Candle), 3);
                this.Add(typeof(Chessboard), 1);
                this.Add(typeof(CheckerBoard), 1);
                this.Add(typeof(Backgammon), 1);
                this.Add(typeof(Dices), 1);
                this.Add(typeof(ContractOfEmployment), 626);
            }
        }
    }
}