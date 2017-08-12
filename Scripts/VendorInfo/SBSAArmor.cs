using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBSAArmor : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBSAArmor()
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
                Add(new GenericBuyInfo(typeof(GargishPlateArms), 363, 20, 0x307, 0));
                Add(new GenericBuyInfo(typeof(GargishPlateArms), 328, 20, 0x308, 0));
                Add(new GenericBuyInfo(typeof(GargishPlateChest), 481, 20, 0x309, 0));
                Add(new GenericBuyInfo(typeof(GargishPlateChest), 462, 20, 0x30A, 0));
                Add(new GenericBuyInfo(typeof(GargishPlateKilt), 338, 20, 0x30B, 0));
                Add(new GenericBuyInfo(typeof(GargishPlateKilt), 370, 20, 0x30C, 0));
                Add(new GenericBuyInfo(typeof(GargishPlateLegs), 372, 20, 0x30D, 0));
                Add(new GenericBuyInfo(typeof(GargishPlateLegs), 355, 20, 0x30E, 0));
                Add(new GenericBuyInfo(typeof(GargishStoneArms), 116, 20, 0x283, 0));
                Add(new GenericBuyInfo(typeof(GargishStoneArms), 121, 20, 0x284, 0));
                Add(new GenericBuyInfo(typeof(GargishStoneChest), 135, 20, 0x285, 0));
                Add(new GenericBuyInfo(typeof(GargishStoneChest), 142, 20, 0x286, 0));
                Add(new GenericBuyInfo(typeof(GargishStoneKilt), 135, 20, 0x287, 0));
                Add(new GenericBuyInfo(typeof(GargishStoneKilt), 132, 20, 0x288, 0));
                Add(new GenericBuyInfo(typeof(GargishStoneLegs), 116, 20, 0x289, 0));
                Add(new GenericBuyInfo(typeof(GargishStoneLegs), 113, 20, 0x28A, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(GargishPlateArms), 181);
                Add(typeof(GargishPlateArms), 164);
                Add(typeof(GargishPlateChest), 240);
                Add(typeof(GargishPlateChest), 231);
                Add(typeof(GargishPlateKilt), 169);
                Add(typeof(GargishPlateKilt), 185);
                Add(typeof(GargishPlateLegs), 186);
                Add(typeof(GargishPlateLegs), 177);
                Add(typeof(GargishStoneArms), 58);
                Add(typeof(GargishStoneArms), 60);
                Add(typeof(GargishStoneChest), 67);
                Add(typeof(GargishStoneChest), 71);
                Add(typeof(GargishStoneKilt), 67);
                Add(typeof(GargishStoneKilt), 66);
                Add(typeof(GargishStoneLegs), 58);
                Add(typeof(GargishStoneLegs), 56);
            }
        }
    }
}