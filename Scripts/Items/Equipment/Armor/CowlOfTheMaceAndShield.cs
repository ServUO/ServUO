using System;

namespace Server.Items
{
    public class CowlOfTheMaceAndShield : AssassinsCowl
    {
        public override int LabelNumber { get { return 1159228; } } // cowl of the mace & shield

        public override int BasePhysicalResistance { get { return 10; } }
        public override int BaseFireResistance { get { return 10; } }
        public override int BaseColdResistance { get { return 10; } }
        public override int BasePoisonResistance { get { return 10; } }
        public override int BaseEnergyResistance { get { return 10; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public CowlOfTheMaceAndShield()
            : this(0)
        {
        }

        [Constructable]
        public CowlOfTheMaceAndShield(int hue)
            : base(hue)
        {
            WeaponAttributes.HitLowerDefend = 30;
            Attributes.BonusStr = 10;
            Attributes.BonusDex = 5;
        }

        public CowlOfTheMaceAndShield(Serial serial)
            : base(serial)
        {
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
