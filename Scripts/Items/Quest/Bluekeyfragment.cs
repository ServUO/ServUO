namespace Server.Items
{
    public class Bluekeyfragment : Item
    {
        [Constructable]
        public Bluekeyfragment()
            : base(0x1012)
        {
            Movable = false;
            Hue = 0x5D;
        }

        public Bluekeyfragment(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1111646;
        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("You make a copy of the key in your pack");

            BlueKey1 bluekey = new BlueKey1();
            if (!from.AddToBackpack(bluekey))
                bluekey.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}