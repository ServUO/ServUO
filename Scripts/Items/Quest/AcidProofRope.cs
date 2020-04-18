namespace Server.Items
{
    public class AcidProofRope : Item
    {
        [Constructable]
        public AcidProofRope()
            : base(0x20D)
        {
            Hue = 0x3D1; // TODO check
        }

        public AcidProofRope(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074886;// Acid Proof Rope
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