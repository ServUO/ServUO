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
            Weight = 8.0;
            Layer = Layer.TwoHanded;
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