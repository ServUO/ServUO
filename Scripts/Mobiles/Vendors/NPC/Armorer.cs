using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Armorer : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Armorer()
            : base("the armorer")
        {
            this.SetSkill(SkillName.ArmsLore, 64.0, 100.0);
            this.SetSkill(SkillName.Blacksmith, 60.0, 83.0);
        }

        public Armorer(Serial serial)
            : base(serial)
        {
        }

        public override VendorShoeType ShoeType
        {
            get
            {
                return VendorShoeType.Boots;
            }
        }
        protected override List<SBInfo> SBInfos
        {
            get
            {
                return this.m_SBInfos;
            }
        }
        public override void InitSBInfo()
        {
            switch ( Utility.Random(4))
            {
                case 0:
                    {
                        this.m_SBInfos.Add(new SBLeatherArmor());
                        this.m_SBInfos.Add(new SBStuddedArmor());
                        this.m_SBInfos.Add(new SBMetalShields());
                        this.m_SBInfos.Add(new SBPlateArmor());
                        this.m_SBInfos.Add(new SBHelmetArmor());
                        this.m_SBInfos.Add(new SBChainmailArmor());
                        this.m_SBInfos.Add(new SBRingmailArmor());
                        break;
                    }
                case 1:
                    {
                        this.m_SBInfos.Add(new SBStuddedArmor());
                        this.m_SBInfos.Add(new SBLeatherArmor());
                        this.m_SBInfos.Add(new SBMetalShields());
                        this.m_SBInfos.Add(new SBHelmetArmor());
                        break;
                    }
                case 2:
                    {
                        this.m_SBInfos.Add(new SBMetalShields());
                        this.m_SBInfos.Add(new SBPlateArmor());
                        this.m_SBInfos.Add(new SBHelmetArmor());
                        this.m_SBInfos.Add(new SBChainmailArmor());
                        this.m_SBInfos.Add(new SBRingmailArmor());
                        break;
                    }
                case 3:
                    {
                        this.m_SBInfos.Add(new SBMetalShields());
                        this.m_SBInfos.Add(new SBHelmetArmor());
                        break;
                    }
            }
            if (this.IsTokunoVendor)
            {
                this.m_SBInfos.Add(new SBSELeatherArmor());	
                this.m_SBInfos.Add(new SBSEArmor());
            }
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            this.AddItem(new Server.Items.HalfApron(Utility.RandomYellowHue()));
            this.AddItem(new Server.Items.Bascinet());
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
}