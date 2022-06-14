using Server.Items;

namespace Server.Mobiles
{
    public class Samurai : BaseCreature
    {
        [Constructable]
        public Samurai()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Title = "the samurai";

            InitStats(100, 100, 25);

            SetSkill(SkillName.ArmsLore, 64.0, 80.0);
            SetSkill(SkillName.Bushido, 64.0, 85.0);
            SetSkill(SkillName.Parry, 64.0, 80.0);
            SetSkill(SkillName.Swords, 64.0, 85.0);

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

            switch (Utility.Random(3))
            {
                case 0:
					SetWearable(new Lajatang(), dropChance: 1);
                    break;
                case 1:
					SetWearable(new Wakizashi(), dropChance: 1);
                    break;
                case 2:
					SetWearable(new NoDachi(), dropChance: 1);
                    break;
            }

            switch (Utility.Random(3))
            {
                case 0:
					SetWearable(new LeatherSuneate(), dropChance: 1);
                    break;
                case 1:
					SetWearable(new PlateSuneate(), dropChance: 1);
                    break;
                case 2:
					SetWearable(new StuddedHaidate(), dropChance: 1);
                    break;
            }

            switch (Utility.Random(4))
            {
                case 0:
					SetWearable(new LeatherJingasa(), dropChance: 1);
                    break;
                case 1:
					SetWearable(new ChainHatsuburi(), dropChance: 1);
					break;
                case 2:
					SetWearable(new HeavyPlateJingasa(), dropChance: 1);
                    break;
                case 3:
					SetWearable(new DecorativePlateKabuto(), dropChance: 1);
                    break;
            }

            SetWearable(new LeatherDo(), dropChance: 1);
            SetWearable(new LeatherHiroSode(), dropChance: 1);
			SetWearable(new SamuraiTabi(), Utility.RandomNondyedHue(), 1); // TODO: Hue

            int hairHue = Utility.RandomNondyedHue();

            Utility.AssignRandomHair(this, hairHue);

            if (Utility.Random(7) != 0)
                Utility.AssignRandomFacialHair(this, hairHue);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.LootGold(250, 300));
        }

        public Samurai(Serial serial)
            : base(serial)
        {
        }

        public override bool CanTeach => true;
        public override bool ClickTitle => false;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}
