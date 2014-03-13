using System;

namespace Server.Items
{
    [FlipableAttribute(0x2D32, 0x2D26)]
    public class RuneBlade : BaseSword
    {
        [Constructable]
        public RuneBlade()
            : base(0x2D32)
        {
            this.Weight = 7.0;
            this.Layer = Layer.TwoHanded;
        }

        public RuneBlade(Serial serial)
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
                return 30;
            }
        }
        public override int AosMinDamage
        {
            get
            {
                return 14;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 17;
            }
        }
        public override int AosSpeed
        {
            get
            {
                return 35;
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
                return 30;
            }
        }
        public override int OldMinDamage
        {
            get
            {
                return 15;
            }
        }
        public override int OldMaxDamage
        {
            get
            {
                return 17;
            }
        }
        public override int OldSpeed
        {
            get
            {
                return 35;
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