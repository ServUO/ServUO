using System;

namespace Server.Mobiles
{
    [CorpseName("a deer corpse")]
    public class Hind : BaseCreature
    {
        [Constructable]
        public Hind()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a hind";
            this.Body = 0xED;

            this.SetStr(21, 51);
            this.SetDex(47, 77);
            this.SetInt(17, 47);

            this.SetHits(15, 29);
            this.SetMana(0);

            this.SetDamage(4);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 5, 15);
            this.SetResistance(ResistanceType.Cold, 5);

            this.SetSkill(SkillName.MagicResist, 15.0);
            this.SetSkill(SkillName.Tactics, 19.0);
            this.SetSkill(SkillName.Wrestling, 26.0);

            this.Fame = 300;
            this.Karma = 0;

            this.VirtualArmor = 8;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 23.1;
        }

        public Hind(Serial serial)
            : base(serial)
        {
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
                return 8;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
            }
        }
        public override int GetAttackSound() 
        { 
            return 0x82; 
        }

        public override int GetHurtSound() 
        { 
            return 0x83; 
        }

        public override int GetDeathSound() 
        { 
            return 0x84; 
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