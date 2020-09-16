using Server.Engines.Quests;

namespace Server.Mobiles
{
    public class Kevin : HumilityQuestMobile
    {
        public override int Greeting => 1075759;

        public override bool IsActiveVendor => true;
        public override bool CanTeach => true;

        [Constructable]
        public Kevin()
            : base("Kevin", "the butcher")
        {
            SetSkill(SkillName.Anatomy, 45.0, 68.0);
        }

        public override void InitSBInfo()
        {
            SBInfos.Add(new SBButcher());
        }

        public Kevin(Serial serial)
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
            base.InitOutfit();

            AddItem(new Items.HalfApron());
            AddItem(new Items.Cleaver());
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