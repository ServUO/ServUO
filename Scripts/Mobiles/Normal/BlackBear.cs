using System;

namespace Server.Mobiles
{
    [CorpseName("a bear corpse")]
    [TypeAlias("Server.Mobiles.Bear")]
    public class BlackBear : BaseCreature
    {
        [Constructable]
        public BlackBear()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a black bear";
            this.Body = 211;
            this.BaseSoundID = 0xA3;

            this.SetStr(76, 100);
            this.SetDex(56, 75);
            this.SetInt(11, 14);

            this.SetHits(46, 60);
            this.SetMana(0);

            this.SetDamage(4, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 20, 25);
            this.SetResistance(ResistanceType.Cold, 10, 15);
            this.SetResistance(ResistanceType.Poison, 5, 10);

            this.SetSkill(SkillName.MagicResist, 20.1, 40.0);
            this.SetSkill(SkillName.Tactics, 40.1, 60.0);
            this.SetSkill(SkillName.Wrestling, 40.1, 60.0);

            this.Fame = 450;
            this.Karma = 0;

            this.VirtualArmor = 24;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 35.1;
        }

        public BlackBear(Serial serial)
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
                return 12;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Fish | FoodType.Meat | FoodType.FruitsAndVegies;
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