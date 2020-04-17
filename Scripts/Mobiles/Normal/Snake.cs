namespace Server.Mobiles
{
    [CorpseName("a snake corpse")]
    public class Snake : BaseCreature
    {
        [Constructable]
        public Snake()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a snake";
            this.Body = 52;
            this.Hue = Utility.RandomSnakeHue();
            this.BaseSoundID = 0xDB;

            this.SetStr(22, 34);
            this.SetDex(16, 25);
            this.SetInt(6, 10);

            this.SetHits(15, 19);
            this.SetMana(0);

            this.SetDamage(1, 4);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 15, 20);
            this.SetResistance(ResistanceType.Poison, 20, 30);

            this.SetSkill(SkillName.Poisoning, 50.1, 70.0);
            this.SetSkill(SkillName.MagicResist, 15.1, 20.0);
            this.SetSkill(SkillName.Tactics, 19.3, 34.0);
            this.SetSkill(SkillName.Wrestling, 19.3, 34.0);

            this.Fame = 300;
            this.Karma = -300;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 59.1;
        }

        public Snake(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune => Poison.Lesser;
        public override Poison HitPoison => Poison.Lesser;
        public override bool DeathAdderCharmable => true;
        public override int Meat => 1;
        public override FoodType FavoriteFood => FoodType.Eggs;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}