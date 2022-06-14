using Server.Items;

namespace Server.Mobiles
{
    public class Artist : BaseCreature
    {
        [Constructable]
        public Artist()
            : base(AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4)
        {
            InitStats(31, 41, 51);

            SetSkill(SkillName.Healing, 36, 68);

            SpeechHue = Utility.RandomDyedHue();
            Title = "the artist";
            Hue = Utility.RandomSkinHue();

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
            }
            SetWearable(new Doublet(), Utility.RandomDyedHue(), 1);
            SetWearable(new Sandals(), Utility.RandomNeutralHue(), 1);
            SetWearable(new ShortPants(), Utility.RandomNeutralHue(), 1);
            SetWearable(new HalfApron(), Utility.RandomDyedHue(), 1);

            Utility.AssignRandomHair(this);

            Container pack = new Backpack();

            pack.DropItem(new Gold(250, 300));

            SetWearable(pack);
        }

        public Artist(Serial serial)
            : base(serial)
        {
        }

        public override bool CanTeach => true;
        public override bool ClickTitle => false;
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
