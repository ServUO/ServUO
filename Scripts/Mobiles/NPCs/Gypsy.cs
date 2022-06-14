using Server.Items;

namespace Server.Mobiles
{
    public class Gypsy : BaseCreature
    {
        [Constructable]
        public Gypsy()
            : base(AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4)
        {
            InitStats(31, 41, 51);

            SpeechHue = Utility.RandomDyedHue();

            SetSkill(SkillName.Begging, 64.0, 100.0);
            SetSkill(SkillName.Cooking, 65, 88);
            SetSkill(SkillName.Snooping, 65, 88);
            SetSkill(SkillName.Stealing, 65, 88);

            Hue = Utility.RandomSkinHue();

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
                SetWearable(new Kilt(), Utility.RandomDyedHue(), 1);
                SetWearable(new Shirt(), Utility.RandomDyedHue(), 1);
				SetWearable(new ThighBoots(), dropChance: 1);
                Title = "the gypsy";
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
				SetWearable(new ShortPants(), Utility.RandomNeutralHue(), 1);
                SetWearable(new Shirt(), Utility.RandomDyedHue(), 1);
                SetWearable(new Sandals(), dropChance: 1);
                Title = "the gypsy";
            }

			SetWearable(new Bandana(), Utility.RandomDyedHue(), 1);
			SetWearable(new Dagger(), dropChance: 1);

            Utility.AssignRandomHair(this);

            Container pack = new Backpack();

            pack.DropItem(new Gold(250, 300));

			SetWearable(pack);
        }

        public Gypsy(Serial serial)
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
