using System;

namespace Server.Mobiles
{
    [CorpseName("an ophidian corpse")]
    public class OphidianMatriarch : BaseCreature
    {
        [Constructable]
        public OphidianMatriarch()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an ophidian matriarch";
            this.Body = 87;
            this.BaseSoundID = 644;

            this.SetStr(416, 505);
            this.SetDex(96, 115);
            this.SetInt(366, 455);

            this.SetHits(250, 303);

            this.SetDamage(11, 13);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 35, 45);
            this.SetResistance(ResistanceType.Poison, 40, 50);
            this.SetResistance(ResistanceType.Energy, 35, 45);

            this.SetSkill(SkillName.EvalInt, 90.1, 100.0);
            this.SetSkill(SkillName.Magery, 90.1, 100.0);
            this.SetSkill(SkillName.Meditation, 5.4, 25.0);
            this.SetSkill(SkillName.MagicResist, 90.1, 100.0);
            this.SetSkill(SkillName.Tactics, 50.1, 70.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 80.0);

            this.Fame = 16000;
            this.Karma = -16000;

            this.VirtualArmor = 50;
        }

        public OphidianMatriarch(Serial serial)
            : base(serial)
        {
        }

        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Greater;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 4;
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
            this.AddLoot(LootPack.Average, 2);
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
