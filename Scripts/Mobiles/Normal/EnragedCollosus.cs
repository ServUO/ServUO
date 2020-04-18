namespace Server.Mobiles
{
    [CorpseName("the remains of an enraged colossus")]
    public class EnragedColossus : BaseCreature
    {
        [Constructable]
        public EnragedColossus()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.5)
        {
            Name = "Rising Colossus";
            Body = 829;

            SetStr(600);
            SetDex(70);
            SetInt(80);

            SetDamage(18, 21);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 56, 63);
            SetResistance(ResistanceType.Fire, 29, 30);
            SetResistance(ResistanceType.Cold, 53, 54);
            SetResistance(ResistanceType.Poison, 54, 58);
            SetResistance(ResistanceType.Energy, 26, 29);

            SetSkill(SkillName.MagicResist, 115.0, 140.0);
            SetSkill(SkillName.Tactics, 120.0, 130.0);
            SetSkill(SkillName.Wrestling, 120.0, 140);

            ControlSlots = 5;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 4);
            AddLoot(LootPack.Gems, 8);
        }

        public EnragedColossus(Serial serial)
            : base(serial)
        {
        }

        public override double DispelDifficulty => 125.0;

        public override double DispelFocus => 45.0;

        public override Poison PoisonImmune => Poison.Lethal;

        public override int GetAttackSound()
        {
            return 0x627;
        }

        public override int GetHurtSound()
        {
            return 0x629;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}