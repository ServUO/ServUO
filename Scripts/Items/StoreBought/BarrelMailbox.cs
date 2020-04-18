namespace Server.Items
{
    [Furniture]
    public class BarrelMailbox : Mailbox, IFlipable
    {
        public override int LabelNumber => 1113927;  // Mailbox
        public override int DefaultGumpID => 0x6D5;

        public override int SouthMailBoxID => 0xA1F8;
        public override int SouthEmptyMailBoxID => 0xA1F9;
        public override int EastMailBoxID => 0xA1F5;
        public override int EastEmptyMailBoxID => 0xA1F7;

        [Constructable]
        public BarrelMailbox()
            : base(0xA1F7)
        {
        }

        public BarrelMailbox(Serial serial)
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
