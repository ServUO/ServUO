using System;

namespace Server.Mobiles
{
    [CorpseName("a cougar corpse")]
    public class Cougar : BaseCreature
    {
        [Constructable]
        public Cougar()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a cougar";
            this.Body = 63;
            this.BaseSoundID = 0x73;

            this.SetStr(56, 80);
            this.SetDex(66, 85);
            this.SetInt(26, 50);

            this.SetHits(34, 48);
            this.SetMana(0);

            this.SetDamage(4, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 20, 25);
            this.SetResistance(ResistanceType.Fire, 5, 10);
            this.SetResistance(ResistanceType.Cold, 10, 15);
            this.SetResistance(ResistanceType.Poison, 5, 10);

            this.SetSkill(SkillName.MagicResist, 15.1, 30.0);
            this.SetSkill(SkillName.Tactics, 45.1, 60.0);
            this.SetSkill(SkillName.Wrestling, 45.1, 60.0);

            this.Fame = 450;
            this.Karma = 0;

            this.VirtualArmor = 16;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 41.1;
        }

        public Cougar(Serial serial)
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
                return FoodType.Fish | FoodType.Meat;
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