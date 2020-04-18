namespace Server.Items
{
    public class AutomatonActuator : Item
    {
        public override int LabelNumber => 1156997;  // Automaton Actuator

        [Constructable]
        public AutomatonActuator()
            : base(0x9CE9)
        {
        }

        public AutomatonActuator(Serial serial)
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
