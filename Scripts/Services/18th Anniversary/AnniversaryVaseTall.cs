namespace Server.Items
{
    public class AnniversaryVaseTall : Item
    {
        public override int LabelNumber => 1156147;  // Tall 18th Anniversary Vase

        [Constructable]
        public AnniversaryVaseTall() : base(0x9BC7)
        {
        }

        public AnniversaryVaseTall(Serial serial) : base(serial)
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