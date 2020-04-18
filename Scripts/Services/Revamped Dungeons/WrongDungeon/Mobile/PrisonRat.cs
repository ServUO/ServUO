namespace Server.Mobiles
{
    [CorpseName("a rat corpse")]
    public class PrisonRat : BaseCreature
    {
        [Constructable]
        public PrisonRat()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a prison rat";
            Body = 0xEE;
            Hue = 443;
            BaseSoundID = 0xCC;

            SetStr(9);
            SetDex(35);
            SetInt(7, 10);

            SetHits(50);
            SetStam(25);
            SetMana(7, 10);

            SetDamage(5, 8);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 5, 10);
            SetResistance(ResistanceType.Poison, 15, 25);
            SetResistance(ResistanceType.Energy, 5, 10);

            SetSkill(SkillName.MagicResist, 3.7, 20.7);
            SetSkill(SkillName.Tactics, 6.7, 17.0);
            SetSkill(SkillName.Wrestling, 9.1, 19.5);

            Fame = 150;
            Karma = -150;
        }

        public PrisonRat(Serial serial)
            : base(serial)
        {
        }

        public override int Meat => 1;

        public override FoodType FavoriteFood => FoodType.Fish;

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