using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Factions
{
    public class FactionReagentVendor : BaseFactionVendor
    {
        public FactionReagentVendor(Town town, Faction faction)
            : base(town, faction, "the Reagent Man")
        {
            this.SetSkill(SkillName.EvalInt, 65.0, 88.0);
            this.SetSkill(SkillName.Inscribe, 60.0, 83.0);
            this.SetSkill(SkillName.Magery, 64.0, 100.0);
            this.SetSkill(SkillName.Meditation, 60.0, 83.0);
            this.SetSkill(SkillName.MagicResist, 65.0, 88.0);
            this.SetSkill(SkillName.Wrestling, 36.0, 68.0);
        }

        public FactionReagentVendor(Serial serial)
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
            this.SBInfos.Add(new SBFactionReagent());
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            this.AddItem(new Robe(Utility.RandomBlueHue()));
            this.AddItem(new GnarledStaff());
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

    public class SBFactionReagent : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBFactionReagent()
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
                for (int i = 0; i < 2; ++i)
                {
                    this.Add(new GenericBuyInfo(typeof(BlackPearl), 5, 20, 0xF7A, 0));
                    this.Add(new GenericBuyInfo(typeof(Bloodmoss), 5, 20, 0xF7B, 0));
                    this.Add(new GenericBuyInfo(typeof(MandrakeRoot), 3, 20, 0xF86, 0));
                    this.Add(new GenericBuyInfo(typeof(Garlic), 3, 20, 0xF84, 0));
                    this.Add(new GenericBuyInfo(typeof(Ginseng), 3, 20, 0xF85, 0));
                    this.Add(new GenericBuyInfo(typeof(Nightshade), 3, 20, 0xF88, 0));
                    this.Add(new GenericBuyInfo(typeof(SpidersSilk), 3, 20, 0xF8D, 0));
                    this.Add(new GenericBuyInfo(typeof(SulfurousAsh), 3, 20, 0xF8C, 0));
                }
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