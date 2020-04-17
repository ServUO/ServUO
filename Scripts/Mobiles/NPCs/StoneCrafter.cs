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
            SetSkill(SkillName.Carpentry, 85.0, 100.0);
        }

        public StoneCrafter(Serial serial)
            : base(serial)
        {
        }

        public override NpcGuild NpcGuild => NpcGuild.TinkersGuild;
        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBStoneCrafter());
            m_SBInfos.Add(new SBStavesWeapon());
            m_SBInfos.Add(new SBCarpenter());
            m_SBInfos.Add(new SBWoodenShields());
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (Title == "the stonecrafter")
                Title = "the stone crafter";
        }
    }
}