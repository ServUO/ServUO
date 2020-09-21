using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a water elemental corpse")]
    public class WaterElemental : BaseCreature
    {
        private bool m_HasDecanter = true;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasDecanter { get { return m_HasDecanter; } set { m_HasDecanter = value; } }

        [Constructable]
        public WaterElemental()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a water elemental";
            Body = 16;
            BaseSoundID = 278;

            SetStr(126, 155);
            SetDex(66, 85);
            SetInt(101, 125);

            SetHits(76, 93);

            SetDamage(7, 9);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 10, 25);
            SetResistance(ResistanceType.Cold, 10, 25);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 5, 10);

            SetSkill(SkillName.EvalInt, 60.1, 75.0);
            SetSkill(SkillName.Magery, 60.1, 75.0);
            SetSkill(SkillName.MagicResist, 100.1, 115.0);
            SetSkill(SkillName.Tactics, 50.1, 70.0);
            SetSkill(SkillName.Wrestling, 50.1, 70.0);

            Fame = 4500;
            Karma = -4500;

            ControlSlots = 3;
            CanSwim = true;
        }

        public WaterElemental(Serial serial)
            : base(serial)
        {
        }

        public override double DispelDifficulty => 117.5;
        public override double DispelFocus => 45.0;
        public override bool BleedImmune => true;
        public override int TreasureMapLevel => 2;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.Potions);
            AddLoot(LootPack.LootItem<BlackPearl>(3, true));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(m_HasDecanter);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    break;
                case 1:
                    m_HasDecanter = reader.ReadBool();
                    break;
            }
        }
    }
}
