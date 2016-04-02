/* Based on AbysmalHorror, still no infos on Abyssal Abomination... Including correct body ID */
using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an abyssal abomination corpse")]
    public class AbyssalAbomination : BaseCreature
    {
        [Constructable]
        public AbyssalAbomination()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an Abyssal abomination";
            this.Body = 742;
            this.Hue = 769;
            this.BaseSoundID = 0x451;

            this.SetStr(401, 420);
            this.SetDex(81, 90);
            this.SetInt(401, 420);

            this.SetHits(600, 750);

            this.SetDamage(13, 17);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Poison, 50);

            this.SetResistance(ResistanceType.Physical, 30, 35);
            this.SetResistance(ResistanceType.Fire, 100);
            this.SetResistance(ResistanceType.Cold, 50, 55);
            this.SetResistance(ResistanceType.Poison, 60, 65);
            this.SetResistance(ResistanceType.Energy, 77, 80);

            this.SetSkill(SkillName.EvalInt, 200.0);
            this.SetSkill(SkillName.Magery, 112.6, 117.5);
            this.SetSkill(SkillName.Meditation, 200.0);
            this.SetSkill(SkillName.MagicResist, 117.6, 120.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 84.1, 88.0);

            this.Fame = 26000;
            this.Karma = -26000;

            this.VirtualArmor = 54;
        }

        public AbyssalAbomination(Serial serial)
            : base(serial)
        {
        }

        public override bool IgnoreYoungProtection
        {
            get
            {
                return Core.ML;
            }
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
        public override WeaponAbility GetWeaponAbility()
        {
            return Utility.RandomBool() ? WeaponAbility.MortalStrike : WeaponAbility.WhirlwindAttack;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 2);
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

            if (this.BaseSoundID == 357)
                this.BaseSoundID = 0x451;
        }
    }
}