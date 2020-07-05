namespace Server.Items
{
    public class EcruCitrineRing : GoldRing
    {
        public override int LabelNumber => 1073457; // ecru citrine ring

        [Constructable]
        public EcruCitrineRing()
            : base()
        {
            Weight = 1.0;

            if (.75 > Utility.RandomDouble())
                Attributes.EnhancePotions = 50;
            else
                Attributes.BonusStr = Utility.RandomMinMax(5, 6);
        }

        public EcruCitrineRing(Serial serial)
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
