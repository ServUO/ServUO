using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an ophidian corpse")]
    [TypeAlias("Server.Mobiles.OphidianShaman")]
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
            this.Name = m_Names[Utility.Random(m_Names.Length)];
            this.Body = 85;
            this.BaseSoundID = 639;

            this.SetStr(181, 205);
            this.SetDex(191, 215);
            this.SetInt(96, 120);

            this.SetHits(109, 123);

            this.SetDamage(5, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 25, 35);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 35, 45);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 35, 45);

            this.SetSkill(SkillName.EvalInt, 85.1, 100.0);
            this.SetSkill(SkillName.Magery, 85.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 75.0, 97.5);
            this.SetSkill(SkillName.Tactics, 65.0, 87.5);
            this.SetSkill(SkillName.Wrestling, 20.2, 60.0);

            this.Fame = 4000;
            this.Karma = -4000;

            this.VirtualArmor = 30;

            this.PackReg(10);

			switch (Utility.Random(6))
            {
                case 0: PackItem(new PainSpikeScroll()); break;
			}

        }

        public OphidianMage(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 2;
            }
        }

        public override TribeType Tribe { get { return TribeType.Ophidian; } }

        public override OppositionGroup OppositionGroup
        {
            get
            {
                return OppositionGroup.TerathansAndOphidians;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
            this.AddLoot(LootPack.LowScrolls);
            this.AddLoot(LootPack.MedScrolls);
            this.AddLoot(LootPack.Potions);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
