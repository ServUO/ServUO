using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an ophidian corpse")]
    [TypeAlias("Server.Mobiles.OphidianAvenger")]
    public class OphidianKnight : BaseCreature
    {
        private static readonly string[] m_Names = new string[]
        {
            "an ophidian knight-errant",
            "an ophidian avenger"
        };
        [Constructable]
        public OphidianKnight()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = m_Names[Utility.Random(m_Names.Length)];
            this.Body = 86;
            this.BaseSoundID = 634;

            this.SetStr(417, 595);
            this.SetDex(166, 175);
            this.SetInt(46, 70);

            this.SetHits(266, 342);
            this.SetMana(0);

            this.SetDamage(16, 19);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 35, 40);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 35, 45);
            this.SetResistance(ResistanceType.Poison, 90, 100);
            this.SetResistance(ResistanceType.Energy, 35, 45);

            this.SetSkill(SkillName.Poisoning, 60.1, 80.0);
            this.SetSkill(SkillName.MagicResist, 65.1, 80.0);
            this.SetSkill(SkillName.Tactics, 90.1, 100.0);
            this.SetSkill(SkillName.Wrestling, 90.1, 100.0);

            this.Fame = 10000;
            this.Karma = -10000;

            this.VirtualArmor = 40;

            this.PackItem(new LesserPoisonPotion());
        }

        public OphidianKnight(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 2;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 3;
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
            this.AddLoot(LootPack.Rich, 2);
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
