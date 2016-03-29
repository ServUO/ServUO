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
            this.Name = "a greater dragon";
            this.Body = Utility.RandomList(12, 59);
            this.BaseSoundID = 362;

            this.SetStr(1025, 1425);
            this.SetDex(81, 148);
            this.SetInt(475, 675);

            this.SetHits(1000, 2000);
            
            this.SetDamage(24, 33);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 60, 85);
            this.SetResistance(ResistanceType.Fire, 65, 90);
            this.SetResistance(ResistanceType.Cold, 40, 55);
            this.SetResistance(ResistanceType.Poison, 40, 60);
            this.SetResistance(ResistanceType.Energy, 50, 75);

            this.SetSkill(SkillName.Meditation, 0);
            this.SetSkill(SkillName.EvalInt, 110.0, 140.0);
            this.SetSkill(SkillName.Magery, 110.0, 140.0);
            this.SetSkill(SkillName.Poisoning, 0);
            this.SetSkill(SkillName.Anatomy, 0);
            this.SetSkill(SkillName.MagicResist, 110.0, 140.0);
            this.SetSkill(SkillName.Tactics, 110.0, 140.0);
            this.SetSkill(SkillName.Wrestling, 115.0, 145.0);

            this.Fame = 22000;
            this.Karma = -15000;

            this.VirtualArmor = 60;

            this.Tamable = true;
            this.ControlSlots = 5;
            this.MinTameSkill = 104.7;
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
                return !this.Controlled;
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
                return !this.Controlled;
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
                return (this.Body == 12 ? ScaleType.Yellow : ScaleType.Red);
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
            this.AddLoot(LootPack.FilthyRich, 4);
            this.AddLoot(LootPack.Gems, 8);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.BleedAttack;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)2);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

			switch(version)
			{
				case 2:
					break;
				case 1:
					this.SetDamage(24, 33);
					SetStam(0);
					break;
				case 0:
					Server.SkillHandlers.AnimalTaming.ScaleStats(this, 0.50);
					Server.SkillHandlers.AnimalTaming.ScaleSkills(this, 0.80, 0.90); // 90% * 80% = 72% of original skills trainable to 90%
					this.Skills[SkillName.Magery].Base = this.Skills[SkillName.Magery].Cap; // Greater dragons have a 90% cap reduction and 90% skill reduction on magery
					SetStam(0);
					break;
			}
        }
    }
}