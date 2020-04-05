using System;
using Server.Engines.Harvest;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(DiscMace))]
    [FlipableAttribute(0x13B0, 0x13AF)]
    public class WarAxe : BaseAxe
    {
        [Constructable]
        public WarAxe()
            : base(0x13B0)
        {
            Weight = 8.0;
        }

        public WarAxe(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.ArmorIgnore;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.BleedAttack;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 35;
            }
        }
        public override int MinDamage
        {
            get
            {
                return 12;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 16;
            }
        }
        public override float Speed
        {
            get
            {
                return 3.00f;
            }
        }
        
        public override int DefHitSound
        {
            get
            {
                return 0x233;
            }
        }
        public override int DefMissSound
        {
            get
            {
                return 0x239;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 31;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 80;
            }
        }
        public override SkillName DefSkill
        {
            get
            {
                return SkillName.Macing;
            }
        }
        public override WeaponType DefType
        {
            get
            {
                return WeaponType.Bashing;
            }
        }
        public override WeaponAnimation DefAnimation
        {
            get
            {
                return WeaponAnimation.Bash1H;
            }
        }
        public override HarvestSystem HarvestSystem
        {
            get
            {
                return null;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
