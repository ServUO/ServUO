namespace Server.Items
{
    public class PortcullisNS : BaseDoor
    {
        [Constructable]
        public PortcullisNS()
            : base(0x6F5, 0x6F5, 0xF0, 0xEF, new Point3D(0, 0, 20))
        {
        }

        public PortcullisNS(Serial serial)
            : base(serial)
        {
        }

        public override bool UseChainedFunctionality => true;
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

    public class PortcullisEW : BaseDoor
    {
        [Constructable]
        public PortcullisEW()
            : base(0x6F6, 0x6F6, 0xF0, 0xEF, new Point3D(0, 0, 20))
        {
        }

        public PortcullisEW(Serial serial)
            : base(serial)
        {
        }

        public override bool UseChainedFunctionality => true;
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