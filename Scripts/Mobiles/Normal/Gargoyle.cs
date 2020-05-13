using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a gargoyle corpse")]
    public class Gargoyle : BaseCreature
    {
        [Constructable]
        public Gargoyle()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a gargoyle";
            Body = 4;
            BaseSoundID = 372;

            SetStr(146, 175);
            SetDex(76, 95);
            SetInt(81, 105);

            SetHits(88, 105);

            SetDamage(7, 14);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30, 35);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 5, 10);
            SetResistance(ResistanceType.Poison, 15, 25);

            SetSkill(SkillName.EvalInt, 70.1, 85.0);
            SetSkill(SkillName.Magery, 70.1, 85.0);
            SetSkill(SkillName.MagicResist, 70.1, 85.0);
            SetSkill(SkillName.Tactics, 50.1, 70.0);
            SetSkill(SkillName.Wrestling, 40.1, 80.0);

            Fame = 3500;
            Karma = -3500;
        }

        public Gargoyle(Serial serial)
            : base(serial)
        {
        }

        public override int TreasureMapLevel => 1;
        public override int Meat => 1;
        public override bool CanFly => true;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.MedScrolls);
            AddLoot(LootPack.Gems, Utility.RandomMinMax(1, 4));
            AddLoot(LootPack.LootItem<GargoylesPickaxe>(2.5));
            AddLoot(LootPack.LootItem<PainSpikeScroll>(15.0, true));
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
