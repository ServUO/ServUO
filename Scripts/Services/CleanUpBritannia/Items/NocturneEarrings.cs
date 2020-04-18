namespace Server.Items
{
    public class NocturneEarrings : SilverEarrings
    {
        public override int LabelNumber => 1080189;  // Nocturne Earrings

        [Constructable]
        public NocturneEarrings()
        {
            Hue = 0x3E5;
            Attributes.NightSight = 1;
        }

        public NocturneEarrings(Serial serial)
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