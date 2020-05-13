using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a giant spider corpse")]
    public class GiantSpider : BaseCreature
    {
        [Constructable]
        public GiantSpider()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a giant spider";
            Body = 28;
            BaseSoundID = 0x388;

            SetStr(76, 100);
            SetDex(76, 95);
            SetInt(36, 60);

            SetHits(46, 60);
            SetMana(0);

            SetDamage(5, 13);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 20);
            SetResistance(ResistanceType.Poison, 25, 35);

            SetSkill(SkillName.Poisoning, 60.1, 80.0);
            SetSkill(SkillName.MagicResist, 25.1, 40.0);
            SetSkill(SkillName.Tactics, 35.1, 50.0);
            SetSkill(SkillName.Wrestling, 50.1, 65.0);

            Fame = 600;
            Karma = -600;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 59.1;
        }

        public GiantSpider(Serial serial)
            : base(serial)
        {
        }

        public override FoodType FavoriteFood => FoodType.Meat;
        public override PackInstinct PackInstinct => PackInstinct.Arachnid;
        public override Poison PoisonImmune => Poison.Regular;
        public override Poison HitPoison => Poison.Regular;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Poor);
            AddLoot(LootPack.LootItem<SpidersSilk>(5, true));
        }

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
