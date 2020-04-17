namespace Server.Mobiles
{
    [CorpseName("a wanderer of the void corpse")]
    public class WandererOfTheVoid : BaseCreature
    {
        [Constructable]
        public WandererOfTheVoid()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a wanderer of the void";
            Body = 316;
            BaseSoundID = 377;

            SetStr(111, 200);
            SetDex(101, 125);
            SetInt(301, 390);

            SetHits(351, 400);

            SetDamage(11, 13);

            SetDamageType(ResistanceType.Physical, 0);
            SetDamageType(ResistanceType.Cold, 15);
            SetDamageType(ResistanceType.Energy, 85);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 50, 75);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.EvalInt, 60.1, 70.0);
            SetSkill(SkillName.Magery, 60.1, 70.0);
            SetSkill(SkillName.Meditation, 60.1, 70.0);
            SetSkill(SkillName.MagicResist, 50.1, 75.0);
            SetSkill(SkillName.Tactics, 60.1, 70.0);
            SetSkill(SkillName.Wrestling, 60.1, 70.0);

            Fame = 20000;
            Karma = -20000;
        }

        public WandererOfTheVoid(Serial serial)
            : base(serial)
        {
        }

        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override int TreasureMapLevel => 4;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
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