using System;

namespace Server.Mobiles
{
    [CorpseName("an ophidian corpse")]
    public class OphidianWarrior : BaseCreature
    {
        private static readonly string[] m_Names = new string[]
        {
            "an ophidian warrior",
            "an ophidian enforcer"
        };
        [Constructable]
        public OphidianWarrior()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = m_Names[Utility.Random(m_Names.Length)];
            this.Body = 86;
            this.BaseSoundID = 634;

            this.SetStr(150, 320);
            this.SetDex(94, 190);
            this.SetInt(64, 160);

            this.SetHits(128, 155);
            this.SetMana(0);

            this.SetDamage(5, 11);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 35, 40);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 25, 35);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 25, 35);

            this.SetSkill(SkillName.MagicResist, 70.1, 85.0);
            this.SetSkill(SkillName.Swords, 60.1, 85.0);
            this.SetSkill(SkillName.Tactics, 75.1, 90.0);

            this.Fame = 4500;
            this.Karma = -4500;

            this.VirtualArmor = 36;
        }

        public OphidianWarrior(Serial serial)
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
            this.AddLoot(LootPack.Meager);
            this.AddLoot(LootPack.Average);
            this.AddLoot(LootPack.Gems);
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
