using System;

namespace Server.Mobiles
{
    [CorpseName("a chicken corpse")]
    public class Chicken : BaseCreature
    {
        [Constructable]
        public Chicken()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a chicken";
            this.Body = 0xD0;
            this.BaseSoundID = 0x6E;

            this.SetStr(5);
            this.SetDex(15);
            this.SetInt(5);

            this.SetHits(3);
            this.SetMana(0);

            this.SetDamage(1);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 1, 5);

            this.SetSkill(SkillName.MagicResist, 4.0);
            this.SetSkill(SkillName.Tactics, 5.0);
            this.SetSkill(SkillName.Wrestling, 5.0);

            this.Fame = 150;
            this.Karma = 0;

            this.VirtualArmor = 2;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = -0.9;
        }

        public Chicken(Serial serial)
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
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.GrainsAndHay;
            }
        }
        public override bool CanFly
        {
            get
            {
                return true;
            }
        }
        public override int Feathers
        {
            get
            {
                return 25;
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