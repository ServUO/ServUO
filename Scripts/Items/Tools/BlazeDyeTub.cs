namespace Server.Items
{
    public class BlazeDyeTub : DyeTub
    {
        [Constructable]
        public BlazeDyeTub()
        {
            Hue = DyedHue = 0x489;
            Redyable = false;
        }

        public BlazeDyeTub(Serial serial)
            : base(serial)
        {
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