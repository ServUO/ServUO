using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an ophidian corpse")]
    public class OphidianMage : BaseCreature
    {
        private static readonly string[] m_Names = new string[]
        {
            "an ophidian apprentice mage",
            "an ophidian shaman"
        };
        [Constructable]
        public OphidianMage()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = m_Names[Utility.Random(m_Names.Length)];
            Body = 85;
            BaseSoundID = 639;

            SetStr(181, 205);
            SetDex(191, 215);
            SetInt(96, 120);

            SetHits(109, 123);

            SetDamage(5, 10);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 25, 35);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 35, 45);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.EvalInt, 85.1, 100.0);
            SetSkill(SkillName.Magery, 85.1, 100.0);
            SetSkill(SkillName.MagicResist, 75.0, 97.5);
            SetSkill(SkillName.Tactics, 65.0, 87.5);
            SetSkill(SkillName.Wrestling, 20.2, 60.0);

            Fame = 4000;
            Karma = -4000;
        }

        public OphidianMage(Serial serial)
            : base(serial)
        {
        }

        public override int Meat => 1;
        public override int TreasureMapLevel => 2;

        public override TribeType Tribe => TribeType.Ophidian;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.LowScrolls);
            AddLoot(LootPack.MedScrolls);
            AddLoot(LootPack.Potions);
            AddLoot(LootPack.MageryRegs, 10);
            AddLoot(LootPack.LootItem<PainSpikeScroll>(16.7));
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
