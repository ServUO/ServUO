using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a human corpse")]
    public class ExodusZealot : BaseCreature
    {
        [Constructable]
        public ExodusZealot()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 401;
            Female = false;
            Hue = 33875;
            HairItemID = Race.Human.RandomHair(this);
            HairHue = Race.Human.RandomHairHue();

            Name = NameList.RandomName("male");
            Title = "the Exodus Zealot";

            SetStr(150, 210);
            SetDex(75, 90);
            SetInt(255, 310);

            SetHits(325, 390);

            SetDamage(6, 12);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 40);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 35, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.Wrestling, 70.0, 100.0);
            SetSkill(SkillName.Tactics, 80.0, 100.0);
            SetSkill(SkillName.MagicResist, 50.0, 70.0);
            SetSkill(SkillName.Anatomy, 70.0, 100.0);
            SetSkill(SkillName.Magery, 85.0, 100.0);
            SetSkill(SkillName.EvalInt, 80.0, 100.0);
            SetSkill(SkillName.Poisoning, 70.0, 100.0);

            Fame = 10000;
            Karma = -10000;

            Item boots = new ThighBoots
            {
                Movable = false
            };
            SetWearable(boots);

            Item item = new HoodedShroudOfShadows(2702)
            {
                LootType = LootType.Blessed
            };
            SetWearable(item);

            item = new Spellbook
            {
                LootType = LootType.Blessed
            };
            SetWearable(item);
        }

        public ExodusZealot(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer => true;

        public override bool ShowFameTitle => false;

        public override Poison PoisonImmune => Poison.Lethal;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.MedScrolls);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
