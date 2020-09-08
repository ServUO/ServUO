namespace Server.Mobiles
{
    [CorpseName("The remains of Flurry")]
    public class Flurry : BaseCreature
    {
        [Constructable]
        public Flurry()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Flurry";
            Body = 13;
            Hue = 3;
            BaseSoundID = 655;

            SetStr(149, 195);
            SetDex(218, 264);
            SetInt(130, 199);

            SetHits(474, 477);

            SetDamage(10, 15);  // Erica's

            SetDamageType(ResistanceType.Energy, 20);
            SetDamageType(ResistanceType.Cold, 80);

            SetResistance(ResistanceType.Physical, 56, 57);
            SetResistance(ResistanceType.Fire, 38, 44);
            SetResistance(ResistanceType.Cold, 40, 45);
            SetResistance(ResistanceType.Poison, 31, 37);
            SetResistance(ResistanceType.Energy, 39, 41);

            SetSkill(SkillName.EvalInt, 99.1, 100.2);
            SetSkill(SkillName.Magery, 105.1, 108.8);
            SetSkill(SkillName.MagicResist, 104.0, 112.8);
            SetSkill(SkillName.Tactics, 113.1, 119.8);
            SetSkill(SkillName.Wrestling, 105.6, 106.4);

            Fame = 4500;
            Karma = -4500;
        }

        public override bool GivesMLMinorArtifact => true;

        public Flurry(Serial serial)
            : base(serial)
        {
        }

        public override double DispelDifficulty => 117.5;
        public override double DispelFocus => 45.0;
        public override bool BleedImmune => true;
        public override int TreasureMapLevel => 2;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 10);
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
