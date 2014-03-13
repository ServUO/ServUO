using System;

namespace Server.Items
{
    [FlipableAttribute(0x27A6, 0x27F1)]
    public class Tetsubo : BaseBashing
    {
        [Constructable]
        public Tetsubo()
            : base(0x27A6)
        {
            this.Weight = 8.0;
            this.Layer = Layer.TwoHanded;
        }

        public Tetsubo(Serial serial)
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
                return WeaponAbility.CrushingBlow;
            }
        }
        public override int AosStrengthReq
        {
            get
            {
                return 35;
            }
        }
        public override int AosMinDamage
        {
            get
            {
                return 12;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 15;
            }
        }
        public override int AosSpeed
        {
            get
            {
                return 45;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 2.50f;
            }
        }
        public override int OldStrengthReq
        {
            get
            {
                return 35;
            }
        }
        public override int OldMinDamage
        {
            get
            {
                return 12;
            }
        }
        public override int OldMaxDamage
        {
            get
            {
                return 14;
            }
        }
        public override int OldSpeed
        {
            get
            {
                return 45;
            }
        }
        public override int DefHitSound
        {
            get
            {
                return 0x233;
            }
        }
        public override int DefMissSound
        {
            get
            {
                return 0x238;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 60;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 65;
            }
        }
        public override WeaponAnimation DefAnimation
        {
            get
            {
                return WeaponAnimation.Bash2H;
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