namespace Server.Mobiles
{
    [CorpseName("a stygian drake corpse")]
    public class StygianDrake : BaseCreature
    {
        [Constructable]
        public StygianDrake()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Stygian Drake";
            Body = 0x58E;
            Hue = 32768;
            Female = true;
            BaseSoundID = 362;

            SetStr(790, 830);
            SetDex(85, 125);
            SetInt(400, 450);

            SetHits(480, 510);

            SetDamage(11, 17);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.MagicResist, 95.0, 105.0);
            SetSkill(SkillName.Tactics, 95.0, 105.0);
            SetSkill(SkillName.Wrestling, 90.0, 100.0);
            SetSkill(SkillName.DetectHidden, 75.0);
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 95.0, 105.0);

            Fame = 5500;
            Karma = -5500;

            Tamable = true;
            ControlSlots = 4;
            MinTameSkill = 85.0;

            SetMagicalAbility(MagicalAbility.MageryMastery);
        }

        public StygianDrake(Serial serial)
            : base(serial)
        {
        }

        public override bool AutoDispel => !Controlled;
        public override bool ReacquireOnMovement => !Controlled;
        public override int TreasureMapLevel => 2;
        public override int Meat => 10;
        public override int DragonBlood => 8;
        public override int Hides => 22;
        public override HideType HideType => HideType.Horned;
        public override int Scales => 2;
        public override ScaleType ScaleType => ScaleType.Yellow;
        public override FoodType FavoriteFood => FoodType.Meat | FoodType.Fish;
        public override bool CanFly => true;
        public override bool CanAngerOnTame => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MedScrolls, 2);
            AddLoot(LootPack.MageryRegs, 3);
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
