using System;

namespace Server.Mobiles
{
    [CorpseName("a reindeer corpse")]
    [TypeAlias("Server.Mobiles.Greathart")]
    public class Reindeer : BaseCreature
    {
        [Constructable]
        public Reindeer()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "Reindeer";
            Body = 0xEA;
            Blessed = true;

            SetStr(41, 71);
            SetDex(47, 77);
            SetInt(27, 57);

            SetHits(27, 41);
            SetMana(0);

            SetDamage(5, 9);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 20, 25);
            SetResistance(ResistanceType.Cold, 5, 10);

            SetSkill(SkillName.MagicResist, 26.8, 44.5);
            SetSkill(SkillName.Tactics, 29.8, 47.5);
            SetSkill(SkillName.Wrestling, 29.8, 47.5);

            Fame = 300;
            Karma = 0;

            VirtualArmor = 24;

            Tamable = false;
            ControlSlots = 1;
            MinTameSkill = 59.1;
        }

        public Reindeer(Serial serial)
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