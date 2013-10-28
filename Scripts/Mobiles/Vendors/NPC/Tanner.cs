using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Tanner : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Tanner()
            : base("the tanner")
        {
            this.SetSkill(SkillName.Tailoring, 36.0, 68.0);
        }

        public Tanner(Serial serial)
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
            this.m_SBInfos.Add(new SBTanner());
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