namespace Server.Items
{
    public class DecoIronIngots6 : Item
    {
        [Constructable]
        public DecoIronIngots6()
            : base(0x1BF4)
        {
            Movable = true;
            Stackable = false;
        }

        public DecoIronIngots6(Serial serial)
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