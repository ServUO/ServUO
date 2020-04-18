namespace Server.Items
{
    public class EmptyToolKit2 : Item
    {
        [Constructable]
        public EmptyToolKit2()
            : base(0x1EB7)
        {
            Movable = true;
            Stackable = false;
        }

        public EmptyToolKit2(Serial serial)
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