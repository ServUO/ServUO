namespace Server.Mobiles
{
    [CorpseName("a korpre corpse")]
    public class Korpre : BaseVoidCreature
    {
        public override VoidEvolution Evolution => VoidEvolution.None;
        public override int Stage => 0;

        [Constructable]
        public Korpre()
            : base(AIType.AI_Melee, 10, 1, 0.2, 0.4)
        {
            Name = "korpre";
            Body = 51;
            BaseSoundID = 456;

            Hue = 2071;

            SetStr(22, 34);
            SetDex(16, 21);
            SetInt(16, 20);

            SetHits(50, 60);

            SetDamage(1, 5);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 5, 10);
            SetResistance(ResistanceType.Poison, 15, 20);

            SetSkill(SkillName.Poisoning, 36.0, 49.1);
            SetSkill(SkillName.Anatomy, 0);
            SetSkill(SkillName.MagicResist, 15.9, 18.9);
            SetSkill(SkillName.Tactics, 24.6, 26.1);
            SetSkill(SkillName.Wrestling, 24.9, 26.1);

            Fame = 300;
            Karma = -300;
        }

        public Korpre(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune => Poison.Regular;

        public override Poison HitPoison => Poison.Regular;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Poor);
            AddLoot(LootPack.Gems);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
