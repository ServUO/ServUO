using Server.Items;

namespace Server.Mobiles
{
    public class HireMage : BaseHire
    {
        [Constructable]
        public HireMage()
            : base(AIType.AI_Mage)
        {
            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();
            Title = "the mage";
            if (Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
				SetWearable(new ShortPants(), Utility.RandomNeutralHue(), 1);
            }

            HairItemID = Race.RandomHair(Female);
            HairHue = Race.RandomHairHue();
            Race.RandomFacialHair(this);

            SetStr(61, 75);
            SetDex(81, 95);
            SetInt(86, 100);

            SetDamage(10, 23);

            SetSkill(SkillName.EvalInt, 100.0, 125);
            SetSkill(SkillName.Magery, 100, 125);
            SetSkill(SkillName.Meditation, 100, 125);
            SetSkill(SkillName.MagicResist, 100, 125);
            SetSkill(SkillName.Tactics, 100, 125);
            SetSkill(SkillName.Macing, 100, 125);

            Fame = 100;
            Karma = 100;

			SetWearable(new Shirt(), dropChance: 1);

			SetWearable(new Robe(), Utility.RandomNeutralHue(), 1);

            if (Utility.RandomBool())
				SetWearable(new Shoes(), Utility.RandomNeutralHue(), 1);
            else
				SetWearable(new ThighBoots(), dropChance: 1);

            PackGold(20, 100);
        }

        public HireMage(Serial serial)
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