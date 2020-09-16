namespace Server.Items
{
    [Flipable(0x9DB1, 0x9DB2)]
    public class InoperativeAutomatonHead : Item
    {
        public override int LabelNumber => 1157002;  // Inoperative Automaton Head

        [Constructable]
        public InoperativeAutomatonHead()
            : base(0x9DB1)
        {
        }

        public InoperativeAutomatonHead(Serial serial)
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
