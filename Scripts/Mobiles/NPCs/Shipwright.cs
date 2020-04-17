using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Shipwright : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();

        [Constructable]
        public Shipwright()
            : base("the shipwright")
        {
            SetSkill(SkillName.Carpentry, 60.0, 83.0);
            SetSkill(SkillName.Macing, 36.0, 68.0);
        }

        public Shipwright(Serial serial)
            : base(serial)
        {
        }

        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBShipwright(this));
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            AddItem(new Items.SmithHammer());
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
