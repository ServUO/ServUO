using System;

namespace Server.Mobiles
{
    [CorpseName("a chicken lizard corpse")]
    public class ChickenLizard : BaseCreature
    {
        [Constructable]
        public ChickenLizard()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a chicken lizard";
            this.Body = 716;

            this.SetStr(74, 95);
            this.SetDex(78, 95);
            this.SetInt(6, 10);

            this.SetHits(74, 95);
			this.SetMana(6, 10);
			this.SetStam(78, 95);

            this.SetDamage(2, 5);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 20);
            this.SetResistance(ResistanceType.Fire, 5, 15);

            this.SetSkill(SkillName.MagicResist, 25.1, 29.6);
            this.SetSkill(SkillName.Tactics, 30.1, 44.9);
            this.SetSkill(SkillName.Wrestling, 26.2, 38.2);

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 0.0;
        }

        public ChickenLizard(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 3;
            }
        }
        public override MeatType MeatType
        {
            get
            {
                return MeatType.Bird;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.GrainsAndHay;
            }
        }
        public override int GetIdleSound()
        {
            return 1511;
        }

        public override int GetAngerSound()
        {
            return 1508;
        }

        public override int GetHurtSound()
        {
            return 1510;
        }

        public override int GetDeathSound()
        {
            return 1509;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}