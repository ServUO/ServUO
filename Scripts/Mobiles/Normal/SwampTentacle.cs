namespace Server.Mobiles
{
    [CorpseName("a swamp tentacle corpse")]
    public class SwampTentacle : BaseCreature
    {
        [Constructable]
        public SwampTentacle()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a swamp tentacle";
            Body = 66;
            BaseSoundID = 352;

            SetStr(96, 120);
            SetDex(66, 85);
            SetInt(16, 30);

            SetHits(58, 72);
            SetMana(0);

            SetDamage(6, 12);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Poison, 60);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 60, 80);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.MagicResist, 15.1, 20.0);
            SetSkill(SkillName.Tactics, 65.1, 80.0);
            SetSkill(SkillName.Wrestling, 65.1, 80.0);

            Fame = 3000;
            Karma = -3000;
        }

        public SwampTentacle(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune => Poison.Greater;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.MageryRegs, 3);
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
