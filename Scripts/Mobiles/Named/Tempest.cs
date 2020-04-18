namespace Server.Mobiles
{
    [CorpseName("the remains of tempest")]
    public class Tempest : BaseCreature
    {
        [Constructable]
        public Tempest()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Tempest";
            Body = 13;
            Hue = 1175;
            BaseSoundID = 263;

            SetStr(116, 135);
            SetDex(166, 185);
            SetInt(101, 125);

            SetHits(602);

            SetDamage(18, 20);

            SetDamageType(ResistanceType.Energy, 80);
            SetDamageType(ResistanceType.Cold, 20);

            SetResistance(ResistanceType.Physical, 46);
            SetResistance(ResistanceType.Fire, 39);
            SetResistance(ResistanceType.Cold, 33);
            SetResistance(ResistanceType.Poison, 36);
            SetResistance(ResistanceType.Energy, 58);

            SetSkill(SkillName.EvalInt, 99.6);
            SetSkill(SkillName.Magery, 101.0);
            SetSkill(SkillName.MagicResist, 104.6);
            SetSkill(SkillName.Tactics, 111.8);
            SetSkill(SkillName.Wrestling, 116.0);

            Fame = 4500;
            Karma = -4500;
        }

        public override bool GivesMLMinorArtifact => true;

        public Tempest(Serial serial)
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