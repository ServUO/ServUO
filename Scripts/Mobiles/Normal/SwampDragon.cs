using System;
using Server.Items;

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
            this.BaseSoundID = 0x16A;

            this.SetStr(201, 300);
            this.SetDex(66, 85);
            this.SetInt(61, 100);

            this.SetHits(121, 180);

            this.SetDamage(3, 4);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Poison, 25);

            this.SetResistance(ResistanceType.Physical, 35, 40);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 20, 40);
            this.SetResistance(ResistanceType.Poison, 20, 30);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.Anatomy, 45.1, 55.0);
            this.SetSkill(SkillName.MagicResist, 45.1, 55.0);
            this.SetSkill(SkillName.Tactics, 45.1, 55.0);
            this.SetSkill(SkillName.Wrestling, 45.1, 55.0);

            this.Fame = 2000;
            this.Karma = -2000;

            this.Hue = 0x851;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 93.9;
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
                return this.m_BardingCrafter;
            }
            set
            {
                this.m_BardingCrafter = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool BardingExceptional
        {
            get
            {
                return this.m_BardingExceptional;
            }
            set
            {
                this.m_BardingExceptional = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int BardingHP
        {
            get
            {
                return this.m_BardingHP;
            }
            set
            {
                this.m_BardingHP = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasBarding
        {
            get
            {
                return this.m_HasBarding;
            }
            set
            {
                this.m_HasBarding = value;

                if (this.m_HasBarding)
                {
                    this.Hue = CraftResources.GetHue(this.m_BardingResource);
                    this.BodyValue = 0x31F;
                    this.ItemID = 0x3EBE;
                }
                else
                {
                    this.Hue = 0x851;
                    this.BodyValue = 0x31A;
                    this.ItemID = 0x3EBD;
                }

                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource BardingResource
        {
            get
            {
                return this.m_BardingResource;
            }
            set
            {
                this.m_BardingResource = value;

                if (this.m_HasBarding)
                    this.Hue = CraftResources.GetHue(value);

                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int BardingMaxHP
        {
            get
            {
                return this.m_BardingExceptional ? 2500 : 1000;
            }
        }
        public override bool ReacquireOnMovement
        {
            get
            {
                return true;
            }
        }
        public override bool AutoDispel
        {
            get
            {
                return !this.Controlled;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat;
            }
        }
        public override int Meat
        {
            get
            {
                return 19;
            }
        }
        public override int Hides
        {
            get
            {
                return 20;
            }
        }
        public override int Scales
        {
            get
            {
                return 5;
            }
        }
        public override ScaleType ScaleType
        {
            get
            {
                return ScaleType.Green;
            }
        }
        public override bool CanAngerOnTame
        {
            get
            {
                return true;
            }
        }
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
            return 1.0;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_HasBarding && this.m_BardingExceptional && this.m_BardingCrafter != null)
                list.Add(1060853, this.m_BardingCrafter.Name); // armor exceptionally crafted by ~1_val~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((bool)this.m_BardingExceptional);
            writer.Write((Mobile)this.m_BardingCrafter);
            writer.Write((bool)this.m_HasBarding);
            writer.Write((int)this.m_BardingHP);
            writer.Write((int)this.m_BardingResource);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_BardingExceptional = reader.ReadBool();
                        this.m_BardingCrafter = reader.ReadMobile();
                        this.m_HasBarding = reader.ReadBool();
                        this.m_BardingHP = reader.ReadInt();
                        this.m_BardingResource = (CraftResource)reader.ReadInt();
                        break;
                    }
            }

            if (this.Hue == 0 && !this.m_HasBarding)
                this.Hue = 0x851;

            if (this.BaseSoundID == -1)
                this.BaseSoundID = 0x16A;
        }
    }
}