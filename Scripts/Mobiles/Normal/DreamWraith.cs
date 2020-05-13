namespace Server.Mobiles
{
    [CorpseName("a dream wraith corpse")]
    public class DreamWraith : BaseCreature
    {
        [Constructable]
        public DreamWraith()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a dream wraith";
            Body = 740;
            BaseSoundID = 0x482;

            SetStr(200, 300);
            SetDex(100, 200);
            SetInt(600, 700);

            SetHits(550, 650);

            SetDamage(18, 25);

            SetDamageType(ResistanceType.Physical, 10);
            SetDamageType(ResistanceType.Cold, 45);
            SetDamageType(ResistanceType.Energy, 45);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 30, 50);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.Necromancy, 100.0, 120.0);
            SetSkill(SkillName.SpiritSpeak, 100.0, 120.0);
            SetSkill(SkillName.Anatomy, 0.0, 10.0);
            SetSkill(SkillName.EvalInt, 100.0, 120.0);
            SetSkill(SkillName.Magery, 100.0, 120.0);
            SetSkill(SkillName.Meditation, 100.0, 110.0);
            SetSkill(SkillName.MagicResist, 120.0, 150.0);
            SetSkill(SkillName.Tactics, 70.0, 80.0);
            SetSkill(SkillName.Wrestling, 90.0, 100.0);

            Fame = 4000;
            Karma = -4000;
        }

        public DreamWraith(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MageryRegs, 10);
        }

        public override int GetIdleSound()
        {
            return 0x5F4;
        }

        public override int GetAngerSound()
        {
            return 0x5F1;
        }

        public override int GetDeathSound()
        {
            return 0x5F2;
        }

        public override int GetHurtSound()
        {
            return 0x5F3;
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
