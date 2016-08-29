using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("The remains of Flurry")]
    public class Flurry : BaseCreature
    {
        [Constructable]
        public Flurry()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Flurry";
            this.Body = 13;
            this.Hue = 3;
            this.BaseSoundID = 655;

            this.SetStr(149, 195);
            this.SetDex(218, 264);
            this.SetInt(130, 199);

            this.SetHits(474, 477);

            this.SetDamage(10, 15);  // Erica's

            this.SetDamageType(ResistanceType.Energy, 20);
            this.SetDamageType(ResistanceType.Cold, 80);

            this.SetResistance(ResistanceType.Physical, 56, 57);
            this.SetResistance(ResistanceType.Fire, 38, 44);
            this.SetResistance(ResistanceType.Cold, 40, 45);
            this.SetResistance(ResistanceType.Poison, 31, 37);
            this.SetResistance(ResistanceType.Energy, 39, 41);

            this.SetSkill(SkillName.EvalInt, 99.1, 100.2);
            this.SetSkill(SkillName.Magery, 105.1, 108.8);
            this.SetSkill(SkillName.MagicResist, 104.0, 112.8);
            this.SetSkill(SkillName.Tactics, 113.1, 119.8);
            this.SetSkill(SkillName.Wrestling, 105.6, 106.4);

            this.Fame = 4500;
            this.Karma = -4500;

            this.VirtualArmor = 54;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public Flurry(Serial serial)
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
            this.AddLoot(LootPack.Rich, 10);
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