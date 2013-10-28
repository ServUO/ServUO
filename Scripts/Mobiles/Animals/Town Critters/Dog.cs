using System;

namespace Server.Mobiles
{
    [CorpseName("a dog corpse")]
    public class Dog : BaseCreature
    {
        [Constructable]
        public Dog()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a dog";
            this.Body = 0xD9;
            this.Hue = Utility.RandomAnimalHue();
            this.BaseSoundID = 0x85;

            this.SetStr(27, 37);
            this.SetDex(28, 43);
            this.SetInt(29, 37);

            this.SetHits(17, 22);
            this.SetMana(0);

            this.SetDamage(4, 7);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 10, 15);

            this.SetSkill(SkillName.MagicResist, 22.1, 47.0);
            this.SetSkill(SkillName.Tactics, 19.2, 31.0);
            this.SetSkill(SkillName.Wrestling, 19.2, 31.0);

            this.Fame = 0;
            this.Karma = 300;

            this.VirtualArmor = 12;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = -15.3;
        }

        public Dog(Serial serial)
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
                return FoodType.Meat;
            }
        }
        public override PackInstinct PackInstinct
        {
            get
            {
                return PackInstinct.Canine;
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