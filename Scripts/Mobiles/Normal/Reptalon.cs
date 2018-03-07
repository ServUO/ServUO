using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a reptalon corpse")]
    public class Reptalon : BaseMount
    {
        [Constructable]
        public Reptalon()
            : base("a reptalon", 0x114, 0x3E90, AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.35)
        {
            this.BaseSoundID = 0x16A;

            this.SetStr(1001, 1025);
            this.SetDex(152, 164);
            this.SetInt(251, 289);

            this.SetHits(833, 931);

            this.SetDamage(21, 28);
			
            this.SetDamageType(ResistanceType.Physical, 0);
            this.SetDamageType(ResistanceType.Poison, 25);
            this.SetDamageType(ResistanceType.Energy, 75);

            this.SetResistance(ResistanceType.Physical, 53, 64);
            this.SetResistance(ResistanceType.Fire, 35, 45);
            this.SetResistance(ResistanceType.Cold, 36, 45);
            this.SetResistance(ResistanceType.Poison, 52, 63);
            this.SetResistance(ResistanceType.Energy, 71, 83);

            this.SetSkill(SkillName.Wrestling, 101.5, 118.2);
            this.SetSkill(SkillName.Tactics, 101.7, 108.2);
            this.SetSkill(SkillName.MagicResist, 76.4, 89.9);
            this.SetSkill(SkillName.Anatomy, 56.4, 59.7);
			
            this.Tamable = true;
            this.ControlSlots = 4;
            this.MinTameSkill = 101.1;
        }

        public Reptalon(Serial serial)
            : base(serial)
        {
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
                return 5;
            }
        }
        public override int Hides
        {
            get
            {
                return 10;
            }
        }
        public override bool CanBreath
        {
            get
            {
                return true;
            }
        }
        public override bool CanAngerOnTame
        {
            get
            {
                return true;
            }
        }
        public override bool StatLossAfterTame
        {
            get
            {
                return true;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat;
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
            this.AddLoot(LootPack.AosUltraRich, 3);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.ParalyzingBlow;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}