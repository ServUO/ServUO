using System;

namespace Server.Items
{
    public class MoctapotlsObsidianSword : BaseSword
    {
        [Constructable]
        public MoctapotlsObsidianSword()
            : base(9934)
        {
            Weight = 6;
            Hue = 1910;
            Attributes.WeaponSpeed = 40;
            Attributes.WeaponDamage = 75;
            AbsorptionAttributes.SplinteringWeapon = 20;
            WeaponAttributes.HitLeechStam = 100;
            WeaponAttributes.HitPhysicalArea = 50;
            WeaponAttributes.HitHarm = 50;
        }

        public MoctapotlsObsidianSword(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.CrushingBlow;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.ConcussionBlow;
            }
        }
        public override int AosStrengthReq
        {
            get
            {
                return 85;
            }
        }
        public override int AosMinDamage
        {
            get
            {
                return 20;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 24;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 5.00f;
            }
        }
        public override int DefHitSound
        {
            get
            {
                return 0x237;
            }
        }
        public override int DefMissSound
        {
            get
            {
                return 0x23A;
            }
        }

        public override int LabelNumber
        {
            get
            {
                return 1156131;
            }
        }// Moctapotl's Obsidian Sword

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