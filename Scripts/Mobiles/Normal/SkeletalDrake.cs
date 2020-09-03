namespace Server.Mobiles
{
    [CorpseName("a skeletal drake corpse")]
    public class SkeletalDrake : BaseCreature
    {
        [Constructable]
        public SkeletalDrake()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a skeletal drake";
            Body = 104;
            Hue = 2101;
            BaseSoundID = 0x488;

            SetStr(600, 700);
            SetDex(70, 100);
            SetInt(300, 400);

            SetHits(300, 400);

            SetDamage(29, 35);

            SetDamageType(ResistanceType.Physical, 75);
            SetDamageType(ResistanceType.Fire, 25);

            SetResistance(ResistanceType.Physical, 75, 80);
            SetResistance(ResistanceType.Fire, 40, 60);
            SetResistance(ResistanceType.Cold, 40, 60);
            SetResistance(ResistanceType.Poison, 70, 80);
            SetResistance(ResistanceType.Energy, 40, 60);

            SetSkill(SkillName.EvalInt, 45, 60);
            SetSkill(SkillName.Magery, 50, 65);
            SetSkill(SkillName.MagicResist, 75, 90);
            SetSkill(SkillName.Tactics, 70, 85);
            SetSkill(SkillName.Wrestling, 60, 75);

            Fame = 15000;
            Karma = -15000;
        }

        public SkeletalDrake(Serial serial)
            : base(serial)
        {
        }

        public override bool AutoDispel => true;
		
        public override bool BleedImmune => true;
		
        public override bool ReacquireOnMovement => true;
		
        public override int Hides => 20;
		
        public override int Meat => 19; 
		
        public override HideType HideType => HideType.Barbed;
		
        public override Poison PoisonImmune => Poison.Lethal;
		
        public override TribeType Tribe => TribeType.Undead;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.Gems, 4);
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
