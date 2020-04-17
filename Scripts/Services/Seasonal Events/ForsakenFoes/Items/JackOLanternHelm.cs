namespace Server.Items
{
    public class JackOLanternHelm : BaseArmor
    {
        public override int LabelNumber => 1125986;  // jack o' lantern helm
        public override bool IsArtifact => true;

        [Constructable]
        public JackOLanternHelm()
            : base(0xA3EA)
        {
            Weight = 3.0;
            Layer = Layer.Helm;
            Light = LightType.Circle300;
        }

        public override int BasePhysicalResistance => 12;
        public override int BaseFireResistance => 14;
        public override int BaseColdResistance => 4;
        public override int BasePoisonResistance => 8;
        public override int BaseEnergyResistance => 10;

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override int StrReq => 10;

        public override ArmorMaterialType MaterialType => ArmorMaterialType.Plate;

        public JackOLanternHelm(Serial serial)
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
