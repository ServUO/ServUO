using System;
using System.Collections.Generic;
using Server.Multis.Deeds;

namespace Server.Mobiles
{
    public class SBHouseDeed : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBHouseDeed()
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
                this.Add(new GenericBuyInfo("deed to a stone-and-plaster house", typeof(StonePlasterHouseDeed), 43800, 20, 0x14F0, 0));
                this.Add(new GenericBuyInfo("deed to a field stone house", typeof(FieldStoneHouseDeed), 43800, 20, 0x14F0, 0));
                this.Add(new GenericBuyInfo("deed to a small brick house", typeof(SmallBrickHouseDeed), 43800, 20, 0x14F0, 0));
                this.Add(new GenericBuyInfo("deed to a wooden house", typeof(WoodHouseDeed), 43800, 20, 0x14F0, 0));
                this.Add(new GenericBuyInfo("deed to a wood-and-plaster house", typeof(WoodPlasterHouseDeed), 43800, 20, 0x14F0, 0));
                this.Add(new GenericBuyInfo("deed to a thatched-roof cottage", typeof(ThatchedRoofCottageDeed), 43800, 20, 0x14F0, 0));
                this.Add(new GenericBuyInfo("deed to a brick house", typeof(BrickHouseDeed), 144500, 20, 0x14F0, 0));
                this.Add(new GenericBuyInfo("deed to a two-story wood-and-plaster house", typeof(TwoStoryWoodPlasterHouseDeed), 192400, 20, 0x14F0, 0));
                this.Add(new GenericBuyInfo("deed to a tower", typeof(TowerDeed), 433200, 20, 0x14F0, 0));
                this.Add(new GenericBuyInfo("deed to a small stone keep", typeof(KeepDeed), 665200, 20, 0x14F0, 0));
                this.Add(new GenericBuyInfo("deed to a castle", typeof(CastleDeed), 1022800, 20, 0x14F0, 0));
                this.Add(new GenericBuyInfo("deed to a large house with patio", typeof(LargePatioDeed), 152800, 20, 0x14F0, 0));
                this.Add(new GenericBuyInfo("deed to a marble house with patio", typeof(LargeMarbleDeed), 192000, 20, 0x14F0, 0));
                this.Add(new GenericBuyInfo("deed to a small stone tower", typeof(SmallTowerDeed), 88500, 20, 0x14F0, 0));
                this.Add(new GenericBuyInfo("deed to a two story log cabin", typeof(LogCabinDeed), 97800, 20, 0x14F0, 0));
                this.Add(new GenericBuyInfo("deed to a sandstone house with patio", typeof(SandstonePatioDeed), 90900, 20, 0x14F0, 0));
                this.Add(new GenericBuyInfo("deed to a two story villa", typeof(VillaDeed), 136500, 20, 0x14F0, 0));
                this.Add(new GenericBuyInfo("deed to a small stone workshop", typeof(StoneWorkshopDeed), 60600, 20, 0x14F0, 0));
                this.Add(new GenericBuyInfo("deed to a small marble workshop", typeof(MarbleWorkshopDeed), 63000, 20, 0x14F0, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                /*Add( typeof( StonePlasterHouseDeed ), 43800 );
                Add( typeof( FieldStoneHouseDeed ), 43800 );
                Add( typeof( SmallBrickHouseDeed ), 43800 );
                Add( typeof( WoodHouseDeed ), 43800 );
                Add( typeof( WoodPlasterHouseDeed ), 43800 );
                Add( typeof( ThatchedRoofCottageDeed ), 43800 );
                Add( typeof( BrickHouseDeed ), 144500 );
                Add( typeof( TwoStoryWoodPlasterHouseDeed ), 192400 );
                Add( typeof( TowerDeed ), 433200 );
                Add( typeof( KeepDeed ), 665200 );
                Add( typeof( CastleDeed ), 1022800 );
                Add( typeof( LargePatioDeed ), 152800 );
                Add( typeof( LargeMarbleDeed ), 192800 );
                Add( typeof( SmallTowerDeed ), 88500 );
                Add( typeof( LogCabinDeed ), 97800 );
                Add( typeof( SandstonePatioDeed ), 90900 );
                Add( typeof( VillaDeed ), 136500 );
                Add( typeof( StoneWorkshopDeed ), 60600 );
                Add( typeof( MarbleWorkshopDeed ), 60300 );
                Add( typeof( SmallBrickHouseDeed ), 43800 );*/
            }
        }
    }
}