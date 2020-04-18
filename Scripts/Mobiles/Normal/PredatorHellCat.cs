namespace Server.Mobiles
{
    [CorpseName("a hell cat corpse")]
    public class PredatorHellCat : BaseCreature
    {
        [Constructable]
        public PredatorHellCat()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a predator hellcat";
            Body = 127;
            BaseSoundID = 0xBA;

            SetStr(161, 185);
            SetDex(96, 115);
            SetInt(76, 100);

            SetHits(97, 131);

            SetDamage(5, 17);

            SetDamageType(ResistanceType.Physical, 75);
            SetDamageType(ResistanceType.Fire, 25);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Energy, 5, 15);

            SetSkill(SkillName.MagicResist, 75.1, 90.0);
            SetSkill(SkillName.Tactics, 50.1, 65.0);
            SetSkill(SkillName.Wrestling, 50.1, 65.0);
            SetSkill(SkillName.Necromancy, 20.0);
            SetSkill(SkillName.SpiritSpeak, 20.0);
            SetSkill(SkillName.Wrestling, 50.1, 65.0);
            SetSkill(SkillName.DetectHidden, 41.2);

            Fame = 2500;
            Karma = -2500;

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 90.0;

            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public PredatorHellCat(Serial serial)
            : base(serial)
        {
        }

        public override int Hides => 10;
        public override HideType HideType => HideType.Spined;
        public override FoodType FavoriteFood => FoodType.Meat;
        public override PackInstinct PackInstinct => PackInstinct.Feline;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
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
