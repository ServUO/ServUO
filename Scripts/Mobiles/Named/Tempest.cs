using System;

namespace Server.Mobiles
{
    [CorpseName("the remains of Tempest")]
    public class Tempest : BaseCreature
    {
        [Constructable]
        public Tempest()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Tempest";
            this.Body = 13;
            this.Hue = 1175;
            this.BaseSoundID = 263;

            this.SetStr(116, 135);
            this.SetDex(166, 185);
            this.SetInt(101, 125);

            this.SetHits(602);

            this.SetDamage(18, 20);  // Erica's

            this.SetDamageType(ResistanceType.Energy, 80);
            this.SetDamageType(ResistanceType.Cold, 20);

            this.SetResistance(ResistanceType.Physical, 46);
            this.SetResistance(ResistanceType.Fire, 39);
            this.SetResistance(ResistanceType.Cold, 33);
            this.SetResistance(ResistanceType.Poison, 36);
            this.SetResistance(ResistanceType.Energy, 58);

            this.SetSkill(SkillName.EvalInt, 99.6);
            this.SetSkill(SkillName.Magery, 101.0);
            this.SetSkill(SkillName.MagicResist, 104.6);
            this.SetSkill(SkillName.Tactics, 111.8);
            this.SetSkill(SkillName.Wrestling, 116.0);

            this.Fame = 4500;
            this.Karma = -4500;

            this.VirtualArmor = 40;
            this.ControlSlots = 2;
        }

        public override bool GivesMLMinorArtifact
        {
            get { return true; }
        }

        public Tempest(Serial serial)
            : base(serial)
        {
        }

        public override double DispelDifficulty
        {
            get
            {
                return 117.5;
            }
        }
        public override double DispelFocus
        {
            get
            {
                return 45.0;
            }
        }
        public override bool BleedImmune
        {
            get
            {
                return true;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 2;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
            this.AddLoot(LootPack.Meager);
            this.AddLoot(LootPack.LowScrolls);
            this.AddLoot(LootPack.MedScrolls);
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

            if (this.BaseSoundID == 263)
                this.BaseSoundID = 655;
        }
    }
}