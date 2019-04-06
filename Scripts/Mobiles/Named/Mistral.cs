using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a mistral corpse")]
    public class Mistral : BaseCreature
    {
        [Constructable]
        public Mistral()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Mistral";
            this.Body = 13;
            this.Hue = 924;
            this.BaseSoundID = 263;

            this.SetStr(134, 201);
            this.SetDex(226, 238);
            this.SetInt(126, 134);

            this.SetHits(386, 609);

            this.SetDamage(17, 20);  // Erica's

            this.SetDamageType(ResistanceType.Energy, 20);
            this.SetDamageType(ResistanceType.Cold, 40);
            this.SetDamageType(ResistanceType.Physical, 40);

            this.SetResistance(ResistanceType.Physical, 55, 64);
            this.SetResistance(ResistanceType.Fire, 36, 40);
            this.SetResistance(ResistanceType.Cold, 33, 39);
            this.SetResistance(ResistanceType.Poison, 30, 39);
            this.SetResistance(ResistanceType.Energy, 49, 53);

            this.SetSkill(SkillName.EvalInt, 96.2, 97.8);
            this.SetSkill(SkillName.Magery, 100.8, 112.9);
            this.SetSkill(SkillName.MagicResist, 106.2, 111.2);
            this.SetSkill(SkillName.Tactics, 110.2, 117.1);
            this.SetSkill(SkillName.Wrestling, 100.3, 104.0);

            this.Fame = 4500;
            this.Karma = -4500;

            this.VirtualArmor = 40;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public override bool GivesMLMinorArtifact
        {
            get { return true; }
        }

        public Mistral(Serial serial)
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