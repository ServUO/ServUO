using System;
using System.Collections.Generic;
using Server;

namespace Server.Mobiles
{
    public class BoatPainter : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public BoatPainter()
            : base("the boat painter")
        {
            SetSkill(SkillName.Carpentry, 36.0, 68.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBBoatPainter());
        }

        public BoatPainter(Serial serial)
            : base(serial)
        {
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