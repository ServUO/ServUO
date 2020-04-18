namespace Server.Items
{
    public class GlobOfMonstreousInterredGrizzle : Item
    {
        [Constructable]
        public GlobOfMonstreousInterredGrizzle()
            : base(0x2F3)
        {
        }

        public GlobOfMonstreousInterredGrizzle(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1072117;// Glob of Monsterous Interred Grizzle
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