namespace Server.Items
{
    public class InspectedKegofTotalRefreshment : Item
    {
        [Constructable]
        public InspectedKegofTotalRefreshment()
            : base(0x1940)
        {
            Name = "Inspected Keg of Total Refreshment";

            Hue = 2418;
        }

        public InspectedKegofTotalRefreshment(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}