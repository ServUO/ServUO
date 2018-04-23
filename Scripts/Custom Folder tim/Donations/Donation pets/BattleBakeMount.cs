namespace Server.Mobiles
{
    [CorpseName("a battle bake kitsune corpse")]
    public class BattleBakeMount : BaseMount
    {

        public override bool InitialInnocent { get { return true; } }
        public override bool DeleteOnRelease { get { return true; } }
        
        [Constructable]
        public BattleBakeMount()
            : this("A Battle Bake Kitsune")

       
        {
        }

        [Constructable]
        public BattleBakeMount(string name)
            : base(name, 0xF6, 0x38FB, AIType.AI_Animal, FightMode.Good, 10, 1, 0.2, 0.4)
        {
            //BaseSoundID = 0xAD;

            SetStr(600,602);
            SetDex(610,620);
            SetInt(600, 650);

            SetHits(35, 50);
            SetMana(300);

            SetDamage(20, 35);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 10, 15);
            SetResistance(ResistanceType.Fire, 5, 10);
            SetResistance(ResistanceType.Cold, 5, 10);
            SetResistance(ResistanceType.Poison, 5, 10);
            SetResistance(ResistanceType.Energy, 5, 10);

            SetSkill(SkillName.MagicResist, 15.1, 20.0);
            SetSkill(SkillName.Tactics, 19.2, 29.0);
            SetSkill(SkillName.Wrestling, 19.2, 29.0);

            Fame = 300;
            Karma = 0;

            Tamable = false;
            //ControlSlots = 1;
            MinTameSkill = 120.0;
        }

        public override int Meat { get { return 4; } }
        //public override int Hides { get { return 12; } }
        public override FoodType FavoriteFood { get { return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

        public BattleBakeMount(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}