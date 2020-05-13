namespace Server.Mobiles
{
    [CorpseName("a crimson drake corpse")]
    public class CrimsonDrake : BaseCreature, IElementalCreature
    {
        private ElementType m_Type;

        public ElementType ElementType => m_Type;

        [Constructable]
        public CrimsonDrake()
            : this((ElementType)Utility.Random(5))
        {
        }

        [Constructable]
        public CrimsonDrake(ElementType type)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            m_Type = type;

            switch (type)
            {
                case ElementType.Physical:
                    Body = 0x58B;
                    Hue = 0;
                    SetDamageType(ResistanceType.Physical, 100);
                    break;
                case ElementType.Fire:
                    Body = 0x58C;
                    Hue = 33929;
                    SetDamageType(ResistanceType.Physical, 0);
                    SetDamageType(ResistanceType.Fire, 100);
                    break;
                case ElementType.Cold:
                    Body = 0x58C;
                    Hue = 34134;
                    SetDamageType(ResistanceType.Physical, 0);
                    SetDamageType(ResistanceType.Cold, 100);
                    break;
                case ElementType.Poison:
                    Body = 0x58C;
                    Hue = 34136;
                    SetDamageType(ResistanceType.Physical, 0);
                    SetDamageType(ResistanceType.Poison, 100);
                    break;
                case ElementType.Energy:
                    Body = 0x58C;
                    Hue = 34141;
                    SetDamageType(ResistanceType.Physical, 0);
                    SetDamageType(ResistanceType.Energy, 100);
                    break;
            }

            Name = "Crimson Drake";
            Female = true;
            BaseSoundID = 362;

            SetStr(400, 430);
            SetDex(133, 152);
            SetInt(101, 140);

            SetHits(241, 258);

            SetDamage(11, 17);

            SetResistance(ResistanceType.Physical, 30, 50);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 30, 40);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 30, 50);

            SetSkill(SkillName.MagicResist, 65.1, 80.0);
            SetSkill(SkillName.Tactics, 65.1, 90.0);
            SetSkill(SkillName.Wrestling, 65.1, 80.0);
            SetSkill(SkillName.DetectHidden, 50.0, 60.0);
            SetSkill(SkillName.Focus, 5.0, 10.0);

            Fame = 5500;
            Karma = -5500;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 85.0;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public CrimsonDrake(Serial serial)
            : base(serial)
        {
        }

        public static TrainingDefinition _PoisonDrakeDefinition;

        public override TrainingDefinition TrainingDefinition
        {
            get
            {
                if (m_Type == ElementType.Poison)
                {
                    if (_PoisonDrakeDefinition == null)
                    {
                        _PoisonDrakeDefinition = new TrainingDefinition(GetType(), Class.None, MagicalAbility.Dragon2, PetTrainingHelper.SpecialAbilityNone, PetTrainingHelper.WepAbility2, PetTrainingHelper.AreaEffectArea2, 2, 5);
                    }

                    return _PoisonDrakeDefinition;
                }

                return base.TrainingDefinition;
            }
        }

        public override bool ReacquireOnMovement => !Controlled;
        public override int TreasureMapLevel => 2;
        public override int Meat => 10;
        public override int DragonBlood => 8;
        public override int Hides => 22;
        public override HideType HideType => HideType.Horned;
        public override int Scales => 2;
        public override ScaleType ScaleType => ScaleType.Black;
        public override FoodType FavoriteFood => FoodType.Meat | FoodType.Fish;
        public override bool CanFly => true;

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

            writer.Write((int)m_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Type = (ElementType)reader.ReadInt();
        }
    }
}
