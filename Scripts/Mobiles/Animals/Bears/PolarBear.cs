using System;

namespace Server.Mobiles
{
    [CorpseName("a polar bear corpse")]
    [TypeAlias("Server.Mobiles.Polarbear")]
    public class PolarBear : BaseCreature
    {
        [Constructable]
        public PolarBear()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a polar bear";
            this.Body = 213;
            this.BaseSoundID = 0xA3;

            this.SetStr(116, 140);
            this.SetDex(81, 105);
            this.SetInt(26, 50);

            this.SetHits(70, 84);
            this.SetMana(0);

            this.SetDamage(7, 12);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 25, 35);
            this.SetResistance(ResistanceType.Cold, 60, 80);
            this.SetResistance(ResistanceType.Poison, 15, 25);
            this.SetResistance(ResistanceType.Energy, 10, 15);

            this.SetSkill(SkillName.MagicResist, 45.1, 60.0);
            this.SetSkill(SkillName.Tactics, 60.1, 90.0);
            this.SetSkill(SkillName.Wrestling, 45.1, 70.0);

            this.Fame = 1500;
            this.Karma = 0;

            this.VirtualArmor = 18;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 35.1;
        }

        public PolarBear(Serial serial)
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
                return 16;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Fish | FoodType.FruitsAndVegies | FoodType.Meat;
            }
        }
        public override PackInstinct PackInstinct
        {
            get
            {
                return PackInstinct.Bear;
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