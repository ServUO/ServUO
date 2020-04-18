namespace Server.Items
{
    public class ToolKit : Item
    {
        [Constructable]
        public ToolKit()
            : base(Utility.Random(2) + 0x1EBA)
        {
            Movable = true;
            Stackable = false;
        }

        public ToolKit(Serial serial)
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