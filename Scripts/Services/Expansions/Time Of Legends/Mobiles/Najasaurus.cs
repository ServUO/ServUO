namespace Server.Mobiles
{
    [CorpseName("a najasaurus corpse")]
    public class Najasaurus : BaseCreature
    {
        public override bool AttacksFocus => !Controlled;

        [Constructable]
        public Najasaurus()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .2, .4)
        {
            Name = "a najasaurus";
            Body = 1289;
            BaseSoundID = 219;

            SetStr(162, 346);
            SetDex(151, 218);
            SetInt(21, 40);

            SetDamage(13, 24);
            SetHits(737, 854);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 45, 55);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.MagicResist, 150.0, 190.0);
            SetSkill(SkillName.Tactics, 80.0, 95.0);
            SetSkill(SkillName.Wrestling, 80.0, 100.0);
            SetSkill(SkillName.Poisoning, 90.0, 100.0);
            SetSkill(SkillName.DetectHidden, 45.0, 55.0);

            Fame = 17000;
            Karma = -17000;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 102.0;
        }

        public override Poison HitPoison => Poison.Lethal;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool CanAngerOnTame => true;
        public override int TreasureMapLevel => 2;
        public override int Meat => 15;
        public override MeatType MeatType => MeatType.DinoRibs;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
        }

        public Najasaurus(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                SetMagicalAbility(MagicalAbility.Poisoning);
            }
        }
    }
}
