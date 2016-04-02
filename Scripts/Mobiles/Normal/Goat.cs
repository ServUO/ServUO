using System;

namespace Server.Mobiles
{
    [CorpseName("a goat corpse")]
    public class Goat : BaseCreature
    {
        [Constructable]
        public Goat()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a goat";
            this.Body = 0xD1;
            this.BaseSoundID = 0x99;

            this.SetStr(19);
            this.SetDex(15);
            this.SetInt(5);

            this.SetHits(12);
            this.SetMana(0);

            this.SetDamage(3, 4);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 5, 15);

            this.SetSkill(SkillName.MagicResist, 5.0);
            this.SetSkill(SkillName.Tactics, 5.0);
            this.SetSkill(SkillName.Wrestling, 5.0);

            this.Fame = 150;
            this.Karma = 0;

            this.VirtualArmor = 10;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 11.1;
        }

        public Goat(Serial serial)
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
                return 8;
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