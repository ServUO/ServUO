using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a snake corpse")]
    public class CoralSnake : BaseCreature
    {
        [Constructable]
        public CoralSnake()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a coral snake";
            Body = 52;
            Hue = 0x21;
            BaseSoundID = 0xDB;

            SetStr(205, 340);
            SetDex(248, 300);
            SetInt(28, 35);

            SetHits(132, 200);
            SetMana(28, 35);

            SetDamage(5, 21);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetResistance(ResistanceType.Physical, 42, 50);
            SetResistance(ResistanceType.Fire, 5, 20);
            SetResistance(ResistanceType.Physical, 5, 20);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 5, 20);

            SetSkill(SkillName.Poisoning, 99.7, 110.9);
            SetSkill(SkillName.MagicResist, 98.1, 105.0);
            SetSkill(SkillName.Tactics, 82.0, 98.0);
            SetSkill(SkillName.Wrestling, 90.3, 105.0);

            Fame = 300;
            Karma = -300;

            VirtualArmor = 16;

            Tamable = false;
            ControlSlots = 1;
            MinTameSkill = 59.1;

            QLPoints = 3;
        }

        public CoralSnake(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get { return Poison.Lesser; }
        }

        public override Poison HitPoison
        {
            get { return Poison.Deadly; }
        }

        //public override bool DeathAdderCharmable{ get{ return true; } }
        public override int Meat
        {
            get { return 1; }
        }

        public override FoodType FavoriteFood
        {
            get { return FoodType.Eggs; }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            var version = reader.ReadInt();
        }
    }
}