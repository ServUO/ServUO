using System;

namespace Server.Mobiles
{
    [CorpseName("a cat corpse")]
    [TypeAlias("Server.Mobiles.Housecat")]
    public class Cat : BaseCreature
    {
        [Constructable]
        public Cat()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a cat";
            this.Body = 0xC9;
            this.Hue = Utility.RandomAnimalHue();
            this.BaseSoundID = 0x69;

            this.SetStr(9);
            this.SetDex(35);
            this.SetInt(5);

            this.SetHits(6);
            this.SetMana(0);

            this.SetDamage(1);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 5, 10);

            this.SetSkill(SkillName.MagicResist, 5.0);
            this.SetSkill(SkillName.Tactics, 4.0);
            this.SetSkill(SkillName.Wrestling, 5.0);
            this.SetSkill(SkillName.Hiding, 20.0);

            this.Fame = 0;
            this.Karma = 150;

            this.VirtualArmor = 8;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = -0.9;
        }

        public Cat(Serial serial)
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
                return FoodType.Meat | FoodType.Fish;
            }
        }
        public override PackInstinct PackInstinct
        {
            get
            {
                return PackInstinct.Feline;
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