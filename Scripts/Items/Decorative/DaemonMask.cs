namespace Server.Items.Holiday
{
    public class PaintedDaemonMask : BasePaintedMask
    {
        [Constructable]
        public PaintedDaemonMask()
            : base(0x4a92)
        {
        }

        public PaintedDaemonMask(Serial serial)
            : base(serial)
        {
        }

        public override string MaskName => "Daemon Mask";
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