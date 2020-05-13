namespace Server.Mobiles
{
    [CorpseName("a devourer of souls corpse")]
    public class Devourer : BaseCreature
    {
        [Constructable]
        public Devourer()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a devourer of souls";
            Body = 303;
            BaseSoundID = 357;

            SetStr(801, 950);
            SetDex(126, 175);
            SetInt(201, 250);

            SetHits(650);

            SetDamage(22, 26);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.Necromancy, 90.1, 100.0);
            SetSkill(SkillName.SpiritSpeak, 90.1, 100.0);
            SetSkill(SkillName.EvalInt, 90.1, 100.0);
            SetSkill(SkillName.Magery, 90.1, 100.0);
            SetSkill(SkillName.Meditation, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 90.1, 105.0);
            SetSkill(SkillName.Tactics, 75.1, 85.0);
            SetSkill(SkillName.Wrestling, 80.1, 100.0);

            Fame = 9500;
            Karma = -9500;
        }

        public Devourer(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune => Poison.Lethal;
        public override int Meat => 3;
        public override int TreasureMapLevel => 4;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.NecroRegs, 24, 45);
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
