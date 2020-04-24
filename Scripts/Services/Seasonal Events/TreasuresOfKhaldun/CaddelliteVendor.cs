using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.Khaldun
{
    public class CaddelliteVendor : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBCaddelliteTinker(this));
        }

        public static CaddelliteVendor InstanceTram { get; set; }
        public static CaddelliteVendor InstanceFel { get; set; }

        [Constructable]
        public CaddelliteVendor()
            : base("the Tinker")
        {
        }

        public override void InitBody()
        {
            base.InitBody();

            Name = NameList.RandomName("male");
            CantWalk = true;

            Hue = Utility.RandomSkinHue();
            Body = 0x190;
            HairItemID = Race.RandomHair(false);
            HairHue = Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            SetWearable(new SmithyHammer());
            SetWearable(new LongPants(), Utility.RandomBlueHue());
            SetWearable(new Shirt());
            SetWearable(new FullApron());
            SetWearable(new ThighBoots());
        }

        public CaddelliteVendor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (Map == Map.Felucca)
            {
                InstanceFel = this;
            }

            if (Map == Map.Trammel)
            {
                InstanceTram = this;
            }
        }
    }

    public class SBCaddelliteTinker : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo;
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBCaddelliteTinker(BaseVendor owner)
        {
            m_BuyInfo = new InternalBuyInfo(owner);
        }

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo(BaseVendor owner)
            {
                Add(new GenericBuyInfo(typeof(CaddellitePickaxe), 101267, 500, 3718, 0));
                Add(new GenericBuyInfo(typeof(CaddelliteHatchet), 101266, 500, 3907, 0));
                Add(new GenericBuyInfo(typeof(CaddelliteFishingPole), 101265, 500, 3520, 0));
            }
        }
        public class InternalSellInfo : GenericSellInfo
        {
        }

    }
}
