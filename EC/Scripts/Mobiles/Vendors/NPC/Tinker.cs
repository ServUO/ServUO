using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Tinker : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Tinker()
            : base("the tinker")
        {
            this.SetSkill(SkillName.Lockpicking, 60.0, 83.0);
            this.SetSkill(SkillName.RemoveTrap, 75.0, 98.0);
            this.SetSkill(SkillName.Tinkering, 64.0, 100.0);
        }

        public Tinker(Serial serial)
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
            this.m_SBInfos.Add(new SBTinker());
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