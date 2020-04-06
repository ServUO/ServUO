using System;

namespace Server.Items
{
    [FlipableAttribute(0x27A4, 0x27EF)]
    public class Wakizashi : BaseSword
    {
        [Constructable]
        public Wakizashi()
            : base(0x27A4)
        {
            Weight = 5.0;
            Layer = Layer.OneHanded;
        }

        public Wakizashi(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.FrenziedWhirlwind;
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
                return 20;
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
                return 14;
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
                return 45;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 50;
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