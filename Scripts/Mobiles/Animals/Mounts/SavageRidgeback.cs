using System;

namespace Server.Mobiles
{
    [CorpseName("a savage ridgeback corpse")]
    public class SavageRidgeback : BaseMount
    {
        [Constructable]
        public SavageRidgeback()
            : this("a savage ridgeback")
        {
        }

        [Constructable]
        public SavageRidgeback(string name)
            : base(name, 188, 0x3EB8, AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.BaseSoundID = 0x3F3;

            this.SetStr(58, 100);
            this.SetDex(56, 75);
            this.SetInt(16, 30);

            this.SetHits(41, 54);
            this.SetMana(0);

            this.SetDamage(3, 5);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 20);
            this.SetResistance(ResistanceType.Fire, 10, 15);
            this.SetResistance(ResistanceType.Cold, 15, 20);
            this.SetResistance(ResistanceType.Poison, 10, 15);
            this.SetResistance(ResistanceType.Energy, 10, 15);

            this.SetSkill(SkillName.MagicResist, 25.3, 40.0);
            this.SetSkill(SkillName.Tactics, 29.3, 44.0);
            this.SetSkill(SkillName.Wrestling, 35.1, 45.0);

            this.Fame = 300;
            this.Karma = 0;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 83.1;
        }

        public SavageRidgeback(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override int Hides
        {
            get
            {
                return 12;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Spined;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
            }
        }
        public override bool OverrideBondingReqs()
        {
            return true;
        }

        public override double GetControlChance(Mobile m, bool useBaseSkill)
        {
            return 1.0;
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