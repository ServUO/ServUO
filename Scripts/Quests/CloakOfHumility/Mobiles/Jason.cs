using Server.Engines.Quests;

namespace Server.Mobiles
{
    public class Jason : HumilityQuestMobile
    {
        public override int Greeting => 1075764;

        public override bool IsActiveVendor => true;
        public override bool CanTeach => true;

        public override bool CheckTeach(SkillName skill, Mobile from)
        {
            return (skill == SkillName.Forensics)
                || (skill == SkillName.Healing)
                || (skill == SkillName.SpiritSpeak)
                || (skill == SkillName.Swords);
        }

        public override void InitSBInfo()
        {
            SBInfos.Add(new SBHealer());
        }

        [Constructable]
        public Jason()
            : base("Jason", "the Healer")
        {
            SetSkill(SkillName.Forensics, 80.0, 100.0);
            SetSkill(SkillName.SpiritSpeak, 80.0, 100.0);
            SetSkill(SkillName.Swords, 80.0, 100.0);
        }

        public Jason(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;
            Body = 0x190;

            Hue = Race.RandomSkinHue();
            HairItemID = Race.RandomHair(false);
            HairHue = Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            AddItem(new Items.Backpack());
            AddItem(new Items.Robe(Utility.RandomYellowHue()));
            AddItem(new Items.Sandals());
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