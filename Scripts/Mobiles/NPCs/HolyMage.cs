using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class HolyMage : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public HolyMage()
            : base("the Holy Mage")
        {
            SetSkill(SkillName.EvalInt, 65.0, 88.0);
            SetSkill(SkillName.Inscribe, 60.0, 83.0);
            SetSkill(SkillName.Magery, 64.0, 100.0);
            SetSkill(SkillName.Meditation, 60.0, 83.0);
            SetSkill(SkillName.MagicResist, 65.0, 88.0);
            SetSkill(SkillName.Wrestling, 36.0, 68.0);
        }

        public HolyMage(Serial serial)
            : base(serial)
        {
        }

        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBHolyMage());
        }

        public override void InitOutfit()
        {
            SetWearable(new Robe(), 0x47E, 1);
            SetWearable(new ThighBoots(), 0x47E, 1);
            SetWearable(new BlackStaff(), 0x47E, 1);

            if (Female)
            {
				SetWearable(new LeatherGloves(), 0x47E, 1);
				SetWearable(new GoldNecklace(), 0x47E, 1);
            }
            else
            {
				SetWearable(new PlateGloves(), 0x47E, 1);
				SetWearable(new PlateGorget(), 0x47E, 1);
            }

            switch (Utility.Random(Female ? 2 : 1))
            {
                case 0:
                    HairItemID = 0x203C;
                    break;
                case 1:
                    HairItemID = 0x203D;
                    break;
            }

            HairHue = 0x47E;
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
