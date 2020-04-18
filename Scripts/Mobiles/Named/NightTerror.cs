namespace Server.Mobiles
{
    [CorpseName("a night terror corpse")]
    public class NightTerror : BaseCreature
    {
        [Constructable]
        public NightTerror()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Night Terror";
            Body = 0x30c;
            Hue = 2963;

            SetStr(385, 467);
            SetDex(40, 70);
            SetInt(600, 800);

            SetHits(50000);

            SetDamage(10, 23);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Poison, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 100);
            SetResistance(ResistanceType.Cold, 80, 90);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.MagicResist, 90.0, 100.0);
            SetSkill(SkillName.Tactics, 120.0);
            SetSkill(SkillName.Wrestling, 110.0);
            SetSkill(SkillName.Poisoning, 120.0);
            SetSkill(SkillName.DetectHidden, 120.0);
            SetSkill(SkillName.Parry, 60.0, 70.0);
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 110.0);
            SetSkill(SkillName.Necromancy, 120.0);
            SetSkill(SkillName.SpiritSpeak, 120.0);

            Fame = 8000;
            Karma = -8000;
        }

        public NightTerror(Serial serial)
            : base(serial)
        {
        }

        public override int TreasureMapLevel => 4;

        public override Poison PoisonImmune => Poison.Lethal;

        public override Poison HitPoison => Poison.Lethal;

        public override double HitPoisonChance => 0.75;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
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