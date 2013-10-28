using System;
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
            this.SetSkill(SkillName.Cooking, 75.0, 98.0);
            this.SetSkill(SkillName.TasteID, 36.0, 68.0);
        }

        public Baker(Serial serial)
            : base(serial)
        {
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
            this.m_SBInfos.Add(new SBBaker());
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