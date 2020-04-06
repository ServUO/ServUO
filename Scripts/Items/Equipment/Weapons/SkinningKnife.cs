using System;

namespace Server.Items
{
    [FlipableAttribute(0xEC4, 0xEC5)]
    public class SkinningKnife : BaseKnife
    {
        [Constructable]
        public SkinningKnife()
            : base(0xEC4)
        {
            Weight = 1.0;
        }

        public SkinningKnife(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.ShadowStrike;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.BleedAttack;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 5;
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
                return 13;
            }
        }
        public override float Speed
        {
            get
            {
                return 2.25f;
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
                return 40;
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
