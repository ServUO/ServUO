namespace Server.Items
{
    public class GargishClockworkLeggings : GargishPlateLegs
    {
        public override bool IsArtifact => true;

        [Constructable]
        public GargishClockworkLeggings()
        {
            Hue = 0xA91;
            Attributes.RegenStam = 5;
            Attributes.DefendChance = 25;
            Attributes.BonusDex = 5;
            StrRequirement = 90;
        }

        public GargishClockworkLeggings(Serial serial) : base(serial)
        {
        }

        public override int LabelNumber => 1153536;

        public override int BasePhysicalResistance => 8;
        public override int BaseFireResistance => 6;
        public override int BaseColdResistance => 5;
        public override int BasePoisonResistance => 6;
        public override int BaseEnergyResistance => 5;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

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
