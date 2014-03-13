using System;

namespace Server.Items
{
    [FlipableAttribute(0x26BB, 0x26C5)]
    public class BoneHarvester : BaseSword
    {
        [Constructable]
        public BoneHarvester()
            : base(0x26BB)
        {
            this.Weight = 3.0;
        }

        public BoneHarvester(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.ParalyzingBlow;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.MortalStrike;
            }
        }
        public override int AosStrengthReq
        {
            get
            {
                return 25;
            }
        }
        public override int AosMinDamage
        {
            get
            {
                return 12;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 16;
            }
        }
        public override int AosSpeed
        {
            get
            {
                return 36;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 3.00f;
            }
        }
        public override int OldStrengthReq
        {
            get
            {
                return 25;
            }
        }
        public override int OldMinDamage
        {
            get
            {
                return 13;
            }
        }
        public override int OldMaxDamage
        {
            get
            {
                return 15;
            }
        }
        public override int OldSpeed
        {
            get
            {
                return 36;
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
                return 0x23A;
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
                return 70;
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