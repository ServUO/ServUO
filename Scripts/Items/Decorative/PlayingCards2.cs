namespace Server.Items
{
    public class PlayingCards2 : Item
    {
        [Constructable]
        public PlayingCards2()
            : base(0xFA2)
        {
            Movable = true;
            Stackable = false;
        }

        public PlayingCards2(Serial serial)
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