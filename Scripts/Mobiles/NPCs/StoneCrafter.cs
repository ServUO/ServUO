using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [TypeAlias("Server.Mobiles.GargoyleStonecrafter")]
    public class StoneCrafter : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public StoneCrafter()
            : base("the stone crafter")
        {
            this.SetSkill(SkillName.Carpentry, 85.0, 100.0);
        }

        public StoneCrafter(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild
        {
            get
            {
                return NpcGuild.TinkersGuild;
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
            this.m_SBInfos.Add(new SBStoneCrafter());
            this.m_SBInfos.Add(new SBStavesWeapon());
            this.m_SBInfos.Add(new SBCarpenter());
            this.m_SBInfos.Add(new SBWoodenShields());
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

            if (this.Title == "the stonecrafter")
                this.Title = "the stone crafter";
        }
    }
}