using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an imp corpse")]
    public class Imp : BaseCreature
    {
        [Constructable]
        public Imp()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an imp";
            Body = 74;
            BaseSoundID = 422;

            SetStr(91, 115);
            SetDex(61, 80);
            SetInt(86, 105);

            SetHits(55, 70);

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
            SetSkill(SkillName.Necromancy, 20);
            SetSkill(SkillName.SpiritSpeak, 20);

            Fame = 2500;
            Karma = -2500;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 83.1;
        }

        public Imp(Serial serial)
            : base(serial)
        {
        }

        public override int Meat => 1;
        public override int Hides => 6;
        public override HideType HideType => HideType.Spined;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override PackInstinct PackInstinct => PackInstinct.Daemon;
        public override bool CanFly => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.MedScrolls, 2);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            switch (Utility.Random(13))
            {
                case 0: c.DropItem(new BloodOathScroll()); break;
                case 1: c.DropItem(new CorpseSkinScroll()); break;
                case 2: c.DropItem(new CurseWeaponScroll()); break;
                case 3: c.DropItem(new EvilOmenScroll()); break;
                case 4: c.DropItem(new HorrificBeastScroll()); break;
                case 5: c.DropItem(new LichFormScroll()); break;
                case 6: c.DropItem(new MindRotScroll()); break;
                case 7: c.DropItem(new PainSpikeScroll()); break;
                case 8: c.DropItem(new PoisonStrikeScroll()); break;
                case 9: c.DropItem(new StrangleScroll()); break;
                case 10: c.DropItem(new SummonFamiliarScroll()); break;
                case 11: c.DropItem(new WitherScroll()); break;
                case 12: c.DropItem(new WraithFormScroll()); break;
            }
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
