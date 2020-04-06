using System;

namespace Server.Items
{
    [FlipableAttribute(0x27A8, 0x27F3)]
    public class Bokuto : BaseSword
    {
        [Constructable]
        public Bokuto()
            : base(0x27A8)
        {
            Weight = 7.0;
        }

        public Bokuto(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.Feint;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.NerveStrike;
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
                return 12;
            }
        }
        public override float Speed
        {
            get
            {
                return 2.00f;
            }
        }
        
        public override int DefHitSound
        {
            get
            {
                return 0x536;
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
                return 25;
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