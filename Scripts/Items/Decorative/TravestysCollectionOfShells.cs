namespace Server.Items
{
    public class TravestysCollectionOfShells : Item
    {
        [Constructable]
        public TravestysCollectionOfShells()
            : base(0xFD3)
        {
        }

        public TravestysCollectionOfShells(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1072090;// Travesty's Collection of Shells
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