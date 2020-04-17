namespace Server.Items
{
    public class FrostguardTalisman : BaseTalisman
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1115516;  // Frostguard Talisman
        public override int ColdResistance => 3;

        [Constructable]
        public FrostguardTalisman()
            : base(0x2F5B)
        {
            Hue = 0x556;
            Weight = 1.0;
            SAAbsorptionAttributes.EaterCold = 5;
            Attributes.RegenMana = 1;
            Attributes.LowerManaCost = 5;
        }

        public FrostguardTalisman(Serial serial)
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