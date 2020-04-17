namespace Server.Mobiles
{
    [CorpseName("an alligator corpse")]
    public class Alligator : BaseCreature
    {
        [Constructable]
        public Alligator()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an alligator";
            Body = 0xCA;
            BaseSoundID = 660;

            SetStr(76, 100);
            SetDex(6, 25);
            SetInt(11, 20);

            SetHits(46, 60);
            SetStam(46, 65);
            SetMana(0);

            SetDamage(5, 15);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 5, 10);
            SetResistance(ResistanceType.Poison, 5, 10);

            SetSkill(SkillName.MagicResist, 25.1, 40.0);
            SetSkill(SkillName.Tactics, 40.1, 60.0);
            SetSkill(SkillName.Wrestling, 40.1, 60.0);

            Fame = 600;
            Karma = -600;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 47.1;
        }

        public Alligator(Serial serial)
            : base(serial)
        {
        }

        public override int Meat => 1;
        public override int Hides => 12;
        public override HideType HideType => HideType.Spined;
        public override FoodType FavoriteFood => FoodType.Meat | FoodType.Fish;
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