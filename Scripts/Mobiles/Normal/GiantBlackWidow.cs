using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a giant black widow spider corpse")]
    public class GiantBlackWidow : BaseCreature
    {
        [Constructable]
        public GiantBlackWidow()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a giant black widow";
            Body = 0x9D;
            BaseSoundID = 0x388; // TODO: validate

            SetStr(76, 100);
            SetDex(96, 115);
            SetInt(36, 60);

            SetHits(46, 60);

            SetDamage(5, 17);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 20, 30);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.Anatomy, 30.3, 75.0);
            SetSkill(SkillName.Poisoning, 60.1, 80.0);
            SetSkill(SkillName.MagicResist, 45.1, 60.0);
            SetSkill(SkillName.Tactics, 65.1, 80.0);
            SetSkill(SkillName.Wrestling, 70.1, 85.0);

            Fame = 3500;
            Karma = -3500;
        }

        public GiantBlackWidow(Serial serial)
            : base(serial)
        {
        }

        public override FoodType FavoriteFood => FoodType.Meat;
        public override Poison PoisonImmune => Poison.Deadly;
        public override Poison HitPoison => Poison.Deadly;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.LootItem<SpidersSilk>(5, true));
            AddLoot(LootPack.LootItem<LesserPoisonPotion>(2, true));
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
