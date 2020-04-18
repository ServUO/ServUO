namespace Server.Items
{
    public class DecoIronIngot : Item
    {
        [Constructable]
        public DecoIronIngot()
            : base(0x1BEF)
        {
            Movable = true;
            Stackable = true;
        }

        public DecoIronIngot(Serial serial)
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