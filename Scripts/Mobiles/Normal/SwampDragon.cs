using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("a swamp dragon corpse")]
    public class SwampDragon : BaseMount
    {
        private bool m_BardingExceptional;
        private Mobile m_BardingCrafter;
        private int m_BardingHP;
        private bool m_HasBarding;
        private CraftResource m_BardingResource;

        [Constructable]
        public SwampDragon()
            : this("a swamp dragon")
        {
        }

        [Constructable]
        public SwampDragon(string name)
            : base(name, 0x31A, 0x3EBD, AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            BaseSoundID = 0x16A;

            SetStr(201, 300);
            SetDex(66, 85);
            SetInt(61, 100);

            SetHits(121, 180);

            SetDamage(3, 4);

            SetDamageType(ResistanceType.Physical, 75);
            SetDamageType(ResistanceType.Poison, 25);

            SetResistance(ResistanceType.Physical, 35, 40);
            SetResistance(ResistanceType.Fire, 20, 30);
            SetResistance(ResistanceType.Cold, 20, 40);
            SetResistance(ResistanceType.Poison, 20, 30);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.Anatomy, 45.1, 55.0);
            SetSkill(SkillName.MagicResist, 45.1, 55.0);
            SetSkill(SkillName.Tactics, 45.1, 55.0);
            SetSkill(SkillName.Wrestling, 45.1, 55.0);

            Fame = 2000;
            Karma = -2000;

            Hue = 0x851;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 93.9;
        }

        public SwampDragon(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile BardingCrafter
        {
            get
            {
                return m_BardingCrafter;
            }
            set
            {
                m_BardingCrafter = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool BardingExceptional
        {
            get
            {
                return m_BardingExceptional;
            }
            set
            {
                m_BardingExceptional = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int BardingHP
        {
            get
            {
                return m_BardingHP;
            }
            set
            {
                m_BardingHP = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasBarding
        {
            get
            {
                return m_HasBarding;
            }
            set
            {
                m_HasBarding = value;

                if (m_HasBarding)
                {
                    Hue = CraftResources.GetHue(m_BardingResource);
                    BodyValue = 0x31F;
                    ItemID = 0x3EBE;
                }
                else
                {
                    Hue = 0x851;
                    BodyValue = 0x31A;
                    ItemID = 0x3EBD;
                }

                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource BardingResource
        {
            get
            {
                return m_BardingResource;
            }
            set
            {
                m_BardingResource = value;

                if (m_HasBarding)
                    Hue = CraftResources.GetHue(value);

                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int BardingMaxHP
        {
            get
            {
                switch (m_BardingResource)
                {
                    default:
                        return BardingExceptional ? 12000 : 10000;
                    case CraftResource.DullCopper:
                    case CraftResource.Valorite:
                        return BardingExceptional ? 14500 : 12500;
                    case CraftResource.ShadowIron:
                        return BardingExceptional ? 17000 : 15000;
                }
            }
        }

        private int CalculateBardingResistance(ResistanceType type)
        {
            if (m_BardingResource == CraftResource.None || !m_HasBarding)
                return 0;

            CraftResourceInfo resInfo = CraftResources.GetInfo(m_BardingResource);

            if (resInfo == null)
                return 0;

            CraftAttributeInfo attrs = resInfo.AttributeInfo;

            if (attrs == null)
                return 0;

            int expBonus = BardingExceptional ? 1 : 0;
            int resBonus = 0;

            switch (type)
            {
                default:
                case ResistanceType.Physical: resBonus = Math.Max(5, attrs.ArmorPhysicalResist); break;
                case ResistanceType.Fire: resBonus = Math.Max(3, attrs.ArmorFireResist); break;
                case ResistanceType.Cold: resBonus = Math.Max(2, attrs.ArmorColdResist); break;
                case ResistanceType.Poison: resBonus = Math.Max(3, attrs.ArmorPoisonResist); break;
                case ResistanceType.Energy: resBonus = Math.Max(2, attrs.ArmorEnergyResist); break;
            }

            return (resBonus + expBonus) * 5;
        }

        public override int GetResistance(ResistanceType type)
        {
            return base.GetResistance(type) + CalculateBardingResistance(type);
        }

        public override bool ReacquireOnMovement => true;
        public override bool AutoDispel => !Controlled;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override int Meat => 19;
        public override int Hides => 20;
        public override int Scales => 5;
        public override ScaleType ScaleType => ScaleType.Green;
        public override bool CanAngerOnTame => true;
        public override bool OverrideBondingReqs()
        {
            return true;
        }

        public override int GetIdleSound()
        {
            return 0x2CE;
        }

        public override int GetDeathSound()
        {
            return 0x2CC;
        }

        public override int GetHurtSound()
        {
            return 0x2D1;
        }

        public override int GetAttackSound()
        {
            return 0x2C8;
        }

        public override double GetControlChance(Mobile m, bool useBaseSkill)
        {
            AbilityProfile profile = PetTrainingHelper.GetAbilityProfile(this);

            if (profile != null && profile.HasCustomized())
            {
                return base.GetControlChance(m, useBaseSkill);
            }

            return 1.0;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_HasBarding && m_BardingExceptional && m_BardingCrafter != null)
            {
                list.Add(1060853, m_BardingCrafter.Name); // armor exceptionally crafted by ~1_val~
            }

            if (m_HasBarding)
            {
                list.Add(1115719, m_BardingHP.ToString()); // armor points: ~1_val~
            }
        }

        public override void OnRiderDamaged(Mobile from, ref int amount, bool willKill)
        {
            base.OnRiderDamaged(from, ref amount, willKill);

            if (Rider == null)
                return;

            if ((from == null || !from.Player) && Rider.Player && Rider.Mount == this)
            {
                if (HasBarding)
                {
                    int percent = (BardingExceptional ? 20 : 10);
                    int absorbed = AOS.Scale(amount, percent);

                    amount -= absorbed;

                    // Mondain's Legacy mod
                    if (!(this is ParoxysmusSwampDragon))
                        BardingHP -= absorbed;

                    if (BardingHP < 0)
                    {
                        HasBarding = false;
                        BardingHP = 0;

                        Rider.SendLocalizedMessage(1053031); // Your dragon's barding has been destroyed!
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            writer.Write(m_BardingExceptional);
            writer.Write(m_BardingCrafter);
            writer.Write(m_HasBarding);
            writer.Write(m_BardingHP);
            writer.Write((int)m_BardingResource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_BardingExceptional = reader.ReadBool();
                        m_BardingCrafter = reader.ReadMobile();
                        m_HasBarding = reader.ReadBool();
                        m_BardingHP = reader.ReadInt();
                        m_BardingResource = (CraftResource)reader.ReadInt();
                        break;
                    }
            }

            if (Hue == 0 && !m_HasBarding)
                Hue = 0x851;

            if (BaseSoundID == -1)
                BaseSoundID = 0x16A;
        }
    }
}
