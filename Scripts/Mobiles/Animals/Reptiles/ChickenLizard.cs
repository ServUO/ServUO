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

            this.SetStr(78, 87);
            this.SetDex(87, 92);
            this.SetInt(8);

            this.SetHits(77, 82);

            this.SetDamage(2, 5);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 18, 20);
            this.SetResistance(ResistanceType.Fire, 7, 14);

            this.SetSkill(SkillName.MagicResist, 0.0, 28.5);
            this.SetSkill(SkillName.Tactics, 0.0, 41.3);
            this.SetSkill(SkillName.Wrestling, 0.0, 35.8);

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
                return FoodType.Meat;
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