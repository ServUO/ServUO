using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Thief : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        [Constructable]
        public Thief()
            : base("the thief")
        {
            SetSkill(SkillName.Camping, 55.0, 78.0);
            SetSkill(SkillName.DetectHidden, 65.0, 88.0);
            SetSkill(SkillName.Hiding, 45.0, 68.0);
            SetSkill(SkillName.Archery, 65.0, 88.0);
            SetSkill(SkillName.Tracking, 65.0, 88.0);
            SetSkill(SkillName.Veterinary, 60.0, 83.0);
            SetSkill(SkillName.RemoveTrap, 75.0, 98.0);
        }

        public Thief(Serial serial)
            : base(serial)
        {
        }

        protected override List<SBInfo> SBInfos => m_SBInfos;
        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBThief());
        }

        public override void InitOutfit()
        {
            AddItem(new Items.Shirt(Utility.RandomNeutralHue()));
            AddItem(new Items.LongPants(Utility.RandomNeutralHue()));
            AddItem(new Items.Dagger());
            AddItem(new Items.ThighBoots(Utility.RandomNeutralHue()));

            base.InitOutfit();
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
