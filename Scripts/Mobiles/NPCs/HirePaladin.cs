using Server.Items;

namespace Server.Mobiles
{
    public class HirePaladin : BaseHire
    {
        [Constructable]
        public HirePaladin()
        {
            SpeechHue = Utility.RandomDyedHue();
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

            Title = "the paladin";
            HairItemID = Race.RandomHair(Female);
            HairHue = Race.RandomHairHue();
            Race.RandomFacialHair(this);

            switch (Utility.Random(5))
            {
                case 0:
                    break;
                case 1:
					SetWearable(new Bascinet(), dropChance: 1);
                    break;
                case 2:
					SetWearable(new CloseHelm(), dropChance: 1);
                    break;
                case 3:
					SetWearable(new NorseHelm(), dropChance: 1);
                    break;
                case 4:
					SetWearable(new Helmet(), dropChance: 1);
                    break;
            }

            SetStr(86, 100);
            SetDex(81, 95);
            SetInt(61, 75);

            SetDamage(10, 23);

            SetSkill(SkillName.Swords, 66.0, 97.5);
            SetSkill(SkillName.Anatomy, 65.0, 87.5);
            SetSkill(SkillName.MagicResist, 25.0, 47.5);
            SetSkill(SkillName.Healing, 65.0, 87.5);
            SetSkill(SkillName.Tactics, 65.0, 87.5);
            SetSkill(SkillName.Wrestling, 15.0, 37.5);
            SetSkill(SkillName.Parry, 45.0, 60.5);
            SetSkill(SkillName.Chivalry, 85, 100);

            Fame = 100;
            Karma = 250;

            SetWearable(new Shoes(), Utility.RandomNeutralHue(), 1);
            SetWearable(new Shirt(), dropChance: 1);
            SetWearable(new VikingSword(), dropChance: 1);
			SetWearable(new MetalKiteShield(), dropChance: 1);

            SetWearable(new PlateChest(), dropChance: 1);
            SetWearable(new PlateLegs(), dropChance: 1);
            SetWearable(new PlateArms(), dropChance: 1);
			SetWearable(new LeatherGorget(), dropChance: 1);
            PackGold(20, 100);
        }

        public HirePaladin(Serial serial)
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