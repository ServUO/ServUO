using System;

namespace Server.Items
{
    [FlipableAttribute(0x26CE, 0x26CF)]
    public class PaladinSword : BaseSword
    {
        [Constructable]
        public PaladinSword()
            : base(0x26CE)
        {
            Weight = 6.0;
        }

        public PaladinSword(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.WhirlwindAttack;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.Disarm;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 85;
            }
        }
        public override int MinDamage
        {
            get
            {
                return 20;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 24;
            }
        }
        public override float Speed
        {
            get
            {
                return 5.0f;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 36;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 48;
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