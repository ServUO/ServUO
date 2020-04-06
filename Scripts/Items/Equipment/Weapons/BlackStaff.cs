using System;

namespace Server.Items
{
    [FlipableAttribute(0xDF1, 0xDF0)]
    public class BlackStaff : BaseStaff
    {
        [Constructable]
        public BlackStaff()
            : base(0xDF0)
        {
            Weight = 6.0;
        }

        public BlackStaff(Serial serial)
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
                return WeaponAbility.ParalyzingBlow;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 35;
            }
        }
        public override int MinDamage
        {
            get
            {
                return 13;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 16;
            }
        }
        public override float Speed
        {
            get
            {
                return 2.75f;
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
