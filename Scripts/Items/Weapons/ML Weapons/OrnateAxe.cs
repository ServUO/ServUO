using System;

namespace Server.Items
{
    [FlipableAttribute(0x2D28, 0x2D34)]
    public class OrnateAxe : BaseAxe
    {
        [Constructable]
        public OrnateAxe()
            : base(0x2D28)
        {
            this.Weight = 12.0;
            this.Layer = Layer.TwoHanded;
        }

        public OrnateAxe(Serial serial)
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
                return WeaponAbility.CrushingBlow;
            }
        }
        public override int AosStrengthReq
        {
            get
            {
                return 45;
            }
        }
        public override int AosMinDamage
        {
            get
            {
                return 17;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 20;
            }
        }
        public override int AosSpeed
        {
            get
            {
                return 26;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 3.75f;
            }
        }
        public override int OldStrengthReq
        {
            get
            {
                return 45;
            }
        }
        public override int OldMinDamage
        {
            get
            {
                return 18;
            }
        }
        public override int OldMaxDamage
        {
            get
            {
                return 20;
            }
        }
        public override int OldSpeed
        {
            get
            {
                return 26;
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