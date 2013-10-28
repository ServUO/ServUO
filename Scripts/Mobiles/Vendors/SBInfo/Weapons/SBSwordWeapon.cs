using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBSwordWeapon : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBSwordWeapon()
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
                this.Add(new GenericBuyInfo(typeof(Cutlass), 24, 20, 0x1441, 0));
                this.Add(new GenericBuyInfo(typeof(Katana), 33, 20, 0x13FF, 0));
                this.Add(new GenericBuyInfo(typeof(Kryss), 32, 20, 0x1401, 0));
                this.Add(new GenericBuyInfo(typeof(Broadsword), 35, 20, 0xF5E, 0));
                this.Add(new GenericBuyInfo(typeof(Longsword), 55, 20, 0xF61, 0));
                this.Add(new GenericBuyInfo(typeof(ThinLongsword), 27, 20, 0x13B8, 0));
                this.Add(new GenericBuyInfo(typeof(VikingSword), 55, 20, 0x13B9, 0));
                this.Add(new GenericBuyInfo(typeof(Scimitar), 36, 20, 0x13B6, 0));

                if (Core.AOS)
                {
                    this.Add(new GenericBuyInfo(typeof(BoneHarvester), 35, 20, 0x26BB, 0));
                    this.Add(new GenericBuyInfo(typeof(CrescentBlade), 37, 20, 0x26C1, 0));
                    this.Add(new GenericBuyInfo(typeof(DoubleBladedStaff), 35, 20, 0x26BF, 0));
                    this.Add(new GenericBuyInfo(typeof(Lance), 34, 20, 0x26C0, 0));
                    this.Add(new GenericBuyInfo(typeof(Pike), 39, 20, 0x26BE, 0));
                    this.Add(new GenericBuyInfo(typeof(Scythe), 39, 20, 0x26BA, 0));
                }
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(Broadsword), 17);
                this.Add(typeof(Cutlass), 12);
                this.Add(typeof(Katana), 16);
                this.Add(typeof(Kryss), 16);
                this.Add(typeof(Longsword), 27);
                this.Add(typeof(Scimitar), 18);
                this.Add(typeof(ThinLongsword), 13);
                this.Add(typeof(VikingSword), 27);

                if (Core.AOS)
                {
                    this.Add(typeof(Scythe), 19);
                    this.Add(typeof(BoneHarvester), 17);
                    this.Add(typeof(Scepter), 18);
                    this.Add(typeof(BladedStaff), 16);
                    this.Add(typeof(Pike), 19);
                    this.Add(typeof(DoubleBladedStaff), 17);
                    this.Add(typeof(Lance), 17);
                    this.Add(typeof(CrescentBlade), 18);
                }
            }
        }
    }
}