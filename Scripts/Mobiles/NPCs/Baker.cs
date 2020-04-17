using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Baker : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Baker()
            : base("the baker")
        {
            SetSkill(SkillName.Cooking, 75.0, 98.0);
            SetSkill(SkillName.TasteID, 36.0, 68.0);
        }

        public Baker(Serial serial)
            : base(serial)
        {
        }

        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBBaker());
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