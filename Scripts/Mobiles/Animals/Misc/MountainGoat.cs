using System;

namespace Server.Mobiles
{
    [CorpseName("a mountain goat corpse")]
    public class MountainGoat : BaseCreature
    {
        [Constructable]
        public MountainGoat()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a mountain goat";
            this.Body = 88;
            this.BaseSoundID = 0x99;

            this.SetStr(22, 64);
            this.SetDex(56, 75);
            this.SetInt(16, 30);

            this.SetHits(20, 33);
            this.SetMana(0);

            this.SetDamage(3, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 10, 20);
            this.SetResistance(ResistanceType.Fire, 5, 10);
            this.SetResistance(ResistanceType.Cold, 10, 20);
            this.SetResistance(ResistanceType.Poison, 10, 15);
            this.SetResistance(ResistanceType.Energy, 10, 15);

            this.SetSkill(SkillName.MagicResist, 25.1, 30.0);
            this.SetSkill(SkillName.Tactics, 29.3, 44.0);
            this.SetSkill(SkillName.Wrestling, 29.3, 44.0);

            this.Fame = 300;
            this.Karma = 0;

            this.VirtualArmor = 10;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = -0.9;
        }

        public MountainGoat(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 2;
            }
        }
        public override int Hides
        {
            get
            {
                return 12;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.GrainsAndHay | FoodType.FruitsAndVegies;
            }
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