namespace Server.Items
{
    public class MagesHood : BaseHat
    {
        public override int LabelNumber => 1159227;  // mage's hood

        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 3;
        public override int BaseColdResistance => 5;
        public override int BasePoisonResistance => 8;
        public override int BaseEnergyResistance => 8;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 40;

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
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
