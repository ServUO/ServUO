using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Factions
{
    public class FactionBoardVendor : BaseFactionVendor
    {
        public FactionBoardVendor(Town town, Faction faction)
            : base(town, faction, "the LumberMan")// NOTE: title inconsistant, as OSI
        {
            this.SetSkill(SkillName.Carpentry, 85.0, 100.0);
            this.SetSkill(SkillName.Lumberjacking, 60.0, 83.0);
        }

        public FactionBoardVendor(Serial serial)
            : base(serial)
        {
        }

        public override void InitSBInfo()
        {
            this.SBInfos.Add(new SBFactionBoard());
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

    public class SBFactionBoard : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBFactionBoard()
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
                    this.Add(new GenericBuyInfo(typeof(Board), 3, 20, 0x1BD7, 0));
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