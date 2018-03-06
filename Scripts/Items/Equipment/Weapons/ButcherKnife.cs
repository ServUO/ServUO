using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishButcherKnife))]
    [FlipableAttribute(0x13F6, 0x13F7)]
    public class ButcherKnife : BaseKnife
    {
        [Constructable]
        public ButcherKnife()
            : base(0x13F6)
        {
            this.Weight = 1.0;
        }

        public ButcherKnife(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.InfectiousStrike;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.Disarm;
            }
        }
        public override int AosStrengthReq
        {
            get
            {
                return 10;
            }
        }
        public override int AosMinDamage
        {
            get
            {
                return 10;
            }
        }
        public override int AosMaxDamage
        {
            get
            {
                return 13;
            }
        }
        public override int AosSpeed
        {
            get
            {
                return 49;
            }
        }
        public override float MlSpeed
        {
            get
            {
                return 2.25f;
            }
        }
        public override int OldStrengthReq
        {
            get
            {
                return 5;
            }
        }
        public override int OldMinDamage
        {
            get
            {
                return 2;
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
                return 40;
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
