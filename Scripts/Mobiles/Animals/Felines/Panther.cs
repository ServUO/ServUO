using System;

namespace Server.Mobiles
{
    [CorpseName("a panther corpse")]
    public class Panther : BaseCreature
    {
        [Constructable]
        public Panther()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a panther";
            this.Body = 0xD6;
            this.Hue = 0x901;
            this.BaseSoundID = 0x462;

            this.SetStr(61, 85);
            this.SetDex(86, 105);
            this.SetInt(26, 50);

            this.SetHits(37, 51);
            this.SetMana(0);

            this.SetDamage(4, 12);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 20, 25);
            this.SetResistance(ResistanceType.Fire, 5, 10);
            this.SetResistance(ResistanceType.Cold, 10, 15);
            this.SetResistance(ResistanceType.Poison, 5, 10);

            this.SetSkill(SkillName.MagicResist, 15.1, 30.0);
            this.SetSkill(SkillName.Tactics, 50.1, 65.0);
            this.SetSkill(SkillName.Wrestling, 50.1, 65.0);

            this.Fame = 450;
            this.Karma = 0;

            this.VirtualArmor = 16;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 53.1;
        }

        public Panther(Serial serial)
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
                return 10;
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