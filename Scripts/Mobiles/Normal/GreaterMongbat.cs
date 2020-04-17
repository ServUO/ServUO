namespace Server.Mobiles
{
    [CorpseName("a mongbat corpse")]
    public class GreaterMongbat : BaseCreature
    {
        [Constructable]
        public GreaterMongbat()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a greater mongbat";
            Body = 39;
            BaseSoundID = 422;

            SetStr(56, 80);
            SetDex(61, 80);
            SetInt(26, 50);

            SetHits(34, 48);
            SetStam(61, 80);
            SetMana(26, 50);

            SetDamage(5, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 25);

            SetSkill(SkillName.MagicResist, 15.1, 30.0);
            SetSkill(SkillName.Tactics, 35.1, 50.0);
            SetSkill(SkillName.Wrestling, 20.1, 35.0);

            Fame = 450;
            Karma = -450;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 71.1;
        }

        public GreaterMongbat(Serial serial)
            : base(serial)
        {
        }

        public override int Meat => 1;
        public override int Hides => 6;
        public override FoodType FavoriteFood => FoodType.Meat;
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