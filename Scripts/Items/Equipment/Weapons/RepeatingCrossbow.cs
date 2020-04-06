using System;

namespace Server.Items
{
    [FlipableAttribute(0x26C3, 0x26CD)]
    public class RepeatingCrossbow : BaseRanged
    {
        [Constructable]
        public RepeatingCrossbow()
            : base(0x26C3)
        {
            Weight = 6.0;
        }

        public RepeatingCrossbow(Serial serial)
            : base(serial)
        {
        }

        public override int EffectID
        {
            get
            {
                return 0x1BFE;
            }
        }
        public override Type AmmoType
        {
            get
            {
                return typeof(Bolt);
            }
        }
        public override Item Ammo
        {
            get
            {
                return new Bolt();
            }
        }
        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.DoubleStrike;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.MovingShot;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 30;
            }
        }
        public override int MinDamage
        {
            get
            {
                return 11;
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
                return 2.75f;
            }
        }
        
        public override int DefMaxRange
        {
            get
            {
                return 7;
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
                return 80;
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
