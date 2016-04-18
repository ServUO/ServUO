using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a fleshrenderer corpse")]
    public class FleshRenderer : BaseCreature
    {
        [Constructable]
        public FleshRenderer()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a fleshrenderer";
            this.Body = 315;

            this.SetStr(401, 460);
            this.SetDex(201, 210);
            this.SetInt(221, 260);

            this.SetHits(4500);

            this.SetDamage(16, 20);

            this.SetDamageType(ResistanceType.Physical, 80);
            this.SetDamageType(ResistanceType.Poison, 20);

            this.SetResistance(ResistanceType.Physical, 80, 90);
            this.SetResistance(ResistanceType.Fire, 50, 60);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 70, 80);

            this.SetSkill(SkillName.DetectHidden, 80.0);
            this.SetSkill(SkillName.MagicResist, 155.1, 160.0);
            this.SetSkill(SkillName.Meditation, 100.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 90.1, 100.0);

            this.Fame = 23000;
            this.Karma = -23000;

            this.VirtualArmor = 24;
        }

        public FleshRenderer(Serial serial)
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
        public override int TreasureMapLevel
        {
            get
            {
                return 1;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return Utility.RandomBool() ? WeaponAbility.Dismount : WeaponAbility.ParalyzingBlow;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.UltraRich, 2);
        }

        

        public override int GetAttackSound()
        {
            return 0x34C;
        }

        public override int GetHurtSound()
        {
            return 0x354;
        }

        public override int GetAngerSound()
        {
            return 0x34C;
        }

        public override int GetIdleSound()
        {
            return 0x34C;
        }

        public override int GetDeathSound()
        {
            return 0x354;
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

            if (this.BaseSoundID == 660)
                this.BaseSoundID = -1;
        }
    }
}