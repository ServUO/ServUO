namespace Server.Items.Holiday
{
    public class PaintedEvilJesterMask : BasePaintedMask
    {
        [Constructable]
        public PaintedEvilJesterMask()
            : base(0x4BA5)
        {
        }

        public PaintedEvilJesterMask(Serial serial)
            : base(serial)
        {
        }

        public override string MaskName => "Evil Jester Mask";
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