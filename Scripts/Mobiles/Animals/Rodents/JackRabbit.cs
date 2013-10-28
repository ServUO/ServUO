using System;

namespace Server.Mobiles
{
    [CorpseName("a jack rabbit corpse")]
    [TypeAlias("Server.Mobiles.Jackrabbit")]
    public class JackRabbit : BaseCreature
    {
        [Constructable]
        public JackRabbit()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a jack rabbit";
            this.Body = 0xCD;
            this.Hue = 0x1BB;

            this.SetStr(15);
            this.SetDex(25);
            this.SetInt(5);

            this.SetHits(9);
            this.SetMana(0);

            this.SetDamage(1, 2);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 2, 5);

            this.SetSkill(SkillName.MagicResist, 5.0);
            this.SetSkill(SkillName.Tactics, 5.0);
            this.SetSkill(SkillName.Wrestling, 5.0);

            this.Fame = 150;
            this.Karma = 0;

            this.VirtualArmor = 4;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = -18.9;
        }

        public JackRabbit(Serial serial)
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
        public override int GetAttackSound() 
        { 
            return 0xC9; 
        }

        public override int GetHurtSound() 
        { 
            return 0xCA; 
        }

        public override int GetDeathSound() 
        { 
            return 0xCB; 
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