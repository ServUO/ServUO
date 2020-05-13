using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a gremlin corpse")]
    public class Gremlin : BaseCreature
    {
        [Constructable]
        public Gremlin()
            : base(AIType.AI_Archer, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a gremlin";
            Body = 724;

            SetStr(106);
            SetDex(130);
            SetInt(36);

            SetHits(70);

            SetDamage(5, 7);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 26);
            SetResistance(ResistanceType.Fire, 36);
            SetResistance(ResistanceType.Cold, 22);
            SetResistance(ResistanceType.Poison, 17);
            SetResistance(ResistanceType.Energy, 30);

            SetSkill(SkillName.Anatomy, 78.5);
            SetSkill(SkillName.MagicResist, 82.5);
            SetSkill(SkillName.Tactics, 65.3);

            AddItem(new Bow());
        }

        public Gremlin(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.LootItem<Arrow>(60, 80));
            AddLoot(LootPack.LootItem<Apple>(5));
            AddLoot(LootPack.LootItem<LuckyCoin>(1.0));
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
