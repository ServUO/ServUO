namespace Server.Items
{
    public class Beads : Item
    {
        [Constructable]
        public Beads()
            : base(0x108B)
        {
            Weight = 1.0;
        }

        public Beads(Serial serial)
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