/* Based on Gargoyle, still no infos on Undead Gargoyle... Have to get also the correct body ID */
using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a putrid undead gargoyle corpse")]
    public class PutridUndeadGargoyle : BaseCreature
    {
        [Constructable]
        public PutridUndeadGargoyle()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a putrid undead gargoyle";
            this.Body = 722;
            this.BaseSoundID = 372;
            this.Hue = 1778;

            this.SetStr(525);
            this.SetDex(120, 125);
            this.SetInt(1145);

            this.SetHits(660, 665);

            this.SetDamage(21, 30);

            this.SetDamageType(ResistanceType.Physical, 20);
            this.SetDamageType(ResistanceType.Cold, 40);
            this.SetDamageType(ResistanceType.Energy, 40);

            this.SetResistance(ResistanceType.Physical, 55, 60);
            this.SetResistance(ResistanceType.Fire, 33);
            this.SetResistance(ResistanceType.Cold, 50);
            this.SetResistance(ResistanceType.Poison, 50, 55);
            this.SetResistance(ResistanceType.Energy, 45, 50);

            this.SetSkill(SkillName.EvalInt, 125.0);
            this.SetSkill(SkillName.Magery, 127.0);
            this.SetSkill(SkillName.Mysticism, 100.0);
            this.SetSkill(SkillName.Meditation, 105.0, 108.0);
            this.SetSkill(SkillName.MagicResist, 180.0, 187.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 100.0, 102.0);

            this.Fame = 3500;
            this.Karma = -3500;

            this.VirtualArmor = 32;

            if (0.05 > Utility.RandomDouble())
                this.PackItem(new UndyingFlesh());

            if (0.05 > Utility.RandomDouble())
                this.PackItem(new TatteredAncientScroll());

            if (0.10 > Utility.RandomDouble())
                this.PackItem(new InfusedGlassStave());

            if (0.15 > Utility.RandomDouble())
                this.PackItem(new AncientPotteryFragments());
        }

        public PutridUndeadGargoyle(Serial serial)
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
        public override bool Unprovokable
        {
            get
            {
                return true;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Deadly;
            }
        }
        public override Poison HitPoison
        {
            get
            {
                return Poison.Deadly;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.AosFilthyRich, 5);
            this.AddLoot(LootPack.MedScrolls);
            this.AddLoot(LootPack.Gems, Utility.RandomMinMax(1, 4));
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