using System;

namespace Server.Mobiles
{
    [CorpseName("a snake corpse")]
    public class CoralSnake : BaseCreature
    {
        [Constructable]
        public CoralSnake()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a coral snake";
            this.Body = 52;
            this.Hue = 0x21;
            this.BaseSoundID = 0xDB;

            this.SetStr(205, 340);
            this.SetDex(248, 300);
            this.SetInt(28, 35);

            this.SetHits(132, 200);
            this.SetMana(28, 35);

            this.SetDamage(5, 21);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Poison, 50);

            this.SetResistance(ResistanceType.Physical, 42, 50);
            this.SetResistance(ResistanceType.Fire, 5, 20);
            this.SetResistance(ResistanceType.Physical, 5, 20);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 5, 20);

            this.SetSkill(SkillName.Poisoning, 99.7, 110.9);
            this.SetSkill(SkillName.MagicResist, 98.1, 105.0);
            this.SetSkill(SkillName.Tactics, 82.0, 98.0);
            this.SetSkill(SkillName.Wrestling, 90.3, 105.0);

            this.Fame = 300;
            this.Karma = -300;

            this.VirtualArmor = 16;

            this.Tamable = false;
            this.ControlSlots = 1;
            this.MinTameSkill = 59.1;
        }

        public CoralSnake(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lesser;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Poison.Deadly;
            }
        }
        //public override bool DeathAdderCharmable{ get{ return true; } }
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
                return FoodType.Eggs;
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