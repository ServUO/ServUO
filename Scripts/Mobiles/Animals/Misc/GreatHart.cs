using System;

namespace Server.Mobiles
{
    [CorpseName("a deer corpse")]
    [TypeAlias("Server.Mobiles.Greathart")]
    public class GreatHart : BaseCreature
    {
        [Constructable]
        public GreatHart()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a great hart";
            this.Body = 0xEA;

            this.SetStr(41, 71);
            this.SetDex(47, 77);
            this.SetInt(27, 57);

            this.SetHits(27, 41);
            this.SetMana(0);

            this.SetDamage(5, 9);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 20, 25);
            this.SetResistance(ResistanceType.Cold, 5, 10);

            this.SetSkill(SkillName.MagicResist, 26.8, 44.5);
            this.SetSkill(SkillName.Tactics, 29.8, 47.5);
            this.SetSkill(SkillName.Wrestling, 29.8, 47.5);

            this.Fame = 300;
            this.Karma = 0;

            this.VirtualArmor = 24;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 59.1;
        }

        public GreatHart(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 6;
            }
        }
        public override int Hides
        {
            get
            {
                return 15;
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