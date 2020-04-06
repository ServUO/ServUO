using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(Shortblade))]
    [FlipableAttribute(0x2D2F, 0x2D23)]
    public class WarCleaver : BaseKnife
    {
        [Constructable]
        public WarCleaver()
            : base(0x2D2F)
        {
            Weight = 10.0;
            Layer = Layer.TwoHanded;
        }

        public WarCleaver(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.Disarm;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.Bladeweave;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 15;
            }
        }
        public override int MinDamage
        {
            get
            {
                return 10;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 13;
            }
        }
        public override float Speed
        {
            get
            {
                return 2.25f;
            }
        }
        
        public override int DefHitSound
        {
            get
            {
                return 0x23B;
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
                return 30;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 60;
            }
        }
        public override SkillName DefSkill
        {
            get
            {
                return SkillName.Fencing;
            }
        }
        public override WeaponType DefType
        {
            get
            {
                return WeaponType.Piercing;
            }
        }
        public override WeaponAnimation DefAnimation
        {
            get
            {
                return WeaponAnimation.Pierce1H;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();           
        }
    }
}
