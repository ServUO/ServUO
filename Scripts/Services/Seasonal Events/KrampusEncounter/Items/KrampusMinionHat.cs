namespace Server.Items
{
    public class KrampusMinionHat : BaseHat
    {
        public override int LabelNumber => 1125639;  // krampus minion hat

        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;

        public override int InitMinHits => 20;
        public override int InitMaxHits => 30;

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
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
