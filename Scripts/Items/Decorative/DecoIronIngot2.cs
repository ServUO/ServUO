namespace Server.Items
{
    public class DecoIronIngot2 : Item
    {
        [Constructable]
        public DecoIronIngot2()
            : base(0x1BEF)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoIronIngot2(Serial serial)
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