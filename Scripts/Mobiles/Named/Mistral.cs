namespace Server.Mobiles
{
    [CorpseName("a mistral corpse")]
    public class Mistral : BaseCreature
    {
        [Constructable]
        public Mistral()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Mistral";
            Body = 13;
            Hue = 924;
            BaseSoundID = 263;

            SetStr(134, 201);
            SetDex(226, 238);
            SetInt(126, 134);

            SetHits(386, 609);

            SetDamage(17, 20);

            SetDamageType(ResistanceType.Energy, 20);
            SetDamageType(ResistanceType.Cold, 40);
            SetDamageType(ResistanceType.Physical, 40);

            SetResistance(ResistanceType.Physical, 55, 64);
            SetResistance(ResistanceType.Fire, 36, 40);
            SetResistance(ResistanceType.Cold, 33, 39);
            SetResistance(ResistanceType.Poison, 30, 39);
            SetResistance(ResistanceType.Energy, 49, 53);

            SetSkill(SkillName.EvalInt, 96.2, 97.8);
            SetSkill(SkillName.Magery, 100.8, 112.9);
            SetSkill(SkillName.MagicResist, 106.2, 111.2);
            SetSkill(SkillName.Tactics, 110.2, 117.1);
            SetSkill(SkillName.Wrestling, 100.3, 104.0);

            Fame = 4500;
            Karma = -4500;
        }

        public override bool GivesMLMinorArtifact => true;

        public Mistral(Serial serial)
            : base(serial)
        {
        }

        public override double DispelDifficulty => 117.5;

        public override double DispelFocus => 45.0;

        public override bool BleedImmune => true;

        public override int TreasureMapLevel => 2;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.LowScrolls);
            AddLoot(LootPack.MedScrolls);
            AddLoot(LootPack.ArcanistScrolls);
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
