namespace Server.Items
{
    public class LightweightShortbow : MagicalShortbow
    {
        public override int LabelNumber => 1073510; // lightweight shortbow

        [Constructable]
        public LightweightShortbow()
        {
            Balanced = true;
        }

        public LightweightShortbow(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}