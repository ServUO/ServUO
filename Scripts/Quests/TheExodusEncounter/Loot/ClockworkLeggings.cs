using System;

namespace Server.Items
{
    public class ClockworkLeggings : PlateLegs
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public ClockworkLeggings()
        {
            this.Hue = 0xA91;
            this.Attributes.RegenStam = 5;
            this.Attributes.DefendChance = 25;
            this.Attributes.BonusDex = 5;
            this.StrRequirement = 90;
            this.Weight = 7;
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

        public override bool CanFortify { get { return false; } }

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