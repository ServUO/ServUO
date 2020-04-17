using System.Collections.Generic;

namespace Server.Mobiles
{
    public class HairStylist : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public HairStylist()
            : base("the hair stylist")
        {
            SetSkill(SkillName.Alchemy, 80.0, 100.0);
            SetSkill(SkillName.Magery, 90.0, 110.0);
            SetSkill(SkillName.TasteID, 85.0, 100.0);
        }

        public HairStylist(Serial serial)
            : base(serial)
        {
        }

        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBHairStylist());
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
        }
    }
}