namespace Server.Mobiles
{
    [CorpseName("a snake corpse")]
    public class CoralSnake : BaseCreature
    {
        [Constructable]
        public CoralSnake()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a coral snake";
            Body = 52;
            Hue = 0x21;
            BaseSoundID = 0xDB;

            SetStr(205, 340);
            SetDex(248, 300);
            SetInt(28, 35);

            SetHits(132, 200);
            SetMana(28, 35);

            SetDamage(5, 21);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetResistance(ResistanceType.Physical, 42, 50);
            SetResistance(ResistanceType.Fire, 5, 20);
            SetResistance(ResistanceType.Physical, 5, 20);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 5, 20);

            SetSkill(SkillName.Poisoning, 99.7, 110.9);
            SetSkill(SkillName.MagicResist, 98.1, 105.0);
            SetSkill(SkillName.Tactics, 82.0, 98.0);
            SetSkill(SkillName.Wrestling, 90.3, 105.0);

            Fame = 300;
            Karma = -300;

            Tamable = false;
            ControlSlots = 1;
            MinTameSkill = 59.1;
        }

        public CoralSnake(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune => Poison.Lesser;

        public override Poison HitPoison => Poison.Deadly;

        public override int Meat => 1;

        public override FoodType FavoriteFood => FoodType.Eggs;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0 && (AbilityProfile == null || AbilityProfile.MagicalAbility == MagicalAbility.None))
            {
                SetMagicalAbility(MagicalAbility.Poisoning);
            }
        }
    }
}