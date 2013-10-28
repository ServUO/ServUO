using System;

namespace Server.Mobiles
{
    [CorpseName("a squirrel corpse")]	
    public class Squirrel : BaseCreature
    {
        [Constructable]
        public Squirrel()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a squirrel";
            this.Body = 0x116;

            this.SetStr(44, 50);
            this.SetDex(35);
            this.SetInt(5);

            this.SetHits(42, 50);

            this.SetDamage(1, 2);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 30, 34);
            this.SetResistance(ResistanceType.Fire, 10, 14);
            this.SetResistance(ResistanceType.Cold, 30, 35);
            this.SetResistance(ResistanceType.Poison, 20, 25);
            this.SetResistance(ResistanceType.Energy, 20, 25);

            this.SetSkill(SkillName.MagicResist, 4.0);
            this.SetSkill(SkillName.Tactics, 4.0);
            this.SetSkill(SkillName.Wrestling, 4.0);

            this.Tamable = true;	
            this.ControlSlots = 1;
            this.MinTameSkill = -21.3;
        }

        public Squirrel(Serial serial)
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
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.FruitsAndVegies;
            }
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