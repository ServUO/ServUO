using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an anzuanord corpse")]
    public class Anzuanord : BaseCreature
    {
        [Constructable]
        public Anzuanord()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an Anzuanord";
            Body = 74;
            Hue = 2071;
            BaseSoundID = 422;

            SetStr(91, 115);
            SetDex(61, 80);
            SetInt(86, 105);

            SetHits(255, 270);

            SetDamage(10, 14);

            SetDamageType(ResistanceType.Physical, 0);
            SetDamageType(ResistanceType.Fire, 50);
            SetDamageType(ResistanceType.Poison, 50);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 20, 30);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.EvalInt, 20.1, 30.0);
            SetSkill(SkillName.Magery, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 30.1, 50.0);
            SetSkill(SkillName.Tactics, 42.1, 50.0);
            SetSkill(SkillName.Wrestling, 40.1, 44.0);

            Fame = 2500;
            Karma = -2500;

            VirtualArmor = 50;

            QLPoints = 10;
        }

        public Anzuanord(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer
        {
            get { return true; }
        }

        public override bool BardImmune
        {
            get { return true; }
        }

        public override int Meat
        {
            get { return 1; }
        }

        public override int Hides
        {
            get { return 7; }
        }

        public override HideType HideType
        {
            get { return HideType.Spined; }
        }

        public override FoodType FavoriteFood
        {
            get { return FoodType.Meat; }
        }

        public override PackInstinct PackInstinct
        {
            get { return PackInstinct.Daemon; }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.MedScrolls, 2);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.5)
                c.DropItem(new VoidOrb());
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