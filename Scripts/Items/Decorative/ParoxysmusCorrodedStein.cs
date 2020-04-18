namespace Server.Items
{
    public class ParoxysmusCorrodedStein : Item
    {
        [Constructable]
        public ParoxysmusCorrodedStein()
            : base(0x9D6)
        {
        }

        public ParoxysmusCorrodedStein(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1072083;// Paroxysmus' Corroded Stein
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