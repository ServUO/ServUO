using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Factions
{
    public class FactionOreVendor : BaseFactionVendor
    {
        public FactionOreVendor(Town town, Faction faction)
            : base(town, faction, "the Ore Man")
        {
            // NOTE: Skills verified
            this.SetSkill(SkillName.Carpentry, 85.0, 100.0);
            this.SetSkill(SkillName.Lumberjacking, 60.0, 83.0);
        }

        public FactionOreVendor(Serial serial)
            : base(serial)
        {
        }

        public override void InitSBInfo()
        {
            this.SBInfos.Add(new SBFactionOre());
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            this.AddItem(new HalfApron());
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

    public class SBFactionOre : SBInfo
    {
        private static readonly object[] m_FixedSizeArgs = { true };
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBFactionOre()
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
                    this.Add(new GenericBuyInfo(typeof(IronOre), 16, 20, 0x19B8, 0, m_FixedSizeArgs));
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