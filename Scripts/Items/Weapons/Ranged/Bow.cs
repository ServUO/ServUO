using System;

namespace Server.Items
{
    [FlipableAttribute(0x13B2, 0x13B1)]
    public class Bow : BaseRanged
    {
        [Constructable]
        public Bow()
            : base(0x13B2)
        {
            this.Weight = 6.0;
            this.Layer = Layer.TwoHanded;
        }

        public Bow(Serial serial)
            : base(serial)
        {
        }

        public override int EffectID
        {
            get
            {
                return 0xF42;
            }
        }
        public override Type AmmoType
        {
            get
            {
                return typeof(Arrow);
            }
        }
        public override Item Ammo
        {
            get
            {
                return new Arrow();
            }
        }
        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.ParalyzingBlow;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.MortalStrike;
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
                return Core.ML ? 17 : 16;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return Core.ML ? 21 : 18;
            }
        }
        public override int AosSpeed
        {
            get
            {
                return 25;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 4.25f;
            }
        }
        public override int OldStrengthReq
        {
            get
            {
                return 20;
            }
        }
        public override int OldMinDamage
        {
            get
            {
                return 9;
            }
        }
        public override int OldMaxDamage
        {
            get
            {
                return 41;
            }
        }
        public override int OldSpeed
        {
            get
            {
                return 20;
            }
        }
        public override int DefMaxRange
        {
            get
            {
                return 10;
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
                return 60;
            }
        }
        public override WeaponAnimation DefAnimation
        {
            get
            {
                return WeaponAnimation.ShootBow;
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

            if (this.Weight == 7.0)
                this.Weight = 6.0;
        }
    }
}