namespace Server.Mobiles
{
    [CorpseName("a skeletal dragon corpse")]
    public class SkeletalDragon : BaseCreature
    {
        [Constructable]
        public SkeletalDragon()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a skeletal dragon";
            Body = 104;
            BaseSoundID = 0x488;

            SetStr(898, 1030);
            SetDex(68, 200);
            SetInt(488, 620);

            SetHits(558, 599);

            SetDamage(29, 35);

            SetDamageType(ResistanceType.Physical, 75);
            SetDamageType(ResistanceType.Fire, 25);

            SetResistance(ResistanceType.Physical, 75, 80);
            SetResistance(ResistanceType.Fire, 40, 60);
            SetResistance(ResistanceType.Cold, 40, 60);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 40, 60);

            SetSkill(SkillName.EvalInt, 80.1, 100.0);
            SetSkill(SkillName.Magery, 80.1, 100.0);
            SetSkill(SkillName.MagicResist, 100.3, 130.0);
            SetSkill(SkillName.Tactics, 97.6, 100.0);
            SetSkill(SkillName.Wrestling, 97.6, 100.0);
            SetSkill(SkillName.Necromancy, 80.1, 100.0);
            SetSkill(SkillName.SpiritSpeak, 80.1, 100.0);

            Fame = 22500;
            Karma = -22500;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public SkeletalDragon(Serial serial)
            : base(serial)
        {
        }

        public override bool AutoDispel => !Controlled;
        public override bool BleedImmune => true;
        public override bool ReacquireOnMovement => !Controlled;
        public override double BonusPetDamageScalar => 3.0;
        public override int Hides => 20;
        public override int Meat => 19;  // where's it hiding these? :)
        public override HideType HideType => HideType.Barbed;
        public override Poison PoisonImmune => Poison.Lethal;
        public override TribeType Tribe => TribeType.Undead;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 4);
            AddLoot(LootPack.Gems, 5);
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
