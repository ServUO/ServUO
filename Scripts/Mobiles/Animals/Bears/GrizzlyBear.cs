using System;

namespace Server.Mobiles
{
    [CorpseName("a grizzly bear corpse")]
    [TypeAlias("Server.Mobiles.Grizzlybear")]
    public class GrizzlyBear : BaseCreature
    {
        [Constructable]
        public GrizzlyBear()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a grizzly bear";
            this.Body = 212;
            this.BaseSoundID = 0xA3;

            this.SetStr(126, 155);
            this.SetDex(81, 105);
            this.SetInt(16, 40);

            this.SetHits(76, 93);
            this.SetMana(0);

            this.SetDamage(8, 13);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 25, 35);
            this.SetResistance(ResistanceType.Cold, 15, 25);
            this.SetResistance(ResistanceType.Poison, 5, 10);
            this.SetResistance(ResistanceType.Energy, 5, 10);

            this.SetSkill(SkillName.MagicResist, 25.1, 40.0);
            this.SetSkill(SkillName.Tactics, 70.1, 100.0);
            this.SetSkill(SkillName.Wrestling, 45.1, 70.0);

            this.Fame = 1000;
            this.Karma = 0;

            this.VirtualArmor = 24;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 59.1;
        }

        public GrizzlyBear(Serial serial)
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