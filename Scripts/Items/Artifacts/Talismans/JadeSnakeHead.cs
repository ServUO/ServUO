namespace Server.Items
{
    public class JadeSnakeHead : BaseTalisman
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1115647;  // Jade Snake Head
        public override int PoisonResistance => 3;

        [Constructable]
        public JadeSnakeHead()
            : base(0x2F59)
        {
            Hue = 0x48A;
            Weight = 1.0;
            SAAbsorptionAttributes.EaterPoison = 5;
            Attributes.RegenStam = 2;
            Attributes.LowerManaCost = 5;
        }

        public JadeSnakeHead(Serial serial)
            : base(serial)
        {
        }

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