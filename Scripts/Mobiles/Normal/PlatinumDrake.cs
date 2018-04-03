using System;

namespace Server.Mobiles
{
    public enum DrakeType
    {
        Physical,
        Fire,
        Cold,
        Poison,
        Energy
    }

    public interface IDrake
    {
        DrakeType DrakeType { get; }
    }

    [CorpseName("a platinum drake corpse")]
    public class PlatinumDrake : BaseCreature, IDrake
    {
        private DrakeType m_Type;

        public DrakeType DrakeType { get { return m_Type; } }

        [Constructable]
        public PlatinumDrake()
            : this((DrakeType)Utility.Random(5))
        {
        }

        [Constructable]
        public PlatinumDrake(DrakeType type)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            m_Type = type;

            switch (type)
            {
                case DrakeType.Physical:
                    Body = 0x589;
                    Hue = 0;
                    SetDamageType(ResistanceType.Physical, 100);
                    break;
                case DrakeType.Fire:
                    Body = 0x58A;
                    Hue = 33929;
                    SetDamageType(ResistanceType.Physical, 0);
                    SetDamageType(ResistanceType.Fire, 100);
                    break;
                case DrakeType.Cold:
                    Body = 0x58A;
                    Hue = 34134;
                    SetDamageType(ResistanceType.Physical, 0);
                    SetDamageType(ResistanceType.Cold, 100);
                    break;
                case DrakeType.Poison:
                    Body = 0x58A;
                    Hue = 34136;
                    SetDamageType(ResistanceType.Physical, 0);
                    SetDamageType(ResistanceType.Poison, 100);
                    break;
                case DrakeType.Energy:
                    Body = 0x58A;
                    Hue = 34141;
                    SetDamageType(ResistanceType.Physical, 0);
                    SetDamageType(ResistanceType.Energy, 100);
                    break;
            }

            Name = "Platinum Drake";
            Female = true;
            BaseSoundID = 362;

            SetStr(400, 430);
            SetDex(133, 152);
            SetInt(101, 140);

            SetHits(241, 258);

            SetDamage(11, 17);

            SetResistance(ResistanceType.Physical, 30, 50);
            SetResistance(ResistanceType.Fire, 30, 50);
            SetResistance(ResistanceType.Cold, 30, 50);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 30, 50);

            SetSkill(SkillName.MagicResist, 65.1, 80.0);
            SetSkill(SkillName.Tactics, 65.1, 90.0);
            SetSkill(SkillName.Wrestling, 65.1, 80.0);
            SetSkill(SkillName.DetectHidden, 50.0, 60.0);
            SetSkill(SkillName.Focus, 5.0, 20.0);

            Fame = 5500;
            Karma = -5500;

            VirtualArmor = 46;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 85.0;

            PackReg(3);
        }

        public PlatinumDrake(Serial serial)
            : base(serial)
        {
        }

        public override bool ReacquireOnMovement { get { return !Controlled; } }
        public override bool HasBreath { get { return true; } }
        public override int BreathPhysicalDamage { get { return m_Type == DrakeType.Physical ? 100 : 0; } }
        public override int BreathFireDamage { get { return m_Type == DrakeType.Fire ? 100 : 0; } }
        public override int BreathColdDamage { get { return m_Type == DrakeType.Cold ? 100 : 0; } }
        public override int BreathPoisonDamage { get { return m_Type == DrakeType.Poison ? 100 : 0; } }
        public override int BreathEffectHue { get { return m_Type == DrakeType.Cold ? 0x480 : 0; } }
        public override int TreasureMapLevel { get { return 2; } }
        public override int Meat { get { return 10; } }
        public override int DragonBlood { get { return 8; } }
        public override int Hides { get { return 22; } }
        public override HideType HideType { get { return HideType.Horned; } }
        public override int Scales { get { return 2; } }
        public override ScaleType ScaleType { get { return ScaleType.Black; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat | FoodType.Fish; } }
        public override bool CanFly { get { return true; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.MedScrolls, 2);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write((int)m_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Type = (DrakeType)reader.ReadInt();
        }
    }
}