using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a dragon corpse")]
    public class GreaterDragon : BaseCreature
    {
        [Constructable]
        public GreaterDragon()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.3, 0.5)
        {
            Name = "a greater dragon";
            Body = Utility.RandomList(12, 59);
            BaseSoundID = 362;

            SetStr(1025, 1425);
            SetDex(81, 148);
            SetInt(475, 675);

            SetHits(1000, 2000);
            
            SetDamage(24, 33);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 60, 85);
            SetResistance(ResistanceType.Fire, 65, 90);
            SetResistance(ResistanceType.Cold, 40, 55);
            SetResistance(ResistanceType.Poison, 40, 60);
            SetResistance(ResistanceType.Energy, 50, 75);

            SetSkill(SkillName.Meditation, 0);
            SetSkill(SkillName.EvalInt, 110.0, 140.0);
            SetSkill(SkillName.Magery, 110.0, 140.0);
            SetSkill(SkillName.Poisoning, 0);
            SetSkill(SkillName.Anatomy, 0);
            SetSkill(SkillName.MagicResist, 110.0, 140.0);
            SetSkill(SkillName.Tactics, 110.0, 140.0);
            SetSkill(SkillName.Wrestling, 115.0, 145.0);

            Fame = 22000;
            Karma = -15000;

            VirtualArmor = 60;

            Tamable = true;
            ControlSlots = 5;
            MinTameSkill = 104.7;

            SetWeaponAbility(WeaponAbility.BleedAttack);
        }

        public GreaterDragon(Serial serial)
            : base(serial)
        {
        }

        public override bool StatLossAfterTame
        {
            get
            {
                return true;
            }
        }
        public override bool ReacquireOnMovement
        {
            get
            {
                return !Controlled;
            }
        }
        public override bool HasBreath
        {
            get
            {
                return true;
            }
        }// fire breath enabled
        public override bool AutoDispel
        {
            get
            {
                return !Controlled;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
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
                return 30;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Barbed;
            }
        }
        public override int Scales
        {
            get
            {
                return 7;
            }
        }
        public override ScaleType ScaleType
        {
            get
            {
                return (Body == 12 ? ScaleType.Yellow : ScaleType.Red);
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat;
            }
        }
        public override bool CanAngerOnTame
        {
            get
            {
                return true;
            }
        }
        public override bool CanFly
        {
            get
            {
                return true;
            }
        }
        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 4);
            AddLoot(LootPack.Gems, 8);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)3);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

			switch(version)
			{
                case 3:
				case 2:
					break;
				case 1:
					SetDamage(24, 33);
					SetStam(0);
					break;
				case 0:
					Server.SkillHandlers.AnimalTaming.ScaleStats(this, 0.50);
					Server.SkillHandlers.AnimalTaming.ScaleSkills(this, 0.80, 0.90); // 90% * 80% = 72% of original skills trainable to 90%
					Skills[SkillName.Magery].Base = Skills[SkillName.Magery].Cap; // Greater dragons have a 90% cap reduction and 90% skill reduction on magery
					SetStam(0);
					break;
			}

            if (version == 2)
            {
                SetWeaponAbility(WeaponAbility.BleedAttack);
            }
        }
    }
}