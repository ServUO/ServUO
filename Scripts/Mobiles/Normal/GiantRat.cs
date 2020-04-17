namespace Server.Mobiles
{
    [CorpseName("a giant rat corpse")]
    public class GiantRat : BaseCreature
    {
        [Constructable]
        public GiantRat()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a giant rat";
            Body = 0xD7;
            BaseSoundID = 0x188;

            SetStr(32, 74);
            SetDex(46, 65);
            SetInt(16, 30);

            SetHits(26, 39);
            SetMana(0);

            SetDamage(4, 8);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 20);
            SetResistance(ResistanceType.Fire, 5, 10);
            SetResistance(ResistanceType.Poison, 25, 35);

            SetSkill(SkillName.MagicResist, 25.1, 30.0);
            SetSkill(SkillName.Tactics, 29.3, 44.0);
            SetSkill(SkillName.Wrestling, 29.3, 44.0);

            Fame = 300;
            Karma = -300;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 29.1;
        }

        public GiantRat(Serial serial)
            : base(serial)
        {
        }

        public override int Meat => 1;
        public override int Hides => 6;
        public override FoodType FavoriteFood => FoodType.Fish | FoodType.Meat | FoodType.FruitsAndVegies | FoodType.Eggs;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Poor);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}