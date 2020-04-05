using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishPike))]
    [FlipableAttribute(0x26BE, 0x26C8)]
    public class Pike : BaseSpear
    {
        [Constructable]
        public Pike()
            : base(0x26BE)
        {
            Weight = 8.0;
        }

        public Pike(Serial serial)
            : base(serial)
        {
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
                return WeaponAbility.InfectiousStrike;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 50;
            }
        }
        public override int MinDamage
        {
            get
            {
                return 14;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 17;
            }
        }
        public override float Speed
        {
            get
            {
                return 3.00f;
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
                return 110;
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
