namespace Server.Items
{
    public class ColorFixative : Item
    {
        [Constructable]
        public ColorFixative()
            : base(0x182D)
        {
            Weight = 1.0;
            Hue = 473;  // ...make this the proper shade of green
        }

        public ColorFixative(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1112135;// color fixative
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