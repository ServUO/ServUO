using System;

namespace Server.Items
{
    public class ClockworkLeggings : PlateLegs
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public ClockworkLeggings()
        {
            Hue = 0xA91;
            Attributes.RegenStam = 5;
            Attributes.DefendChance = 25;
            Attributes.BonusDex = 5;
        }

        public ClockworkLeggings(Serial serial)  : base(serial)
        {
        }

        public override int LabelNumber { get { return 1153536; } }

        public override int BasePhysicalResistance { get { return 8; } }
        public override int BaseFireResistance { get { return 6; } }
        public override int BaseColdResistance { get { return 5; } }
        public override int BasePoisonResistance { get { return 6; } }
        public override int BaseEnergyResistance { get { return 5; } }
        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}
