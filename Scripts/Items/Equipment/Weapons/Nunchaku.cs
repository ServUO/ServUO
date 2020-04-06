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
            Weight = 5.0;
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
                return 12;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 15;
            }
        }
        public override float Speed
        {
            get
            {
                return 2.50f;
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