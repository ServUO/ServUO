using System;

namespace Server.Items
{
    [FlipableAttribute(0xF50, 0xF4F)]
    public class Crossbow : BaseRanged
    {
        [Constructable]
        public Crossbow()
            : base(0xF50)
        {
            Weight = 7.0;
            Layer = Layer.TwoHanded;
        }

        public Crossbow(Serial serial)
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
                return WeaponAbility.ConcussionBlow;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.MortalStrike;
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
                return 18;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 22;
            }
        }
        public override float Speed
        {
            get
            {
                return 4.50f;
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