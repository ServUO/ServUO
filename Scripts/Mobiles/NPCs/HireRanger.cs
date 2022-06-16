using Server.Items;

namespace Server.Mobiles
{
    public class HireRanger : BaseHire
    {
        [Constructable]
        public HireRanger()
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
                SetWearable(new ShortPants(), Utility.RandomNeutralHue(), 1);
            }

            Title = "the ranger";
            HairItemID = Race.RandomHair(Female);
            HairHue = Race.RandomHairHue();
            Race.RandomFacialHair(this);

            SetStr(91, 91);
            SetDex(76, 76);
            SetInt(61, 61);

            SetDamage(13, 24);

            SetSkill(SkillName.Wrestling, 15, 37);
            SetSkill(SkillName.Parry, 45, 60);
            SetSkill(SkillName.Archery, 66, 97);
            SetSkill(SkillName.Magery, 62, 62);
            SetSkill(SkillName.Swords, 35, 57);
            SetSkill(SkillName.Fencing, 15, 37);
            SetSkill(SkillName.Tactics, 65, 87);

            Fame = 100;
            Karma = 125;

			SetWearable(new Shoes(), Utility.RandomNeutralHue(), 1);
			SetWearable(new Shirt(), dropChance: 1);

            // Pick a random sword
            switch (Utility.Random(3))
            {
                case 0:
					SetWearable(new Longsword(), dropChance: 1);
                    break;
                case 1:
					SetWearable(new VikingSword(), dropChance: 1);
                    break;
                case 2:
					SetWearable(new Broadsword(), dropChance: 1);
                    break;
            }

            SetWearable(new StuddedChest(), 0x59C, 1);
            SetWearable(new StuddedArms(), 0x59C, 1);
            SetWearable(new StuddedGloves(), 0x59C, 1);
            SetWearable(new StuddedLegs(), 0x59C, 1);
            SetWearable(new StuddedGorget(), 0x59C, 1);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.LootItem<Arrow>(20, true));
            AddLoot(LootPack.LootGold(10, 75));
        }

        public HireRanger(Serial serial)
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
