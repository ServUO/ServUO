namespace Server.Items
{
    public class DecoWyrmsHeart : Item
    {
        [Constructable]
        public DecoWyrmsHeart()
            : base(0xF91)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoWyrmsHeart(Serial serial)
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