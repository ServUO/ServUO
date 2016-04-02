using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an impaler corpse")]
    public class Impaler : BaseCreature
    {
        [Constructable]
        public Impaler()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = NameList.RandomName("impaler");
            this.Body = 306;
            this.BaseSoundID = 0x2A7;

            this.SetStr(190);
            this.SetDex(45);
            this.SetInt(190);

            this.SetHits(5000);

            this.SetDamage(31, 35);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 90);
            this.SetResistance(ResistanceType.Fire, 60);
            this.SetResistance(ResistanceType.Cold, 75);
            this.SetResistance(ResistanceType.Poison, 60);
            this.SetResistance(ResistanceType.Energy, 100);

            this.SetSkill(SkillName.DetectHidden, 80.0);
            this.SetSkill(SkillName.Meditation, 120.0);
            this.SetSkill(SkillName.Poisoning, 160.0);
            this.SetSkill(SkillName.MagicResist, 100.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 80.0);

            this.Fame = 24000;
            this.Karma = -24000;

            this.VirtualArmor = 49;
        }

        public Impaler(Serial serial)
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
        public override bool AutoDispel
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
        public override Poison HitPoison
        {
            get
            {
                return (0.8 >= Utility.RandomDouble() ? Poison.Greater : Poison.Deadly);
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 1;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return Utility.RandomBool() ? WeaponAbility.MortalStrike : WeaponAbility.BleedAttack;
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

            if (this.BaseSoundID == 1200)
                this.BaseSoundID = 0x2A7;
        }
    }
}