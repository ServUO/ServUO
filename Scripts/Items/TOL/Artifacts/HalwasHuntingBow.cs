using System;

namespace Server.Items
{
    public class HalwasHuntingBow : BaseRanged
    {
        [Constructable]
        public HalwasHuntingBow()
            : base(0x10149)
        {
            WeaponAttributes.Velocity = 60;
            Attributes.AttackChance = 20;
            Attributes.WeaponSpeed = 45;
            Attributes.WeaponDamage = 60;
            WeaponAttributes.HitLeechMana = 20;
            //Slayer = SlayerName.Eodon; New Not Yet Implented
        }

        public HalwasHuntingBow(Serial serial)
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
                return WeaponAbility.ForceArrow;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.SerpentArrow;
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
                return 13;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 17;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 3.25f;
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
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override WeaponAnimation DefAnimation
        {
            get
            {
                return WeaponAnimation.ShootBow;
            }
        }


        public override int LabelNumber
        {
            get
            {
                return 1156127;
            }
        }// Halawa's Hunting Bow
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}