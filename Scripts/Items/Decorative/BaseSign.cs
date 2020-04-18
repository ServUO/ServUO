namespace Server.Items
{
    public abstract class BaseSign : Item
    {
        public BaseSign(int dispID)
            : base(dispID)
        {
            Movable = false;
        }

        public BaseSign(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}