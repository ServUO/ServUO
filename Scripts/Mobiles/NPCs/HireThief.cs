using Server.Items;

namespace Server.Mobiles
{
    public class HireThief : BaseHire
    {
        [Constructable]
        public HireThief()
        {
            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();

            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");

                switch (Utility.Random(2))
                {
                    case 0:
						SetWearable(new Skirt(), Utility.RandomNeutralHue(), 1);
                        break;
                    case 1:
						SetWearable(new Kilt(), Utility.RandomNeutralHue(), 1);
                        break;
                }
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
				SetWearable(new ShortPants(), Utility.RandomNeutralHue(), 1);
            }
            Title = "the thief";
            HairItemID = Race.RandomHair(Female);
            HairHue = Race.RandomHairHue();
            Race.RandomFacialHair(this);

            SetStr(81, 95);
            SetDex(86, 100);
            SetInt(61, 75);

            SetDamage(10, 23);

            SetSkill(SkillName.Stealing, 66.0, 97.5);
            SetSkill(SkillName.Peacemaking, 65.0, 87.5);
            SetSkill(SkillName.MagicResist, 25.0, 47.5);
            SetSkill(SkillName.Healing, 65.0, 87.5);
            SetSkill(SkillName.Tactics, 65.0, 87.5);
            SetSkill(SkillName.Fencing, 65.0, 87.5);
            SetSkill(SkillName.Parry, 45.0, 60.5);
            SetSkill(SkillName.Lockpicking, 65, 87);
            SetSkill(SkillName.Hiding, 65, 87);
            SetSkill(SkillName.Snooping, 65, 87);
            Fame = 100;
            Karma = 0;

			SetWearable(new Sandals(), Utility.RandomNeutralHue(), 1);
			SetWearable(new Dagger(), dropChance: 1);
            switch (Utility.Random(2))
            {
                case 0:
					SetWearable(new Doublet(), Utility.RandomNeutralHue(), 1);
                    break;
                case 1:
					SetWearable(new Shirt(), Utility.RandomNeutralHue(), 1);
                    break;
            }

            PackGold(0, 25);
        }

        public HireThief(Serial serial)
            : base(serial)
        {
        }

        public override bool ClickTitle => false;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);// version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}