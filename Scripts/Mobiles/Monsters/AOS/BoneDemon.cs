using System;

namespace Server.Mobiles
{
    [CorpseName("a bone demon corpse")]
    public class BoneDemon : BaseCreature
    {
        [Constructable]
        public BoneDemon()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a bone demon";
            this.Body = 308;
            this.BaseSoundID = 0x48D;

            this.SetStr(1000);
            this.SetDex(151, 175);
            this.SetInt(171, 220);

            this.SetHits(3600);

            this.SetDamage(34, 36);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Cold, 50);

            this.SetResistance(ResistanceType.Physical, 75);
            this.SetResistance(ResistanceType.Fire, 60);
            this.SetResistance(ResistanceType.Cold, 90);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 60);

            this.SetSkill(SkillName.DetectHidden, 80.0);
            this.SetSkill(SkillName.EvalInt, 77.6, 87.5);
            this.SetSkill(SkillName.Magery, 77.6, 87.5);
            this.SetSkill(SkillName.Meditation, 100.0);
            this.SetSkill(SkillName.MagicResist, 50.1, 75.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 100.0);

            this.Fame = 20000;
            this.Karma = -20000;

            this.VirtualArmor = 44;
        }

        public BoneDemon(Serial serial)
            : base(serial)
        {
        }

        public override bool BardImmune
        {
            get
            {
                return !Core.SE;
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return Core.SE;
            }
        }
        public override bool AreaPeaceImmune
        {
            get
            {
                return Core.SE;
            }
        }
        public override Poison PoisonImmune
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
                return 1;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 8);
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