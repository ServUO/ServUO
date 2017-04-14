using System;

namespace Server.Mobiles
{
    [CorpseName("a terathan warrior corpse")]
    public class TerathanWarrior : BaseCreature
    {
        [Constructable]
        public TerathanWarrior()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a terathan warrior";
            this.Body = 70;
            this.BaseSoundID = 589;

            this.SetStr(166, 215);
            this.SetDex(96, 145);
            this.SetInt(41, 65);

            this.SetHits(100, 129);
            this.SetMana(0);

            this.SetDamage(7, 17);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 30, 35);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 25, 35);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 25, 35);

            this.SetSkill(SkillName.Poisoning, 60.1, 80.0);
            this.SetSkill(SkillName.MagicResist, 60.1, 75.0);
            this.SetSkill(SkillName.Tactics, 80.1, 100.0);
            this.SetSkill(SkillName.Wrestling, 80.1, 90.0);

            this.Fame = 4000;
            this.Karma = -4000;

            this.VirtualArmor = 30;

            if (Core.ML && Utility.RandomDouble() < .33)
                this.PackItem(Engines.Plants.Seed.RandomPeculiarSeed(3));
        }

        public TerathanWarrior(Serial serial)
            : base(serial)
        {
        }

        public override int TreasureMapLevel
        {
            get
            {
                return 1;
            }
        }
        public override int Meat
        {
            get
            {
                return 4;
            }
        }

        public override TribeType Tribe { get { return TribeType.Terathan; } }

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
