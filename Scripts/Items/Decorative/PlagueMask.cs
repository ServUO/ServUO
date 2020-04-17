namespace Server.Items.Holiday
{
    public class PaintedPlagueMask : BasePaintedMask
    {
        [Constructable]
        public PaintedPlagueMask()
            : base(0x4A8E)
        {
        }

        public PaintedPlagueMask(Serial serial)
            : base(serial)
        {
        }

        public override string MaskName => "Plague Mask";
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