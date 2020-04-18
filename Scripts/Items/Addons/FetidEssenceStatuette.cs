namespace Server.Items
{
    public class FetidEssenceStatuette : ShimmeringEffusionStatuette
    {
        [Constructable]
        public FetidEssenceStatuette()
            : base(0x2D94)
        {
            Weight = 1.0;
        }

        public FetidEssenceStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074504;// Fetid Essence Statuette
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