using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class Butcher : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Butcher()
            : base("the butcher")
        {
            SetSkill(SkillName.Anatomy, 45.0, 68.0);
        }

        public Butcher(Serial serial)
            : base(serial)
        {
        }

        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBButcher());
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            SetWearable(new HalfApron(), dropChance: 1);
            SetWearable(new Cleaver(), dropChance: 1);
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