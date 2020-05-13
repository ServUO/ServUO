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
                    AddItem(new Lajatang());
                    break;
                case 1:
                    AddItem(new Wakizashi());
                    break;
                case 2:
                    AddItem(new NoDachi());
                    break;
            }

            switch (Utility.Random(3))
            {
                case 0:
                    AddItem(new LeatherSuneate());
                    break;
                case 1:
                    AddItem(new PlateSuneate());
                    break;
                case 2:
                    AddItem(new StuddedHaidate());
                    break;
            }

            switch (Utility.Random(4))
            {
                case 0:
                    AddItem(new LeatherJingasa());
                    break;
                case 1:
                    AddItem(new ChainHatsuburi());
                    break;
                case 2:
                    AddItem(new HeavyPlateJingasa());
                    break;
                case 3:
                    AddItem(new DecorativePlateKabuto());
                    break;
            }

            AddItem(new LeatherDo());
            AddItem(new LeatherHiroSode());
            AddItem(new SamuraiTabi(Utility.RandomNondyedHue())); // TODO: Hue

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
