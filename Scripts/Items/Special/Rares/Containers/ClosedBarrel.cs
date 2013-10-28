namespace Server.Items
{
    class ClosedBarrel : TrapableContainer
    { 
        [Constructable]
        public ClosedBarrel()
            : base(0x0FAE)
        {
        }

        public ClosedBarrel(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultGumpID
        {
            get
            {
                return 0x3e;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}