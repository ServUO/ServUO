using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Factions
{
    public class FactionBottleVendor : BaseFactionVendor
    {
        public FactionBottleVendor(Town town, Faction faction)
            : base(town, faction, "the Bottle Seller")
        {
            this.SetSkill(SkillName.Alchemy, 85.0, 100.0);
            this.SetSkill(SkillName.TasteID, 65.0, 88.0);
        }

        public FactionBottleVendor(Serial serial)
            : base(serial)
        {
        }

        public override VendorShoeType ShoeType
        {
            get
            {
                return Utility.RandomBool() ? VendorShoeType.Shoes : VendorShoeType.Sandals;
            }
        }
        public override void InitSBInfo()
        {
            this.SBInfos.Add(new SBFactionBottle());
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            this.AddItem(new Robe(Utility.RandomPinkHue()));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class SBFactionBottle : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBFactionBottle()
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
                for (int i = 0; i < 5; ++i)
                    this.Add(new GenericBuyInfo(typeof(Bottle), 5, 20, 0xF0E, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }
}