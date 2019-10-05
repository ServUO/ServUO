using System;

namespace Server.Items
{
    public class MagesHood : BaseHat
    {
        public override int LabelNumber { get { return 1159227; } } // mage's hood

        public override int BasePhysicalResistance { get { return 0; } }
        public override int BaseFireResistance { get { return 3; } }
        public override int BaseColdResistance { get { return 5; } }
        public override int BasePoisonResistance { get { return 8; } }
        public override int BaseEnergyResistance { get { return 8; } }

        public override int InitMinHits { get { return 20; } }
        public override int InitMaxHits { get { return 40; } }

        [Constructable]
        public MagesHood()
            : this(0)
        {
        }

        [Constructable]
        public MagesHood(int hue)
            : base(0xA411, hue)
        {
            Weight = 3.0;
            StrRequirement = 10;
        }

        public MagesHood(Serial serial)
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
