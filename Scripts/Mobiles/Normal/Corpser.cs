using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a corpser corpse")]
    public class Corpser : BaseCreature
    {
        [Constructable]
        public Corpser()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a corpser";
            Body = 8;
            BaseSoundID = 684;

            SetStr(156, 180);
            SetDex(26, 45);
            SetInt(26, 40);

            SetHits(94, 108);
            SetMana(0);

            SetDamage(10, 23);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Poison, 40);

            SetResistance(ResistanceType.Physical, 15, 20);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 10, 20);
            SetResistance(ResistanceType.Poison, 20, 30);

            SetSkill(SkillName.MagicResist, 15.1, 20.0);
            SetSkill(SkillName.Tactics, 45.1, 60.0);
            SetSkill(SkillName.Wrestling, 45.1, 60.0);

            Fame = 1000;
            Karma = -1000;
        }

        public Corpser(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune => Poison.Lesser;
        public override bool DisallowAllMoves => true;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.LootItem<MandrakeRoot>(3, true));
            AddLoot(LootPack.LootItems(new[] { new LootPackItem(typeof(Board), 1), new LootPackItem(typeof(Log), 3) }, 10));
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
