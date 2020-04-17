namespace Server.Items
{
    public class MeltedWax : Item
    {
        [Constructable]
        public MeltedWax()
            : base(0x122A)
        {
            Movable = false;
            Hue = 0x835;
        }

        public MeltedWax(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1016492;// melted wax
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