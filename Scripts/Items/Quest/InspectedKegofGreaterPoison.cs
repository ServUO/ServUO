namespace Server.Items
{
    public class InspectedKegofGreaterPoison : Item
    {
        [Constructable]
        public InspectedKegofGreaterPoison()
            : base(0x1940)
        {
            Name = "Inspected Keg of Greater Poison";

            Hue = 2425;
        }

        public InspectedKegofGreaterPoison(Serial serial)
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