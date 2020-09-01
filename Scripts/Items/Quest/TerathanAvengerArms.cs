namespace Server.Items
{
    public class TerathanAvengerArms : Item
    {
        [Constructable]
        public TerathanAvengerArms()
            : base(0x1B9D)
        {
        }

        public TerathanAvengerArms(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1150044;// Terathan Avenger arms
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            reader.ReadInt();
        }
    }
}
