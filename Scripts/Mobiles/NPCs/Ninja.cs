using Server.Items;

namespace Server.Mobiles
{
    public class Ninja : BaseCreature
    {
        [Constructable]
        public Ninja()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Title = "the ninja";

            InitStats(100, 100, 25);

            SetSkill(SkillName.Fencing, 64.0, 80.0);
            SetSkill(SkillName.Macing, 64.0, 80.0);
            SetSkill(SkillName.Ninjitsu, 60.0, 80.0);
            SetSkill(SkillName.Parry, 64.0, 80.0);
            SetSkill(SkillName.Tactics, 64.0, 85.0);
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

            if (!Female)
                AddItem(new LeatherNinjaHood());

            AddItem(new LeatherNinjaPants());
            AddItem(new LeatherNinjaBelt());
            AddItem(new LeatherNinjaJacket());
            AddItem(new NinjaTabi());

            int hairHue = Utility.RandomNondyedHue();

            Utility.AssignRandomHair(this, hairHue);

            if (Utility.Random(7) != 0)
                Utility.AssignRandomFacialHair(this, hairHue);
        }

        public Ninja(Serial serial)
            : base(serial)
        {
        }

        public override bool CanTeach => true;
        public override bool ClickTitle => false;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.LootGold(250, 300));
        }

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
