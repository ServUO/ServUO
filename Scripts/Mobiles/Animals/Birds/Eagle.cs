using System;

namespace Server.Mobiles
{
    [CorpseName("an eagle corpse")]
    public class Eagle : BaseCreature
    {
        [Constructable]
        public Eagle()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "an eagle";
            this.Body = 5;
            this.BaseSoundID = 0x2EE;

            this.SetStr(31, 47);
            this.SetDex(36, 60);
            this.SetInt(8, 20);

            this.SetHits(20, 27);
            this.SetMana(0);

            this.SetDamage(5, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 20, 25);
            this.SetResistance(ResistanceType.Fire, 10, 15);
            this.SetResistance(ResistanceType.Cold, 20, 25);
            this.SetResistance(ResistanceType.Poison, 5, 10);
            this.SetResistance(ResistanceType.Energy, 5, 10);

            this.SetSkill(SkillName.MagicResist, 15.3, 30.0);
            this.SetSkill(SkillName.Tactics, 18.1, 37.0);
            this.SetSkill(SkillName.Wrestling, 20.1, 30.0);

            this.Fame = 300;
            this.Karma = 0;

            this.VirtualArmor = 22;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 17.1;
        }

        public Eagle(Serial serial)
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
        public override MeatType MeatType
        {
            get
            {
                return MeatType.Bird;
            }
        }
        public override int Feathers
        {
            get
            {
                return 36;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat | FoodType.Fish;
            }
        }
        public override bool CanFly
        {
            get
            {
                return true;
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