namespace Server.Items
{
    public class DarkSource : Item
    {
        [Constructable]
        public DarkSource()
            : base(0x1646)
        {
            Layer = Layer.TwoHanded;
            Movable = false;
        }

        public DarkSource(Serial serial)
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