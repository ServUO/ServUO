using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefCarpentry), typeof(GargishGnarledStaff))]
    [FlipableAttribute(0x13F8, 0x13F9)]
    public class GnarledStaff : BaseStaff
    {
        [Constructable]
        public GnarledStaff()
            : base(0x13F8)
        {
            Weight = 3.0;
        }

        public GnarledStaff(Serial serial)
            : base(serial)
        {
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
                return WeaponAbility.ForceOfNature;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 20;
            }
        }
        public override int MinDamage
        {
            get
            {
                return 15;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 18;
            }
        }
        public override float Speed
        {
            get
            {
                return 3.25f;
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
                return 50;
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
