using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Veterinarian : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Veterinarian()
            : base("the vet")
        {
            this.SetSkill(SkillName.AnimalLore, 85.0, 100.0);
            this.SetSkill(SkillName.Veterinary, 90.0, 100.0);
        }

        public Veterinarian(Serial serial)
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
            this.m_SBInfos.Add(new SBVeterinarian());
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