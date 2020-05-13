using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a terathan matriarch corpse")]
    public class TerathanMatriarch : BaseCreature
    {
        [Constructable]
        public TerathanMatriarch()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a terathan matriarch";
            Body = 72;
            BaseSoundID = 599;

            SetStr(316, 405);
            SetDex(96, 115);
            SetInt(366, 455);

            SetHits(190, 243);

            SetDamage(11, 14);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 35, 45);
            SetResistance(ResistanceType.Poison, 40, 50);
            SetResistance(ResistanceType.Energy, 35, 45);

            SetSkill(SkillName.EvalInt, 90.1, 100.0);
            SetSkill(SkillName.Magery, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 90.1, 100.0);
            SetSkill(SkillName.Tactics, 50.1, 70.0);
            SetSkill(SkillName.Wrestling, 60.1, 80.0);

            Fame = 10000;
            Karma = -10000;
        }

        public TerathanMatriarch(Serial serial)
            : base(serial)
        {
        }

        public override int TreasureMapLevel => 4;

        public override TribeType Tribe => TribeType.Terathan;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Average, 2);
            AddLoot(LootPack.MedScrolls, 2);
            AddLoot(LootPack.Potions);
            AddLoot(LootPack.NecroRegs, 4, 10);
            AddLoot(LootPack.LootItem<SpidersSilk>(100.0, 5, false, true));
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
