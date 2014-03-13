using System;

namespace Server.Items
{
    [FlipableAttribute(0x2D2F, 0x2D23)]
    public class WarCleaver : BaseKnife
    {
        [Constructable]
        public WarCleaver()
            : base(0x2D2F)
        {
            this.Weight = 10.0;
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
        public override int AosStrengthReq
        {
            get
            {
                return 15;
            }
        }
        public override int AosMinDamage
        {
            get
            {
                return 10;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 13;
            }
        }
        public override int AosSpeed
        {
            get
            {
                return 48;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 2.25f;
            }
        }
        public override int OldStrengthReq
        {
            get
            {
                return 15;
            }
        }
        public override int OldMinDamage
        {
            get
            {
                return 9;
            }
        }
        public override int OldMaxDamage
        {
            get
            {
                return 11;
            }
        }
        public override int OldSpeed
        {
            get
            {
                return 48;
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
        }// TODO
        public override int InitMaxHits
        {
            get
            {
                return 60;
            }
        }// TODO
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

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}