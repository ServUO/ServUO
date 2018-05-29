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
            Name = "a fleshrenderer";
            Body = 315;

            SetStr(401, 460);
            SetDex(201, 210);
            SetInt(221, 260);

            SetHits(4500);

            SetDamage(16, 20);

            SetDamageType(ResistanceType.Physical, 80);
            SetDamageType(ResistanceType.Poison, 20);

            SetResistance(ResistanceType.Physical, 80, 90);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.Wrestling, 90.1, 100.0);
            SetSkill(SkillName.Tactics, 100.0);
            SetSkill(SkillName.MagicResist, 155.1, 160.0);
            SetSkill(SkillName.DetectHidden, 100.0);
            SetSkill(SkillName.Necromancy, 105.0, 110.0);
            SetSkill(SkillName.Focus, 5.0, 15.0);

            Fame = 23000;
            Karma = -23000;

            VirtualArmor = 24;

            SetWeaponAbility(WeaponAbility.Dismount);
            SetWeaponAbility(WeaponAbility.ParalyzingBlow);
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

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
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

            if (BaseSoundID == 660)
                BaseSoundID = -1;
        }
    }
}
