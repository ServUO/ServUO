namespace Server.Mobiles
{
    [CorpseName("a myrmidex corpse")]
    public class DescicatedMyrmidexLarvae : BaseCreature
    {
        [Constructable]
        public DescicatedMyrmidexLarvae()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .2, .4)
        {
            Name = "a descicated myrmidex larvae";
            Hue = 2949;

            Body = 1293;
            BaseSoundID = 959;

            SetStr(350, 450);
            SetDex(80, 95);
            SetInt(15, 25);

            SetDamage(5, 10);

            SetHits(446, 588);
            SetMana(20, 50);

            SetResistance(ResistanceType.Physical, 20, 25);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Poison, 40);

            SetSkill(SkillName.MagicResist, 30.1, 43.5);
            SetSkill(SkillName.Tactics, 60, 70);
            SetSkill(SkillName.Wrestling, 55, 60);
            SetSkill(SkillName.Poisoning, 80, 100);
            SetSkill(SkillName.DetectHidden, 30, 40);

            Fame = 2500;
            Karma = -2500;
        }

        public override Poison HitPoison => Poison.Lesser;
        public override Poison PoisonImmune => Poison.Lesser;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.LootGold(20, 40));
        }

        public DescicatedMyrmidexLarvae(Serial serial)
            : base(serial)
        {
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
