namespace Server.Items
{
    public class HoveringWisp : Item
    {
        [Constructable]
        public HoveringWisp()
            : base(0x2100)
        {
        }

        public HoveringWisp(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1072881;// hovering wisp
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