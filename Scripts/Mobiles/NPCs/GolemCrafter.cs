using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class GolemCrafter : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public GolemCrafter()
            : base("the golem crafter")
        {
            this.SetSkill(SkillName.Lockpicking, 60.0, 83.0);
            this.SetSkill(SkillName.RemoveTrap, 75.0, 98.0);
            this.SetSkill(SkillName.Tinkering, 64.0, 100.0);
        }

        public GolemCrafter(Serial serial)
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
            this.m_SBInfos.Add(new SBTinker(this));
            this.m_SBInfos.Add(new SBVagabond());
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