namespace Server.Mobiles
{
    [CorpseName("a bulbous putrification corpse")]
    public class BulbousPutrification : BaseCreature
    {
        [Constructable]
        public BulbousPutrification()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a bulbous putrification";
            Body = 0x307;
            Hue = 0x55C;
            BaseSoundID = 0x165;

            SetStr(755, 800);
            SetDex(53, 60);
            SetInt(51, 59);

            SetHits(1211, 1231);

            SetDamage(22, 29);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Poison, 40);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 55, 70);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.Wrestling, 104.8, 114.7);
            SetSkill(SkillName.Tactics, 111.9, 119.1);
            SetSkill(SkillName.MagicResist, 55.5, 64.1);
            SetSkill(SkillName.Anatomy, 110.0);
            SetSkill(SkillName.Poisoning, 80.0);
        }

        public BulbousPutrification(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune => Poison.Lethal;
        public override Poison HitPoison => Poison.Lethal;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 5);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}