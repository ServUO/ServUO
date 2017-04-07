using System;

namespace Server.Mobiles
{
    [CorpseName("an ophidian corpse")]
    [TypeAlias("Server.Mobiles.OphidianJusticar", "Server.Mobiles.OphidianZealot")]
    public class OphidianArchmage : BaseCreature
    {
        private static readonly string[] m_Names = new string[]
        {
            "an ophidian justicar",
            "an ophidian zealot"
        };
        [Constructable]
        public OphidianArchmage()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = m_Names[Utility.Random(m_Names.Length)];
            this.Body = 85;
            this.BaseSoundID = 639;

            this.SetStr(281, 305);
            this.SetDex(191, 215);
            this.SetInt(226, 250);

            this.SetHits(169, 183);
            this.SetStam(36, 45);

            this.SetDamage(5, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 40, 45);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 25, 35);
            this.SetResistance(ResistanceType.Poison, 35, 40);
            this.SetResistance(ResistanceType.Energy, 25, 35);

            this.SetSkill(SkillName.EvalInt, 95.1, 100.0);
            this.SetSkill(SkillName.Magery, 95.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 75.0, 97.5);
            this.SetSkill(SkillName.Tactics, 65.0, 87.5);
            this.SetSkill(SkillName.Wrestling, 20.2, 60.0);

            this.Fame = 11500;
            this.Karma = -11500;

            this.VirtualArmor = 44;

            this.PackReg(5, 15);
            this.PackNecroReg(5, 15);
        }

        public OphidianArchmage(Serial serial)
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
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.MedScrolls, 2);
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
