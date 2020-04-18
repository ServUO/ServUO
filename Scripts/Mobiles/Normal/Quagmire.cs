namespace Server.Mobiles
{
    [CorpseName("a quagmire corpse")]
    public class Quagmire : BaseCreature
    {
        [Constructable]
        public Quagmire()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8)
        {
            Name = "a quagmire";
            Body = 789;
            BaseSoundID = 352;

            SetStr(101, 130);
            SetDex(66, 85);
            SetInt(31, 55);

            SetHits(91, 105);

            SetDamage(10, 14);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Poison, 40);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.MagicResist, 65.1, 75.0);
            SetSkill(SkillName.Tactics, 50.1, 60.0);
            SetSkill(SkillName.Wrestling, 60.1, 80.0);

            Fame = 1500;
            Karma = -1500;
        }

        public Quagmire(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune => Poison.Lethal;
        public override Poison HitPoison => Poison.Lethal;
        public override double HitPoisonChance => 0.1;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
        }

        public override int GetAngerSound()
        {
            return 353;
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