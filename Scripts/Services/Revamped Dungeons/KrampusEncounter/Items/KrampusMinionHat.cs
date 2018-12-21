using System;

namespace Server.Items
{
    public class KrampusMinionHat : BaseHat
    {
        public override int LabelNumber { get { return 1125639; } } // krampus minion hat

        public override int BasePhysicalResistance { get { return 0; } }
        public override int BaseFireResistance { get { return 5; } }
        public override int BaseColdResistance { get { return 9; } }
        public override int BasePoisonResistance { get { return 5; } }
        public override int BaseEnergyResistance { get { return 5; } }

        public override int InitMinHits { get { return 20; } }
        public override int InitMaxHits { get { return 30; } }

        [Constructable]
        public KrampusMinionHat()
            : this(0)
        {
        }

        [Constructable]
        public KrampusMinionHat(int hue)
            : base(0xA28F, hue)
        {
            Weight = 3.0;
        }

        public KrampusMinionHat(Serial serial)
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
