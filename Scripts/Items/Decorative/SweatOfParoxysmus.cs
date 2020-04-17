namespace Server.Items
{
    public class SweatOfParoxysmus : Item
    {
        [Constructable]
        public SweatOfParoxysmus()
            : base(0xF01)
        {
        }

        public SweatOfParoxysmus(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1072081;// Sweat of Paroxysmus
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