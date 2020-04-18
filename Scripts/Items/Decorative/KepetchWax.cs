namespace Server.Items
{
    public class KepetchWax : Item
    {
        [Constructable]
        public KepetchWax()
            : base(0x5745)
        {
        }

        public KepetchWax(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1112412;// kepetch wax
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