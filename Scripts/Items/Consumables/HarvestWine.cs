namespace Server.Items
{
    public class HarvestWine : BeverageBottle
    {
        public override double DefaultWeight => 1;
        public override int LabelNumber => 1153873;  // Harvest Wine

        [Constructable]
        public HarvestWine()
            : base(BeverageType.Wine)
        {
            Hue = 0xe0;
        }

        public HarvestWine(Serial serial)
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