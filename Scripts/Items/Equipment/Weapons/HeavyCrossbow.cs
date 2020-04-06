using System;

namespace Server.Items
{
    [FlipableAttribute(0x13FD, 0x13FC)]
    public class HeavyCrossbow : BaseRanged
    {
        [Constructable]
        public HeavyCrossbow()
            : base(0x13FD)
        {
            Weight = 9.0;
            Layer = Layer.TwoHanded;
        }

        public HeavyCrossbow(Serial serial)
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
                return WeaponAbility.MovingShot;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.Dismount;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 80;
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
                return 5.00f;
            }
        }
       
        public override int DefMaxRange
        {
            get
            {
                return 8;
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
                return 100;
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
