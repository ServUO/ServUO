using System;

namespace Server.Items
{
    [FlipableAttribute(0x27AE, 0x27F9)]
    public class Nunchaku : BaseBashing
    {
        [Constructable]
        public Nunchaku()
            : base(0x27AE)
        {
            this.Weight = 5.0;
        }

        public Nunchaku(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.Block;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.DoubleStrike;
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
                return 12;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 15;
            }
        }
        public override int AosSpeed
        {
            get
            {
                return 47;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 2.50f;
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
                return 11;
            }
        }
        public override int OldMaxDamage
        {
            get
            {
                return 13;
            }
        }
        public override int OldSpeed
        {
            get
            {
                return 47;
            }
        }
        public override int DefHitSound
        {
            get
            {
                return 0x535;
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
                return 40;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 55;
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