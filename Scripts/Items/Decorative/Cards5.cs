namespace Server.Items
{
    public class DecoCards5 : Item
    {
        [Constructable]
        public DecoCards5()
            : base(0xE18)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoCards5(Serial serial)
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